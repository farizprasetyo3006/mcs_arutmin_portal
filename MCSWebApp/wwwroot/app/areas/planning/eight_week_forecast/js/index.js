$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "EightWeekForecast";
    var url = "/api/" + areaName + "/" + entityName;
    var eightweekforecastitemdata = null;

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
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                formItem: {
                    colSpan: 2,
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/BusinessUnitIdLookup",
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
                dataField: "planning_number",
                dataType: "string",
                caption: "Planning Number",
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }]
            },
            {
                dataField: "version",
                dataType: "string",
                caption: "Version",
                formItem: {
                    colSpan: 2,
                },
                allowEditing: false
            },
            {
                dataField: "year_id",
                dataType: "string",
                caption: "Year",
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
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
                            filter: ["item_group", "=", "years"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                sortOrder: "asc"
            },
            {
                dataField: "total_quantity",
                dataType: "number",
                caption: "Total",
                formItem: {
                    colSpan: 2,
                },
                allowEditing: false
            }
            //{
            //    dataField: "uom_id",
            //    dataType: "string",
            //    caption: "Unit",
            //    formItem: {
            //        colSpan: 2,
            //    },
            //    validationRules: [{
            //        type: "required",
            //        message: "The field is required."
            //    }],
            //    lookup: {
            //        dataSource: function (options) {
            //            return {
            //                store: DevExpress.data.AspNet.createStore({
            //                    key: "value",
            //                    loadUrl: url + "/UomIdLookup",
            //                    onBeforeSend: function (method, ajaxOptions) {
            //                        ajaxOptions.xhrFields = { withCredentials: true };
            //                        ajaxOptions.beforeSend = function (request) {
            //                            request.setRequestHeader("Authorization", "Bearer " + token);
            //                        };
            //                    }
            //                })
            //            }
            //        },
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    sortOrder: "asc"
            //}
        ],
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentRecord = options.data;
                renderEightWeekForecast(currentRecord, container)
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
        },
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0) {
                selectedIds = $.map(data, function (value) {
                    return value.id;
                }).join(",");
                $("#dropdown-delete-selected").removeClass("disabled");
                $("#dropdown-download-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-delete-selected").addClass("disabled");
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
            mode: "popup",
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

    const renderEightWeekForecast = function (currentRecord, container) {
        var detailName = "EightWeekForecastItem";
        var urlDetail = "/api/" + areaName + "/" + detailName;

        let salesPlanDetailsContainer = $("<div class='mb-5'>")
        salesPlanDetailsContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-2")
            .text("Item Entry/View")
            .appendTo(salesPlanDetailsContainer);

        $("<div id='monthly-grid'>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: urlDetail + "/DataGrid?headerId=" + encodeURIComponent(currentRecord.id),
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
                        dataField: "activity_plan_id",
                        dataType: "string",
                        caption: "Activity Plan",
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
                                    filter: ["item_group", "=", "activity-plan-type"]
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        }
                    },
                    {
                        dataField: "location_id",
                        dataType: "string",
                        caption: "Location",
                        lookup: {
                            dataSource: function (options) {
                                return {
                                    store: DevExpress.data.AspNet.createStore({
                                        key: "value",
                                        loadUrl: urlDetail + "/LocationIdLookup",
                                        onBeforeSend: function (method, ajaxOptions) {
                                            ajaxOptions.xhrFields = { withCredentials: true };
                                            ajaxOptions.beforeSend = function (request) {
                                                request.setRequestHeader("Authorization", "Bearer " + token);
                                            };
                                        }
                                    })
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        }
                    },
                    {
                        dataField: "business_area_pit_id",
                        dataType: "string",
                        caption: "Pit",
                        lookup: {
                            dataSource: function (options) {
                                return {
                                    store: DevExpress.data.AspNet.createStore({
                                        key: "value",
                                        loadUrl: urlDetail + "/PitIdLookup",
                                        onBeforeSend: function (method, ajaxOptions) {
                                            ajaxOptions.xhrFields = { withCredentials: true };
                                            ajaxOptions.beforeSend = function (request) {
                                                request.setRequestHeader("Authorization", "Bearer " + token);
                                            };
                                        }
                                    })
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        }
                    },
                    {
                        dataField: "product_category_id",
                        dataType: "string",
                        caption: "Product Category",
                        lookup: {
                            dataSource: function (options) {
                                return {
                                    store: DevExpress.data.AspNet.createStore({
                                        key: "value",
                                        loadUrl: urlDetail + "/ProductCategoryIdLookup",
                                        onBeforeSend: function (method, ajaxOptions) {
                                            ajaxOptions.xhrFields = { withCredentials: true };
                                            ajaxOptions.beforeSend = function (request) {
                                                request.setRequestHeader("Authorization", "Bearer " + token);
                                            };
                                        }
                                    })
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        }
                    },
                    //{
                    //    dataField: "product_id",
                    //    dataType: "string",
                    //    caption: "Product",
                    //    lookup: {
                    //        dataSource: function (options) {
                    //            return {
                    //                store: DevExpress.data.AspNet.createStore({
                    //                    key: "value",
                    //                    loadUrl: urlDetail + "/ProductIdLookup",
                    //                    onBeforeSend: function (method, ajaxOptions) {
                    //                        ajaxOptions.xhrFields = { withCredentials: true };
                    //                        ajaxOptions.beforeSend = function (request) {
                    //                            request.setRequestHeader("Authorization", "Bearer " + token);
                    //                        };
                    //                    }
                    //                })
                    //            }
                    //        },
                    //        valueExpr: "value",
                    //        displayExpr: "text"
                    //    }
                    //},
                    {
                        dataField: "contractor_id",
                        dataType: "string",
                        caption: "Contractor",
                        lookup: {
                            dataSource: function (options) {
                                return {
                                    store: DevExpress.data.AspNet.createStore({
                                        key: "value",
                                        loadUrl: urlDetail + "/ContractorIdLookup",
                                        onBeforeSend: function (method, ajaxOptions) {
                                            ajaxOptions.xhrFields = { withCredentials: true };
                                            ajaxOptions.beforeSend = function (request) {
                                                request.setRequestHeader("Authorization", "Bearer " + token);
                                            };
                                        }
                                    })
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        }
                    },
                    {
                        dataField: "total_quantity",
                        dataType: "number",
                        caption: "Total",
                        allowEditing: false
                    },
                    {
                        dataField: "uom_id",
                        dataType: "string",
                        caption: "Unit",
                        formItem: {
                            colSpan: 2,
                        },
                        validationRules: [{
                            type: "required",
                            message: "The field is required."
                        }],
                        lookup: {
                            dataSource: function (options) {
                                return {
                                    store: DevExpress.data.AspNet.createStore({
                                        key: "value",
                                        loadUrl: url + "/UomIdLookup",
                                        onBeforeSend: function (method, ajaxOptions) {
                                            ajaxOptions.xhrFields = { withCredentials: true };
                                            ajaxOptions.beforeSend = function (request) {
                                                request.setRequestHeader("Authorization", "Bearer " + token);
                                            };
                                        }
                                    })
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        },
                        sortOrder: "asc"
                    },
                    {
                        dataField: "sum",
                        dataType: "number",
                        caption: "Sum",
                        allowEditing: false
                    },
                    {
                        caption: "Detail",
                        type: "buttons",
                        width: 200,
                        buttons: [{
                            cssClass: "btn-dxdatagrid",
                            hint: "Weekly Details",
                            text: "Weekly Details",
                            onClick: function (e) {
                                eightweekforecastitemdata = e.row.data;
                                showItemDetails(eightweekforecastitemdata);
                            }
                        }]
                    },
                    {
                        type: "buttons",
                        caption: "Action",
                        buttons: ["delete"],
                        showInColumnChooser: true
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
                    pageSize: 20
                },
                pager: {
                    allowedPageSizes: [20, 20, 50, 100],
                    showNavigationButtons: true,
                    showPageSizeSelector: true,
                    showInfo: true,
                    visible: true
                },
                showBorders: true,
                editing: {
                    mode: "batch",
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
                    e.data.header_id = currentRecord.id;
                },
                onRowUpdated: function (e) {
                    $("#grid").dxDataGrid("getDataSource").reload();
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
            }).appendTo(salesPlanDetailsContainer);
    }

    let popupOptions = {
        title: "Details Entry/View",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function () {
            let container = $("<div>")

            $(`<div class="mb-3">
                    <div class="row">
                        <div class="col-md-3">
                            <small class="font-weight-normal">Planning Number</small>
                            <h6 class="font-weight-bold">`+ eightweekforecastitemdata.planning_number + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Version</small>
                            <h6 class="font-weight-bold">`+ eightweekforecastitemdata.version + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Year</small>
                            <h6 class="font-weight-bold">`+ eightweekforecastitemdata.item_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Location</small>
                            <h6 class="font-weight-bold">`+ eightweekforecastitemdata.location_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Pit</small>
                            <h6 class="font-weight-bold">`+ eightweekforecastitemdata.pit_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Total</small>
                            <h6 class="font-weight-bold" id="week-quantity">`+ formatNumber(eightweekforecastitemdata.total_quantity) + `</h6>
                        </div>
                    </div>
                </div>
            `).appendTo(container)

            let url = "/api/Planning/EightWeekForecastItemDetail";
            var detailName = "EightWeekForecastItem";
            var urlDetail = "/api/" + areaName + "/" + detailName;

            $("<div id='week-grid'>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/DataGrid?item_id=" + eightweekforecastitemdata.id,
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
                            dataField: "week_id",
                            dataType: "string",
                            caption: "Week"
                        },
                        {
                            dataField: "from_date",
                            dataType: "date",
                            caption: "From Date"
                        },
                        {
                            dataField: "to_date",
                            dataType: "date",
                            caption: "To Date"
                        },
                        {
                            dataField: "product_id",
                            dataType: "string",
                            caption: "Product",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: urlDetail + "/ProductIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        })
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            }
                        },
                        {
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity"
                        },
                        {
                            dataField: "ash_adb",
                            dataType: "number",
                            caption: "Ash (adb)"
                        },
                        {
                            dataField: "ts_adb",
                            dataType: "number",
                            caption: "TS (adb)"
                        },
                        {
                            dataField: "im_adb",
                            dataType: "number",
                            caption: "IM (adb)"
                        },
                        {
                            dataField: "tm_arb",
                            dataType: "number",
                            caption: "TM (ARB)"
                        },
                        {
                            dataField: "gcv_gad",
                            dataType: "number",
                            caption: "CV gad"
                        },
                        {
                            dataField: "gcv_gar",
                            dataType: "number",
                            caption: "CV gar"
                        },
                        {
                            dataField: "is_using",
                            dataType: "boolean",
                            caption: "Is Using"
                        },
                        {
                            caption: "Duplicate",
                            type: "buttons",
                            buttons: [{
                                cssClass: "btn-dxdatagrid",
                                text: "Duplicate",
                                onClick: function (e) {
                                    eightWeekData = e.row.data;
                                    let formData = new FormData();
                                    formData.append("key", eightWeekData.id);
                                    saveApprovalForm(formData);
                                }
                            },]
                        },
                        {
                            type: "buttons",
                            caption: "Action",
                            buttons: ["delete"],
                            showInColumnChooser: true
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
                        pageSize: 40
                    },
                    height: 500,
                    pager: {
                        allowedPageSizes: [40, 80, 100],
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        showInfo: true,
                        visible: true
                    },
                    showBorders: true,
                    editing: {
                        mode: "batch",
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
                        e.data.item_id = eightweekforecastitemdata.id;
                        e.data.header_id = eightweekforecastitemdata.header_id;
                    },
                    onRowUpdated: function (e) {
                        //$("#grid").dxDataGrid("refresh");
                        //$("<div>").dxDataGrid("refresh");
                        $.ajax({
                            url: "/api/Planning/EightWeekForecastItem/DataDetail?id=" + encodeURIComponent(eightweekforecastitemdata.id),
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (r) {
                                if (r.data.length > 0) {
                                    let data = r.data[0];
                                    $("#week-quantity").html(formatNumber(data.total));
                                    $("#monthly-grid").dxDataGrid("getDataSource").reload();
                                    $("#grid").dxDataGrid("getDataSource").reload();
                                    //$("#grid").dxDataGrid("refresh");
                                }
                            }
                        })
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
                }).appendTo(container);

            return container;
        }
    }

    let approvalPopupOptions = {
        title: "Split Quantity",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function () {

            var approvalForm = $("<div>").dxForm({
                formData: {
                    id: "",
                    comment: "",
                },
                colCount: 2,
                //readOnly: true,
                items: [
                    {
                        dataField: "id",
                        visible: false,
                    },
                    {
                        dataField: "number",
                        dataType: "number",
                        label: {
                            text: "Split To ",
                        },
                        colSpan: 2,
                        editorOptions: {
                            placeholder: "Select a number from 2 to 60",
                            onValueChanged: function (e) {
                                if (isNaN(e.value) || e.value < 2 || e.value > 20) {
                                    e.component.option("isValid", false);
                                    e.component.option("validationError", {
                                        message: "Please enter a number between 1 and 20.",
                                    });
                                } else {
                                    e.component.option("isValid", true);
                                }
                            },
                        },
                    },
                    {
                        itemType: "button",
                        colSpan: 2,
                        horizontalAlignment: "right",
                        buttonOptions: {
                            text: "Save",
                            type: "secondary",
                            useSubmitBehavior: true,
                            onClick: function () {
                                let data = approvalForm.dxForm("instance").option("formData");
                                let formData = new FormData();
                                formData.append("key", data.id);
                                saveApprovalForm(formData);
                            }
                        }
                    },

                ]
            })

            return approvalForm;
        }
    }

    var approvalPopup = $("#split-popup").dxPopup(approvalPopupOptions).dxPopup("instance")

    const showApprovalPopup = function () {
        approvalPopup.option("contentTemplate", approvalPopupOptions.contentTemplate.bind(this));
        approvalPopup.show()
    }

    const saveApprovalForm = (formData) => {
        $.ajax({
            type: "POST",
            url: "/api/Planning/EightWeekForecastItemDetail/DuplicateRow",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    // Show successfuly saved popup
                    let successPopup = $("<div>").dxPopup({
                        width: 300,
                        height: "auto",
                        dragEnabled: false,
                        hideOnOutsideClick: true,
                        showTitle: true,
                        title: "Success",
                        contentTemplate: function () {
                            return $(`<p class="text-center">Data saved.</p>`)
                        }
                    }).appendTo("body").dxPopup("instance");
                    approvalPopup.hide();
                    successPopup.show();
                    $.ajax({
                        url: "/api/Planning/EightWeekForecastItem/DataDetail?id=" + encodeURIComponent(eightweekforecastitemdata.id),
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (r) {
                            if (r.data.length > 0) {
                                let data = r.data[0];
                                $("#week-quantity").html(formatNumber(data.total));
                                $("#week-grid").dxDataGrid("getDataSource").reload();
                                $("#monthly-grid").dxDataGrid("getDataSource").reload();
                                $("#grid").dxDataGrid("getDataSource").reload();
                            }
                        }
                    })
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            approvalPopup.hide();
            Swal.fire("Failed !", jqXHR.responseText, "error");
        });
    }


    var popup = $("#popup2").dxPopup(popupOptions).dxPopup("instance")

    const showItemDetails = function (myData) {
        if (myData.quantity === 0) {
            alert("Quantity is empty. Please edit quantity");
            return;
        }
        popup.option("contentTemplate", popupOptions.contentTemplate.bind(this));
        popup.show()
    }



    //History
    let popupMonthlyHistoryOptions = {
        title: "Monthly History",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function () {
            ////console.log("Monthly History", productionPlanMonthlyHistoryData);
            //<div class="col-md-3">
            //    <small class="font-weight-normal">Production Plan Number</small>
            //    <h3 class="font-weight-bold">`+ eightweekforecastitemdata.hauling_plan_number +`</h6>
            //</div>
            let container = $("<div>")

            $(`<div class="mb-3">
                    <div class="row">

                        <div class="col-md-2">
                            <small class="font-weight-normal">Month</small>
                            <h3 class="font-weight-bold">`+ productionPlanMonthlyHistoryData.month_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Quantity</small>
                            <h3 class="font-weight-bold">`+ formatNumber(productionPlanMonthlyHistoryData.quantity) + `</h6>
                        </div>
                    </div>
                </div>
            `).appendTo(container)


            var detailName = "ProductionPlanMonthly";
            var urlDetail = "/api/" + areaName + "/" + detailName;

            $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/History/ByProductionPlanMonthlyId/" + productionPlanMonthlyHistoryData.id,
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
                    columnResizingMode: "widget",
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        {
                            dataField: "month_id",
                            dataType: "number",
                            caption: "Month",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }],
                            formItem: {
                                visible: false
                            },
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/MonthIndexLookup",
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
                        },
                        {
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Last Quantity",
                            format: "fixedPoint",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            },
                            customizeText: function (cellInfo) {
                                return numeral(cellInfo.value).format('0,0.00');
                            }
                        },
                        {
                            dataField: "created_on",
                            dataType: "date",
                            caption: "Update Date",
                            format: "MMMM dd, yyyy HH:mm:ss",
                            allowEditing: false,
                            allowSorting: true,
                            formItem: {
                                visible: false
                            },
                            sortOrder: "desc"
                        },
                        {
                            dataField: "record_created_by",
                            dataType: "string",
                            caption: "Updated By",
                            formItem: {
                                colSpan: 2,
                            },
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
                        pageSize: 40
                    },
                    height: 500,
                    pager: {
                        allowedPageSizes: [40, 80, 100],
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
                        e.data.sales_plan_detail_id = eightweekforecastitemdata.id;
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
                }).appendTo(container);

            return container;
        }
    }

    var monthlyHistoryPopup = $("#popup").dxPopup(popupMonthlyHistoryOptions).dxPopup("instance")

    const showPlanMonthlyHistoryPopup = function (myData) {
        ////console.log("showPlanMonthlyHistoryPopup", myData);

        //if (myData.quantity === 0) {
        //    alert("Quantity is empty. Please edit quantity");
        //    return;
        //}
        monthlyHistoryPopup.option("contentTemplate", popupMonthlyHistoryOptions.contentTemplate.bind(this));
        monthlyHistoryPopup.show()
    }
    $('#btnDeleteSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnDeleteSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Deleting ...');

            $.ajax({
                url: url + "/DeleteSelectedRows",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Delete rows success");
                        $("#modal-delete-selected").modal('hide');
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnDeleteSelectedRow').html('Delete');
            });
        }
    });
    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');

            $.ajax({
                url: "/Planning/EightWeekForecast/ExcelExportSelected",
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
                // Check if the response is a blob
                if (data instanceof Blob) {
                    // Create a temporary anchor element
                    var a = document.createElement('a');
                    var url = window.URL.createObjectURL(data);
                    a.href = url;
                    a.download = "EightWeekForecast.xlsx"; // Set the appropriate file name here
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                    toastr["success"]("File downloaded successfully.");
                    //  $("#modal-download-selected").modal('hide');////

                } else {
                    toastr["error"]("File download failed.");
                }

                /*if (result) {
                    if (result.success) {
                        
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }*/
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnDownloadSelectedRow').html('Download');
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
                url: "/api/Planning/EightWeekForecast/UploadDocument",
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