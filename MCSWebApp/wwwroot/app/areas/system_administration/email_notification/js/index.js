$(function () {

    var token = $.cookie("Token");
    var areaName = "SystemAdministration";
    var entityName = "EmailNotification";
    var url = "/api/" + areaName + "/" + entityName;

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
                dataField: "email_subject",
                dataType: "string",
                caption: "Email Subject",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "recipients",
                dataType: "string",
                caption: "Email Recipients",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "delivery_schedule",
                dataType: "datetime",
                caption: "Delivery Schedule",
                format: "yyyy-MM-dd HH:mm:ss",
                validationRules: [{
                    type: "required",
                    message: "The Delivery Schedule field is required."
                }]
            },
            {
                dataField: "delivered_on",
                dataType: "datetime",
                caption: "Delivered On",
                format: "yyyy-MM-dd HH:mm:ss"
            },
            {
                dataField: "attempt_count",
                dataType: "string",
                caption: "Attempt",
                width: 90
            },
            {
                dataField: "table_name",
                dataType: "string",
                caption: "Table/View Name",
                validationRules: [{
                    type: "required",
                    message: "The Table/View Name field is required."
                }],
                visible: false
            },
            {
                dataField: "fields",
                dataType: "string",
                caption: "Fields Name",
                validationRules: [{
                    type: "required",
                    message: "The Fields Name field is required."
                }],
                visible: false
            },
            {
                dataField: "criteria",
                dataType: "string",
                caption: "Criteria",
                validationRules: [{
                    type: "required",
                    message: "The Criteria field is required."
                }],
                visible: false
            },
            {
                dataField: "email_content",
                dataType: "string",
                caption: "Email Content",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                visible: false
            },
            {
                dataField: "email_code",
                dataType: "string",
                caption: "Email Code",
                allowEditing: false,
                formItem: {
                    visible: false
                },
                visible: false
            },
            {
                dataField: "attachment_file",
                dataType: "string",
                caption: "Attachment",
                allowEditing: false,
                formItem: {
                    visible: false
                }
            },
            {
                type: "buttons",
                width: 110,
                buttons: [
                    //{
                    //    hint: "Send",
                    //    icon: "email",
                    //    visible: true,
                    //    onClick: function (e) {
                    //        window.location = "/" + areaName + "/" + entityName + "/Detail/" + encodeURIComponent(e.row.data.id);
                    //        e.event.preventDefault();
                    //    }
                    //},
                    {
                        hint: "Edit",
                        icon: "edit",
                        visible: true,
                        onClick: function (e) {
                            window.location = "/" + areaName + "/" + entityName + "/Detail/" + encodeURIComponent(e.row.data.id);
                            e.event.preventDefault();
                        }
                    },
                    "delete"
                ]
            }
        ],
        onToolbarPreparing: function (e) {
            let toolbarItems = e.toolbarOptions.items;
            toolbarItems.forEach(function (item) {
                if (item.name === "addRowButton") {
                    item.options = {
                        icon: "plus",
                        onClick: function (e) {
                            window.location = "/" + areaName + "/" + entityName + "/Detail"
                        }
                    }
                }
            });
        },

        masterDetail: {
            enabled: false,
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
        height: 600,
        showBorders: true,
        editing: {
            mode: "form", // 'batch' | 'cell' | 'form' | 'popup' | 'row'
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
        onEditorPreparing(e) {
            if (e.dataField == "email_content" || e.dataField == "recipients")
                e.editorName = "dxTextArea";
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

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Email Recipients",
                    template: createEmailRecipientsTab(masterDetailOptions.data)
                }
            ]
        });
    }

    function createEmailRecipientsTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "EmailRecipient";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            bargingTransactionData = currentRecord

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/EmailRecipientById/" + encodeURIComponent(currentRecord.id),
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
                            dataField: "email_address",
                            dataType: "text",
                            caption: "Email Address",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }]
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
                        e.data.email_notification_id = currentRecord.id;
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

});