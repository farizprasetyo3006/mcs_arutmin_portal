$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "HaulingPlan";
    var url = "/api/" + areaName + "/" + entityName;
    var haulingPlanMonthlyData = null;
    var haulingPlanMonthlyHistoryData = null;
    var CustomerId = ""
    //const planTypes = [
    //    'RKAB',
    //    'PRODUCTION PLAN',
    //    'HAULING PLAN',
    //];

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
            },
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
                dataField: "hauling_plan_number",
                dataType: "string",
                caption: "Planning Number",
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "location_id",
                dataType: "text",
                caption: "Location",
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The Location field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Location/BusinessArea/BusinessAreaIdLookup",
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
                dataField: "pit_id",
                dataType: "text",
                caption: "Pit",
                formItem: {
                    colSpan: 2,
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Location/BusinessArea/BusinessAreaIdLookup",
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
                dataField: "master_list_id",
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
                dataField: "mine_location_id",
                dataType: "string",
                caption: "Seam",
                formItem: {
                    colSpan: 2,
                },
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Location/MineLocation/MineLocationIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            //filter: ["item_group", "=", "years"]
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
                caption: "Quantity",
                format: "fixedPoint",
                formItem: {
                    visible: false
                },
                customizeText: function (cellInfo)
                {
                    return numeral(cellInfo.value).format('0,0.00');
                },
                setCellValue: function (newData, value) {

                }
            },
            //{
            //    dataField: "plan_type",
            //    dataType: "dxSelectBox",
            //    caption: "Plan Type",
            //    formItem: {
            //        colSpan: 2,
            //    },
            //    validationRules: [{
            //        type: "required",
            //        message: "The Mine Location is required."
            //    }],
            //    lookup: {
            //        dataSource: planTypes,
            //    },
            //},
            {
                dataField: "plan_type",
                dataType: "string",
                caption: "Plan Type",
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The Plan Type is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=plan-type",
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
                dataField: "activity_plan",
                dataType: "text",
                caption: "Activity Plan",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=activity-plan-type",
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
                formItem: {
                    colSpan: 2,
                },
                /*validationRules: [{
                    type: "required",
                    message: "The Activity Plan is required."
                }],*/
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },

            {
                dataField: "product_id",
                dataType: "string",
                caption: "Product",
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The field Product is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Material/Product/ProductIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            //filter: ["item_group", "=", "years"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                sortOrder: "asc"
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                allowEditing: false,
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
                    displayExpr: "text"
                }
            },
            //{
            //    dataField: "notes",
            //    label: {
            //        text: "Remark"
            //    },
            //    formItem: {
            //        colSpan: 2,
            //    },
            //    editorType: "dxTextArea",
            //    editorOptions: {
            //        height: 50,
            //    },
            //    visible: false
            //},
            //{
            //    caption: "See Report",
            //    type: "buttons",
            //    width: 150,
            //    buttons: [{
            //        cssClass: "btn-dxdatagrid",
            //        hint: "See Contract Terms",
            //        text: "Open Report",
            //        onClick: function (e) {
            //            salesPlanSnapshotId = e.row.data.id
            //            window.location = "/Planning/SalesPlan/Report?salesPlanId=" + salesPlanSnapshotId
            //        }
            //    }]
            //},
            {
                type: "buttons",
                buttons: ["edit", "delete"]
            }
        ],
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentRecord = options.data;

                // Sales Plan Details (Monthly) Container
                renderSalesPlanMonthly(currentRecord, container)
            }
        },
        onContentReady: function (e) {
            let grid = e.component
            let queryString = window.location.search
            let params = new URLSearchParams(queryString)

            let salesPlanId = params.get("Id")

            if (salesPlanId) {
                grid.filter(["id", "=", salesPlanId])

                /* Open edit form */
                if (params.get("openEditingForm") == "true") {
                    let rowIndex = grid.getRowIndexByKey(salesPlanId)

                    grid.editRow(rowIndex)
                }
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
            if (e.parentType == "dataRow" && e.dataField == "plan_type") {
                var editor = e.component;
                e.editorOptions.hint = "hauling plan type";
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
    const renderSalesPlanMonthly = function (currentRecord, container) {
        var detailName = "HaulingPlanMonthly";
        var urlDetail = "/api/" + areaName + "/" + detailName;

        let salesPlanDetailsContainer = $("<div class='mb-5'>")
        salesPlanDetailsContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-2")
            .text("Monthly Entry/View")
            .appendTo(salesPlanDetailsContainer);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: urlDetail + "/ByHaulingPlanId/" + encodeURIComponent(currentRecord.id),
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
                    //{
                    //    dataField: "hauling_plan_id",
                    //    allowEditing: false,
                    //    visible: false,
                    //    formItem: {
                    //        colSpan: 2
                    //    },
                    //    calculateCellValue: function () {
                    //        return currentRecord.id;
                    //    }
                    //},
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
                        sortOrder: "asc"
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
                        customizeText: function (cellInfo) {
                            return numeral(cellInfo.value).format('0,0.00');
                        }
                    },
                    {
                        caption: "History",
                        type: "buttons",
                        width: 200,
                        buttons: [{
                            cssClass: "btn-dxdatagrid",
                            hint: "History",
                            text: "History",
                            onClick: function (e) {
                                haulingPlanMonthlyHistoryData = e.row.data;
                                showPlanMonthlyHistoryPopup(haulingPlanMonthlyHistoryData);
                            }
                        }]
                    },
                    {
                        caption: "Detail",
                        type: "buttons",
                        width: 200,
                        buttons: [{
                            cssClass: "btn-dxdatagrid",
                            hint: "Daily Details",
                            text: "Daily Details",
                            onClick: function (e) {
                                haulingPlanMonthlyData = e.row.data;
                                showSalesPlanMonthlyCustomerPopup(haulingPlanMonthlyData);
                            }
                        }]
                    },
                    {
                        type: "buttons",
                        buttons: ["edit"]
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
                    e.data.sales_plan_id = currentRecord.id;
                },
                onRowUpdated: function (e) {

                    $.ajax({
                        url: url + "/GetMontlyPlanTotalQuantityByHaulingPlanId/" + currentRecord.id,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            currentRecord.total_quantity = response || 0;
                            setTimeout(function () {
                                var dataGrid = $('#grid').dxDataGrid('instance');
                                dataGrid.saveEditData();
                            }, 100);
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
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailName + '.xlsx');
                        });
                    });
                    e.cancel = true;
                }
            }).appendTo(salesPlanDetailsContainer);
    }

    let popupOptions = {
        title: "Daily Entry/View",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function () {
                                    //<div class="col-md-3">
                        //    <small class="font-weight-normal">Hauling Plan Number</small>
                        //    <h3 class="font-weight-bold">`+ haulingPlanMonthlyData.hauling_plan_number +`</h6>
                        //</div>
            let container = $("<div>")

            $(`<div class="mb-3">
                    <div class="row">

                        <div class="col-md-2">
                            <small class="font-weight-normal">Month</small>
                            <h3 class="font-weight-bold">`+ haulingPlanMonthlyData.month_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Quantity</small>
                            <h3 class="font-weight-bold">`+ formatNumber(haulingPlanMonthlyData.quantity) + `</h6>
                        </div>
                    </div>
                </div>
            `).appendTo(container)


            let url = "/api/Planning/HaulingPlanDaily";
            $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/ByHaulingMonthlyId/" + haulingPlanMonthlyData.id,
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
                        //{
                        //    dataField: "hauling_plan_monthly_id",
                        //    dataType: "string",
                        //    caption: "Hauling Plan Monthly",
                        //    allowEditing: false,
                        //    visible: false,
                        //    formItem: {
                        //        visible: false
                        //    },
                        //    calculateCellValue: function () {
                        //        return haulingPlanMonthlyData.id;
                        //    }
                        //},
                        {
                            dataField: "daily_date",
                            dataType: "date",
                            caption: "Date",
                            allowEditing: false,
                            allowSorting: true,
                            formItem: {
                                visible: false
                            },
                            sortOrder: "asc"
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
                            customizeText: function (cellInfo) {
                                return numeral(cellInfo.value).format('0,0.00');
                            }
                        },
                        {
                            type: "buttons",
                            buttons: ["edit"]
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
                        e.data.sales_plan_detail_id = haulingPlanMonthlyData.id;
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

    var popup = $("#popup").dxPopup(popupOptions).dxPopup("instance")

    const showSalesPlanMonthlyCustomerPopup = function (myData) {
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
            //<div class="col-md-3">
            //    <small class="font-weight-normal">Hauling Plan Number</small>
            //    <h3 class="font-weight-bold">`+ haulingPlanMonthlyData.hauling_plan_number +`</h6>
            //</div>
            let container = $("<div>")

            $(`<div class="mb-3">
                    <div class="row">

                        <div class="col-md-2">
                            <small class="font-weight-normal">Month</small>
                            <h3 class="font-weight-bold">`+ haulingPlanMonthlyHistoryData.month_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Quantity</small>
                            <h3 class="font-weight-bold">`+ formatNumber(haulingPlanMonthlyHistoryData.quantity) + `</h6>
                        </div>
                    </div>
                </div>
            `).appendTo(container)


            let url = "/api/Planning/HaulingPlanMonthly";
            var detailName = "HaulingPlanMonthly";
            var urlHistoryDetail = "/api/" + areaName + "/HaulingPlanMonthly";

            $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/History/ByHaulingPlanMonthlyId/" + haulingPlanMonthlyHistoryData.id,
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
                        //{
                        //    dataField: "hauling_plan_monthly_id",
                        //    dataType: "string",
                        //    caption: "Hauling Plan Monthly",
                        //    allowEditing: false,
                        //    visible: false,
                        //    formItem: {
                        //        visible: false
                        //    },
                        //    calculateCellValue: function () {
                        //        return haulingPlanMonthlyData.id;
                        //    }
                        //},
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
                                    loadUrl: urlHistoryDetail + "/MonthIndexLookup",
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
                            sortOrder: "asc"
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
                            }
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
                        e.data.sales_plan_detail_id = haulingPlanMonthlyData.id;
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

        //if (myData.quantity === 0) {
        //    alert("Quantity is empty. Please edit quantity");
        //    return;
        //}
        monthlyHistoryPopup.option("contentTemplate", popupMonthlyHistoryOptions.contentTemplate.bind(this));
        monthlyHistoryPopup.show()
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
                url: "/api/Planning/SalesPlan/UploadDocument",
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