$(function () {
    // -- signalr *Fariz Prasetyo* -- //
    //build singalr connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/progressHub", { transport: signalR.HttpTransportType.LongPolling })
       // .configureLogging(signalR.LogLevel.Debug) // add this to get more detail log in console
        .build();
    //starting the signalr connection
    connection.start()
        .then(() => {
            console.log("SignalR Connected - Fariz Prasetyo");
        })
        .catch(err => console.error("SignalR Connection Error: ", err));
    //update uploader queue
    connection.on("QueueUpdate", (queuePosition) => {
        if (queuePosition > 0) {
            $('#queueStatus').text(`Your upload is queued. There are ${queuePosition} upload(s) ahead of you. Please wait`);
        } else if (queuePosition === 0) {
            $('#queueStatus').text('Your upload is next in line. Please wait.');
        } else if (queuePosition === -1) {
            $('#queueStatus').text('Your upload is being processed.');
        }
    });
    //uploader progress bar
    connection.on("UpdateUploaderProgress", (currentRow, totalRows) => {
        if (currentRow === "error") {
            $('#progressBar').css('width', '0%').attr('aria-valuenow', 0);
            $('#progressBar').text('0%');
            $('#progressBar').removeClass('bg-success').addClass('bg-danger');
            $('#uploadStatus').html('<span class="text-danger">Error occurred: ' + totalRows + '</span>');
        } else if (currentRow === "complete") {
            $('#progressBar').css('width', '100%').attr('aria-valuenow', 100);
            $('#progressBar').text('100%');
            $('#progressBar').removeClass('bg-danger').addClass('bg-success');
            $('#uploadStatus').html('<span class="text-success">Upload completed successfully!</span>');

            setTimeout(() => {
                $('#progressBar').css('width', '0%').attr('aria-valuenow', 0);
                $('#progressBar').text('0%');
                $('#uploadStatus').html('');
            }, 2000);
        } else {
            const percentage = Math.round((currentRow / totalRows) * 100);
            $('#progressBar').css('width', percentage + '%').attr('aria-valuenow', percentage);
            $('#progressBar').text(percentage + '%');
            $('#progressBar').removeClass('bg-danger').addClass('bg-success');
            $('#uploadStatus').html('Uploading...');
        }
    });
    //downloader progress bar
    connection.on("ReceiveDownloadProgress", (current, total) => {
        if (current === "error") {
            $('#downloadProgressBar').css('width', '0%').attr('aria-valuenow', 0);
            $('#downloadProgressBar').text('0%');
            $('#downloadProgressBar').removeClass('bg-success').addClass('bg-danger');
            $('#downloadStatus').html('<span class="text-danger">Error occurred: ' + total + '</span>');
        } else if (current === "complete") {
            $('#downloadProgressBar').css('width', '100%').attr('aria-valuenow', 100);
            $('#downloadProgressBar').text('100%');
            $('#downloadProgressBar').removeClass('bg-danger').addClass('bg-success');
            $('#downloadStatus').html('<span class="text-success">Download completed successfully!</span>');
            setTimeout(() => {
                $('#modal-download-selected').modal('hide');
                $('#downloadProgressBar').css('width', '0%').attr('aria-valuenow', 0);
                $('#downloadProgressBar').text('0%');
                $('#downloadStatus').html('');
                $('#downloadQueueStatus').text('');
            }, 2000);
        } else {
            const percentage = Math.round((current / total) * 100);
            $('#downloadProgressBar').css('width', percentage + '%').attr('aria-valuenow', percentage);
            $('#downloadProgressBar').text(percentage + '%');
            $('#downloadProgressBar').removeClass('bg-danger').addClass('bg-success');
            $('#downloadStatus').html('Downloading...');
        }
    });
    // function for grouping signalr connection
    function generateUniqueId() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0,
                v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
    // -- end of signalr *Fariz Prasetyo* -- //

    var token = $.cookie("Token");
    var areaName = "Mining";
    var entityName = "Hauling";
    var url = "/api/" + areaName + "/" + entityName;    
    var selectedIds = null;
    var tareValue = 0;
    var grossValue = 0;

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 100,
        "timeOut": 3000,
        "extendedTimeOut": 1000,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    $("#AccountingPeriod").select2({
        ajax:
        {
            url: "/api/Accounting/AccountingPeriod/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0,
        width: '100%',
        dropdownParent: $("#modal-accounting-period")
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#accounting_period_id').val(data.id);
    }).on('select2:clear', function (e) {
        $('#accounting_period_id').val('');
    });

    $("#QualitySampling").select2({
        ajax:
        {
            url: "/api/StockpileManagement/QualitySampling/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0,
        width: '100%',
        dropdownParent: $("#modal-quality-sampling")
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#quality_sampling_id').val(data.id);
    }).on('select2:clear', function (e) {
        $('#quality_sampling_id').val('');
    });

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("haulingDate1");
    var tgl2 = sessionStorage.getItem("haulingDate2");

    var date = new Date(), y = date.getFullYear(), m = date.getMonth(), d = date.getDate();
    // Set the hours, minutes, and seconds for the first day
    var firstDay = new Date(y, m, d - 1, 7, 0, 0);

    // Set the hours, minutes, and seconds for the last day
    var lastDay = new Date(y, m, d, 6, 59, 59);

    if (tgl1 != null)
        firstDay = Date.parse(tgl1);

    if (tgl2 != null)
        lastDay = Date.parse(tgl2);

    $("#date-box1").dxDateBox({
        type: "datetime",
        displayFormat: 'dd MMM yyyy HH:mm',
        value: firstDay,
        onValueChanged: function (data) {
            firstDay = new Date(data.value);
            sessionStorage.setItem("haulingDate1", formatTanggal(firstDay));
            _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                + "/" + encodeURIComponent(formatTanggal(lastDay));
        }
    });

    $("#date-box2").dxDateBox({
        type: "datetime",
        displayFormat: 'dd MMM yyyy HH:mm',
        value: lastDay,
        onValueChanged: function (data) {
            lastDay = new Date(data.value);
            sessionStorage.setItem("haulingDate2", formatTanggal(lastDay));
            _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                + "/" + encodeURIComponent(formatTanggal(lastDay));
        }
    });

    $('#btnView').on('click', function () {
        location.reload();
    })

    var _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
        + "/" + encodeURIComponent(formatTanggal(lastDay));

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            //loadUrl: url + "/DataGrid",
            loadUrl: _loadUrl,
            insertUrl: url + "/InsertData",
            updateUrl: url + "/UpdateData",
            deleteUrl: url + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function(request){
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };                
            }
        }),
        selection: {
            mode: "multiple"
        },
        remoteOperations: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "integration_status",
                dataType: "string",
                allowEditing: false,
                placeholder: "NOT APPROVED",
                caption: "Integration Status",
                formItem: {
                    colSpan: 2,
                    editorType: "dxTextArea"
                },
            },
            {
                dataField: "transaction_number",
                dataType: "string",
                caption: "Transaction Number",
                allowEditing: false,
                visible: false,
                sortOrder: "asc",
                formItem: {
                    colSpan: 2
                },
            },
            {
                dataField: "loading_datetime",
                dataType: "datetime",
                caption: "DateTime In",
                validationRules: [{
                    type: "required",
                    message: "The DateTime In field is required."
                }],
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "unloading_datetime",
                dataType: "datetime",
                caption: "DateTime Out",
                visible: true,
                validationRules: [{
                    type: "required",
                    message: "The DateTime Out field is required."
                }],
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "source_shift_id",
                dataType: "text",
                caption: "Shift",
                validationRules: [{
                    type: "required",
                    message: "The Shift field is required."
                }],
                formItem: {
                    colSpan: 2,
                    editorOptions: {
                        showClearButton: true
                    }
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Shift/Shift/ShiftIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                }
            },

            {
                dataField: "process_flow_id",
                dataType: "text",
                caption: "Process Flow",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "The Process Flow field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/ProcessFlowIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                setCellValue: function (rowData, value) {
                    rowData.process_flow_id = value;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "transport_id",
                dataType: "text",
                caption: "Transport",
                validationRules: [{
                    type: "required",
                    message: "The Transport field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Transport/Truck/TransportIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "source_location_id",
                dataType: "text",
                caption: "Source",
                validationRules: [{
                    type: "required",
                    message: "The Source field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var _url = "/api/Mining/Hauling/SourceLocationIdLookup";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.process_flow_id !== undefined
                                    && options.data.process_flow_id !== null) {
                                    _url += "?ProcessFlowId=" + encodeURIComponent(options.data.process_flow_id);
                                }
                            }
                        }

                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: _url,
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                setCellValue: function (rowData, value) {
                    rowData.source_location_id = value;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "destination_location_id",
                dataType: "text",
                caption: "Destination",
                validationRules: [{
                    type: "required",
                    message: "The Destination field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var _url = "/api/Mining/Production/DestinationLocationIdLookup";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.process_flow_id !== undefined
                                    && options.data.process_flow_id !== null) {
                                    _url += "?ProcessFlowId=" + encodeURIComponent(options.data.process_flow_id);
                                }
                            }
                        }

                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: _url,
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                hidingPriority: 0
            },
            {
                dataField: "gross",
                dataType: "number",
                caption: "Gross (MT)",
                validationRules: [{
                    type: "required",
                    message: "The Gross field is required."
                }],
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 3
                        }
                    }
                }
            },
            {
                dataField: "tare",
                dataType: "number",
                caption: "Tare (MT)",
                visible: false,
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 3
                        }
                    }
                },
                allowEditing: true,
            },
            {
                dataField: "loading_quantity",
                dataType: "number",
                caption: "Loading Quantity (MT)",
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 3
                        }
                    }
                },
                validationRules: [{
                    type: "required",
                    message: "The Loading Quantity field is required."
                }]
            },
            {
                dataField: "product_id",
                dataType: "text",
                caption: "Product",
                validationRules: [{
                    type: "required",
                    message: "The Product field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var _url = "/api/Material/Product/ProductIdLookupB";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.process_flow_id !== undefined
                                    && options.data.process_flow_id !== null) {
                                    _url += "?ProcessFlowId=" + encodeURIComponent(options.data.process_flow_id);
                                }
                            }
                        }

                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: _url,
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                setCellValue: function (rowData, value) {
                    rowData.product_id = value;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "equipment_id",
                dataType: "text",
                caption: "Equipment",
                visible: false,
                lookup: {
                     dataSource: function (options) {
                        var _url = "/api/Mining/Production/EquipmentIdLookup";

                            if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.contractor_id !== undefined
                                    && options.data.contractor_id !== null) {
                                    _url += "?contractorId=" + encodeURIComponent(options.data.contractor_id);
                                }
                            }
                        }
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: _url,
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "contractor_id",
                dataType: "text",
                caption: "Contractor",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Organisation/Contractor/ContractorIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "distance",
                dataType: "number",
                caption: "Distance (meter)",
                visible: false,
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 2
                        }
                    }
                },
            },
            {
                dataField: "quality_sampling_id",
                dataType: "text",
                caption: "Quality Sampling",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/QualitySamplingIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "despatch_order_id",
                dataType: "text",
                caption: "Shipping Order",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/DespatchOrderIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "advance_contract_id",
                dataType: "text",
                caption: "Contract Reference",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/ContractRefIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "pic",
                dataType: "text",
                caption: "P I C",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/Employee/EmployeeIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "netto_rekon",
                dataType: "number",
                caption: "Netto Rekon",
                visible: false,
                allowEditing : false,
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 2
                        }
                    }
                },
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/SystemAdministration/BusinessUnit/BusinessUnitIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                }
            },
            {
                dataField: "note",
                dataType: "string",
                caption: "Note",
                visible: false,
                formItem: {
                    colSpan: 2,
                    editorType: "dxTextArea"
                }
            }
        ],
        summary: {
            totalItems: [
                {
                    column: 'gross',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'tare',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'loading_quantity',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                }
            ]
        },
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: true
        },
        groupPanel: {
            visible: true
        },
        searchPanel: {
            visible: true,
            width: "240px",
            placeholder: "Search..."
        },
        filterPanel: {
            visible: true
        },
        filterBuilderPopup: {
            position: { of: window, at: "top", my: "top", offset: { y: 10 } },
        },
        columnChooser: {
            enabled: true,
            mode: "select"
        },
        paging: {
            pageSize: 10
        },
        pager: {
            allowedPageSizes: [10, 20, 50, 100],
            showNavigationButtons: true,
            showPageSizeSelector: true,
            showInfo: true,
            visible: true
        },
        showBorders: true,        
        editing: {
            mode: "popup",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },
        grouping: {
            contextMenuEnabled: true,
            autoExpandAll: false
        },
        rowAlternationEnabled: true,        
        export: {
            enabled: true,
            allowExportSelectedData: true
        },
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0) {
                selectedIds = $.map(data, function (value) {
                    return value.id;
                }).join(",");
                $("#dropdown-item-accounting-period").removeClass("disabled");
                $("#dropdown-item-quality-sampling").removeClass("disabled");
                $("#dropdown-delete-selected").removeClass("disabled");
                $("#dropdown-approve-selected").removeClass("disabled");
                $("#dropdown-download-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-item-accounting-period").addClass("disabled");
                $("#dropdown-item-quality-sampling").addClass("disabled");
                $("#dropdown-delete-selected").addClass("disabled");
                $("#dropdown-approve-selected").addClass("disabled");
                $("#dropdown-download-selected").addClass("disabled");
            }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var $links = e.cellElement.find(".dx-link");
                if (e.row.data.integration_status != "NOT APPROVED" && e.row.data.integration_status != "REQUESTED FOR APPROVAL")
                    $links.filter(".dx-link-edit").remove();

            }
        },
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            if (e.parentType === "dataRow") {
                e.editorOptions.disabled = e.row.data && e.row.data.accounting_period_is_closed;
            }
            if (e.parentType == "dataRow" && e.dataField === "transport_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                let rowData = e.row.data;

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let transportId = e.value;

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Transport/Truck/DataTruck?Id=' + transportId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data;

                            grid.cellValue(index, "tare", record.tare);
                            grid.cellValue(index, "contractor_id", record.vendor_id);
                            tareValue = record.tare;
                        }
                    })
                    standardHandler(e); // Calling the standard handler to save the edited value
                }
            }
            if (e.parentType == "dataRow" && e.dataField == "gross") {

                let index = e.row.rowIndex;
                let grid = e.component;

                let rowData = e.row.data;
                grossValue = rowData.gross;

                let standardHandler = e.editorOptions.onValueChanged;

                e.editorOptions.onValueChanged = function (e) {
                    grossValue = e.value;

                    grid.beginUpdate();
                    grid.cellValue(index, "gross", grossValue);
                    grid.cellValue(index, "loading_quantity", grossValue - tareValue);
                    //grid.cellValue(index, "unloading_quantity", grossValue - tareValue);
                    grid.endUpdate();

                    standardHandler(e);
                }
            }
            if (e.dataField === "contractor_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    grid.beginCustomLoading()
                    // Get its value (Id) on value changed

                    grid.beginUpdate()
                    grid.cellValue(index, "contractor_id", e.value);
                    //grid.cellValue(index, "equipment_id", "");
                    grid.endUpdate()

                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500)

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
            if (e.parentType == "dataRow" && e.dataField == "tare") {

                let index = e.row.rowIndex;
                let grid = e.component;

                let rowData = e.row.data;
                tareValue = rowData.tare;

                let standardHandler = e.editorOptions.onValueChanged;

                e.editorOptions.onValueChanged = function (e) {
                    tareValue = e.value;

                    grid.beginUpdate();
                    grid.cellValue(index, "tare", tareValue);
                    grid.cellValue(index, "loading_quantity", grossValue - tareValue);
                    //grid.cellValue(index, "unloading_quantity", grossValue - tareValue);
                    grid.endUpdate();

                    standardHandler(e);
                }
            }
        },
        onExporting: function (e) {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet(entityName);

            DevExpress.excelExporter.exportDataGrid({
                component: e.component,
                worksheet: worksheet,
                autoFilterEnabled: true
            }).then(function () {
                // https://github.com/exceljs/exceljs#writing-xlsx
                workbook.xlsx.writeBuffer().then(function (buffer) {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), entityName + '.xlsx');
                });
            });
            e.cancel = true;
        }
    });
    $('#btnApproveSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnApproveSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing ...');

            $.ajax({
                url: url + "/RequestIntegration",
                type: 'PUT',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Approve rows success");
                        $("#modal-approve-selected").modal('hide');
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnApproveSelectedRow').html('<i class="fas fa-paper-plane mr-1"></i>Send Request');
            });
        }
    });

    $('#btnApplyAccountingPeriod').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.category = "hauling";
            payload.id = $('#accounting_period_id').val();
            payload.production_ids = selectedIds;

            $('#btnApplyAccountingPeriod')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Applying ...');

            $.ajax({
                url: "/api/Accounting/AccountingPeriod/ApplyToTransactions",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                //console.log(result);
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Success");
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnApplyAccountingPeriod').html('Apply');
            });
        }
    });

    $('#btnApplyQualitySampling').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.category = "hauling";
            payload.id = $('#quality_sampling_id').val();
            payload.production_ids = selectedIds;

            $('#btnApplyQualitySampling')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Applying ...');

            $.ajax({
                url: "/api/StockpileManagement/QualitySampling/ApplyToTransactions",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                //console.log(result);
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Success");
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnApplyQualitySampling').html('Apply');
            });
        }
    });

    $('#btnDeleteSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnDeleteSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Deleting ...');

            $.ajax({
                url: url + "/DeleteSelectedRows",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Delete rows success");
                        $("#modal-delete-selected").modal('hide');
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnDeleteSelectedRow').html('Delete');
            });
        }
    });

    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            const operationId = generateUniqueId();
            connection.invoke("JoinGroup", operationId).catch(err => console.error(err));
            $('#downloadQueueStatus').text('Initializing download...');

            let payload = {};
            payload.selectedIds = selectedIds;
            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');
            $.ajax({
                url: "/Mining/Hauling/ExcelExport",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify({ ...payload, operationId }),
                headers: {
                    "Authorization": "Bearer " + token
                },
                xhrFields: {
                    responseType: 'blob' // Set the response type to blob
                }
            }).done(function (data, textStatus, xhr) {
                // Check if the response is a blob
                if (data instanceof Blob) {
                    // Create a temporary anchor element
                    var a = document.createElement('a');
                    var url = window.URL.createObjectURL(data);
                    a.href = url;
                    a.download = "CoalHauling.xlsx"; // Set the appropriate file name here
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                    toastr["success"]("File downloaded successfully.");
                    //  $("#modal-download-selected").modal('hide');////

                } else {
                    toastr["error"]("File download failed.");
                }

                /*if (result) {
                    if (result.success) {
                        
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }*/
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnDownloadSelectedRow').html('Download');
            });
        }
    });

    $('#dropdown-download-template').on('click', function () {
        var urlTemplate = document.getElementById('url-download-template').value;

        $('#text-download-template')
            .html('Downloading ... <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');

        window.location.href = urlTemplate;

        setTimeout(function () {
            $('#text-download-template').html('Download Template');
        }, 22000);
    });

    $('#btnUpload').on('click', function () {
        const operationId = generateUniqueId();
        connection.invoke("JoinGroup", operationId).catch(err => console.error(err));
        $('#queueStatus').text('Initializing upload...');

        var f = $("#fUpload")[0].files;
        var filename = $('#fUpload').val();

        if (f.length == 0) {
            alert("Please select a file.");
            return false;
        }
        else {
            var fileExtension = ['xlsx', 'xlsm', 'xls'];
            var extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert("Please select only Excel files.");
                return false;
            }
        }

        $('#btnUpload')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Uploading ...');

        var reader = new FileReader();
        reader.readAsDataURL(f[0]);
        reader.onload = function () {
            var formData = {
                "filename": f[0].name,
                "filesize": f[0].size,
                "data": reader.result.split(',')[1]
            };
            $.ajax({
                url: "/api/Mining/Hauling/UploadDocument",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify({ ...formData, operationId: operationId }),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                //alert('File berhasil di-upload!');
                //location.reload();
                $("#modal-upload-file").modal('hide');
                Swal.fire("Success!", "Upload Data Success.", "success");
                $("#grid").dxDataGrid("refresh");
                $('#progressBar').css('width', '0%').attr('aria-valuenow', 0);
                $('#progressBar').text('0%');
            }).fail(function (jqXHR, textStatus, errorThrown) {
                $('#progressBar').css('width', '0%').attr('aria-valuenow', 0);
                $('#progressBar').text('0%');
                $('#progressBar').removeClass('bg-success').addClass('bg-danger');
                window.location = '/General/General/UploadError';
                $("#modal-upload-file").modal('hide');
                Swal.fire("Error !", "Error Upload Data, Please check the .txt file.", "error");
            }).always(function () {
                $('#btnUpload').html('Upload');
                $('#uploadStatus').html('');
                $('#queueStatus').text('');
            });
        };
        reader.onerror = function (error) {
            alert('Error: ' + error);
        };
    });

});