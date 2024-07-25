$(function () {

    var token = $.cookie("Token");
    var areaName = "Sales";
    var entityName = "SalesContract";
    var salesContractUrl = "/api/" + areaName + "/" + entityName;

    /**
     * ===================
     * Sales Contract Grid
     * ===================
     */


    $("#sales-contract-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: salesContractUrl + "/DataGrid",
            insertUrl: salesContractUrl + "/InsertData",
            updateUrl: salesContractUrl + "/UpdateData",
            deleteUrl: salesContractUrl + "/DeleteData",
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
                dataField: "sales_contract_name",
                dataType: "string",
                caption: "Sales Contract Name",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }]
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Contract Start Date",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "Contract End Date",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "document_reference",
                dataType: "string",
                caption: "Document Reference",
            },
            {
                dataField: "customer_id",
                dataType: "string",
                caption: "Buyer",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Sales/Customer/CustomerIdLookup",
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
                editorOptions: {
                    onOpened: function (e) {
                        renderAddNewButton("/Sales/Customer/Index")

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
                    rowData.customer_id = value
                }
            },
            {
                dataField: "buyer_agent_id",
                dataType: "string",
                caption: "Buyer Agent",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Sales/Customer/CustomerIdLookup",
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
                editorOptions: {
                    onOpened: function (e) {
                        renderAddNewButton("/Sales/Customer/Index")

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
                /*setCellValue: function (rowData, value) {
                    rowData.customer_id = value
                }*/
            },
            {
                dataField: "end_user_id",
                dataType: "string",
                caption: "End User",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "seller_id",
                dataType: "string",
                caption: "Seller",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "seller_agent_id",
                dataType: "string",
                caption: "Seller Agent",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Sales/Customer/CustomerIdLookup",
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "contract_basis_id",
                dataType: "string",
                caption: "Contract Basis",
                visible: false,
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
                            filter: ["item_group", "=", "contract-basis"]
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
            {
                dataField: "commitment_id",
                dataType: "string",
                caption: "Commitment",
                visible: false,
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
                            filter: ["item_group", "=", "commitment"]
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
            {
                dataField: "contract_status_id",
                dataType: "string",
                caption: "Contract Status",
                visible: false,
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
                            filter: ["item_group", "=", "contract-status"]
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
            {
                dataField: "invoice_target_id",
                dataType: "string",
                caption: "Invoice Target",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            /*{
                dataField: "credit_limit_activation",
                dataType: "boolean",
                caption: "Credit Limit Activation",
                allowEditing: false
            },*/
            {
                dataField: "surveyor_id",
                dataType: "string",
                caption: "Surveyor",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Organisation/Contractor/ContractorIdLookupByIsSurveyor?isSurveyor=true",
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
            {
                dataField: "description",
                dataType: "string",
                caption: "Description",
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
                caption: "Detail",
                type: "buttons",
                width: 150,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "See Contract Terms",
                    text: "Open Detail",
                    onClick: function (e) {
                        salesContractId = e.row.data.id
                        window.location = "/Sales/SalesContract/Detail/" + salesContractId
                    }
                }]
            },
            {
                type: "buttons",
                buttons: ["edit", "delete"]
            }
        ],
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            // Set onValueChanged for sales_contract_id
            if (e.parentType === "dataRow" && e.dataField == "customer_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let customer_id = e.value;

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Sales/Customer/GetCustomerById?Id=' + customer_id,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let customerData = response.data[0];

                            // Set its corresponded field's value
                            grid.cellValue(index, "credit_limit_activation", customerData.credit_limit_activation)
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
        },
        masterDetail: {
            enabled: false,
            template: function (container, options) {

                // Not used again
                // Documentation-purpose-only

                var currentRecord = options.data;

                // Sales Contract Information Container
                renderSalesContractInformation(currentRecord, container)
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
                        dataField: "sales_contract_name",
                        colSpan: 2
                    },
                    {
                        dataField: "start_date"
                    },
                    {
                        dataField: "end_date"
                    },
                    {
                        dataField: "seller_id"
                    },
                    {
                        dataField: "seller_agent_id"
                    },
                    {
                        dataField: "customer_id"
                    },
                    {
                        dataField: "buyer_agent_id"
                    },
                    {
                        dataField: "document_reference"
                    },
                    //{
                    //    dataField: "end_user_id"
                    //},
                    {
                        dataField: "contract_basis_id"
                    },
                    {
                        dataField: "commitment_id"
                    },
                    {
                        dataField: "contract_status_id"
                    },
                    {
                        dataField: "surveyor_id"
                    },
                    //{
                    //    dataField: "invoice_target_id"
                    //},
                    /*{
                        dataField: "credit_limit_activation",
                        colSpan: 2
                    },*/
                    {
                        dataField: "description",
                        editorType: "dxTextArea",
                        colSpan: 2,
                        editorOptions: {
                            height: 50
                        }
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

    // Sales Contract information
    // Not used - documentation-purpose only
    const renderSalesContractInformation = function (currentRecord, container) {
        let startDateFormatted = currentRecord.start_date ? moment(currentRecord.start_date.split('T')[0]).format("D MMM YYYY") : '-'
        let endDateFormatted = currentRecord.end_date ? moment(currentRecord.end_date.split('T')[0]).format("D MMM YYYY") : '-'

        $(`
            <div>
                <div class="row align-items-center mb-3">
                    <div class="col-md-6">
                        <h5>Sales Contract</h5>
                    </div>
                    <div class="col-md-6 d-flex justify-content-end">
                        <button class="btn btn-primary" onclick="openContractTerms('${currentRecord.id}')">See Contract Terms</button>
                    </div>
                </div>
                
                <div class="row mb-5">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Overview</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-6 pr-0">
                                    <div class="headline-title-container">
                                        <small class="font-weight-normal d-block mb-1">Contract Name</small>
                                        <h4 class="headline-title font-weight-bold">${(currentRecord.contract_name ? currentRecord.contract_name : "-")}</h4>
                                    </div>
                                </div>
                                <div class="col-md-6 pl-0">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-list fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Contract Number</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.contract_number ? currentRecord.contract_number : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-calendar-alt fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Sales Plan Name</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.sales_plan_name ? currentRecord.sales_plan_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-user fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Customer</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.customer_name ? currentRecord.customer_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-box fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Product</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.product_name ? currentRecord.product_name : "-")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Contract Information</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Start Date</small>
                                                    <p class="font-weight-bold m-0">${ startDateFormatted }</p>
                                                </div>
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Ship To</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.ship_to ? currentRecord.ship_to : "-")}</p>
                                                </div>
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Bill To</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.bill_to ? currentRecord.bill_to : "-")}</p>
                                                </div>
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Demurrage Rate</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.demurrage_rate ? currentRecord.demurrage_rate : "-")}</p>
                                                </div>
                                                <div>
                                                    <small class="font-weight-normal text-muted d-block mb-1">Loading Rate</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.loading_rate ? currentRecord.loading_rate : "-")}</p>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">End Date</small>
                                                    <p class="font-weight-bold m-0">${ endDateFormatted }</p>
                                                </div>
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Ship Address</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.ship_to_address ? currentRecord.ship_to_address : "-")}</p>
                                                </div>
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Bill Address</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.bill_to_address ? currentRecord.bill_to_address : "-")}</p>
                                                </div>
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Despatch Percent</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.despatch_percent ? currentRecord.despatch_percent : "-")}</p>
                                                </div>
                                                <div>
                                                    <small class="font-weight-normal text-muted d-block mb-1">Turn Time</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.turn_time ? currentRecord.turn_time : "-")}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `).appendTo(container)
    }

    
   
    // Functions
    // Not used - documentation-purpose only

    window.openContractTerms = function(contractId) {
        $("[href='#sales-contract-term-container']").tab("show")
        salesContractTermGrid.columnOption("sales_contract_id", {
            filterValue: contractId
        })
    }
});