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
    var areaName = "Planning";
    var entityName = "LTP";
    var url = "/api/" + areaName + "/" + entityName;
    var haulingPlanMonthlyData = null;
    var haulingPlanMonthlyHistoryData = null;
    var CustomerId = ""
    var selectedPlanRecord;

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: url + "/DataGrid",
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
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            /*{
                dataField: "name",
                dataType: "string",
                caption: "Name",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "subname",
                dataType: "string",
                caption: "Subname"
            },*/
            {
                dataField: "mine_code",
                dataType: "string",
                caption: "Mine",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "submine_code",
                dataType: "string",
                caption: "Submine",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "pit_code",
                dataType: "string",
                caption: "Pit",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "subpit_code",
                dataType: "string",
                caption: "Subpit",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "contractor_code",
                dataType: "string",
                caption: "Contractor",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "seam_code",
                dataType: "string",
                caption: "SEAM",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "blok_code",
                dataType: "string",
                caption: "Blok",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
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
            /*{
                dataField: "strip_code",
                dataType: "string",
                caption: "Strip"
            },*/
            {
                dataField: "material_type_id",
                dataType: "string",
                caption: "Material Type",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/MasterList/MasterListIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            filter: ["item_group", "=", "ltp-material-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
            },
            {
                dataField: "reserve_type_id",
                dataType: "string",
                caption: "Reserve Type",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/MasterList/MasterListIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            filter: ["item_group", "=", "ltp-reserve-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
            },
            /*{
                dataField: "int_truethick",
                dataType: "number",
                caption: "Truethick",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: {
                            type: "fixedPoint",
                            precision: 2
                        }
                    }
                }
            },*/
            {
                dataField: "waste_bcm",
                dataType: "number",
                caption: "Waste (bcm)",
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
                }
            },
            {
                dataField: "coal_tonnage",
                dataType: "number",
                caption: "Coal (Tonnage)",
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
                }
            },
            {
                dataField: "tm_ar",
                dataType: "number",
                caption: "TM% (ar)",
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
                visible: false
            },
            {
                dataField: "im_ar",
                dataType: "number",
                caption: "IM% (adb)",
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
                visible: false
            },
            {
                dataField: "ash_ar",
                dataType: "number",
                caption: "Ash% (adb)",
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
                visible: false
            },
            {
                dataField: "vm_ar",
                dataType: "number",
                caption: "VM% (adb)",
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
                visible: false
            },
            {
                dataField: "fc_ar",
                dataType: "number",
                caption: "FC% (adb)",
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
                visible: false
            },
            {
                dataField: "ts_ar",
                dataType: "number",
                caption: "TS% (adb)",
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
                visible: false
            },
            {
                dataField: "gcv_adb_ar",
                dataType: "number",
                caption: "CV Kcal/Kg (adb)",
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
                visible: false
            },
            {
                dataField: "gcv_ar_ar",
                dataType: "number",
                caption: "CV Kcal/Kg (arb)",
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
                visible: false
            },
            {
                dataField: "rd_ar",
                dataType: "number",
                caption: "RD (gr/cc)",
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
                visible: false
            },
            {
                dataField: "rdi_ar",
                dataType: "number",
                caption: "RDI (gr/cc)",
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
                visible: false
            },
            {
                dataField: "hgi_ar",
                dataType: "number",
                caption: "HGI",
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
                visible: false
            },
            {
                dataField: "model_date",
                dataType: "datetime",
                caption: "Model Date",
                format: "yyyy-MM-dd HH:mm",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            /*{
                dataField: "approved",
                dataType: "boolean",
                caption: "Approved",
                visible: false,
            },
            {
                dataField: "approved_by",
                dataType: "text",
                caption: "Approved By",
                visible: false,
                allowEditing: false
            }*/
        ],/*
        onToolbarPreparing: function (e) {

            //let dataGrid2 = e.row.data;
            e.toolbarOptions.items.unshift({
                location: "before",
                widget: "dxButton",
                options: {
                    text: "Fetch Code",
                    icon: "refresh",
                    width: 150,
                    onClick: function (e) {
                        //console.log(masterDetailData);
                        //$("#grid-shipping-transaction-detail").dxDataGrid("getDataSource").reload();
                        let dataGrid = e.component;
                        dataGrid.option('text', 'Fetching ...');

                        $.ajax({
                            url: '/api/Planning/LTP/UpdateContractor/',
                            type: 'PUT',
                            contentType: "application/json",
                            headers: {
                                "Authorization": "Bearer " + token
                            },
                        }).done(function (result) {
                            if (result.success) {
                                Swal.fire("Success!", "Fetching Data successfully.", "success");
                                dataGrid.option('text', 'Fetch Virtual');
                                $("#grid").dxDataGrid("getDataSource").reload();// $("#grid-shipping-transaction-detail").dxDataGrid("getDataSource").reload();
                            } else {
                                Swal.fire("Error !", result.message, "error");
                            }
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            Swal.fire("Failed !", textStatus, "error");
                        }).always(function () {
                            // Reset button text and icon after AJAX request completes

                        });
                    }
                }
            });
        },*/
        onContentReady: function (e) {
            let grid = e.component
            let queryString = window.location.search
            let params = new URLSearchParams(queryString)

            let salesPlanId = params.get("Id")

            if (salesPlanId) {
                grid.filter(["id", "=", salesPlanId])

                if (params.get("openEditingForm") == "true") {
                    let rowIndex = grid.getRowIndexByKey(salesPlanId)

                    grid.editRow(rowIndex)
                }
            }
        },
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0) {
                selectedIds = $.map(data, function (value) {
                    return value.id;
                }).join(",");
                $("#dropdown-delete-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-delete-selected").addClass("disabled");
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

            if (e.parentType == "dataRow" && e.dataField == "plan_type") {
                var editor = e.component;
                e.editorOptions.hint = "barging plan type";
            }
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
        height: 600,
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
    $('#btnFetchAsm').on('click', function () {

        $('#btnFetchAsm')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Fetching ...');

            $.ajax({
                url: url + "/FetchToQualitySampling?id=" + "76d138be53ad48d1822a3bd76aacab1b",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Fetching Data successfully.", "success");
                    $("#modal-fetch-asm").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                }
                }
            }).fail(function (result, jqXHR, textStatus, errorThrown) {
                $("#modal-fetch-asm").modal('hide');
                Swal.fire("Error !", result.responseJSON.message, "error");
            }).always(function () {
                $('#btnFetchAsm').html('Delete');
            });
    });
    $('#btnFetchSnk').on('click', function () {

        $('#btnFetchSnk')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Fetching ...');

        $.ajax({
            url: url + "/FetchToQualitySampling?id=" + "ee0b98be20d44a08bb9c00e56c335327",
            type: 'POST',
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result) {
                if (result.success) {
                    $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Fetching Data successfully.", "success");
                    $("#modal-fetch-snk").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                }
            }
        }).fail(function (result, jqXHR, textStatus, errorThrown) {
            $("#modal-fetch-snk").modal('hide');
            Swal.fire("Error !", result.responseJSON.message, "error");
        }).always(function () {
            $('#btnFetchSnk').html('Delete');
        });
    });
    $('#btnFetchBtl').on('click', function () {

        $('#btnFetchBtl')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Fetching ...');

        $.ajax({
            url: url + "/FetchToQualitySampling?id=" + "7d1bf0bd927c43df822f3287461cdde3",
            type: 'POST',
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result) {
                if (result.success) {
                    $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Fetching Data successfully.", "success");
                    $("#modal-fetch-btl").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                }
            }
        }).fail(function (result, jqXHR, textStatus, errorThrown) {
            $("#modal-fetch-btl").modal('hide');
            Swal.fire("Error !", result.responseJSON.message, "error");
        }).always(function () {
            $('#btnFetchBtl').html('Delete');
        });
    });
    $('#btnFetchKtp').on('click', function () {

        $('#btnFetchKtp')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Fetching ...');

        $.ajax({
            url: url + "/FetchToQualitySampling?id=" + "ab743db8c718439a8de1c00216f56b98",
            type: 'POST',
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result) {
                if (result.success) {
                    $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Fetching Data successfully.", "success");
                    $("#modal-fetch-ktp").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                }
            }
        }).fail(function (result, jqXHR, textStatus, errorThrown) {
            $("#modal-fetch-ktp").modal('hide');
            Swal.fire("Error !", result.responseJSON.message, "error");
        }).always(function () {
            $('#btnFetchKtp').html('Delete');
        });
    });
    $('#btnFetchSti').on('click', function () {

        $('#btnFetchSti')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Fetching ...');

        $.ajax({
            url: url + "/FetchToQualitySampling?id=" + "cd33ec601e334b8ea03daecc45eed8a0",
            type: 'POST',
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result) {
                if (result.success) {
                    $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Fetching Data successfully.", "success");
                    $("#modal-fetch-sti").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                }
            }
        }).fail(function (result, jqXHR, textStatus, errorThrown) {
            $("#modal-fetch-sti").modal('hide');
            Swal.fire("Error !", result.responseJSON.message, "error");
        }).always(function () {
            $('#btnFetchSti').html('Delete');
        });
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
    function masterDetailTemplate(_, masterDetailOptions) {
        var masterDat = masterDetailOptions.data
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "History",
                    template: createHistoryTabTemplate(masterDetailOptions.data)
                },
            ]

        });

    }
    function createHistoryTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "SILS";
            let urlDetail = "/api/" + areaName + "/" + entityName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/HistoryById?Id=" + encodeURIComponent(currentRecord.id),
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
                            dataField: "material_type_id",
                            dataType: "string",
                            caption: "Material Type",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/General/MasterList/MasterListIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: ["item_group", "=", "ltp-material-type"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                        },
                        {
                            dataField: "reserve_type_id",
                            dataType: "string",
                            caption: "Reserve Type",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/General/MasterList/MasterListIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: ["item_group", "=", "ltp-reserve-type"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                        },
                        /*{
                            dataField: "int_truethick",
                            dataType: "number",
                            caption: "Truethick",
                            format: {
                                type: "fixedPoint",
                                precision: 2
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: {
                                        type: "fixedPoint",
                                        precision: 2
                                    }
                                }
                            }
                        },*/
                        {
                            dataField: "waste_bcm",
                            dataType: "number",
                            caption: "Waste (bcm)",
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
                            }
                        },
                        {
                            dataField: "coal_tonnage",
                            dataType: "number",
                            caption: "Coal (Tonnage)",
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
                            }
                        },
                        {
                            dataField: "tm_ar",
                            dataType: "number",
                            caption: "TM% (ar)",
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
                            visible: true
                        },
                        {
                            dataField: "im_ar",
                            dataType: "number",
                            caption: "IM% (adb)",
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
                            visible: true
                        },
                        {
                            dataField: "ash_ar",
                            dataType: "number",
                            caption: "Ash% (adb)",
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
                            visible: true
                        },
                        {
                            dataField: "vm_ar",
                            dataType: "number",
                            caption: "VM% (adb)",
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
                            visible: true
                        },
                        {
                            dataField: "fc_ar",
                            dataType: "number",
                            caption: "FC% (adb)",
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
                            visible: true
                        },
                        {
                            dataField: "ts_ar",
                            dataType: "number",
                            caption: "TS% (adb)",
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
                            visible: true
                        },
                        {
                            dataField: "gcv_adb_ar",
                            dataType: "number",
                            caption: "CV Kcal/Kg (adb)",
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
                            visible: true
                        },
                        {
                            dataField: "gcv_ar_ar",
                            dataType: "number",
                            caption: "CV Kcal/Kg (arb)",
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
                            visible: true
                        },
                        {
                            dataField: "rd_ar",
                            dataType: "number",
                            caption: "RD (gr/cc)",
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
                            visible: true
                        },
                        {
                            dataField: "rdi_ar",
                            dataType: "number",
                            caption: "RDI (gr/cc)",
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
                            visible: true
                        },
                        {
                            dataField: "hgi_ar",
                            dataType: "number",
                            caption: "HGI",
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
                            visible: true
                        },
                        {
                            dataField: "created_on",
                            dataType: "datetime",
                            caption: "Date",
                            format: "yyyy-MM-dd HH:mm"
                        },
                        {
                            dataField: "record_created_by",
                            dataType: "string",
                            caption: "Updated By",
                            //format: "yyyy-MM-dd HH:mm"
                        },
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
                        enabled: false,
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
                        mode: 'batch',
                        allowUpdating: false,
                        allowAdding: false,
                        allowDeleting: false
                    },
                    onEditorPreparing: function (e) {
                        if (e.parentType === "dataRow" && e.dataField === "type_id") {
                            // Access the value of the selected category_id
                            var Id = e.row.data.category_id;

                            // Define the dataSource for the type_id lookup based on the selected category_id
                            var typeDataSource = {
                                store: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/TypeIdLookup?Id=" + Id, // Pass the selected category_id to the controller
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                })
                            };

                            // Set the dataSource for the type_id lookup
                            e.editorOptions.dataSource = typeDataSource;
                        }

                        /* if (e.parentType === "dataRow" && e.dataField === "category_id") {
                             // Di sini Anda dapat memindahkan kode dari event onValueChanged
                             // yang sebelumnya ada di field category_id.
                             // Anda bisa tetap menggunakan e.value untuk mendapatkan nilai yang dipilih.
                             var selectedCategoryId = e.value;
 
                             // Mendapatkan is_problem_productivity dari data yang dipilih
                             var selectedCategory = e.component.lookupData.filter(function (item) {
                                 return item.value === selectedCategoryId;
                             })[0];
 
                             // Mendefinisikan dataSource untuk dropdown "Type" berdasarkan kondisi is_problem_productivity
                             var typeDataSource;
                             if (selectedCategory && selectedCategory.is_problem_productivity === true) {
                                 // Kondisi ketika is_problem_productivity == true
                                 typeDataSource = function (options) {
                                     return {
                                         store: DevExpress.data.AspNet.createStore({
                                             key: "value",
                                             loadUrl: url + "/Category",
                                             onBeforeSend: function (method, ajaxOptions) {
                                                 ajaxOptions.xhrFields = { withCredentials: true };
                                                 ajaxOptions.beforeSend = function (request) {
                                                     request.setRequestHeader("Authorization", "Bearer " + token);
                                                 };
                                             }
                                         })
                                     };
                                 };
                             } else {
                                 // Kondisi ketika is_problem_productivity == false
                                 typeDataSource = function (options) {
                                     return {
                                         store: DevExpress.data.AspNet.createStore({
                                             key: "value",
                                             loadUrl: url + "/Type",
                                             onBeforeSend: function (method, ajaxOptions) {
                                                 ajaxOptions.xhrFields = { withCredentials: true };
                                                 ajaxOptions.beforeSend = function (request) {
                                                     request.setRequestHeader("Authorization", "Bearer " + token);
                                                 };
                                             }
                                         })
                                     };
                                 };
                             }
 
                             // Mengatur dataSource untuk dropdown "Type"
                             e.editorOptions.dataSource = typeDataSource;
                         }*/
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
                    onInitNewRow: function (e) {
                        e.data.sils_id = currentRecord.id;
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
                    }
                });
        }
    }
    $('#btnDownloadSelectedRow').on('click', function () {
        const operationId = generateUniqueId();
        connection.invoke("JoinGroup", operationId).catch(err => console.error(err));
        $('#downloadQueueStatus').text('Initializing download...');

        $('#btnDownloadSelectedRow')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');
        $.ajax({
            url: "/Planning/LTP/ExcelExport",
            type: 'POST',
            cache: false,
            contentType: "application/json",
            data: JSON.stringify(operationId),
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
                url: "/api/Planning/LTP/UploadDocument",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify({ ...formData, operationId: operationId }),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                alert('File berhasil di-upload!');
                //location.reload();
                $("#modal-upload-file").modal('hide');
                $("#grid").dxDataGrid("refresh");
                $('#progressBar').css('width', '0%').attr('aria-valuenow', 0);
                $('#progressBar').text('0%');
            }).fail(function (jqXHR, textStatus, errorThrown) {
                $('#progressBar').css('width', '0%').attr('aria-valuenow', 0);
                $('#progressBar').text('0%');
                $('#progressBar').removeClass('bg-success').addClass('bg-danger');
                window.location = '/General/General/UploadError';
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