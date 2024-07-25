$(function () {

    var token = $.cookie("Token");
    var areaName = "Port";
    var entityName = "StatementOfFactDetail";
    var url = "/api/" + areaName + "/" + entityName;
    var $recordId = $("input[name=statement_of_fact_id]");

    $("#statement-of-fact-detail-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: url + "/DataGrid?statementOfFactId=" + $recordId.val(),
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
                dataField: "start_datetime",
                dataType: "datetime",
                caption: "Start Date",
                format: "yyyy-MM-dd HH:mm:ss"
            },
            {
                dataField: "end_datetime",
                dataType: "datetime",
                caption: "End Date",
                format: "yyyy-MM-dd HH:mm:ss"
            },
            {
                dataField: "event_category_id",
                dataType: "text",
                caption: "Code",
                validationRules: [{
                    type: "required",
                    message: "The Code is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/EventCategoryCodeIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search","text"]
                }
            },
           /* {
                dataField: "event_category_name",
                dataType: "text",
                caption: "Code Name",
                editorOptions: { readOnly: true }
            },*/
            {
                dataField: "percentage",
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 2,
                    }
                },
                caption: "Percentage",
                validationRules: [
                    {
                        type: "custom",
                        message: "The entered was out of min/max range",
                        validationCallback: function (args) {
                            if (args.value > 100) {
                                args.rule.message = "Percentage maximum is 100"
                                return false;
                            }
                            if (args.value < 0) {
                                args.rule.message = "Percentage minimum is 0"
                                return false;
                            }
                            return true;
                        }
                    }
                ]
            },
            {
                dataField: "remark",
                dataType: "text",
                caption: "Remark",
            },
        ],
        masterDetail: {
            enabled: false,
            template: function (container, options) {
                var currentRecord = options.data;
                var urlDetail = "/api/Sales/SalesOrderDetail";
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
            // Set onValueChanged for event_category_id
            /*if (e.parentType === "dataRow" && e.dataField == "event_category_id") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/General/EventCategory/DataDetail?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0]

                            // Set its corresponded field's value
                            grid.cellValue(index, "event_category_name", record.event_category_name)
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }*/
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
            useIcons: true
        },
        grouping: {
            contextMenuEnabled: true,
            autoExpandAll: false
        },
        rowAlternationEnabled: true,
        onInitNewRow: function (e) {
            e.data.sof_id = $recordId.val();
        },
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
                url: "/api/Port/StatementOfFactDetail/UploadDocumentDetail?Id=" + $recordId.val(),
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