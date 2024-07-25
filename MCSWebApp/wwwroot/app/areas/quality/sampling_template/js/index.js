$(function () {

    var token = $.cookie("Token");
    var areaName = "Quality";
    var entityName = "SamplingTemplate";
    var url = "/api/" + areaName + "/" + entityName;

    var samplingTemplateGrid = $("#grid").dxDataGrid({
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
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/SystemAdministration/BusinessUnit/BusinessUnitIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                }
            },
            {
                dataField: "sampling_template_code",
                dataType: "string",
                caption: "Sampling Template Code",
                formItem: {
                    colSpan: 2
                },
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "sampling_template_name",
                dataType: "string",
                caption: "Sampling Template Name",
                formItem: {
                    colSpan: 2
                },
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "is_active",
                dataType: "boolean",
                caption: "Is Active",
                width: "10%"
            },
            {
                dataField: "is_despatch_order_required",
                dataType: "boolean",
                caption: "Is Shipping Order Required?",
                width: "10%"
            },
            {
                dataField: "is_stock_state",
                dataType: "boolean",
                caption: "Is Stock State",
                width: "10%"
            },
        ],
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentRecord = options.data;
                var detailName = "SamplingTemplateDetail";
                var urlDetail = "/api/" + areaName + "/" + detailName;

                $("<div>")
                    .addClass("master-detail-caption")
                    .text("Analytes")
                    .appendTo(container);

                $("<div class='grid-sampling-template-detail'>")
                    .dxDataGrid({
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "id",
                            loadUrl: urlDetail + "/BySamplingTemplateId/" + encodeURIComponent(currentRecord.id),
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
                                },
                                formItem: {
                                    visible: false,
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
                            {
                                dataField: "order",
                                caption: "Order",
                                dataType: "numeric",
                                sortOrder: "asc"
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
                                            debugger;
                                            //console.log(e);
                                            if (index == 0) {
                                                alert("First data cannot be moved up")
                                                return false
                                            }
                                            

                                            let formData = new FormData();
                                            formData.append("key", e.row.data.id)
                                            formData.append("id", currentRecord.id)
                                            formData.append("type", -1)

                                            $.ajax({
                                                type: "PUT",
                                                url: urlDetail + "/UpdateOrderData",
                                                data: formData,
                                                processData: false,
                                                contentType: false,
                                                beforeSend: function (xhr) {
                                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                                },
                                                success: function (response) {
                                                    if (response) {
                                                        var samplingTemplateGrid = $("#grid").dxDataGrid("instance");
                                                        var detailRowIndex = samplingTemplateGrid.getRowIndexByKey(e.row.data.sampling_template_id) + 1
                                                        var detailGrid = samplingTemplateGrid.getRowElement(detailRowIndex).find(".dx-datagrid").first().parent().dxDataGrid("instance");
                                                        detailGrid.refresh()
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
                                            debugger;
                                            var samplingTemplateGrid = $("#grid").dxDataGrid("instance");
                                            var detailRowIndex = samplingTemplateGrid.getRowIndexByKey(e.row.data.sampling_template_id) + 1
                                            var detailGrid = samplingTemplateGrid.getRowElement(detailRowIndex).find(".dx-datagrid").first().parent().dxDataGrid("instance");

                                            let lastIndex = detailGrid.totalCount() - 1

                                            if (index == lastIndex) {
                                                alert("Last data cannot be moved down")
                                                return false
                                            }
                                            

                                            let formData = new FormData();
                                            formData.append("key", e.row.data.id)
                                            formData.append("id", currentRecord.id)
                                            formData.append("type", 1)

                                            $.ajax({
                                                type: "PUT",
                                                url: urlDetail + "/UpdateOrderData",
                                                data: formData,
                                                processData: false,
                                                contentType: false,
                                                beforeSend: function (xhr) {
                                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                                },
                                                success: function (response) {
                                                    if (response) {
                                                        var samplingTemplateGrid = $("#grid").dxDataGrid("instance");
                                                        var detailRowIndex = samplingTemplateGrid.getRowIndexByKey(e.row.data.sampling_template_id) + 1
                                                        var detailGrid = samplingTemplateGrid.getRowElement(detailRowIndex).find(".dx-datagrid").first().parent().dxDataGrid("instance");
                                                        detailGrid.refresh()
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
                                        dataField: "analyte_id",
                                    },
                                    {
                                        dataField: "uom_id",
                                    },
                                    {
                                        dataField: "order",
                                    },
                                    {
                                        dataField: "remark",
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
                            e.data.sampling_template_id = currentRecord.id;
                        },
                        onEditorPreparing: function (e) {
                            if (e.parentType === 'searchPanel') {
                                e.editorOptions.onValueChanged = function (arg) {
                                    if (arg.value.length == 0 || arg.value.length > 2) {
                                        e.component.searchByText(arg.value);
                                    }
                                }
                            }
                            
                            if (e.dataField === "analyte_id" && e.parentType == "dataRow") {
                                let standardHandler = e.editorOptions.onValueChanged
                                let index = e.row.rowIndex
                                let grid = e.component
                                let rowData = e.row.data

                                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                                    // Get its value (Id) on value changed
                                    let analyteId = e.value

                                    // Get another data from API after getting the Id
                                    $.ajax({
                                        url: '/api/Quality/Analyte/DataDetail?Id=' + analyteId,
                                        type: 'GET',
                                        contentType: "application/json",
                                        beforeSend: function (xhr) {
                                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                                        },
                                        success: function (response) {

                                            let record = response.data[0];
                                            
                                            grid.cellValue(index, "uom_id", record.uom_id)

                                           
                                        }
                                    })
                                    standardHandler(e) // Calling the standard handler to save the edited value
                                }
                            }

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
                    }).appendTo(container);
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
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            if (e.parentType === "dataRow" && e.dataField === "sampling_template_code") {
                e.editorOptions.disabled = e.row.data.sampling_template_code;
            }
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
                url: "/api/Quality/SamplingTemplate/UploadDocument",
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