$(function () {

    var token = $.cookie("Token");
    var salesContractTermId = document.querySelector("[name=sales_contract_term_id]").value
    var salesContractTermUomId = document.querySelector("[name=sales_contract_term_uom_id]").value
    var salesContractProductId = document.querySelector("[name=sales_contract_product_id]").value || null
    var salesContractDespatchDemurrageTermId = document.querySelector("[name=sales_contract_despatch_demurrage_term_id]").value || null
    var PricingMethod = null;

    /**
     * =========================
     * Quotation Pricing Grid
     * =========================
     */
    let quotationPricingUrl = "/api/Sales/SalesContractQuotationPrice";
    let quotationPricingGrid = $("#quotation-pricing-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: quotationPricingUrl + "/DataGrid?termId=" + encodeURIComponent(salesContractTermId),
            insertUrl: quotationPricingUrl + "/InsertData",
            updateUrl: quotationPricingUrl + "/UpdateData",
            deleteUrl: quotationPricingUrl + "/DeleteData",
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
                dataField: "sales_contract_term_id",
                caption: "Contract Term",
                allowEditing: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContract/SalesContractIdLookup",
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
                visible: false,
            },
            {
                dataField: "quotation_type_id",
                caption: "Price Term",
                dataType: "string",
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
                            filter: ["item_group", "=", "quotation-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "quotation_period_freq_id",
                caption: "Price Period",
                dataType: "string",
                formItem: {
                    colSpan: 2
                },
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Sales/SalesContractQuotationPrice/MasterListIdLookup?itemGroup=quotation-period",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            //filter: ["item_group", "=", "quotation-period"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            /*{
                dataField: "quotation_period_desc",
                caption: "Description",
                dataType: "string",
                allowEditing: false,
            },*/
            {
                dataField: "pricing_method_id",
                dataType: "string",
                caption: "Pricing Method",
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
                            filter: ["item_group", "=", "pricing-method"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                setCellValue: function (rowData, value) {
                    rowData.pricing_method_id = value

                    //if (value == "7e75e97e2c4e4ceca4ae9231f3ff5073") // Fixed
                    rowData.price_index_id = null

                    //if (value == "adc14328e1e9422e91ce84311a6b0245") // Calculated
                    rowData.price_value = 0
                }
            },
            {
                dataField: "price_index_id",
                dataType: "string",
                caption: "Price Index",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/PriceIndex/PriceIndexIdLookup",
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
                dataField: "index_calculation_id",
                caption: "Index Calculation",
                dataType: "string",
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
                            filter: ["item_group", "=", "index-calculation"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "price_value",
                dataType: "number",
                caption: "Fix Price",
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
                        },
                        onKeyDown: function (e) {
                            if (e.event && (e.event.key === "ArrowUp" || e.event.key === "ArrowDown")) {
                                e.event.preventDefault();
                            }
                        }
                    }
                }
            },
            {
                dataField: "decimal_places",
                dataType: "number",
                caption: "Decimal Places",
            },
            {
                dataField: "currency_id",
                dataType: "string",
                caption: "Currency",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/Currency/CurrencyIdLookup",
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
            //{
            //    dataField: "uom_id",
            //    dataType: "string",
            //    caption: "Price Unit",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
            //},
            {
                dataField: "weightening_value",
                dataType: "string",
                caption: "Weightening Value",
                value: 100
            },
            {
                dataField: "quotation_uom_id",
                dataType: "string",
                caption: "Quotation Price Unit",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "desc"
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
                        dataField: "quotation_type_id",
                    },
                    {
                        dataField: "quotation_uom_id",
                    },
                    {
                        dataField: "quotation_period_freq_id",
                    },
                    {
                        dataField: "quotation_period_desc",
                    },
                    {
                        dataField: "pricing_method_id",
                    },
                    {
                        dataField: "price_value",
                        editorType: "dxNumberBox",
                        editorOptions: {
                            format: "fixedPoint",
                            step: 0
                        }
                    },
                    {
                        dataField: "price_index_id",
                    },
                    {
                        dataField: "index_calculation_id",
                    },
                    {
                        dataField: "decimal_places",
                    },
                    {
                        dataField: "currency_id",
                    },
                    //{
                    //    dataField: "uom_id",
                    //},
                    {
                        dataField: "weightening_value",
                    },
                ]
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

            if (e.dataField === "weightening_value" && e.parentType === "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data
                let wf = e.value
                e.editorOptions.onValueChanged = function (e) {
                    let wf = e.value
                    if (wf == null) {
                        alert("Weightening Value cannot be empty.");
                        return false;
                    }
                    if (wf) {
                        grid.cellValue(index, "weightening_value", wf);
                    }
                }
                standardHandler(e)

            }

            if (e.parentType === "dataRow" && e.dataField === "pricing_method_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let pricingMethodId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/General/MasterList/DataDetail?Id=' + pricingMethodId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let theData = response.data[0];

                            PricingMethod = theData.item_name;

                            if (PricingMethod == "Index Linked") {
                                e.row.data.price_value = null;
                            }
                            else {
                                e.row.data.price_index_id = null;
                            }
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            //if (e.dataField === "price_value" && e.parentType === "dataRow") {
            //    e.editorOptions.disabled = !e.row.data.price_value && (!e.row.data.pricing_method_id || e.row.data.pricing_method_id != "7e75e97e2c4e4ceca4ae9231f3ff5073") // pricing_method != Fixed;
            //}

            //if (e.dataField === "price_index_id" && e.parentType === "dataRow") {
            //    e.editorOptions.disabled = !e.row.data.price_index_id && (!e.row.data.pricing_method_id || e.row.data.pricing_method_id != "adc14328e1e9422e91ce84311a6b0245") // pricing_method != Calculated;
            //}

            if (e.dataField === "price_index_id" && e.parentType === "dataRow") {
                e.editorOptions.disabled = (PricingMethod == "Index Linked");

                //if (PricingMethod == "Index Linked") {
                //    e.row.data.price_value = null;
                //}
                //else {
                //    e.row.data.price_index_id = null;
                //}
            }

            if (e.dataField === "price_value" && e.parentType === "dataRow") {
                e.editorOptions.disabled = (PricingMethod == "Non Index Linked");

                //if (PricingMethod == "Non Index Linked") {
                //    //e.editorOptions.disabled = false;
                //    e.row.data.price_index_id = null;
                //}
                //else {
                //    //e.editorOptions.disabled = true;
                //    //e.row.data.price_value = null;
                //}
            }


            if (e.dataField === "quotation_period_freq_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let quotationPerioFreqId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/General/MasterList/DataDetail?Id=' + quotationPerioFreqId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let quotationPeriodFreqData = response.data[0]

                            // Set its corresponded field's value
                            grid.cellValue(index, "quotation_period_desc", quotationPeriodFreqData.notes)
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
            e.data.sales_contract_term_id = salesContractTermId;
        },
        onExporting: function (e) {
            var entityName = "QuotationPricing"
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
    }).dxDataGrid("instance");


    /**
     * =========================
     * Payment Term Grid
     * =========================
     */

    let paymentTermUrl = "/api/Sales/SalesContractPaymentTerm";
    let paymenTermGrid = $("#payment-term-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: paymentTermUrl + "/DataGrid?termId=" + encodeURIComponent(salesContractTermId),
            insertUrl: paymentTermUrl + "/InsertData",
            updateUrl: paymentTermUrl + "/UpdateData",
            deleteUrl: paymentTermUrl + "/DeleteData",
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
                dataField: "sales_contract_term_id",
                caption: "Contract Term",
                allowEditing: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContract/SalesContractIdLookup",
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
                    visible: false
                },
                visible: false,
            },
            {
                dataField: "invoice_type_id",
                caption: "Invoice Type",
                dataType: "string",
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
                            filter: ["item_group", "=", "invoice-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                validationRules: [
                    {
                        type: "required",
                        message: "Invoice Type is required."
                    },
                ],
                setCellValue: function (rowData, value) {
                    rowData.invoice_type_id = value
                    rowData.downpayment_value = null
                }
            },
            {
                dataField: "payment_method_id",
                dataType: "string",
                caption: "Payment Method",
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
                            filter: ["item_group", "=", "payment-method"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                visible: false,
                setCellValue: function (rowData, value) {
                    rowData.payment_method_id = value
                }
            },
            {
                dataField: "downpayment_value",
                dataType: "number",
                caption: "Down Payment",
                visible: true,
                formItem: {
                    colSpan: 1,
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                }
                //validationRules: [
                //    {
                //        type: "required",
                //        message: "Down Payment is required."
                //    },
                //    {
                //        type: "custom",
                //        message: "The entered was out of min/max range",
                //        validationCallback: function (args) {
                //            let invoiceTypeId = args.data.invoice_type_id

                //            if (invoiceTypeId == "85b7a23ed9954f269b59520629ce1b11") {
                //                var max = 100
                //                var min = 1
                //                if (args.value < min || args.value > max) {
                //                    args.rule.message = "Down Payment must be 1-100%"
                //                    return false;
                //                }
                //                return true;
                //            }
                //            else if (invoiceTypeId == "0c693e072c55408895121d9781abd577") {
                //                var min = 10000
                //                if (args.value < min) {
                //                    args.rule.message = "Down Payment minimum must be 10.000"
                //                    return false;
                //                }
                //                return true;
                //            }

                //        }
                //    }
                //]
            },
            {
                dataField: "reference_date_id",
                dataType: "string",
                caption: "Reference Date",
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
                            filter: ["item_group", "=", "reference-date"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "number_of_days",
                dataType: "number",
                caption: "Number of Days"
            },
            {
                dataField: "days_type_id",
                dataType: "string",
                caption: "Type of Days",
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
                            filter: ["item_group", "=", "days-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "calendar_id",
                dataType: "string",
                caption: "Calendar",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/Calendar/CalendarIdLookup",
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
                dataField: "exchange_date_id",
                dataType: "string",
                caption: "Exchange Date",
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
                            filter: ["item_group", "=", "reference-date"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "desc",
                formItem: {
                    visible: false
                }
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
                colCount: 2
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
            if (e.dataField === "downpayment_value" && e.parentType == "dataRow") {
                e.editorOptions.disabled = !e.row.data.downpayment_value &&
                    (!e.row.data.invoice_type_id || !["4889385f911c4c819539b1e9eb66061d", "c52b839771b44e28919238e04bcd9d14", "2999361f2c2e4be5aa60d440fa8a484b"].includes(e.row.data.invoice_type_id)) // invoice_type_id not DP % or DP Fixed Amount
            }
        },
        onEditingStart: function (e) {
        },
        onInitNewRow: function (e) {
            e.data.sales_contract_term_id = salesContractTermId;
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
            var entityName = "PaymentTerm"
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
    }).dxDataGrid("instance");


    /**
     * =========================
     * Taxes Grid
     * =========================
     */

    let taxesUrl = "/api/Sales/SalesContractTax";
    let taxesGrid = $("#taxes-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: taxesUrl + "/DataGrid?termId=" + encodeURIComponent(salesContractTermId),
            insertUrl: taxesUrl + "/InsertData",
            updateUrl: taxesUrl + "/UpdateData",
            deleteUrl: taxesUrl + "/DeleteData",
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
                dataField: "sales_contract_term_id",
                caption: "Contract Term",
                allowEditing: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContract/SalesContractIdLookup",
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
                visible: false,
            },
            {
                dataField: "tax_name",
                dataType: "string",
                caption: "Tax Name",
                formItem: {
                    colSpan: 2
                }
            },
            {
                dataField: "tax_id",
                caption: "Tax",
                dataType: "string",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        //loadUrl: "/api/Sales/Tax/TaxIdLookup",
                        loadUrl: "/api/General/Tax/TaxIdLookup",
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
                    rowData.tax_id = value;
                }
            },
            {
                dataField: "tax_rate",
                dataType: "number",
                caption: "Rate",
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    editorType: "dxNumberBox",
                    step: 0,
                    editorOptions: {
                        format: {
                            type: "fixedPoint",
                            precision: 3
                        }
                    }
                },
                customizeText: function (cellInfo) {
                    return numeral(cellInfo.value).format('0,0.00');
                }
            },
            {
                dataField: "calculation_sign",
                caption: "Calculation Sign",
                dataType: "number",
                lookup: {
                    dataSource: [
                        {
                            value: -1,
                            text: "Decrease"
                        },
                        {
                            value: 1,
                            text: "Increase"
                        },
                    ],
                    valueExpr: "value",
                    displayExpr: "text"
                },
                editorOptions: { readOnly: true }
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "desc"
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
                        dataField: "tax_name"
                    },
                    {
                        dataField: "tax_id",
                    },
                    {
                        dataField: "tax_rate",
                    },
                    {
                        dataField: "calculation_sign",
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
        onInitNewRow: function (e) {
            e.data.sales_contract_term_id = salesContractTermId;
        },
        onExporting: function (e) {
            var entityName = "Taxes"
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
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "tax_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    let recordId = e.value

                    $.ajax({
                        url: '/api/General/Tax/TaxByIdLookup?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];

                            grid.cellValue(index, "tax_rate", record.rate);
                            grid.cellValue(index, "calculation_sign", record.calculation_sign);

                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
        }
    }).dxDataGrid("instance");

    /**
     * =========================
     * Contract Product
     * =========================
     */

    const getSalesContractProductHeader = () => {
        $.ajax({
            type: "GET",
            url: "/api/Sales/SalesContractProduct/Detail/" + encodeURIComponent(salesContractProductId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                //console.log(response)
                // If salesContractProduct exist
                if (response) {
                    let salesContractProductHeaderData = response
                    salesContractProductHeaderData = {
                        ...salesContractProductHeaderData,
                        "just_received_data": true
                    }

                    contractProductHeaderForm.option("formData", salesContractProductHeaderData)
                }
            }
        })
    }

    const saveSalesContractProductHeader = (formData) => {
        $.ajax({
            type: "POST",
            //url: "/api/Sales/SalesContractProduct/InsertData",
            url: "/api/Sales/SalesContractProduct/SaveData",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let salesContractProductHeaderData = response;

                    // Show successfuly saved popup
                    let successPopup = $("<div>").dxPopup({
                        width: 300,
                        height: "auto",
                        dragEnabled: false,
                        hideOnOutsideClick: true,
                        showTitle: true,
                        title: "Success",
                        contentTemplate: function () {
                            return $(`<h5 class="text-center">All changes are saved.</h5>`)
                        }
                    }).appendTo("#contract-product-header-form").dxPopup("instance");

                    successPopup.show();

                    if (!salesContractProductId) {
                        // Update sales contract product specification grid
                        updateSalesContractProductSpecification(salesContractProductHeaderData);
                    }
                }

            }
        })
    }

    const updateSalesContractProductSpecification = (salesContractProductHeaderData) => {
        /**
         * Update contractProductSpecificationGrid options
         * after getting salesContractProduct data
         * Options that need to updated:
         * - editing.allowAdding : set true
         * - onInitNewRow : set sales_contract_product_id
         * - dataSource : set params
         */
        contractProductSpecificationGrid.option("editing.allowAdding", true)
        contractProductSpecificationGrid.option("onInitNewRow", function (e) {
            e.data.sales_contract_product_id = salesContractProductHeaderData.id;
        })
        contractProductSpecificationGrid.option("dataSource", DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: contractProductSpecificationUrl + "/DataGrid?termProductId=" + encodeURIComponent(salesContractProductHeaderData.id),
            insertUrl: contractProductSpecificationUrl + "/InsertData",
            updateUrl: contractProductSpecificationUrl + "/UpdateData",
            deleteUrl: contractProductSpecificationUrl + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };
            }
        }))
    }

    let contractProductHeaderForm = $("#contract-product-header-form").dxForm({
        formData: {
            sales_contract_term_id: salesContractTermId,
            contract_product_name: "",
            product_id: "",
            mass_required: "",
            maximum_order: "",
            minimum_order: "",
            quantity_uom: ""
        },
        colCount: 2,
        items: [
            /*{
                dataField: "contract_product_name",
                label: {
                    text: "Contract Product Name"
                },
                colSpan: 2
            },*/
            {
                dataField: "product_category_id",
                label: {
                    text: "Product Category"
                },
                colSpan: 2,
                caption: "Product Category",
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Material/ProductSpecification/ProductCategoryIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "product_id",
                label: {
                    text: "Product"
                },
                colSpan: 2,
                editorType: "dxSelectBox",
                editorOptions: {
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
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "analyte_standard_id",
                label: {
                    text: "Standard Analysis"
                },
                colSpan: 2,
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContractTerm/MasterListIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },

            //{
            //    dataField: "mass_required",
            //    label: {
            //        text: "Mass Required"
            //    },
            //    editorType: "dxNumberBox",
            //    editorOptions: {
            //        format: {
            //            type: "fixedPoint",
            //            precision: 2
            //        }
            //    },
            //    colSpan: 2
            //},
            //{
            //    dataField: "minimum_order",
            //    label: {
            //        text: "Minimum Order"
            //    },
            //    editorType: "dxNumberBox",
            //    editorOptions: {
            //        format: {
            //            type: "fixedPoint",
            //            precision: 2
            //        }
            //    },
            //},
            //{
            //    dataField: "maximum_order",
            //    label: {
            //        text: "Maximum Order"
            //    },
            //    editorType: "dxNumberBox",
            //    editorOptions: {
            //        format: {
            //            type: "fixedPoint",
            //            precision: 2
            //        }
            //    },
            //},
            //{
            //    dataField: "uom_id",
            //    label: {
            //        text: "Quantity Unit",
            //    },
            //    editorType: "dxSelectBox",
            //    editorOptions: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/UOM/UOM/UOMIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        searchEnabled: true,
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    }, 
            //    colSpan: 2
            //},
            {
                itemType: "button",
                colSpan: 2,
                horizontalAlignment: "right",
                buttonOptions: {
                    text: "Save",
                    type: "secondary",
                    useSubmitBehavior: true,
                    onClick: function () {
                        let data = contractProductHeaderForm.option("formData");
                        let formData = new FormData()
                        formData.append("values", JSON.stringify(data))

                        saveSalesContractProductHeader(formData)
                    }
                }
            }
        ],
        onInitialized: function (e) {
            this.updateData("uom_id", salesContractTermUomId)

            // Get sales contract product data if has salesContractProductId
            if (salesContractProductId) {
                getSalesContractProductHeader()
            }

        },
        onFieldDataChanged: function (data) {
            if (data.dataField == "mass_required") {
                if (!this.option('formData.just_received_data')) {
                    let massRequired = data.value
                    let minimumOrder = massRequired - (massRequired * 0.1)
                    let maximumOrder = massRequired + (massRequired * 0.1)

                    this.updateData("minimum_order", minimumOrder)
                    this.updateData("maximum_order", maximumOrder)
                }
            }

            // Set just_received_data to false
            // To be able to automate the value of minimum and maximum order
            // by mass_required onchange
            if (data.dataField == "just_received_data") {
                if (data.value == true) {
                    this.updateData("just_received_data", false)
                }
            }
            if (data.dataField === "product_category_id") {
                var categoryId = data.value;

                var productEditor = contractProductHeaderForm.getEditor("product_id");
                var dataSource = productEditor.getDataSource();

                dataSource.filter(["product_category_id", "=", categoryId]);
                dataSource.load();
            }
            /* if (data.dataField == "product_category_id") {
                 var categoryId = data.value;
 
                 var secondDropDown = contractProductHeaderForm.getEditor("product_id");
                 secondDropDown.getDataSource().filter(['product_category_id', '=', categoryId]);
                 secondDropDown.getDataSource().load();
             }*/
        }
    }).dxForm("instance");

    let contractProductSpecificationUrl = "/api/Sales/SalesContractProductSpecification";
    let contractProductSpecificationGrid = $("#contract-product-specification-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: contractProductSpecificationUrl + "/DataGrid?termProductId=" + encodeURIComponent(salesContractProductId),
            insertUrl: contractProductSpecificationUrl + "/InsertData",
            updateUrl: contractProductSpecificationUrl + "/UpdateData",
            deleteUrl: contractProductSpecificationUrl + "/DeleteData",
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
                dataField: "sales_contract_product_id",
                caption: "Contract Product",
                allowEditing: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContract/SalesContractIdLookup",
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
                visible: false,
            },
            {
                dataField: "analyte_id",
                caption: "Analyte Definition",
                dataType: "string",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Quality/Analyte/AnalyteIdLookup",
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
            },
            {
                dataField: "analyte_standard_id",
                dataType: "string",
                caption: "Standard",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
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
                            filter: ["item_group", "=", "analyte-standard"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            //{
            //    dataField: "value",
            //    dataType: "number",
            //    caption: "Value",
            //    validationRules: [{
            //        type: "required",
            //        message: "The field is required."
            //    }]
            //},
            {
                dataField: "target",
                dataType: "number",
                caption: "Typical",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
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
                dataField: "minimum",
                dataType: "number",
                caption: "Minimum",
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
                dataField: "maximum",
                dataType: "number",
                caption: "Maximum",
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
                dataField: "uom_id",
                dataType: "string",
                caption: "Unit",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "desc"
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
            allowAdding: salesContractProductId ? true : false,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 2,
                items: [
                    {
                        dataField: "analyte_id",
                    },
                    {
                        dataField: "analyte_standard_id",
                    },
                    //{
                    //    dataField: "value",
                    //},
                    {
                        dataField: "target",
                        editorType: "dxNumberBox",
                        editorOptions: {
                            format: "fixedPoint",
                            step: 0
                        }
                    },
                    {
                        dataField: "minimum",
                        editorType: "dxNumberBox",
                        editorOptions: {
                            format: "fixedPoint",
                            step: 0
                        }
                    },
                    {
                        dataField: "maximum",
                        editorType: "dxNumberBox",
                        editorOptions: {
                            format: "fixedPoint",
                            step: 0
                        }
                    },
                    {
                        dataField: "uom_id",
                    },
                ]
            }
        },
        onInitNewRow: function (e) {
            e.data.sales_contract_product_id = salesContractProductId
            e.data.uom_id = "9e8a50e8d9f24fe392b958165a2f392e" // Percent Uom Id
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
        onContentReady: function (e) {
            $("#btn-fetch").remove();
            var $customButton = $('<div id="btn-fetch">').dxButton({
                icon: 'refresh',
                text: "Fetch",
                onClick: function () {
                    $.ajax({
                        url: '/api/Sales/SalesContractTerm/FetchProductAnalyteIntoSalesContractProduct/' + salesContractProductId,
                        type: 'GET',
                        contentType: "application/json",
                        headers: {
                            "Authorization": "Bearer " + token
                        },
                    }).done(function (result) {
                        if (result.status.success) {
                            Swal.fire("Success!", "Fetching Data successfully.", "success");
                            $("#contract-product-specification-grid").dxDataGrid("getDataSource").reload();
                        } else {
                            Swal.fire("Error !", result.message, "error");
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        Swal.fire("Failed !", textStatus, "error");
                    });
                }
            })

            e.element.find('.dx-datagrid-header-panel').append($customButton)
        },

        onExporting: function (e) {
            var entityName = "ContractProduct"
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
    }).dxDataGrid("instance");

    /**
     * =========================
     * Sales Contract Charge // Price Adjustment
     * =========================
     */

    let contractChargesUrl = "/api/Sales/SalesContractCharge";
    let contractChargesGrid = $("#contract-charges-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: contractChargesUrl + "/DataGrid?termId=" + encodeURIComponent(salesContractTermId),
            insertUrl: contractChargesUrl + "/InsertData",
            updateUrl: contractChargesUrl + "/UpdateData",
            deleteUrl: contractChargesUrl + "/DeleteData",
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
                dataField: "sales_contract_term_id",
                caption: "Sales Contract Term",
                allowEditing: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContractTerm/SalesContractTermIdLookup",
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
                visible: false,
            },
            {
                dataField: "sales_charge_id",
                caption: "Sales Charge",
                dataType: "string",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesCharge/SalesChargeIdLookup",
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
            },
            {
                dataField: "charge_type_id",
                caption: "Charge Type",
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
                            filter: ["item_group", "=", "charge-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                dataType: "string",
                visible: false,
                allowEditing: false
            },
            {
                dataField: "prerequisite",
                caption: "Prerequisite",
                dataType: "string",
            },
            {
                dataField: "charge_formula",
                caption: "Charge Formula",
                dataType: "string",
                allowEditing: false
            },
            {
                dataField: "formula_creator_btn",
                caption: "Edit Formula",
                dataType: "string",
                visible: false
            },
            {
                dataField: "decimal_places",
                dataType: "number",
                caption: "Decimal Places",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "rounding_type_id",
                dataType: "string",
                caption: "Rounding Type",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
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
                            filter: ["item_group", "=", "rounding-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "description",
                dataType: "string",
                caption: "Description",
            },
            {
                dataField: "order",
                caption: "Order",
                dataType: "string",
                sortOrder: "asc"
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "desc"
            },
            {
                width: "130px",
                type: "buttons",
                buttons: [
                    {
                        hint: "Move up",
                        icon: "arrowup",
                        onClick: function (e) {
                            let index = e.row.rowIndex

                            if (index == 0) {
                                alert("First data cannot be moved up")
                                return false
                            }

                            let formData = new FormData();
                            formData.append("key", e.row.data.id)
                            formData.append("termId", salesContractTermId)
                            formData.append("type", -1)

                            $.ajax({
                                type: "PUT",
                                url: "/Api/Sales/SalesContractCharge/UpdateOrderData",
                                data: formData,
                                processData: false,
                                contentType: false,
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                },
                                success: function (response) {
                                    if (response) {
                                        contractChargesGrid.refresh()
                                    }

                                }
                            })

                        }
                    },
                    {
                        hint: "Move down",
                        icon: "arrowdown",
                        onClick: function (e) {
                            let index = e.row.rowIndex
                            let lastIndex = contractChargesGrid.totalCount() - 1

                            if (index == lastIndex) {
                                alert("Last data cannot be moved down")
                                return false
                            }

                            let formData = new FormData();
                            formData.append("key", e.row.data.id)
                            formData.append("termId", salesContractTermId)
                            formData.append("type", 1)

                            $.ajax({
                                type: "PUT",
                                url: "/Api/Sales/SalesContractCharge/UpdateOrderData",
                                data: formData,
                                processData: false,
                                contentType: false,
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                },
                                success: function (response) {
                                    if (response) {
                                        contractChargesGrid.refresh()
                                    }

                                }
                            })
                        }
                    },
                    "edit",
                    "delete"
                ]
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
                        dataField: "sales_charge_id",
                    },
                    {
                        dataField: "charge_type_id",
                    },
                    {
                        dataField: "prerequisite",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 50
                        },
                        colSpan: 2
                    },
                    {
                        dataField: "charge_formula",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 100
                        },
                        colSpan: 2
                    },
                    {
                        dataField: "formula_creator_btn",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "Open Formula Editor",
                        },
                        horizontalAlignment: "right",
                        colSpan: 2
                    },
                    {
                        dataField: "decimal_places",
                    },
                    {
                        dataField: "rounding_type_id",
                    },
                    {
                        dataField: "description",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 50
                        },
                        colSpan: 2
                    },
                ]
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
            // Set onValueChanged for sales_charge_id
            if (e.parentType === "dataRow" && e.dataField == "sales_charge_id") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let salesChargeId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Sales/SalesCharge/DataDetail?Id=' + salesChargeId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let salesCharge = response.data[0]
                            //console.log(salesCharge)

                            // Set its corresponded field's value
                            grid.cellValue(index, "charge_type_id", salesCharge.charge_type_id)
                            grid.cellValue(index, "prerequisite", salesCharge.prerequisite)
                            grid.cellValue(index, "charge_formula", salesCharge.charge_formula)
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            // Set Formula Creator onchange handler
            if (e.parentType === "dataRow" && e.dataField == "formula_creator_btn") {
                let formula = e.row.data.charge_formula
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onClick = function (e) {
                    let formulaCreator = new FormulaCreator({
                        formula: formula,
                        saveFormulaCallback: function (value) {
                            grid.cellValue(index, "charge_formula", value)
                        }
                    })
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
            e.data.sales_contract_term_id = salesContractTermId;
        },
        onExporting: function (e) {
            var entityName = "ContractCharges"
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
    }).dxDataGrid("instance");

    /**
     * =========================
     * Despatch/Demurrage Term Grid
     * =========================
     */

    const getSalesContractDespatchDemurrageTerm = () => {
        $.ajax({
            type: "GET",
            url: "/api/Sales/SalesContractDespatchDemurrageTerm/Detail/" + encodeURIComponent(salesContractDespatchDemurrageTermId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                // If salesContractDespatchDemurrageTerm exist
                if (response) {
                    let salesContractDespatchDemurrageTermData = response
                    despatchDemurrageTermForm.option("formData", salesContractDespatchDemurrageTermData)
                }
            }
        })
    }

    const saveSalesContractDespatchDemurrageTerm = (formData) => {
        $.ajax({
            type: "POST",
            url: "/api/Sales/SalesContractDespatchDemurrageTerm/InsertData",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let salesContractDespatchDemurrageTermData = response

                    // Show successfuly saved popup
                    let successPopup = $("<div>").dxPopup({
                        width: 300,
                        height: "auto",
                        dragEnabled: false,
                        hideOnOutsideClick: true,
                        showTitle: true,
                        title: "Success",
                        contentTemplate: function () {
                            return $(`<h5 class="text-center">All changes are saved.</h5>`)
                        }
                    }).appendTo("#despatch-demurrage-term-form").dxPopup("instance")

                    successPopup.show()

                    if (!salesContractDespatchDemurrageTermId) {
                        // Update sales contract despatch demurrage term grid
                        updateSalesContractDespatchDemurrageDelay(salesContractDespatchDemurrageTermData)
                    }
                }
            }
        })
    }

    const updateSalesContractDespatchDemurrageDelay = (salesContractDespatchDemurrageTermData) => {
        /**
         * Update despatchDemurrageTermDelayGrid options
         * after getting salesContractDespatchDemurrageTerm data
         * Options that need to updated:
         * - editing.allowAdding : set true
         * - onInitNewRow : set sales_contract_despatch_demurrage_id
         * - dataSource : set params
         */
        despatchDemurrageTermDelayGrid.option("editing.allowAdding", true)
        despatchDemurrageTermDelayGrid.option("onInitNewRow", function (e) {
            e.data.sales_contract_despatch_demurrage_id = salesContractDespatchDemurrageTermData.id;
        })
        contractProductSpecificationGrid.option("dataSource", DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: despatchDemurrageTermDelayUrl + "/DataGrid?termProductId=" + encodeURIComponent(salesContractDespatchDemurrageTermData.id),
            insertUrl: despatchDemurrageTermDelayUrl + "/InsertData",
            updateUrl: despatchDemurrageTermDelayUrl + "/UpdateData",
            deleteUrl: despatchDemurrageTermDelayUrl + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };
            }
        }))
    }

    let despatchDemurrageTermForm = $("#despatch-demurrage-term-form").dxForm({
        formData: {
            id: "",
            sales_contract_term_id: salesContractTermId,
            despatch_demurrage_id: "",
            location_id: "",
            loading_rate: "",
            //loading_rate_uom_id: "",
            turn_time: "",
            turn_time_ending: "",
            //turn_time_uom_id: "",
            despatch_percentage: "",
            rate: "",
            currency_id: "",
            //sof_id: ""
        },
        colCount: 2,
        items: [
            {
                dataField: "sales_contract_term_id",
                label: {
                    text: "Sales Contract Term"
                },
                colSpan: 2,
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContractTerm/SalesContractTermIdLookup",
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
                visible: false,
            },
            {
                dataField: "location_id",
                label: {
                    text: "Port of Loading 1"
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Location/PortLocation/PortLocationIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }]
            },
            {
                dataField: "location2_id",
                label: {
                    text: "Port of Loading 2"
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Location/PortLocation/PortLocationIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "location3_id",
                label: {
                    text: "Port of Loading 3"
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Location/PortLocation/PortLocationIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "loading_rate_geared",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                label: {
                    text: "Loadrate Offshore G&G (mt/day)"
                },
            },
            {
                dataField: "loading_rate_gearless",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                label: {
                    text: "Loadrate Offshore Gearless (mt/day)"
                },
            },
            {
                dataField: "loading_rate_nplct",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                label: {
                    text: "NPLCT Loadrate (mt/day)"
                },
            },

            //{
            //    dataField: "loading_rate_uom_id",
            //    label: {
            //        text: "Loading Rate Unit"
            //    },
            //    editorType: "dxSelectBox",
            //    editorOptions: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/UOM/UOM/UOMIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        searchEnabled: true,
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //},
            {
                dataField: "turn_time",
                label: {
                    text: "Turn Time Begining (Hour)"
                },
                colSpan: 1,
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
            },
            {
                dataField: "turn_time_ending",
                label: {
                    text: "Turn Time Ending (Hour)"
                },
                colSpan: 1,
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
            },
            //{
            //    dataField: "turn_time_uom_id",
            //    label: {
            //        text: "Turn Time Unit",
            //    },
            //    editorType: "dxSelectBox",
            //    editorOptions: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/UOM/UOM/UOMIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        searchEnabled: true,
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //},
            {
                dataField: "rate",
                label: {
                    text: "Demurrage Rate"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
            },
            {
                dataField: "currency_id",
                label: {
                    text: "Currency",
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/Currency/CurrencyIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }]
            },
            {
                dataField: "despatch_percentage",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    value: 50,
                    readOnly: true,
                    step: 0
                },
                label: {
                    text: "Despatch (%)"
                },
                colSpan: 2,
            },
            //{
            //    dataField: "sof_id",
            //    label: {
            //        text: "State of Fact",
            //    },
            //    editorType: "dxSelectBox",
            //    editorOptions: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/Port/StatementOfFact/StatemenfOfFactIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        searchEnabled: true,
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //},
            {
                itemType: "button",
                colSpan: 2,
                horizontalAlignment: "right",
                buttonOptions: {
                    text: "Save",
                    type: "secondary",
                    useSubmitBehavior: true,
                    onClick: function () {
                        let data = despatchDemurrageTermForm.option("formData");
                        let formData = new FormData()
                        formData.append("values", JSON.stringify(data))

                        saveSalesContractDespatchDemurrageTerm(formData)
                    }
                }
            }
        ],
        onInitialized: function () {
            // Get sales contract product data if has salesContractProductId
            if (salesContractDespatchDemurrageTermId) {
                getSalesContractDespatchDemurrageTerm()
            }
        }
    }).dxForm("instance");

    let despatchDemurrageTermDelayUrl = "/api/Sales/SalesContractDespatchDemurrageDelay";
    //let despatchDemurrageTermDelayGrid = $("#despatch-demurrage-term-delay-grid").dxDataGrid({
    //    dataSource: DevExpress.data.AspNet.createStore({
    //        key: "id",
    //        loadUrl: despatchDemurrageTermDelayUrl + "/DataGrid?demurrageTermId=" + encodeURIComponent(salesContractDespatchDemurrageTermId),
    //        insertUrl: despatchDemurrageTermDelayUrl + "/InsertData",
    //        updateUrl: despatchDemurrageTermDelayUrl + "/UpdateData",
    //        deleteUrl: despatchDemurrageTermDelayUrl + "/DeleteData",
    //        onBeforeSend: function (method, ajaxOptions) {
    //            ajaxOptions.xhrFields = { withCredentials: true };
    //            ajaxOptions.beforeSend = function (request) {
    //                request.setRequestHeader("Authorization", "Bearer " + token);
    //            };
    //        }
    //    }),
    //    remoteOperations: true,
    //    allowColumnResizing: true,
    //    columnResizingMode: "widget",
    //    columns: [
    //        {
    //            dataField: "sales_contract_despatch_demurrage_id",
    //            caption: "Sales Despatch Demurrage",
    //            allowEditing: false,
    //            visible: false,
    //        },
    //        {
    //            dataField: "incident_id",
    //            caption: "Event Category",
    //            dataType: "string",
    //            lookup: {
    //                dataSource: function (options) {
    //                    return {
    //                        store: DevExpress.data.AspNet.createStore({
    //                            key: "value",
    //                            loadUrl: "/api/General/EventCategory/EventCategoryIdLookup",
    //                            onBeforeSend: function (method, ajaxOptions) {
    //                                ajaxOptions.xhrFields = { withCredentials: true };
    //                                ajaxOptions.beforeSend = function (request) {
    //                                    request.setRequestHeader("Authorization", "Bearer " + token);
    //                                };
    //                            }
    //                        }),
    //                    }
    //                },
    //                valueExpr: "value",
    //                displayExpr: "text"
    //            },
    //        },
    //        {
    //            dataField: "demurrage_applicable_percentage",
    //            caption: "Demurrage Applicable %",
    //            dataType: "number",
    //        },
    //        {
    //            dataField: "despatch_applicable_percentage",
    //            dataType: "number",
    //            caption: "Despatch Applicable %",
    //        },
    //        {
    //            dataField: "created_on",
    //            caption: "Created On",
    //            dataType: "string",
    //            visible: false,
    //            sortOrder: "desc"
    //        },
    //    ],
    //    filterRow: {
    //        visible: true
    //    },
    //    headerFilter: {
    //        visible: true
    //    },
    //    groupPanel: {
    //        visible: true
    //    },
    //    searchPanel: {
    //        visible: true,
    //        width: 240,
    //        placeholder: "Search..."
    //    },
    //    filterPanel: {
    //        visible: true
    //    },
    //    filterBuilderPopup: {
    //        position: { of: window, at: "top", my: "top", offset: { y: 10 } },
    //    },
    //    columnChooser: {
    //        enabled: true,
    //        mode: "select"
    //    },
    //    paging: {
    //        pageSize: 10
    //    },
    //    pager: {
    //        allowedPageSizes: [10, 20, 50, 100],
    //        showNavigationButtons: true,
    //        showPageSizeSelector: true,
    //        showInfo: true,
    //        visible: true
    //    },
    //    height: 800,
    //    showBorders: true,
    //    editing: {
    //        mode: "form",
    //        allowAdding: salesContractDespatchDemurrageTermId ? true : false,
    //        allowUpdating: true,
    //        allowDeleting: true,
    //        useIcons: true,
    //        form: {
    //            colCount: 2,
    //            items: [
    //                {
    //                    dataField: "incident_id",
    //                    colSpan: 2
    //                },
    //                {
    //                    dataField: "demurrage_applicable_percentage",
    //                },
    //                {
    //                    dataField: "despatch_applicable_percentage",
    //                },
    //            ]
    //        }
    //    },
    //    onInitNewRow: function (e) {
    //        e.data.sales_contract_despatch_demurrage_id = salesContractDespatchDemurrageTermId
    //    },
    //    grouping: {
    //        contextMenuEnabled: true,
    //        autoExpandAll: false
    //    },
    //    rowAlternationEnabled: true,
    //    export: {
    //        enabled: true,
    //        allowExportSelectedData: true
    //    },
    //    onExporting: function (e) {
    //        var entityName = "DespatchDemurrageTerm"
    //        var workbook = new ExcelJS.Workbook();
    //        var worksheet = workbook.addWorksheet(entityName);

    //        DevExpress.excelExporter.exportDataGrid({
    //            component: e.component,
    //            worksheet: worksheet,
    //            autoFilterEnabled: true
    //        }).then(function () {
    //            // https://github.com/exceljs/exceljs#writing-xlsx
    //            workbook.xlsx.writeBuffer().then(function (buffer) {
    //                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), entityName + '.xlsx');
    //            });
    //        });
    //        e.cancel = true;
    //    }
    //}).dxDataGrid("instance");

    /**
     * =========================
     * Despatch Plan Grid
     * =========================
     */

    let despatchPlanUrl = "/api/Sales/SalesContractDespatchPlan";
    let despatchPlanGrid = $("#despatch-plan-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: despatchPlanUrl + "/DataGrid?termId=" + encodeURIComponent(salesContractTermId),
            insertUrl: despatchPlanUrl + "/InsertData",
            updateUrl: despatchPlanUrl + "/UpdateData",
            deleteUrl: despatchPlanUrl + "/DeleteData",
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
                dataField: "sales_contract_term_id",
                caption: "Sales Despatch Demurrage",
                allowEditing: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/SalesContract/SalesContractIdLookup",
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
                visible: false,
            },
            {
                dataField: "despatch_plan_name",
                caption: "Shipping Plan Name",
                dataType: "string",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
            },
            {
                dataField: "despatch_date",
                caption: "Despatch Date",
                dataType: "date",
            },
            {
                dataField: "quantity",
                caption: "Quantity",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
                dataType: "number",
            },
            {
                dataField: "uom_id",
                caption: "Unit",
                dataType: "string",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/UOM/UOM/UOMIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "fulfilment_type_id",
                caption: "Despatch Fulfilment Type",
                dataType: "string",
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
                            filter: ["item_group", "=", "fulfilment-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "delivery_term_id",
                caption: "Delivery Term",
                dataType: "string",
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
                            filter: ["item_group", "=", "delivery-term"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "notes",
                caption: "Notes",
                dataType: "string",
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "desc"
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
                        dataField: "despatch_plan_name",
                        colSpan: 2
                    },
                    {
                        dataField: "despatch_date",
                    },
                    {
                        dataField: "delivery_term_id",
                    },
                    {
                        dataField: "quantity",
                    },
                    {
                        dataField: "uom_id",
                    },
                    {
                        dataField: "fulfilment_type_id",
                        colSpan: 2
                    },
                    {
                        dataField: "notes",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 50
                        },
                        colSpan: 2
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
        onInitNewRow: function (e) {
            e.data.sales_contract_term_id = salesContractTermId;
        },
        onExporting: function (e) {
            var entityName = "DespatchPlan"
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
    }).dxDataGrid("instance");

});


