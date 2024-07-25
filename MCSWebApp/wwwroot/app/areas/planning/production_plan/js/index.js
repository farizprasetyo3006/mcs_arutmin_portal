$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "ProductionPlan";
    var url = "/api/" + areaName + "/" + entityName;
    var productionPlanMonthlyData = null;
    var productionPlanMonthlyHistoryData = null;
    var selectedIds = null;

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
                    searchExpr: ["search", "text"]
                }
            },
            {
                dataField: "production_plan_number",
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
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },

            //{
            //    dataField: "stock_location_id",
            //    dataType: "text",
            //    caption: "Location",
            //    validationRules: [{
            //        type: "required",
            //        message: "This field is required."
            //    }],
            //    formItem: {
            //        colSpan: 2,
            //    },
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: url + "/StockpileLocationIdLookup",
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
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]    
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
                dataType: "text",
                caption: "Plan Type",
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
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
                },
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The Plan Type is required."
                }],
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
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
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
                dataField: "product_category_id",
                dataType: "string",
                caption: "Product Category",
                formItem: {
                    colSpan: 2,
                },
                validationRules: [{
                    type: "required",
                    message: "The Activity Plan is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/ProductCategoryIdLookup",
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
                    displayExpr: "text",
                    searchExpr: ["search", "text"],
                },
            },
            {
                dataField: "product_id",
                dataType: "string",
                caption: "Product",
                formItem: {
                    colSpan: 2,
                },
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
                    displayExpr: "text",
                    searchExpr: ["search", "text"],
                },
                sortOrder: "asc"
            },
            {
                dataField: "contractor_id",
                dataType: "text",
                caption: "Contractor",
                formItem: {
                    colSpan: 2,
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/ContractorIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                    searchExpr: ["search", "text"]
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
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            //if (e.parentType == "dataRow") {

            //    // Disabled all columns/fields if is_locked is true
            //    if (e.dataField !== "is_locked" && e.row.data.is_locked) {
            //        e.editorOptions.disabled = true
            //    }
            //}
        },
        //onInitNewRow: function (e) {
        //    e.data.is_locked = false
        //    e.data.is_baseline = false
        //},
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

    const renderSalesPlanInformation = function (currentRecord, container) {
        let createdOnFormatted = currentRecord.created_on ? moment(currentRecord.created_on.split('T')[0]).format("D MMM YYYY") : '-'
        let modifiedOnFormatted = currentRecord.modified_on ? moment(currentRecord.modified_on.split('T')[0]).format("D MMM YYYY") : '-'

        let salesPlanInformationContainer = $(`
            <div>
                <h5 class="mb-3">Sales Plan Detail</h5>

                <div class="row mb-4">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Overview</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-6 pr-0">
                                    <div class="headline-title-container">
                                        <small class="font-weight-normal d-block mb-1">Sales Plan Name</small>
                                        <h4 class="headline-title font-weight-bold">${(currentRecord.plan_name ? currentRecord.plan_name : "-")}</h4>
                                    </div>
                                </div>
                                <div class="col-md-6 pl-0">
                                    <div class="headline-detail-container">
                                        <div class="d-flex align-items-start mb-3">
                                            <div class="d-inline-block mr-3">
                                                <div class="icon-circle">
                                                    <i class="fas fa-th-large fa-sm"></i>
                                                </div>
                                            </div>
                                            <div class="d-inline-block">
                                                <small class="font-weight-normal text-muted d-block mb-1">Site</small>
                                                <h5 class="font-weight-bold">${(currentRecord.site_name ? currentRecord.site_name : "-")}</h5>
                                            </div>
                                        </div>
                                        <div class="d-flex align-items-start mb-3">
                                            <div class="d-inline-block mr-3">
                                                <div class="icon-circle">
                                                    <i class="fas fa-box fa-sm"></i>
                                                </div>
                                            </div>
                                            <div class="d-inline-block">
                                                <small class="font-weight-normal text-muted d-block mb-1">Revision Number</small>
                                                <h5 class="font-weight-bold">${(currentRecord.revision_number ? currentRecord.revision_number : "-")}</h5>
                                            </div>
                                        </div>
                                        <div class="d-flex align-items-start">
                                            <div class="d-inline-block mr-3">
                                                <div class="icon-circle">
                                                    <i class="fas fa-flag fa-sm"></i>
                                                </div>
                                            </div>
                                            <div class="d-inline-block">
                                                <small class="font-weight-normal text-muted d-block mb-1">Is Baseline</small>
                                                <h5 class="font-weight-bold">${(currentRecord.is_baseline ? "Yes" : "No")}</h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Plan Information</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-calendar-alt fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Created Date</small>
                                                        <h5 class="font-weight-bold">${ createdOnFormatted }</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-calendar-alt fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Modified Date</small>
                                                        <h5 class="font-weight-bold">${ modifiedOnFormatted }</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-calculator fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Quantity</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.quantity ? formatNumber(currentRecord.quantity) : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-lock fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Is Locked</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.is_locked ? "Yes" : "No")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-user fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Created By</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.created_by_name ? currentRecord.created_by_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-user fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Modified By</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.modified_by_name ? currentRecord.modified_by_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-box fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Unit</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.uom_name ? currentRecord.uom_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-list fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Notes</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.notes ? currentRecord.notes : "-")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Not used -->
                <div class="row mb-5 d-none">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Plan Information</div>
                        <div class="card card-mcs">
                            <dl class="row card-body">
                                <dt class="col-md-4 mb-2">Sales Plan Name</dt>
                                <dd class="col-md-8">`+ (currentRecord.plan_name ? currentRecord.plan_name : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Start Date</dt>
                                <dd class="col-md-8">`+ (currentRecord.start_date ? currentRecord.start_date.split('T')[0] : "-") + `</dd>
                                        
                                <dt class="col-md-4 mb-2">End Date</dt>
                                <dd class="col-md-8">`+ (currentRecord.end_date ? currentRecord.end_date.split('T')[0] : "-") + `</dd>

                                <dt class="col-md-4 mb-2">Quantity</dt>
                                <dd class="col-md-8">`+ (currentRecord.quantity ? currentRecord.quantity : "-") + `</dd>
                                
                                <dt class="col-md-4 mb-2">Unit</dt>
                                <dd class="col-md-8">`+ (currentRecord.uom_name ? currentRecord.uom_name : "-") + `</dd>
                            </dl>
                        </div>
                    </div>
                    <div class="col-md-6">
                    </div>
                </div>
            </div>
        `).appendTo(container)
    }

    const renderSalesPlanMonthly = function (currentRecord, container) {
        var detailName = "ProductionPlanMonthly";
        var urlDetail = "/api/" + areaName + "/" + detailName;

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
                    //{
                    //    dataField: "quantity",
                    //    dataType: "number",
                    //    caption: "Quantity",
                    //    format: "fixedPoint",
                    //    formItem: {
                    //        editorType: "dxNumberBox",
                    //        editorOptions: {
                    //            format: "fixedPoint",
                    //        }
                    //    },
                    //    customizeText: function (cellInfo) {
                    //        return numeral(cellInfo.value).format('0,0.00');
                    //    }
                    //},
                    {
                        dataField: "quantity",
                        dataType: "number",
                        caption: "Quantity",
                        format: {
                            type: "fixedPoint",
                            precision: 3,
                            visible: false
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
                                productionPlanMonthlyHistoryData = e.row.data;
                                showPlanMonthlyHistoryPopup(productionPlanMonthlyHistoryData);
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
                                productionPlanMonthlyData = e.row.data;
                                showSalesPlanMonthlyCustomerPopup(productionPlanMonthlyData);
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
                        url: urlDetail + "/GetTotalQuantityByPlanId/" + currentRecord.id,
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
                        //    <small class="font-weight-normal">Production Plan Number</small>
                        //    <h3 class="font-weight-bold">`+ productionPlanMonthlyData.production_plan_number +`</h6>
                        //</div>
            let container = $("<div>")

            $(`<div class="mb-3">
                    <div class="row">

                        <div class="col-md-2">
                            <small class="font-weight-normal">Month</small>
                            <h3 class="font-weight-bold">`+ productionPlanMonthlyData.month_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Quantity</small>
                            <h3 class="font-weight-bold" id="monthly-quantity">`+ formatNumber(productionPlanMonthlyData.quantity) + `</h6>
                        </div>
                    </div>
                </div>
            `).appendTo(container)


            let url = "/api/Planning/ProductionPlanDaily";
            $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/ByProductionPlanDailyId/" + productionPlanMonthlyData.id,
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
                        //        return productionPlanMonthlyData.id;
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
                            dataField: "week_count",
                            dataType: "string",
                            caption: "Week",
                           allowEditing: false,
                        },
                        {
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity",
                            format: {
                                type: "fixedPoint",
                                precision: 3
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
                    onRowUpdated: function (e) {
                        //haulingPlanMonthlyData.quantity = 77;
                        //selectedPlanRecord.total_quantity = 88;

                        $.ajax({
                            url: "/api/planning/ProductionPlanMonthly/GetResultById/" + productionPlanMonthlyData.id,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (r) {
                                if (r.success) {
                                    $("#monthly-quantity").html(formatNumber(r.data.monthly_quantity_updated));
                                    $("#monthly-grid").dxDataGrid("getDataSource").reload();
                                    selectedPlanRecord.total_quantity = r.data.total_quantity_updated;
                                    var dataGrid = $('#grid').dxDataGrid('instance');
                                    dataGrid.saveEditData();
                                }
                            }
                        })

                        /*var gridInstance = $("#grid").dxDataGrid("instance");
                        gridInstance.refresh();
                        popup.repaint();*/
                    },
                    onInitNewRow: function (e) {
                        e.data.sales_plan_detail_id = productionPlanMonthlyData.id;
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

    var popup = $("#popup2").dxPopup(popupOptions).dxPopup("instance")

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
            ////console.log("Monthly History", productionPlanMonthlyHistoryData);
            //<div class="col-md-3">
            //    <small class="font-weight-normal">Production Plan Number</small>
            //    <h3 class="font-weight-bold">`+ productionPlanMonthlyData.hauling_plan_number +`</h6>
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
                        e.data.sales_plan_detail_id = productionPlanMonthlyData.id;
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
                url: "/Planning/ProductionPlan/ExcelExport",
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
                    a.download = "Production_Plan.xlsx"; // Set the appropriate file name here
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