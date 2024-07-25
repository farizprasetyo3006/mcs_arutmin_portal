$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "ProductionPlan";
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
                dataField: "production_plan_number",
                dataType: "string",
                caption: "Production Plan Number",
                width: "160px",
                formItem: {
                    colSpan: 2
                },
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Start Date"
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "End Date"
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
                width: "150px"
            },
            {
                dataField: "previous_quantity",
                dataType: "number",
                format: "fixedPoint",
                caption: "Prev. Qty",
                allowEditing: false,
                width: "150px"
            },
            {
                dataField: "uom_id",
                dataType: "text",
                caption: "Unit",
                formItem: {
                    colSpan: 2
                },
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
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "budget_amount",
                dataType: "number",
                format: "fixedPoint",
                caption: "Budget",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
            },
            {
                dataField: "budget_currency_id",
                dataType: "text",
                caption: "Currency",
                formItem: {
                    colSpan: 2
                },
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
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            }
        ],

        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentRecord = options.data;
                var detailName = "ProductionPlanDetail";
                var urlDetail = "/api/" + areaName + "/" + detailName;

                $("<div>")
                    .addClass("master-detail-caption")
                    .text("Details")
                    .appendTo(container);

                $("<div>")
                    .dxDataGrid({
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "id",
                            loadUrl: urlDetail + "/ByProductionPlanId/" + encodeURIComponent(currentRecord.id),
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
                                dataField: "production_plan_id",
                                allowEditing: false,
                                visible: false,
                                formItem: {
                                    colSpan: 2
                                },
                                calculateCellValue: function () {
                                    return currentRecord.id;
                                }
                            },
                            {
                                dataField: "month_index",
                                dataType: "number",
                                caption: "Month",
                                sortOrder: "asc"
                            },
                            {
                                dataField: "quantity",
                                dataType: "number",
                                caption: "Quantity"
                            },
                            {
                                dataField: "previous_quantity",
                                dataType: "number",
                                caption: "Prev. Qty",
                                allowEditing: false
                            }

                            //{
                            //    dataField: "plan_month",
                            //    dataType: "text",
                            //    caption: "Month",
                            //    allowEditing: false
                            //    groupIndex: 0
                            //},
                            //{
                            //    dataField: "plan_date",
                            //    dataType: "date",
                            //    caption: "Plan Date",
                            //    sortIndex: 0,
                            //    sortOrder: "asc",
                            //    validationRules: [{
                            //        type: "required",
                            //        message: "The field is required."
                            //    }]
                            //},
                        ],

                        masterDetail: {
                            enabled: true,
                            template: function (container, options) {
                                var currentRecord = options.data;
                                var detailName = "ProductionPlanDetail";
                                var urlDetail = "/api/" + areaName + "/" + detailName;

                                $("<div>")
                                    .addClass("master-detail-caption")
                                    .text("History")
                                    .appendTo(container);

                                $("<div>")
                                    .dxDataGrid({
                                        dataSource: DevExpress.data.AspNet.createStore({
                                            key: "id",
                                            loadUrl: urlDetail + "/ByProductionPlanIdMonth/" + encodeURIComponent(currentRecord.production_plan_id) + "/" +
                                                encodeURIComponent(currentRecord.month_index),
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
                                                dataField: "production_plan_id",
                                                allowEditing: false,
                                                visible: false,
                                                formItem: {
                                                    colSpan: 2
                                                },
                                                calculateCellValue: function () {
                                                    return currentRecord.id;
                                                }
                                            },
                                            {
                                                dataField: "created_on",
                                                dataType: "datetime",
                                                caption: "Changed Date",
                                                sortOrder: "desc"
                                            },
                                            {
                                                dataField: "quantity",
                                                dataType: "number",
                                                caption: "Quantity",
                                            },
                                            {
                                                dataField: "previous_quantity",
                                                dataType: "number",
                                                caption: "Prev. Qty",
                                                allowEditing: false
                                            }
                                        ],
                                        onSaved: function () {
                                            location.reload();
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
                                            enabled: false,
                                            allowExportSelectedData: false
                                        },
                                        onInitNewRow: function (e) {
                                            e.data.production_plan_id = currentRecord.id;
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
                            },
                        },

                        //summary: {
                        //    groupItems: [
                        //        {
                        //            column: "quantity",
                        //            summaryType: "sum"
                        //        },
                        //        {
                        //            column: "previous_quantity",
                        //            summaryType: "sum"
                        //        }
                        //    ]
                        //},

                        onSaved: function () {
                            location.reload();
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
                        showBorders: true,
                        editing: {
                            mode: "form",
                            allowAdding: false,
                            allowUpdating: true,
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
                            e.data.production_plan_id = currentRecord.id;
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
            },
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
                url: "/api/Planning/ProductionPlan/UploadDocument",
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