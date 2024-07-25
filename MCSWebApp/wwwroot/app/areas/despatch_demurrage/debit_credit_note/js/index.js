$(function () {

    var token = $.cookie("Token");
    var areaName = "DespatchDemurrage";
    var entityName = "DebitCreditNote";
    var url = "/api/" + areaName + "/" + entityName;
    var targetType = "";
    var recordId = "";

    /**
     * ===================
     * Despatch Demurrage Contract Grid
     * ===================
     */

    $("#dx-grid").dxDataGrid({
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
                dataField: "debit_credit_number",
                dataType: "text",
                caption: "Debit Credit Number",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "despatch_demurrage_invoice_id",
                colSpan: 2,
                dataType: "text",
                caption: "Valuation Number",
                validationRules: [{
                    type: "required",
                    message: "The Valuation Number is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/DespatchDemurrage/Invoice/DesDemInvoiceIdLookup",
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
                dataField: "valuation_target_type",
                dataType: "text",
                caption: "Valuation Target Type",
            },
            {
                dataField: "valuation_target_name",
                dataType: "text",
                caption: "Valuation Target",
            },
            {
                dataField: "valuation_target_id",
                dataType: "text",
                caption: "Valuation Target",
                validationRules: [{
                    type: "required",
                    message: "Valuation Target is required."
                }],
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        let _url = "/api/DespatchDemurrage/DebitCreditNote/ValuationTargetLookup?TargetType=Buyer";
                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {

                                //console.log("Options Data", options.data);
                                targetType = "Buyer";
                                if (options.data.valuation_target_type !== undefined &&
                                    options.data.valuation_target_type !== null && 
                                    options.data.valuation_target_type == "Seller") {
                                    targetType = options.data.valuation_target_type;
                                    _url = "/api/DespatchDemurrage/DebitCreditNote/ValuationTargetLookup?TargetType=seller";
                                }
                            }
                        }
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: _url,
                                //loadUrl: "/api/DespatchDemurrage/DebitCreditNote/ValuationTargetLookup?TargetType=" + encodeURIComponent(targetType),
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
                setCellValue: function (rowData, value) {
                    rowData.valuation_target_id = value;
                    //rowData.daywork_type = targetType;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            //{
            //    dataField: "valuation_target_id",
            //    dataType: "text",
            //    caption: "Valuation Target",
            //    validationRules: [{
            //        type: "required",
            //        message: "The field is required."
            //    }],
            //    //visible: false,
            //    //editorOptions: { readOnly: true },
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/General/Currency/CurrencyIdLookup",
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
                dataField: "debit_credit_date",
                caption: "Debit Credit Date",
                editorType: "dxDateBox",
                editorOptions: {
                    format: "fixedPoint",
                },
            },
            //{
            //    dataField: "despatch_order_link",
            //    dataType: "string",
            //    caption: "Shipping Order",
            //    visible: false,
            //    allowFiltering: false
            //},
            {
                dataField: "attn",
                dataType: "text",
                caption: "Attn",
            },
            {
                dataField: "re",
                dataType: "text",
                caption: "Signed By",
            },
            {
                dataField: "vessel_name",
                dataType: "text",
                caption: "Vessel",
                editorOptions: { readOnly: true }
            },
            {
                dataField: "currency_id",
                dataType: "text",
                caption: "Currency",
                visible: false,
                editorOptions: { readOnly: true }
            },
            {
                dataField: "currency_code",
                dataType: "text",
                caption: "Currency",
                visible: false,
                editorOptions: { readOnly: true }
            },
            //{
            //    dataField: "currency_id",
            //    dataType: "text",
            //    caption: "Currency",
            //    validationRules: [{
            //        type: "required",
            //        message: "The field is required."
            //    }],
            //    visible: false,
            //    editorOptions: { readOnly: true },
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/General/Currency/CurrencyIdLookup",
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
                dataField: "rate",
                dataType: "dxNumberBox",
                caption: "Rate",
                format: "#,##0.##",
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
                visible: false,
                editorOptions: { readOnly: true }
            },
            {
                dataField: "cow_bill_lading_date",
                caption: "B/L Date",
                editorType: "dxDateBox",
                editorOptions: {
                    format: "fixedPoint",
                },
                editorOptions: { readOnly: true }
            },
            {
                dataField: "cow_quantity",
                dataType: "dxNumberBox",
                caption: "Quantity",
                format: "#,##0.####",
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
                editorOptions: { readOnly: true }
            },
            {
                dataField: "desdem_valuation_type",
                dataType: "text",
                caption: "DesDem Type",
                visible: false,
                editorOptions: { readOnly: true }
            },
            {
                dataField: "total_time",
                dataType: "text",
                caption: "Total Time",
                visible: false,
                editorOptions: { readOnly: true }
            },
            {
                dataField: "total_price",
                dataType: "dxNumberBox",
                caption: "Total Price",
                format: "#,##0.####",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 4
                        }
                    }
                },
                editorOptions: { readOnly: true }
            },

            {
                dataField: "bank_account_id",
                dataType: "string",
                caption: "Bank Account",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/BankAccount/BankAccountIdLookup", // API for Bank account
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            // filter: options.data ? ["bank_id", "=", options.data.bank_id] : null
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "correspondent_bank_id",
                dataType: "string",
                caption: "Correspondent Bank",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/BankAccount/BankAccountIdLookup", // API for Bank account
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            // filter: options.data ? ["bank_id", "=", options.data.bank_id] : null
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "currency_exchange_id",
                dataType: "string",
                caption: "Exchange Rate",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        //console.log(options);
                        let _url = "/api/DespatchDemurrage/DebitCreditNote/CurrencyExchangeIdLookupByBLDate";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                //if (options.data.despatch_order_id !== undefined
                                //    && options.data.despatch_order_id !== null) {
                                //    _url += "?despatch_order_id=" + encodeURIComponent(options.data.despatch_order_id);
                                //}
                                if (options.data.currency_id !== undefined
                                    && options.data.currency_id !== null) {
                                    _url += "?source_currency_id=" + encodeURIComponent(options.data.currency_id);
                                }
                                if (options.data.cow_bill_lading_date !== undefined
                                    && options.data.cow_bill_lading_date !== null) {
                                    var _cow_bill_lading_date = new Date(options.data.cow_bill_lading_date);

                                    _url += "&invoice_date=" + encodeURIComponent(_cow_bill_lading_date.toISOString());
                                }
                            }
                        }
                        //if (options.data) {
                        //    //console.log('currency id:', options.data.currency_id);
                        //    //console.log('invoice date:', options.data.invoice_date);
                        //}
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                //loadUrl: "/api/Sales/SalesInvoice/InvoiceCurrencyExchangeIdLookupByDo?despatch_order_id=333333&invoice_date=4444", // + options.data.currency_id,
                                loadUrl: _url,
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            //filter: options.data ? [["source_currency_id", "=", options.data.currency_id], "and", ["start_date", "<=", options.data.invoice_date], "and", ["end_date", ">=", options.data.invoice_date]] : null
                        }
                    },
                    valueExpr: "value",
                    displayExpr: function (item) {
                        return item.text + ' (' + formatNumber(item.xchange).toString() + ')'
                    }
                },
            },
            //notes
            {
                dataField: "notes",
                caption: "Notes",
                dataType: "string",
                visible: false
            },

            {
                caption: "Print",
                type: "buttons",
                width: 120,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "See Terms Detail",
                    text: "Print",
                    onClick: function (e) {
                        applicationEntityId = e.row.data.entity_id;
                        recordId = e.row.data.id;

                        $("#print-out-list").select2({
                            dropdownParent: $("#print-out-modal .modal-body"),
                            ajax:
                            {
                                url: "/api/Report/ReportTemplate/PrintOutListSelect2/" +
                                    encodeURIComponent(applicationEntityId),
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
                            minimumInputLength: 0
                        }).on('select2:select', function (e) {
                            var data = e.params.data;
                            reportTemplateId = data.id;

                            $("#print-out-btn").on("click", function () {
                                let printPage = "/Report/PrintOutViewer/Index/"
                                    + "?Id=" + encodeURIComponent(recordId)
                                    + "&reportTemplateId=" + encodeURIComponent(reportTemplateId);
                                window.open(printPage);
                            });
                        }).on('select2:clear', function (e) {
                            reportTemplateId = "";
                        });

                        $("#print-out-modal").modal("show");
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

                // Not used again
                // Documentation-purpose-only

                var currentRecord = options.data;
                // DesDem Information Container
                renderSalesContractInformation(currentRecord, container)
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
            if (e.parentType === "dataRow" && e.dataField == "despatch_demurrage_invoice_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value
                    // Get another data from API after getting the Id

                    //console.log("recordId: ", recordId);

                    $.ajax({
                        url: '/api/DespatchDemurrage/Invoice/DataDetail?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (r) {
                            //console.log("Despatch Demurrage Invoice Detail", r)
                            if (r.success || (r.data && r.data.length > 0)) {
                                var resultData = r.data[0];
                                grid.cellValue(index, "total_time", resultData.total_time);
                                grid.cellValue(index, "currency_id", resultData.currency_id);
                                grid.cellValue(index, "currency_code", resultData.currency_code); 
                                grid.cellValue(index, "total_price", resultData.total_price);
                                grid.cellValue(index, "rate", resultData.rate);
                                grid.cellValue(index, "vessel_name", resultData.vessel_name);
                                grid.cellValue(index, "desdem_valuation_type", resultData.invoice_type);
                                if (resultData.invoice_type === "Despatch") {
                                    //-- Buyer
                                    grid.cellValue(index, "valuation_target_type", "Buyer");
                                } else {
                                    //-- Seller / Contractor
                                    grid.cellValue(index, "valuation_target_type", "Seller");
                                }

                                //-- Get COW
                                if (resultData.despatch_order_id === "" && resultData.despatch_order_id === null) {
                                    alert("Shipping Order in " + resultData.despatch_order_number + " is not found");
                                } else {
                                    $.ajax({
                                        url: '/api/SurveyManagement/COW/LookupByDespatchOrderId/' + resultData.despatch_order_id,
                                        type: 'GET',
                                        contentType: "application/json",
                                        beforeSend: function (xhr) {
                                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                                        },
                                        success: function (cowResult) {
                                            //console.log("cowResult", cowResult);
                                            if (cowResult.success) {
                                                grid.cellValue(index, "cow_quantity", cowResult.data.quantity);
                                                grid.cellValue(index, "cow_bill_lading_date", cowResult.data.bill_lading_date);

                                                //var laytime_used_duration = sofResult.data.laytime_duration;
                                                //var laytime_allowed_duration = r.data.laytime_duration;

                                                ////console.log("laytime_used_duration", laytime_used_duration);
                                                ////console.log("laytime_allowed_duration", laytime_allowed_duration);

                                                //grid.cellValue(index, "laytime_used_duration", laytime_used_duration);
                                                //grid.cellValue(index, "laytime_used_text", sofResult.data.laytime_text);

                                                //let subtract_duration = Math.abs(laytime_allowed_duration - laytime_used_duration);
                                                //let multiplier = 1;
                                                //if (laytime_allowed_duration > laytime_used_duration) {
                                                //    grid.cellValue(index, "invoice_type", "Despatch");
                                                //    multiplier = 0.5;
                                                //} else {
                                                //    grid.cellValue(index, "invoice_type", "Demurrage");
                                                //}
                                                //var text = secondsToDhms(subtract_duration);
                                                //grid.cellValue(index, "total_time", text);
                                                //grid.cellValue(index, "total_price", (parseFloat(subtract_duration / 86400) * r.data.despatch_demurrage_rate) * multiplier);

                                            }
                                        }
                                    })
                                }
                                

                                
                                //grid.cellValue(index, "valuation_target_type", "Buyer");
                                //grid.cellValue(index, "vessel_name", r.data.vessel_name);
                                //grid.cellValue(index, "despatch_percentage", r.data.despatch_percentage);
                                //grid.cellValue(index, "laytime_allowed_duration", r.data.laytime_duration);
                                //grid.cellValue(index, "laytime_allowed_text", r.data.laytime_text);
                                //grid.cellValue(index, "currency_id", r.data.currency_id);
                                //grid.cellValue(index, "rate", r.data.despatch_demurrage_rate);
                                //grid.cellValue(index, "currency_code", r.data.currency_code);
                                //grid.cellValue(index, "sof_name", r.data.sof_name);

                                ////-- Get SOF Details
                                //$.ajax({
                                //    url: '/api/Port/StatementOfFact/GetSofDetailById/' + r.data.sof_id,
                                //    type: 'GET',
                                //    contentType: "application/json",
                                //    beforeSend: function (xhr) {
                                //        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                //    },
                                //    success: function (sofResult) {
                                //        if (sofResult.success) {

                                //            //var laytime_used_duration = sofResult.data.laytime_duration;
                                //            //var laytime_allowed_duration = r.data.laytime_duration;

                                //            //grid.cellValue(index, "laytime_used_duration", laytime_used_duration);
                                //            //grid.cellValue(index, "laytime_used_text", sofResult.data.laytime_text);

                                //            //let subtract_duration = Math.abs(laytime_allowed_duration - laytime_used_duration);
                                //            //let multiplier = 1;
                                //            //if (laytime_allowed_duration > laytime_used_duration) {
                                //            //    grid.cellValue(index, "invoice_type", "Despatch");
                                //            //    multiplier = 0.5;
                                //            //} else {
                                //            //    grid.cellValue(index, "invoice_type", "Demurrage");
                                //            //}
                                //            //var text = secondsToDhms(subtract_duration);
                                //            //grid.cellValue(index, "total_time", text);
                                //            //grid.cellValue(index, "total_price", (parseFloat(subtract_duration / 86400) * r.data.despatch_demurrage_rate) * multiplier);

                                //        }
                                //    }
                                //})

                            }
                            // Set its corresponded field's value
                            //grid.cellValue(index, "desdem_type", record.desdem_type);

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
                        dataField: "debit_credit_number",
                        colSpan: 2,
                    },
                    {
                        dataField: "despatch_demurrage_invoice_id",
                        colSpan: 2,
                    },
                    //{
                    //    dataField: "despatch_order_link",
                    //    editorType: "dxButton",
                    //    editorOptions: {
                    //        text: "See Shipping Order Detail",
                    //        disabled: true
                    //    }
                    //},
                    {
                        dataField: "debit_credit_date",
                        colSpan: 2,
                    },
                    {
                        dataField: "valuation_target_type",
                        colSpan: 2,
                    },
                    {
                        dataField: "valuation_target_id",
                        colSpan: 2,
                    },
                    {
                        dataField: "attn",
                        colSpan: 2,
                    },
                    {
                        dataField: "re",
                        colSpan: 2,
                    },
                    {
                        dataField: "vessel_name",
                        colSpan: 2,
                    },
                    {
                        dataField: "cow_bill_lading_date",
                        colSpan: 2,
                    },

                    {
                        dataField: "cow_quantity",
                        colSpan: 2,
                    },
                    {
                        dataField: "desdem_valuation_type",
                        colSpan: 2,
                    },
                    {
                        dataField: "total_time",
                        colSpan: 2,
                    },
                    {
                        dataField: "rate",
                    },
                    {
                        dataField: "currency_id",
                        visible: false,
                    },
                    {
                        dataField: "currency_code",
                    },
                    {
                        dataField: "total_price",
                        colSpan: 2,
                    },
                    {
                        dataField: "bank_account_id",
                        colSpan: 2,
                    },
                    {
                        dataField: "correspondent_bank_id",
                        colSpan: 2,
                    },
                    {
                        dataField: "currency_exchange_id",
                        colSpan: 2,
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

    window.openContractTerms = function(contractId) {
        $("[href='#sales-contract-term-container']").tab("show")
        salesContractTermGrid.columnOption("sales_contract_id", {
            filterValue: contractId
        })
    }
});