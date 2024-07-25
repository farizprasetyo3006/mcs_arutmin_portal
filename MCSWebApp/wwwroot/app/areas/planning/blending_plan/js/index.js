$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "BlendingPlan";
    var url = "/api/" + areaName + "/" + entityName;
    let isProductTabDataEntered = false;
    let isSourceTabDataEntered = false;

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("blendingplanDate1");
    var tgl2 = sessionStorage.getItem("blendingplanDate2");

    var date = new Date(), y = date.getFullYear(), m = date.getMonth(), d = date.getDate();
    var firstDay = new Date(y, m, d - 1);
    var lastDay = new Date(y, m, d, 23, 59, 59);

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
            sessionStorage.setItem("blendingplanDate1", formatTanggal(firstDay));
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
            sessionStorage.setItem("blendingplanDate2", formatTanggal(lastDay));
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
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "sales_plan_id",
                dataType: "string",
                caption: "Sales Plan Customer Detail",
                formItem: {
                    colSpan: 2
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Planning/SalesPlan/SalesPlanCustomerIdLookup",
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
                    rowData.sales_plan_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "transaction_number",
                dataType: "string",
                caption: "Transaction Number",
                allowEditing: false,
                sortOrder: "asc"
            },
            {
                dataField: "planning_category",
                dataType: "string",
                caption: "Category",
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
                            filter: ["item_group", "=", "blending-plan-category"]
                        }
                    },
                    valueExpr: 'value', // contains the same values as the "statusId" field provides
                    displayExpr: 'text' // provides display values
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            /* {
                 dataField: "accounting_period_id",
                 dataType: "text",
                 caption: "Accounting Period",
                 visible: false,
                 lookup: {
                     dataSource: DevExpress.data.AspNet.createStore({
                         key: "value",
                         loadUrl: "/api/Accounting/AccountingPeriod/AccountingPeriodIdLookup",
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
                 dataField: "process_flow_id",
                 dataType: "text",
                 caption: "Process Flow",
                 visible: false,
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
                 calculateSortValue: function (data) {
                     var value = this.calculateCellValue(data);
                     return this.lookup.calculateCellValue(value);
                 }
             },*/
            {
                dataField: "product_id",
                dataType: "text",
                caption: "Product",
                width: "160px",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Material/Product/ProductIdLookup",
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
                dataField: "unloading_datetime",
                dataType: "date",
                caption: "Plan Date"
            },
            {
                dataField: "source_shift_id",
                dataType: "text",
                caption: "Shift",
                visible: false,
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
                    displayExpr: "text"
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "despatch_order_id",
                dataType: "text",
                caption: "Shipping Order",

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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "destination_location_id",
                dataType: "text",
                caption: "Destination Location",
                /*validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],*/
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/StockpileLocationIdLookup",
                        //loadUrl: "/api/Transport/Vessel/VesselBargeIdLookup",
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
                dataField: "sumprod_volume",
                dataType: "number",
                caption: "Volume",
                format: {
                    type: "fixedPoint",
                    precision: 4
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_volume !== null) {
                        return rowData.sumprod_volume;
                    } else {
                        return rowData.sumprod_volume_product;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_1",
                dataType: "number",
                caption: "TM",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_1 !== null) {
                        return rowData.sumprod_analyte_1;
                    } else {
                        return rowData.sumprod_analyte_product_1;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_2",
                dataType: "number",
                caption: "IM",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_2 !== null) {
                        return rowData.sumprod_analyte_2;
                    } else {
                        return rowData.sumprod_analyte_product_2;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_3",
                dataType: "number",
                caption: "AC",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_3 !== null) {
                        return rowData.sumprod_analyte_3;
                    } else {
                        return rowData.sumprod_analyte_product_3;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_4",
                dataType: "number",
                caption: "VM",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_4 !== null) {
                        return rowData.sumprod_analyte_4;
                    } else {
                        return rowData.sumprod_analyte_product_4;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_5",
                dataType: "number",
                caption: "FC",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_5 !== null) {
                        return rowData.sumprod_analyte_5;
                    } else {
                        return rowData.sumprod_analyte_product_5;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_6",
                dataType: "number",
                caption: "TS",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_6 !== null) {
                        return rowData.sumprod_analyte_6;
                    } else {
                        return rowData.sumprod_analyte_product_6;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_7",
                dataType: "number",
                caption: "CV (adb)",
                format: "fixedPoint",
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_7 !== null) {
                        return rowData.sumprod_analyte_7;
                    } else {
                        return rowData.sumprod_analyte_product_7;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_8",
                dataType: "number",
                caption: "CV (ar)",
                format: "fixedPoint",
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_8 !== null) {
                        return rowData.sumprod_analyte_8;
                    } else {
                        return rowData.sumprod_analyte_product_8;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_9",
                dataType: "number",
                caption: "RD",
                format: "fixedPoint",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_9 !== null) {
                        return rowData.sumprod_analyte_9;
                    } else {
                        return rowData.sumprod_analyte_product_9;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_10",
                dataType: "number",
                caption: "RDI",
                format: "fixedPoint",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_10 !== null) {
                        return rowData.sumprod_analyte_10;
                    } else {
                        return rowData.sumprod_analyte_product_10;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "sumprod_analyte_11",
                dataType: "number",
                caption: "HGI",
                format: "fixedPoint",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                calculateCellValue: function (rowData) {
                    if (rowData.sumprod_analyte_11 !== null) {
                        return rowData.sumprod_analyte_11;
                    } else {
                        return rowData.sumprod_analyte_product_11;
                    }
                },
                formItem: {
                    visible: false
                }
            },
            /*{
                dataField: "unloading_quantity",
                dataType: "number",
                format: "fixedPoint",
                caption: "Quantity"
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
                    displayExpr: "text"
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "survey_id",
                dataType: "text",
                caption: "Quality Sampling",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/QualitySamplingIdLookup",
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
            }*/
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
        height: 800,
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
        onInitNewRow: function (e) {
            e.data.is_draft_survey = false;
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
        },
        onEditorPreparing: function (e) {
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                let rowData = e.row.data;

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                    let despatchId = e.value;

                    grid.beginCustomLoading();
                    $.ajax({
                        url: "/api/" + areaName + "/" + "BlendingPlanProduct" + '/DespatchOrderDetail?Id=' + despatchId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let hasil = response.data[0];

                            grid.beginUpdate();
                            grid.cellValue(index, "unloading_datetime", hasil.laycan_start);
                            grid.cellValue(index, "destination_location_id", hasil.vessel_id);
                            grid.endUpdate();
                        }
                    })

                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500);

                    standardHandler(e); // Calling the standard handler to save the edited value
                   
                }
            }
        }
    });

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                title: "Sources",
                template: createSourcesTabTemplate(masterDetailOptions.data)
                },
                {
                    title: "Product",
                    template: createProductTabTemplate(masterDetailOptions.data)
                }
            ]
        });
    }
    function createProductTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "BlendingPlanProduct";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByBlendingPlanId/" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/InsertData",
                        updateUrl: urlDetail + "/UpdateData",
                        deleteUrl: urlDetail + "/DeleteData",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    /*onInitialized: function (e) {
                        const productDataSource = e.component.getDataSource();
                        productDataSource.on("changed", function (e) {
                            if (e.changeType === "insert" || e.changeType === "update") {
                                isProductTabDataEntered = true;
                            }
                        });
                    },
                    editing: {
                        mode: "form",
                        allowAdding: true,
                        allowUpdating: true,
                        allowDeleting: true,
                        useIcons: true,
                        // Add the onSaving event handler
                        onSaving: function (e) {
                            if (!isProductTabDataEntered) {
                                e.cancel = true; // Prevent saving if no data is entered
                                // Display an error message or take other actions
                                alert("Please enter data in the Product tab before saving.");
                            }
                        }
                    },*/
                    remoteOperations: true,
                    allowColumnResizing: true,
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        {
                            dataField: "blending_plan_id",
                            caption: "Blending Plan Id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            },
                            formItem: {
                                visible: false
                            }
                        },
                        {
                            dataField: "product_id",
                            dataType: "string",
                            caption: "Product Spesification",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/ProductIdLookup",
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
                                rowData.product_id = value;
                                //rowData.sampling_number = null;
                            },
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                        },
                        /*{
                            dataField: "volume",
                            dataType: "string",
                            caption: "Volume",
                            formItem: {
                                colSpan: 2,
                                editorType: "dxTextArea"
                            }
                        },*/
                        {
                            dataField: "volume",
                            dataType: "number",
                            caption: "Volume (MT)",
                            //format: "fixedPoint",
                            format: {
                                type: "fixedPoint",
                                precision: 4
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },
                        {
                            dataField: "analyte_1",
                            dataType: "number",
                            caption: "TM (%ar)",
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
                                        precision: 2,
                                    }
                                }
                            }
                        },
                        {
                            dataField: "analyte_2",
                            dataType: "number",
                            caption: "IM (%adb)",
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
                            dataField: "analyte_3",
                            dataType: "number",
                            caption: "Ash (%adb)",
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
                            dataField: "analyte_4",
                            dataType: "number",
                            caption: "VM (%adb)",
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
                            dataField: "analyte_5",
                            dataType: "number",
                            caption: "FC (%adb)",
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
                            dataField: "analyte_6",
                            dataType: "number",
                            caption: "TS (%adb)",
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
                            dataField: "analyte_7",
                            dataType: "number",
                            caption: "CV Kcal/Kg (adb)",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },
                        {
                            dataField: "analyte_8",
                            dataType: "number",
                            caption: "CV Kcal/Kg (ar)",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },
                        {
                            dataField: "analyte_9",
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
                            }
                        },
                        {
                            dataField: "analyte_10",
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
                            }
                        },
                        {
                            dataField: "analyte_11",
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
                            }
                        },
                        {
                            dataField: "note",
                            dataType: "string",
                            caption: "Note",
                            formItem: {
                                colSpan: 2,
                                editorType: "dxTextArea"
                            }
                        },
                        /*{
                            dataField: "equipment_id",
                            dataType: "text",
                            caption: "Equipment",
                            visible: false,
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Equipment/EquipmentList/EquipmentIdLookup",
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
                            },
                        },
                        {
                            dataField: "transport_id",
                            dataType: "string",
                            caption: "Transport",
                            visible: false,
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Transport/Vessel/VesselBargeIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                    }
                                },
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
                            },
                        },*/
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
                        e.data.blending_plan_id = currentRecord.id;
                    },
                    onRowInserted: function (e) {
                        $.ajax({
                            url: urlDetail + "/GetTotalAnalyteByBlendingPlanId/" + currentRecord.id + "",
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                currentRecord.sumprod_volume = response.sumprod_volume_product || 0;
                                currentRecord.sumprod_analyte_1 = response.sumprod_analyte_product_1 || 0;
                                currentRecord.sumprod_analyte_2 = response.sumprod_analyte_product_2 || 0;
                                currentRecord.sumprod_analyte_3 = response.sumprod_analyte_product_3 || 0;
                                currentRecord.sumprod_analyte_4 = response.sumprod_analyte_product_4 || 0;
                                currentRecord.sumprod_analyte_5 = response.sumprod_analyte_product_5 || 0;
                                currentRecord.sumprod_analyte_6 = response.sumprod_analyte_product_6 || 0;
                                currentRecord.sumprod_analyte_7 = response.sumprod_analyte_product_7 || 0;
                                currentRecord.sumprod_analyte_8 = response.sumprod_analyte_product_8 || 0;
                                currentRecord.sumprod_analyte_9 = response.sumprod_analyte_product_9 || 0;
                                currentRecord.sumprod_analyte_10 = response.sumprod_analyte_product_10 || 0;
                                currentRecord.sumprod_analyte_11 = response.sumprod_analyte_product_11 || 0;

                                setTimeout(function () {
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();
                                }, 100);
                            }
                        })

                    },
                   /* onRowUpdated: function (e) {
                        handleRowEvent(e);
                    },
                    onRowInserted: function (e) {
                        handleRowEvent(e);
                    },*/
                    onRowRemoved: function (e) {
                        $.ajax({
                            url: urlDetail + "/GetTotalAnalyteByBlendingPlanId/" + currentRecord.id,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                currentRecord.sumprod_volume = response.sumprod_volume_product || 0;
                                currentRecord.sumprod_analyte_1 = response.sumprod_analyte_product_1 || 0;
                                currentRecord.sumprod_analyte_2 = response.sumprod_analyte_product_2 || 0;
                                currentRecord.sumprod_analyte_3 = response.sumprod_analyte_product_3 || 0;
                                currentRecord.sumprod_analyte_4 = response.sumprod_analyte_product_4 || 0;
                                currentRecord.sumprod_analyte_5 = response.sumprod_analyte_product_5 || 0;
                                currentRecord.sumprod_analyte_6 = response.sumprod_analyte_product_6 || 0;
                                currentRecord.sumprod_analyte_7 = response.sumprod_analyte_product_7 || 0;
                                currentRecord.sumprod_analyte_8 = response.sumprod_analyte_product_8 || 0;
                                currentRecord.sumprod_analyte_9 = response.sumprod_analyte_product_9 || 0;
                                currentRecord.sumprod_analyte_10 = response.sumprod_analyte_product_10 || 0;
                                currentRecord.sumprod_analyte_11 = response.sumprod_analyte_product_11 || 0;

                                setTimeout(function () {
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();
                                }, 100);
                            }
                        })

                    },

                    onRowUpdated: function (e) {
                        $.ajax({
                            url: urlDetail + "/GetTotalAnalyteByBlendingPlanId/" + currentRecord.id,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                currentRecord.sumprod_volume = response.sumprod_volume_product || 0;
                                currentRecord.sumprod_analyte_1 = response.sumprod_analyte_product_1 || 0;
                                currentRecord.sumprod_analyte_2 = response.sumprod_analyte_product_2 || 0;
                                currentRecord.sumprod_analyte_3 = response.sumprod_analyte_product_3 || 0;
                                currentRecord.sumprod_analyte_4 = response.sumprod_analyte_product_4 || 0;
                                currentRecord.sumprod_analyte_5 = response.sumprod_analyte_product_5 || 0;
                                currentRecord.sumprod_analyte_6 = response.sumprod_analyte_product_6 || 0;
                                currentRecord.sumprod_analyte_7 = response.sumprod_analyte_product_7 || 0;
                                currentRecord.sumprod_analyte_8 = response.sumprod_analyte_product_8 || 0;
                                currentRecord.sumprod_analyte_9 = response.sumprod_analyte_product_9 || 0;
                                currentRecord.sumprod_analyte_10 = response.sumprod_analyte_product_10 || 0;
                                currentRecord.sumprod_analyte_11 = response.sumprod_analyte_product_11 || 0;

                                setTimeout(function () {
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();
                                }, 100);
                            }
                        })

                    },
                    onEditorPreparing: function (e) {
                        if (e.parentType === "dataRow" && e.dataField == "product_id") {
                            let standardHandler = e.editorOptions.onValueChanged;
                            let index = e.row.rowIndex;
                            let grid = e.component;
                            let rowData = e.row.data;

                            e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                                let productId = e.value;

                                grid.beginCustomLoading();
                                $.ajax({
                                    url: urlDetail + '/AnalyteByProductId?Id=' + productId,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        let analyteData = response;

                                        grid.beginUpdate();
                                        grid.cellValue(index, "product_id", analyteData.id)
                                        grid.cellValue(index, "analyte_1", analyteData.tm);
                                        grid.cellValue(index, "analyte_2", analyteData.im);
                                        grid.cellValue(index, "analyte_3", analyteData.ash);
                                        grid.cellValue(index, "analyte_4", analyteData.vm);
                                        grid.cellValue(index, "analyte_5", analyteData.fc);
                                        grid.cellValue(index, "analyte_6", analyteData.ts);
                                        grid.cellValue(index, "analyte_7", analyteData.gcv_adb);
                                        grid.cellValue(index, "analyte_8", analyteData.gcv_ar);
                                        grid.cellValue(index, "analyte_9", analyteData.rd);
                                        grid.cellValue(index, "analyte_10", analyteData.rdi);
                                        grid.cellValue(index, "analyte_11", analyteData.hgi);
                                        grid.endUpdate();
                                    }
                                })

                                setTimeout(() => {
                                    grid.endCustomLoading()
                                }, 500);

                                standardHandler(e); // Calling the standard handler to save the edited value
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
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailName + '.xlsx');
                            });
                        });
                        e.cancel = true;
                    }
                });
        }
    }
    function createSourcesTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "BlendingPlanSource";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByBlendingPlanId/" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/InsertData",
                        updateUrl: urlDetail + "/UpdateData",
                        deleteUrl: urlDetail + "/DeleteData",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                   /* onInitialized: function (e) {
                        const sourceDataSource = e.component.getDataSource();
                        sourceDataSource.on("changed", function (e) {
                            if (e.changeType === "insert" || e.changeType === "update") {
                                isSourceTabDataEntered = true;
                            }
                        });
                    },*/
                    remoteOperations: true,
                    allowColumnResizing: true,
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        {
                            dataField: "blending_plan_id",
                            caption: "Blending Plan Id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            },
                            formItem: {
                                visible: false
                            }
                        },
                        {
                            dataField: "spec_ts",
                            dataType: "number",
                            caption: "SPEC TS",
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
                            dataField: "source_location_id",
                            dataType: "string",
                            caption: "Source Location",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/SourceLocationIdLookup",
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
                                rowData.source_location_id = value;
                                rowData.sampling_number = null;
                            },
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                        },
                        {
                            dataField: "sampling_number",
                            dataType: "string",
                            caption: "Sampling Number",
                            lookup: {
                                dataSource: function (options) {
                                    var _url = urlDetail + "/SamplingNumberBySourceLocationId";
                                    if (options !== undefined && options !== null) {
                                        if (options.data !== undefined && options.data !== null) {
                                            if (options.data.source_location_id !== undefined
                                                && options.data.source_location_id !== null) {
                                                _url += "?Id=" + encodeURIComponent(options.data.source_location_id);
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
                            setCellValue: function (rowData, value) {
                                rowData.sampling_number = value;
                            },
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                        },
                        {
                            dataField: "ikh_pit_id",
                            dataType: "string",
                            caption: "IKH Pit",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/TransactionNumberLookup",
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
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                            setCellValue: function (rowData, value) {
                                rowData.ikh_pit_id = value;
                                rowData.analyte_1 = null;
                                rowData.analyte_2 = null;
                                rowData.analyte_3 = null;
                                rowData.analyte_4 = null;
                                rowData.analyte_5 = null;
                                rowData.analyte_6 = null;
                                rowData.analyte_7 = null;
                                rowData.analyte_8 = null;
                            },
                        },
                        {
                            dataField: "note",
                            dataType: "string",
                            caption: "Note",
                            formItem: {
                                colSpan: 2,
                                editorType: "dxTextArea"
                            }
                        },
                        {
                            dataField: "volume",
                            dataType: "number",
                            caption: "Volume",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },
                        {
                            dataField: "analyte_1",
                            dataType: "number",
                            caption: "TM",
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
                            dataField: "analyte_2",
                            dataType: "number",
                            caption: "IM",
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
                            dataField: "analyte_3",
                            dataType: "number",
                            caption: "AC",
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
                            dataField: "analyte_4",
                            dataType: "number",
                            caption: "VM",
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
                            dataField: "analyte_5",
                            dataType: "number",
                            caption: "FC",
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
                            dataField: "analyte_6",
                            dataType: "number",
                            caption: "TS",
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
                            dataField: "analyte_7",
                            dataType: "number",
                            caption: "CV (adb)",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },
                        {
                            dataField: "analyte_8",
                            dataType: "number",
                            caption: "CV (ar)",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },

                        {
                            dataField: "analyte_9",
                            dataType: "number",
                            caption: "RD",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },
                        {
                            dataField: "analyte_10",
                            dataType: "number",
                            caption: "RDI",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        },
                        {
                            dataField: "analyte_11",
                            dataType: "number",
                            caption: "HGI",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            }
                        }, 
                        {
                            dataField: "equipment_id",
                            dataType: "text",
                            caption: "Equipment",
                            visible: false,
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Equipment/EquipmentList/EquipmentIdLookup",
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
                            },
                        },
                        {
                            dataField: "transport_id",
                            dataType: "string",
                            caption: "Transport",
                            visible: false,
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Transport/Vessel/VesselBargeIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                    }
                                },
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
                            },            
                        },

                        /* {
                            dataField: "loading_datetime",
                            dataType: "date",
                            caption: "Plan Date"
                        },
                        {
                            dataField: "loading_quantity",
                            dataType: "number",
                            caption: "Quantity",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                }
                            },
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }]
                        },*/
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
                        e.data.blending_plan_id = currentRecord.id;
                    },
                    onRowInserted: function (e) {
                         $.ajax({
                            url: urlDetail + "/GetTotalAnalyteByBlendingPlanId/" + currentRecord.id,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                currentRecord.sumprod_volume = response.sumprod_volume || 0;
                                currentRecord.sumprod_analyte_1 = response.sumprod_analyte_1 || 0;
                                currentRecord.sumprod_analyte_2 = response.sumprod_analyte_2 || 0;
                                currentRecord.sumprod_analyte_3 = response.sumprod_analyte_3 || 0;
                                currentRecord.sumprod_analyte_4 = response.sumprod_analyte_4 || 0;
                                currentRecord.sumprod_analyte_5 = response.sumprod_analyte_5 || 0;
                                currentRecord.sumprod_analyte_6 = response.sumprod_analyte_6 || 0;
                                currentRecord.sumprod_analyte_7 = response.sumprod_analyte_7 || 0;
                                currentRecord.sumprod_analyte_8 = response.sumprod_analyte_8 || 0;
                                currentRecord.sumprod_analyte_9 = response.sumprod_analyte_9 || 0;
                                currentRecord.sumprod_analyte_10 = response.sumprod_analyte_10 || 0;
                                currentRecord.sumprod_analyte_11 = response.sumprod_analyte_11 || 0;

                                setTimeout(function () {
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();
                                }, 100);
                            }
                        })

                    },
                    onRowRemoved: function (e) {
                        $.ajax({
                            url: urlDetail + "/GetTotalAnalyteByBlendingPlanId/" + currentRecord.id,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                currentRecord.sumprod_volume = response.sumprod_volume || 0;
                                currentRecord.sumprod_analyte_1 = response.sumprod_analyte_1 || 0;
                                currentRecord.sumprod_analyte_2 = response.sumprod_analyte_2 || 0;
                                currentRecord.sumprod_analyte_3 = response.sumprod_analyte_3 || 0;
                                currentRecord.sumprod_analyte_4 = response.sumprod_analyte_4 || 0;
                                currentRecord.sumprod_analyte_5 = response.sumprod_analyte_5 || 0;
                                currentRecord.sumprod_analyte_6 = response.sumprod_analyte_6 || 0;
                                currentRecord.sumprod_analyte_7 = response.sumprod_analyte_7 || 0;
                                currentRecord.sumprod_analyte_8 = response.sumprod_analyte_8 || 0;
                                currentRecord.sumprod_analyte_9 = response.sumprod_analyte_9 || 0;
                                currentRecord.sumprod_analyte_10 = response.sumprod_analyte_10 || 0;
                                currentRecord.sumprod_analyte_11 = response.sumprod_analyte_11 || 0;

                                setTimeout(function () {
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();
                                }, 100);
                            }
                        })

                    },
                    onRowUpdated: function (e) {
                        $.ajax({
                            url: urlDetail + "/GetTotalAnalyteByBlendingPlanId/" + currentRecord.id,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                currentRecord.sumprod_volume = response.sumprod_volume || 0;
                                currentRecord.sumprod_analyte_1 = response.sumprod_analyte_1 || 0;
                                currentRecord.sumprod_analyte_2 = response.sumprod_analyte_2 || 0;
                                currentRecord.sumprod_analyte_3 = response.sumprod_analyte_3 || 0;
                                currentRecord.sumprod_analyte_4 = response.sumprod_analyte_4 || 0;
                                currentRecord.sumprod_analyte_5 = response.sumprod_analyte_5 || 0;
                                currentRecord.sumprod_analyte_6 = response.sumprod_analyte_6 || 0;
                                currentRecord.sumprod_analyte_7 = response.sumprod_analyte_7 || 0;
                                currentRecord.sumprod_analyte_8 = response.sumprod_analyte_8 || 0;
                                currentRecord.sumprod_analyte_9 = response.sumprod_analyte_9 || 0;
                                currentRecord.sumprod_analyte_10 = response.sumprod_analyte_10 || 0;
                                currentRecord.sumprod_analyte_11 = response.sumprod_analyte_11 || 0;

                                setTimeout(function () {
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();
                                }, 100);
                            }
                        })

                    },
                    onEditorPreparing: function (e) {
                        if (e.parentType === "dataRow" && e.dataField == "source_location_id") {
                            let standardHandler = e.editorOptions.onValueChanged;
                            let index = e.row.rowIndex;
                            let grid = e.component;
                            let rowData = e.row.data;

                            e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                                let sourceLocationId = e.value;

                                grid.beginCustomLoading();
                                $.ajax({
                                    url: urlDetail + '/AnalyteBySourceLocationId?Id=' + sourceLocationId,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        let analyteData = response;

                                        grid.beginUpdate();
                                        grid.cellValue(index, "analyte_1", analyteData.tm);
                                        grid.cellValue(index, "analyte_2", analyteData.im);
                                        grid.cellValue(index, "analyte_3", analyteData.ash);
                                        grid.cellValue(index, "analyte_4", analyteData.vm);
                                        grid.cellValue(index, "analyte_5", analyteData.fc);
                                        grid.cellValue(index, "analyte_6", analyteData.ts);
                                        grid.cellValue(index, "analyte_7", analyteData.gcv_adb);
                                        grid.cellValue(index, "analyte_8", analyteData.gcv_ar);
                                        grid.endUpdate();
                                    }
                                })

                                setTimeout(() => {
                                    grid.endCustomLoading()
                                }, 500);

                                standardHandler(e); // Calling the standard handler to save the edited value
                            }
                        }
                        if (e.parentType === "dataRow" && e.dataField == "sampling_number") {
                            let standardHandler = e.editorOptions.onValueChanged;
                            let index = e.row.rowIndex;
                            let grid = e.component;
                            let rowData = e.row.data;

                            e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                                // Get its value (Id) on value changed
                                let samplingNumber = e.value;

                                grid.beginCustomLoading();

                                // Get another data from API after getting the Id
                                $.ajax({
                                    url: urlDetail + '/AnalyteBySamplingNumber?SamplingNumber=' + samplingNumber,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        let analyteData = response;

                                        grid.beginUpdate();
                                        grid.cellValue(index, "analyte_1", analyteData.tm);
                                        grid.cellValue(index, "analyte_2", analyteData.im);
                                        grid.cellValue(index, "analyte_3", analyteData.ash);
                                        grid.cellValue(index, "analyte_4", analyteData.vm);
                                        grid.cellValue(index, "analyte_5", analyteData.fc);
                                        grid.cellValue(index, "analyte_6", analyteData.ts);
                                        grid.cellValue(index, "analyte_7", analyteData.gcv_adb);
                                        grid.cellValue(index, "analyte_8", analyteData.gcv_ar);
                                        grid.endUpdate();
                                    }
                                })

                                setTimeout(() => {
                                    grid.endCustomLoading()
                                }, 500);

                                standardHandler(e); // Calling the standard handler to save the edited value
                            }
                        }
                        if (e.parentType === "dataRow" && e.dataField == "ikh_pit_id") {
                            let standardHandler = e.editorOptions.onValueChanged;
                            let index = e.row.rowIndex;
                            let grid = e.component;
                            let rowData = e.row.data;

                            e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                                // Get its value (Id) on value changed
                                let IKHPIT = e.value;

                                grid.beginCustomLoading();

                                // Get another data from API after getting the Id
                                $.ajax({
                                    url: '/api/Planning/BlendingPlan/DataDetail?Id=' + IKHPIT,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        let BlendingPlanData = response.data[0];

                                        grid.beginUpdate();
                                        grid.cellValue(index, "volume", BlendingPlanData.sumprod_volume);
                                        grid.cellValue(index, "analyte_1", BlendingPlanData.sumprod_analyte_1);
                                        grid.cellValue(index, "analyte_2", BlendingPlanData.sumprod_analyte_2);
                                        grid.cellValue(index, "analyte_3", BlendingPlanData.sumprod_analyte_3);
                                        grid.cellValue(index, "analyte_4", BlendingPlanData.sumprod_analyte_4);
                                        grid.cellValue(index, "analyte_5", BlendingPlanData.sumprod_analyte_5);
                                        grid.cellValue(index, "analyte_6", BlendingPlanData.sumprod_analyte_6);
                                        grid.cellValue(index, "analyte_7", BlendingPlanData.sumprod_analyte_7);
                                        grid.cellValue(index, "analyte_8", BlendingPlanData.sumprod_analyte_8);
                                        grid.endUpdate();
                                    }
                                })

                                setTimeout(() => { grid.endCustomLoading() }, 500);

                                standardHandler(e); // Calling the standard handler to save the edited value
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
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailName + '.xlsx');
                            });
                        });
                        e.cancel = true;
                    }
                });
        }
    }

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
                url: "/api/Planning/BlendingPlan/UploadDocument",
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