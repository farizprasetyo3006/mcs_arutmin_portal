$(function () {

    var token = $.cookie("Token");
    var areaName = "Port";
    var entityName = "SILSNPLCT";
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
        columnMinWidth: 100,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "transaction_number",
                dataType: "string",
                caption: "Transaction Number",
                allowEditing: false,
                //width: "140px",
                formItem: {
                    colSpan: 2,
                },
                //sortOrder: "asc"
            },
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
                            console.log(token);
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text",
                }
            },
            {
                dataField: "despatch_order_id",
                dataType: "string",
                caption: "Shipping Order",
               // width: "100px",
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
                dataField: "process_flow_id",
                dataType: "text",
                caption: "Process Flow",
                //width: "100px",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/ProcessFlowIdLookup", //ganti ke process flow lookup
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
                dataField: "source_location_id",
                dataType: "text",
                caption: "Source Location",
               // width: "100px",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/SourceIdLookup", //ganti ke source lookup
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
                dataField: "vessel_id",
                dataType: "text",
                caption: "Vessel",
               // width: "100px",
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
                dataField: "product_brand", 
                dataType: "string",
                caption: "Product Brand",
                allowEditing: true,
               // width: "140px",
                formItem: {
                    colSpan: 2,
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Material/Product/ProductIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                }, 
            },
            {
                dataField: "draft_survey_number",
                dataType: "string",
                caption: "Draft Survey Number",
                allowEditing: true,
               // width: "140px",
                formItem: {
                    colSpan: 2,
                },
            },
            {
                dataField: "date_arrived",
                dataType: "datetime",
                caption: "Date Arrived",
               //width: "140px",
                format: "yyyy-MM-dd HH:mm",
                validationRules: [{
                    type: "required",
                    message: "This Field is Required",
                }],
            },
            {
                dataField: "date_berthed",
                dataType: "datetime",
                caption: "Date Berthed",
               // width: "140px",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "start_loading",
                dataType: "datetime",
                caption: "Start Loading",
               // width: "140px",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "unberthed_time",
                dataType: "datetime",
                caption: "Unberthed Time",
               // width: "140px",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "finish_loading",
                dataType: "datetime",
                caption: "Finish Loading",
              //  width: "140px",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "departed_time",
                dataType: "datetime",
                caption: "Departed",
               // width: "140px",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "bw_start",
                dataType: "number",
                caption: "Total BW CV02 Start",
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
                dataField: "bw_end",
                dataType: "number",
                caption: "Total BW CV2 End",
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
                dataField: "tonnage_scale",
                dataType: "number",
                caption: "Tonnage Scale",
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
                dataField: "tonnage_draft",
                dataType: "number",
                caption: "Tonnage Draft",
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
                dataField: "total_on_board",
                dataType: "number",
                caption: "Total On Board",
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
                dataField: "gross_loading_time",
                dataType: "number",
                caption: "Gross Loading Time",
                visible: false,
                format: {
                    type: "fixedPoint",
                    //precision: 3
                },
                formItem: {
                    visible: true,
                }
            },
            {
                dataField: "nett_loading_time",
                dataType: "number",
                caption: "Nett Loading Time",
                visible: false,
                format: {
                    type: "fixedPoint",
                    //precision: 3
                },
                formItem: {
                    visible: false,
                }
            },
            {
                dataField: "gross_loading_rate",
                dataType: "number",
                caption: "Gross Loading Rate",
                visible: false,
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    visible: false,
                }
            },
            {
                dataField: "nett_loading_rate",
                dataType: "number",
                caption: "Nett Loading Rate",
                visible: false,
                format: {
                    type: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    /*editorType: "dxNumberBox",
                    editorOptions: {
                        format: {
                            type: "fixedPoint"
                        }
                    },*/
                    visible: false,
                }
            },
            {
                dataField: "supervisor",
                dataType: "string",
                caption: "TS",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "at_anchorage",
                dataType: "string",
                caption: "At Anchorage",
                allowEditing: true,
                visible: false
            },
            {
                dataField: "description",
                dataType: "string",
                caption: "Description",
                allowEditing: true,
                visible: false
            },
            {
                dataField: "despatch_demmurage",
                dataType: "string",
                caption: "Despatch Demmurage",
                allowEditing: true,
                visible: false
            },
            {
                type: "buttons",
                buttons: ["edit", "delete"],
                showInColumnChooser: false
            }
        ],
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
                colCount: 1,
                items: [{
                    itemType: "group",
                    caption: "Date Time",
                    colCount: 2,
                    items: [
                        {
                            dataField: "transaction_number",
                        },
                        {
                            dataField: "business_unit_id",
                        },
                        {
                            dataField: "despatch_order_id",
                        },
                        {
                            dataField: "process_flow_id",
                        },
                        {
                            dataField: "source_location_id",
                        },
                        {
                            dataField: "vessel_id",
                        },
                        {
                            dataField: "product_brand",
                        },
                        {
                            dataField: "draft_survey_number",
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
                    ],
                },
                {
                    itemType: "group",
                    caption: "Measurment",
                    colCount: 2,
                    items: [
                        {
                            dataField: "bw_start",
                        },
                        {
                            dataField: "bw_end",
                        },
                        {
                            dataField: "tonnage_scale",
                        },
                        {
                            dataField: "tonnage_draft",
                        },
                        {
                            dataField: "total_on_board",
                        },
                    ]
                },
                {
                    itemType: "group",
                    caption: "Performance",
                    colCount: 2,
                    items: [
                        {
                            dataField: "gross_loading_time",
                        },
                        {
                            dataField: "nett_loading_time",
                        },
                        {
                            dataField: "gross_loading_rate",
                        },
                        {
                            dataField: "nett_loading_rate",
                        },
                    ]
                },
                {
                    itemType: "group",
                    caption: "Additional Information",
                    colCount: 2,
                    items: [
                        {
                            dataField: "supervisor",
                        },
                        {
                            dataField: "at_anchorage",
                        },
                        {
                            dataField: "description",
                        },
                        {
                            dataField: "despatch_demmurage",
                        },
                    ]
                }],
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
                $("#dropdown-download-selected").removeClass("disabled");
                $("#dropdown-delete-selected").removeClass("disabled");
            }
            else {

                $("#dropdown-download-selected").addClass("disabled");
                $("#dropdown-delete-selected").addClass("disabled");
            }
        },
        /*onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var $links = e.cellElement.find(".dx-link");
                if (e.row.data.approve_status == "APPROVED")
                    $links.filter(".dx-link-edit").remove();

                var dataGridOptions = $("#grid").dxDataGrid("option");
                ////console.log(dataGridOptions.columns);
                var judul;
                if (e.row.data.approve_status == "APPROVED")
                    judul = "UnApprove";
                else if (e.row.data.approve_status != "APPROVED" || e.row.data.approve_status == null)
                    judul = "Approve";

                dataGridOptions.columns[24].buttons[0].text = judul;
            }
        },*/
        onEditorPreparing: function (e) {
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                //let rowData = e.row.data
                e.editorOptions.onValueChanged = function (innerE) {
                    // Check if a value is selected in "despatch_order_id"
                   /* if (innerE.value) {
                        grid.beginCustomLoading();
                        // Prevent editing of the "voyage_id" field
                        e.component.columnOption("barge_rotation_id", "allowEditing", false);
                        grid.cellValue(index, "despatch_order_id", innerE.value);
                        grid.endCustomLoading();
                    } else {
                        // Allow editing of the "voyage_id" field if no value is selected
                        e.component.columnOption("barge_rotation_id", "allowEditing", true);
                    }*/
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
                            grid.cellValue(index, "vessel_id", record.vessel_id)
                            grid.cellValue(index, "despatch_order_id", despatchId)
                            grid.cellValue(index, "tug_id", record.tug_name)
                            grid.cellValue(index, "destination", record.vessel_name)
                            grid.cellValue(index, "product_brand", record.product_id)
                            grid.endUpdate();
                           
                        }
                    })
                    standardHandler(e)

                }
            }
            /*if (e.parentType === "dataRow" && e.dataField == "barge_rotation_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                //let rowData = e.row.data
                e.editorOptions.onValueChanged = function (innerE) {
                    // Check if a value is selected in "despatch_order_id"
                    *//*if (innerE.value) {
                        grid.beginCustomLoading();
                        // Prevent editing of the "voyage_id" field
                        e.component.columnOption("despatch_order_id", "allowEditing", false);
                        grid.cellValue(index, "barge_rotation_id", innerE.value);
                        grid.endCustomLoading();
                    } else {
                        // Allow editing of the "voyage_id" field if no value is selected
                        e.component.columnOption("despatch_order_id", "allowEditing", true);
                    }*//*
                    let voyageId = e.value
                    $.ajax({
                        url: '/api/Port/SILS/VoyageDetail?Id=' + voyageId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];

                            grid.cellValue(index, "barge_id", record.transport_id)
                            //grid.cellValue(index, "tug_id", record.tug_name)
                            grid.cellValue(index, "destination", record.destination_location)

                        }
                    })
                    standardHandler(e)
                }
            }*/
           /* if (e.parentType === "dataRow" && e.dataField == "product_id") {
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
            }*/

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
    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');

            $.ajax({
                url: "/Port/SILSNPLCT/ExcelExport",
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
                    a.download = "SILS_NPLCT_Loading_Vessel.xlsx"; // Set the appropriate file name here
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
                url: "/api/Port/SILSNPLCT/UploadDocument",
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
                        template: DetailTabTemplate(masterDetailOptions.data)
                    },
                ]

            });
       
    }

    function subDetailTemplate(_, masterDetailOptions) {
        var masterDat = masterDetailOptions.data
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Sub-Detail",
                    template: subDetailTabTemplate(masterDetailOptions.data)
                },
            ]

        });

    }

    function DetailTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "SILSNPLCT";
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
                            dataField: "date",
                            dataType: "datetime",
                            caption: "Date",
                          //  width: "140px",
                            format: "yyyy-MM-dd HH:mm"
                        },
                        {
                            dataField: "total_flow_time",
                            dataType: "number",
                            caption: "Total Flow Time",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                visible: false,
                            }
                        },
                        {
                            dataField: "total_down_time",
                            dataType: "number",
                            caption: "Total Down Time",
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                visible: false,
                            }
                        },
                        {
                            dataField: "gross_loading_time",
                            dataType: "number",
                            caption: "Gross Loading Time",
                         //   width: "7%",
                            format: {
                                type: "fixedPoint",
                                //precision: 3
                            },
                            formItem: {
                                visible: false,
                            }
                        },
                        {
                            dataField: "nett_loading_time",
                            dataType: "number",
                            caption: "Nett Loading Time",
                          //  width: "7%",
                            format: {
                                type: "fixedPoint",
                                //precision: 3
                            },
                            formItem: {
                                visible: false,
                            }
                        },
                        {
                            dataField: "gross_loading_rate",
                            dataType: "number",
                            caption: "Gross Loading Rate",
                           // width: "7%",
                            format: {
                                type: "fixedPoint",
                                precision: 3
                            },
                            formItem: {
                                visible: false,
                            }
                        },
                        {
                            dataField: "nett_loading_rate",
                            dataType: "number",
                            caption: "Nett Loading Rate",
                           // width: "7%",
                            format: {
                                type: "fixedPoint",
                                precision: 3
                            },
                            formItem: {
                                /*editorType: "dxNumberBox",
                                editorOptions: {
                                    format: {
                                        type: "fixedPoint"
                                    }
                                },*/
                                visible: false,
                            }
                        },
                        {
                            dataField: "total_on_board",
                            dataType: "number",
                            caption: "Nett Loading Rate",
                          //  width: "5%",
                            format: {
                                type: "fixedPoint",
                                precision: 3
                            },
                            formItem: {
                                /*editorType: "dxNumberBox",
                                editorOptions: {
                                    format: {
                                        type: "fixedPoint"
                                    }
                                },*/
                                visible: false,
                            }
                        },
                        {
                            dataField: "total_shift",
                            dataType: "number",
                            caption: "Nett Loading Rate",
                           // width: "5%",
                            format: {
                                type: "fixedPoint",
                                precision: 3
                            },
                            formItem: {
                                /*editorType: "dxNumberBox",
                                editorOptions: {
                                    format: {
                                        type: "fixedPoint"
                                    }
                                },*/
                                visible: false,
                            }
                        },
                        {
                            dataField: "operator1_id",
                            dataType: "string",
                            caption: "Operator 1",
                          //  width: "5%",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    loadUrl: url + "/EmployeeIdLookup",
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
                            dataField: "operator2_id",
                            dataType: "string",
                            caption: "Operator 2",
                           // width: "5%",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    loadUrl: url + "/EmployeeIdLookup",
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
                            dataField: "foreman_id",
                            dataType: "text",
                            caption: "Foreman Name",
                           // width: "100px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/EmployeeIdLookup",
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
                            
                    ],
                    masterDetail: {
                        enabled: true,
                        template: subDetailTemplate
                    },
                    filterRow: {
                        visible: false
                    },
                    headerFilter: {
                        visible: false
                    },
                    groupPanel: {
                        visible: false
                    },
                    searchPanel: {
                        visible: true,
                        width: 240,
                        placeholder: "Search..."
                    },
                    filterPanel: {
                        visible: false
                    },
                    filterBuilderPopup: {
                        position: { of: window, at: "top", my: "top", offset: { y: 10 } },
                    },
                    columnChooser: {
                        enabled: false,
                        mode: "select"
                    },
                    paging: {
                        enabled: false,
                        pageSize: 1000
                    },
                    pager: {
                        allowedPageSizes: [10, 20, 50, 100],
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        showInfo: true,
                        visible: false
                    },
                    showBorders: true,
                    editing: {
                        mode: 'popup',
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
                        e.data.sils_nplct_id = currentRecord.id;
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

    function subDetailTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "SILSNPLCT";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/subDetailById?Id=" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/InsertSubDetailData",
                        updateUrl: urlDetail + "/UpdateSubDetailData",
                        deleteUrl: urlDetail + "/DeleteSubDetailData",

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
                            dataField: "stockpile_1",
                            dataType: "text",
                            caption: "Stockpile 1",
                          //  width: "8%",
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Location/StockpileLocation/StockpileLocationIdLookup",
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
                                formItem: {
                                    editorOptions: {
                                        showClearButton: true
                                    }
                                },
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }
                        },
                        {
                            dataField: "cv05",
                            dataType: "string",
                            caption: "CV05",
                            allowEditing: true,
                            visible: true,
                           // width: "4%",
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/EquipmentCVIdLookup",
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
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }

                        },
                        {
                            dataField: "cv06",
                            dataType: "string",
                            caption: "CV06",
                            allowEditing: true,
                            visible: true,
                           // width: "4%",
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/EquipmentCVIdLookup",
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
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }

                        },
                        {
                            dataField: "hatch_no",
                            dataType: "number",
                            caption: "Hatch No",
                            allowEditing: true,
                            visible: true,
                           // width: "4%",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Port/SILSNPLCT/MasterListIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: ["item_group", "=", "hatch-vessel"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            },
                        },
                        {
                            dataField: "hatch_this_run",
                            dataType: "number",
                            caption: "Hatch This Run",
                            allowEditing: false
                           // width: "4%",
                        },
                        {
                            dataField: "hatch_total1",
                            dataType: "number",
                            caption: "Hatch Total",
                            allowEditing: false,
                           // width: "4%",
                            /*format: {
                                type: "fixedPoint"
                            },*/
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
                            dataField: "total_hatch_this_run",
                            dataType: "number",
                            caption: "Progressive Total",
                          //  width: "4%",
                            /*format: {
                                type: "fixedPoint"
                            },*/
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
                            dataField: "start_datetime",
                            dataType: "datetime",
                            caption: "Start Time",
                          //  width: "140px",
                            format: "yyyy-MM-dd HH:mm"
                        },
                        {
                            dataField: "stop_datetime",
                            dataType: "datetime",
                            caption: "Stop Time",
                          //  width: "140px",
                            format: "yyyy-MM-dd HH:mm"
                        },
                        {
                            dataField: "total_time",
                            dataType: "number",
                            caption: "Total Time",
                            allowEditing: false,
                            format: {
                                type: "fixedPoint",
                                precision: 3
                            },
                         //   width: "2.5%"
                            /*format: {
                                type: "fixedPoint"
                            },*/
                            /*formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }*/
                        },
                        {
                            dataField: "category_id",
                            dataType: "text",
                            caption: "Category",
                           // width: "120px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Port/SILSUnloading/CategoryIdLookup",
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
                           // width: "120px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Port/SILSUnloading/AllIdLookup",
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
                            //width: "160px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: "/api/Port/SILSUnloading/LocationIdLookup",
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
                            dataField: "product_id",
                            dataType: "text",
                            caption: "Product Name",
                           // width: "180px",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/ProductIdLookup",
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
                            dataField: "description",
                            dataType: "string",
                            caption: "Description",
                            allowEditing: true,
                            visible: true,
                            //width: "7%"
                        },
                        {
                            dataField: "operator_id",
                            dataType: "string",
                            caption: "Operator",
                            //width: "4%",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    loadUrl: url + "/EmployeeIdLookup",
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
                    ],
                    /*masterDetail: {
                        enabled: true,
                        template: subDetailTemplate
                    },*/
                    filterRow: {
                        visible: false
                    },
                    headerFilter: {
                        visible: false
                    },
                    groupPanel: {
                        visible: false
                    },
                    searchPanel: {
                        visible: true,
                        width: 240,
                        placeholder: "Search..."
                    },
                    filterPanel: {
                        visible: false
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
                        pageSize: 1000
                    },
                    pager: {
                        allowedPageSizes: [10, 20, 50, 100],
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        showInfo: true,
                        visible: false
                    },
                    showBorders: true,
                    editing: {
                        mode: 'batch',
                        allowUpdating: true,
                        allowAdding: true,
                        allowDeleting: true
                    },
                    onEditorPreparing: function (e) {
                        /*if (e.parentType === "dataRow" && e.dataField === "type_id") {
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
                        }*/

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
                        e.data.sils_nplct_detail_id = currentRecord.id;
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