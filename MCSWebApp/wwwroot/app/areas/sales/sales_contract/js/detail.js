$(function () {

    var token = $.cookie("Token");
    var entityName = "ContractTerms";
    var salesContractId = document.querySelector("[name=sales_contract_id]").value;
    var salesContractTermName = document.querySelector("[name=sales_contract_term_name]").value;
    var salesContractStartDate = document.querySelector("[name=sales_contract_start_date]").value;
    var salesContractEndDate = document.querySelector("[name=sales_contract_end_date]").value;

    /**
     * =========================
     * End User Grid
     * =========================
     */

    let salesContractEndUser = "/api/Sales/SalesContractEndUser";
    let salesContractEndUserGrid = $("#end-user-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: salesContractEndUser + "/DataGrid?salesContractId=" + salesContractId,
            insertUrl: salesContractEndUser + "/InsertData",
            updateUrl: salesContractEndUser + "/UpdateData",
            deleteUrl: salesContractEndUser + "/DeleteData",
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
                caption: "End User Name",
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
                setCellValue: function (rowData, value) {
                    rowData.customer_id = value
                }
            },
            {
                type: "buttons",
                dataField: "",
                caption: "",
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
                        dataField: "customer_id",
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
            e.data.sales_contract_id = salesContractId;
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
    }).dxDataGrid("instance");
    //* ===== End User


   /**
     * =========================
     * Invoice Target Grid
     * =========================
     */

    let salesContractInvoiceTarget = "/api/Sales/SalesContractInvoiceTarget";
    let salesContractInvoiceTargetGrid = $("#invoice-target-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: salesContractInvoiceTarget + "/DataGrid?salesContractId=" + salesContractId,
            insertUrl: salesContractInvoiceTarget + "/InsertData",
            updateUrl: salesContractInvoiceTarget + "/UpdateData",
            deleteUrl: salesContractInvoiceTarget + "/DeleteData",
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
                dataField: "invoice_target_id",
                dataType: "string",
                caption: "Invoice Target Name",
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
                }
            },
            {
                dataField: "",
                caption: "",
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
                        dataField: "invoice_target_id",
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
            e.data.sales_contract_id = salesContractId;
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
    }).dxDataGrid("instance");
     //* ===== Invoice Target

    /**
     * =========================
     * Sales Contract Terms Grid
     * =========================
     */

    let salesContractTermUrl = "/api/Sales/SalesContractTerm";
    let salesContractTermGrid = $("#contract-term-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            /*loadUrl: urlDetail + "/BySalesOrderId/" + encodeURIComponent(currentRecord.id),*/
            loadUrl: salesContractTermUrl + "/DataGrid?salesContractId=" + salesContractId,
            insertUrl: salesContractTermUrl + "/InsertData",
            updateUrl: salesContractTermUrl + "/UpdateData",
            deleteUrl: salesContractTermUrl + "/DeleteData",
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
                dataField: "sales_contract_id",
                caption: "Sales Contract",
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
                dataField: "contract_term_name",
                caption: "Contract Term Name",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Start Date",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "End Date",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "currency_id",
                dataType: "string",
                caption: "Currency",
                //visible: false,
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

            {
                dataField: "incoterm_id",
                dataType: "string",
                caption: "Incoterm",
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
                /*customizeText: function (cellInfo) {
                    return numeral(cellInfo.value).format('0,0.00');
                }*/
            },
            {
                dataField: "uom_id",
                dataType: "string",
                caption: "Unit",
                //visible: false,
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
                dataField: "decimal_places",
                dataType: "number",
                caption: "Decimal Places",
            },
            {
                dataField: "rounding_type_id",
                dataType: "string",
                caption: "Rounding Type",
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
                dataField: "notes",
                dataType: "string",
                caption: "Notes",
                visible: false
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "asc"
            },
            {
                caption: "Detail",
                type: "buttons",
                width: 150,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "See Terms Detail",
                    text: "Open Detail",
                    onClick: function (e) {
                        contractTermId = e.row.data.id
                        window.location = "/Sales/SalesContractTerm/Detail/" + contractTermId
                    }
                }]
            },
            {
                dataField: "",
                caption: "",
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
                        dataField: "contract_term_name",
                        colSpan: 2
                    },
                    {
                        dataField: "start_date",
                    },
                    {
                        dataField: "end_date",
                    },
                    {
                        dataField: "incoterm_id",
                        colSpan: 2
                    },
                    {
                        dataField: "currency_id",
                        colSpan: 2
                    },
                    {
                        dataField: "quantity",
                    },
                    {
                        dataField: "uom_id",
                    },
                    {
                        dataField: "decimal_places",
                    },
                    {
                        dataField: "rounding_type_id",
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
            e.data.sales_contract_id = salesContractId;
            e.data.contract_term_name = salesContractTermName;
            e.data.start_date = salesContractStartDate;
            e.data.end_date = salesContractEndDate;
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
    }).dxDataGrid("instance");


    /**
     * =========================
     * Attachments Grid
     * =========================
     */

    const maxFileSize = 52428800;

    let salesContractAttachmentUrl = "/api/Sales/SalesContractAttachment";
    let salesContractAttachmentGrid = $("#attachment-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: salesContractAttachmentUrl + "/BySalesContractId/" + salesContractId,
            insertUrl: salesContractAttachmentUrl + "/InsertData",
            updateUrl: salesContractAttachmentUrl + "/UpdateData",
            deleteUrl: salesContractAttachmentUrl + "/DeleteData",
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
                dataField: "sales_contract_id",
                dataType: "string",
                caption: "Sales Contract",
                formItem: {
                    colSpan: 2
                },
                allowEditing: false,
                visible: false,
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
                        xhr.open("GET", "/api/Sales/SalesContractAttachment/DownloadDocument/" + attachment.id, true)
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
                dataField: "deletebutton",
                caption: "",
                type: "buttons",
                buttons: ["delete"]
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
        height: 800,
        showBorders: true,
        editing: {
            mode: "form",
            allowAdding: true,
            allowUpdating: false,
            allowDeleting: true,
            useIcons: true,
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
            e.data.sales_contract_id = salesContractId;
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
    }).dxDataGrid("instance");

    const addAttachmentPopupOptions = {
        width: 500,
        height: "auto",
        showTitle: true,
        title: "Add Attachment",
        visible: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        contentTemplate: function () {
            let salesContractIdInput =
                $("<div>")
                    .dxTextBox({
                        name: "sales_contract_id",
                        value: salesContractId,
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
                            let salesContractId = salesContractIdInput.dxTextBox("instance").option("value")
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
                                    "salesContractId": salesContractId,
                                    "fileName": fileName,
                                    "fileSize": fileSize,
                                    "data": data
                                }

                                $.ajax({
                                    url: "/api/Sales/SalesContractAttachment/InsertData",
                                    data: JSON.stringify(formData),
                                    type: "POST",
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        addAttachmentPopup.hide()
                                        salesContractAttachmentGrid.refresh()
                                    }
                                })
                            }
                        }
                    })

            let formContainer = $("<form enctype='multipart/form-data'>")
                .append(salesContractIdInput, attachmentInput, submitButton)

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