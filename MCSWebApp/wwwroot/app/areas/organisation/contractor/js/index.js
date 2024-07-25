$(function () {

    var token = $.cookie("Token");
    var areaName = "Organisation";
    var entityName = "Contractor";
    var url = "/api/" + areaName + "/" + entityName;

    var contractorData = null

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
                caption: "Contractor Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "business_partner_code",
                dataType: "string",
                caption: "Contractor Code",
            },
            //{
            //    dataField: "contractor_type_id",
            //    dataType: "string",
            //    caption: "Contractor Type",
            //    visible: false,
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/General/ContractorType/ContractorTypeIdLookup", // API for Contractor Type
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
                dataField: "primary_address",
                dataType: "string",
                caption: "Primary Address",
                visible: false,
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
                visible: false,
            },
            {
                dataField: "primary_contact_email",
                dataType: "string",
                caption: "Primary Contact Email",
                visible: false,
            },
            {
                dataField: "primary_contact_phone",
                dataType: "string",
                caption: "Primary Contact Phone",
                visible: false,
            },
            {
                dataField: "bank_id",
                dataType: "text",
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
                dataType: "text",
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
                dataField: "is_equipment_owner",
                caption: "Equipment Owner",
                dataType: "boolean",
                visible: true
            },
            {
                dataField: "is_truck_owner",
                caption: "Truck Owner",
                dataType: "boolean",
                visible: true
            },
            {
                dataField: "is_barge_owner",
                caption: "Barge Owner",
                dataType: "boolean",
                visible: true
            },
            {
                dataField: "is_tug_owner",
                caption: "Tug Owner",
                dataType: "boolean",
                visible: true
            },
            {
                dataField: "is_vessel_owner",
                caption: "Vessel Owner",
                dataType: "boolean",
                visible: true
            },
            {
                dataField: "is_train_owner",
                caption: "Train Owner",
                dataType: "boolean",
                visible: false
            },
            {
                dataField: "is_surveyor",
                caption: "Surveyor",
                dataType: "boolean",
                visible: true
            },
            {
                dataField: "is_other",
                caption: "Other",
                dataType: "boolean",
                visible: false
            },
            {
                dataField: "barge_trip_plan",
                dataType: "number",
                caption: "Barge Trip Plan",
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "is_active",
                dataType: "boolean",
                caption: "Is Active",
                width: "10%"
            }
        ],
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                let currentRecord = options.data;
                contractorData = currentRecord

                // Contractor Detail Information Container
                renderContractorDetailInformation(currentRecord, container) // UI/UX Improvement

                // Contractor Another Informations Tabs
                renderContractorTabs(currentRecord, container)
            }
        },
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0) {
                selectedIds = $.map(data, function (value) {
                    return value.id;
                }).join(",");
                $("#dropdown-download-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-download-selected").addClass("disabled");
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
                            //{
                            //    dataField: "contractor_type_id"
                            //},
                            {
                                dataField: "primary_address"
                            },
                            {
                                dataField: "country_id",
                            },
                            {
                                dataField: "primary_contact_name"
                            },
                            {
                                dataField: "primary_contact_email"
                            },
                            {
                                dataField: "primary_contact_phone"
                            },
                            {
                                dataField: "barge_trip_plan"
                            },
                            {
                                dataField: "is_equipment_owner"
                            },
                            {
                                dataField: "is_truck_owner"
                            },
                            {
                                dataField: "is_barge_owner"
                            },
                            {
                                dataField: "is_tug_owner"
                            },
                            {
                                dataField: "is_vessel_owner"
                            },
                            {
                                dataField: "is_train_owner"
                            },
                            {
                                dataField: "is_surveyor"
                            },
                            {
                                dataField: "is_active"
                            },
                            {
                                dataField: "is_other"
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

    const renderContractorDetailInformation = function (currentRecord, container) {
        let contractorDetailContainer = $(`
            <div>
                <h5 class="mb-3">Contractor Detail</h5>
                <div class="row mb-5">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Primary Contact</div>
                        <div class="card">
                            <dl class="row card-body">
                                <dt class="col-md-4 mb-2">Contractor Name</dt>
                                <dd class="col-md-8">`+ (currentRecord.business_partner_name ? currentRecord.business_partner_name : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Contractor Code</dt>
                                <dd class="col-md-8">`+ (currentRecord.business_partner_code ? currentRecord.business_partner_code : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Contractor Type</dt>
                                <dd class="col-md-8">`+ (currentRecord.contractor_type_name ? currentRecord.contractor_type_name : "-") + `</dd>
                                        
                                <dt class="col-md-4 mb-2">Primary Contact Name</dt>
                                <dd class="col-md-8">`+ (currentRecord.primary_contact_name ? currentRecord.primary_contact_name : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Primary Email</dt>
                                <dd class="col-md-8">`+ (currentRecord.primary_contact_email ? currentRecord.primary_contact_email : "-") + `</dd>
                                
                                <dt class="col-md-4 mb-2">Primary Phone</dt>
                                <dd class="col-md-8">`+ (currentRecord.primary_contact_phone ? currentRecord.primary_contact_phone : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Primary Address</dt>
                                <dd class="col-md-8">`+ (currentRecord.primary_address ? currentRecord.primary_address : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Country</dt>
                                <dd class="col-md-8">`+ (currentRecord.country_name ? currentRecord.country_name : "-") + `</dd>
                            </dl>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Bank Information</div>
                        <div class="card">
                            <dl class="row card-body">
                                <dt class="col-md-4 mb-2">Bank Name</dt>
                                <dd class="col-md-8">`+ (currentRecord.bank_name ? currentRecord.bank_name : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Account Holder</dt>
                                <dd class="col-md-8">`+ (currentRecord.account_holder ? currentRecord.account_holder : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Account Number</dt>
                                <dd class="col-md-8">`+ (currentRecord.account_number ? currentRecord.account_number : "-") + `</dd>
                                
                                <dt class="col-md-4 mb-2">Currency</dt>
                                <dd class="col-md-8">`+ (currentRecord.currency_name ? currentRecord.currency_name : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Is Taxable</dt>
                                <dd class="col-md-8">`+ (currentRecord.is_taxable ? "Yes" : "No") + `</dd>
                            </dl>
                        </div>
                    </div>
                </div>
            </div>
        `).appendTo(container)
    }

    const renderContractorTabs = function (currentRecord, container) {
        let contactsContainer = $("<div>")
        let attachmentsContainer = $("<div>")

        renderContractorContact(currentRecord, contactsContainer)
        renderContractorAttachment(currentRecord, attachmentsContainer)

        let tabContainer = $(`
            <ul class="nav nav-pills pills-blue mb-3" role="tablist">
                <li class="nav-item"><a class="nav-link active" data-toggle="pill" href="#contacts-container-${currentRecord.id}"><i class="fas fa-user mr-2"></i>Contacts</a></li>
                <li class="nav-item"><a class="nav-link" data-toggle="pill" href="#attachments-container-${currentRecord.id}"><i class="fas fa-file mr-2"></i>Attachments</a></li>
            </ul>
            <div class="tab-content py-3">
                <div class="tab-pane fade show active" id="contacts-container-${currentRecord.id}" role="tabpanel">
                </div>
                <div class="tab-pane fade" id="attachments-container-${currentRecord.id}" role="tabpanel">
                </div>
            </div>
        `).appendTo(container)

        contactsContainer.appendTo(tabContainer.find("#contacts-container-" + currentRecord.id))
        attachmentsContainer.appendTo(tabContainer.find("#attachments-container-" + currentRecord.id))
    }

    const renderContractorContact = function (currentRecord, container) {
        let contactUrlDetail = "/api/Organisation/Contact";
        let contractorContactsContainer = $("<div class='mb-5'>")
        contractorContactsContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-3")
            .text("Contacts")
            .appendTo(contractorContactsContainer);

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
                        caption: "Contractor ID",
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
            }).appendTo(contractorContactsContainer);
    }

    let contractorAttachmentDataGrid
    const renderContractorAttachment = function (currentRecord, container) {
        let attachmentUrlDetail = "/api/Organisation/ContractorAttachment";
        let contractorAttachmentsContainer = $("<div class='mb-5'>")
        contractorAttachmentsContainer.appendTo(container)

        let titleContainer = $(`
            <div class="row mb-3 align-items-center">
                <div class="col-md-6">
                    <div class="master-detail-caption">File Attachments</div>
                </div>
                <div class="col-md-6 btn-container float-right">
                </div>
            </div>
        `).appendTo(contractorAttachmentsContainer)

        $("<div>")
            .dxButton({
                stylingMode: "contained",
                icon: "plus",
                text: "Add Attachments",
                type: "normal",
                width: "'auto'",
                onClick: function () {
                    openAddAttachmentPopup()
                },
                elementAttr: {
                    class: "float-right"
                }
            }).appendTo(titleContainer.find(".btn-container")[0])

        contractorAttachmentDataGrid = $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: attachmentUrlDetail + "/ByContractorId/" + encodeURIComponent(currentRecord.id),
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
                        dataField: "contractor_id",
                        dataType: "string",
                        caption: "Contractor ID",
                        formItem: {
                            colSpan: 2
                        },
                        allowEditing: false,
                        visible: false,
                        calculateCellValue: function () {
                            return currentRecord.id;
                        }
                    },
                    {
                        dataField: "file_name",
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
                                xhr.open("GET", "/api/Organisation/ContractorAttachment/DownloadDocument/" + attachment.id, true)
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
            }).appendTo(contractorAttachmentsContainer).dxDataGrid("instance");
    }
    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;
            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');
            $.ajax({
                url: "/Organisation/Contractor/ExcelExport",
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
                    a.download = "Contractor.xlsx"; // Set the appropriate file name here
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

    const addAttachmentPopupOptions = {
        width: 500,
        height: "auto",
        showTitle: true,
        title: "Add Attachment",
        visible: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        contentTemplate: function () {
            let contractorIdInput =
                $("<div>")
                    .dxTextBox({
                        name: "contractor_id",
                        value: contractorData.id,
                        readOnly: true,
                        visible: false
                    })

            let attachmentInput =
                $("<div class='mb-5 dx-fileuploader-mcs'>")
                    .dxFileUploader({
                        uploadMode: "useForm",
                        multiple: false
                    })

            let submitButton =
                $("<div>")
                    .dxButton({
                        text: "Submit",
                        onClick: function (e) {
                            let contractorId = contractorIdInput.dxTextBox("instance").option("value")
                            let file = attachmentInput.dxFileUploader("instance").option("value")[0]

                            var reader = new FileReader();
                            reader.readAsDataURL(file);
                            reader.onload = function () {
                                let fileName = file.name
                                let fileSize = file.size
                                let data = reader.result.split(',')[1]

                                let formData = {
                                    "contractorId": contractorId,
                                    "fileName": fileName,
                                    "fileSize": fileSize,
                                    "data": data
                                }

                                $.ajax({
                                    url: "/api/Organisation/ContractorAttachment/InsertData",
                                    data: JSON.stringify(formData),
                                    type: "POST",
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        addAttachmentPopup.hide()
                                        contractorAttachmentDataGrid.refresh()
                                    }
                                })
                            }
                        }
                    })

            let formContainer = $("<form enctype='multipart/form-data'>")
                .append(contractorIdInput, attachmentInput, submitButton)

            return formContainer;
        }
    }

    const addAttachmentPopup = $("<div>")
        .dxPopup(addAttachmentPopupOptions).appendTo("body").dxPopup("instance")

    const openAddAttachmentPopup = function () {
        addAttachmentPopup.option("contentTemplate", addAttachmentPopupOptions.contentTemplate.bind(this));
        addAttachmentPopup.show()
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
                url: "/api/Organisation/Contractor/UploadDocument",
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