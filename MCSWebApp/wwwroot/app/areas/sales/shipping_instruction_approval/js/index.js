$(function () {

    var token = $.cookie("Token");
    var areaName = "Sales";
    var entityName = "ShippingInstructionApproval";
    var url = "/api/" + areaName + "/" + entityName;
    var salesInvoiceData = null;
    const maxFileSize = 52428800;
    var salesTypeId = "";
    var salesTypeName = "";
    var applicationEntityId = "";
    var reportTemplateId = "";
    var recordId = "";

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
        remoteOperations: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "id",
                dataType: "string",
                caption: "ID",
                visible: false
            },
            {
                dataField: "shipping_instruction_number",
                dataType: "string",
                caption: "SI Number",
                sortOrder: "asc",
                placeholder: "[auto generate]",
                allowEditing: false
            },
            {
                dataField: "shipping_instruction_date",
                dataType: "date",
                caption: "SI Date",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                setCellValue: function (rowData, value) {
                    //rowData.invoice_date = value
                    rowData.shipping_instruction_date = new Date(value).toLocaleDateString();
                },
            },
            {
                dataField: "despatch_order_id",
                dataType: "string",
                caption: "Shipping Order",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Sales/SalesInvoice/DespatchOrderIdLookup",
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
                setCellValue: function (rowData, value) {
                    rowData.despatch_order_id = value
                    rowData.quotation_type_id = null
                    rowData.invoice_type_id = null
                    rowData.currency_exchange_id = null
                },
                visible: true,
            },
            {
                dataField: "despatch_order_link",
                dataType: "string",
                caption: "Shipping Order",
                visible: false,
                allowFiltering: false
            },
            {
                dataField: "vessel_id",
                dataType: "string",
                caption: "Transport",
                visible: false,
                allowFiltering: false,
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
                setCellValue: function (rowData, value) {
                    rowData.vessel_id = value
                    rowData.contract_term_id = null
                    rowData.contract_product_id = null
                    rowData.turn_time = null
                    rowData.despatch_demurrage_rate = null
                    rowData.despatch_percentage = null
                    rowData.loading_rate = null
                    rowData.laytime_duration = null
                    rowData.laytime_text = null

                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "is_geared",
                dataType: "boolean",
                caption: "Is Geared",
                visible: false,
                allowEditing: false
            },
            {
                dataField: "sales_contract_id",
                dataType: "string",
                caption: "Sales Contract",
                visible: false,
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                allowEditing: false
            },
            {
                dataField: "delivery_term_id",
                dataType: "string",
                caption: "Delivery Term",
                visible: false,
                allowEditing: false,
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
            },
            {
                dataField: "seller_id",
                dataType: "string",
                caption: "Seller",
                allowEditing: false,
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/SystemAdministration/Organization/OrganizationIdLookup",
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
                dataField: "eta_plan",
                dataType: "date",
                caption: "ETA",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "customer_id",
                dataType: "string",
                caption: "Buyer",
                allowEditing: false,
                visible: true,
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
                }
            },
            {
                dataField: "ship_to",
                dataType: "string",
                caption: "Ship To (End User)",
                allowEditing: false,
                visible: false,
            },
            {
                dataField: "shipping_agent",
                dataType: "string",
                caption: "Shipping Agent",
                visible: true,
                allowEditing: false
            },
            {
                dataField: "laycan_start",
                dataType: "date",
                caption: "Laycan Start",
                allowEditing: false,
                visible: false,
            },
            {
                dataField: "laycan_end",
                dataType: "date",
                caption: "Laycan End",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "loading_port",
                dataType: "string",
                caption: "Loading Port",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "discharge_port",
                dataType: "string",
                caption: "Discharge Port",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "required_quantity",
                dataType: "number",
                caption: "Quantity",
                allowEditing: false,
                visible: false
            },
            //{
            //    dataField: "to_other",
            //    dataType: "string",
            //    caption: "To Other",
            //    allowEditing: true,
            //    visible: false
            //},
            {
                dataField: "notify_party",
                dataType: "string",
                caption: "Notify Party",
                allowEditing: true,
                visible: false
            },
            {
                dataField: "cargo_description",
                dataType: "string",
                caption: "Cargo Description",
                allowEditing: true,
                visible: false
            },
            {
                dataField: "cc",
                dataType: "string",
                caption: "To Address Email",
                allowEditing: true,
                visible: false
            },
            {
                dataField: "sampling_template_id",
                dataType: "text",
                caption: "Sampling Template",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/SamplingTemplateIdLookup",
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
                allowEditing: true,
                visible: false
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "desc"
            },
            {
                dataField: "approved_by",
                caption: "Approved By",
                dataType: "string",
            },
            {
                caption: "Print",
                type: "buttons",
                width: 80,
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
                caption: "Approve",
                type: "buttons",
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "Approve",
                    text: "Approve",
                    onClick: function (e) {
                        recordId = e.row.data.id;

                        $.ajax({
                            url: url + "/Approve/" + encodeURIComponent(recordId),
                            type: 'GET',
                            headers: {
                                "Authorization": "Bearer " + token
                            }
                        }).done(function (result) {
                            if (result != null) {
                                alert("Approve success!");
                                location.reload();
                            }
                            else {
                                alert("Data has been approved!");
                            }
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            alert('Failed');
                        });
                    }
                }]
            },
            //{
            //    type: "buttons",
            //    buttons: ["edit", "delete"]
            //}
        ],
        onRowValidating: function (e) {
            if (!e.brokenRules.length)
                return false

            // Ref: https://supportcenter.devexpress.com/ticket/details/t512157/how-to-automatically-focus-the-first-non-validated-field
            e.brokenRules[0].validator.focus();
        },
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            //if (e.parentType === "dataRow" && (e.dataField == "unit_price" || e.dataField == "downpayment" || e.dataField == "total_price")) {
            //if (e.parentType === "dataRow" && (e.dataField == "unit_price" || e.dataField == "downpayment")) {
            //    if (salesTypeName == 'Domestic PLN')
            //        e.editorOptions.readOnly = false
            //    else
            //        e.editorOptions.readOnly = true
            //}
            //if (e.parentType === "dataRow" && e.dataField == "total_price") {
                //e.editorOptions.disabled = (salesTypeName == "Domestic PLN" || e.row.data.sales_type_name == "Domestic PLN");
                //alert('Hai')
            //}

            // Set onValueChanged for despatch_order_id
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    

                    // Get its value (Id) on value changed
                    let despatchOrderId = e.value

                    grid.beginCustomLoading()

                    var _qty = 0;

                    // Get another data from API after getting the Id
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
                            if (despatchOrderData.draft_survey_quantity == null) { _qty = despatchOrderData.required_quantity }
                            else { _qty = despatchOrderData.draft_survey_quantity }

                            // Set its corresponded field's value
                            grid.cellValue(index, "vessel_id", despatchOrderData.vessel_id)
                            grid.cellValue(index, "is_geared", despatchOrderData.is_geared)
                            grid.cellValue(index, "sales_contract_id", despatchOrderData.sales_contract_id)
                            grid.cellValue(index, "delivery_term_id", despatchOrderData.delivery_term_id)
                            //grid.cellValue(index, "bill_to", despatchOrderData.ship_to)
                            grid.cellValue(index, "seller_id", despatchOrderData.seller_id)
                            grid.cellValue(index, "eta_plan", despatchOrderData.eta_plan)
                            grid.cellValue(index, "customer_id", despatchOrderData.customer_id)
                            grid.cellValue(index, "ship_to", despatchOrderData.ship_to)
                            grid.cellValue(index, "shipping_agent", despatchOrderData.shipping_agent)
                            grid.cellValue(index, "laycan_start", despatchOrderData.laycan_start)
                            grid.cellValue(index, "laycan_end", despatchOrderData.laycan_end)
                            grid.cellValue(index, "loading_port", despatchOrderData.loading_port)
                            grid.cellValue(index, "discharge_port", despatchOrderData.discharge_port)
                            grid.cellValue(index, "required_quantity", despatchOrderData.required_quantity)
                            grid.endUpdate()
                        }
                    })

                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500)

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            // Enable and Set Despatch Order Link button after Despatch Order selected
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_link") {
                if (e.row.data.despatch_order_id) {
                    let despatchOrderId = e.row.data.despatch_order_id

                    e.editorOptions.onClick = function (e) {
                        window.open("/Sales/DespatchOrder/Index?Id=" + despatchOrderId + "&openEditingForm=true", "_blank")
                    }
                    e.editorOptions.disabled = false
                }
            }

            // //console.log(e)

        },
        masterDetail: {
            enabled: true,
            //template: masterDetailTemplate,
            template: function (container, options) {
                var currentRecord = options.data;

                masterDetailTemplate(currentRecord, container)
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
            pageSize: 20
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
            allowAdding: false,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 1,
                items: [
                    {
                        itemType: "group",
                        caption: "Shipping Instruction",
                        colCount: 2,
                        items: [
                            {
                                dataField: "shipping_instruction_number",
                            },
                            {
                                dataField: "shipping_instruction_date"
                            },
                            {
                                dataField: "despatch_order_id"
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
                                dataField: "vessel_id",
                                colSpan: 2
                            },
                            {
                                dataField: "is_geared"
                            },
                            {
                                dataField: "sales_contract_id"
                            },
                            {
                                dataField: "delivery_term_id"
                            },
                            {
                                dataField: "seller_id"
                            },
                            {
                                dataField: "eta_plan"
                            },
                            {
                                dataField: "customer_id"
                            },
                            {
                                dataField: "ship_to",
                            },
                            {
                                dataField: "shipping_agent"
                            },
                            {
                                dataField: "laycan_start",
                            },
                            {
                                dataField: "laycan_end",
                            },
                            {
                                dataField: "loading_port",
                            },
                            {
                                dataField: "discharge_port",
                            },
                            {
                                dataField: "required_quantity",
                            },
                            //{
                            //    dataField: "to_other",
                            //},
                            {
                                dataField: "notify_party",
                            },
                            {
                                dataField: "cargo_description",
                            },
                            {
                                dataField: "cc",
                                colSpan: 2
                            },
                            {
                                dataField: "sampling_template_id",
                            },
                        ]
                    },
                ]
            }
        },
        onInitNewRow: async function (e) {
            let grid = e.component
            //await deleteAdjustmentAndPrice(grid)
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
    /** bob
   */

    function masterDetailTemplate(masterDetailOptions, container) {

        $("<div>").dxTabPanel({
            items: [
                {
                    title: "Analyts",
                    template: createAnalytsSamplingTemplateTabTemplate(masterDetailOptions)
                },
                {
                    title: "Detail Survey",
                    template: createDetailSurveyTabTemplate(masterDetailOptions)
                },
                {
                    title: "Detail Survey Document",
                    template: createDetailSurveyDocumentTabTemplate(masterDetailOptions)
                },
                {
                    title: "Tug Boat",
                    template: createTugBoatTabTemplate(masterDetailOptions)
                },
                {
                    title: "Ansuransi",
                    template: createAnsuransiTabTemplate(masterDetailOptions)
                },
                {
                    title: "Pekerjaan Shipping Agent",
                    template: createPekerjaanShippingAgentTabTemplate(masterDetailOptions)
                },
                {
                    title: "Dokumen Shipping Agent",
                    template: createDokumenShippingAgentTabTemplate(masterDetailOptions)
                }
            ]
        }).appendTo(container);
    }

    function createAnalytsSamplingTemplateTabTemplate(masterDetailData) {
        return function () {
            //console.log(masterDetailData);
            let currentRecord = masterDetailData;
            var urlDetail = "/api/Quality/SamplingTemplateDetail";

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/BySamplingTemplateId/" + encodeURIComponent(currentRecord.sampling_template_id),
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
                            dataField: "sampling_template_id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            }
                        },
                        {
                            dataField: "analyte_id",
                            dataType: "text",
                            caption: "Analyte",
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
                            }],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/AnalyteIdLookup",
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
                            dataField: "remark",
                            dataType: "string",
                            caption: "Remark",
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
                        e.data.sampling_template_id = currentRecord.id;
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

    function createDetailSurveyTabTemplate(masterDetailData) {
        return function () {
            //console.log(masterDetailData);
            let currentRecord = masterDetailData;
            var urlDetail = "/api/Quality/SamplingTemplateDetail";

            documentDataGrid = $("<div>");

            return documentDataGrid
        }
    }

    function createDetailSurveyDocumentTabTemplate(masterDetailData) {
        return function () {
            //console.log(masterDetailData);
            let currentRecord = masterDetailData;
            var urlDetail = "/api/Quality/SamplingTemplateDetail";

            documentDataGrid = $("<div>");

            return documentDataGrid
        }
    }

    function createTugBoatTabTemplate(masterDetailData) {
        return function () {
            //console.log(masterDetailData);
            let currentRecord = masterDetailData;
            var urlDetail = "/api/Quality/SamplingTemplateDetail";

            documentDataGrid = $("<div>");

            return documentDataGrid
        }
    }

    function createAnsuransiTabTemplate(masterDetailData) {
        return function () {
            //console.log(masterDetailData);
            let currentRecord = masterDetailData;
            var urlDetail = "/api/Quality/SamplingTemplateDetail";

            documentDataGrid = $("<div>");

            return documentDataGrid
        }
    }

    function createPekerjaanShippingAgentTabTemplate(masterDetailData) {
        return function () {
            //console.log(masterDetailData);
            let currentRecord = masterDetailData;
            var urlDetail = "/api/Quality/SamplingTemplateDetail";

            documentDataGrid = $("<div>");

            return documentDataGrid
        }
    }

    function createDokumenShippingAgentTabTemplate(masterDetailData) {
        return function () {
            //console.log(masterDetailData);
            let currentRecord = masterDetailData;
            var urlDetail = "/api/Quality/SamplingTemplateDetail";

            documentDataGrid = $("<div>");

            return documentDataGrid
        }
    }

    var salesInvoiceData
    function createAttachmentsTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            //let detailName = "SalesInvoice";
            let detailName = "SalesInvoiceAttachment";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            salesInvoiceData = currentRecord

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/DataGridAttachments?salesInvoiceId=" + encodeURIComponent(currentRecord.id),
                        //insertUrl: urlDetail + "/InsertData",
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
                                    xhr.open("GET", "/api/Sales/SalesInvoiceAttachment/DownloadDocument/" + documentData.id, true)
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
                    sales_invoice_id: salesInvoiceData.id,
                    //quantity: false,
                    //quality: false,
                    file: ""
                },
                colCount: 2,
                items: [
                    {
                        dataField: "sales_invoice_id",
                        label: {
                            text: "Sales Invoice Id"
                        },
                        validationRules: [{
                            type: "required"
                        }],
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
                                        return;
                                    }

                                    let newFormData = {
                                        "salesInvoiceId": formData.sales_invoice_id,
                                        "fileName": fileName,
                                        "fileSize": fileSize,
                                        "data": data
                                    }

                                    /*//console.log(newFormData)*/

                                    $.ajax({
                                        //url: `/api/Sales/SalesInvoice/InsertAttachmentData`,
                                        url: `/api/Sales/SalesInvoiceAttachment/InsertAttachment`,
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

    const prt_getClassElm = (prt_className) => {
        return document.getElementsByClassName(prt_className)[0];
    }

    const iconPrint = `<svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="#000000"><path d="M0 0h24v24H0V0z" fill="none"/><path d="M19 8h-1V3H6v5H5c-1.66 0-3 1.34-3 3v6h4v4h12v-4h4v-6c0-1.66-1.34-3-3-3zM8 5h8v3H8V5zm8 12v2H8v-4h8v2zm2-2v-2H6v2H4v-4c0-.55.45-1 1-1h14c.55 0 1 .45 1 1v4h-2z"/><circle cx="18" cy="11.5" r="1"/></svg>`;
    const iconLoading = `<svg class="prt-svg-spinner" width="30"  height="30"  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100" preserveAspectRatio="xMidYMid" class="lds-rolling" style="background: none;"><circle class="loader-svg" cx="50" cy="50" fill="none" stroke="var(--color)" stroke-width="var(--stroke-width)" r="35" stroke-dasharray="164.93361431346415 56.97787143782138" transform="rotate(159.821 50 50)"><animateTransform attributeName="transform" type="rotate" calcMode="linear" values="0 50 50;360 50 50" keyTimes="0;1" dur="0.6s" begin="0s" repeatCount="indefinite"></animateTransform></circle></svg>`;

    const initializeCetak = (all_datas) => {
        //console.log("Data yang akan dicetak", all_datas);
        //loadStyle('/css/displaymodes/mode-normal.css');
        loadStyle('/css/displaymodes/mode-printing.css');
        // <div class="prt-container elemen-cetak">
        //$(``).appendTo($("body")[0]);
        $(`
<div class="prt-container elemen-cetak">
<div class="prt-papersheet">
<div class="prt-paper-pad">
	<div class="prt-d-flex prt-fx-center">
		<div>
		<img src="data:image/png;base64,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" class="logo-adjust">
		</div>
		<div class="prt-full-w prt-right prt-bold">
		ORIGINAL
		</div>
	</div>
	<div class="prt-right prt-pad-1 prt-bold">

	</div>
	<div class="prt-center prt-bold prt-fsz-large prt-uline">
		Commercial Invoice
	</div>
	<div class="prt-sans prt-bold" style="padding-left: 1mm;">
		Page 1 of 2
	</div>
	<div class="prt-d-flex">
		<div style="width: 50%">
			<table>
				<tr>
					<td>
					<span class="prt-api prt-sans prt-bold prt_field_invoiceBuyerName">

					</span>
					</td>
				</tr>
			</table>
		</div>
		<div style="width: 50%" class="prt-sans">
			<table class="prt-bold">
				<tr>
					<td class="prt-right">No. :</td>
					<td class="prt-value">
					<span class="prt-api prt_field_invoiceName">
					</span>
					</td>
				</tr>
				<tr>
					<td class="prt-right">Date. :</td>
					<td class="prt-value">
					<span class="prt-api prt_field_dateOfIssue">
					</span>
					</td>
				</tr>
				<tr>
					<td class="prt-right">Laycan. :</td>
					<td class="prt-value">
						<span class="prt-api prt_field_laycan">
						</span>
					</td>
				</tr>
			</table>
		</div>
	</div>

	<div class="prt-vpad"></div>

	<div>
		<table class="prt-sans prt-fsz-small">
				<tr>
					<td class="prt-fsz-small prt-key-1">The Name of Vessel</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
						<span class="prt-api prt_field_invoiceVesselName">
						</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">Bill of Lading Number</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
						<span class="prt-api prt_field_invoiceBillOfLadingNumber">
						</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">Bill of Lading Date</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
						<span class="prt-api prt_field_invoiceBillOfLadingDate">
						</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">From</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
						<span class="prt-api prt_field_vesselFrom">
						</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">To</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
						<span class="prt-api prt_field_vesselTo">
						</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">Payment</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
						<span class="prt-api prt_field_invoicePayment">
						</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">Letter of credit number</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
						<span class="prt-api prt_field_letterOfCreditNumber">
						</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">Date of issue L/C</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
					<span class="prt-api prt_field_lcDateIssue">
					</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">Bank of Issuance L/C</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
					<span class="prt-api prt_field_lcIssuingBank">
					</span>
					</td>
				</tr>
				<tr>
					<td class="prt-fsz-small prt-key-1">Contract Number</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
					<span class="prt-api prt_field_contractNumber">
					</span>
					</td>
					<td class="prt-fsz-small prt-value prt-value-1">DATED</td>
					<td class="prt-fsz-small prt-dot-1">:</td>
					<td class="prt-fsz-small prt-value prt-value-1">
					<span class="prt-api prt_field_invoiceDueDate">
					</span>
					</td>

				</tr>
		</table>
	</div>

	<div class="prt-v-pad">
		<table class="prt-bold prt-sans prt-table-border prt-full-w">
			<tr>
				<th class="prt-fsz-small prt-head-2 prt-cola-1">
					DESCRIPTION OF GOODS
				</th>
				<th class="prt-fsz-small prt-head-2 prt-cola-2">
					QUANTITY SHIPPED
				</th>
				<th class="prt-fsz-small prt-head-2 prt-cola-3">
					UNIT PRICE
				</th>
				<th class="prt-fsz-small prt-head-2 prt-cola-4">
					TOTAL INVOICE AMOUNT
				</th>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-value-2 prt-cola-1">
					INDONESIAN STEAM COAL BULK
				</td>
				<td class="prt-fsz-small prt-value-2 prt-cola-2 prt-right">
					<span class="prt-api prt_field_retVal__quantity">
					</span>
				</td>
				<td class="prt-fsz-small prt-value-2 prt-cola-3 prt-right">
					<span class="prt-api prt_field_retVal__price">
					</span>
				</td>
				<td class="prt-fsz-small prt-value-2 prt-cola-4 prt-right">
					<span class="prt-api prt_field_invoiceCurrencySymbol"></span> <span class="prt-api prt_field_retVal__value">
					</span>
				</td>
			</tr>
		</table>
	</div>

	<div class="prt-uline">
			<table class="prt-bold prt-full-w prt-sans prt-table-border prt_table_items">
					<tr>
						<th class="prt-fsz-small prt-bold prt-head-2 prt-uline">
							GUARANTEED
						</th>
						<th class="prt-fsz-small prt-bold prt-head-2 prt-uline">
							ACTUAL
						</th>
						<th class="prt-fsz-small prt-bold prt-head-2 prt-uline">
							ADJUSTMENT PRICE
						</th>
						<th class="prt-fsz-small prt-bold prt-head-2 prt-uline">
							TOTAL ADJUSTMENT
						</th>
					</tr>

				</table>
		</div>

	<!-- div class="prt-v-pad">
		<table class="prt-sans">
			<tr>
				<th class="prt-head-3 prt-uline prt-bold" colspan="2">
					Guaranteed
				</th>
				<th class="prt-head-3 prt-uline">
					Actual
				</th>
				<th class="prt-head-3 prt-uline prt-cola-3" colspan="2">
					FOBT Bonus/Penalty Adjusted (Price Adjustment):
				</th>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-value-3 prt-cola-1">
					GCV
				</td>
				<td class="prt-fsz-small prt-value-3 prt-cola-2">
					4200 KCAL/KG
				</td>
				<td class="prt-fsz-small prt-value-3 prt-cola-3">
					.... KCAL/KG
				</td>
				<td class="prt-fsz-small prt-value-3 prt-cola-4">
					( USD ......... X ............ ) / 4200 KCAL/KG
				</td>
				<td class="prt-fsz-small prt-value-3 prt-cola-5">
					= USD. XXX.XX / MT
				</td>
			</tr>
		</table>
	</div -->

	<div class="prt-v-pad prt-right">
		Total Invoice Amount after adjustment is : <span class="prt-api prt_field_invoiceCurrencySymbol">
					</span>. <span class="prt-api prt_field_invoiceTotal">
					</span>
		<!-- table>
			<tr>
				<td class="prt-value-4">
					XX.XXX MT
				</td>
				<td class="prt-value-4">
					=
				</td>
			</tr>
		</table -->
	</div>

	<div class="prt-fsz-small prt-sans prt-v-pad">
		(<span class="prt-api prt_field_invoiceCurrencySymbol"></span>. AMOUNT IN WORDING : <span class="prt-api prt_field_generated__invoiceTotalWords"></span>)
	</div>

	<hr>

	<div class="prt-d-flex prt-sans">
		<div class="prt-note-styling prt-bd-blue prt-t-blue prt-fsz-small prt-m-r">
			<div class="prt-uline prt-bold">
				<div>Note :</div>
				<!-- div>
					Please pay to (In Full Amount)
				</div -->
			</div>
			<!-- div>
				<div>
					 PT. Indexim Coalindo (USD)XX
				</div>
				<table>
					<tr>
						<td class="prt-t-blue prt-fsz-small prt-colb-1">
							PT. Bank Ganesha
						</td>
						<td class="prt-t-blue prt-fsz-small prt-colb-2">
							BANK CORRESPONDENT : 
						</td>
					</tr>
					<tr>
						<td class="prt-t-blue prt-fsz-small prt-colb-1">
							A/C No 0910
						</td>
						<td class="prt-t-blue prt-fsz-small prt-colb-2">
							Bank Negara Indonesia, New York Agency
						</td>
					</tr>
					<tr>
						<td class="prt-t-blue prt-fsz-small prt-colb-1">
							Wisma Hayam Wuruk <br> J. Hayam
						</td>
						<td class="prt-t-blue prt-fsz-small prt-colb-1">
							Swift code: BNINUS33
						</td>
					</tr>
				</table>
			</div -->
			<div>
				<span class="prt-api prt_field_invoiceNotes">
					</span>
			</div>
		</div>
		<div style="width: 50%" class="prt-inline prt-m-l prt-mark-layout-1">
			<div>
				PT. INDEXIM COALINDO
			</div>
			<div class="prt-mark-layout-2">
				<div>BRILIANTO WIDRADJATI</div>
				<div>MANAGER</div>
			</div>
		</div>
	</div>
  </div>
  </div>
  <div class="prt-papersheet">
	<div class="prt-paper-pad">
		<div class="prt-d-flex prt-fx-center">
		<div>
		<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAfQAAADWCAYAAAAnxXADAAABhGlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV9TxQ+qHewg4pChOlkQK+IoVSyChdJWaNXB5NIvaNKQpLg4Cq4FBz8Wqw4uzro6uAqC4AeIo5OToouU+L+k0CLGg+N+vLv3uHsHCI0KU82uSUDVLCMVj4nZ3KrY8woBfRhEFEGJmXoivZiB5/i6h4+vdxGe5X3uzzGg5E0G+ETiOaYbFvEG8cympXPeJw6xkqQQnxNPGHRB4keuyy6/cS46LPDMkJFJzROHiMViB8sdzEqGSjxNHFZUjfKFrMsK5y3OaqXGWvfkLwzktZU012mOIo4lJJCECBk1lFGBhQitGikmUrQf8/CPOP4kuWRylcHIsYAqVEiOH/wPfndrFqJTblIgBnS/2PbHGNCzCzTrtv19bNvNE8D/DFxpbX+1Acx+kl5va+EjILgNXFy3NXkPuNwBhp90yZAcyU9TKBSA9zP6phwwdAv0r7m9tfZx+gBkqKvlG+DgEBgvUva6x7t7O3v790yrvx9ykXKn44cYVAAAAAZiS0dEAP8ABwAHqAz/1QAAAAlwSFlzAAAuIwAALiMBeKU/dgAAAAd0SU1FB+YCCQ44IsNgnsAAAAAZdEVYdENvbW1lbnQAQ3JlYXRlZCB3aXRoIEdJTVBXgQ4XAAAgAElEQVR42uydd3xUVdrHf8+5dyYhhZbQe++9gwgWQCwIKGVdewVfu+7ae3ddXduKba27NsSOItJLpIkQSChSkpAEUklPZu45z/vHvZNMIEBmJqHlfD+fSBIz987ce8/5nec5T6GSkhJoNBqNRqM5tRH6Emg0Go1GowVdo9FoNBqNFnSNRqPRaDRa0DUajUaj0WhB12g0Go1GC7pGo9FoNBot6BqNRqPRaLSgazQajUaj0YKu0Wg0Go0WdI1Go9FoNFrQNRqNRqPRaEHXaDQajUajBV2j0Wg0Gi3oGo1Go9FotKBrNBqNRqPRgq7RaDQajUYLukaj0Wg0WtA1Go3mNIPAIABk/0QAMUOwAEEBUPafsQCXv4JAbH/B91oieyplAfKbUsn5l0Fgcs7B9jk0mtrA1JdAo9HURZgUwACTLcLMCkQMsIIUALOAqQwQAEUKDGFLPDEIDLAt8IoZRIBggmICC4JgQDDAYEhisE/gne81Gi3oGo1GUwMs2b40dnvS9tFZ+Zk46M1Ds7bN20THNhxzsDQPOZ48uGGiQb36qB8RzSUFhRsS927bJiyBUX1HlvZo321RH1d3j4vCIEG2UvsWBmTb5EwMBYYi2yoXjopLMiBJwKWUvgmaGodKSkr0VdBoNHVN0I3tSdvD27Zq2y5x37ZpBRGFXVNLU88qMzyxxarYpYQ9LwoVDrALlmFBKIKbXOxhlMS4GnOzsNj5Ucq1tVAWfNiva7+DU1tMPQi2rX2f450hIFiBoMAgkOOWV6QFXaMFXaPRaGpq+gOBwVCQhgUPLHy46r9D0jnr7DzOn5FWljqgjIsBAiyyIJw9c4awX8u2a93FhBbhzQ40QuPfWjdu/fWZ3UZ81N3ow6b0/S1DCgUpGIIZpmJIIn35NVrQNRqNJlQYAMgJbmNb2AFAQYENQJGFT7Z9clFy9r6H95SmDslCFizhBTHBdFzoCgKKDIAUmC0IYri9brR3tc9t17jjHLfLmHNtryuTw6UbxAY8gsCkYLACsY5H1mhB12g0mhpCARBgItiB58qxvRlMdoDbwrzlRtL+3RcnZe38258lScOLTA8A6Vj27Fj5AmACSPkc63BLA03MpgXtXR3nDure79mJjcfuZOWGEnb0vL3tLnxLC41GC7pGo6ljE9YR9I9D9GD7v/7QcyhSsMjC63Gv3/OnJ+Wpfd7kMBMKHgEAAoKr3hNnAKYy0cXVLqdddLsHBnQZ9N+RkSMLAQkTEhLhICgQLCjnOL6AOo1GC7pGo6m7k1lVQn2Mv+eqDlDFi5gZyvTg3c0f9duevevn3dafzUtNCSYBt2Qnn/3w4xMbYBAiKIwHRfddcUHP887pGdHLUkQwFGAoE4AEizJIuACIcve/RqMFXaPRaMvdT1X5KK+pjmXPzCAiewGgTLy3+8MeWzK3LNjp+bONoSwwCIqqEnQFO3nNtFPXYKBnWLfM7s27Tb2q45UrpeGBUCZMaYLIC0uwU5xGo9GCrtFo6srERQSlFIgI7FReI0dUfT/zIRXZ/P9WCFHp91X9/WHCTnb9OJOBOdvf67Ija9fKPaU7m0phlUet+5/DFnWGwYCCAUswTEnoGN6hZGiLgeP/0mnaSigDTARTGY7bXqezabSgazSaOggzw7IslHm8eGPOnBnPPPss/jpjRudt27YPX7d+vV2ZtZIvnaBYYca0aVZS8t5v+vfrn37ZXy/7dUC//gpEx7CODRgogiIB5ki8n/CfbhuyN/2yRya1pSMKsQlmAWWUwFAKLg6DBNDJ1f7giFZDLpjRYdpqS0gYygDDBTtgTou6Rgu6RqM57WcsYNPmzcY/XvhH67Zt25zxn/ff7zJxwnn3fvn1N0ZUVJSrsLCwktjTEfO97fKtYWFh3L5dO6t3796fjjlzzMfR9aNXXDr1krIqT80EAQWvAIhNECQ+2fW/mxYlLft3psgSVf09E0MaEkIRDHZDQgFkQTChr7uX6tu29+iZrWasloIgWDqZ8drprtGCrtFoTrUJyPFOH7qXXb4nLmwXdlFxMW7+v5unAOL8+PjNfZl56M5du8pLwyAoEaxouQIw2rdti+EjRvzRslXLKx5/5JEtdiMWOPvwvnP4Us0YYIZlSrwS9+rbqwrX3lDi8sBwiskYKAPYbdeAJwliE2AnZ50UAILbEujTsNe+Pu369JoWOz2flNf+vFrQNUGia7lrNJoTSlWBaYoVHn3i8YYpScn9vNK6aufOndds27YNstIf++LBgxXAysfak5SMvckp/fv27hVfVlp6x7NPP/OKAQKYUa7s/u5wIpjSRJtmrf7WFd1mbimMjxZEsIggyYBhV6+xxRwASJZb7iAJyxBIyNneupm76TcytuxsYRiVFzMajRZ0jUZzSoo5AR6vFx99+NEZvy78dVb8lvjR6QcOtPV6vYfY0rXkLSACEWHz1gQUFRb+q6iwsN5rr7z6nP17UWXQHBFhZvtpeRkF7/1zV96OxyzDcqLV3VCOgB++jGAAChYpwPRibca6MbG7Gt04tdP0tw0ytH2uCf4Z1i53jUZz3IXc2d9m5/sbbryhRXhY2Ixt27dfuX7DhgG+ZmSOcxvkpJ/5rNdDo8lrfGIE0KNbV4wbN+7up5966qWj7cczBNYVxDX5KvG7/QlF2wSTgmA3pFMq9kiirsAQbAJQGBI9cE/PJn26XdJuklcLukZb6BqN5tSwIhxhvPqa69CuXbsB6elpj65auXp0anp6YxCBFdsK7ieuh26T16aY+06XsH0HQPRi45jGCXffedfPR/YyMIbUH5S5xL36Xyh232VyESyyQHykvX0C2IBJHhALeA2J+PxtHWIjm90OUi9C13nXaAtdo9GcCnzzzTeUmJj41/nz509OS99/SWZmFiQqLODadKtXB8GAoorFx7QpU/YNHjyo48033+yteoGiAMX4aPdXfZenrNx0ACn269k46ifxBfRJsnPb+7p6Z53Z96zu59U/K7u2FywabaFrNBpN9a0Fvwh0Zsas2TdHuN2uS/btS3lm06ZNrTOzs8stWH939vGSsiO50fkQT8CPP81v3bFThycZfJ+oai9dMZiA5pFNE2Ko/vo08GBiw2n9cmQfgIIdHCdgAFDYU7o3tnNW6nQVrd4kSBDCwLCc9LcTvczRnApo345Go6kVsWTH6t7w+8YGV19z7T1/bNy48aNPPvno1yVLWx/IynYywUNvrHLYpOa/OPATXyEEhBAgEpUWEf7/+vb1/SkuLcGGjX/cOO/bb1tWvQgwwADGtRxrNY2KTTRZgJmOEa7uZJyzYZeghUCBKEJ2bvZMJRiCCKxs4dd76hptoWs0muNrkfvllJMQuO3OO6KVZU3d9eeuf65aFRfjL95HLvgS7LkZggQiIyMQE9O4zOuVKc2bNWNLyi+ZuUwIgcjISISFhcHr9aKkpASlpaVo2qTJmbv37O4AcMekpBTAaZ1aIbsAKyAxIaFRakrKpcz86pHkGRIwwkWcKzfsCjYtWAG6zZkY6SVpZ67NXNthVOMRe9iQQLn7XwFa2jVa0DUaTa2JuF9NdThu4TKPB9dee+2sXbt33ZO4fXsnKblci45eve3YVv+hrzUMA82aNcXQwYMSCgoKFjdv1uxHIcS+m//v/7b27t2bywPqqhRQ+985c+a4NmzYMHDwkMG37N2z9/INGzaCqSLCHkTYl5aOjAMHZhLRq1XKOQNMAj1a9ViemLUD6eqA814DE/VdZbux9+CeGSMaj3wO8IJgOB3WtaBrtKBrNJpaxF9k167f4Fq48JeZa9b+dv+y5St6SKVslzdVtDwLxTIXjkBGR0WhV8+e+S1attgqFT7v3qPH6w8/cL9k5vKAuqMJ+aEehdmzZnnBWANgzXMvPP/PqKioRzZvjp+Sc/Bg+SGYgLz8/BFSSpimWXkf3e9kZ8aO3vorlmI/pwf1WS3DQnpB5sXSsJ4znaYugFPYRm+ha461wNZR7hqNJlRRv+uuu4anpOx7YfOWLSPT9+834JcnXu5mD1KQfMLYqmVLjDlj1IKYmJj/NGnWNO7O2+9IqWphEWyOuu8Yjzz2aLjX4/3iv59+elFO7sHy99+hTWv84/kXrpg4ceInh1no8ELBBSUsvLz+DV55cDEsw7SrzAWyaGEDndwdMy7tPaXv6PqDD0gKA8ELkHKi5jUabaFrNJoaF3Lgldde67ItMeH5FcuXT0lOTSt3UcOxllFeC93ZW6+2vtl/2LJ5czRt2uz3ESNHLomJjX3xvnvu3n+o0DMzhBBHbJtabTF1jvH444+VChKT8gsKvvzuu+8uzcnPA0BISk5BWVlZ66rfrf1eDBbIQe4cJpqFIN6HUASP8DTJR14LgnGg3OOgc9M1WtA1Gk3NQgAIb739dvimTZueWv/7hlmJ27ZFsr8/2Cesvj/nyru/5RY7CKxUeW9yVgogghCEdm3aeEaOGLGmZ69eL3i93l/uuetuT1UWdVXfh+JpsD0JDKUUWrRo/tezzz4r8tvvv5toWQoMRnFpyREWHyYICkIJQKoihgliFXAEvzQUsksy6GDewZFWfeMPghcsFEiZ0D53jRZ0jUYTmoQ7Lmx2hO7e++67fN++fc//svDXlh6vx7aUBVXbvWwHkPmVf3Ve53K5cN748YiKiv53n769Xrv11lu3nchGJQ898KDn+x9/uDcjI6PfqtVxLRUz0tLSqvX5/NYyAaGY4VVeKOKOTLbdT6yD4TRa0DUaTQ2gnJzyDz/6sPcfG/94d968b4bm5eWRAoMcN3WgMVvkK8xOQHREJDp36pw5cuSI90ePHv3qhedfkOozfGu7ZvvRFjEAcOGFF8YnJCQ8sXPHzjf3Z2RQrb8XIjBJHCzJBQkFZtLd1zRa0DUaTUjKYos5GB6PB3ffffcza9etvW3b9h2RFSXKKah9a9/eusvlwqCBA3e1bdf21aZNmnz47NPP5Plb8IEet1auAgN//9vf3oqP33Ltt99/N7R169a1fNUJkixkZGfFKFIgNmCA/Zu2ajRa0DUaTWBIpfDww4+MSU5O+uTbH35oDQKC1VefMAsihLncGDZ0aF6TprHPvP322y+4Xa7Kf3tI8NyJ9jgTCC2atXiiXeu2P4SHh9fquRgAC0akGdmDSQFwKsmRrhin0YKu0WiC4Lbb7wgvKip65/cNGy7/c88e+CqocYXxHpArmIggQDhj1EgMHjzk3VatWj1904037D1VAr0U8y+t27RaHx4ennosSeZQfORkb3FAwO1Uez+s05xGowVdo6nDVJULXskKdvbDl69YRQsWLJialpb6ws8LF3Y8RNWqVbDFX8SZGYKA3j17FHft2u1/Uy+Z+q9JF160tcIePfr7PTEcniL2zxdf8M6ZM+eps8ee9W1Vf69IQTCBCXAbrpa2l0EAATrLDbZgwQWwtIgFBCRYAIJFaAsFjRZ0jUZzelClEPu50BnAwoWL2n722aevxMfHX7xt+46Q+nsR2SlpsTExGD/+3A0jR4688corr/xdkAjeb3+CmTVr1rdHssp9WEIizBs2w1e1LrjFF4FIuGG3bwHrdDWNFnSNRlOF5lT+lZM+9tzzL9ww/8f5f9scH9+Fmcv7gQcmRFxu9UdG1MMFE8/f36ZNu/977NGH55XncJ0Gfb4r1a8/TNAt5JTlCEAEFcrmd3WU/+9Iu9w1WtA1Gs1houFXJvX/brs9LMxtfjL/x5+mpKWlG4oQ0H6tHexOTuqaXbHtzFFneIcMGfJCy5Ytnrvx+hsKGTitaqIc3iSGIZxWsCtzl4cXiyKQRWAR3Ie2O7wpSXYPNwCkbXSNFnSNRnM4vhKnt9x26/Bdf+56d+XquF4V6uz/zbFlxBZr273cvUtXDB027IdevXs9OfumWWt9vcVPpoj1WrHYyx0PhNSMtGtyvLlOP5ogotmYICBQ4M3/k5T/vQhlA0SjBV2j0Zw+ouOX271o8eLIBQsWPL52zZq7ErfvIJ9o2JanCEg8CEBERATGjx+f3at7t/sbNGjwzs2zby4/Xp3weECBYABESM1PRSlKARiVL3oA9wkASsyydFLa1a7Rgq7RaA4THduXfuNNN7UuLi76YeWqlf2ycnIrWZAV/bv5SMZjeV1239fA/v3RrXv358eMHfvqZTNnpFUl5Kd/5VITxBYssqC8YrxHMAR7IZQLimSAKy8TJjM6N+mu7ABCgIUFcJh+iDVa0DUaDbB5yxbx0ksvTS8tLfn0x59+gmIGCV85tsCsRwCIiojE+PHj9gwcOOjm22+77Wc7RU3WyWvLICgirC/ZaB705rdmsFOrPvCgOEUMl2Q0MKPZ10tep6FrtKBrNHUdJ1jN4/Hg5ZdfnhsXt/rC9P37wYxK7UYDpXPH9pg06eLXunXt/uRfL/tLJsB1xr1e5WUGAzCQuG9bqzL29BcgkBKQhhdCBdjDXCiYwg3lkVtJUPnevEajBV2jqcMwGHffc3e//en7P/7+h/l9fH3JQRWtSo99EHb6mwMR9eph/Lhzt486Y+T1s2bNWklOIvuJaqByMl1pkEBhWX7LTJVtggHBBizyBH4kZsS6YzgiImyjYgWCALEIqhWrRgu6RqM5pREgAm697XZq2rTJrNWr4p7fmpgYfZgYHEPMGXaetRB2gZge3burs84+6812bTvcO/umm4pQofN1XMxtJBTyi/MnFslix6BWQQUPCAUQU1ar8BYZBAJIASygI9w1WtA1mjrI3LlftcjPz797wc8/3526f39QHlsix/oWAuPGjdszfvz452664ca3ffnXvt7opMOwbZcHKcBNtwKAIoIJCcFG4Edigcio6IxBDQekA76qes7xtetdowVdozkdqRwqRSAoZrz82itNFy1c+PO6dev6lpSUwt6H5aOLkf8xHRc7MdChXXucdfZZP44cOfK26dOm78Yh1njdE3N2RJfAvowAUoAS+G739332FCQ3YNg13ZXwAuzGkWu5s9+d808TZLQMa5FnwgTKy74ydK81jRZ0jea0FRY+TIz/75ZbpmRkZX24fOXKaDtCGtUoteo7hvKpNAjAuHPOKevTu/ftgwYNenvSpEna3wtfW9eKUnq+Nq9ECgeKsiZkleYSTAli0+5MR+ooHekq7h+DHOlmGDDRKLLhgvJ4BwCCGZKErhen0YKu0Zxu2MFoVL5Fe/8DD4WZpnHL2rXrXty2c0eASwMuFwoCEBFeD2PGnLn+oosm3X3F5Zct11fb77qz3YVNCbvMLTFBsIF1JesoJS91vG2VE5SwRV3w0dzk/h3dFBTZxwzjeujWtMNWJQSYAYMdsafA2tVqtKBrNJqTXVTKI8ptC6/M48HWrVs/+WPzpktzcnODs/Ydj2//vn0xatQZ7/fv1+eemTNm5OirfeilsluYKlIwwHa8GhM27d3SoAiFo1nY7nYJCWKXs+99BDlngiL2i5uzLf/ujbqisVH/GwmGwQo6tF2jBV2jOV01xedCFwK33nprN6/XWrJsxfIWUqkqmoYcGwHAIAMDBvRPnzF9+muzZt34rL7KR1n8OMJOrEDE8BoShtu8Jd2zP1yxBQE3iA0QVED73sQEoQj1OHxJ18ie0ipvzKLj2zVa0DWa09I6V0qBhMDTzz47ct2GDd8lJCTGqCCE3IfLMPGXmTMye/XuM3bWrBt3+BYNOnq9qhsgAQgQE4gNQHjhMb3YfzDnmjIuBgmCYoJgARNlKCMT4ogxDFz+RRAwlP3VJKLhMgUXBCsQFEAGFHCU42g0WtA1mlPSQFRK4frrr/97UnLS81u2bHV6ZROguDwvvLq0b9sGF11w0aeXXXbZVX379vb6rH8t5kfzaDAUbAtdwcC3W+f221uwpxWjciOWiqiEqu+I8nfHswCI0NhshMgm0csUAyaoPGNNo9GCrtGcJlY5sx0H/fkXX4YtWLDggSWLlzyUffBgeUQ6UP2tVnJS0kaPHuUdMWL4wz179nypT59eXl0cplpXD8QMCC8UESSbSM/fNyUD+8IkMewNDAkmwILhBMUd5VjlaWkCCl40dTfNah/RYZMB5VT08xWU0aqu0YKu0Zwuhjlm3Twr1vLKn39d9OugnIMHA99XdYxF0zRxztixqeecc84ts2bd+A1psQgIrxAQUGAY+DTlU5ElC6+3grgVdlCcgqkMSEiYzGga1WTlkKgBuVD6Omu0oGs0p511rpTCv159JfbPP3f9uHb9hkGSVdAGW7jbjSsvvyK+W7duo2fddEOeNv4CFWKuWGERgOLi8buL97ZSQVxDhoAiCaEEQBL1UA/dO3T9zpQmWOenabSgazSnF0opPPLYo+O2bdv2vzVr18QyhF0FlH2qcgzRYEA4LVK7demKceeO+2LYsGG3Tb74ojw/o11TXUFnsidMBrymB7vzkx4tVgVQRmA54gy7hoAkAAIwmdAxukuht7Tsf4jSd0WjBV2jOe148umnzvstLu6njMxMdOzQwW+jvPoTPoPRtEkTXHD+hc/ceccdD/qXgrUtQW2iVxdJgKkYDIFvk789M6U4YzCcgjCBXkdfvTkmCUO60Ci82fcXNLmgjLWYa7SgazSnHw0aNNjZpm3bfm3btXPUWfhMxWqLOjOja9euuOuOOzbbZr9TSAYMJnZKmWqq5TERdiEZBpCQsX3qAZVrKiIn8j0QGORUgWMohCOMWzVo+bXBAqybsGhCgEpKSvRV0Gg0mmqY1RLAwrSFrZbuWrZvs9wKl3SByS7dGoiggxjgMDAUBkX2KJra9ZLY/g37lmoDXRMKQl8CjUajqaYUE2N39p5ndpbtAAGwDAkmGfjKgAlgCZditIlqM6dvox6lQmrLXKMFXaPRaI6Lif7Z7i/bbM/bcVGpywNFJsAqyNamAkQKbV0tEOGq97LBht2hTV9kjRZ0jUajqSUZdyrnKTCKCwtmp3pTG5EywWCYDAgOPBRJwo6abxrebEnnFp33QxmQQundc40WdI1Go6ktfFkBi3OXdt5buO9+D3sAFk7Ee3ASLECI8NZDh9hOXwyOHioJdlU4HeWu0YKu0Wg0tYgihS3JW27dUbgdFjFY2NXdJAlIYQV8PAGFPo17FV7cYfIcO+qdYSqAdFEZTQiccmlr/lk2+tnX101fk5MFAtiAwRJMClLYmdbEXKPXlkCQZKd3CWYYzJAwjtp7PODPAQHBFohNWELBAPDBjv81TSpMvrpMlEE49fWVrzQPH+1oTjMXKBhsB8N5DYV60o1m9Zo+H0luKDAYBgCFQw/mu3ahZheerM96XRqDxIffR6aa/dzmqXxxNPp61fQA0wQ9NQOiDBaUHcANAaFMEAsw1ZwjmaBA4PJqeZIAJlWDzzcDkFAwoAwLJhO8JJFbnPtEcklKfTbsPyEQFPkyxumISwN/m5zBYGHnmbdxty6Kiq7/sVAAGz5X++Ht8mpqwj9Zx39dm5cOnXNq+vObx+PmHGnSPOKHOUobyKpeoydmTSCThLbMawnlAgsGKYYJBcCCJQCCqLHrbAnALRmGAqQQkEI5lm1N7R7aRXuUMKCEBWGZ+GLv18N25e++vszwgJkhhEAg3ekIALGAFF4oUgizwtG8XrPPL+0wJYk8BDbgCH3ViwM9t2lD4oQIut2bueL7QNwtYATcz/l0f+B9rTNr8kEiIl9zxorrx3xa9cBmOnxaLC9/TrV3zvLp+HReJBDKe7Cz8g8JYxiQYDbAJCCd8azIgGCrhlzi5DQzYXgNgoQBwQIu9kCRgqoRUScABggKLkn4KXelkXww9ZFkK9UgewAFNCadIruOp4IgWKCVaIkBnQb+0+11QRBAzjaFXbWPqpoInBa6oc0lUFzlM8vMIBHAxF2jA8duDVwXFi7H4/PVqKD/97PP6+Xn5d1rGAaUCn4Au93upddec9XSqh9MA2+++eY0IUQv5uMXE+pyubA/PfXphx566Lj1jvad55133jlHKYwOvGVmxT1o3LDRH9OnT/8GzIfNGUSEd999d5xUPOp0aItNAEgIREVFISoqShYWFr5bWFKcb5oGmjdvZk2cMKFMwO5kFqi1VRVSKcz9au7tubm5DZUCCRKHLh9CXUoc6Vgn5ByKLRiGASHEbzdcd/3PFdfd7gW+IHFpF1nP+1dplsDNBK80wRAhfw4GQwqJemTKg2UHn5vZ+S9eUgbKDAPEDME19PSwAFAGECGnIPfcnfl/ng8h7dqvAbka7D12Jgm7rQshzHKhS0yXr89tOjaBvATbAW+UK+mh1ttXX88bu3///jGm6SKnvXr177nf+I+IiHjpqiuuzAcfvuf/zXffUn5+/iMlJaXknDyQ54qP4FrgQy7EYf+PiEhJmX7mmWe+1atnzxO2Ev7ggw9GlJZ5xwf7OYL5vb0gViCip2666Uarpj67WZPTaFi98Hv/PnvWo8G+NZ87q0H9+qha0O1J+O/33jsdRJfC2Xmq7ceAiEBEuP7qq4asXbt2ypAhQzzH00pPTk5+8JXXXj8r4CUSKXtrE0DPnj0/mj59+jdHWiXm5uZ+9cTTz0SfLsYlESEsLAzRUVHo3LnTEy1atIQQhOjoqH3LlyxZ5w4L29G6deu1HTp0WH/uOecmixC8IWWeMrz99tu3r1m3rkNF243T2EgnBphx2cyZB66//vrmPo2zhAEBwOsqbrUxZcN9m0sSwizDgp2kRTUyxxAbCJdh6Nu456j3tn908exOV5URUQ1aPwyQhCBg+cGNMdvSt7ycJdMhqcL7GODSEorsYDeDCbEi1ts6ptW/DClgGRJgAaEq13AnZviM9a/nzbvw2++/v5uDea6c8U8APv7oo33M/K6/yvgWD//+979FelraY0kp+8DH49l1YgkFAx07tM9PTEz8+dVXX0063oJORJBSYv78+c/+9MvCMXycPre/t/Scs8963hb0k85CZwgCwAogUdVnqLZb6MiLYJ8Dy94zO163n5nBzJj/808TmzZvdtmQIUM+qFi31rQft/LxmBley1ukgjiPz8NgQ84AACAASURBVJmlwCAh/G5GxbGICMpeSLn4NGqQzcwoLS1FaWkpMrOy/J+f1gBau1wuNImJQd8+fTxxcb/tCo+IvLdDhw6Lpk2dUgxH2Kv77NpbI0BdaaqhFEPQoQIjwDDhshQmd5ywlCKsyXIXz433JkZ6yQKRhM/1TKxg9xAXAPiwSG7yiw/zDTFfcJgSDMsswe/5myYMDR/y9ccpn0+9rN3MUgGGJHuUmAqwBDsiCp93oPI9YzsC3WsQhCKQYxwYTFCCIImQmL7pmm3F23qALDAZld5rwF4cYcGUJto1aLuqTevWK+CcURCDy13tfNg2ZEZGVhEHvUisMLKJyF1+PZ0LzH6jgnEcq9Q5J1Jg7N6bVH/osLJ7QbgZXPPbjEfRVChm3HHnnX137vxzNB/Hz13+rLPEosWLavTUNZqHzkoetooNdABQNa5KsHvtobIvPY1+i4t7/6133u5s7zmx31cNXcNDLxgBJHxqHOC19NuTsxcl/oJeIXz23Hz6W5YV1jPB67WQfiADPy/81f3Ci//s8cH773/384/z1785583Lq3S2HWsgkUBdsM7tBYyoYkOQQeyFZVggmLigxaSf+7TtN71PvV7Sbg5nAsqEJIYiA4DpiIiAIvuLISp+9n2Piv9n/2tCKKBEFGFtxoaJKXlpc/+X8rnBisAgCCcJzFQEYluYj/Y8MAgG22dSwl5oCEVYV/x76z+zdv29UBTaC+Mghrid2CYg2ASTQgtqhj6t+/xtKA1gABBMdtP6o8QXEAkK+pli8n8mDVQS8Mo6c6IW8wqMbTu2z/7qm2+6+uaj4yWqAIFIPLVr957jUo+l8paQM+/WsH7owjIByYGBRUuWYs1vaz586JGHTfuBMKDbHZ6qlqYqDwZMTknBl3Pn9njn7Xc/fuTRR95ftmJ5rI4uDlQ/lJ2DTgQXuzG5/aT5w9sOPb9vve6FhkVOu1ZAwQCYgrIJDVhQpGAoA8WiGL/lrL0gM//Aj18emBfBfm1lBRsw+MhjUxHBEgKmUnbuPJQdWEcWNpStMVdsW/l6oie5iSQBRbalH1SrWWaQcsGlXGjqjnlrYvMJ66lCW+v2fCoEwIzfN27E4kWLbvT3eNXeM1rx/QfvfzBg3ryvLzox47x2TqoFPaCxaZd6XLFy5cjYmNi3fe4hZp37dGpampXdewqMnbt24b3/fHD1okWLlqxcvSrW/95rju0RYmfPWIERZrlxfsvxvwxuPXj6wAZ9lMEKBMPeuxUyYDm3fWICXsOAEgputuClUqzOXDshLSt1bhkVQMIFJdyQQkFBwVB0lPnUtswlAQwXTMsFkwV2HNh76Y6DOy6QRiEUMRRV4TmrpjnIQoKFQgxiinvE9njDxeRMu3q1yMzlgQmbN/0x+bEnHo8+XmONBGFV3Mp78/LzgwmOqGlXgRb0E0n6gQz879NPL5/z1lvn1PaKUnN8V8wMIL+gEG/Oebv34sVLv/Blawihh8oxJxNlp5GB2M65FhYMJlzU5uKfBrbsN7Gvq28eKYCEFwiyDjopA4Y0YQkJrwAM6UIx5SMuc/XEj+M//WlRxq+RzAoMOxXMOkLoOzEg2I7GsYQBxQYMElhTuCFq64Htj2bKLNOAXY5VkbDz6TkovztMBvo06j1/Wo+p8Swk7Lx5jT8bN23qlJiYOJEPuUe1VSfi2eeei01PS5+kTphBpi30k2dlScDWxG2upUuXvvraa6+Z+oqcLoLuE3VGSWkp/vfpp2e9OefNBwCElIZZV1DCrtgmlF3qVJGEIoaLCZNaTvllWIfhlw6O6g/DIoBMBB7kaVvKpmIQm/AKghRegA0UiCIsO7DmvOTMvfMWZS1wyrLS0Sc4ti1lhoAA4zfPb1i3e8NziQUJ3S2DbJFnBaGEvd8dRJS7YAM93V3RLrbtnaQMkDLgF8yiKb9ShIO5B1/5cu6XtXsep0Xt+nXrrly1enW98vz702VRrR+l4FmwYGHPHTt3zqkrwVCn+TLtMG3fl5qKZcuW3/nxx580J22hV2Ohy07xFAKxgGDhlIAVUCQxsc34X3u37HVhr3p9igWTn2eLD/GRHO0cEkpIGErALQEWXniFAFMY8ikXS/YvH783M3leXNHqMEOZMNjO9PDpMftKtpbnewuYbB9nZ9reUYm5264vM4phOXFkvv1zwQRFR11q2N4JEFhUnMtgF5qENXnmorbn77O3Gww9U1R5BRnxW+Kb79mz97ZqPAYhDfHPv/i8WXFJyVNSqRO4UNcu95NnNencC4+0sCou7pp/v/nmbF9TB82pLOh82Ler4n6LTdq370ZtUFVnXNgypnx5Z46sSSeFzC0NTG09+ccxncZc2De8l0dIZ7uKfXvUbFvCx5iy2BF2BgHsgsESQkkQCDlmIZalx03em7Z33pLCpW6pyKktb6fN+bI52OfPZYZghQX7F7v352R9naqSwxQIBluwayraos5HTVlTzucXYHIK0Tr5390juhQP7tB/jku67etCli7lWrXpjPzCYmzeFD/9118XG0QABNXstXKOtX79+hvXrttQj0EncLtUu9xPSnbu/FP88ssv93/99dc99Fb66Ud+QQH+3LHjJh0UF4rl7ot+B4Ry4awWY5cMaD1gav96vUqERXagHJSTqmYGVWGCyAuGnXKWY+TSD/sWn78nfffna/OXuSwQDCYw2alshhJ26VgGBAGWKMOezD0vbMhZ3aRUBONts2MHvIYHUnjhkgIgEw1UJLrHdPr7mTEjU8Bkizzbe/Kaw9fTBGDjxo2jVq1eOZRRO8Fxr73xuis5KWl2aWnpSWBAaEE/iR5ALt+PWbxkSZvPPv/8ifc/+NCtL8zpdpsZq+PiWr7/wYdX1J7gnWJfvom2mrrns2wV2YVT3F43pra56MdhHYdO7R3RowRCOqJoQAoZVMiYhAFLAAISJnuRa+RgceqqyYnZSa+uK41zMxtg5XK8AQphXoLpeBG+TJ4/OjFv623FZgGYgivg4lu0mGwAsMCK0TWqy6auTTu9L2QY2LD36cnJfddbdFVLXHLqPsTFxT0olayVc6SlpV2weOnSFiA6wZ4SbaHXsIeHKv0bjIvIN7ExA1vi4y+Ni1s9meugb/Z0j/JPT9+PrVvj+9Xa9WOu6Buu+KT/IgqskYdgAWKjPPqdDQlTCUxsPvHnYe0HXzLUPUAayoAUHgiWQYqqCZciGCwdK9xAppGLhclLbtqWtO2tuOLVjrNdQrAXypAwFWNh7pr2G9MTPtjjTSGLCK4gdUSRglB26huTQhMRi85Nu98zosEZxVIYdltZp3iLcTr63JlRXmExhPmAmbF7z+7xDzz44JCa0Dz/xlPMjLi4uIfKysqCTEOsYt4L2otQOzpRZyO0WSlblJ1/Q7WwUlJTkZFx4PP/vP9+ynXXXBtX16zY43muE7GA2L1rV9/aW6szmjZpghEjRpwCiyMqr5wWGxubXT2xQ3n0u694C4QdOT6p+cU/GZb77LJ91vwtJfGRisjpmqYCfFe2QCsy4RH2HrbBCnkilxbtW3W1YCPPaMX3DYkaUSrIhILED/lLaXPS1r9vK0zsaFtsFfZzQE80SScI0IAlFCKsCAxu2f+LaR0n/8qKAJIwnCNKgvPZTq+FP/k1OQplTiUipB3Y75JK3cPMM0LtAeBfa+KxJx6fkpGROUjVgIXsqz9SE82dtKCH+qFNExPGjQMA/LRgQY1EOjIYvy5ejMYxjd+MjIwc85cZM/Pq0r5rTOPGOOvssxHmdtt1vquoz12pVncA/w/+pX6da1pQWGhPHH7k5+dja0KCX+32GnRlCTHi1Vdfjbntttuya/rYBGDIkCGYPn16H8M42auIEZQj6EpZGdVb8NqNR4Sylda+twTDCVg7r9345V5Rdqkr1fxpQ9FmVG5WxThy8yr/idsDS5hQcEHYDniATVgQyDFysDh16a1eeMv69Rh6b5isB5MUDuQeuGxH3rbZQInzrAl4hapGYF5VHhYXAMAyPWgpOh5oFtX8ThcLEMnyfGpFzuIGqtbyq0/MEwEMGTwYLZo3BxhISknGH5s2BSW+yhnf69etm1RYWIToqKgasaSZGdmZWfcnpSSHpOVCCEw87zyYhgmv14tffl0Iy7KCvGo1/xDUSUGPDK+HrVu2xsRv/iPnplmzv/30iy8mwWnFGkzzeS6/N4Qffpjfr2HDhjfPnD7jWV+gh//xDhWu6lBaUlpwsl/T7t26YNolU8afP/H8hSfKxiwpLcEXX3x5TUJCglFUWHjlps3xgzdt2lyPQ6j9b8dJMLJzcqJ2791jMtm9w2pysaYgIMjEpAsnbTnlLLdqvF1fyVQlbGH3/SydYi0uKTC59UU/G+w+u2yv99sEb2K0lxi+EqlMHlQ0djnSLqHLLidLtlgy7LKvAgoKhFw6KFbvW/13JVE6sEu/p3al7+iy/UD8W9neLHhcdqS7YMOpMx/oAl/AjoNXaORtgN5tuj87pd3Fab5CNOw37sVpusaPadjglv9+/NEbIMJ1111307btiXPKysqc1hFUzXFgT5jEAht+/yP8rrvueeett966QQTTEMS/EQoDH3z48Vk//Dh/CCCCq8tP9rbYyGHDd//v4486CUNg3dp1UcuXL8krtCxRKwMnmAUH6jAEoGevXjcPGjAgichx7oSoRsXFJVi+fPnTTz/z9KiqYl8CrX7EDHg8njJojkm98Hq46sor33/+uefevfa6G8bMnn3zhWeNHRsfqmsNAPLy8jBs2LDB/r/ThL6UsRuiMAzpwsS2E5YM7Tb80oH1+ksBCcGlELBs65dd5YvjKr9Q0cq7vHsYOe5zYnhMhQwzB0syVz64LSvhvT3ZB97ZWZgU6XGx42oX5dZ2wJ+CDQiSgCAMiO6/ZninQa/Z3eHqSOAb++2bM6N9h/af9u7Zcy9zaM1Yt27dMuXNN99sVRNa+cUXn9+QlZMbvFYwEBYejrbt2t5tGIadIpmTM9HtdgepoTooruYFXQjcdustqYMGDrq1U8eO9p56kDfbz6TD9u07aPv2HV/+8+WXompi6s/Nzc3Xk39gIjxwQD++7LKZi6decslNw4YMLQ71mCn79iEhIWGIzkeveTWw66UT3JYbk5qd/0vvVv0uGGAOKzFUJEoNC4CESwlnulK2LedElR/6/aE/C2XXjjeVHYhXyAeN73ctuGJD7vqRxa4ySJK2Za+cqZAC334jYlhQaIeWxS2jWl7fmwcqQyknH79uEBYWVu7tffjBh/L79O37sWEYdjO5APbTya8UREJCYsyOHTtmhvre7rjjzvapqakXhKqhQwcN3t6pS5eFvp+9Xq8ZynOvBb0mxdzZVxFEePHFF74fMnjwP9xuVw1MTwzFjB9/mt8iKzPrv6Fac0SEmNiYhnriD+ya+ayFK6+8PG7s2LFzQw02K/N4kJGRwTrbqAbvE9ulUUF2v3AlFFwWYWrb8xeM6jZkav+IHiXNZWPEoAHqUzSiRTRiuBFiuBFilf116PeH/tyYIhGDSDTxxqKhbArBEchTRSijIhAsuJSCSxLIcacFNVrJi3COwoDYfs//pcf0LcpQdp34umOgw+VyVwr+6Nqt25NnjjqjSATg0Tp0/WMphZyc7Lu8Xm9w4995b8UlJXftTUmpH3Q8uqMTHTt2ePHee+4p8v0+Pz8fUgYbf1U7D0edjXK3b7odcSUARETWe/K8CeMv/va777uGerFJEMo8HnzxxRcT3W73zMcfe+yzUIZLWZl2uQc2VLhSMdFevXs8ecaI4VeuWL061IGk5bxmB6Fdqa08+t12WxMLjG969s9GoTWot+rR2qMYSjCYPDCtsIBugjK8sEgi3AoHNXQPX7B3yROZBcWQhoBQCgR73135SsJCBGw9uWU4+kf33li/UeyTggWYvPAKgsnBNaA5FTGMykXRb5k92/v7+vV3hoeHv11a6rH3x4MYbd98933L4pkzJ8+dO/eb6i7K/RcQ83+a3y05ee+tlpQhDHrCmDNHJ7z2yqvv+n+MtLQ0qKDz5XXaWq2sLe2Wj8DLL71UcN+DD17bpVOnH3fu2t0glKMqpUCCkJmd5dq7d8/77/7nvUXXX3tdZiWXUgDjPCsrS7vcgxL18qSX3Z27dC5YuXp1tPaYn0yjz05vIiYYLKCEdO6ZHQ5+bvsLEpk4kUlCOLnlUqiAzgAYgCIsOfhT7O9Jm9/KKNwPZQgnAMsOgCOwE/Rkp9IFUkuCQGgpWns7x3S66ZJWF7FQdoCcJAoqwPZUvZOmaRqHXpehQ4d+sz99/+MrV8W18A1EpurFEPmnhOXl5T3+wgsvzL/33ns9gb6zr+fNuy5u7dqKpXgQE0CT2Bj07NH7uUM7LjJzCLqso9xr5tFjhpQSt956SyNA5JRHnYPw3FNPr8rLPfhUTk7uP3IOHnTyKf17F1dvMiEyALajNr/57ofwiIioH6+79tqh5Lj0KKD3C7hcLl3RL+C1b8Ulm3zxZLVs2bIfGPSXGjl0DSMYKCjIx9PPPHvbqXB9Bw4esGTi+AmVgg2rO1FX5fDwFfkgdqLaYbs4FaQd9Qw71c0Wcz7yEo4sEBsQygQLL5SQgCRYLg/2ZO59Y03ehg5ew4LbMkHCC0nKeQfkvHc+qpgrIrjYC+WrSqcUGlsx6N2297PTO09dZwfx+4rHoM7UbCcieL3ew0zV2bNmZSYnJ3/w25rf7vdKDqgQi3/++N6k5L5eS57HjO8Ejm0Q+V7793vvr79lS8KlUqrg/SSsMGjggN2dO3f+ilDx/pmAvXv3wmt5T6appO5a6JGRUeGHCj0R4fXXXnvx2utvuGze118PEIbhl6MeXElIqRSWLV825N777rv9H8+/8AohsChpApCallaoZTqAoXKouDCjYYPQwxDy8/NrZ4omYNmy5Vi2fMUrONkj6Am46567Hp84YUJ8bQYIli982X9xxkdZFBEkTFuUhQUJBUOaIFL4ZMe8Z5amrp/uIQ9crOA1PQCLgHfMBUtYCIPJEqZSMFQYOjXsvK5JVONnDVWxGKndKfvkRCnFVXkvnn766Qe2xMdft3T5yqa+qPdAr8uBzEzs2bPn/xj4rprvBQRCVlbGxJ1/7uoQyucKC3OjdZvWT9xww3XFlaL2GfB6vSEUJasdC73OWX5EBKUU3G73SP/f+fZnTMNEi2bNJ54xYngKl++7BN+/mEBIStmHVStXP/b2O+8NV+Udn6o/sWVl51jQhER0/WiEWrSlQ4cO3WpjllZwio6Ay4uPnLRfjvV80o1rZjvADgBgwYALigjzMr4btzF17W3ZRjbsbGEGSAZV+pPgdFSDADGjnasd92nZ8+ZLWk0uJd2Z6fC5ywkma9K06WNhYWHlvS+CWdz9umjR+BdffHGkL03xWHM8BJCXn/damddTyRMWKGeMGrXxxRdf/NDfMvdRVFR0WHGrE02ddOUqpZCZmdmq0orOrzfzc889c6BDx44PNGrYUFZYfcHN5Ax7Lyg+fkvD1atXvvOf9z+MCmyPDmjcuJGeHUJcxGVl53zTsEFIoRFo3LjxIBLHd+I+2RqzgAhSKQ40Ham2kUKBIAGSdktVZeC79G+H/J70x7d7PfsiXZJhCQlLGHBJF4xg8s1hQJAHiiQaGI3Rs0nnq6a2mLweThlQTdWifuaZZ341YtiwTK40zwZGdm4uvpr31ZWqGteZATz9zDN3rVi5skko771F82bo07ff6+YhhgA79UU8Hs9Jd9/r5t4sVa70RYf+DOCN1177ZNLFF/1smmYI/jMnFcapQrd40ZLemzdvuj+QyFcGIzY2Vs8MIVIvIrIwol69kI5RVlrWnNXxHcC+QkQny5evmBfRyVVgRwo4e+gEJQjf5XwfviV1y5xNhZvqeUwJSSZMSTClANgM+maQsmvSd4/u9nlkvaj/sUHQtQmqfm7tGZBw9VVXZ/Tu3efNeuHhQT8zRIQyT9k1t9x2a0s+xnz83fffiVWrVl1YEmKL1AEDBiZ179HzkyOdyy75GuyiVueh15yFLiV27959VBkWQqB///5/veC8CfudNUCwjzZ8+a25Bw9i4+8bH7j//vvHBfJ60v2TQ7bQY2Ji4HaH1t1WKgntWT3WdHqCLBOnzzgpE6sLVjbYlbFr2eb8rQOV856lUDDYAMiCZXgQTJiUYAaRgR7h3XYPajfgvivbXinBCkoPz6OKOpjhdrsfHzJ4cHIww8eXy75r1253dHT0i8d6/JYsWTJ4167dZzFz5bLbAcwXUVFR6Na9231/nTnjiJH1xcXFUHxy5aHXuUfRt0L0eDxHn7MYuP7a6/KaNGl2Qbs2rStKQgZ8HypexwRs+GMjErclzvngow9bVHXSqjz7bVq21LNDMBOJHw2j68NlhhgDyoD2rNbibBTIIrdSZLqvA5yAJSQ27PnjvpVZq4YWiCIIuB0xtnPNJdk114MKcVWMpkYL78A2A2eNjxm/VzBgSK3mVQ6VQ/a6H3/sUdW7d+83o6Mi7cDBALYwuUJp8csvCye9/K9/RfkvrCuCJ23jKSkp5bHU9HRn0uUAn1oGs8J548cn9OnTdy4dpW+DJa3DDkmBfyot6KFaa0op7N+fXq0Z66WX/vH7iBHDH4mMqHfY81G9m1YRUKfsjUcsW7GyY3x8/L8PD+6o7Iz3RVW6TJeuaBKC2nAN7XGSIOcZYH11T+j5neAqYoAUmJyoFJL4bPcXDyRmb7uviMsg4LJT38pFRsJuzhFccGQ9UR9DYgc8d2n7ixcSKzAIUjBAOma1OnPhgP793j5j1KgCZxgFLH3MwO49eyOXL1s2mw/5A1/64d333DN865Yt43zPiX/71eqMWiY777x7966vTJsy2QKO3K65qLgYh+7pV3+doi30GiUzs3otNoUQmDBhwhtjx47dwiq0uk++vXppWfhq7rzJ99/34PWqUmtIcdjfA0D9EIO5NDW3GPT/t65OzDiBLndD2YVf7MpyvmIwCgQLXyTNu+m31N+eTlP7QFLYlnsNtTcjJvRrNGDNX/tMe8RtuQASUMIDFhJgbaVXZ+zMnDkzp3nzZg+63e6gW1YrpXDgQMYNt99xh3moPi5dtgwej/fh9AMHzODvMzBk8JA9f7vnnrePNc5LS0sPs/CC8DtoQQ9tfU/VstjYr7LQtEsuzRkzZsyFwwYPAZgrrfoCmgqdXHeGHbX5xx9/vPj+fz5syMcQCR0UFzqWZYVsWYeFhWXr9KSa6EkYwnKC2G6+AoKhXBDKBEjg812fD49LWf/abpUCJkAZEgSrxvZIBAk0iWykDJiQLCANBqEMLssEOEwPsGrOfW3atXtv8MCBu4NJfWQCFCtsSdjapbCw4IZDn8svvvjcSEhMOD+Ucd6oYUP07t3rLkMc24vj8XgOm7cpsHGkBT10+8IWc+soBf+rynOcfdOspM6dO1/eolnzYFs4HNY0YPWa3xqsXr3q+++//7Hh0Vas4eFhOpA2ROugoKDAiUoNnqysrBU6PenEopwccsF2YrzXkPg65Yfu8VnbvtpdmuRSMByxtx3zAjWTJywhsSJp2YhPEz6fvyxncSRJEwzTadGq9I2pJn+7++7iAQMGfBQeFhZcHIMzh3ot65G4Nb9FEJHTPpeRmZn12toNG0AiOFkjEM4bPy5x6NChC6ozzourcLmfaOqkryjYAgf9+/efO2jgwG9rRF2JoFhh8ZIlZyxfvvz6o0WyR0VFb9FTQWjWQU5OztEDIatBSkrKbn01T3SUO0EwYLBdEe6X7F96JR5IWLmleHtLggWTlb2vDROAUaNSm2McxML9iyfuzU7+ennuUpJEkMIb9AK/bvp3CD169HhxwoQJVqX9bar+08cErFi5stk3X38z1ie8r//7jTZpaakzWcngRJbsTKa+ffu/NmHc+BJfHfljzSsnm8vdrKtTkgyi+87sWbPKunTp8mDuwdxBq377rbXP4g/GBc92vSlkZmdjS3z8Px56+JFvn3ry8Z3M5GtC5Sw8GErJAoJOdw3FK2NJK6aoOLS26NHR0bX2Hg3DQIP60ZWDIk/Sex7mDqt1lzsBTvtRAjE7jU7sRi6AhCLG4gNLu2xO2jRvXeG6GDu9XIKUBJELkpRtxZMJ1JCsE7uQR/mYn75o3Hg15ishyy4fHTOumMrfL0Mo22JUBJgKdaaFaiCL66uuuLLoj41/zA4Lc79TUlYKIlH97TB7fxJZ2dm0ds3a26VU8w3DQNzquMsStm9vFPyGKDBj2rSdN8+e9aYAgZVd5Y6P6S043OXO1X7CdXOWGkGBIYPIHxQCOPecs7fGxa2+M2Xfvs+T9+0TdoWNYCaHirrPq+Pi0KhRo59/+mnBgIkTJ+T7bret6oBiKTngti513IY8ZPCER4RPyM0/eZvWjR45ClddecXj7PdcMuGkc+YSM1iqN+wUvooIYKrBucmXhsTOhGrAl09MjjgLLM5e2uiPtC3frc3f0NVjSic2hsBklqeYMnGNP1mkDOQb+ViSvmKKW4ydtxhLLj6n8bllRArMEgQD7LRgVaTH7JFEvV//fl/9uevPR5YtX9HGVyL2WKJekdduPxeJ27aNf/jhR864ePLkNV6v9bjX64Wv4FGgst66eXN079HtAcM07GnXt5qmY1joQdvd2kKvuQnDLl8Z1INIBDz00ENzs7NzPnjvg/evDfqh9msgQkRYuXJlx/bt2//r/PPPu7a8iYGeC2pMIcpKy0QN1F2uNYO5QYMGmDZt2mOnih/GX8xrfMFNtpjb5Vl9QXD21RdkoJS8WJO05p31eRu6WwbDgICECsE2q+684bHLvyoDOUYBfkxbNmECj563mhdMGd5knMeEgG/hbSiC11Aw9fZ6lfPv1Vddlbt79+4PV62Oe8jjDS54sai4BDt27nj4s88+nbtw0eIwW2CdQlwBHm/MmDN39O3b96fjN/x0c5YanYy8Xm/QrxUCaBwbe/P5552XGKwR4P86yYy8/Hz8OP/Hax55ANV4XQAAIABJREFU9JFzQFrMa/JeL1y0iHJzczrWlPFYO+/z1Jr5azvanyv91wSxCy5l4Mv988LeSZzz9ca8TZeUCQtSKEjI42IHS4LdNpUBkyUOGnlYnLbi/J15qe/ElawKZ2WA4XYWIRJhUo+/I95fxejVq9fD55599sFgdY0IyM7KOis5Ofl1j9eOOC/3bAZA/cgodOvW7clx544r4iqasNTmE64F/WSYyBh45OEHyoYNHXrrwH79JUK1Vpw9t91JSfjttzXv3f/AAw31la65+xUfHy+ys7KHhTRQhEBYWFgtGtB6Bee/2DVYgUlBkQCzCYBQZhYjM+/Ax2vS1k4uoVIwm3YsCovjc/3YBZcCDNg14w1FyDIP4pfkJVf8mbz9o1WFK8kucsIgtmAJHfVytHE5Y/p0tGjR8t6IeuFBzZ8KwLqNG10Lfv3VXd3Xc6UKc3a1wQsvvGDz7bfd/smhzWOCEXWdtnaCCC6FyS9MiYG77r5zUecune5o0iS2xqqHrVm3tp205H/1/F6D87CUL/8wf35IVn7DBvXRsWPHXJ2Hfhwme1Q4JIkJAhI/5M4P/zDh0y+Xp66YVsylTj9zpzkGieMyXAh2oBtDwOO0YjUUkC/yaGHK8ml/pu/6T1zh4khJDBYu3YHtSPfWN4UycNZZZ39z5ujR+1kePw8V+zU1b9+uLQ8YMPAFwzAgROhyeKLveJ0VdKVkSLfL92C2aNnirXPOOntdqKsuX+1jS0rM+3re+Y89/viDeviHzj//+c9eP//089VKBu//JCK0btUaOTk5c3WuwXGYcGGLJkPAVAwWpcjJzPx0aVrcpQVUCo8pnTKuduczRfKY0eR0xNHp9FA4xl/Z/9cDSwAWGc6780KwgGIXcs2D+HXf0qsTD+x5w2NIsDJhVjG96qen8j2ZPPnijGHDhn8QHhZ4cR6uga3Jfv36re3Tp89c38K9Jj7TiZT+OivowXUwO9y199QTT3o7dGx/ed+ePZKEU1Q4KFcN+yLfBdIPZOKXhYtufua553swBKRXeukk3VTnk3CdyhB4593/dPr7vfc9sW7dhi1r1m2IDmWxRQy4XS4U5hdYtfPRFEhwpahs4lNtPAX50AMwlIDBACDLxZVggdjAt9nz3e8lfPj5Lym/TM4RuXalOObyscikQExHvF7KkV67PKyd7qaInYIw5LRdlXZe+zGmQ4bhhLxJEBMYBhTZZzBYINs4iIXJy6/6fOt/P1tZsDRCwgABsEgCxFAkYAk6/fLWGSgqKvJUd76oNJWxwt/uuev+s88em0bEIFLVnkOJAx8n5fMsKTRtEoNRo0b844xRI8pqdz4MTfoDoU5Gudc0D97/wI7c7Jz7ioqL//vn3r0ipGhbx9e4ZevWli2aNfvX3XffPaVf376xdt/Ok28i8L0tZkZZWVnYnXfdGVGbEdCHUlZWhuHDh9crLSm9YcvWLeEEhJeWlM3cs2dvTEJiYlR+YQGU836oGqkxR7onTZs2lZ06deLauYYV5Yh9MRq+Ce1knvoPbSQU+OxnC6jlLGaY7BalDAIpEwsP/GQcKDjw+bL01ZOLUOK0SA3sFAYUGAyvIBAEDGVAsL0cUGAYMMAwUGYwDPZCCguGdB1hwq26txbBCzjd3vKMAixIWzF9gmEgjPHXIfXHSBcxoBQEGXBJAY8BJ4L/9DG1e/fuPRLA6wHdfuZyN3eHDu2fj4qIeKWwuBjHI0WXAJxxxuils2bN+qqmj1u9O6vT1k5qXnjhhc9unHXTX3bv3TsplFtFTuEEpRRWrlo9viC/oGjt2jX2hHkyGunOh/39j8149ZVXv3e5XDh+OfMMy5JYt249DmRk4ODBg/YKQ7Gz0iD418kPxaVWUlq6+Oabb86olU/BQGFhMe644+4pdNjDcLJN/BXmU0RkOM4999xV55x1dkaQR7JLMTvd0oRyQTiLmfnZC0RKTvJ/V+xfPjlPlEIaTjAaREBXhJykYoZhW/LO+TxEkELCZAWXFQ4LEgQFQ9aDHW4VSEYxQQmAFGBwGXKMLPpl79IZUo4qRUTEDUNdA7xEbhATFCm4ld3G9XSiqKgoNtCFvP9C9swxYz5NSk5+5Icff4whw6z1+IOWzZpbffv1/UcwKW7HW/q1oJ8ADMNA586dL53x/+2deZhdVZX237X2ObfGzPM8z3MIJFEQIoPBMIjM2qK2CHxKa6OtID1IO2Jrt0/bH3bbauOntgoqoCJhCNCNAQMkJGROyEQGMlXGqlTVvefstb4/9r63boUEUpVbJFXZPx9JKKrqnnvuOefda+213nXdtQcfePA3laX4qI401OOFl15sywxNyWhsbMTipctOA7HJpw1Kuwru26dvQ9tlHQj/89xz6Lz0lYdQvG47XR/66oxTBg4cgP79+19y4Zz3PtXqlQy5aeaRNWAYCAse3P5gpqZu/wP/u3vhB+q5wVWUg5AawEjLFotNPe0M1hREFikRoGUYTgMaKmODTXZ7hVIOpAZGI1hqmUWwwgACGCRu60AYB+LDePKNhTdFcaYq7ZP98OzK83OgFIwEAkZH62qwJ1mjMu/S9+9dsnjxd55+5plvNmaTNl3GEoAhQ4Y88Z7zzpuPU5YpCRH6aYxbZd71xTuTO++66339+vZ+7I1dezqdmeeC2vSCPaFPg0rtXEaIM2W/bctjFhEcPHS4XX2+vXO9IdL6fIz6ylKXAicklGBltCbeunfrz5btX/6Beq5HCkYEBSgLIxm01BBXCCBlGHHOcUKEjC3DQB58eGbfsy7vXF1dXrlj6UPLa1dUJSQA14E0grawASkSBcOl092evcVhPkBPvP7M1dlsroaH0O0zK2daJYbAD5fpKE8/VezateukzYa6de/+z+8577xbn3hywdC23LIbMmgwLrjggn88+6wZWurtwVO9BA+CXsqPUYFvfv3rC7O57C9/8atf3lLf0HjSZZj5H28fGToqqhc+xYVxJTxvUyZPwrnnnru0PR5721zr/OaDbRUMAYEhsGzx5M4n+6zas+53L+9fNrM2qkckAFMCSwzWGKwppBXFrPkkfcoERow+tg9mDjvruzeMuOY5VkIFl8/JrU9/v1rW9lU1LRJzd63noExItQwKAWvqBdvgAB+m595YeFtFHJVlhpTfMi06K41gO5THOxGd9OAjAPirT92eHDly5EeLXnz5a4fawqaZ3OJ86pQpf7x07rwl5L5Q8uXuqbxtg7FMyU4jA0RgYzB29Jjbrnj/ZetI1Qtb6z/i1lRynsqHPUFO+TjJ/Dk7mfOW1ykmxZQpk1+88YbrVrxTn8fp/5n7PWZSt3F8Qg86C+EUiXH3CyvA6qrFU2Is2Pv05Fe3L33phf2LZh6O6gEIbKGCPe/edfynr5H8/rj1BXbu/5E1UDXIRTkQDLraHhjfZ9yXbhxxzT2RxDBqMLfXhS+/a+Ssq2aUTa1niaDELssD62rk1TiDGzreVW9cNT0J2NuP5o/ViOKgOYT52565aclrL30LnIVQBFaBkECJQWp80Z71P9/+4pkjDfUn765GwPTp0++7dO4l9W0REJACVeVldty4sf80fdoU0TZ4jVNd5R4EvQ247bbbdOLEiR+YOnlyI6AIXiTtMOrwd+bA/gOQyWS+EfqHT+YhZ2CJYVRRluajZUVinAD+cesjYxe9vvjxFw4vG5xFFhmRJvMR5Isb9S0jZ2HxxZgMVgMjEYwYWCMgShELoZt0wpTOo77yyWmfuJeVvEe8ADCY13/uoqmjpl45qXziAVJFbI2bdw641jTht0iTF+atIT/Rw/1T4BLsgoNUb557Y/Hn7l/3sx+82LCwTCjjvSwSWFJkLMNShJTb58Ni967dJSlku+TiSw726d3nb6qqKtvkSpxzwQVPfelLdz13emS6gqC3Gz772c+unTlr1sfLy8tRgqEggVPEjLNnrJv9rtkLEFy/TiLwsiA1YFsGgiI1ORCAWBl/qPnDpGU7VixcdGRZvzTOwqi0ypndRbsElgikXPTaCguLztIFk6rH3HXZlPd9uTqt9IV9AiGFJUJsY7y/19wFs4fMvOCciml1rphfIIj9gzJt1UPYskJYwRDUZGrw9Pbnbl65ac03kyhXKOAkcsdBapodezv6gCElmkWgIpg0edJP3z179qZSa97I4SPQqVOnz7e6fbWkcXeI0NvXNU6ECRMmPHTZvHnzjTGgoAftKaQEKfCumTMxZvSYj1x79TX1hda3MDindQ8aFQgBKUVgMRCy+NXG337guXUL5y9pWNpDNQeyjNS0tqLRReYEgiWLJMohNQmghO7SCxOqx37ljpl//a0x8XgIJWB1xwEQhC2UgUxqcFn/9y2fNmjaheMzo3cbZQDeIppsiwVdAQhikDAizSGyFge5np/Z/fwd/7H0P+9Z2rAkhmTgSgJTV1HQTlPuBGTWrFlz0nrCzLj+uuuPzJr1rp+WZTIlPcwePXs8ePvtt69l5jbzyQjWrx2Yj3/0Y7krLrv8zvdeMKeBEMZvtBvxAdCrZ08ZM2bsHX97990vhzNysk+5CMIWadQAo4rHdz2JX2586M5nti389dp0wwCBRawK0hjQuGDr2uJFtLqSJOHU7WdbQue0q07uOuneC8fMuScjZRBW5NhnDUBusAsJLOUg7HID8wa+76ULhs951+Ro4hsGLv3vBsG08G0TgYWQsQaWAGE3ZOag2YeX9rz098u2rPr6kuyfDasFILBIoe10oEtVVXWsevLphfzo6KlTp9177TXX1JdKIbt07oy575/3H1OmTJG27HEP1q8dnKuuumrFtKlTrxw1amQ4Gaez5hTdiSNHjMAHrrzy3ls++cnvhTNTiqdcCsDA2DJkTSN2Hdx596Jtf753m26NhAAhgxwzQBastuAg19r4iABEiUEP6YVz+83+7wsnXvCVs7vMVCMEo4RICUrwaW7AWC4YzigIRg0u7nfhprOHTLtsStmUPUYZwnhba9g3LzAERAksAynKoQAMsmBl7IsO8DNv/OkLizcs/daixiWxoAwG1D4jdABRFBnVk8tdFSdmLrrovdlevXp9pnN1VUmOb+7cuU/c8deffbatDWs6WNsaQbWoFYBcmQiV+kwc9Rp4KxM1RdHopqKVzDty5t2L9unbf8H06dN/sH7Da7fmD5pa+v6pZefO3VrFJ4mKfg+1fHWpLTwuLc37aM1rtOSxQkQQFTAIJjI4e/r0g5dddvkXLrhgzo8mTZqIvMcAM7/lvpsWH2oH3l5pdl1p87yTEMAaAbBg5OAaxgwUKUgyWHh4UdWrW5b920v7X/xYDe2HgYGIgsn1RlhOQQpfVa7HuRR8gZyvgPevjEgMBG7PmiRCZ6nG2X1m/Pe0sdM/MT2anCOoa38T709H7P90Ikqa94ZnQACiBFcMvnxpp8oul9q18vuVubUDXImecYV05PfU821udLyrQmFJXIpfvWqpAcA4FB3Ec3te+lyisqd8hH53avnsBCQFZRMCKqsqo3yJXcss8nBUEeGJ3RQt3e1QcoujJEktnezGIjX/6/nnn//Ia6+t/9tH588fdtxvP86zr9iLYuigQbnqyqrPM1FJXeFU3DnWYxz7m46L8iZH+eMitIWhZlTquz1/ESk109sT/jD1hB7ILXgNav4nnfBrlE7Ub7nlZu3StdM/1tYemv3H+fMnH7NLm0pzIzT/onu3rvWHjkpNvnk/WEuTS2r9+3kHXiNvJFG4ZtQN6ZgwfhzeO2fOE4MHD/7rW2+9da276bQg+ie6slfgDNhbKbrjii4gVoFSCmgE0jIAie/9zuCRmke7bd6++bGX9700qzY6BJbYJQi9MLI29bcLvdXCKT/Ehvw8a4KQr1hXgJXRSTphVu+zvj9z9IwvnhWdlStEvSRFMbw2/cZmi1yBct6MhjCn5/mvHBlZ/6HKrRULFh1ZHAsniCy7+nU/TIT0rZWQkAKUFpo5CTlE7tLDAbOfXtzz8jfL43h4/ZD0U7MrZkshFCACGSoouRKV5HM61gWqXs21FTchQX3hL5/kk7I5F1/03n0rVrx639PPPvudhsbG1t1rCvTs2et3//Ld76yCSglvTsK5557b6dH589+0SDjecb1Zc6jom0uz0IhKfZ8zFJI3F9HWfJwn0IFIzo6iNedA3/HUiBOPG66/fufhw4fv2Llz5x+XLF1e3vYP/aZ3Sv6cvX0U+055sJ8auHATKbp374Zp06ahR4+emwcPGfL8pCmT/v0DV1zxAqEVq3ifIXJ102cCRd4Kza4rglAKo4QEMWxEMNbi91sfO2/Z9iXfXp5bObM+rkfGZrxwp62IGQSk7CvCyZuFWKRswZbQLe2Cc3pO+c/3jJr1uamZs7Ktu8/FLyoiGBthbr+5z8XITK7f1PjMuiPr++WinPtvwlBK3Z6/mha8BwXDIoXzrt8bH+Cnd754K6upj4eau6ZVzcixEEiAwwcP5tTPJWj5Q8t/Toq39QtobWaJoCBVlJWXRyebcn9T5kwEn7vjjn9etOjFT81/4onh0gJBLkTnQwbh4osv/jYTgUr8bNu/f19CkjeNOLH8RNP3FD9rS6dGJRV0pvxsYS085Fx3Jr/9g6HZ1+St14Pqx+z5QRGElk1hIqDNzf+Pxc033/zM5s2bv/fK0uVffCdevWBSQdpsW+I4Yt5htIggiIxBRUUFpk6diqlTp4K96Y9N7R969+m9Joqi17t37/6nPn36bJkzZ05tswi+ha+3ccMGZjak4lLz0sGnXuevKyL34OX8kA0YsBhYSgFYsAIPb3vk4le2L3t4bf26qiRqRKRlEDJQyvrnRNSK13fpcWsEQqkXowhdtRvGVo2888KxF3xnQsV4gbhWsJYW2VFhj11AZFBuM5jb58K1qc3Oq9hQ9vsV2VUDFRaZNIaQQcrudVomte41MmIBWKSUxaLNSz97eMeRNdPnzvghidvft0mq+eNp9f3vrscsHUc4nS1W68SukP1Q1VIKUz6TpqoYNWrENxY+X/mj2traZnpxPF1pWpwoJk2c8OBdd33xZSp5oKKYM+eCeS/8+QWfKSoOSOk4n4c0z49Q07S501LQk1zy1fvvv3/H0dsDetwJSc0FPf8BVpSVvXLcSIiAH//4xz8wxjx5tKC35HKqrqh85Z2LZggqCsOMr33lq3eeNf3sDcptPx6wsBokQadOnVYfa+qYX1xi8JAh7/nJT34yvSO0WxMEcRQhk8mgS5cuT8yaNWtr4cZRdTNBqHTpmgkTJshnPvOZT912220DVbVD2Xq+zXXVMHbs2AX56ym/lDFi8NTep8yOfdu+sWTf8r/ZnL7OGitIM35tmYCVW5HezW8Xkesf5xwARWRjdLE9MbF61D2fn/WZf4qRQUpuxrxphQcEiwFYICywnECVwEq4vP+8pV3Ku8w4J5pxhYWgLM1AWJCyBWBadBYtYhhNEGkKBcMSQ4jRqb5ykVqFmwJGuPvuu/9wqPbIDnDLe6cLdUKkSNP058XZuYJxjyjuvvtuqauruyWbS1r8mTTVIumOKZMnJW1xzc299NKHZ5x9Nqy13PzJemxdKb5GU5v7fWRMmwRwI0eMvPP+++9/goiK9I4KGeo3ZwjlTYsWESnpOaOGhobS/TLKG51qmxQHuchagII1IwqCrqdlVNS0amu+D/sOOf4WZXSON4Qgn9oiolNvRFzqt3+Mve/8OSi859LFrW+RdeqAql50eTddW65l7Lldz5ev3LPuJ4v2Lbl+P9UgEkDVQIgASpyDmsQ+Sm2p4ApYYwgJrEkQpzG6ZbvphO7j7779XZ++t0uuApYscoZgREGt2Eaiom1nJev855RhWWBAMNZAfIRN3h1OW3EOI2GkREhZEKm4ee8gt9rU41/DJ3v/Fws6FdfbtbJorNQDTlp6/Z34s/ioOQQlyCKoz1KdkN7RMaShxJQ0Qi9ceEXCQK3d56bjxLpEEF/I1HS1tuCWpfyPKN4ZT9Zjz+J+J4xmil+iuEq72Qq96GakDqJF+fTksR6EqtpM2Et9tjt6lXtBwqhQ/uqekeqyZI/umT96zbb1TyyqfWVolnLO0tRvw0WisOxawEDW38PHLtDKX6N0VK5PGOBUQKSILKO37Y6RvcZ+ed7s93+rMmeQcALWGLGVo4rdWnL9uKp3J7DsfwXDiHtcWgIUOV9vH0FbnLBWKFLkyEDBMOJMa0SjwnNNoH57sSje09bf/82udz3uA7wVVe54Z8Ucb68txfc/NRulXNrjJPKBGvSE9E7fJtg47QT9WGJV6mpyBb1Jh7VV1dHvnJgf84Z+p++Bo9PsLVhIBU78KXMmnEMCg1QKXuhuy4vwwGsP37T8jWX/usau75o1ORiJILAAkUvG5wfeCL+tPpHmU/iuih1we8KsDLBblfeSHjqx19i/71He4+sTcuOQr70Rsif9Dl1Pus+uFToebNF3mOLvbsUzMvJLo3yhl/FV5locBjQ/T1T6p5Ke7LP0NFi4v5XgvjmDVvIH61uezxN5Hp/2gh4IBDoq4r3ZXWTCEmPB/mc7r9u97u9W7t/4hW3pFtioEbEYkBAsc4tDS6NApIKUCEIMlqYIOBZBQ2QxOh4hZ/Wd8rc3jLr+3shGThiDvXLgDCcIeiAQaAEMyxaWUsRpGf7rtZ8PqMnW/OLFvYveU2+y0Mi6fVh1hWspw49KbcGSgYAsE4QMSBWxul71lIAUBiN16P6z+5x9w03DrnlKUjfN7YyoXQgEgqAHAoGTpXjPz0gGlnP46dafXrR+37rfrMmt65KNEhghqOTHjjrDkYx1U9BaZAQJt3dNSmA/R93ZhJdjiBm0e2SnYe+9adgNqwV5e9W0MGY1EAiCHggEAm8lsoWCSsWfDzzfafmOV7+5pGbJp3fLHpBGMOzS485mLW8Ao4iEkHIL7XgBROL2zC0Dlg2qctUYUz1u95BuQ+Z+bMLVq5ECAgOYFLFVKLhDtwsGAkHQA4HAyUo58hZRwha/3vrQkI07Nj2+um7N2AOmFmIMWAFjDYitq/xWQFiRMJCwAau+beFYU+tp3qPZFcRZBsrTSkzpNunJyf0nfuLy/pduh3XjVxkJRI0Tdt8+FggEQQ8ETnOcxWTxgAOCq51mPxTEt/eo8dFgeLifGOz90QFWAvveWoGCSMECQBmL5MWypRuXf2LLvi33rW5cBzECgsJ453FhJ/qF+QpKMAq8leujkkKVESmDYJEYZ6Ea2xhKKSxbdLKdMKl60m8+N/P2a6vTCtf/XRB+11rmetnD5x0IBEEPtAMxhy+ycv9CSv5BDv9QDw/z1iMw6tqlLCtSuAYfN9eUYWGwYO/j3VbuXvXgKzUr3lsnRyCxHKPtpjWtW4rYO/dlIwbBoMyyszwloDv3sOf3f/d9Faj6QqzlyBmFkWL3o9J7YQcCQdADgTYUc1KFkOtHZi3uC6aCFaj6Ht6wjdqak+yyHyzOpcyoG4NsTYqfrb3/o+sObPzntUc29chyDhKlMGJK9MIRBAJrsmCNENsISorEJBik/WRY9fAPzx4764FJ6UQkUJBK+KwCgSDogXZLs+mPeZtfgWWBQBD7qVtuLnOI1FqKZQXEp73z6WsGfvP6w73eOLj9/750YPF1++gwIvIpdDUlfHVBygQgBosgNRYmjTChauTeSb3G/eWHhn/00TiJoZxFbBWWTIjGA4Eg6IF2q+d5G0eQ72dWpGxxKK3DyrUrzzp34uwlkUTITxDID2bQEKqf2ANAgJQJCTMiSbCaV2Hxulc/unL36nu25LYMbaSs7/J2E85iSZBSiVy3KAVrBJYyKKeILGFC5bjlI3sMu+wjw2/aBk0hzK7PnHNg4fC5BgJB0APtGSl4zSugFsKCP6966Yb6urreQukS9SML9eiwPnAC5xaIRCGU4Pfbn5i09dCW+5bsXnzefj4ASwJDBhYAkSs6TKiEcwjUgCAQU4/OtivO6jntd2P6jrr5ip6X11hVMBikCYQAQQSlM2fifCAQBD3QMaN0EIyKGwPJBJIMdtitf2EpeUphIJwCWlZojxIqHt5zZmUzFAJWA1L2A4jEiSYiEOVd3GK3dcEWUEIuyuFXa3/5jSU7V35yY7q9p3LW1SoQwZI0n2ioLY/OyR2Vs3GFIBLXnZCywqhisPbDqO5jv3LhlIu+OjWdnCql3pbGbaUQ8k5zQcwDgSDogXZP05hH96BvREOvrGY/RMr/2jTIwldAd7AxsC05R4UqA3IObQCBlWE0hfWiSpT4s0j4zfaH523YufWrKw+vmXaY9gHGQoghILAUTeXz0tyqZZJGAKeI1ILFwFKElBPEShiGYTXT+065+qPjPvIcKSGNcn74S9NnGvItgUAQ9EBHic+1uT4rCXImwQE+2Pf5mue7zel+/gHLCaDeclTzE6zOREFnCDlBJxBI2Rey2cLIUhbCy3VLOi/a8PIPV9SuuqImqSlPOEEauQEnsXXCbUt0CpXEtxkqrLEwKaHcVmFc59HPTu05/vNXjbh6qZs3nsIIwYiL3gOBQBD0QEcTKqAQbZICAosDyUHsydUMXrVjVZ/zu19woCkdW/q5x+3rTLnUO0MRCQBV39LHUAhe3v/nLit2rLl14+Gtf7WhYfPABlMHNm7+d2SNj9u9b3uJTqOwgMQZ11gQesa9MKVq3C8qMuUfv3rYdTlLjRAYRGkGSkDKNlz0gUAQ9ECHFPNm/65YV7eWDmUP8770IBq0/jrLyVe48J1nrqDnZ3gLAKNuZjnYIuEUz+x+LjpUu/eiZXtX37O2YcvMBhyBRBaRuvS6MDtXOCiyxvXzmxJtWRtrwH7/fWhmWOPYnqNvmTV6xi9m8CRrKYGRGKQRlAhCFkoKDuXsgUAQ9EAHFCov5fnCrJzNdYo06m6hqE2OfMJy9iskGRBJ8Q+coWdK/G3tnPUaTQ6/XvPomF11r/9g/eGN5+/U3W5/HQQDgnW75WC1AKkvQFSUsqWfkCIjnTCucszaMX1HXv3h4deuVmVTvDZeAAASb0lEQVRYFrCQ21dn9Sl3RSxUsnR/IBAEPRA4raJ0ghT1mFtJyglULZxgY92WwX/Y+NSHrxx22X9HRVFdR9B0Ui1sN4gfkEJwjm5K8EY6qS8WBAAGCftuAMEvVv28b52tv3t9zZbbN8lrJKyAd9tjAFYBYgB5H3y4VH1+bNmxppc1VZ6L6ypQd3zWdxZE1hn8pAyQGkSaohd6YUz3Mf/vvAnn3T6rbGYdiQBEQN4/gF0lvvE1AHqmVjUGAkHQAx1f0J0QuT8IRJwqaQ4QHJbDWLdv/SdfGbL0wXMwI1GyheEt7R3LeQGHzz64GgLrx5ESLEgjsFNPiBEoAQt3/8+wFW+s/uL6uk03bM3t6CpsoYUiM/V93fl/pWYLIHUn/LiwAiwK4bzRLvtNDnF75Gyh5EyAMgIMN6PrxvYe+fEbJ3/oN5W5an/84gfBUGGbAIXKdkCC418gEAQ90DHRo2LuDGcOJ5rUsJiBoBxea1hz/tAdg943q//MR1NO8y7v7f99E7u+cbIwEIgXeXcuGIoIBIWNshBiPLj9d4Pf2Lvjtpr6PV9aX78eCWehBkgJMFIadzfyfe2kBkLGi3dezAVQhhFBZ+6EURUjn71o/EVXndNj5qFMLoaxDGusW6Go840PBAJB0ANnkJgrvEscnL6N7zw+rTAVOWEGgVCT1uC1vZv/LTuo/tFIypr20ts5mVShJBAGrJLf6zYgETAUKQHWAE/U/G+fzbs2fGfdvnXztttt3VJViFEIAyyMTGpc9F6CNLYlN+fciEvbC3IAu9S7EQAaY0g8HCO7Drx5xqBpD8zu8u46k/gxpyxQSpEffRoIBIKgB84ojoq1CWA16FnVA7ZxIwwikBJW718x9N+W/vu35o6/8M6xmfEdwymOxI00JUKkplB1LgwkbPHbjY8M3bLv9VsP5g7ctbZhLXJR4lrVfJsa1N/enHgRLcU5ceNqI2EAFhKlSAiIbAadpByjq8etHtxn+LUfG/6h1ZHkZ9inSLlpZjkL+4E64eoOBIKgB86wKJ28X5hAlWDYoExjRKqw7Kq060w91tVu+EzP7d2fHj1y7JOlG/F56rDkLFNJDYwCYnJ45tBLfGj/vsGrdq396tZk6xV77e7OqTRCjIFQBGGnkkZTGM1BQLAUoVRFZqRaiK1TBgSMiqQcfdE3ndxv/PcGDBjyd5d3vbTBpK7izu21o7CYYGEA7Ka6FX09EAgEQQ+cETF6Yfo5mBgWKWKJt0eWznEWpxaWCNvtzvLl29Y8Up787oNXjbvi8YwYQJ3rGKsTI6U3z1Fvs2WINr2DQlW6t2elQgLcfxMRVOH7r10luUtrE4QES+qXZp5bu/CieiR3b6jd+O59sheWLKwRwPeQGyuwbMGq3r419lFwa9vQmloF1c9MJ7hhOZbdjPqqXCUmVI56dVD3wZ/62LibX4jEgq2FZYaygi3AyC8o1J13ylfQa4coXgwEgqAHAico5vCV3UYJpAoxFv26Dvlfqok+yJrAgmHIghRYLxsqyg6W/R5bdN51Az/4FBFBWSAWMGD3y/yAEaW8wDuntJIed5HRWr7q3oizZVXSQiW3s2c1ECsgdlX8qq5PPGcED214aMDGgxuuSdLky+vqNnWt5Qay5AvL4FrYmhziFNzsfbS+n7wg4EoQJggpjF9wJLAgMIZIv8YZA6d8tzpTfs91w/8ixwqADIR8gt878Cpss9/rzk+ptgACgUAQ9EC7QOHUgQpCyTBq0KOq+74YMbJc78ZwivrIz2JZ49L48Nbah3bX11z9qfG3PGksFVLP+aldrl1K2iBKVL8/7IxdNJ9qBvkWMXHV4GCwRL5vPIUahkBgKUGOc3h41R9uWnlg5ftzSe76bckbqI2OQGMLIwDBFH5vW+FayqzrL9cYRmK/GElRZSsxvtPY/xnda9QdNw6/cRlp5FzpKExDCwROaQDU0NAQzkLg9I7R82NBhWAQISVBranFt1/+F11cuwSssY+8XfQbqYHCon+mL87pc9b3NJP5m5sH35SAUt9/7R3L1Q8vAZWw91kLaWn1vdX5Tm9LCpMP2SG+eE0hLFhSv7zy6RVPD5as3tjIjf+wrvE1HOEjrmYAkc/MO9Es3pNuu4UUwyiBYZGwBSkjk5ahb9xv3+xB53z/2lFX/EO5rfQNdPSm1sJAIBAi9EDgWOriHdMAq24PvAJl6IrOyyKNpqoXlHy7WkqurWtHshtPblv4mTHlw89/gH991weHXPG4sTEYvsoa3qRFpaRNVOpFm+GLyEigKhBSsGQAJVijyEZZPLLusXlrd6+c05A2XrbH7h1zUA+4YjiKQJqBUXEGMvmZ4ojgLF3aOBomixQKpghGGN2kK8Z3G/PzIT0H33X9kOt2GGt90Z4BVGGQuL+HNHogEAQ9EHh7mXSFWG5wCKFLptMDsY2m5jgt2JESIli2sDAgJdRSLV5tWDVl9/q981dtXnPfoK4D7h8/ctyScytmu/YpEaj3bylZRgEoDBdRFmQp9e12ER7c/LtRr+5c0T+l5JoyE92+uW4zDvFB5EigJDAE76KWAC6e997rCpDC+gxAW5upsTAYiozEGFE2evW0YdM/f/WAeY+XKZByilRjREIgsm7P3EYgDjF6IHAqCSn3wOl+ifp/Wte4Rq4PmqD49a6Hz1nw2jMv7kh3ORMVtSDNQMjtUQMKpiygAtFyMDLozZ1tn7jv8jHdRz107aSrv5aR2Fmrainb3FwFuzWCRs7i0eWP/5/XD22buNvuvLjBNvTbb/dV11ODr9xnGHELFSGA1S1clBWqKNir5mv9WaXIu72l5zFv04OiSNp9nQpf97kKZYwtH41hlQM/nSmL7//EuI83CBgGMSAKMSkUKWLrji9rCHHYQg8EgqAHAq0R+mX1i8v/uGL+6j83LB+mlCK2jJSbKrSbR/fOXxzk5ouVaTn6m76olIo/DO46YNX+I/uenT5u2tYLe1+81miESExh0MlxZZtcH3xKKV6o/xPt2Lnj4pXb1qFnRe8Ju/bvmhZXx1Nqsvsm1+oRNGij6732++ZNY17pBHagqeh9tA4hg0gsCOLOka+uV3ILITdtDTCSQV/un+0V9frhpG4T7r1q0mU7MjYGADcBjfIdAc1H1Z7JU+gDgSDogcBJIqx4YNOvvvzQhkfvaYgawMg7pB27v9xF4Qzh1BWXCVzMqQblGqNT1KnBJFFdv4o+2CsHvh9RDPJx8lFhLkSBqriqRyZnbtibq0E2bkSiaa8jUo+UUqRIIBAQTKGVTE5hFTirr8VTgvHHkzBgkIMigqUYlbZcRlUNWjB98JQvD+o85MWzO09Tty3hp83TmT1vPhAIgh4ItAkuTbz8yPLKXy1/eOOK7Iq+KVtENj8LXI8RpbpJXkYAAwJUfE+4H3SiPrnti+RIAUvHLpdjiK+qJ5cu93pP6jeS2ULhesshp1YE3cQ2RWLg5507gRcCjBhUSCVGVo9a1r2yxxfmTHjPs1PisdaJuF8g+SwCqRZmpgcCgdOPUBQXaK9rUQDAuM5T64d2XfZf63e+dndKDW7kqMoxBTQW60XKQMFQGJAqjLqhIW6P3qXBEz8JLNLkmPKVIwOB8dO7xVui+tGkpmigjBo4vzeUZDhK65Y+QC5SP4aVYP1M9CqJMaxy6J4JPSf+Q5fq7j+6vN9cCyjYsq9AIN9+V5iyGqQ8EAgReiDQNiRMWLDjsW7PbVm0cXX9mm6JsShMGD06qvbuZNYXoDkncusL0YqLxPLWrORtT49x4xQ5suV95gs/7YvZlAAFHyWtp2LpQ2CbAUwKQQ4ZW46hmWGbh3Yb9C9Deg382eX9Lj9krPNeT8gdsRE+7nsPBAIhQg8ESh55RkKY1++SA4eSur/bvnnHfQd1P0SPPWvNsitIYwWi/DhWlzxHU7V3sXe8+uKxtz8ON3DEW5r6KJ98RfqpbuZSKGyUQ0VShoGZodsH9Rhw3yen/uW91dLZGd+k7LYOkMBAoHD+94Qm//b8AgVo+5a5QCAQBD1whuHsSQVQg6Sa/2NK10mXL6xZOFcNQZH6avcIIOsjaJdqdhrb5CdLR7Vw6QnF08cqumveGtZW0a3bFlAYbTKcUUT+XQhADIXAqIu0WSKMKh+7s1NlxZ0jBg3/4419r9mPlP33AsoWzu+Ni4bHAMUV7EHIA4HTn5ByD7Tfixfq3dciKCl+/cZvh7+6efmCVdkVw3LGSXikCiMGQgohJ1ztHSGXSYiscVPVSEBIEalAiaECqGFUaTUGRgNfnN5n2i/fP/6Sf6225SiTDBJiZ0Or9qhhLoFAIETogcApwhVuAZHEuHrABzc11uYuqa2pX741u6kCSJEyYIlABVvS9u9+Yrz5jDUuOnfpfedxr6zoZrvrqHjYs3069/z6zLHv+tOUeHxicmVgBSwJxKfTI/VNeWGfPBAIgh4InEpYGMJuIIsli8gyPjTuug2N6+xndbv9zy3YBAL87G6BkY4SjbrFiREpVKNHEqN3Wc/DgyoGPDyk39Af3TDoyoWRRIBGgBpXP6ACQBCJH6mipsjkJhAIBEEPBE4h5O1klFOIuh7zm0fd+MOfqWmk3ebH62VTzMrIiEIo7RA91HljW1JGBVUlPdBj49Q+47/fwNmfnD/u/NopNB5IXZEeyLqecz+pjb12WyJkGTAa9sYDgSDogcApxrIFYArtaG5XnQAS3DT6up/Fxmwq2535r/WNr422ZKHK7V69VBXlNsLgzMDa8kznX43oOvQnfzHu+hcquMLNd1cFS+zFPOeq1BWIhCF+75x85b6BLfEs+EAgcEoDnFAUF2i/V68Amncyy9ens69uJ0AEv9zy80Eb929/YMXhDbOPcB0UCdi3rok3TgG881vRiBIqVHg3LQCaZpu7wrTCTNejcgbN/07+5/RN7nX5jrZ8wT35vxca6tR4J7sEGRujh3Sv7RJ3Wzp6wOjHbhh39bc6SyXYRgAZWAhIBUKCCATyAp4/F3lnODcAxpneEJzTXQjQA4Eg6IHA6RnFwkDJGcawEn6+4xE6fGT/pzftW//t9fWbyzUCUrLO3c33o4sP8hVNg10IKAi+194iMSZACXSUgAtbKFz1uPEu8Nb/biNN4ulEXAvirQSQOFtWQwKrFqRl6Epd0SfTe37XTJeHh/Ya/Oy1w67dEGkUHNsCgUAQ9MAZcFHDVXMrEWIbwwggUQP+dOj5ros2rPjO4gNrLz1kavoTcq6JTQnCUmQrw87XXblgDiPIC74rrGNNm4S+oK7i28DY28i6gjMXfVOz6L0pEiff+y0gKDJSjm7cfVe3qh4b+nCPRy6eedH3x2B0Q5kth6pAjXtdltBuFggEgqAHOjhGAUsKywpoBKMGirRguPLDV3/Q+40juz4ipPesqnutOjUpFOL2oJkKld8EBYOckBb84JzYW+Qd4bww+/x53va1KLCHqrdQJfisAPwgGIJRg06NXdG/a7+Xk0if7BF3f2xEt+Ebrhp22Z5I81G8uv5ycGGxQQjDxwOBQBD0wBkRo7vpYETq98rze8cEIZeSXxGtih5fsuADB+v2XZnl7CV7avf2bkAjrElhOXH71xIBBvC/BSzsR60U7683zSt3LXIuvW7yo0eJvRmMotyWYWD5gAONhxteGdit/6s2k6ycN+vK34zTEbUVaSWMNQBZJFEC1Qikxkf4CpD1r+myCIFAIBAEPdChyReCGWE/7lMK3dapcSKcsQRWhiXAsuDxhgXdFi9ZXN2lvNNNNbm9I7LUeN22Q1vBJlOW0yRKTQpLFiACaX6wqE+XK3u7VAILUEYRUpEGEiODuw5EpSl/bP/+/asnDZi2ePH2xUvmnj03N7f3xfsyEoNhkJCAVXyNvjddVQLU+CyA6x8nP8vcud6FUrZAIBAEPdDhr2rrq9+NT1f7Sm8AOKoIzg1dc+JsSaGssCSw/n9/qvnTOYsWLxqw8/Bu2FgwrPfwPr269J5GjJ4CC0Nct2vf7hc379m8E8qoRCVmTZiJ8aPGPTnSDD8Sa4QIMYwasLgoPp+ez0f4Rt1IFC0McslH386rHfl9eV+lTkWFe4FAIBAEPRBo8d3i9sPdX6k40x4IBAKnnGAsEwicKIrmbWpByAOBwGlEqKwJBAKBQCAIeiAQCAQCgSDogUAgEAgEgqAHAoFAIBAIgh4IBAKBQBD0QCAQCAQCQdADgUAgEAgEQQ8EAoFAIBAEPRAIBAKBIOiBQCAQCASCoAcCgUAgEAiCHggEAoFAIAh6IBAIBAJB0AOBQCAQCARBDwQCgUAgEAQ9EAgEAoFAEPRAIBAIBIKgBwKBQCAQOJ35/xdhAfHwm9R2AAAAAElFTkSuQmCC" class="logo-adjust">
		</div>
		<div class="prt-full-w prt-right prt-bold">
		ORIGINAL
		</div>
	</div>
	<div class="prt-sans prt-bold prt-v-pad">
		Attachment Page 2 of 2
	</div>

	<div class="prt-v-pad">
		<table class="prt-sans prt-bold">
			<tr>
				<td class="prt-key-3">Invoice No</td>
				<td class="prt-dot-3">:</td>
				<td class="prt-value prt-value-3">
					<span class="prt-api prt_field_invoiceName">
					</span>
				</td>
			</tr>
			<tr>
				<td class="prt-key-3">Date</td>
				<td class="prt-dot-3">:</td>
				<td class="prt-value prt-value-3">
					<span class="prt-api prt_field_dateOfIssue">
					</span>
				</td>
			</tr>
			<tr>
				<td class="prt-key-3">Laycan</td>
				<td class="prt-dot-3">:</td>
				<td class="prt-value prt-value-3">
					<span class="prt-api prt_field_laycan">
					</span>
				</td>
			</tr>
		</table>
	</div>

	<div class="prt-v-pad">
		<table class="prt-sans prt-fsz-small">
			<tr>
				<td class="prt-key-4">The Name of Vessel</td>
				<td class="prt-dot-4">:</td>
				<td class="prt-value prt-value-4">
				<span class="prt-api prt_field_invoiceVesselName">
						</span>
				</td>
			</tr>
			<tr>
				<td class="prt-key-4">Bill of Lading Number</td>
				<td class="prt-dot-4">:</td>
				<td class="prt-value prt-value-4">
				<span class="prt-api prt_field_invoiceBillOfLadingNumber">
						</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">Bill of Lading Date</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
					<span class="prt-api prt_field_invoiceBillOfLadingDate">
						</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">From</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
					<span class="prt-api prt_field_vesselFrom">
						</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">To</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
					<span class="prt-api prt_field_vesselTo">
						</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">Payment</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
					<span class="prt-api prt_field_invoicePayment">
						</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">Letter of credit number</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
				<span class="prt-api prt_field_letterOfCreditNumber">
						</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">Date of issue L/C</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
					<span class="prt-api prt_field_lcDateIssue">
					</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">Bank of Issuance L/C</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
					<span class="prt-api prt_field_lcIssuingBank">
					</span>
				</td>
			</tr>
			<tr>
				<td class="prt-fsz-small prt-key-3">Contract Number</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
				<span class="prt-api prt_field_contractNumber">
					</span>
				</td>
				<td class="prt-fsz-small prt-value prt-value-3">DATED</td>
				<td class="prt-fsz-small prt-dot-3">:</td>
				<td class="prt-fsz-small prt-value prt-value-3">
				<span class="prt-api prt_field_invoiceDueDate">
				</span>
				</td>
			</tr>
		</table>
	</div>

	<hr>

	<div class="prt-sans">
		<div class="prt-uline prt-bold">
			Cert. No : .............................
		</div>

		<!-- div class="prt-bold">
			THE CHARACTERISTICS LISTED BELOW IN ACCORDANCE WITH THE ISO STANDARD:
		</div -->
	</div>

</div>
</div>
</div>
`).appendTo($("body")[0]);

        const prt_salesInvoice = (prt_data) => {
            const prt_fn_table_items = function (tableData) {
                const prt_tableRef = prt_getClassElm("prt_table_items");
                let tableContent = '';
                for (let i = 0; i < tableData.length; i++) {
                    const tableElm = tableData[i];
                    // ${tableElm.invoice_item_type}
                    if (tableElm.invoice_type != 'tax' && tableElm.invoice_item_type != 'Initial Price') {
                        tableContent += `
					<tr>
					<td class="prt-fsz-small prt-colc-1"><span class="prt-api prt_field_table1-c1-r${i + 1}">${tableElm.invoice_item}</span></td>
					
					<td class="prt-fsz-small prt-colc-2 prt-right"><span class="prt-api  prt_field_table1-c2-r${i + 1}">${tableElm.actualValue}</span></td>
					
					<td class="prt-fsz-small prt-colc-3 prt-right "><span class="prt-api  prt_field_table1-c4-r${i + 1}">${tableElm.value}</span></td>
					
					<td class="prt-fsz-small prt-colc-4 prt-right"><span class="prt-api prt_field_table1-c5-r${i + 1}">${tableElm.total_invoice}</span></td>
					</tr>
					`;
                    }
                }
                prt_tableRef.insertAdjacentHTML('beforeend', tableContent);
            }

            const num2text = {
                ones: ['', 'ONE', 'TWO', 'THREE', 'FOUR', 'FIVE', 'SIX', 'SEVEN', 'EIGHT', 'NINE', 'TEN', 'ELEVEN', 'TWELVE', 'THIRTEEN', 'FOURTEEN', 'FIFTEEN', 'SIXTEEN', 'SEVENTEEN', 'EIGHTEEN', 'NINETEEN'],
                tens: ['', '', 'TWENTY', 'THIRTY', 'FOURTH', 'FIFTY', 'SIXTY', 'SEVENTY', 'EIGHTY', 'NINETY'],
                sep: ['', ' THOUSAND ', ' MILLION ', ' BILLION ', ' TRILLION ', ' QUADRILLION ', ' QUINTILLION ', ' SEXTILLION ']
            };
            const convert = function (val, unit) {
                if (val.length === 0) {
                    return '';
                }

                val = val.replace(/,/g, '');
                if (isNaN(val)) {
                    return 'Invalid input.';
                }


                let [val1, val2] = val.split(".")
                let str2 = "";
                if (val2 != null && val2 != '') {
                    //convert the decimals here
                    var digits = (val2 + "0").slice(0, 2).split("");
                    str2 = num2text.tens[+digits[0]] + " " + num2text.ones[+digits[1]]
                }
                let arr = [];
                while (val1) {
                    arr.push(val1 % 1000);
                    val1 = parseInt(val1 / 1000, 10);
                }
                let i = 0;
                let str = "";
                while (arr.length) {
                    str = (function (a) {
                        var x = Math.floor(a / 100),
                            y = Math.floor(a / 10) % 10,
                            z = a % 10;

                        return (x > 0 ? num2text.ones[x] + ' HUNDRED ' : '') +
                            (y >= 2 ? num2text.tens[y] + ' ' + num2text.ones[z] : num2text.ones[10 * y + z]);
                    })(arr.shift()) + num2text.sep[i++] + str;
                }

                return str +
                    ' ' + unit + ' ' +
                    (str2 ? ' AND ' + str2 + ' CENTS' : '') +
                    ' ';
            };

            const prt_fn_toWords = (prt_num, prt_unit) => {
                return convert(prt_num, prt_unit);
            }

            const prt_fn_getMainPrice = (prtInvoices) => {
                return prtInvoices.filter(prtx => prtx.invoice_item_type == "Initial Price")[0];
            }

            const prt_fn_fillAll = (prt_className, prt_fillData) => {
                const prt_classElements = document.getElementsByClassName(prt_className);
                for (let i = 0; i < prt_classElements.length; i++) {
                    prt_classElements[i].innerText = prt_fillData;
                }
            }

            const prt_getClassElm = (prt_className) => {
                return document.getElementsByClassName(prt_className)[0];
            }

            const prt_invoices = prt_data.retVal;
            prt_fn_fillAll("prt_field_laycan", prt_data.invoiceLaycanDate);
            prt_fn_fillAll("prt_field_invoiceVesselName", prt_data.invoiceVesselName);
            prt_fn_fillAll("prt_field_contractNumber", prt_data.salesContractNumber);
            prt_fn_fillAll("prt_field_vesselFrom", prt_data.vesselFrom);
            prt_fn_fillAll("prt_field_vesselTo", prt_data.vesselTo);
            prt_fn_fillAll("prt_field_invoiceBillOfLadingNumber", prt_data.invoiceBillOfLadingNumber);
            prt_fn_fillAll("prt_field_invoiceBillOfLadingDate", prt_data.invoiceBillOfLadingDate);
            prt_fn_fillAll("prt_field_invoicePayment", prt_data.invoicePayment);
            prt_fn_fillAll("prt_field_letterOfCreditNumber", prt_data.letterOfCreditNumber);
            prt_fn_fillAll("prt_field_lcDateIssue", prt_data.lcDateIssue);

            // prt_fn_fillAll("prt_field_invoiceDueDate", prt_data.invoiceDueDate);
            prt_fn_fillAll("prt_field_invoiceNotes", prt_data.invoiceNotes);
            prt_fn_fillAll("prt_field_invoiceCurrencySymbol", prt_data.invoiceCurrencySymbol);
            prt_fn_fillAll("prt_field_invoiceName", prt_data.invoiceName);
            prt_fn_fillAll("prt_field_dateOfIssue", prt_data.dateOfIssue);
            prt_fn_fillAll("prt_field_lcIssuingBank", prt_data.lcIssuingBank);
            // //console.log("elRef", prt_getClassElm("prt_field_issuedByUser"), prt_data.issuedByUser);
            prt_getClassElm("prt_field_invoiceTotal").innerText = prt_data.invoiceTotal;
            prt_getClassElm("prt_field_invoiceBuyerName").innerText = prt_data.invoiceBuyerName;
            prt_getClassElm("prt_field_generated__invoiceTotalWords").innerText = prt_fn_toWords(prt_data.invoiceTotal.toString(), prt_data.invoiceCurrencyName);

            prt_var_mainPrice = prt_fn_getMainPrice(prt_invoices);
            if (prt_var_mainPrice) {
                // //console.log("Initial Price", prt_var_mainPrice);
                prt_getClassElm("prt_field_retVal__quantity").innerText = prt_var_mainPrice.quantity;
                prt_getClassElm("prt_field_retVal__price").innerText = prt_var_mainPrice.price;
                prt_getClassElm("prt_field_retVal__value").innerText = prt_var_mainPrice.value;
            }
            else { alert("Main Price not found!"); }

            prt_fn_table_items(prt_invoices);
        }

        prt_salesInvoice(all_datas);

        const ikon_container = prt_getClassElm("ikon-cetak-container");
        if (ikon_container) {
            ikon_container.innerHTML = iconLoading;
        }

        setTimeout(() => {
            const ikon_container = prt_getClassElm("ikon-cetak-container");
            if (ikon_container) {
                ikon_container.innerHTML = iconPrint;
            }
            window.print();
        }, 2000);

        setTimeout(() => {
            loadStyle('/css/displaymodes/mode-normal.css');
            $(".elemen-cetak").remove();
        }, 4000)
    }

    const loadStyle = (letak) => {
        //console.log("masuk");
        const headTag = $("head")[0];
        const theme = $("#tema-css-klien")[0];
        if (theme) {
            theme.href = letak;
        }
        else {
            const styleElm = document.createElement('link');
            styleElm.href = letak;
            //'/css/components/mode-cetak.css'
            styleElm.rel = 'stylesheet';
            styleElm.id = 'tema-css-klien';
            $(styleElm).appendTo(headTag);
        }
    };


    //bob
    let salesInvoiceAttachmentDataGrid

    const addAttachmentPopupOptions = {
        width: 500,
        height: "auto",
        showTitle: true,
        title: "Add Attachment",
        visible: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        contentTemplate: function () {
            let salesInvoiceIdInput =
                $("<div>")
                    .dxTextBox({
                        name: "sales_invoice_id",
                        value: salesInvoiceData.id,
                        readOnly: true,
                        visible: false
                    })

            let attachmentInput =
                $("<div class='mb-5 dx-fileuploader-mcs'>")
                    .dxFileUploader({
                        uploadMode: "useForm",
                        multiple: false,
                        maxFileSize: maxFileSize,
                        invalidMaxFileSizeMessage: "Max. file size is 50 Mb"
                    })

            let submitButton =
                $("<div>")
                    .dxButton({
                        text: "Submit",
                        onClick: function (e) {
                            let salesInvoiceId = salesInvoiceIdInput.dxTextBox("instance").option("value")
                            let file = attachmentInput.dxFileUploader("instance").option("value")[0]

                            var reader = new FileReader();
                            reader.readAsDataURL(file);
                            reader.onload = function () {
                                let fileName = file.name
                                let fileSize = file.size
                                let data = reader.result.split(',')[1]

                                if (fileSize >= maxFileSize) {
                                    return;
                                }

                                let formData = {
                                    "salesInvoiceId": salesInvoiceId,
                                    "fileName": fileName,
                                    "fileSize": fileSize,
                                    "data": data
                                }

                                $.ajax({
                                    url: "/api/Sales/SalesInvoiceAttachment/InsertData",
                                    data: JSON.stringify(formData),
                                    type: "POST",
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        addAttachmentPopup.hide()
                                        salesInvoiceAttachmentDataGrid.refresh()
                                    }
                                })
                            }
                        }
                    })

            let formContainer = $("<form enctype='multipart/form-data'>")
                .append(salesInvoiceIdInput, attachmentInput, submitButton)

            return formContainer;
        }
    }

    const addAttachmentPopup = $("<div>")
        .dxPopup(addAttachmentPopupOptions).appendTo("body").dxPopup("instance")

    const openAddAttachmentPopup = function () {
        addAttachmentPopup.option("contentTemplate", addAttachmentPopupOptions.contentTemplate.bind(this));
        addAttachmentPopup.show()
    }
});