$(function () {

    var token = $.cookie("Token");
    var areaName = "Port";
    var entityName = "Shipping";
    var url = "/api/" + areaName + "/" + entityName;
    const maxFileSize = 52428800;
    var selectedIds = null;

    var shippingTransactionData;

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0") + "-"
            + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("ShipUnloadingDate1");
    var tgl2 = sessionStorage.getItem("ShipUnloadingDate2");

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
            sessionStorage.setItem("ShipUnloadingDate1", formatTanggal(firstDay));
            _loadUrl = url + "/Unloading/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                + "/" + encodeURIComponent(formatTanggal(lastDay));
        }
    });

    $("#date-box2").dxDateBox({
        type: "datetime",
        displayFormat: 'dd MMM yyyy HH:mm',
        value: lastDay,
        onValueChanged: function (data) {
            lastDay = new Date(data.value);
            sessionStorage.setItem("ShipUnloadingDate2", formatTanggal(lastDay));
            _loadUrl = url + "/Unloading/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                + "/" + encodeURIComponent(formatTanggal(lastDay));
        }
    });

    $('#btnView').on('click', function () {
        location.reload();
    })

    var _loadUrl = url + "/Unloading/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
        + "/" + encodeURIComponent(formatTanggal(lastDay));

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: _loadUrl,
            insertUrl: url + "/Unloading/InsertData",
            updateUrl: url + "/Unloading/UpdateData",
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
                sortOrder: "asc"
            },
            {
                dataField: "accounting_period_id",
                dataType: "text",
                caption: "Accounting Period",
                visible:false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/AccountingPeriodIdLookup",
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
                dataField: "despatch_order_id",
                dataType: "text",
                caption: "Shipping Order",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
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
                    displayExpr: "text"
                },
                setCellValue: function (rowData, value) {
                    rowData.despatch_order_id = value;
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
                formItem: {
                    colSpan: 2
                },
                validationRules: [{
                    type: "required",
                    message: "This field is required."
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
                    displayExpr: "text"
                },
                setCellValue: function (rowData, value) {
                    rowData.process_flow_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "ship_location_id",
                dataType: "text",
                caption: "Vessel",
                //formItem: {
                //    colSpan: 2
                //},
                allowEditing: false,
                lookup: {
                    dataSource: function (options) {
                        var _url = url + "/ShipLocationIdLookup";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.despatch_order_id !== undefined
                                    && options.data.despatch_order_id !== null) {
                                    _url += "?DespatchOrderId=" + encodeURIComponent(options.data.despatch_order_id);
                                     //_url += "?ProcessFlowId=" + encodeURIComponent(options.data.process_flow_id);
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "quantity",
                dataType: "number",
                caption: "Barge Unloading Quantity",
                visible: false,
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
                allowEditing: false,
            },
            {
                dataField: "uom_id",
                dataType: "text",
                caption: "Unit",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
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
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "product_name",
                dataType: "string",
                caption: "Product Name",
                visible: false,
                allowEditing: false
            },
            {
                dataField: "equipment_id",
                dataType: "text",
                caption: "Equipment",
                visible: false,
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
                }
            },
            {
                dataField: "quality_sampling_id",
                dataType: "text",
                caption: "Quality Sampling",
                formItem: {
                    colSpan: 2
                },
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
                    displayExpr: "text"
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "despatch_order_link",
                dataType: "string",
                caption: "Shipping Order",
                visible: false,
                allowFiltering: false
            },
            {
                dataField: "delivery_term_name",
                caption: "Delivery Term",
                dataType: "string",
                visible: false,
                allowEditing: false,
            },
            {
                dataField: "reference_number",
                dataType: "string",
                caption: "Draft Survey Number",
                visible: false
            },
            {
                dataField: "draft_survey_id",
                dataType: "text",
                caption: "Draft Survey",
                visible: false,
                formItem: {
                    colSpan: 2
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/DraftSurveyIdLookup",
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
            },
            //{
            //    dataField: "distance",
            //    dataType: "number",
            //    caption: "Distance",
            //    format: "fixedPoint",
            //    formItem: {
            //        editorType: "dxNumberBox",
            //        editorOptions: {
            //            format: "fixedPoint",
            //        }
            //    }
            //},
            {
                dataField: "note",
                dataType: "string",
                caption: "Note",
                visible: false,
                formItem: {
                    colSpan: 2,
                    editorType: "dxTextArea"
                }
            },
            {
                dataField: "ref_work_order",
                dataType: "string",
                caption: "Ref.Work Order",
                visible: false,
            },
            {
                dataField: "arrival_datetime",
                dataType: "datetime",
                caption: "Arrival DateTime",
                format: "yyyy-MM-dd HH:mm:ss",
                visible: false
            },
            {
                dataField: "berth_datetime",
                dataType: "datetime",
                caption: "Alongside DateTime",
                format: "yyyy-MM-dd HH:mm:ss",
                visible: false
            },
            {
                dataField: "start_datetime",
                dataType: "datetime",
                caption: "Commenced Unloading DateTime",
                format: "yyyy-MM-dd HH:mm:ss",
                sortOrder: "desc",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "end_datetime",
                dataType: "datetime",
                caption: "Completed Unloading DateTime",
                format: "yyyy-MM-dd HH:mm:ss",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "unberth_datetime",
                dataType: "datetime",
                caption: "Cast Off DateTime",
                format: "yyyy-MM-dd HH:mm:ss",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "departure_datetime",
                dataType: "datetime",
                caption: "Departure DateTime",
                format: "yyyy-MM-dd HH:mm:ss",
                visible: false
            },
            {
                dataField: "original_quantity",
                dataType: "number",
                caption: "Draft Survey Quantity",
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
                    message: "The field is required."
                }],
                visible: false
            },
            //initial draft survey
            {
                dataField: "initial_draft_survey",
                dataType: "datetime",
                caption: "Initial Draft Survey",
                format: "yyyy-MM-dd HH:mm:ss",
                visible: false
            },
            //Final draft survey
            {
                dataField: "final_draft_survey",
                dataType: "datetime",
                caption: "Final Draft Survey",
                format: "yyyy-MM-dd HH:mm:ss",
                visible: false
            },
            {
                dataField: "imo_number",
                caption: "IMO Number",
                dataType: "string",
                visible: false,
                allowEditing: false
            },
            {
                dataField: "is_geared",
                dataType: "boolean",
                caption: "Is Geared",
                visible: true,
                allowEditing: false
            },
            {
                dataField: "owner_name",
                caption: "Owner",
                dataType: "string",
                visible: false,
                allowEditing: false
            },
            {
                dataField: "destination_location_id",
                dataType: "text",
                caption: "Destination Location",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var _url = url + "/Unloading/DestinationLocationIdLookup";
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
                    rowData.destination_location_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                allowEditing: true,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/BusinessUnitIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                }
            },
        ],
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
            useIcons: true,
            form: {
                itemType: "group",
                caption: "Unloading",
                items: [
                    {
                        dataField: "transaction_number",
                    },
                    {
                        dataField: "process_flow_id",
                    },
                    {
                        dataField: "ref_work_order",
                    },
                    /*{
                        dataField: "accounting_period_id",
                    },*/
                    {
                        dataField: "despatch_order_id",
                    },
                    {
                        dataField: "delivery_term_name",
                    },
                    {
                        dataField: "despatch_order_link",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "See Shipping Order Detail",
                            disabled: true
                        }
                    },
                    
                    {
                        dataField: "destination_location_id",
                    },
                    {
                        dataField: "ship_location_id",
                    },
                    {
                        dataField: "imo_number",
                    },
                    {
                        dataField: "is_geared",
                    },
                    {
                        dataField: "owner_name",
                    },
                    {
                        dataField: "reference_number",
                    },
                    {
                        dataField: "original_quantity",
                    },
                    //{
                    //    dataField: "quantity",
                    //},
                    {
                        dataField: "uom_id",
                    },
                    
                    {
                        dataField: "product_name",
                    },
                    {
                        dataField: "initial_draft_survey",
                    },
                    {
                        dataField: "final_draft_survey",
                    },
                    {
                        dataField: "equipment_id",
                    },
                    //{
                    //    dataField: "quality_sampling_id",
                    //},
                    //{
                    //    dataField: "draft_survey_id",
                    //},
                    //{
                    //    dataField: "distance",
                    //},
                    {
                        dataField: "note",
                    },
                    {
                        dataField: "arrival_datetime",
                    },
                    {
                        dataField: "berth_datetime",
                    },
                    {
                        dataField: "start_datetime",
                    },
                    {
                        dataField: "end_datetime",
                    },
                    {
                        dataField: "unberth_datetime",
                    },
                    {
                        dataField: "departure_datetime",
                    },
                    {
                        dataField: "quantity",
                    },
                    {
                        dataField: "business_unit_id",
                    }
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
                // $("#dropdown-delete-selected").removeClass("disabled");
                $("#dropdown-approve-selected").removeClass("disabled");
                $("#dropdown-download-selected").removeClass("disabled");
            }
            else {
                // $("#dropdown-delete-selected").addClass("disabled");
                $("#dropdown-approve-selected").addClass("disabled");
                $("#dropdown-download-selected").addClass("disabled");
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
            // Enable and Set Shipping Order Link button after Shipping Order selected
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_link") {
                if (e.row.data.despatch_order_id) {
                    let despatchOrderId = e.row.data.despatch_order_id

                    e.editorOptions.onClick = function (e) {
                        window.open("/Sales/DespatchOrder/Index?Id=" + despatchOrderId + "&openEditingForm=true", "_blank")
                    }
                    e.editorOptions.disabled = false
                }
            }

            if (e.parentType === "dataRow") {
                e.editorOptions.disabled = e.row.data && e.row.data.accounting_period_is_closed;
            }
            if (e.dataField === "despatch_order_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let despatchOrderId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Sales/DespatchOrder/DataDetail?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            //let record = response.data[0]
                            let record = response.data;
                            // Set its corresponded field's value
                            grid.cellValue(index, "delivery_term_name", record.delivery_term_name)
                            grid.cellValue(index, "ship_location_id", record.vessel_name != null ? record.vessel_id : "")
                            grid.cellValue(index, "product_name", record.product_name)

                            grid.cellValue(index, "imo_number", record.imo_number)
                            grid.cellValue(index, "is_geared", record.is_geared)
                            grid.cellValue(index, "owner_name", record.owner_name)
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
                //{
                //    title: "Sources",
                //    template: createSourcesTabTemplate(masterDetailOptions.data)
                //},
                {
                    title: "Documents",
                    template: createDocumentsTabTemplate(masterDetailOptions.data)
                },
                //{
                //    title: "Quality Sampling",
                //    template: createQualitySamplingTab(masterDetailOptions.data)
                //}
            ]
        });
    }

    function createSourcesTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "ShippingDetail";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByShippingId/" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/Unloading/InsertData",
                        updateUrl: urlDetail + "/Unloading/UpdateData",
                        deleteUrl: urlDetail + "/DeleteData",
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
                            dataField: "shipping_transaction_id",
                            caption: "Shipping Transaction Id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            }
                        },
                        {
                            dataField: "transaction_number",
                            dataType: "string",
                            caption: "Transaction Number",
                            allowEditing: false
                        },
                        {
                            dataField: "detail_location_id",
                            dataType: "text",
                            caption: "Destination",
                            formItem: {
                                colSpan: 2
                            },
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
                            }],
                            lookup: {
                                dataSource: function (options) {
                                    var _url = urlDetail + "/Unloading/DestinationLocationIdLookup";

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
                                rowData.detail_location_id = value;
                                rowData.survey_id = null;
                            }
                        },
                        {
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            },
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }]
                        },
                        {
                            dataField: "uom_id",
                            dataType: "text",
                            caption: "Unit",
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
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
                            dataField: "equipment_id",
                            dataType: "text",
                            caption: "Equipment",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/EquipmentIdLookup",
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
                            visible: false
                        },
                        {
                            dataField: "hour_usage",
                            dataType: "number",
                            caption: "Hour Usage"
                        },
                        {
                            dataField: "survey_id",
                            dataType: "text",
                            caption: "Survey",
                            lookup: {
                                dataSource: function (options) {
                                    var _url = urlDetail + "/SurveyIdLookup";

                                    if (options !== undefined && options !== null) {
                                        if (options.data !== undefined && options.data !== null) {
                                            if (options.data.detail_location_id !== undefined
                                                && options.data.detail_location_id !== null) {
                                                _url += "?LocationId=" + encodeURIComponent(options.data.detail_location_id);
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
                            }
                        },
                        {
                            dataField: "final_quantity",
                            dataType: "number",
                            caption: "Final Quantity",
                            allowEditing: false
                        },
                        {
                            dataField: "note",
                            dataType: "string",
                            caption: "Note",
                            formItem: {
                                colSpan: 2,
                                editorType: "dxTextArea"
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
                        mode: "form",
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
                    onInitNewRow: function (e) {
                        e.data.shipping_transaction_id = currentRecord.id;
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

    let documentDataGrid
    function createDocumentsTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "ShippingLoadUnloadDocument";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            shippingTransactionData = currentRecord

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByShippingTransactionId/" + encodeURIComponent(currentRecord.id),
                        updateUrl: urlDetail + "/Loading/UpdateData",
                        deleteUrl: urlDetail + "/DeleteData",
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
                        //{
                        //    dataField: "document_type_id",
                        //    caption: "Document Type",
                        //    visible: true,
                        //    lookup: {
                        //        dataSource: function () {
                        //            return {
                        //                store: DevExpress.data.AspNet.createStore({
                        //                    key: "value",
                        //                    loadUrl: "/api/General/MasterList/MasterListIdLookup",
                        //                    onBeforeSend: function (method, ajaxOptions) {
                        //                        ajaxOptions.xhrFields = { withCredentials: true };
                        //                        ajaxOptions.beforeSend = function (request) {
                        //                            request.setRequestHeader("Authorization", "Bearer " + token);
                        //                        };
                        //                    }
                        //                }),
                        //                filter: ["item_group", "=", "document-type"]
                        //            }
                        //        },
                        //        searchEnabled: true,
                        //        valueExpr: "value",
                        //        displayExpr: "text"
                        //    },
                        //},
                        {
                            dataField: "activity_id",
                            dataType: "string",
                            caption: "Activity",
                            lookup: {
                                dataSource: function () {
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
                                        filter: ["item_group", "=", "activity"]
                                    }
                                },
                                searchEnabled: true,
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                        },
                        {
                            dataField: "remark",
                            dataType: "string",
                            caption: "Remark",
                        },
                        //{
                        //    dataField: "quantity",
                        //    dataType: "boolean",
                        //    caption: "Quantity"
                        //},
                        //{
                        //    dataField: "quality",
                        //    dataType: "boolean",
                        //    caption: "Quality"
                        //},
                        {
                            dataField: "filename",
                            dataType: "string",
                            caption: "Document"
                        },
                        {
                            caption: "Download",
                            type: "buttons",
                            width: 100,
                            buttons: [{
                                cssClass: "btn-dxdatagrid",
                                hint: "Download attachment",
                                text: "Download",
                                onClick: function (e) {
                                    // Download file from Ajax. Ref: https://stackoverflow.com/a/9970672
                                    let documentData = e.row.data
                                    let documentName = /[^\\]*$/.exec(documentData.filename)[0]

                                    let xhr = new XMLHttpRequest()
                                    xhr.open("GET", "/api/Port/ShippingLoadUnloadDocument/DownloadDocument/" + documentData.id, true)
                                    xhr.responseType = "blob"
                                    xhr.setRequestHeader("Authorization", "Bearer " + token)

                                    xhr.onload = function (e) {
                                        let blobURL = window.webkitURL.createObjectURL(xhr.response)

                                        let a = document.createElement("a")
                                        a.href = blobURL
                                        a.download = documentName
                                        document.body.appendChild(a)
                                        a.click()
                                    };

                                    xhr.send()
                                }
                            }]
                        },
                        {
                            type: "buttons",
                            buttons: ["edit", "delete"]
                        }
                    ],
                    onToolbarPreparing: function (e) {
                        let toolbarItems = e.toolbarOptions.items;

                        // Modifies an existing item
                        toolbarItems.forEach(function (item) {
                            if (item.name === "addRowButton") {
                                item.options = {
                                    icon: "plus",
                                    onClick: function (e) {
                                        openDocumentPopup()
                                    }
                                }
                            }

                            if (item.name === "editRowButton") {
                                item.options = {
                                    icon: "edit",
                                    onClick: function (e) {
                                        openDocumentPopup()
                                    }
                                }
                            }
                        });
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
                    showBorders: true,
                    editing: {
                        mode: "form",
                        allowAdding: true,
                        allowUpdating: false,
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
                    onInitNewRow: function (e) {
                        e.data.shipping_transaction_id = currentRecord.id;
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

            return documentDataGrid
        }
    }

    function createQualitySamplingTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "Shipping";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            bargingTransactionData = currentRecord

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByShippingTransactionId/" + encodeURIComponent(currentRecord.id),
                        //updateUrl: urlDetail + "/UpdateData",
                        //deleteUrl: urlDetail + "/DeleteData",
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
                            dataField: "quality_sampling_id",
                            caption: "Quality Sampling Id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            }
                        },
                        {
                            dataField: "analyte_name",
                            dataType: "text",
                            caption: "Analyte",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }]
                        },
                        {
                            dataField: "uom_id",
                            dataType: "text",
                            caption: "Unit",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
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
                            dataField: "analyte_value",
                            dataType: "number",
                            caption: "Value",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }]
                        }
                    ],
                    onToolbarPreparing: function (e) {
                        let toolbarItems = e.toolbarOptions.items;

                        // Modifies an existing item
                        toolbarItems.forEach(function (item) {
                            if (item.name === "addRowButton") {
                                item.options = {
                                    icon: "plus",
                                    onClick: function (e) {
                                        openDocumentPopup()
                                    }
                                }
                            }

                            if (item.name === "editRowButton") {
                                item.options = {
                                    icon: "edit",
                                    onClick: function (e) {
                                        openDocumentPopup()
                                    }
                                }
                            }
                        });
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
                    showBorders: true,
                    editing: {
                        mode: "form",
                        allowAdding: false,
                        allowUpdating: false,
                        allowDeleting: false,
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
                    onInitNewRow: function (e) {
                        e.data.shipping_transaction_id = currentRecord.id;
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

            return documentDataGrid
        }
    }

    const documentPopupOptions = {
        width: "80%",
        height: "auto",
        showTitle: true,
        title: "Add Attachment",
        visible: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        contentTemplate: function (e) {
            let formContainer = $("<div>")
            formContainer.dxForm({
                formData: {
                    id: "",
                    shipping_transaction_id: shippingTransactionData.id,
                    activity_id: "",
                    document_type_id: "",
                    remark: "",
                    quantity: false,
                    quality: false,
                    file: ""
                },
                colCount: 2,
                items: [
                    {
                        dataField: "shipping_transaction_id",
                        label: {
                            text: "Shipping Transaction Id"
                        },
                        validationRules: [{
                            type: "required"
                        }],
                        visible: false
                    },
                    {
                        dataField: "activity_id",
                        editorType: "dxSelectBox",
                        label: {
                            text: "Activity"
                        },
                        editorOptions: {
                            dataSource: new DevExpress.data.DataSource({
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
                                filter: ["item_group", "=", "activity"]
                            }),
                            searchEnabled: true,
                            valueExpr: "value",
                            displayExpr: "text"
                        },
                    },
                    //{
                    //    dataField: "document_type_id",
                    //    editorType: "dxSelectBox",
                    //    label: {
                    //        text: "Document Type"
                    //    },
                    //    editorOptions: {
                    //        dataSource: new DevExpress.data.DataSource({
                    //            store: DevExpress.data.AspNet.createStore({
                    //                key: "value",
                    //                loadUrl: "/api/General/MasterList/MasterListIdLookup",
                    //                onBeforeSend: function (method, ajaxOptions) {
                    //                    ajaxOptions.xhrFields = { withCredentials: true };
                    //                    ajaxOptions.beforeSend = function (request) {
                    //                        request.setRequestHeader("Authorization", "Bearer " + token);
                    //                    };
                    //                }
                    //            }),
                    //            filter: ["item_group", "=", "document-type"]
                    //        }),
                    //        searchEnabled: true,
                    //        valueExpr: "value",
                    //        displayExpr: "text"
                    //    },
                    //},
                    //{
                    //    dataField: "quantity",
                    //    editorType: "dxCheckBox",
                    //    label: {
                    //        text: "Quantity"
                    //    },
                    //},
                    //{
                    //    dataField: "quality",
                    //    editorType: "dxCheckBox",
                    //    label: {
                    //        text: "Quality"
                    //    },
                    //},
                    {
                        dataField: "remark",
                        editortype: "dxTextArea",
                        label: {
                            text: "Remark"
                        },
                        editorOptions: {
                            height: 50
                        },
                        colSpan: 2
                    },
                    {
                        dataField: "file",
                        name: "file",
                        label: {
                            text: "File"
                        },
                        template: function (data, itemElement) {
                            itemElement.append($("<div>").attr("id", "file").dxFileUploader({
                                uploadMode: "useForm",
                                multiple: false,
                                maxFileSize: maxFileSize,
                                invalidMaxFileSizeMessage: "Max. file size is 50 Mb",
                                onValueChanged: function (e) {
                                    data.component.updateData(data.dataField, e.value)
                                }
                            }));
                        },
                        validationRules: [{
                            type: "required"
                        }],
                        colSpan: 2
                    },
                    {
                        itemType: "button",
                        colSpan: 2,
                        horizontalAlignment: "right",
                        buttonOptions: {
                            text: "Save",
                            type: "secondary",
                            useSubmitBehavior: true,
                            onClick: function () {
                                let formData = formContainer.dxForm("instance").option('formData')
                                let file = formData.file[0]

                                var reader = new FileReader();
                                reader.readAsDataURL(file);
                                reader.onload = function () {
                                    let fileName = file.name
                                    let fileSize = file.size
                                    let data = reader.result.split(',')[1]

                                    if (fileSize >= maxFileSize) {
                                        return;
                                    }

                                    let newFormData = {
                                        "shippingTransId": formData.shipping_transaction_id,
                                        "activityId": formData.activity_id,
                                        "documentTypeId": formData.document_type_id,
                                        "remark": formData.remark,
                                        "quantity": formData.quantity,
                                        "quality": formData.quality,
                                        "fileName": fileName,
                                        "fileSize": fileSize,
                                        "data": data
                                    }

                                    /*//console.log(newFormData)*/

                                    $.ajax({
                                        url: `/api/${areaName}/ShippingLoadUnloadDocument/InsertData`,
                                        data: JSON.stringify(newFormData),
                                        type: "POST",
                                        contentType: "application/json",
                                        beforeSend: function (xhr) {
                                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                                        },
                                        success: function (response) {
                                            documentPopup.hide()
                                            documentDataGrid.dxDataGrid("instance").refresh()
                                        }
                                    })
                                }
                            }
                        }
                    }
                ]
            })
            e.append(formContainer)
        }
    }

    const documentPopup = $("<div>")
        .dxPopup(documentPopupOptions).appendTo("body").dxPopup("instance")

    const openDocumentPopup = function () {
        documentPopup.option("contentTemplate", documentPopupOptions.contentTemplate.bind(this));
        documentPopup.show()
    }
    $('#btnApproveSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;
            payload.isLoading = true;
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


    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');

            $.ajax({
                url: "/Port/Shipping/UnloadingExcelExport",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
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
                    a.download = "Shipping_Unloading.xlsx"; // Set the appropriate file name here
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
                url: "/api/Port/Shipping/Unloading/UploadDocument",
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