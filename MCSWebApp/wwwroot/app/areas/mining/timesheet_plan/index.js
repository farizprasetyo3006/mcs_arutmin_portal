$(function () {

    var token = $.cookie("Token");
    var areaName = "Timesheet";
    var entityName = "TimesheetPlan";
    var gridUrl = "/api/" + areaName + "/" + entityName;

    /**
     * ===================
     * Timesheet Grid
     * ===================
     */

    var lookupTimeDataSource = {
        store: new DevExpress.data.CustomStore({
            key: "value",
            loadMode: "raw",
            load: function () {
                return $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: url + "/TimeLookup",
                    headers: {
                        "Authorization": "Bearer " + token
                    }
                });
            }
        }),
        sort: "text"
    };

    $("#dt-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: gridUrl + "/DataGrid",
            insertUrl: gridUrl + "/InsertData",
            updateUrl: gridUrl + "/UpdateData",
            deleteUrl: gridUrl + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };
            }
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "timesheet_date",
                caption: "Date",
                dataType: "date",
                validationRules: [{
                    type: "required",
                    message: "Date is required."
                }],
                sortOrder: "desc",
            },
            {
                dataField: "shift_id",
                dataType: "text",
                caption: "Shift",
                validationRules: [{
                    type: "required",
                    message: "Shift is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Shift/Shift/ShiftIdLookup",
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
                    displayExpr: "text"
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            /*{
                dataField: "activity_id",
                dataType: "text",
                caption: "Activity",
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
                            filter: ["item_group", "=", "timesheet-activity"],
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                validationRules: [{
                    type: "required",
                    message: "Activity is required."
                }],
            },*/
            {
                dataField: "cn_unit_id",
                dataType: "text",
                caption: "CN Unit",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Transport/Truck/TransportIdLookup",
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
                    displayExpr: "text"
                },
                validationRules: [{
                    type: "required",
                    message: "CN Unit is required."
                }],
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "cn_name",
                dataType: "text",
                caption: "Equipment Name",
                editorOptions: { readOnly: true },
            },
            {
                dataField: "operator_id",
                dataType: "text",
                caption: "Operator NIK",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/Employee/EmployeeOperatorNumberLookup",
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
                    displayExpr: "text"
                },
                visible: false,
				formItem: {
                    visible: false,
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "operator_name",
                dataType: "text",
                caption: "Operator",
                editorOptions: { readOnly: true },
                visible: false,
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "supervisor_id",
                dataType: "text",
                caption: "Supervisor NIK",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/Employee/EmployeeSupervisorNumberLookup",
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
                    displayExpr: "text"
                },
                visible: false,
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "supervisor_name",
                dataType: "text",
                caption: "Supervisor",
                editorOptions: { readOnly: true },
                visible: false,
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "hour_start",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                caption: "Hour Meter Awal",
                validationRules: [
                    {
                        type: "custom",
                        message: "The entered was out of min/max range",
                        validationCallback: function (args) {
                            var currentMax = args.data.hour_end
                            if (currentMax != null && args.value > currentMax) {
                                args.rule.message = "Hour Meter Awal must less than " + currentMax
                                return false;
                            }
                            return true;
                        },
                        reevaluate: true
                    }
                ],
                visible: false,
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "hour_end",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                caption: "Hour Meter Akhir",
                validationRules: [
                    {
                        type: "custom",
                        message: "The entered was out of min/max range",
                        validationCallback: function (args) {
                            var currentMin = args.data.hour_start
                            if (currentMin != null && args.value < currentMin) {
                                args.rule.message = "Hour Meter Akhir must more than " + currentMin
                                return false;
                            }
                            return true;
                        },
                        reevaluate: true
                    }
                ],
                visible: false,
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "plan_type",
                dataType: "string",
                caption: "Plan Type",
                validationRules: [{
                    type: "required",
                    message: "The Plan Type is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=plan-type",
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
                }
            },
            {
                dataField: "activity_plan",
                dataType: "text",
                caption: "Activity Plan",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=activity-plan-type",
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
                /*validationRules: [{
                    type: "required",
                    message: "The Activity Plan is required."
                }],*/
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            /*{
                dataField: "quantity",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                },
                label: {
                    text: "Quantity"
                },
                validationRules: [{
                    type: "required",
                    message: "Quantity is required."
                }],
            },
            {
                dataField: "mine_location_id",
                dataType: "text",
                caption: "Location",
                validationRules: [{
                    type: "required",
                    message: "The Mine Location is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Location/MineLocation/MineLocationCodeLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "uom_id",
                dataType: "text",
                caption: "UoM",
                validationRules: [{
                    type: "required",
                    message: "Unit of Measurement is required."
                }],
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
                    displayExpr: "text"
                }
            },
            {
                dataField: "material_id",
                dataType: "text",
                caption: "Material Code",
                validationRules: [{
                    type: "required",
                    message: "Material is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Timesheet/Timesheet/ProductOrWasteIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },*/
            {
                caption: "Detail",
                type: "buttons",
                width: 150,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "See Timesheet Detail",
                    text: "Open Detail",
                    onClick: function (e) {
                        recordId = e.row.data.id
                        window.location = "/mining/timesheetplan/detail/" + recordId
                    }
                }]
            },
            {
                type: "buttons",
                buttons: ["edit", "delete"]
            }
        ],
        masterDetail: {
            enabled: false,
            template: function (container, options) {
                var currentRecord = options.data;
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
            // Set onValueChanged for equipment_id
            if (e.parentType === "dataRow" && e.dataField == "cn_unit_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value
                    // Get another data from API after getting the Id

                    $.ajax({
                        url: '/api/Timesheet/Timesheet/EGIDataDetail?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            //console.log(response);
                            let record = response.data[0];
                            // Set its corresponded field's value
                            var equipment_name = "";
                            if (record.equipment_name == undefined) {
                                equipment_name = record.vehicle_name;
                            } else {
                                equipment_name = record.equipment_name;
                            }
                            grid.cellValue(index, "cn_name", equipment_name);
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "operator_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value
                    // Get another data from API after getting the Id

                    $.ajax({
                        url: '/api/General/Employee/DataDetail?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];
                            // Set its corresponded field's value
                            grid.cellValue(index, "operator_name", record.employee_name);
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "supervisor_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value
                    // Get another data from API after getting the Id

                    $.ajax({
                        url: '/api/General/Employee/DataDetail?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];
                            // Set its corresponded field's value
                            grid.cellValue(index, "supervisor_name", record.employee_name);
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
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
        height: 800,
        showBorders: true,
        editing: {
            mode: "form",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 2,
                items: [
                    {
                        dataField: "timesheet_date",
                    },
                    {
                        dataField: "shift_id",
                    },
                    /*{
                        dataField: "activity_id",
                        colSpan: 2,
                    },*/
                    {
                        dataField: "cn_unit_id",
                    },
                    {
                        dataField: "cn_name",
                    },
                    {
                        dataField: "plan_type",
                    },
                    {
                        dataField: "activity_plan",
                    },

                    /*{
                        dataField: "quantity",
                    },
                    {
                        dataField: "uom_id",
                    },
                    {
                        dataField: "mine_location_id",
                    },
                    {
                        dataField: "material_id",
                    },*/
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

    $('#btnUpload').on('click', function () {
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
                url: "/api/Timesheet/TimesheetPlan/UploadDocument",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(formData),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                alert('File berhasil di-upload!');
                //location.reload();
                $("#modal-upload-file").modal('hide');
                $("#grid").dxDataGrid("refresh");
            }).fail(function (jqXHR, textStatus, errorThrown) {
                window.location = '/General/General/UploadError';
                alert('File gagal di-upload!');
            }).always(function () {
                $('#btnUpload').html('Upload');
            });
        };
        reader.onerror = function (error) {
            alert('Error: ' + error);
        };
    });

});