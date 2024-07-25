$(function () {

    var token = $.cookie("Token");
    var areaName = "Sales";
    var entityName = "Customer";
    var url = "/api/" + areaName + "/" + entityName;

    var customerData = null

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
            {
                dataField: "business_partner_name",
                dataType: "string",
                caption: "Customer Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "business_partner_code",
                dataType: "string",
                caption: "Customer Code",
                editorOptions: {
                    readOnly: false, //set to true if Code already updated automatically from ellipse
                },
            },
            {
                dataField: "alias_name",
                dataType: "string",
                caption: "Alias Name",
                visible: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
            },
            {
                dataField: "customer_type_id",
                dataType: "string",
                caption: "Customer Type",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/CustomerType/CustomerTypeIdLookup", // API for Customer Type
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
                dataField: "customer_category_id",
                dataType: "string",
                caption: "Customer Category",
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
                            filter: ["item_group", "=", "customer-category"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "customer_status_id",
                dataType: "string",
                caption: "Status",
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
                            filter: ["item_group", "=", "customer-status"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "industry_type_id",
                dataType: "string",
                caption: "Industry Type",
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
                            filter: ["item_group", "=", "industry-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "primary_address",
                dataType: "string",
                caption: "Primary Address",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
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
                }
            },
            {
                dataField: "primary_contact_name",
                dataType: "string",
                caption: "Primary Contact Name",
            },
            {
                dataField: "primary_contact_email",
                dataType: "string",
                caption: "Primary Contact Email",
            },
            {
                dataField: "primary_contact_phone",
                dataType: "string",
                caption: "Primary Contact Phone",
            },
            {
                dataField: "secondary_contact_name",
                dataType: "string",
                caption: "Secondary Contact Name",
                visible: false
            },
            {
                dataField: "secondary_contact_email",
                dataType: "string",
                caption: "Secondary Contact Email",
                visible: false
            },
            {
                dataField: "secondary_contact_phone",
                dataType: "string",
                caption: "Secondary Contact Phone",
                visible: false
            },
            {
                dataField: "additional_information",
                dataType: "string",
                caption: "Additional Information",
                visible: false
            },
            {
                dataField: "bank_id",
                dataType: "string",
                caption: "Bank",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/Bank/BankIdLookup", // API for Bank
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
                    rowData.bank_id = value
                    rowData.bank_account_id = null
                }
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
                            filter: options.data ? ["bank_id", "=", options.data.bank_id] : null
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "currency_id",
                dataType: "string",
                caption: "Currency",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/Currency/CurrencyIdLookup", // API for Currency
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
                dataField: "is_taxable",
                caption: "Tax",
                dataType: "boolean",
                visible: false,
                editCellTemplate: function (cellElement, cellInfo) {
                    $("<div>").dxRadioGroup({
                        items: [
                            {
                                text: "Yes",
                                value: true
                            },
                            {
                                text: "No",
                                value: false
                            }
                        ],
                        valueExpr: "value",
                        displayExpr: "text",
                        layout: "horizontal",
                        value: cellInfo.data.is_taxable,
                        onValueChanged: function (e) {
                            cellInfo.setValue(e.value)
                        }
                    }).appendTo(cellElement);
                }
            },
            {
                dataField: "credit_limit",
                dataType: "number",
                caption: "Credit Limit Value",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
            },
            {
                dataField: "credit_limit_activation",
                dataType: "boolean",
                caption: "Credit Limit Activation"
            },
            {
                dataField: "is_active",
                dataType: "boolean",
                caption: "Is Active",
                width: "10%",
            }
        ],
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                let currentRecord = options.data;
                customerData = currentRecord

                // Customer Detail Information Container
                renderCustomerDetailInformation(currentRecord, container) // UI/UX Improvement
            }
        },
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0) {
                selectedIds = $.map(data, function (value) {
                    return value.id;
                }).join(",");
                $("#dropdown-download-selected").removeClass("disabled");
                $("#dropdown-download-payment-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-download-selected").addClass("disabled");
                $("#dropdown-download-payment-selected").addClass("disabled");
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
            if (e.dataField === "bank_account_id" && e.parentType === "dataRow") {
                e.editorOptions.disabled = !e.row.data.bank_account_id && !e.row.data.bank_id || e.row.data.bank_id === null;
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
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 1,
                items: [
                    {
                        itemType: "group",
                        caption: "Detail",
                        colCount: 2,
                        items: [
                            {
                                dataField: "business_partner_name"
                            },
                            {
                                dataField: "business_partner_code"
                            }, 
                            {
                                dataField: "alias_name"
                            },
                            {
                                dataField: "customer_type_id"
                            },
                            {
                                dataField: "customer_category_id"
                            },
                            {
                                dataField: "customer_status_id"
                            },
                            {
                                dataField: "industry_type_id"
                            },
                            {
                                dataField: "primary_address"
                            },
                            {
                                dataField: "country_id",
                            },
                            {
                                dataField: "primary_contact_name",
                                colSpan: 2
                            },
                            {
                                dataField: "primary_contact_email"
                            },
                            {
                                dataField: "primary_contact_phone"
                            },
                            {
                                dataField: "secondary_contact_name",
                                colSpan: 2
                            },
                            {
                                dataField: "secondary_contact_email"
                            },
                            {
                                dataField: "secondary_contact_phone"
                            },
                            {
                                dataField: "additional_information",
                                editorType: "dxTextArea",
                                editorOptions: {
                                    height: 50,
                                },
                                colSpan: 2
                            },
                            {
                                dataField: "is_active"
                            }
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "Bank Detail",
                        colCount: 2,
                        items: [
                            {
                                dataField: "bank_id"
                            },
                            {
                                dataField: "bank_account_id"
                            },
                            {
                                dataField: "currency_id",
                            },
                            {
                                dataField: "is_taxable",
                            }
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "Credit Limit",
                        colCount: 2,
                        items: [
                            {
                                dataField: "credit_limit"
                            },
                            {
                                dataField: "credit_limit_activation",
                                
                            },
                        ]
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

    const renderCustomerDetailInformation = async function (currentRecord, container) {

        var remainedCreditLimit = 0

        remainedCreditLimit = await new Promise((resolve) => {
            $.ajax({
                url: '/api/Sales/Customer/RemainedCreditLimit?customer_id=' + currentRecord.id,
                type: "GET",
                contentType: "application/json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                },
                success: function (response) {
                    remainedCreditLimit = response
                    resolve(remainedCreditLimit)
                },
            })
        })

        customerData = currentRecord

        $(`
            <div>
                <h5 class="mb-3">Customer Detail</h5>

                <div class="row mb-4">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Overview</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-6 pr-0">
                                    <div class="headline-title-container">
                                        <small class="font-weight-normal d-block mb-1">Customer Name</small>
                                        <h4 class="headline-title font-weight-bold">${(currentRecord.business_partner_name ? currentRecord.business_partner_name : "-")}</h4>
                                    </div>
                                </div>
                                <div class="col-md-6 pl-0">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-building fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Customer Code</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.business_partner_code ? currentRecord.business_partner_code : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-user fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Customer Type</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.customer_type_name ? currentRecord.customer_type_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-flag fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Country</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.country_name ? currentRecord.country_name : "-")}</h5>
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
                        <div class="master-detail-caption mb-2">Primary Contact</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-user fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Primary Contact Name</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.primary_contact_name ? currentRecord.primary_contact_name : "-")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-phone fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Primary Phone</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.primary_contact_phone ? currentRecord.primary_contact_phone : "-")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-envelope fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Primary Contact Email</small>
                                                        <h5 class="font-weight-bold" style="word-break: break-all">${(currentRecord.primary_contact_email ? currentRecord.primary_contact_email : "-")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-map-marker-alt fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Primary Address</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.primary_address ? currentRecord.primary_address : "-")}</h5>
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

                <div class="row mb-5">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Bank Information</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Bank Name</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.bank_name ? currentRecord.bank_name : "-")}</p>
                                                </div>
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Account Holder</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.account_holder ? currentRecord.account_holder : "-")}</p>
                                                </div>
                                                <div>
                                                    <small class="font-weight-normal text-muted d-block mb-1">Is Taxable</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.is_taxable ? "Yes" : "No")}</p>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="mb-2">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Currency</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.currency_name ? currentRecord.currency_name : "-")}</p>
                                                </div>
                                                <div>
                                                    <small class="font-weight-normal text-muted d-block mb-1">Account Number</small>
                                                    <p class="font-weight-bold m-0">${(currentRecord.account_number ? currentRecord.account_number : "-")}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Other</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-check-circle fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Credit Limit Value</small>
                                                        <h5 class="font-weight-bold">${(formatNumber(currentRecord.credit_limit) || "-")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-check-circle fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Outstanding Invoice</small>
                                                        <h5 class="font-weight-bold">${(formatNumber(remainedCreditLimit) || "-")}</h5>
                                                        <button class="btn btn-primary btn-sm credit-limit-popup-btn"
                                                            data-customer-id="${currentRecord.id}"
                                                            data-customer-name="${currentRecord.business_partner_name}">Detail</button>

                                                        <button class="btn btn-primary btn-sm export-customer-invoice-btn"
                                                            data-customer-id="${currentRecord.id}" data-customer-name="${currentRecord.business_partner_name}">
                                                            Export
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-ellipsis-h fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Additional Information</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.additional_information ? currentRecord.additional_information : "-")}</h5>
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
            </div>
        `).appendTo(container);

        // Credit Limit Popup
        let creditLimitPopupOptions = {
            title: "Outstanding Invoice Detail",
            /* height: "'auto'", */
            height: "700",
            hideOnOutsideClick: true,
            contentTemplate: function () {

                let customerName = document.querySelector('[name="active-customer-name"]').value
                var creditLimitDetailContainer = $("<div>")
                $(`<div class="mb-3">

                       <div class="row">
                            <div class="col-12">
                                <h4 class="font-weight-bold">Outstanding Invoice <span>${customerName}</span></h4>

	                            <!-- <button type="button" class="btn btn-info btn-customer-invoice" onClick="hello();">
		                            <i class="fal fa-clipboard-check fa-lg"></i>
		                            Export
	                            </button> -->
                            </div>
                            <div class="col-12">
                                <table class="credit-limit-detail-table-js table table-striped mb-3">
                                    <thead>
                                        <tr>
                                            <td style="width: 2%"><b>No.</b></td>
                                            <td style="width: 8%"><b>No. Invoice</b></td>
                                            <td style="width: 25%"><b>Shipping Order</b></td>
                                            <td style="width: 16%"><b>Customer</b></td>
                                            <td style="width: 11%"><b>BL Date</b></td>
                                            <td style="width: 11%"><b>Payment Date</b></td>
                                            <td style="width: 5%"><b>Aging</b></td>
                                            <td><b>Billing</b></td>
                                            <td><b>Receipt</b></td>
                                            <td><b>Outstanding</b></td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                `).appendTo(creditLimitDetailContainer);

                creditLimitDetailContainer.dxScrollView({
                    width: '100%',
                    height: '100%',
                });

                // Get customer's credit limit history data
                let creditLimitDetailData = null
                let customerId = document.querySelector('[name="active-customer-id"]').value
                $.ajax({
                    url: "/api/Sales/Customer/CustomerTransactionHistory?Id=" + customerId,
                    type: "GET",
                    contentType: "application/json",
                    dataType: "json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    },
                    success: function (response) {
                        creditLimitDetailData = response
                        renderCreditLimitDetailTable(creditLimitDetailData, creditLimitDetailContainer)
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        //console.log(textStatus, errorThrown)
                    }
                })


                return creditLimitDetailContainer;
            }
        }
        var creditLimitPopup = $("#credit-limit-popup").dxPopup(creditLimitPopupOptions).dxPopup("instance");

        const showCreditLimitPopup = function (e) {
            document.querySelector('[name="active-customer-id"]').value = e.target.dataset.customerId
            document.querySelector('[name="active-customer-name"]').value = e.target.dataset.customerName

            creditLimitPopup.option("contentTemplate", creditLimitPopupOptions.contentTemplate.bind(this));
            creditLimitPopup.show()
        }

        const renderCreditLimitDetailTable = (data, container) => {
            let table = container.find(".credit-limit-detail-table-js");
            let tbody = table.find("tbody");

            let dataElements = [];
            let i = 1;
            let currentDate = new Date();
            data.forEach(d => {
                let bl_date = '';
                if (d.bl_date != null && d.bl_date != '')
                    bl_date = moment(d.bl_date);
                    
                let paydate = '';
                if (d.paydate != null)
                    paydate = moment(d.paydate).format("DD MMM YYYY");

                let aging = '';
                if (bl_date != '') {
                    if (paydate == '')
                        aging = currentDate - bl_date;
                    else
                        aging = moment(d.paydate) - bl_date;

                    aging = Math.floor(aging / 1000 / 60 / 60 / 24);
                }
                if (d.bl_date != null && d.bl_date != '') {
                    bl_date = bl_date.format("DD MMM YYYY");
                }

                dataElements.push(`<tr>
                                    <td>${i}.</td>
                                    <td>${d.invoice_number || '-'}</td>
                                    <td>${d.despatch_order_number || '-'}</td>
                                    <td>${d.business_partner_name || '-'}</td>
                                    <td>${bl_date || '-'}</td>
                                    <td>${paydate || '-'}</td>
                                    <td>${aging || '-'}</td>
                                    <td>${formatNumber(d.billing) || '-'}</td>
                                    <td>${formatNumber(d.receipt) || '-'}</td>
                                    <td>${formatNumber(d.outstanding) || '-'}</td>
                                </tr>`);
                i++;
            })

            tbody.prepend(dataElements.join(''))

            creditLimitPopup.repaint()

        }

        $('.credit-limit-popup-btn').unbind().on('click', showCreditLimitPopup);

        $(".export-customer-invoice-btn").unbind().on('click', downloadCustomerInvoice);


        // Customer Another Informations Tabs
        renderCustomerTabs(currentRecord, container)
    }

    const renderCustomerTabs = function (currentRecord, container) {
        let contactsContainer = $("<div>")
        let attachmentsContainer = $("<div>")
        let salesContractContainer = $("<div>")
        let creditLimitHistoryContainer = $("<div>")

        renderCustomerContact(currentRecord, contactsContainer)
        renderCustomerAttachment(currentRecord, attachmentsContainer)
        renderCustomerSalesContract(currentRecord, salesContractContainer)
        renderCustomerCreditLimitHistory(currentRecord, creditLimitHistoryContainer)

        let tabContainer = $(`
            <ul class="nav nav-mcs nav-pills pills-blue mb-3" role="tablist">
                <li class="nav-item"><a class="nav-link active" data-toggle="pill" href="#contacts-container-${currentRecord.id}"><i class="fas fa-address-card mr-2"></i>Contacts</a></li>
                <li class="nav-item"><a class="nav-link" data-toggle="pill" href="#attachments-container-${currentRecord.id}"><i class="fas fa-file mr-2"></i>Attachments</a></li>
                <li class="nav-item"><a class="nav-link" data-toggle="pill" href="#sales-contract-container-${currentRecord.id}"><i class="fas fa-list-alt mr-2"></i>Contracts</a></li>
                <li class="nav-item"><a class="nav-link" data-toggle="pill" href="#credit-limit-history-container-${currentRecord.id}"><i class="fas fa-bookmark mr-2"></i>Credit Limit History</a></li>
            </ul>
            <div class="tab-content py-3">
                <div class="tab-pane fade show active" id="contacts-container-${currentRecord.id}" role="tabpanel">
                </div>
                <div class="tab-pane fade" id="attachments-container-${currentRecord.id}" role="tabpanel">
                </div>
                <div class="tab-pane fade" id="sales-contract-container-${currentRecord.id}" role="tabpanel">
                </div>
                <div class="tab-pane fade" id="credit-limit-history-container-${currentRecord.id}" role="tabpanel">
                </div>
            </div>
        `).appendTo(container);

        contactsContainer.appendTo(tabContainer.find("#contacts-container-" + currentRecord.id ))
        attachmentsContainer.appendTo(tabContainer.find("#attachments-container-"+ currentRecord.id))
        salesContractContainer.appendTo(tabContainer.find("#sales-contract-container-"+ currentRecord.id))
        creditLimitHistoryContainer.appendTo(tabContainer.find("#credit-limit-history-container-"+ currentRecord.id))
    }

    const renderCustomerContact = function (currentRecord, container) {
        let contactUrlDetail = "/api/Organisation/Contact";
        let customerContactsContainer = $("<div class='mb-5'>")
        customerContactsContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-3")
            .text("Contacts")
            .appendTo(customerContactsContainer);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: contactUrlDetail + "/ByBusinessPartnerId/" + encodeURIComponent(currentRecord.id),
                    insertUrl: contactUrlDetail + "/InsertData",
                    updateUrl: contactUrlDetail + "/UpdateData",
                    deleteUrl: contactUrlDetail + "/DeleteData",
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
                        dataField: "business_partner_id",
                        dataType: "string",
                        caption: "Customer ID",
                        allowEditing: false,
                        visible: false,
                        formItem: {
                            visible: false
                        },
                        calculateCellValue: function () {
                            return currentRecord.id;
                        }
                    },
                    {
                        dataField: "contact_name",
                        dataType: "string",
                        caption: "Name",
                        formItem: {
                            colSpan: 2
                        },
                        validationRules: [{
                            type: "required",
                            message: "This field is required."
                        }]
                    },
                    {
                        dataField: "contact_email",
                        dataType: "string",
                        caption: "Email",
                        formItem: {
                            colSpan: 2
                        }
                    },
                    {
                        dataField: "contact_phone",
                        dataType: "string",
                        caption: "Phone"
                    },
                    {
                        dataField: "contact_position",
                        dataType: "string",
                        caption: "Position"
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
                    e.data.business_partner_id = currentRecord.id;
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
            }).appendTo(customerContactsContainer);
    }

    const maxFileSize = 52428800;

    let customerAttachmentDataGrid
    const renderCustomerAttachment = function (currentRecord, container) {
        let attachmentUrlDetail = "/api/Sales/CustomerAttachment";
        let customerAttachmentsContainer = $("<div class='mb-5'>")
        customerAttachmentsContainer.appendTo(container)

        let titleContainer = $(`
            <div class="row mb-3 align-items-center">
                <div class="col-md-6">
                    <div class="master-detail-caption">File Attachments</div>
                </div>
            </div>
        `).appendTo(customerAttachmentsContainer)

        customerAttachmentDataGrid = $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: attachmentUrlDetail + "/ByCustomerId/" + encodeURIComponent(currentRecord.id),
                    insertUrl: attachmentUrlDetail + "/InsertData",
                    updateUrl: attachmentUrlDetail + "/UpdateData",
                    deleteUrl: attachmentUrlDetail + "/DeleteData",
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
                        dataField: "customer_id",
                        dataType: "string",
                        caption: "Customer ID",
                        allowEditing: false,
                        visible: false,
                        calculateCellValue: function () {
                            return currentRecord.id;
                        }
                    },
                    {
                        dataField: "filename",
                        dataType: "string",
                        caption: "File name",
                        formItem: {
                            colSpan: 2
                        },
                        cellTemplate: function (container, options) {
                            let attachmentUrl = options.value
                            let attachmentName = /[^\\]*$/.exec(attachmentUrl)[0] // Get only the file name and its extension

                            $(`<span><i class="fas fa-file mr-2"></i>${attachmentName}</span>`).appendTo(container)
                        }

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
                                let attachment = e.row.data
                                let attachmentName = /[^\\]*$/.exec(attachment.filename)[0]

                                let xhr = new XMLHttpRequest()
                                xhr.open("GET", "/api/Sales/CustomerAttachment/DownloadDocument/" + attachment.id, true)
                                xhr.responseType = "blob"
                                xhr.setRequestHeader("Authorization", "Bearer " + token)

                                xhr.onload = function (e) {
                                    let blobURL = window.webkitURL.createObjectURL(xhr.response)

                                    let a = document.createElement("a")
                                    a.href = blobURL
                                    a.download = attachmentName
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
                                    openAddAttachmentPopup()
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
                    e.data.business_partner_id = currentRecord.id;
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
            }).appendTo(customerAttachmentsContainer).dxDataGrid("instance");
    }

    const addAttachmentPopupOptions = {
        width: 500,
        height: "auto",
        showTitle: true,
        title: "Add Attachment",
        visible: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        contentTemplate: function () {
            let customerIdInput =
                $("<div>")
                    .dxTextBox({
                        name: "customer_id",
                        value: customerData.id,
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
                            let customerId = customerIdInput.dxTextBox("instance").option("value")
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
                                    "customerId" : customerId,
                                    "fileName": fileName,
                                    "fileSize": fileSize,
                                    "data" : data
                                }

                                $.ajax({
                                    url: "/api/Sales/CustomerAttachment/InsertData",
                                    data: JSON.stringify(formData),
                                    type: "POST",
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        addAttachmentPopup.hide()
                                        customerAttachmentDataGrid.refresh()
                                    }
                                })   
                            }
                        }
                    })

            let formContainer = $("<form enctype='multipart/form-data'>")
                .append(customerIdInput, attachmentInput, submitButton)

            return formContainer;
        }
    }

    const addAttachmentPopup = $("<div>")
        .dxPopup(addAttachmentPopupOptions).appendTo("body").dxPopup("instance")

    const openAddAttachmentPopup = function () {
        addAttachmentPopup.option("contentTemplate", addAttachmentPopupOptions.contentTemplate.bind(this));
        addAttachmentPopup.show()
    }

    const renderCustomerSalesContract = function (currentRecord, container) {
        let salesContractUrlDetail = "/api/Sales/SalesContract";
        let customerSalesContractContainer = $("<div class='mb-5'>")
        customerSalesContractContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-3")
            .text("Sales Contract")
            .appendTo(customerSalesContractContainer);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: salesContractUrlDetail + "/ByCustomerId/" + encodeURIComponent(currentRecord.id),
                    insertUrl: salesContractUrlDetail + "/InsertData",
                    updateUrl: salesContractUrlDetail + "/UpdateData",
                    deleteUrl: salesContractUrlDetail + "/DeleteData",
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
                        dataField: "credit_limit_activation",
                        dataType: "boolean",
                        caption: "Credit Limit Activation",
                        validationRules: [{
                            type: "required",
                            message: "The field is required."
                        }],
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
                    e.data.business_partner_id = currentRecord.id;
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
            }).appendTo(customerSalesContractContainer);
    }

    const renderCustomerCreditLimitHistory = function (currentRecord, container) {
        let creditLimitHistoryUrl = "/api/Sales/Customer/CreditLimitHistory?customer_id=" + encodeURIComponent(currentRecord.id);
        let customerSalesContractContainer = $("<div class='mb-5'>")
        customerSalesContractContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-3")
            .text("Credit Limit History")
            .appendTo(customerSalesContractContainer);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: creditLimitHistoryUrl,
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
                        dataField: "credit_limit_value",
                        dataType: "number",
                        editorType: "dxNumberBox",
                        format: "fixedPoint",
                        caption: "Value",
                        editorOptions: {
                            step: 0
                        }
                    },
                    {
                        dataField: "created_on",
                        dataType: "date",
                        caption: "Changed At",
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
                    e.data.business_partner_id = currentRecord.id;
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
            }).appendTo(customerSalesContractContainer);
    }
    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;
            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');
            $.ajax({
                url: "/Sales/Customer/ExcelExport",
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
                if (data instanceof Blob) {
                    var a = document.createElement('a');
                    var url = window.URL.createObjectURL(data);
                    a.href = url;
                    a.download = "Customer.xlsx"; // Set the appropriate file name here
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                    toastr["success"]("File downloaded successfully.");
                } else {
                    toastr["error"]("File download failed.");
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnDownloadSelectedRow').html('Download');
            });
        }
    });

    const downloadCustomerInvoice = function (e) {
        var recordId = e.target.dataset.customerId;
        var customerName = e.target.dataset.customerName;

        if (recordId != null && recordId != '') {
            let payload = {};
            payload.selectedIds = recordId;
            $('.export-customer-invoice-btn')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');

            $.ajax({
                url: "/Sales/Customer/CreditLimitDetailExport",
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
            })
            .done(function (data, textStatus, xhr) {
                if (data instanceof Blob) {
                    var a = document.createElement('a');
                    var url = window.URL.createObjectURL(data);
                    a.href = url;
                    a.download = "Invoice Outstanding " + customerName + ".xlsx"; // Set the appropriate file name here
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                    toastr["success"]("File downloaded successfully.");
                } else {
                    toastr["error"]("File download failed.");
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            })
            .always(function () {
                $('.export-customer-invoice-btn').html('Export');
            });
        }
    }

    $('#btnDownloadSelectedPayment').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;
            $('#btnDownloadSelectedPayment')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');
            $.ajax({
                url: "/Sales/Customer/CustomerPaymentExport",
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
                if (data instanceof Blob) {
                    var a = document.createElement('a');
                    var url = window.URL.createObjectURL(data);
                    a.href = url;
                    a.download = "CustomerPayment.xlsx"; // Set the appropriate file name here
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                    toastr["success"]("File downloaded successfully.");
                } else {
                    toastr["error"]("File download failed.");
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnDownloadSelectedPayment').html('Download');
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
                url: "/api/Sales/Customer/UploadDocument",
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

    $('#btnUploadPayment').on('click', function () {
        var f = $("#fUploadPayment")[0].files;
        var filename = $('#fUploadPayment').val();

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

        $('#btnUploadPayment')
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
                url: "/api/Sales/Customer/UploadCustomerPayment",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(formData),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                alert('File berhasil di-upload!');
                $("#modal-upload-payment").modal('hide');
                $("#grid").dxDataGrid("refresh");
            }).fail(function (jqXHR, textStatus, errorThrown) {
                window.location = '/General/General/UploadError';
                alert('File gagal di-upload!');
            }).always(function () {
                $('#btnUploadPayment').html('Upload');
            });
        };
        reader.onerror = function (error) {
            alert('Error: ' + error);
        };
    });

});