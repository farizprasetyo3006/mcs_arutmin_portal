$(function () {

    var token = $.cookie("Token");
    var areaName = "Port";
    var entityName = "SILS";
    var url = "/api/" + areaName + "/" + entityName;    
    var selectedIds = null;
    var tareValue = 0;
    var grossValue = 0;

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 100,
        "timeOut": 3000,
        "extendedTimeOut": 1000,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    $("#AccountingPeriod").select2({
        ajax:
        {
            url: "/api/Accounting/AccountingPeriod/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0,
        width: '100%',
        dropdownParent: $("#modal-accounting-period")
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#accounting_period_id').val(data.id);
    }).on('select2:clear', function (e) {
        $('#accounting_period_id').val('');
    });

    $("#QualitySampling").select2({
        ajax:
        {
            //url: "/api/StockpileManagement/QualitySampling/select2",
            url: url + "/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0,
        width: '100%',
        dropdownParent: $("#modal-quality-sampling")
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#quality_sampling_id').val(data.id);
    }).on('select2:clear', function (e) {
        $('#quality_sampling_id').val('');
    });

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("productionDate1");
    var tgl2 = sessionStorage.getItem("productionDate2");

    var date = new Date(), y = date.getFullYear(), m = date.getMonth(), d = date.getDate();
    // Set the hours, minutes, and seconds for the first day
    var firstDay = new Date(y, m, d - 1, 7, 0, 0);

    // Set the hours, minutes, and seconds for the last day
    var lastDay = new Date(y, m, d, 6, 59, 59);

    if (tgl1 != null)
        firstDay = Date.parse(tgl1);

    if (tgl2 != null)
        lastDay = Date.parse(tgl2);

    $("#date-box1").dxDateBox({
        type: "datetime",
        displayFormat: 'dd MMM yyyy HH:mm',
        value: firstDay,
        onValueChanged: function (data) {
            firstDay = new Date(data.value);
            sessionStorage.setItem("productionDate1", formatTanggal(firstDay));
            _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                + "/" + encodeURIComponent(formatTanggal(lastDay));
        }
    });

    $("#date-box2").dxDateBox({
        type: "datetime",
        displayFormat: 'dd MMM yyyy HH:mm',
        value: lastDay,
        onValueChanged: function (data) {
            lastDay = new Date(data.value);
            sessionStorage.setItem("productionDate2", formatTanggal(lastDay));
            _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                + "/" + encodeURIComponent(formatTanggal(lastDay));
        }
    });

    $('#btnView').on('click', function () {
        location.reload();
    })

    var _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
        + "/" + encodeURIComponent(formatTanggal(lastDay));

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            //loadUrl: url + "/DataGrid",
            loadUrl: _loadUrl,
            insertUrl: url + "/InsertData",
            updateUrl: url + "/UpdateData",
            deleteUrl: url + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function(request){
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };                
            }
        }),
        selection: {
            mode: "multiple"
        },
        remoteOperations: true,
        allowColumnResizing: true,
        columnMinWidth: 100,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "barge_loading_number",
                dataType: "string",
                caption: "Barge Loading Number",
                allowEditing: false,       
                /*formItem: {
                    colSpan: 2,
                },*/
            },
            {
                dataField: "despatch_order_id",
                dataType: "text",
                caption: "Shipping Order",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/DespatchOrderIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "barge_rotation_id",
                dataType: "text",
                caption: "Voyage Number",
                //allowEditing: true,
                validationRules: [{
                    type: "required",
                    message: "The Voyage Number Field is Required",
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/VoyageIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "master_list_id",
                dataType: "text",
                caption: "Barging Type",
                /*formItem: {
                    colSpan: 2,
                },*/
                validationRules: [{
                    type: "required",
                    message: "The Barging Type Field is Required",
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
                            filter: ["item_group", "=", "sils-barging-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "barge_id",
                dataType: "text",
                caption: "Barge Name",
                validationRules: [{
                    type: "required",
                    message: "The Barge Name Field is Required",
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/BargeIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "tug_id",
                dataType: "text",
                caption: "Tug Boat",
                validationRules: [{
                    type: "required",
                    message: "The Tug Boat Field is Required",
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/TugIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "destination_location",
                dataType: "string",
                caption: "Destination",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "date_arrived",
                dataType: "datetime",
                caption: "Date Arrived",
                format: "yyyy-MM-dd HH:mm",
                validationRules: [{
                    type: "required",
                    message: "The Tug Boat Field is Required",
                }],
            },
            {
                dataField: "date_berthed",
                dataType: "datetime",
                caption: "Date Berthed",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "start_loading",
                dataType: "datetime",
                caption: "Start Loading",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "finish_loading",
                dataType: "datetime",
                caption: "Finish Loading",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "unberthed_time",
                dataType: "datetime",
                caption: "Unberthed Time",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "departed_time",
                dataType: "datetime",
                caption: "Departed",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "product_id",
                dataType: "text",
                caption: "Product Name",
                width: "100px",
                validationRules: [{
                    type: "required",
                    message: "The Product Name Field is Required",
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
                            })
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "analyte_1",
                dataType: "string",
                caption: "Ash",
                allowEditing: true,
                visible: false
            },
            {
                dataField: "analyte_2",
                dataType: "string",
                caption: "TS",
                allowEditing: true,
                visible: false
            },
            {
                dataField: "water_consumption",
                dataType: "number",
                caption: "Water Consumption",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
                visible: false
            },
            {
                dataField: "chemical_consumption",
                dataType: "number",
                caption: "Chemical Consumption",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
                visible: false
            },
            {
                dataField: "draft_scale",
                dataType: "number",
                caption: "Draft Scale",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
                visible: false
            },
            {
                dataField: "belt_scale",
                dataType: "number",
                caption: "Belt Scale (CV-03)",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
                visible: false
            },  
            {
                dataField: "operator_id",
                dataType: "text",
                caption: "Operator Name",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/OperatorIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                sortOrder: "asc"
            },
            {
                dataField: "foreman_id",
                dataType: "text",
                caption: "Foreman Name",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/ForemanIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                sortOrder: "asc"
            },
            {
                dataField: "captain_id",
                dataType: "text",
                caption: "Port Captain",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/PortCaptainIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                sortOrder: "asc"
            },
            /*{
                dataField: "approved_by",
                dataType: "string",
                caption: "Approve By",
                allowEditing: false,
                visible: false
            },*/
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                allowEditing: true,
                validationRules: [{
                    type: "required",
                    message: "The Business Unit Field is Required",
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
                dataField: "approve_status",
                dataType: "string",
                caption: "Status Approve",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                }
            },
            {
                caption: "Approval Button",
                type: "buttons",
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    text: "Approve",
                    onClick: function (e) {
                        salesInvoiceApprovalData = e.row.data;
                        showApprovalPopup();
                    }
                }]
            },
            {
                type: "buttons",
                buttons: ["edit", "delete"],
                showInColumnChooser: true
            }
        ],
       /* summary: {
            totalItems: [
                {
                    column: 'gross',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'tare',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'loading_quantity',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
            ],
        },*/
        masterDetail: {
            enabled: true,
            template: masterDetailTemplate
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
        height: 1200,
        showBorders: true,        
        editing: {
            mode: "popup",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                itemType: "group",
                items: [
                    {
                        dataField: "barge_loading_number",
                    },
                    {
                        dataField: "business_unit_id",
                    },
                    {
                        dataField: "despatch_order_id",
                    },
                    {
                        dataField: "barge_rotation_id",
                    },
                    {
                        dataField: "master_list_id",
                    },
                    {
                        dataField: "barge_id",
                    },
                    {
                        dataField: "tug_id",
                    },
                    {
                        dataField: "destination_location",
                    },
                    {
                        dataField: "date_arrived",
                    },
                    {
                        dataField: "date_berthed",
                    },
                    {
                        dataField: "start_loading",
                    },
                    {
                        dataField: "finish_loading",
                    },
                    {
                        dataField: "unberthed_time",
                    },
                    {
                        dataField: "departed_time",
                    },
                    {
                        dataField: "product_id",
                    },
                    {
                        dataField: "analyte_1",
                    },
                    {
                        dataField: "analyte_2",
                    },
                    {
                        dataField: "water_consumption",
                    },
                    {
                        dataField: "chemical_consumption",
                    },
                    {
                        dataField: "draft_scale",
                    },
                    {
                        dataField: "belt_scale",
                    },
                    {
                        dataField: "operator_id",
                    },
                    {
                        dataField: "foreman_id",
                    },
                    {
                        dataField: "captain_id",
                    },
                    {
                        dataField: "gensetUsedTime",
                    },
                    {
                        dataField: "fuelUsed",
                    },
                    {
                        dataField: "kwhUsed",
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
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0) {
                selectedIds = $.map(data, function (value) {
                    return value.id;
                }).join(",");
                $("#dropdown-item-accounting-period").removeClass("disabled");
                $("#dropdown-item-quality-sampling").removeClass("disabled");
                $("#dropdown-delete-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-item-accounting-period").addClass("disabled");
                $("#dropdown-item-quality-sampling").addClass("disabled");
                $("#dropdown-delete-selected").addClass("disabled");
            }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var $links = e.cellElement.find(".dx-link");
                if (e.row.data.approve_status == "APPROVED") {
                   /* $links.filter(".dx-link-edit").remove();

                    var dataGridOptions = $("#grid").dxDataGrid("option");
                   // console.log(dataGridOptions.columns.buttons);
                    var judul;
                    judul = "UnApprove";
                    dataGridOptions.columns[25].buttons[0].text = judul;*/
                }
                else if (e.row.data.approve_status != "APPROVED" || e.row.data.approve_status == null) {
                  /*  var dataGridOptions = $("#grid").dxDataGrid("option");
                   // console.log(dataGridOptions.columns);
                    var judul;
                    judul = "Approve";
                    dataGridOptions.columns[25].buttons[0].text = judul;*/
                }
               /* if (e.row.data.approve_status == "APPROVED") {
                    judul = "UnApprove";
                    dataGridOptions.columns[24].buttons[0].text = judul;
                }
                else if (e.row.data.approve_status != "APPROVED" || e.row.data.approve_status == null) {
                    judul = "Approve";
                    dataGridOptions.columns[24].buttons[0].text = judul;*/
               // }

                
            }
        },
        onEditorPreparing: function (e) {

            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data
                e.editorOptions.onValueChanged = function (innerE) {
                    // Check if a value is selected in "despatch_order_id"
                    if (innerE.value) {
                        /*grid.beginCustomLoading();
                        grid.endCustomLoading();*/
                        // Prevent editing of the "voyage_id" field
                       // e.component.columnOption("despatch_order_id", "allowEditing", false);
                        //grid.cellValue(index, "despatch_order_id", innerE.value);
                    } else {
                        // Allow editing of the "voyage_id" field if no value is selected
                        //e.component.columnOption("despatch_order_id", "allowEditing", true);
                    }
                    let despatchId = innerE.value
                     $.ajax({
                        url: '/api/Sales/SalesInvoice/DespatchOrderDetail?Id=' + despatchId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];

                            grid.beginUpdate();
                            grid.cellValue(index, "barge_id", record.vessel_id)
                            grid.cellValue(index, "despatch_order_id", record.id)
                            //grid.cellValue(index, "tug_id", record.tug_name)
                            grid.cellValue(index, "destination", record.vessel_name)
                            grid.endUpdate();

                         }
                        
                    })
                    standardHandler(e)
                }
            }
           /* if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                //let rowData = e.row.data
                e.editorOptions.onValueChanged = function (innerE) {
                    // Check if a value is selected in "despatch_order_id"
                    if (innerE.value) {
                        grid.beginCustomLoading();
                        // Prevent editing of the "voyage_id" field
                        //e.component.columnOption("barge_rotation_id", "allowEditing", false);
                        grid.cellValue(index, "despatch_order_id", innerE.value);
                        grid.endCustomLoading();
                    } else {
                        // Allow editing of the "voyage_id" field if no value is selected
                        //e.component.columnOption("barge_rotation_id", "allowEditing", true);
                    }
                    let despatchId = innerE.value
                    $.ajax({
                        url: '/api/Sales/SalesInvoice/DespatchOrderDetail?Id=' + despatchId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];

                            grid.beginUpdate();
                            grid.cellValue(index, "barge_id", record.barge_name)
                            grid.cellValue(index, "tug_id", record.tug_name)
                            grid.cellValue(index, "destination", record.vessel_name)
                            grid.endUpdate();
                           
                        }
                    })
                    standardHandler(e)

                }
            }*/
            if (e.parentType === "dataRow" && e.dataField == "barge_rotation_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data
                e.editorOptions.onValueChanged = function (e) {
                    let bargingRotationId = e.value
                    $.ajax({
                        url: '/api/Port/Barging/Rotation/DataDetail?Id=' + bargingRotationId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            grid.cellValue(index, "barge_id", response.transport_id)
                            grid.cellValue(index, "barge_rotation_id", response.barge_rotation_id)
                            grid.cellValue(index, "destination", response.destination_location) 
                            grid.cellValue(index, "barging_type", response.BargingType)
                            grid.cellValue(index, "tug_id", response.tug_id)
                            grid.cellValue(index, "master_list_id", response.destination_location)
                            grid.cellValue(index, "product_id", response.product_id)
                        }
                    })
                    standardHandler(e)
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "product_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                //let rowData = e.row.data
                e.editorOptions.onValueChanged = function (innerE) {
                    let productId = innerE.value
                    $.ajax({
                        url: '/api/Port/SILS/ProductDetail?Id=' + productId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response;

                            grid.beginUpdate();
                            grid.cellValue(index, "product_id", record.id)
                            grid.cellValue(index, "analyte_1", record.ash);
                            grid.cellValue(index, "analyte_2", record.ts);
                            grid.endUpdate();

                        }
                    })
                    standardHandler(e)

                }
            }

            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            if (e.parentType === "dataRow") {
                e.editorOptions.disabled = e.row.data && e.row.data.accounting_period_is_closed;
            };

            if (e.dataField === "transport_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let transportId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Transport/Truck/DataTruck?Id=' + transportId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data;

                            grid.cellValue(index, "tare", record.tare)
                           
                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.dataField === "source_shift_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let shiftId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Shift/Shift/DataDetail?Id=' + shiftId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];

                            grid.cellValue(index, "duration", record.duration)

                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.dataField === "product_id" && e.parentType == "dataRow") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    // Get its value (Id) on value changed
                    let productId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Material/Product/DataDetail?Id=' + productId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];

                            grid.cellValue(index, "duration", record.duration)

                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType == "dataRow" && e.dataField == "gross") {

                let index = e.row.rowIndex;
                let grid = e.component;

                let rowData = e.row.data;
                grossValue = rowData.gross;

                let standardHandler = e.editorOptions.onValueChanged;

                e.editorOptions.onValueChanged = function (e) {
                    grossValue = e.value;

                    grid.beginUpdate();
                    grid.cellValue(index, "gross", grossValue);
                    grid.cellValue(index, "loading_quantity", grossValue - tareValue);
                    grid.cellValue(index, "unloading_quantity", grossValue - tareValue);
                    grid.endUpdate();

                    standardHandler(e);
                }
            }
            if (e.parentType == "dataRow" && e.dataField == "tare") {

                let index = e.row.rowIndex;
                let grid = e.component;

                let rowData = e.row.data;
                tareValue = rowData.tare;

                let standardHandler = e.editorOptions.onValueChanged;

                e.editorOptions.onValueChanged = function (e) {
                    tareValue = e.value;

                    grid.beginUpdate();
                    grid.cellValue(index, "tare", tareValue);
                    grid.cellValue(index, "loading_quantity", grossValue - tareValue);
                    grid.cellValue(index, "unloading_quantity", grossValue - tareValue);
                    grid.endUpdate();

                    standardHandler(e);
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
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), entityName + '.xlsx');
                });
            });
            e.cancel = true;
        }
    });
    let approvalPopupOptions = {
        title: "Approval Information",
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
                        //itemType: "label",
                        label: {
                            text: "Are Your Sure?",
                        },
                        colSpan: 2
                        /*dataField: "comment",
                        label: {
                            text: "Comment",
                        },
                        colSpan: 2*/
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
                                ////console.log(data);
                                let formData = new FormData();
                                //formData.append("key", data.id);
                                formData.append("key", data.id);

                                formData.append("values", JSON.stringify(data));

                                saveApprovalForm(formData);
                            }
                        }
                    },

                ],
                onInitialized: () => {
                    $.ajax({
                        type: "GET",
                        url: "/api/Port/SILS/GetSIlS/" + encodeURIComponent(salesInvoiceApprovalData.id),
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            // Update form formData with response from api
                            if (response) {
                                approvalForm.dxForm("instance").option("formData", response)
                            }
                        }
                    })
                }
            })

            return approvalForm;
        }
    }
    var approvalPopup = $("#approval-popup").dxPopup(approvalPopupOptions).dxPopup("instance")

    const showApprovalPopup = function () {
       approvalPopup.option("contentTemplate", approvalPopupOptions.contentTemplate.bind(this));
        approvalPopup.show()
    }
    const saveApprovalForm = (formData) => {
        $.ajax({
            type: "POST",
            url: "/api/PORT/SILS/ApproveUnapprove",
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
                    $("#grid").dxDataGrid("getDataSource").reload();
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            approvalPopup.hide();
            Swal.fire("Failed !", jqXHR.responseText, "error");
        });
    }
    $('#btnApplyAccountingPeriod').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.category = "production";
            payload.id = $('#accounting_period_id').val();
            payload.production_ids = selectedIds;

            $('#btnApplyAccountingPeriod')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Applying ...');

            $.ajax({
                url: "/api/Accounting/AccountingPeriod/ApplyToTransactions",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                //console.log(result);
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Success");
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnApplyAccountingPeriod').html('Apply');
            });
        }
    });

    $('#btnApplyQualitySampling').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.category = "production";
            payload.id = $('#quality_sampling_id').val();
            payload.production_ids = selectedIds;
            
            $('#btnApplyQualitySampling')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Applying ...');

            $.ajax({
                url: "/api/StockpileManagement/QualitySampling/ApplyToTransactions",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(payload),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                //console.log(result);
                if (result) {
                    if (result.success) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Success");
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            }).always(function () {
                $('#btnApplyQualitySampling').html('Apply');
            });
        }
    });

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

    $('#dropdown-download-template').on('click', function () {
        var urlTemplate = document.getElementById('url-download-template').value;

        $('#text-download-template')
            .html('Downloading ... <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');

        window.location.href = urlTemplate;

        setTimeout(function () {
            $('#text-download-template').html('Download Template');
        }, 30000);
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
                url: "/api/Port/Production/UploadDocument",
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

    function masterDetailTemplate(_, masterDetailOptions) {
        var masterDat = masterDetailOptions.data
            return $("<div>").dxTabPanel({
                items: [
                    {
                        title: "Detail",
                        template: createDayDetailTabTemplate(masterDetailOptions.data)
                    },
                ]

            });
       
    }

    function createDayDetailTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "SILS";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/GetItemsById?Id=" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/InsertItemData",
                        updateUrl: urlDetail + "/UpdateItemData",
                        deleteUrl: urlDetail + "/DeleteItemData",

                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    remoteOperations: true,
                    allowColumnResizing: true,
                    columnMinWidth: 100,
                    columnResizingMode: "widget",
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        {
                            dataField: "crew_id",
                            dataType: "string",
                            caption: "Crew",
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
                                        filter: ["item_group", "=", "crew-nplct"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }
                        },
                        {
                            dataField: "shift_id",
                            dataType: "string",
                            caption: "Shift",
                            //placeholder: "Autofill",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    loadUrl: url + "/ShiftIdLookup",
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
                            dataField: "source_id",
                            dataType: "string",
                            caption: "Source",
                            //allowEditing: false
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    loadUrl: url + "/SourceIdLookup",
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
                            dataField: "loader_id",
                            dataType: "string",
                            caption: "Loader",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    loadUrl: "/api/Port/SILS/EquipmentIdLookup",
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
                            dataField: "start_flow_time",
                            dataType: "datetime",
                            caption: "Start Flow Time",
                            format: "yyyy-MM-dd HH:mm:ss",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "stop_flow_time",
                            dataType: "datetime",
                            caption: "Stop Flow Time",
                            format: "yyyy-MM-dd HH:mm:ss",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "total",
                            dataType: "number",
                            caption: "Total",
                            allowEditing: false,
                            format: {
                                type: "fixedPoint",
                                precision: 2
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint",
                                        precision: 2
                                    }
                                },
                            },
                        },
                        {
                            dataField: "down_time_from",
                            dataType: "datetime",
                            caption: "Down Time From",
                            format: "yyyy-MM-dd HH:mm:ss",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "down_time_to",
                            dataType: "datetime",
                            caption: "Down Time To",
                            format: "yyyy-MM-dd HH:mm:ss",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "total_down_time",
                            dataType: "number",
                            caption: "Total Down Time",
                            allowEditing: false,
                            format: {
                                type: "fixedPoint",
                                precision: 2
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint",
                                        precision: 2
                                    }
                                },
                            },
                        },
                        {
                            dataField: "progressive_total",
                            dataType: "number",
                            caption: "Progressive Total",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "progress",
                            dataType: "number",
                            caption: "Progress",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "category_id",
                            dataType: "text",
                            caption: "Category",
                            width: "100px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/CategoryIdLookup",
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
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }
                        },
                        {
                            dataField: "location_id",
                            dataType: "text",
                            caption: "Location",
                            width: "100px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Mining/CHLS/AllDestinationIdLookup",
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
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }
                        },
                        {
                            dataField: "type_id",
                            dataType: "text",
                            caption: "Type",
                            width: "200px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/AllIdLookup",
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
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }
                        },
                        /*{
                            dataField: "flow_meter",
                            dataType: "number",
                            caption: "Flow Meter",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },*/
                        {
                            dataField: "description",
                            dataType: "string",
                            caption: "Description",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
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
                        enabled: false,
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
                        mode: 'batch',
                        allowUpdating: true,
                        allowAdding: true,
                        allowDeleting: true
                    },
                    onEditorPreparing: function (e) {
                        if (e.parentType === "dataRow" && e.dataField === "type_id") {
                            // Access the value of the selected category_id
                            var Id = e.row.data.category_id;

                            // Define the dataSource for the type_id lookup based on the selected category_id
                            var typeDataSource = {
                                store: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/TypeIdLookup?Id=" + Id, // Pass the selected category_id to the controller
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                })
                            };

                            // Set the dataSource for the type_id lookup
                            e.editorOptions.dataSource = typeDataSource;
                        }
                        if (e.parentType === "dataRow" && e.dataField === "location_id") {
                            // Access the value of the selected category_id
                            var Id = e.row.data.category_id;

                            // Define the dataSource for the type_id lookup based on the selected category_id 
                            var typeDataSource = {
                                store: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Mining/CHLS/FilteredDestinationIdLookup?Id=" + Id, // Pass the selected category_id to the controller
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                })
                            };

                            // Set the dataSource for the type_id lookup
                            e.editorOptions.dataSource = typeDataSource;
                        }

                       /* if (e.parentType === "dataRow" && e.dataField === "category_id") {
                            // Di sini Anda dapat memindahkan kode dari event onValueChanged
                            // yang sebelumnya ada di field category_id.
                            // Anda bisa tetap menggunakan e.value untuk mendapatkan nilai yang dipilih.
                            var selectedCategoryId = e.value;

                            // Mendapatkan is_problem_productivity dari data yang dipilih
                            var selectedCategory = e.component.lookupData.filter(function (item) {
                                return item.value === selectedCategoryId;
                            })[0];

                            // Mendefinisikan dataSource untuk dropdown "Type" berdasarkan kondisi is_problem_productivity
                            var typeDataSource;
                            if (selectedCategory && selectedCategory.is_problem_productivity === true) {
                                // Kondisi ketika is_problem_productivity == true
                                typeDataSource = function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/Category",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        })
                                    };
                                };
                            } else {
                                // Kondisi ketika is_problem_productivity == false
                                typeDataSource = function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/Type",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        })
                                    };
                                };
                            }

                            // Mengatur dataSource untuk dropdown "Type"
                            e.editorOptions.dataSource = typeDataSource;
                        }*/
                        if (e.parentType === 'searchPanel') {
                            e.editorOptions.onValueChanged = function (arg) {
                                if (arg.value.length == 0 || arg.value.length > 2) {
                                    e.component.searchByText(arg.value);
                                }
                            }
                        }
                        // Set onValueChanged for sales_charge_id
                        if (e.parentType === "dataRow" && e.dataField == "transport_id") {

                            let standardHandler = e.editorOptions.onValueChanged
                            let index = e.row.rowIndex
                            let grid = e.component
                            let rowData = e.row.data

                            e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                                // Get its value (Id) on value changed
                                let transportId = e.value

                                // Get another data from API after getting the Id
                                $.ajax({
                                    url: '/api/Transport/Truck/DataDetail?Id=' + transportId,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        let resultData = response.data[0]
                                        //console.log(salesCharge)

                                        // Set its corresponded field's value
                                        grid.cellValue(index, "truck_factor", resultData.truck_factor)
                                        grid.cellValue(index, "density", resultData.density)
                                    }
                                })

                                standardHandler(e) // Calling the standard handler to save the edited value
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
                    onInitNewRow: function (e) {
                        e.data.sils_id = currentRecord.id;
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