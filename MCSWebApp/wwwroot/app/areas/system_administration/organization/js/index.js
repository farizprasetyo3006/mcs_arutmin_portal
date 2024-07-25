$(function () {

    var token = $.cookie("Token");
    var areaName = "SystemAdministration";
    var entityName = "Organization";
    var url = "/api/" + areaName + "/" + entityName;

    $("#Add").on("click", function () {
        let _url = "/" + areaName + "/" + entityName + "/Detail";
        window.location = _url;
    });

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
                dataField: "parent_organization_name",
                dataType: "text",
                caption: "Parent Organization",
                allowEditing: false
            },
            {
                dataField: "organization_name",
                dataType: "string",
                caption: "Organization Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "organization_code",
                dataType: "string",
                caption: "Organization Code",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            }
        ],
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentRecord = options.data;
                var detailEntityName = "BusinessUnit";
                var urlDetail = "/api/" + areaName + "/" + detailEntityName;

                $("<div>")
                    .addClass("master-detail-caption")
                    .text("Business Units")
                    .appendTo(container);

                $("<div>")
                    .dxTreeList({
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "id",
                            loadUrl: urlDetail + "/ByOrganizationId/" + encodeURIComponent(currentRecord.id),
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
                        keyExpr: "id",
                        parentIdExpr: "parent_business_unit_id",
                        remoteOperations: true,
                        allowColumnResizing: true,
                        columnResizingMode: "widget",
                        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                        allowColumnReordering: true,

                        columns: [
                            {
                                dataField: "organization_id",
                                allowEditing: false,
                                visible: false,
                                formItem: {
                                    colSpan: 2,
                                    visible: false
                                },
                                calculateCellValue: function () {
                                    return currentRecord.id;
                                }
                            },
                            {
                                dataField: "business_unit_name",
                                dataType: "string",
                                caption: "Business Unit Name",
                                formItem: {
                                    colSpan: 2
                                },
                                validationRules: [{
                                    type: "required",
                                    message: "This field is required."
                                }]
                            },
                            {
                                dataField: "business_unit_code",
                                dataType: "string",
                                caption: "Business Unit Code",
                                formItem: {
                                    colSpan: 2
                                },
                                validationRules: [{
                                    type: "required",
                                    message: "This field is required."
                                }]
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
                            e.data.organization_id = currentRecord.id;
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
                                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailEntityName + '.xlsx');
                                });
                            });
                            e.cancel = true;
                        }
                    })
                    .appendTo(container);
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
        height: 600,
        showBorders: true,
        editing: {
            mode: "form",
            allowAdding: false,
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
});