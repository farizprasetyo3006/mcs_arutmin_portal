$(function () {

    var token = $.cookie("Token");
    var areaName = "DespatchDemurrage";
    var entityName = "Invoice";
    var url = "/api/" + areaName + "/" + entityName;
    var reportTemplateId = "";
    var recordId = "";

    /**
     * ===================
     * Despatch Demurrage Contract Grid
     * ===================
     */

    $("#desdem-invoice-grid").dxDataGrid({
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
                dataField: "invoice_number",
                dataType: "string",
                caption: "Valuation Number",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "invoice_date",
                caption: "Valuation Date",
                editorType: "dxDateBox",
                width: 110,
                dataType: "date",
                format: "yyyy-MM-dd",
                allowSearch: false
            },
            {
                dataField: "despatch_order_id",
                dataType: "string",
                caption: "Shipping Order",
                validationRules: [{
                    type: "required",
                    message: "The Shipping Order is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/DespatchOrder/DespatchOrderIdLookup",
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
            },
            {
                dataField: "despatch_order_link",
                dataType: "string",
                caption: "Shipping Order",
                visible: false,
                allowFiltering: false,
                allowSearch: false,
                showInColumnChooser: false
            },
            //{
            //    dataField: "despatch_demurrage_id",
            //    colSpan: 2,
            //    dataType: "text",
            //    caption: "DesDem Contract",
            //    validationRules: [{
            //        type: "required",
            //        message: "The DesDem Contract is required."
            //    }],
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/DespatchDemurrage/Contract/DesDemContractIdLookup",
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
            //{
            //    dataField: "invoice_target_id",
            //    colSpan: 2,
            //    dataType: "text",
            //    caption: "Invoice Target",
            //    validationRules: [{
            //        type: "required",
            //        message: "The Invoice Target is required."
            //    }],
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/Sales/Customer/CustomerIdDespatchOrderBasedLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    setCellValue: function (rowData, value) {
            //        //console.log(value);
            //        rowData.invoice_target_id = value
            //    },
            //    calculateSortValue: function (data) {
            //        var value = this.calculateCellValue(data);
            //        return this.lookup.calculateCellValue(value);
            //    }
            //},
            //{
            //    dataField: "sof_number",
            //    dataType: "text",
            //    caption: "Laytime Calculation Number",
            //    editorOptions: { readOnly: true }
            //},
            //{
            //    dataField: "valuation_target",
            //    dataType: "text",
            //    caption: "Valuation Target",
            //    editorOptions: { readOnly: true }
            //},
            //{
            //    dataField: "invoice_target_type",
            //    dataType: "text",
            //    caption: "Invoice Target Type",
            //    editorOptions: { readOnly: true }
            //},
            {
                dataField: "invoice_status",
                dataType: "string",
                caption: "Invoice Status",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/DesDemInvoiceStatusLookup",
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
                dataField: "customer_name",
                dataType: "string",
                caption: "Valuation Target",
                formItem: {
                    editorOptions: { readOnly: true }
                }
            },
            {
                dataField: "valuation_target_type",
                dataType: "string",
                caption: "Valuation Target Type",
                formItem: {
                    editorOptions: { readOnly: true }
                }
            },
            //{
            //    dataField: "sof_id",
            //    dataType: "text",
            //    caption: "Laytime Calculation",
            //    visible: false,
            //    lookup: {
            //        dataSource: function (options) {
            //            var sofLookupUrl = "/api/Port/StatementOfFact/StatemenfOfFactByDespatchOrderIdLookup?despatchOrderId=";
            //            if (options.data) {
            //                sofLookupUrl += options.data.despatch_order_id || "";
            //            }
            //            return {
            //                store: DevExpress.data.AspNet.createStore({
            //                    key: "value",
            //                    loadUrl: sofLookupUrl,
            //                    onBeforeSend: function (method, ajaxOptions) {
            //                        ajaxOptions.xhrFields = { withCredentials: true };
            //                        ajaxOptions.beforeSend = function (request) {
            //                            request.setRequestHeader("Authorization", "Bearer " + token);
            //                        };
            //                    }
            //                }),
            //            }
            //        },
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    //lookup: {
            //    //    dataSource: DevExpress.data.AspNet.createStore({
            //    //        key: "value",
            //    //        loadUrl: "/api/Port/StatementOfFact/StatemenfOfFactIdLookup",
            //    //        onBeforeSend: function (method, ajaxOptions) {
            //    //            ajaxOptions.xhrFields = { withCredentials: true };
            //    //            ajaxOptions.beforeSend = function (request) {
            //    //                request.setRequestHeader("Authorization", "Bearer " + token);
            //    //            };
            //    //        }
            //    //    }),
            //    //    valueExpr: "value",
            //    //    displayExpr: "text"
            //    //},

            //    calculateSortValue: function (data) {
            //        var value = this.calculateCellValue(data);
            //        return this.lookup.calculateCellValue(value);
            //    }
            //},
            {
                dataField: "sof_number",
                dataType: "string",
                caption: "Laytime Calculation",
                formItem: {
                    editorOptions: { readOnly: true }
                }
            },
            {
                dataField: "vessel_name",
                dataType: "string",
                caption: "Vessel",
                visible: false,
                formItem: {
                    editorOptions: { readOnly: true }
                }
            },
            {
                dataField: "laytime_used_text",
                dataType: "string",
                caption: "Laytime Used",
                visible: false,
                editorOptions: { readOnly: true }
            },
            {
                dataField: "laytime_used_duration",
                dataType: "string",
                caption: "Laytime Used",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                editorOptions: { readOnly: true }
            },
            //{
            //    dataField: "desdem_contract_id",
            //    dataType: "text",
            //    caption: "Desdem Contract",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/DespatchDemurrage/Contract/DesDemContractIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    calculateSortValue: function (data) {
            //        var value = this.calculateCellValue(data);
            //        return this.lookup.calculateCellValue(value);
            //    },
            //    editorOptions: {
            //        showClearButton: true
            //    },
            //},
            {
                dataField: "laytime_allowed_duration",
                dataType: "string",
                caption: "Laytime Allowed",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "The Laytime Allowed field is required."
                }],
                editorOptions: { readOnly: true }
            },
            {
                dataField: "laytime_allowed_text",
                dataType: "string",
                caption: "Laytime Allowed",
                visible: false,
                editorOptions: { readOnly: true }
            },
            {
                dataField: "currency_id",
                dataType: "string",
                caption: "Currency",
                visible: false,
                formItem: {
                    editorOptions: { readOnly: true }
                }
            },
            {
                dataField: "rate",
                dataType: "dxNumberBox",
                caption: "Rate",
                format: "#,##0.##",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
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
                },
                visible: false,
                editorOptions: { readOnly: true },
                allowSearch: false
            },
            {
                dataField: "currency_code",
                dataType: "string",
                caption: "Currency",
                visible: false,
                formItem: {
                    editorOptions: { readOnly: true }
                },
                allowSearch: false
            },
            {
                dataField: "invoice_type",
                dataType: "string",
                caption: "DesDem Type",
                visible: false,
                formItem: {
                    editorOptions: { readOnly: true }
                },
                allowSearch: false
            },
            {
                dataField: "total_time",
                dataType: "string",
                caption: "Total Time",
                visible: false,
                formItem: {
                    editorOptions: { readOnly: true }
                },
                allowSearch: false
            },
            {
                dataField: "total_price",
                dataType: "dxNumberBox",
                caption: "Total Price",
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
                editorOptions: { readOnly: true },
                allowSearch: false
            },
            
            {
                dataField: "total_price_final",
                dataType: "dxNumberBox",
                caption: "Final Total Price",
                format: "#,##0.##",
                visible: false,
                allowSearch: false,
            },
            {
                dataField: "laytime_used_final",
                dataType: "string",
                caption: "Final Laytime Used",
                visible: false,
                allowSearch: false,
            },
            {
                dataField: "notes",
                dataType: "string",
                caption: "Notes",
                visible: false,
                allowSearch: false
            },

            //{
            //    dataField: "allowed_time",
            //    dataType: "dxNumberBox",
            //    caption: "Allowed Time",
            //    format: "#,##0.##",
            //    editorOptions: { readOnly: true }
            //},
            //{
            //    dataField: "actual_time",
            //    dataType: "dxNumberBox",
            //    caption: "Actual Time",
            //    format: "#,##0.##",
            //    editorOptions: { readOnly: true }
            //},
            //{
            //    dataField: "amount",
            //    dataType: "dxNumberBox",
            //    caption: "Amount",
            //    format: "#,##0.##",
            //    editorOptions: { readOnly: true }
            //},
            //area hidden
            /*{
                caption: "Detail",
                type: "buttons",
                width: 150,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "See Terms Detail",
                    text: "Open Detail",
                    onClick: function (e) {
                        contractTermId = e.row.data.id
                        window.location = "/despatchdemurrage/invoice/detail/" + contractTermId
                    }
                }]
            },*/
          
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
                }],
                allowSearch: false,
                showInColumnChooser: false
            },
            {
                type: "buttons",
                buttons: ["edit", "delete"],
                allowSearch: false,
                showInColumnChooser: false
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
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value;
                    // Get another data from API after getting the Id
                    var LaytimeAllowedDuration = 0;

                    if (recordId != null)
                       /* await $.ajax({
                            url: url + '/CountLaytimeAllowed?Id=' + recordId,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (r) {
                                if (r.success) {

                                    LaytimeAllowedDuration = r.data.laytime_duration;
                                   // grid.cellValue(index, "laytime_allowed_duration", r.data.laytime_duration);
                                    grid.cellValue(index, "laytime_allowed_text", r.data.laytime_text);
                                } else {
                                   // grid.cellValue(index, "laytime_allowed_duration", r.data.laytime_text);
                                    grid.cellValue(index, "laytime_allowed_text", "");
                                    toastr["error"](r.message ?? "Error");
                                }
                            }
                        });*/

                   await $.ajax({
                       url: '/api/Sales/DespatchOrder/DataDetailForDesDem?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (r) {
                            if (r.success) {
                                record = r.data[0]
                                LaytimeAllowedDuration = r.data.Data.laytime_duration;
                                grid.cellValue(index, "customer_name", r.data.Data.customer_name);
                                grid.cellValue(index, "valuation_target_type", "Buyer");
                                grid.cellValue(index, "vessel_name", r.data.Data.vessel_name);
                                grid.cellValue(index, "despatch_percentage", r.data.Data.despatch_percentage);
                                grid.cellValue(index, "laytime_allowed_duration", r.data.Data.laytime_duration);
                               // grid.cellValue(index, "laytime_allowed_text", r.data.laytime_text);
                                grid.cellValue(index, "currency_id", r.data.Data.currency_id);
                                grid.cellValue(index, "rate", r.data.Data.despatch_demurrage_rate);
                                grid.cellValue(index, "currency_code", r.data.Data.currency_code);
                                grid.cellValue(index, "sof_number", r.data.Data.sof_number);
                                grid.cellValue(index, "laytime_allowed_text", r.data.laytime_text);

                                //console.log(r);

                                //-- Get SOF Details
                                $.ajax({
                                    url: '/api/Port/StatementOfFact/GetSofDetailById/' + r.data.Data.sof_id,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (sofResult) {
                                        if (sofResult.success) {

                                            var laytime_used_duration = sofResult.data.laytime_duration;
                                            //var laytime_allowed_duration = r.data.laytime_duration;
                                            var laytime_allowed_duration = LaytimeAllowedDuration;

                                           // grid.cellValue(index, "laytime_used_duration", laytime_used_duration);
                                            grid.cellValue(index, "laytime_used_text", sofResult.data.laytime_text);

                                            let subtract_duration = 0;
                                            if (laytime_allowed_duration > laytime_used_duration) {
                                                subtract_duration = Math.abs(laytime_allowed_duration - laytime_used_duration);
                                            } else if (laytime_used_duration > laytime_allowed_duration) {
                                                subtract_duration = Math.abs(laytime_used_duration - laytime_allowed_duration);
                                            }
                                            let multiplier = 1;
                                            if (laytime_allowed_duration > laytime_used_duration) {
                                                grid.cellValue(index, "invoice_type", "Despatch");
                                                grid.cellValue(index, "valuation_target_type", "Buyer");
                                                multiplier = 0.5;
                                            } else {
                                                grid.cellValue(index, "invoice_type", "Demurrage");
                                                grid.cellValue(index, "valuation_target_type", "Seller");
                                            }
                                            var text = secondsToDhms(subtract_duration);
                                            grid.cellValue(index, "total_time", text);
                                            grid.cellValue(index, "total_price", (parseFloat(subtract_duration / 86400) * r.data.Data.despatch_demurrage_rate) * multiplier);

                                        }
                                    }
                                })

                            }
                            // Set its corresponded field's value
                            //grid.cellValue(index, "desdem_type", record.desdem_type);

                        }
                    });

                    standardHandler(e) // Calling the standard handler to save the edited value
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


            //if (e.parentType === "dataRow" && e.dataField == "invoice_target_id") {
            //    let standardHandler = e.editorOptions.onValueChanged
            //    let index = e.row.rowIndex
            //    let grid = e.component

            //    e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
            //        // Get its value (Id) on value changed
            //        let recordId = e.value
            //        // Get another data from API after getting the Id
            //        $.ajax({
            //            url: '/api/DespatchDemurrage/Invoice/InvoiceTargetById?Id=' + recordId,
            //            type: 'GET',
            //            contentType: "application/json",
            //            beforeSend: function (xhr) {
            //                xhr.setRequestHeader("Authorization", "Bearer " + token);
            //            },
            //            success: function (response) {
            //                let record = response.data[0];
            //                // Set its corresponded field's value
            //                grid.cellValue(index, "buyer_name", record.customer_name);
            //                grid.cellValue(index, "valuation_target_type", "Buyer");
            //                //grid.cellValue(index, "desdem_type", record.desdem_type);
            //                grid.cellValue(index, "vessel_name", record.vessel_name);

            //            }
            //        })
            //        standardHandler(e) // Calling the standard handler to save the edited value
            //    }
            //}

            //if (e.parentType === "dataRow" && e.dataField == "sof_id") {
            //    let standardHandler = e.editorOptions.onValueChanged
            //    let index = e.row.rowIndex;
            //    let grid = e.component;

            //    var currentRecord = e.row.data;

            //    e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
            //        // Get its value (Id) on value changed
            //        let recordId = e.value

            //        // Get another data from API after getting the Id
            //        $.ajax({
            //            url: '/api/Port/StatementOfFact/GetSofDetailById/' + recordId,
            //            type: 'GET',
            //            contentType: "application/json",
            //            beforeSend: function (xhr) {
            //                xhr.setRequestHeader("Authorization", "Bearer " + token);
            //            },
            //            success: function (r) {
            //                if (r.success) {

            //                    //console.log("currentRecord", currentRecord);

            //                    var laytime_used_duration = r.data.laytime_duration;
            //                    var totalPrice = 0;
            //                    grid.cellValue(index, "laytime_used_duration", laytime_used_duration);
            //                    grid.cellValue(index, "laytime_used_text", r.data.laytime_text);

            //                    let subtract_duration = 0
            //                    if (currentRecord.laytime_allowed_duration > laytime_used_duration) {
            //                        subtract_duration = currentRecord.laytime_allowed_duration - laytime_used_duration
            //                        grid.cellValue(index, "invoice_type", "Despatch");
            //                    } else {
            //                        subtract_duration = laytime_used_duration - currentRecord.laytime_allowed_duration;
            //                        grid.cellValue(index, "invoice_type", "Demurrage");
            //                    }
            //                    var text = secondsToDhms(subtract_duration);
            //                    grid.cellValue(index, "total_time", text);
            //                    grid.cellValue(index, "total_price", parseFloat(subtract_duration / 86420) * currentRecord.rate);
                                
            //                }
            //            }
            //        })
            //        standardHandler(e) // Calling the standard handler to save the edited value
            //    }
            //}
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
                        dataField: "invoice_number",
                        colSpan: 2,
                    },
                    {
                        dataField: "invoice_date",
                        colSpan: 2,
                    },
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
                        dataField: "customer_name",
                        colSpan: 2,
                    },
                    {
                        dataField: "valuation_target_type",
                        colSpan: 2,
                    },
                    {
                        dataField: "vessel_name",
                        colSpan: 2,
                    },
                    {
                        dataField: "sof_number",
                        colSpan: 2,
                    },
                    {
                        dataField: "laytime_used_text",
                        colSpan: 2,
                    },
                    //{
                    //    dataField: "desdem_contract_id",
                    //    colSpan: 2,
                    //},
                    {
                        dataField: "laytime_allowed_text",
                        colSpan: 2,
                    },
                    {
                        dataField: "rate",
                    },
                    {
                        dataField: "currency_code",
                    },
                    {
                        dataField: "invoice_type",
                        colSpan: 2,
                    },
                    {
                        dataField: "total_time",
                        colSpan: 2,
                    },
                    {
                        dataField: "total_price",
                        colSpan: 2,
                    },
                    {
                        dataField: "invoice_status",
                        colSpan: 2,
                    },
                    {
                        dataField: "total_price_final",
                        colSpan: 2,
                    },
                    {
                        dataField: "laytime_used_final",
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

    // Functions
    // Not used - documentation-purpose only
    function secondsToDhms(seconds) {
        seconds = Number(seconds);
        var d = Math.floor(seconds / (3600 * 24));
        var h = Math.floor(seconds % (3600 * 24) / 3600);
        var m = Math.floor(seconds % 3600 / 60);
        var s = Math.floor(seconds % 60);

        var dDisplay = d > 0 ? d + (d == 1 ? " Day " : " Days ") : "";
        var hDisplay = h > 0 ? h + (h == 1 ? " Hour " : " Hours ") : "";
        var mDisplay = m > 0 ? m + (m == 1 ? " Minute " : " Minutes ") : "";
        var sDisplay = s > 0 ? s + (s == 1 ? " Second " : " Seconds") : "";
        return dDisplay + hDisplay + mDisplay;
    }

    window.openContractTerms = function(contractId) {
        $("[href='#sales-contract-term-container']").tab("show")
        salesContractTermGrid.columnOption("sales_contract_id", {
            filterValue: contractId
        })
    }
});