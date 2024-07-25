$(function () {
    var token = $.cookie("Token");
    var $recordId = document.querySelector("[name=timesheet_id]").value
    var timesheetCNUnitId = document.querySelector("[name=timesheet_cn_unit_id]").value
    var timesheetEquipmentCapacity = 0
    var areaName = "Timesheet";
    var entityName = "TimesheetDetail";
    var gridUrl = "/api/" + areaName + "/" + entityName;
    var lookupTimeDataSource = {
        store: new DevExpress.data.CustomStore({
            key: "value",
            loadMode: "raw",
            load: function () {
                return $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: "/api/Shift/Shift/TimeLookup",
                    headers: {
                        "Authorization": "Bearer " + token
                    }
                });
            }
        }),
        sort: "text"
    };

    // Get equipment capacity
    $.ajax({
        url: '/api/Timesheet/Timesheet/EquipmentOrTruckCapacity?id=' + timesheetCNUnitId,
        type: 'GET',
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (response) {
            timesheetEquipmentCapacity = response.data[0].capacity
        }
    })


    var timesheetDetailDataGrid = $("#dt-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: gridUrl + "/DataGrid?timesheetId=" + encodeURIComponent($recordId),
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
                dataField: "id",
                visible: false,
                showInColumnChooser: false
            },
            {
                dataField: "start_time",
                dataType: "string",
                caption: "Start Time",
                lookup: {
                    dataSource: lookupTimeDataSource,
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "end_time",
                dataType: "string",
                caption: "End Time",
                lookup: {
                    dataSource: lookupTimeDataSource,
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
           /* {
                dataField: "classification",
                dataType: "datetime",
                editorType: "dxDateBox",
                editorOptions: {
                    useMaskBehavior: true,
                    displayFormat: "HH:mm",
                    type: "time",
                },
                format: "HH:mm",
                caption: "Classification",
                validationRules: [{
                    type: "required",
                    message: "Classification is required."
                }],
                *//*sortOrder: "asc",
                sortIndex: 1,*//*
                customizeText: function (cellInfo) {
                    let hour = moment(cellInfo.value).format("HH:mm")
                    let nextHour = moment(cellInfo.value).add(1, "hours").format("HH:mm")

                    return hour + "-" + nextHour
                },
                allowEditing: false
            },*/
            {
                dataField: "accounting_periode_id",
                dataType: "string",
                caption: "Accounting Period",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/accounting/accountingperiod/accountingperiodidlookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                validationRules: [{
                    type: "required",
                    message: "Accounting Period is required."
                }],
            },
            {
                dataField: "mohh",
                dataType: "string",
                caption: "MOHH",
            },
            {
                dataField: "wh",
                dataType: "string",
                caption: "WH",
            },
            {
                dataField: "idle",
                dataType: "string",
                caption: "Idle",
            },
            {
                dataField: "delay",
                dataType: "string",
                caption: "Delay",
            },
            {
                dataField: "breakdown",
                dataType: "string",
                caption: "Breakdown",
            },
            {
                dataField: "pit_id",
                dataType: "string",
                caption: "PIT",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Location/BusinessArea/BusinessAreaIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                validationRules: [{
                    type: "required",
                    message: "PIT is required."
                }],
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "loader_id",
                dataType: "string",
                caption: "Loader",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Equipment/EquipmentList/EquipmentIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "source_id",
                dataType: "string",
                caption: "Source",
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
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "destination_id",
                dataType: "string",
                caption: "Destination",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Timesheet/Timesheet/StockpileOrWasteLocationOrPortLocationIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "material_id",
                dataType: "string",
                caption: "Material",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Timesheet/Timesheet/ProductOrWasteIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            sort: "text"
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                validationRules: [{
                    type: "required",
                    message: "Material is required."
                }],
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "ritase",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                caption: "Ritase",
            },
            {
                dataField: "volume",
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 3
                    }
                },
                setCellValue: function (rowData, value) {
                    rowData.volume = value
                },
                caption: "Volume",
            },
            {
                dataField: "volume_auto_count_btn",
                visible: false,
                allowFiltering: false,
                showInColumnChooser: false,
            },
            {
                dataField: "distance",
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 3
                    }
                },
                caption: "Distance (meter)",
                visible: false
            },
            {
                dataField: "refuelling_quantity",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                caption: "Refuelling Quantity",
                visible: false
            },
            {
                dataField: "rit_rehandling",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                caption: "Ritase Rehandling",
            },
            {
                dataField: "vol_rehandling",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: {
                        type: "fixedPoint",
                        precision: 3,
                        step: 0
                    },
                },
                setCellValue: function (rowData, value) {
                    rowData.vol_rehandling = value
                },
                caption: "Volume Rehandling",
            },
            {
                dataField: "volume_rehandling_auto_count_btn",
                visible: false,
                allowFiltering: false,
                showInColumnChooser: false,
            },
            {
                dataField: "productivity",
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 3
                    },
                },
                caption: "Productivity",
            },
            {
                dataField: "productivity_recount_btn",
                visible: false,
                allowFiltering: false,
                showInColumnChooser: false,
            },
            {
                dataField: "vol_distance",
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 3
                    },
                },
                caption: "Volume x Distance",
            },
            {
                dataField: "vol_density",
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 3
                    },
                },
                caption: "Volume x Density",
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                showInColumnChooser: false
            },
            /*{
                dataField: "event_definition_category_id",
                dataType: "text",
                caption: "Category",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/EventDefinitionCategory/EventDefinitionCategoryIdLookup",
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
                onValueChangeAction: function (e) {
                    var newItem = e.selectedItem;
                    //console.log(newItem);
                    //newItem contains the selected item with all options  
                },
                setCellValue: function (rowData, value) {
                    //console.log("text", value);
                    rowData.event_definition_category_id = value;
                    rowData.event_category_id = " ";
                },
            },
            {
                dataField: "event_category_id",
                dataType: "text",
                caption: "Activity",
                validationRules: [{
                    type: "required",
                    message: "Event Category is required."
                }],
                editorOptions: { disabled: true },
                lookup: {
                    dataSource: function (options) {
                        //console.log($eventDefinitionCategoryId);
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/EventCategory/EventCategoryIdLookupFilterByEventDefinitionCategoryId",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            filter: $eventDefinitionCategoryId ? ["event_definition_category_id", "=", $eventDefinitionCategoryId] : null
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "event_category_code",
                dataType: "text",
                caption: "Activity Code",
                editorOptions: { readOnly: true }
            },
            {
                dataField: "mine_location_id",
                dataType: "text",
                caption: "Source Location",
                validationRules: [{
                    type: "required",
                    message: "The Mine Location is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Location/MineLocation/MineLocationCodeLookup",
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
                dataField: "destination_id",
                dataType: "text",
                caption: "Destination Location",
                validationRules: [{
                    type: "required",
                    message: "Destination Location is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Timesheet/Timesheet/StockpileOrWasteLocationIdLookup",
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
                dataField: 'periode',
                caption: "Hour",
                lookup: {
                    dataSource: {
                        store: {
                            type: 'array',
                            data: [
                                { id: 0, name: '00:00 - 01.00' },
                                { id: 1, name: '01:00 - 02.00' },
                                { id: 2, name: '02:00 - 03.00' },
                                { id: 3, name: '03:00 - 04.00' },
                                { id: 4, name: '04:00 - 05.00' },
                                { id: 5, name: '05:00 - 06.00' },
                                { id: 6, name: '06:00 - 07.00' },
                                { id: 7, name: '07:00 - 08.00' },
                                { id: 8, name: '08:00 - 09.00' },
                                { id: 9, name: '09:00 - 10.00' },
                                { id: 10, name: '10:00 - 11.00' },
                                { id: 11, name: '11:00 - 12.00' },
                                { id: 12, name: '12:00 - 13.00' },
                                { id: 13, name: '13:00 - 14.00' },
                                { id: 14, name: '14:00 - 15.00' },
                                { id: 15, name: '15:00 - 16.00' },
                                { id: 16, name: '16:00 - 17.00' },
                                { id: 17, name: '17:00 - 18.00' },
                                { id: 18, name: '18:00 - 19.00' },
                                { id: 19, name: '19:00 - 20.00' },
                                { id: 20, name: '20:00 - 21.00' },
                                { id: 21, name: '21:00 - 22.00' },
                                { id: 22, name: '22:00 - 23.00' },
                                { id: 23, name: '23:00 - 24.00' },
                            ],
                            key: "id"
                        },
                        pageSize: 10,
                        paginate: true
                    },
                    valueExpr: 'id',
                    displayExpr: 'name' 
                }
            },
            //{
            //    dataField: 'periode', // provides actual values
            //    lookup: {
            //        dataSource: [ 
            //            '00:00 - 01.00',
            //            '01:00 - 02.00',
            //            '02:00 - 03.00',
            //            '03:00 - 04.00',
            //            '04:00 - 05.00',
            //            '05:00 - 06.00',
            //            '06:00 - 07.00',
            //            '07:00 - 08.00',
            //            '08:00 - 09.00',
            //            '09:00 - 10.00',
            //            '10:00 - 11.00',
            //            '11:00 - 12.00',
            //            '12:00 - 13.00',
            //            '13:00 - 14.00',
            //            '14:00 - 15.00',
            //            '15:00 - 16.00',
            //            '16:00 - 17.00',
            //            '17:00 - 18.00',
            //            '18:00 - 19.00',
            //            '19:00 - 20.00',
            //            '20:00 - 21.00',
            //            '21:00 - 22.00',
            //            '22:00 - 23.00',
            //            '23:00 - 24.00',
            //        ]
            //    }
            //},
            {
                dataField: "duration",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                },
                caption: "Minute",
                validationRules: [{
                    type: "required",
                    message: "Minute is required."
                }],
            },
            {
                dataField: "quantity",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                },
                caption: "Quantity",
                validationRules: [{
                    type: "required",
                    message: "Quantity is required."
                }],
            },
            
            {
                dataField: "uom_id",
                dataType: "text",
                caption: "UoM",
                validationRules: [{
                    type: "required",
                    message: "Unit of Measurement is required."
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
            },*/
            {
                type: "buttons",
                buttons: ["edit"],
                showInColumnChooser: false
            }
        ],
        onInitNewRow: function (e) {
            e.data.timesheet_id = $recordId;
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
            mode: "batch",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 6,
                items: [
                    {
                        dataField: "classification",
                        colSpan: 6
                    },
                    {
                        dataField: "accounting_periode_id",
                        colSpan: 6
                    },
                    {
                        dataField: "pit_id",
                        colSpan: 3
                    },
                    {
                        dataField: "loader_id",
                        colSpan: 3
                    },
                    {
                        dataField: "source_id",
                        colSpan: 3
                    },
                    {
                        dataField: "destination_id",
                        colSpan: 3
                    },
                    {
                        dataField: "material_id",
                        colSpan: 3
                    },
                    {
                        dataField: "distance",
                        colSpan: 3
                    },
                    {
                        dataField: "ritase",
                        colSpan: 3
                    },
                    {
                        dataField: "rit_rehandling",
                        colSpan: 3
                    },
                    {
                        dataField: "volume",
                        colSpan: 2
                    },
                    {
                        dataField: "volume_auto_count_btn",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "Auto-count",
                            hint: "Auto-count volume by Ritase x Equipment Capacity",
                            disabled: true,
                        },
                        label: {
                            visible: false
                        }
                    },
                    {
                        dataField: "vol_rehandling",
                        colSpan: 2
                    },
                    {
                        dataField: "volume_rehandling_auto_count_btn",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "Auto-count",
                            hint: "Auto-count volume rehandling by Ritase rehandling x Equipment Capacity",
                            disabled: true,
                        },
                        label: {
                            visible: false
                        }
                    },
                    {
                        dataField: "productivity",
                        colSpan: 5
                    },
                    {
                        dataField: "productivity_recount_btn",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "Re-count",
                            hint: "Re-count Productivity",
                        },
                        label: {
                            visible: false
                        }
                    },
                    {
                        dataField: "vol_distance",
                        colSpan: 3
                    },
                    {
                        dataField: "vol_density",
                        colSpan: 3
                    },
                    {
                        dataField: "refuelling_quantity",
                        colSpan: 6
                    },
                    /*{
                        dataField: "periode",
                        //displayExpr: 'periode_detail'
                    },
                    {
                        dataField: "duration",
                    },
                    {
                        dataField: "quantity",
                    },
                    {
                        dataField: "uom_id",
                    },
                    {
                        dataField: "event_category_id",
                        colSpan: 2
                    },
                    {
                        dataField: "event_category_code",
                        colSpan: 2
                    },
                    {
                        dataField: "mine_location_id",
                        colSpan: 2
                    },*/
                ]
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
            if (e.parentType === "dataRow" && e.dataField == "classification" && !e.row.isNewRow) {
                let grid = e.component
                let index = e.row.rowIndex

                if (!grid.editIsOpenned) {
                    grid.editIsOpenned = true

                    setTimeout(() => {
                        let ritase = grid.cellValue(index, "ritase")
                        let volumeByRitaseTimesCapacity = ritase * timesheetEquipmentCapacity
                        let currentVolume = grid.cellValue(index, "volume")

                        let ritaseRehandling = grid.cellValue(index, "rit_rehandling")
                        let volumeRehandlingByRitaseRehandlingTimesCapacity = ritaseRehandling * timesheetEquipmentCapacity
                        let currentVolumeRehandling = grid.cellValue(index, "vol_rehandling")

                        if (currentVolume != volumeByRitaseTimesCapacity) {
                            grid.manualVolume = true
                        }
                        else {
                            grid.manualVolume = false
                        }

                        if (currentVolumeRehandling != volumeRehandlingByRitaseRehandlingTimesCapacity) {
                            grid.manualVolumeRehandling = true
                        }
                        else {
                            grid.manualVolumeRehandling = false
                        }

                        // Just to refresh all the form
                        // So volume_auto_count_btn is updated
                        grid.cellValue(index, "ritase", ritase)

                        grid.editIsOpenned = false
                    }, 500)
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "ritase") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    let ritase = e.value
                    let volume = ritase * timesheetEquipmentCapacity

                    grid.beginUpdate()

                    grid.cellValue(index, "ritase", ritase)
                    setVolumeByRitase(grid, index, volume, ritase)
                    setProductivity(grid, index)
                    setVolumeTimesDistance(grid, index)
                    setVolumeTimesDensity(grid, index)

                    grid.endUpdate()

                    standardHandler(e)
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "volume") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    let volume = e.value

                    grid.manualVolume = true

                    grid.beginUpdate()

                    grid.cellValue(index, "volume", volume)
                    setProductivity(grid, index)
                    setVolumeTimesDistance(grid, index)
                    setVolumeTimesDensity(grid, index)

                    grid.endUpdate()

                    standardHandler(e)
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "volume_auto_count_btn") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                if (grid.manualVolume) {
                    e.editorOptions.disabled = false
                }
                else {
                    e.editorOptions.disabled = true
                }

                e.editorOptions.onClick = function (e) {
                    let ritase = grid.cellValue(index, "ritase")
                    let volume = timesheetEquipmentCapacity * ritase

                    grid.manualVolume = false

                    setTimeout(function () {
                        grid.beginUpdate()

                        setVolumeByRitase(grid, index, volume, ritase)
                        setProductivity(grid, index)
                        setVolumeTimesDistance(grid, index)
                        setVolumeTimesDensity(grid, index)

                        grid.endUpdate()
                    }, 100)

                    standardHandler(e)
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "rit_rehandling") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    let ritaseRehandling = e.value
                    let volumeRehandling = ritaseRehandling * timesheetEquipmentCapacity

                    grid.beginUpdate()

                    grid.cellValue(index, "rit_rehandling", ritaseRehandling)
                    setVolumeRehandlingByRitaseRehandling(grid, index, volumeRehandling, ritaseRehandling)

                    grid.endUpdate()

                    standardHandler(e)
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "vol_rehandling") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    let volumeRehandling = e.value

                    grid.manualVolumeRehandling = true

                    grid.beginUpdate()
                    grid.cellValue(index, "vol_rehandling", volumeRehandling)
                    grid.endUpdate()

                    standardHandler(e)
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "volume_rehandling_auto_count_btn") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                if (grid.manualVolumeRehandling) {
                    e.editorOptions.disabled = false
                }
                else {
                    e.editorOptions.disabled = true
                }

                e.editorOptions.onClick = function (e) {
                    let ritaseRehandling = grid.cellValue(index, "rit_rehandling")
                    let volume = timesheetEquipmentCapacity * ritaseRehandling

                    grid.manualVolumeRehandling = false

                    setTimeout(function () {
                        grid.beginUpdate()
                        setVolumeRehandlingByRitaseRehandling(grid, index, volume, ritaseRehandling)
                        grid.endUpdate()
                    }, 100)

                    standardHandler(e)
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "productivity_recount_btn") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onClick = async function (e) {
                    grid.beginUpdate()
                    setProductivity(grid, index)
                    grid.endUpdate()

                    standardHandler(e)
                }
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
        },
        masterDetail: {
            enabled: true,
            template: masterDetailTemplate
        },
    }).dxDataGrid("instance");

    function setVolumeByRitase(grid, index, volume, ritase) {
        if (!timesheetEquipmentCapacity) {
            grid.cellValue(index, "volume", ritase);
        }
        else if (!grid.manualVolume) {
            grid.cellValue(index, "volume", volume);
        }
    }

    async function setProductivity(grid, index) {
        let volume = grid.cellValue(index, "volume")
        let minute = null

        await $.ajax({
            url: '/api/Timesheet/TimesheetDetailEvent/GetActivityOBCoal?timesheetDetailId=' + grid.cellValue(index, "id"),
            type: 'GET',
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                minute = response ? response.minute : 0
            }
        })

        let result = volume/(minute/60)

        grid.cellValue(index, "productivity", result)
    }

    function setVolumeTimesDistance(grid, index) {
        let volume = grid.cellValue(index, "volume")
        let distance = grid.cellValue(index, "distance")

        let result = volume * distance

        grid.cellValue(index,"vol_distance", result)
    }

    function setVolumeTimesDensity(grid, index) {
        let volume = grid.cellValue(index, "volume")
        let density = 1.3

        let result = volume * density

        grid.cellValue(index, "vol_density", result)
    }

    function setVolumeRehandlingByRitaseRehandling(grid, index, volume_rehandling, ritaseRehandling) {
        if (!timesheetEquipmentCapacity) {
            grid.cellValue(index, "vol_rehandling", ritaseRehandling);
        }
        else if (!grid.manualVolume) {
            grid.cellValue(index, "vol_rehandling", volume_rehandling);
        }
    }

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Event",
                    template: renderTimesheetDetailEvent(masterDetailOptions.data)
                },
                {
                    title: "Productivity Problem",
                    template: renderTimesheetDetailProductivityProblem(masterDetailOptions.data)
                },
            ]
        });
    }

    function renderTimesheetDetailEvent(currentRecord) {
        return function () {
            var detailName = "TimesheetDetailEvent";
            var urlDetail = "/api/Timesheet/TimesheetDetailEvent";

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/DataGrid?timesheetDetailId=" + encodeURIComponent(currentRecord.id),
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
                            dataField: "timesheet_detail_id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            },
                            formItem: {
                                visible: false
                            }
                        },
                        {
                            dataField: "event_definition_category_id",
                            dataType: "text",
                            caption: "Category",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/General/EventDefinitionCategory/EventDefinitionCategoryIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: ["is_problem_productivity", "<>", 1]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            onValueChangeAction: function (e) {
                                var newItem = e.selectedItem;
                                //console.log(newItem);
                                //newItem contains the selected item with all options  
                            },
                            setCellValue: function (rowData, value) {
                                //console.log("text", value);
                                rowData.event_definition_category_id = value;
                                rowData.event_category_id = null;
                                rowData.activity_code = null;
                            },
                            validationRules: [{
                                type: "required",
                                message: "Category is required."
                            }],
                            formItem: {
                                colSpan: 2,
                                editorOptions: {
                                    showClearButton: true
                                }
                            }
                        },
                        {
                            dataField: "event_category_id",
                            dataType: "text",
                            caption: "Activity",
                            validationRules: [{
                                type: "required",
                                message: "Activity is required."
                            }],
                            editorOptions: { disabled: true },
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/General/EventCategory/EventCategoryIdLookupFilterByEventDefinitionCategoryId",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: options.data ? ["event_definition_category_id", "=", options.data.event_definition_category_id] : null
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            }
                        },
                        {
                            dataField: "activity_code",
                            dataType: "text",
                            caption: "Activity Code",
                            editorOptions: { readOnly: true },
                        },
                        {
                            dataField: "minute",
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            },
                            caption: "Minute",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "Minute is required."
                                },
                                {
                                    type: "custom",
                                    message: "The entered was out of min/max range",
                                    validationCallback: function (args) {
                                        if (args.value > 60) {
                                            args.rule.message = "Minutes max. is 60"
                                            return false;
                                        }
                                        if (args.value < 1) {
                                            args.rule.message = "Minutes min. is 1"
                                            return false;
                                        }
                                        return true;
                                    }
                                }
                            ],
                            formItem: {
                                colSpan: 2
                            },
                        },
                    ],
                    onEditorPreparing: function (e) {
                        if (e.parentType === 'searchPanel') {
                            e.editorOptions.onValueChanged = function (arg) {
                                if (arg.value.length == 0 || arg.value.length > 2) {
                                    e.component.searchByText(arg.value);
                                }
                            }
                        }
                        if (e.dataField === "event_category_id" && e.parentType === "dataRow") {
                            e.editorOptions.disabled = !e.row.data.event_definition_category_id;
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
                                        //console.log("General/EventCategory/Detail: ", response);
                                        let record = response;
                                        grid.cellValue(index, "activity_code", record.event_category_code);
                                    }
                                })

                                standardHandler(e) // Calling the standard handler to save the edited value
                            }
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
                        e.data.timesheet_detail_id = currentRecord.id;
                    },
                    onSaved: function (e) {
                        timesheetDetailDataGrid.refresh()
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
        }
    }

    function renderTimesheetDetailProductivityProblem(currentRecord) {
        return function() {
            var detailName = "TimesheetDetailEvent";
            var urlDetail = "/api/Timesheet/TimesheetDetailProblemProductivity";

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/DataGrid?timesheetDetailId=" + encodeURIComponent(currentRecord.id),
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
                            dataField: "timesheet_detail_id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            },
                            formItem: {
                                visible: false
                            }
                        },
                        {
                            dataField: "event_definition_category_id",
                            dataType: "text",
                            caption: "Category",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/General/EventDefinitionCategory/EventDefinitionCategoryIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: options.data ? ["is_problem_productivity", "=", 1] : null
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            onValueChangeAction: function (e) {
                                var newItem = e.selectedItem;
                                //console.log(newItem);
                                //newItem contains the selected item with all options  
                            },
                            setCellValue: function (rowData, value) {
                                //console.log("text", value);
                                rowData.event_definition_category_id = value;
                                rowData.event_category_id = null;
                                rowData.activity_code = null;
                            },
                            validationRules: [{
                                type: "required",
                                message: "Category is required."
                            }],
                            formItem: {
                                colSpan: 2,
                                editorOptions: {
                                    showClearButton: true
                                }
                            }
                        },
                        {
                            dataField: "event_category_id",
                            dataType: "text",
                            caption: "Activity",
                            validationRules: [{
                                type: "required",
                                message: "Activity is required."
                            }],
                            editorOptions: { disabled: true },
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/General/EventCategory/EventCategoryIdLookupFilterByEventDefinitionCategoryId",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: options.data ? ["event_definition_category_id", "=", options.data.event_definition_category_id] : null
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            }
                        },
                        {
                            dataField: "activity_code",
                            dataType: "text",
                            caption: "Activity Code",
                            editorOptions: { readOnly: true },
                        },
                        {
                            dataField: "frequency",
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            },
                            caption: "Frequency",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "Frequency is required."
                                },
                            ],
                            formItem: {
                                colSpan: 2
                            },
                        },
                    ],
                    onEditorPreparing: function (e) {
                        if (e.parentType === 'searchPanel') {
                            e.editorOptions.onValueChanged = function (arg) {
                                if (arg.value.length == 0 || arg.value.length > 2) {
                                    e.component.searchByText(arg.value);
                                }
                            }
                        }
                        if (e.dataField === "event_category_id" && e.parentType === "dataRow") {
                            e.editorOptions.disabled = !e.row.data.event_definition_category_id;
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
                                        //console.log("General/EventCategory/Detail: ", response);
                                        let record = response;
                                        grid.cellValue(index, "activity_code", record.event_category_code);
                                    }
                                })

                                standardHandler(e) // Calling the standard handler to save the edited value
                            }
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
                        e.data.timesheet_detail_id = currentRecord.id;
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
        }
    }

});


