$(function () {
    var token = $.cookie("Token");
    var $recordId = document.querySelector("[name=advance_contract_item_id]").value
    var $eventDefinitionCategoryId = "";
    var areaName = "ContractManagement";
    var entityName = "AdvanceContractItemDetail";
    var gridUrl = "/api/" + areaName + "/" + entityName;


    $("#dt-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: gridUrl + "/DataGrid?recordId=" + encodeURIComponent($recordId),
            insertUrl: gridUrl + "/InsertData",
            updateUrl: gridUrl + "/UpdateData",
            deleteUrl: gridUrl + "/DeleteData",
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
                dataField: "variable",
                dataType: "string",
                caption: "Variable",
            },
            {
                dataField: "amount",
                editorType: "dxNumberBox",
                caption: "Value",
                editorOptions: {
                    format: "fixedPoint",
                    precision: 3,
                    step: 0
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 3
                        }
                    }
                },
                validationRules: [{
                    type: "required",
                    message: "Value is required."
                }],
            },
            //{
            //    dataField: "currency_id",
            //    dataType: "text",
            //    caption: "Currency",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/General/Currency/CurrencyIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //},
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
            // Set onValueChanged 
            if (e.dataField === "event_category_id" && e.parentType === "dataRow") {
                e.editorOptions.disabled = !e.row.data.event_category_id;
            }
            if (e.parentType === "dataRow" && e.dataField == "event_definition_category_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value
                    // Get another data from API after getting the Id

                    $.ajax({
                        url: '/api/General/EventDefinitionCategory/GetDetailById/' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];
                            $eventDefinitionCategoryId = record.id;
                            grid.cellValue(index, "event_definition_category_id", record.event_definition_category_name);
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "event_category_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    let recordId = e.value
                    $.ajax({
                        url: '/api/General/EventCategory/Detail/' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response;
                            grid.cellValue(index, "event_category_code", record.event_category_code);
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
        },
        onInitNewRow: function (e) {
            e.data.advance_contract_item_id = $recordId;
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
                        dataField: "variable",
                        colSpan: 2
                    },
                    {
                        dataField: "amount",
                    },
                    //{
                    //    dataField: "currency_id",
                    //},
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

});


