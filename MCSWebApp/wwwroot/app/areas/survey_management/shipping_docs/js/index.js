$(function () {

    var token = $.cookie("Token");
    var areaName = "SurveyManagement";
    var entityName = "COW";
    var url = "/api/" + areaName + "/" + entityName;
    const maxFileSize = 52428800;
    var COWData;
    var customer = null;

    var despatchOrderIdDat = "";
    var $mainGrid = $("#grid").dxDataGrid({
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
            {
                dataField: "survey_number",
                dataType: "string",
                caption: "COW Number",
                sortIndex: 0,
                sortOrder: "desc",
                sortOrder: "asc",
                visible: false
            },
            {
                dataField: "survey_date",
                dataType: "date",
                caption: "COW Date",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "bill_lading_date",
                dataType: "date",
                caption: "Bill of Lading Date"
            },
            {
                dataField: "bill_lading_number",
                dataType: "string",
                visible: false,
                caption: "Bill of Lading Number",
            },
            {
                dataField: "surveyor_id",
                dataType: "text",
                visible: false,
                caption: "Surveyor",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        //loadUrl: url + "/SurveyorIdLookup",
                        loadUrl: "/api/Organisation/Contractor/ContractorIdLookupByIsSurveyor?isSurveyor=true",
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
                dataField: "stock_location_id",
                dataType: "text",
                visible: false,
                caption: "Stock Location",
                visible: false,
                formItem: {
                    visible: false
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/StockLocationIdLookup",
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
                dataField: "product_id",
                dataType: "text",
                visible: false,
                caption: "Brand",
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
                },
            },
            {
                dataField: "wlcr",
                visible: false,
                dataType: "date",
                caption: "Workable LC Received",
            },
            {
                dataField: "quantity",
                dataType: "numeric",
                caption: "Quantity",
                format: {
                    //type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            //type: "fixedPoint",
                            precision: 3
                        }
                    }
                },
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                allowEditing: true
            },
            {
                dataField: "uom_id",
                dataType: "text",
                visible: false,
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
                },
                width: "100px"
            },
            //{
            //    dataField: "sampling_template_id",
            //    dataType: "text",
            //    caption: "Sampling Template",
            //    validationRules: [{
            //        type: "required",
            //        message: "This field is required."
            //    }],
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: url + "/SamplingTemplateIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    }
            //}
            //{
            //    dataField: "quality_sampling_id",
            //    dataType: "text",
            //    caption: "Quality Sampling",
            //    visible: false,
            //    lookup: {
            //        dataSource: function (options) {
            //            return {
            //                store: DevExpress.data.AspNet.createStore({
            //                    key: "value",
            //                    loadUrl: url + "/QualitySamplingIdLookup",
            //                    onBeforeSend: function (method, ajaxOptions) {
            //                        ajaxOptions.xhrFields = { withCredentials: true };
            //                        ajaxOptions.beforeSend = function (request) {
            //                            request.setRequestHeader("Authorization", "Bearer " + token);
            //                        };
            //                    }
            //                })
            //            }
            //        },
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    calculateSortValue: function (data) {
            //        var value = this.calculateCellValue(data);
            //        return this.lookup.calculateCellValue(value);
            //    },
            //},
            {
                dataField: "despatch_order_id",
                dataType: "text",
                visible: false,
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
                                loadUrl: url + "/DespatchOrderIdLookup?id=" + encodeURIComponent(despatchOrderIdDat),
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
                    rowData.despatch_order_id = value;
                }
            },
            {
                dataField: "despatch_order_link",
                dataType: "string",
                caption: "Shipping Order Detail",
                visible: false,
                allowFiltering: false
            },
            {
                dataField: "coa_link",
                dataType: "string",
                caption: "Coa Detail",
                visible: false,
                allowFiltering: false
            },
            {
                dataField: "lc_number",
                dataType: "string",
                caption: "LC Number"
            },
            {
                dataField: "transport_id",
                dataType: "string",
                caption: "Vessel/Barge Name",
                visible: true,
                allowEditing: false
            },
            {
                dataField: "peb",
                dataType: "string",
                caption: "HS Code",
                visible: false,
            },
            {
                dataField: "description",
                dataType: "string",
                caption: "Description",
                visible: false,
            },
            /*{
                dataField: "draught_survey",
                dataType: "string",
                caption: "Draught Survey",
                visible: true,
            },*/
            {
                dataField: "royalty",
                dataType: "string",
                caption: "Royalty Number",
                visible: false,
            },
            {
                dataField: "coo_goverment",
                dataType: "string",
                caption: "COO Government",
                visible: false,
            },
            {
                dataField: "invoice",
                dataType: "string",
                caption: "Invoice Number",
                visible: false,
            },
            {
                dataField: "non_commercial",
                dataType: "boolean",
                caption: "Non Commercial",
                visible: false,
            },
            {
                dataField: "customer_id",
                dataType: "string",
                caption: "Customer",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/Customer/CustomerIdLookup",
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
                allowEditing: false
            },
            {
                dataField: "coa_id",
                dataType: "string",
                caption: "COA",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/StockpileManagement/QualitySampling/QualitySamplingIdLookupNoFilterWithShippingOrder",
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
                allowEditing: true
            },
            {
                dataField: "ds_issued_date",
                dataType: "date",
                visible: false,
                caption: "D/S Issued Date",
            },
            {
                dataField: "draught_survey_issued",
                dataType: "date",
                visible: false,
                caption: "Draught Survey Issued",
            },
            {
                dataField: "sof_id",
                dataType: "string",
                visible: false,
                caption: "SOF Number",
                //visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Port/StatementOfFact/StatemenfOfFactIdLookup",
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
                allowEditing: true
            },
            {
                dataField: "sof_link",
                dataType: "string",
                caption: "SOF Detail",
                visible: false,
                allowFiltering: false
            },
            {
                dataField: "sof_issued_date",
                dataType: "date",
                visible: false,
                caption: "SOF Issued Date",
            },
            {
                caption: "Update Number",
                type: "buttons",
                visible: false,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    text: "Update Number",
                    onClick: function (e) {
                        COWData = e.row.data;
                        showUpdateSurveyNumberPopup();
                    }
                }]
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                allowEditing: false,
                visible: false,
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
                }
            },
            {
                dataField: "lc_price",
                dataType: "numeric",
                caption: "LC Price",
                visible: false
            },
            {
                dataField: "lc_date",
                dataType: "date",
                visible: false,
                caption: "LC Date"
            },
            {
                dataField: "lc_amount",
                dataType: "numeric",
                caption: "LC Amount",
                visible: false
            },
            {
                dataField: "issuing_bank",
                dataType: "string",
                caption: "Issuing Bank",
                visible: false
            },
            {
                dataField: "advising_bank",
                dataType: "string",
                caption: "LC Coal Price Index",
                visible: false
            },
            {
                dataField: "peb_request_number",
                dataType: "string",
                visible: false,
                caption: "Nomor aju PEB"
            },
            {
                dataField: "peb_number",
                dataType: "string",
                visible: false,
                caption: "PEB Number"
            },
            {
                dataField: "peb_date",
                dataType: "date",
                visible: false,
                caption: "PEB Date"
            },
            {
                dataField: "pod_on_peb",
                dataType: "string",
                visible: false,
                caption: "POD on PEB"
            },
            {
                dataField: "country_id",
                dataType: "string",
                caption: "Country",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/Country/CountryIdLookup", // API for Country
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
                editorOptions: {
                    onOpened: function (e) {
                        renderAddNewButton("/General/Country/Index")

                        // Always reload dataSource onOpenned to get new data
                        let lookup = e.component
                        lookup._dataSource.reload()
                    }
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                setCellValue: function (rowData, value) {
                    rowData.country_id = value
                }
            },
            {
                dataField: "customs_office",
                dataType: "string",
                caption: "Kantor BEA Cukai",
                visible: false,
            },
            {
                dataField: "cohc",
                dataType: "string",
                visible: false,
                caption: "COHC",
            },
            {
                dataField: "packing_list",
                dataType: "string",
                visible: false,
                caption: "Packing List",
            },
            /* {
                 dataField: "nor_tendered",
                 dataType: "datetime",
                 caption: "NOR Tendered",
                 format: "yyyy-MM-dd HH:mm:ss",
             },
             {
                 dataField: "nor_accepted",
                 dataType: "datetime",
                 caption: "NOR Accepted",
                 format: "yyyy-MM-dd HH:mm:ss",
             },*/
            {
                type: "buttons",
                buttons: ["edit", "delete"],
                showInColumnChooser: false
            }
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
            mode: "form",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 1,
                items: [
                    {
                        itemType: "group",
                        caption: "Shipping Docs",
                        colCount: 2,
                        items: [
                            {
                                dataField: "despatch_order_id",
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
                                dataField: "transport_id",
                            },
                            {
                                dataField: "customer_id",
                            },
                            {
                                dataField: "surveyor_id",
                            },
                            {
                                dataField: "product_id",
                            },
                            {
                                dataField: "wlcr",
                            },
                            {
                                dataField: "business_unit_id",
                            }
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "B/L",
                        colCount: 2,
                        items: [

                            {
                                dataField: "bill_lading_number",
                            },
                            {
                                dataField: "bill_lading_date",
                            },
                            {
                                dataField: "quantity",
                            },
                            {
                                dataField: "uom_id",
                            },
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "Surveyor Certificates",
                        colCount: 2,
                        items: [
                            {
                                dataField: "coa_id",
                            },
                            {
                                dataField: "survey_number",
                            },
                            {
                                dataField: "survey_date",
                            },
                            /*{
                                dataField: "coa_link",
                                editorType: "dxButton",
                                editorOptions: {
                                    text: "See COA Detail",
                                    disabled: true
                                }
                            },*/
                            {
                                dataField: "ds_issued_date",
                            },
                        ]
                    },
                    /*{
                        itemType: "group",
                        caption: "SOF",
                        colCount: 2,
                        items: [
                            *//*{
dataField: "sof_id",
},*//*
                                                                                                       *//* {
dataField: "sof_link",
editorType: "dxButton",
editorOptions: {
text: "See SOF Detail",
disabled: true
}
},*//*
                                                          *//*{
                  dataField: "sof_issued_date",
              },*//*
{
dataField: "nor_tendered"
},
{
dataField: "nor_accepted"
},


]
},*/
                    {
                        itemType: "group",
                        caption: "Docs",
                        colCount: 2,
                        items: [

                            {
                                dataField: "coo_goverment",
                            },
                            {
                                dataField: "royalty",
                            },
                            {
                                dataField: "invoice",
                            },
                            /* {
                                 dataField: "draught_survey",
                             },*/
                            {
                                dataField: "draught_survey_issued",
                            },

                        ]
                    },
                    {
                        itemType: "group",
                        caption: "LC",
                        colCount: 2,
                        items: [
                            {
                                dataField: "lc_price",
                            },
                            {
                                dataField: "lc_number",
                            },
                            {
                                dataField: "lc_date",
                            },
                            {
                                dataField: "lc_amount",
                            },
                            {
                                dataField: "issuing_bank",
                            },
                            {
                                dataField: "advising_bank",
                            },
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "PEB",
                        colCount: 2,
                        items: [
                            {
                                dataField: "peb",
                            },
                            {
                                dataField: "description",
                            },
                            {
                                dataField: "peb_request_number",
                            },
                            {
                                dataField: "peb_number",
                            },
                            {
                                dataField: "peb_date",
                            },
                            {
                                dataField: "pod_on_peb",
                            },
                            {
                                dataField: "country_id",
                            },
                            {
                                dataField: "customs_office",
                            },
                        ]
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
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_link") {
                if (e.row.data.despatch_order_id) {
                    let despatchOrderId = e.row.data.despatch_order_id

                    e.editorOptions.onClick = function (e) {
                        window.open("/Sales/DespatchOrder/Index?Id=" + despatchOrderId + "&openEditingForm=true", "_blank")
                    }
                    e.editorOptions.disabled = false
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "coa_link") {
                if (e.row.data.coa_id) {
                    let qualitySamplingId = e.row.data.coa_id
                    e.editorOptions.onClick = function (e) {
                        window.open("/StockpileManagement/QualitySampling/Index?Id=" + qualitySamplingId + "&openEditingForm=true", "_blank")
                    }
                    e.editorOptions.disabled = false
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "sof_link") {
                if (e.row.data.sof_id) {
                    let sofId = e.row.data.sof_id;
                    e.editorOptions.onClick = function (e) {
                        window.open("/Port/StatementOfFact/Detail?Id=" + sofId + "&openEditingForm=true", "_blank")
                    }
                    e.editorOptions.disabled = false
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data
                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    

                    // Get its value (Id) on value changed
                    let despatchOrderId = e.value;

                    grid.beginCustomLoading();
                    await $.ajax({
                        url: url + '/GetQuantity?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (r) {
                            //let quantity = r && r.data && r.data.length > 0 ? r.data[0].quantity : null;

                            if (Array.isArray(r)) { // Check if the response is an array
                                r = r[0]; // If it is an array, take the first element
                            }

                            if (r && r.original_quantity) {
                                grid.beginUpdate();
                                grid.cellValue(index, "quantity", r.original_quantity);
                                grid.endUpdate();
                            } else if (r && r.quantity) {
                                grid.beginUpdate();
                                grid.cellValue(index, "quantity", r.quantity);
                                grid.endUpdate();
                            }
                        }
                    });
                    await $.ajax({
                        url: '/api/Sales/SalesInvoice/DespatchOrderDetail?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let despatchOrderData = response.data[0]

                            grid.beginUpdate()
                            grid.cellValue(index, "royalty", despatchOrderData.royalty_number);
                            grid.cellValue(index, "invoice", despatchOrderData.invoice_number);
                            grid.endUpdate()
                        }
                    })
                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500)

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    

                    // Get its value (Id) on value changed
                    let despatchOrderId = e.value;

                    grid.beginCustomLoading();

                    // Get another data from API after getting the Id

                    $.ajax({
                        url: url + '/GetBillLadingDate?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (r) {
                            if (r.success) {
                                grid.cellValue(index, "bill_lading_date", r.data.bill_lading_date);
                                grid.cellValue(index, "survey_date", r.data.bill_lading_date);
                            } else {
                                toastr["error"](r.message ?? "Error");
                            }

                        }
                    });

                    /* await $.ajax({
                         url: url + '/GetQuantity?Id=' + despatchOrderId,
                         type: 'GET',
                         contentType: "application/json",
                         beforeSend: function (xhr) {
                             xhr.setRequestHeader("Authorization", "Bearer " + token);
                         },
                         success: function (r) {
                             //let response = r.data[0]
                             grid.beginUpdate()
                                 grid.cellValue(index, "quantity", r.quantity)
                             grid.endUpdate()
                         }
                     });*/
                    //standardHandler(e) // Calling the standard handler to save the edited value

                    $.ajax({
                        url: '/api/StockpileManagement/QualitySampling/QualitySamplingByDo?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (r) {
                            let numb = null;
                            let date = null;

                            if (r.data && r.data.length > 0) {
                                numb = r.data[0].value;
                                date = r.data[0].issued_date;
                            }
                            grid.cellValue(index, "coa_id", numb);
                            grid.cellValue(index, "coa_issued_date", date);


                        }
                    });

                    $.ajax({
                        url: '/api/Port/StatementOfFact/StatemenfOfFactByDespatchOrderIdLookup?despatchOrderId=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (r) {
                            let numb = null;
                            let date = null;

                            if (r.data && r.data.length > 0) {
                                numb = r.data[0].value;
                                date = r.data[0].issued_date;
                            }
                            grid.cellValue(index, "sof_id", numb);
                            grid.cellValue(index, "sof_issued_date", date);


                        }
                    });

                    $.ajax({
                        url: '/api/Sales/ShippingInstruction/GetLc/' + encodeURIComponent(despatchOrderId),
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            //let lcShippingInstruction = response.data;
                            ////console.log(despatchOrderData);
                            if (response) {
                                grid.beginUpdate();
                                // Set its corresponded field's value
                                if (response.lc_price) { grid.cellValue(index, "lc_price", response.lc_price); }

                                grid.cellValue(index, "lc_number", response.lc_number);
                                grid.cellValue(index, "lc_date", response.lc_date);
                                grid.cellValue(index, "lc_amount", response.lc_amount);
                                grid.cellValue(index, "issuing_bank", response.issuing_bank);
                                grid.cellValue(index, "advising_bank", response.advising_bank);
                                grid.endUpdate();
                            } else {
                                grid.beginUpdate();
                                // Set its corresponded field's value
                                grid.cellValue(index, "lc_price", null);
                                grid.cellValue(index, "lc_number", null);
                                grid.cellValue(index, "lc_date", null);
                                grid.cellValue(index, "lc_amount", null);
                                grid.cellValue(index, "issuing_bank", null);
                                grid.cellValue(index, "advising_bank", null);
                                grid.endUpdate();
                            }
                        }
                    });

                    await $.ajax({
                        url: '/api/Sales/DespatchOrder/DataDetail?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let despatchOrderData = response.data;
                            ////console.log(despatchOrderData);
                            grid.beginUpdate()
                            // Set its corresponded field's value
                            grid.cellValue(index, "surveyor_id", despatchOrderData.surveyor_id)
                            grid.cellValue(index, "product_id", despatchOrderData.product_id)
                            grid.cellValue(index, "customer_id", despatchOrderData.customer_id)
                            customer = despatchOrderData.customer_id;
                            if (despatchOrderData.barge_name != null) {
                                grid.cellValue(index, "transport_id", despatchOrderData.barge_name)
                            }
                            else {
                                grid.cellValue(index, "transport_id", despatchOrderData.vessel_name)
                            }
                            grid.endUpdate()
                        }
                    })
                    $.ajax({
                        url: '/api/Sales/Customer/GetCustomerById?Id=' + customer,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let despatchOrderData = response.data[0];
                            //console.log(despatchOrderData);
                            grid.beginUpdate()
                            // Set its corresponded field's value
                            grid.cellValue(index, "country_id", despatchOrderData.country_id)

                            grid.endUpdate()
                        }
                    });
                    $.ajax({
                        url: '/api/Sales/ShippingInstruction/DataDetailByDespatchOrder?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let despatchOrderData = response.data[0];
                            //console.log(despatchOrderData);
                            grid.beginUpdate()
                            // Set its corresponded field's value
                            if (despatchOrderData) {
                                grid.cellValue(index, "peb", despatchOrderData.hs_code)
                            }
                            grid.endUpdate()
                        }
                    })
                    /* $.ajax({
                         url: '/api/Port/StatementOfFact/GetNor/' + despatchOrderId,
                         type: 'GET',
                         contentType: "application/json",
                         beforeSend: function (xhr) {
                             xhr.setRequestHeader("Authorization", "Bearer " + token);
                         },
                         success: function (response) {
                             let despatchOrderData = response.data[0];
                             ////console.log(despatchOrderData);
                             grid.beginUpdate()
                             // Set its corresponded field's value
                             grid.cellValue(index, "nor_tendered", response.nor_tendered)
                             grid.cellValue(index, "nor_accepted", response.nor_accepted)
 
                             grid.endUpdate()
                         }
                     })*/

                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500)

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
        },
        onInitNewRow: function (e) {
            e.data.is_draft_survey = false;
            despatchOrderIdDat = "";
        },
        onEditingStart: function (e) {
            despatchOrderIdDat = e.data.despatch_order_id;
            if (e.data !== null && e.data.approved_on !== null) {
                e.cancel = true;
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

    function createDetailsTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let urlCowDetail = "/api/SurveyManagement/COWDetail";
            //console.log("urlCowDetail", urlCowDetail);

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlCowDetail + "/ByHeaderId/" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlCowDetail + "/InsertData",
                        updateUrl: urlCowDetail + "/UpdateData",
                        deleteUrl: urlCowDetail + "/DeleteData",
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
                            dataField: "survey_number",
                            dataType: "string",
                            caption: "COW Number",
                            sortIndex: 0,
                            sortOrder: "desc",
                            sortOrder: "asc"
                        },
                        {
                            dataField: "survey_date",
                            dataType: "date",
                            caption: "Survey Date",
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
                            }]
                        },
                        {
                            dataField: "bill_lading_date",
                            dataType: "date",
                            caption: "Bill of Lading Date"
                        },
                        {
                            dataField: "bill_lading_number",
                            dataType: "string",
                            caption: "Bill of Lading Number",
                        },
                        {
                            dataField: "surveyor_id",
                            dataType: "text",
                            caption: "Surveyor",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    //loadUrl: url + "/SurveyorIdLookup",
                                    loadUrl: "/api/Organisation/Contractor/ContractorIdLookupByIsSurveyor?isSurveyor=true",
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
                            dataField: "stock_location_id",
                            dataType: "text",
                            caption: "Stock Location",
                            visible: false,
                            formItem: {
                                visible: false
                            },
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/StockLocationIdLookup",
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
                            dataField: "product_id",
                            dataType: "text",
                            caption: "Product",
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
                            },
                        },
                        {
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity",
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
                                message: "This field is required."
                            }],
                            allowEditing: true
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
                            },
                            width: "100px"
                        },
                        {
                            type: "buttons",
                            buttons: ["edit", "delete"]
                        }
                    ],
                    //onToolbarPreparing: function (e) {
                    //    let toolbarItems = e.toolbarOptions.items;

                    //    // Modifies an existing item
                    //    toolbarItems.forEach(function (item) {
                    //        if (item.name === "addRowButton") {
                    //            item.options = {
                    //                icon: "plus",
                    //                onClick: function (e) {
                    //                    openDocumentPopup()
                    //                }
                    //            }
                    //        }

                    //        if (item.name === "editRowButton") {
                    //            item.options = {
                    //                icon: "edit",
                    //                onClick: function (e) {
                    //                    openDocumentPopup()
                    //                }
                    //            }
                    //        }
                    //    });
                    //},
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
                    //filterBuilderPopup: {
                    //    position: { of: window, at: "top", my: "top", offset: { y: 10 } },
                    //},
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
                        e.data.draft_survey_id = currentRecord.id;
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
                    onSaved: function (e) {
                        $mainGrid.dxDataGrid("instance").refresh()
                    },
                });
        }
    }

    let documentDataGrid
    function createDocumentsTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "DraftSurveyDocument";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            COWData = currentRecord;

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByHeaderId/" + encodeURIComponent(currentRecord.id),
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
                    remoteOperations: true,
                    allowColumnResizing: true,
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        {
                            dataField: "draft_survey_id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            }
                        },
                        {
                            dataField: "filename",
                            dataType: "string",
                            caption: "File Name"
                        },
                        {
                            dataField: "created_by",
                            dataType: "string",
                            caption: "Created By",

                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/SystemAdministration/ApplicationUser/lookup?Id=" + currentRecord.created_by,
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
                                    xhr.open("GET", "/api/SurveyManagement/DraftSurveyDocument/DownloadDocument/" + documentData.id, true)
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
                        e.data.draft_survey_id = currentRecord.id;
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

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Documents",
                    template: createDocumentsTab(masterDetailOptions.data)
                },
                //{
                //    title: "Quality Sampling",
                //    template: createQualitySamplingTab(masterDetailOptions.data)
                //},
            ]
        });
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
            //console.log("COWData: ", COWData);
            let formContainer = $("<div>")
            formContainer.dxForm({
                formData: {
                    id: "",
                    draft_survey_id: COWData?.id || "",
                    file: "",
                },
                colCount: 2,
                items: [
                    {
                        dataField: "draft_survey_id",
                        visible: false
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

                                    if (fileSize == 0) {
                                        alert("File content is empty.")
                                        return;
                                    }
                                    if (fileSize >= maxFileSize) {
                                        alert("File size exceeds 50 MB.");
                                        return;
                                    }

                                    let newFormData = {
                                        "draft_survey_id": formData.draft_survey_id,
                                        "fileName": fileName,
                                        "fileSize": fileSize,
                                        "data": data
                                    }

                                    $.ajax({
                                        url: `/api/${areaName}/DraftSurveyDocument/InsertData`,
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

    function createQualitySamplingTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "DraftSurvey";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            bargingTransactionData = currentRecord

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByDraftSurveyId/" + encodeURIComponent(currentRecord.id),
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
                url: url + "/UploadDocument",
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


    //******* Update survey number popup
    let updateSurveyNumberPopupOptions = {
        title: "Update Survey Number",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function () {

            var updateSurveyNumberForm = $("<div>").dxForm({
                formData: {
                    survey_number: "",
                },
                colCount: 2,
                //readOnly: true,
                items: [
                    {
                        dataField: "survey_number",
                        label: {
                            text: "COW Number",
                        },
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
                                let data = updateSurveyNumberForm.dxForm("instance").option("formData");

                                let formData = new FormData();
                                formData.append("key", data.id);
                                formData.append("values", JSON.stringify(data));

                                saveUpdateSurveyNumberForm(formData);
                            }
                        }
                    }
                ],
                onInitialized: () => {
                    $.ajax({
                        type: "GET",
                        url: url + "/GetSurveyNumber/" + encodeURIComponent(COWData.id),
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            // Update form formData with response from api
                            if (response) {
                                updateSurveyNumberForm.dxForm("instance").option("formData", response)
                            }
                        }
                    })
                }
            })

            return updateSurveyNumberForm;
        }
    }

    var updateSurveyNumberPopup = $("#update-survey-number-popup").dxPopup(updateSurveyNumberPopupOptions).dxPopup("instance")

    const showUpdateSurveyNumberPopup = function () {
        updateSurveyNumberPopup.option("contentTemplate", updateSurveyNumberPopupOptions.contentTemplate.bind(this));
        updateSurveyNumberPopup.show()
    }

    const saveUpdateSurveyNumberForm = (formData) => {
        $.ajax({
            type: "POST",
            url: url + "/UpdateSurveyNumber",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    // Show successfuly saved popup
                    let successPopup = $("<div>").dxPopup({
                        width: 300,
                        height: "auto",
                        dragEnabled: false,
                        hideOnOutsideClick: true,
                        showTitle: true,
                        title: "Success",
                        contentTemplate: function () {
                            return $(`<p class="text-center">Data saved.</p>`)
                        }
                    }).appendTo("body").dxPopup("instance");
                    updateSurveyNumberPopup.hide();
                    successPopup.show();
                    $("#grid").dxDataGrid("getDataSource").reload();
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            updateSurveyNumberPopup.hide();
            Swal.fire("Failed !", jqXHR.responseText, "error");
        });
    }
    //********

});