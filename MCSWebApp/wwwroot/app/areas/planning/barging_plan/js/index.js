$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "BargingPlan";
    var url = "/api/" + areaName + "/" + entityName;
    var haulingPlanMonthlyData = null;
    var haulingPlanMonthlyHistoryData = null;
    var CustomerId = ""
    var selectedPlanRecord;
    var ids = "";

    //const planTypes = [
    //    'RKAB',
    //    'PRODUCTION PLAN',
    //    'HAULING PLAN',
    //    'BARGING PLAN',
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
                dataField: "barging_plan_number",
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
            //{
            //    dataField: "pit_id",
            //    dataType: "text",
            //    caption: "Pit",
            //    formItem: {
            //        colSpan: 2,
            //    },
            //    validationRules: [{
            //        type: "required",
            //        message: "The Pit field is required."
            //    }],
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/Location/BusinessArea/BusinessAreaIdLookup",
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
            //    calculateSortValue: function (data) {
            //        var value = this.calculateCellValue(data);
            //        return this.lookup.calculateCellValue(value);
            //    }
            //},
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
            //{
            //    dataField: "mine_location_id",
            //    dataType: "string",
            //    caption: "Seam",
            //    formItem: {
            //        colSpan: 2,
            //    },
            //    validationRules: [{
            //        type: "required",
            //        message: "The Mine Location is required."
            //    }],
            //    lookup: {
            //        dataSource: function (options) {
            //            return {
            //                store: DevExpress.data.AspNet.createStore({
            //                    key: "value",
            //                    loadUrl: "/api/Location/MineLocation/MineLocationIdLookup",
            //                    onBeforeSend: function (method, ajaxOptions) {
            //                        ajaxOptions.xhrFields = { withCredentials: true };
            //                        ajaxOptions.beforeSend = function (request) {
            //                            request.setRequestHeader("Authorization", "Bearer " + token);
            //                        };
            //                    }
            //                }),
            //                //filter: ["item_group", "=", "years"]
            //            }
            //        },
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    sortOrder: "asc"
            //},
            {
                dataField: "total_quantity",
                dataType: "number",
                caption: "Quantity",
                format: "fixedPoint",
                formItem: {
                    visible: false
                },
                customizeText: function (cellInfo) {
                    return numeral(cellInfo.value).format('0,0.00');
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
                visisble:false,
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
                    visible: true,
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
                dataField: "product_category_id",
                dataType: "text",
                caption: "Product Category",
                validationRules: [{
                    type: "required",
                    message: "The Product Category field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Material/ProductSpecification/ProductCategoryIdLookup",
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
                    rowData.product_category_id = value;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
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
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                sortOrder: "asc"
            },
            {
                dataField: "contractor_id",
                dataType: "text",
                caption: "Contractor",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/organisation/contractor/contractoridlookup",
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
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
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
                selectedPlanRecord = options.data;

                // Sales Plan Details (Monthly) Container
                renderSalesPlanMonthly(currentRecord, container)
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
                //e.editorOptions.placeholder = "Barging plan type...";
                e.editorOptions.hint = "barging plan type";
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
        var detailName = "BargingPlanMonthly";
        var urlDetail = "/api/" + areaName + "/" + detailName;
        id = currentRecord.barging_plan_monthly_id;
        let salesPlanDetailsContainer = $("<div class='mb-5'>")
        salesPlanDetailsContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-2")
            .text("Monthly Entry/View")
            .appendTo(salesPlanDetailsContainer);

        $("<div id='monthly-grid'>")
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
                        caption: "Actions",
                        type: "buttons",
                        width: 100,
                        buttons: [{
                            cssClass: "btn-dxdatagrid",
                            hint: "Fetch",
                            text: "Fetch",
                            onClick: function (e) {
                                //console.log("tes", e.row.data)
                                var monthlyGrid = $("#monthly-grid").dxDataGrid("instance");

                                Swal.fire({
                                    title: 'This will affect to daily records!',
                                    text: "Are you sure want to Fetch?",
                                    icon: 'warning',
                                    showCancelButton: true,
                                    confirmButtonText: 'Yes',
                                    cancelButtonText: 'No',
                                }).then((result) => {
                                    if (result) {
                                        monthlyGrid.beginCustomLoading();
                                        $.ajax({
                                            url: '/api/Planning/bargingPlanMonthly/FetchById/' + e.row.data.id,
                                            type: 'GET',
                                            contentType: "application/json",
                                            headers: {
                                                "Authorization": "Bearer " + token
                                            },
                                            //data: JSON.stringify(request)
                                        }).done(function (result) {
                                            //console.log(result);
                                            if (result.success) {
                                                monthlyGrid.refresh(true);
                                                toastr["success"](result.message ?? "Success");
                                            } else {
                                                toastr["error"](result.message ?? "Error");
                                            }
                                            monthlyGrid.endCustomLoading();
                                        }).fail(function (jqXHR, textStatus, errorThrown) {
                                            Swal.fire("Failed !", textStatus, "error");
                                            monthlyGrid.endCustomLoading();
                                        });
                                    }
                                })
                            }
                        }]
                    },
                    {
                        caption: "History",
                        type: "buttons",
                        width: 100,
                        buttons: [{
                            cssClass: "btn-dxdatagrid",
                            hint: "History",
                            text: "History",
                            onClick: function (e) {
                                haulingPlanMonthlyHistoryData = e.row;
                                showPlanMonthlyHistoryPopup(haulingPlanMonthlyHistoryData);
                            }
                        }]
                    },
                    {
                        caption: "Detail",
                        type: "buttons",
                        width: 130,
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
                },
                onRowUpdated: function (e) {
                    currentRecord.total_quantity = 88;
                    var dataGrid = $('#grid').dxDataGrid('instance');
                    dataGrid.saveEditData();

                    setTimeout(function () {
                    }, 100);
                    //}
                    //})

                }, 
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
                            <h3 class="font-weight-bold" id="monthly-quantity">`+ formatNumber(haulingPlanMonthlyData.quantity) + `</h6>
                        </div>
                    </div>
                </div>
            `).appendTo(container)


            let url = "/api/Planning/BargingPlanDaily";
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

                        //{
                        //    dataField: "start_date",
                        //    dataType: "date",
                        //    caption: "Contract Start Date",
                        //    validationRules: [{
                        //        type: "required",
                        //        message: "The field is required."
                        //    }],
                        //},

                        {
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity",
                            format: "fixedPoint",
                            customizeText: function (cellInfo) {
                                return numeral(cellInfo.value).format('0,0.00');
                            },
                            onKeyDown: function (e) {
                                //console.log(e);
                                if (e.name === "changedProperty") {
                                    // handle the property change here
                                }
                            }
                        },
                        {
                            dataField: "operational_hours",
                            dataType: "number",
                            caption: "Operational Hours",
                            format: "fixedPoint",
                            customizeText: function (cellInfo) {
                                return numeral(cellInfo.value).format('0,0.00');
                            },
                            onKeyDown: function (e) {
                                //console.log(e);
                                if (e.name === "changedProperty") {
                                    // handle the property change here
                                }
                            }
                        },

                        {
                            dataField: "loading_rate",
                            allowEditing: false,
                            dataType: "number",
                            caption: "Loading Rate",
                            format: "fixedPoint",
                            customizeText: function (cellInfo) {
                                return numeral(cellInfo.value).format('0,0.00');
                            },
                            formItem: {
                                visible: false
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
                    },
                    onRowUpdated: function (e) {

                        
                        ////console.log("haulingPlanMonthlyData", haulingPlanMonthlyData);
                        ////console.log("selectedPlanRecord", selectedPlanRecord);
                        //haulingPlanMonthlyData.quantity = 77;
                        //selectedPlanRecord.total_quantity = 88;

                        $.ajax({
                            url: "/api/planning/BargingPlanMonthly/GetResultById/" + haulingPlanMonthlyData.id,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (r) {
                                if (r.success) {
                                    selectedPlanRecord.total_quantity = r.data.total_quantity_updated;
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();

                                    $("#monthly-quantity").html(formatNumber(r.data.monthly_quantity_updated));
                                    $("#monthly-grid").dxDataGrid("getDataSource").reload();
                                }
                            }
                        })

                    }, 
                }).appendTo(container);

            return container;
        }
    }

    var popup = $("#popup").dxPopup(popupOptions).dxPopup("instance")

    const showSalesPlanMonthlyCustomerPopup = function (myData) {
        popup.option("contentTemplate", popupOptions.contentTemplate.bind(this));
        popup.show()
    }



    //History
    let popupMonthlyHistoryOptions = {
        title: "Monthly History",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function (e) {
            //console.log("Monthly History", haulingPlanMonthlyHistoryData);
            //<div class="col-md-3">
            //    <small class="font-weight-normal">Hauling Plan Number</small>
            //    <h3 class="font-weight-bold">`+ haulingPlanMonthlyData.hauling_plan_number +`</h6>
            //</div>
            let container = $("<div>")

            $(`<div class="mb-3">
                    <div class="row">

                        <div class="col-md-2">
                            <small class="font-weight-normal">Month</small>
                            <h3 class="font-weight-bold">`+ haulingPlanMonthlyHistoryData.data.month_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Quantity</small>
                            <h3 class="font-weight-bold">`+ formatNumber(haulingPlanMonthlyHistoryData.data.quantity) + `</h6>
                        </div>
                    </div>
                </div>
            `).appendTo(container)


            let url = "/api/Planning/BargingPlanMonthly";
            var detailName = "HaulingPlanMonthly";
            var urlHistoryDetail = "/api/" + areaName + "/HaulingPlanMonthly";
            $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/History/ByHaulingPlanMonthlyId?Id=" + haulingPlanMonthlyHistoryData.key,
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
                            sortOrder: "desc",
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
        //console.log("showPlanMonthlyHistoryPopup", myData);

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
                url: "/Planning/BargingPlan/ExcelExport",
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
                    a.download = "Barging_Plan.xlsx"; // Set the appropriate file name here
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
                url: "/api/Planning/BargingPlan/UploadDocument",
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