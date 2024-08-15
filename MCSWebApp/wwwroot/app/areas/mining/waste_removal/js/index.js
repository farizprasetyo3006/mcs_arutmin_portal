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
    var entityName = "WasteRemoval";
    var url = "/api/" + areaName + "/" + entityName;
    var selectedIds = null;
    var shiftCategory = "";

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("wasteRemovalDate1");
    var tgl2 = sessionStorage.getItem("wasteRemovalDate2");

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
            sessionStorage.setItem("wasteRemovalDate1", formatTanggal(firstDay));
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
            sessionStorage.setItem("wasteRemovalDate2", formatTanggal(lastDay));
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
                ajaxOptions.beforeSend = function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };
            }
        }),
        selection: {
            mode: "multiple"
        },
        remoteOperations: true,
        allowColumnResizing: true,
        columnMinWidth: 100,
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
                width: "150px",
                sortOrder: "asc",
                visible: false,
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                visible: false,
                allowEditing: true,
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
                dataField: "unloading_datetime",
                dataType: "datetime",
                caption: "Date",
                width: "130px",
                validationRules: [{
                    type: "required",
                    message: "The Destination field is required."
                }],
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "source_shift_id",
                dataType: "text",
                caption: "Shift",
                width: "130px",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                visible: true,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Mining/WasteRemoval/ShiftIdLookup", // ---> This Lookup Based on Business Unit or SysAdmin
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
                }
            },
            {
                dataField: "source_location_id",
                dataType: "text",
                caption: "Source",
                width: "120px",
                visible: true,
                validationRules: [{
                    type: "required",
                    message: "This Source field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var _url = url + "/SourceLocationIdLookup";

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
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
/*
            {
                dataField: "source_location_id",
                dataType: "text",
                caption: "Source",
                validationRules: [{
                    type: "required",
                    message: "The Destination field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var _url = "/api/Mining/Processing/SourceLocationIdLookup";

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
                    displayExpr: "text"
                },
                setCellValue: function (rowData, value) {
                    rowData.source_location_id = value;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },*/
            {
                dataField: "destination_location_id",
                dataType: "text",
                caption: "Destination",
                width: "120px",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var _url = url + "/DestinationLocationIdLookup";

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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "waste_id",
                dataType: "text",
                caption: "Material",
                validationRules: [{
                    type: "required",
                    message: "The Destination field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/WasteIdLookup",
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
                dataField: "duration",
                dataType: "string",
                caption: "Duration",
                allowEditing: false,
                visible: false
            },
            /*{
                dataField: "equipment_id",
                dataType: "text",
                caption: "Equipment",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/EquipmentIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text"
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
            },*/
            {
                dataField: "contractor_id",
                dataType: "text",
                caption: "Contractor",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Organisation/Contractor/ContractorIdLookupByIsEquipmentOwner?isEquipmentOwner=true",
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
                sortOrder: "asc",

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
                dataField: "equipment_id",
                dataType: "text",
                caption: "Equipment",
                validationRules: [{
                    type: "required",
                    message: "The Destination field is required."
                }],
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
                setCellValue: function (rowData, value) {
                    rowData.equipment_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "process_flow_id",
                dataType: "text",
                caption: "Process Flow",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "The Destination field is required."
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
                dataField: "transport_id",
                dataType: "text",
                caption: "Transport",
                width: "120px",
                visible: true,
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
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
                dataField: "elevation",
                dataType: "number",
                caption: "Elevation",
                visible: false
            },
            {
                dataField: "density",
                dataType: "number",
                caption: "Density",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "ritase",
                dataType: "number",
                caption: "Ritase",
                visible: false
            },
            {
                dataField: "unloading_quantity",
                dataType: "number",
                caption: "Tonnage",
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                visible: false,
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
                dataField: "uom_id",
                dataType: "text",
                caption: "Unit",

                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/UOM/UOM/UomIdLookup",
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
                },
                visible: false
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
                dataField: "loading_quantity",
                dataType: "number",
                caption: "Volume",
                visible: true,
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
                },
                visible: false,
            },
            {
                dataField: "note",
                dataType: "string",
                caption: "Note",
                formItem: {
                    colSpan: 2,
                    editorType: "dxTextArea"
                },
                visible: false
            },
        ],
        summary: {
            totalItems: [
                {
                    column: 'loading_quantity',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'capacity',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'tare',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'trip_count',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'distance',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'elevation',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
            ],
        },
        masterDetail: {
            enabled: true,
            template: masterDetailTemplate
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
            width: 240,
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
        //height: 1600,
        showBorders: true,
        editing: {
            mode: "popup",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                itemType: "group",
                items: [
                    {
                        dataField: "transaction_number",
                    },  
                    {
                        dataField: "business_unit_id",
                    },
                    {
                        dataField: "unloading_datetime",
                    },
                    {
                        dataField: "source_shift_id",
                    },
                    {
                        dataField: "contractor_id",
                    },
                    {
                        dataField: "duration",
                    },
                    {
                        dataField: "equipment_id",
                    },
                    {
                        dataField: "process_flow_id",
                    },
                    {
                        dataField: "transport_id",
                    },
                    {
                        dataField: "source_location_id",
                    },
                    {
                        dataField: "destination_location_id",
                    },
                    {
                        dataField: "distance",
                    },
                    {
                        dataField: "elevation",
                    },
                    {
                        dataField: "waste_id",
                    },
                    {
                        dataField: "density",
                    },
                    {
                        dataField: "ritase"
                    },
                    {
                        dataField: "unloading_quantity",
                    },
                    {
                        dataField: "loading_quantity"
                    },
                    {
                        dataField: "uom_id",
                    },
                    {
                        dataField: "despatch_order_id",
                    },
                    {
                        dataField: "advance_contract_id",
                    },
                    {
                        dataField: "pic",
                    },
                    {
                        dataField: "note",
                    },
                    {
                        dataField: "status",
                    },
                ]
            }
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
                $("#dropdown-delete-selected").removeClass("disabled");
                $("#dropdown-download-selected").removeClass("disabled");
                $("#dropdown-approve-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-delete-selected").addClass("disabled");
                $("#dropdown-download-selected").addClass("disabled");
                $("#dropdown-approve-selected").addClass("disabled");
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

            if (e.dataField === "source_shift_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let shiftId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Shift/Shift/DataDetail?id=' + shiftId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let shift_category_id = response.shift_category_id;
                            $.ajax({
                                url: '/api/Shift/ShiftCategory/DataDetail?Id=' + shift_category_id,
                                type: 'GET',
                                contentType: "application/json",
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                },
                                success: function (response) {
                                    let result = response.data[0];
                                    shiftCategory = result.shift_category_code;
                                }
                            });
                            grid.cellValue(index, "duration", response.duration);
                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
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

            if (e.dataField === "waste_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let shiftId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Material/Waste/DataDetail?Id=' + shiftId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];

                            grid.cellValue(index, "density", record.density)

                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.dataField === "transport_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let transportId = e.value

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

                            grid.cellValue(index, "tare", record.tare)

                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
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

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Production",
                    template: createDetailTabTemplate(masterDetailOptions.data)
                },
            ]

        });
    }

    function createDetailTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "WasteRemoval";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div class='p-2'>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/DetailsByIdDay?Id=" + encodeURIComponent(currentRecord.id) + "&Shift=" + encodeURIComponent(currentRecord.source_shift_id), // + "&ContractorId=" + encodeURIComponent(currentRecord.contractor_id)
                        insertUrl: urlDetail + "/InsertItemData",
                        updateUrl: urlDetail + "/UpdateItemData",
                        deleteUrl: urlDetail + "/DeleteItemData",

                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    remoteOperations: true,
                    allowColumnResizing: true,
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        {
                            dataField: "truck_id",
                            dataType: "string",
                            caption: "Truck",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    loadUrl: "/api/Transport/Truck/TruckIdLookup",
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
                            sortOrder: "asc",
                        },
                        {
                            dataField: "truck_factor",
                            dataType: "string",
                            caption: "Truck Factor",
                            placeholder: "Autofill",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "ritase",
                            dataType: "number",
                            caption: "Ritase",
                            allowEditing: false
                        },
                        {
                            dataField: "jam01",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "07:00 - 08:00" : "19:00 - 20:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam02",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "08:00 - 09:00" : "20:00 - 21:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam03",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "09:00 - 10:00" : "21:00 - 22:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam04",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "10:00 - 11:00" : "22:00 - 23:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam05",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "11:00 - 12:00" : "23:00 - 00:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam06",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "12:00 - 13:00" : "00:00 - 01:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam07",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "13:00 - 14:00" : "01:00 - 02:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam08",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "14:00 - 15:00" : "02:00 - 03:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam09",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "15:00 - 16:00" : "03:00 - 04:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam10",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "16:00 - 17:00" : "04:00 - 05:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam11",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "17:00 - 18:00" : "05:00 - 06:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "jam12",
                            dataType: "number",
                            caption: masterDetailData.source_shift_name.toLowerCase().includes("day") ? "18:00 - 19:00" : "06:00 - 07:00",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        }
                    ],
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
                        width: 240,
                        placeholder: "Search..."
                    },
                    filterPanel: {
                        visible: false
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

                    //height: 1600,
                    showBorders: true,
                    editing: {
                        mode: 'batch',
                        allowUpdating: true,
                        allowAdding: false,
                        allowDeleting: true
                    },
                    onEditorPreparing: function (e) {
                        if (e.parentType === 'searchPanel') {
                            e.editorOptions.onValueChanged = function (arg) {
                                if (arg.value.length == 0 || arg.value.length > 2) {
                                    e.component.searchByText(arg.value);
                                }
                            }
                        }
                        // Set onValueChanged for sales_charge_id
                        if (e.parentType === "dataRow" && e.dataField == "transport_id") {

                            let standardHandler = e.editorOptions.onValueChanged
                            let index = e.row.rowIndex
                            let grid = e.component
                            let rowData = e.row.data

                            e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                                // Get its value (Id) on value changed
                                let transportId = e.value

                                // Get another data from API after getting the Id
                                $.ajax({
                                    url: '/api/Transport/Truck/DataDetail?Id=' + transportId,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        let resultData = response.data[0]
                                        //console.log(salesCharge)

                                        // Set its corresponded field's value
                                        grid.cellValue(index, "truck_factor", resultData.truck_factor)
                                        grid.cellValue(index, "density", resultData.density)
                                    }
                                })

                                standardHandler(e) // Calling the standard handler to save the edited value
                            }
                        }
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
                    //onInitNewRow: function (e) {
                    //    e.data.waste_removal_id = currentRecord.id;
                    //},
                    onSaved: function (e) {
                        $("#grid").dxDataGrid("refresh");
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
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailName + '.xlsx');
                            });
                        });
                        e.cancel = true;
                    },

                    onToolbarPreparing: function (e) {
                        let dataGrid = e.component;
                        //let dataGrid2 = e.row.data;
                        e.toolbarOptions.items.unshift({
                            location: "before",
                            widget: "dxButton",
                            options: {
                                text: "Fetch Item",
                                icon: "doc",
                                width: 206,
                                //className: "ml-2",
                                //style: "margin-left: 15",
                                onClick: function (e) {
                                    //productionPlanMonthlyHistoryData = dataGrid;
                                    //showPlanMonthlyHistoryPopup(productionPlanMonthlyHistoryData);

                                    dataGrid.beginCustomLoading()

                                    //$("#modal-fetch-item").modal('show')

                                    $.ajax({
                                        url: "/api/Mining/WasteRemoval/FetchItem?id=" + currentRecord.id,
                                        type: 'POST',
                                        cache: false,
                                        contentType: "application/json",
                                        //data: JSON.stringify(payload),
                                        headers: {
                                            "Authorization": "Bearer " + token
                                        }
                                    }).done(function (result) {
                                        if (result) {
                                            if (result.success) {
                                                dataGrid.refresh()
                                                $("#grid").dxDataGrid("refresh");
                                                toastr["success"](result.message ?? "Fetch item success");
                                            }
                                            else {
                                                toastr["error"](result.message ?? "Error");
                                            }
                                        }
                                    }).fail(function (jqXHR, textStatus, errorThrown) {
                                        toastr["error"]("Action failed.");
                                        //dataGrid.endCustomLoading()
                                    }).always(function () {
                                        dataGrid.endCustomLoading()
                                    });
                                }
                            }
                        });
                    },
                });
        }
    }

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
        const operationId = generateUniqueId();
        connection.invoke("JoinGroup", operationId).catch(err => console.error(err));
        $('#downloadQueueStatus').text('Initializing download...');

        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;
            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');
            $.ajax({
                url: "/Mining/WasteRemoval/ExcelExport",
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
                    a.download = "WasteRemoval.xlsx"; // Set the appropriate file name here
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

    $('#btnApproveSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnApproveSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing ...');

            $.ajax({
                url: url + "/ApproveUnapprove",
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

    $('#dropdown-download-template').on('click', function () {
        var urlTemplate = document.getElementById('url-download-template').value;

        $('#text-download-template')
            .html('Downloading ... <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');

        window.location.href = urlTemplate;

        setTimeout(function () {
            $('#text-download-template').html('Download Template');
        }, 15000);
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
                url: "/api/Mining/WasteRemoval/UploadDocument",
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