$(function () {

    var token = $.cookie("Token");
    var areaName = "Sales";
    var entityName = "Royalty";
    var royaltyUrl = "/api/" + areaName + "/Royalty/" + entityName;
    var despatchOrderId = "";
    var shipmentPlanETA = "";
    var billLadingDate = "";
    var vesselId = "";
    var statusId = "";
    /**
     * =========
     *  Royalty
     * =========
     */

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: royaltyUrl + "/DataGrid",
            insertUrl: royaltyUrl + "/InsertData",
            updateUrl: royaltyUrl + "/UpdateData",
            deleteUrl: royaltyUrl + "/DeleteData",
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
            //    dataField: "royalty_code",
            //    dataType: "string",
            //    caption: "Code",
            //    validationRules: [{
            //        type: "required",
            //        message: "The field is required."
            //    }]
            //},
            {
                dataField: "despatch_order_id",
                dataType: "string",
                caption: "Shipping Order",
                validationRules: [{
                    type: "required",
                    message: "The Shipping Order is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/DespatchOrder/DespatchOrderIdLookup",
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
                    rowData.despatch_order_id = value;
                    rowData.delivery_term = null;
                    rowData.buyer = null;
                    rowData.address = null;
                    rowData.loading_port = null;
                    rowData.discharge_port = null;
                    rowData.vessel = null;
                    rowData.imo_number = null;
                    rowData.vessel_flag = null;
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
                dataField: "refreshShippingOrderButton",
                dataType: "string",
                caption: "Refresh Shipment Plan",
                visible: false,
                allowFiltering: false,
                allowSearch: false,
                showInColumnChooser: false
            },
            {
                dataField: "royalty_reference",
                dataType: "string",
                caption: "Reference",
            },
            {
                dataField: "royalty_date",
                dataType: "date",
                caption: "Request Date",
            },
            {
                dataField: "nama_pemegang_et",
                dataType: "string",
                caption: "Nama Pemegang ET",
                editorOptions: {
                    readOnly: true
                },
                visible: false
            },
            {
                dataField: "bl_date",
                dataType: "datetime",
                caption: "BL Date",
                format: "MM/dd/yyyy",
                visible: true,
            },
            {
                dataField: "status_id",
                dataType: "string",
                caption: "Status",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=royalty-status",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                setCellValue: function (rowData, value) {
                    rowData.status_id = value;
                }
            },
            {
                dataField: "nomor_sk_iupk",
                dataType: "string",
                caption: "Nomor SK IUPK",
                visible: false,
            },
            {
                dataField: "nomor_et",
                dataType: "string",
                caption: "Nomor ET-Batubara dan Produk",
                visible: false,
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
                                loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=royalty-destination",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "nomor_invoice",
                dataType: "string",
                caption: "Invoice Number",
            },
            {
                dataField: "delivery_term",
                dataType: "string",
                caption: "Delivery Term",
                visible: false,
            },
            {
                dataField: "currency_exchange_id",
                dataType: "number",
                caption: "Exchange Rate",
                /* lookup: {
                     dataSource: function (options) {
                         return {
                             store: DevExpress.data.AspNet.createStore({
                                 key: "value",
                                 loadUrl: "/api/General/CurrencyExchange/CurrencyExchangeIdLookup",
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
                 calculateSortValue: function (data) {
                     var value = this.calculateCellValue(data);
                     return this.lookup.calculateCellValue(value);
                 },
                 formItem: {
                     editorOptions: {
                         showClearButton: true
                     }
                 },*/
                visible: false
            },
            {
                dataField: "destination_country",
                dataType: "string",
                caption: "Destination Country",
                visible: false,
            },
            {
                dataField: "status_buyer_id",
                dataType: "string",
                caption: "Status Buyer",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=royalty-status-buyer",
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
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "buyer",
                dataType: "string",
                caption: "Buyer",
            },
            {
                dataField: "address",
                dataType: "string",
                caption: "Address",
                visible: false,
            },
            {
                dataField: "loading_port",
                dataType: "string",
                caption: "Loading Port",
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Planning/ShippingProgram/portIdLookup",
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
                dataField: "discharge_port",
                dataType: "string",
                caption: "Discharge Port",
                visible: false,
            },
            //{
            //    dataField: "vessel",
            //    dataType: "string",
            //    caption: "Vessel",
            //    visible: false,
            //},
            {
                dataField: "vessel",
                dataType: "string",
                caption: "Vessel",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Transport/Vessel/VesselIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "barge_id",
                dataType: "string",
                caption: "Barge",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Transport/Barge/BargeIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "imo_number",
                dataType: "string",
                caption: "IMO Number",
                visible: false,
            },
            {
                dataField: "tpk_barge",
                dataType: "string",
                caption: "TPK Barge",
                visible: false,
            },
            {
                dataField: "vessel_flag",
                dataType: "string",
                caption: "Vessel Flag",
                visible: false,
            },
            {
                dataField: "barge_flag",
                dataType: "string",
                caption: "Barge Flag",
                visible: false,
            },
            {
                dataField: "coal_origin_id",
                dataType: "string",
                caption: "Coal Origin",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/SystemAdministration/BusinessUnit/BusinessUnitIdLookupNoFilter",
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
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "tug_id",
                dataType: "string",
                caption: "Tug",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Transport/Tug/TugIdLookup",
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
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "permit_location",
                dataType: "string",
                caption: "Permit Location",
                visible: false,
            },
            {
                dataField: "tpk_tug",
                dataType: "string",
                caption: "TPK Tug",
                visible: false
            },
            {
                dataField: "notes",
                dataType: "string",
                caption: "Notes",
                visible: false
            },
            {
                dataField: "tug_flag",
                dataType: "string",
                caption: "Tug Flag",
                visible: false
            },
            {
                dataField: "btnDetail",
                caption: "Detail",
                type: "buttons",
                width: 150,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "See Contract Terms",
                    text: "Open Detail",
                    onClick: function (e) {
                        royaltyId = e.row.data.id
                        window.location = "/Sales/Royalty/Detail/" + royaltyId
                    }
                }]
            },
            {
                type: "buttons",
                buttons: ["edit", "delete"]
            }
        ],
        onEditorPreparing: function (e) {
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                let rowData = e.row.data;

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                    despatchOrderId = e.value;
                    grid.beginCustomLoading();
                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Sales/DespatchOrder/DataDetail?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let despatchOrderData = response.data;
                            vesselId = despatchOrderData.vessel_id;
                            if (despatchOrderData) {
                                grid.beginUpdate();
                                if (despatchOrderData.delivery_term_name.includes("BARGE")) {
                                    grid.cellValue(index, "barge_id", despatchOrderData.vessel_id);
                                }
                                else {
                                    grid.cellValue(index, "vessel", despatchOrderData.vessel_id);
                                }
                                grid.cellValue(index, "delivery_term", despatchOrderData.delivery_term_name);
                                if (despatchOrderData.delivery_term_name.includes("BARGE")) {
                                    grid.cellValue(index, "barge_id", despatchOrderData.vessel_id);
                                } else {
                                    grid.cellValue(index, "vessel", despatchOrderData.vessel_id);
                                }
                                grid.cellValue(index, "royalty_reference", despatchOrderData.royalty_number);
                                grid.cellValue(index, "delivery_term", despatchOrderData.delivery_term_name);
                                grid.cellValue(index, "buyer", despatchOrderData.customer_name);
                                grid.cellValue(index, "address", despatchOrderData.customer_address);
                                grid.cellValue(index, "loading_port", despatchOrderData.loading_port);
                                grid.cellValue(index, "discharge_port", despatchOrderData.discharge_port);
                                grid.cellValue(index, "imo_number", despatchOrderData.imo_number);
                                grid.cellValue(index, "vessel_flag", despatchOrderData.flag);
                                grid.cellValue(index, "nomor_invoice", despatchOrderData.invoice_number);
                                shipmentPlanETA = despatchOrderData.shipment_plan_eta;
                                billLadingDate = despatchOrderData.draft_survey_bill_lading_date;

                                $.ajax({
                                    url: '/api/Transport/Vessel/DataDetail?Id=' + vesselId,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        let vesselData = response;
                                        if (vesselData) {
                                            grid.beginUpdate();
                                            grid.cellValue(index, "imo_number", vesselData.imo_number);
                                            grid.cellValue(index, "vessel_flag", vesselData.flag);
                                            $.ajax({
                                                url: '/api/Sales/Royalty/Royalty/GetReferenceOnStatus?Id=' + despatchOrderId,
                                                type: 'GET',
                                                contentType: "application/json",
                                                beforeSend: function (xhr) {
                                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                                },
                                                success: function (response) {
                                                    let despatchOrderData = response.data;
                                                    statusId = despatchOrderData[0];
                                                    if (despatchOrderData) {
                                                        grid.beginUpdate();
                                                        grid.cellValue(index, "status_id", despatchOrderData[0]);
                                                        grid.cellValue(index, "delivery_term", despatchOrderData[1]);

                                                        $.ajax({
                                                            url: "/api/General/MasterList/DataDetail?Id=" + statusId,
                                                            type: 'GET',
                                                            contentType: "application/json",
                                                            beforeSend: function (xhr) {
                                                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                                                            },
                                                            success: function (response) {
                                                                statusCode = response.data[0].item_in_coding;
                                                                if (statusCode == 'AWL') {
                                                                    grid.cellValue(index, "bl_date", shipmentPlanETA);
                                                                }
                                                                else {
                                                                    grid.cellValue(index, "bl_date", billLadingDate);
                                                                }
                                                            }
                                                        });
                                                        grid.endUpdate();
                                                    }
                                                }
                                            })
                                            grid.endUpdate();
                                        }
                                    }
                                })
                                grid.endUpdate();
                            }
                        }
                    })
                    setTimeout(() => {
                        grid.endCustomLoading();
                    }, 500);

                    standardHandler(e); // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "refreshShippingOrderButton") {

                if (e.row.data.despatch_order_id) {

                    let standardHandler = e.editorOptions.onValueChanged
                    let index = e.row.rowIndex
                    let grid = e.component

                    let despatchOrderId = e.row.data.despatch_order_id

                    e.editorOptions.onClick = function (e) {

                        $.ajax({
                            url: '/api/Sales/DespatchOrder/DataDetail?Id=' + despatchOrderId,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                let despatchOrderData = response.data;
                                if (despatchOrderData) {
                                    if (despatchOrderData.delivery_term_name.includes("BARGE")) {
                                        grid.cellValue(index, "barge_id", despatchOrderData.vessel_id);
                                    }
                                    else {
                                        grid.cellValue(index, "vessel", despatchOrderData.vessel_id);
                                    }
                                    grid.cellValue(index, "delivery_term", despatchOrderData.delivery_term_name);
                                    if (despatchOrderData.delivery_term_name.includes("BARGE")) {
                                        grid.cellValue(index, "barge_id", despatchOrderData.vessel_id);
                                    } else {
                                        grid.cellValue(index, "vessel", despatchOrderData.vessel_id);
                                    }
                                    grid.cellValue(index, "buyer", despatchOrderData.customer_name);
                                    grid.cellValue(index, "address", despatchOrderData.customer_address);
                                    grid.cellValue(index, "loading_port", despatchOrderData.loading_port);
                                    grid.cellValue(index, "discharge_port", despatchOrderData.discharge_port);
                                    grid.cellValue(index, "imo_number", despatchOrderData.imo_number);
                                    grid.cellValue(index, "vessel_flag", despatchOrderData.vehicle_make);

                                    shipmentPlanETA = despatchOrderData.shipment_plan_eta;
                                    billLadingDate = despatchOrderData.draft_survey_bill_lading_date;
                                }
                            }
                        })
                    }
                    e.editorOptions.disabled = false
                }

            }

            if (e.parentType === "dataRow" && e.dataField == "status_id") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                let rowData = e.row.data;
                var statusCode = "";

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler                    
                    statusId = e.value;

                    $.ajax({
                        url: "/api/General/MasterList/DataDetail?Id=" + statusId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            statusCode = response.data[0].item_in_coding;
                            if (statusCode == 'AWL') {
                                grid.cellValue(index, "bl_date", shipmentPlanETA);
                            }
                            else {
                                grid.cellValue(index, "bl_date", billLadingDate);
                            }
                        }
                    });
                    standardHandler(e);
                }
            }


            if (e.parentType === "dataRow" && e.dataField == "vessel") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                let rowData = e.row.data;

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler                    
                    var vesselId = e.value;

                    grid.beginCustomLoading();

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Transport/Vessel/Detail/' + encodeURIComponent(vesselId),
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let vesselData = response;
                            if (vesselData) {
                                grid.beginUpdate();
                                grid.cellValue(index, "imo_number", vesselData.imo_number);
                                grid.cellValue(index, "vessel_flag", vesselData.flag);
                                grid.endUpdate();
                            }
                        }
                    })

                    setTimeout(() => {
                        grid.endCustomLoading();
                    }, 500);

                    standardHandler(e); // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
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
                    //{
                    //    dataField: "royalty_code"
                    //},
                    {
                        dataField: "despatch_order_id"
                    },
                    {
                        dataField: "refreshShippingOrderButton",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "Refresh",
                            disabled: true
                        }
                    },
                    {
                        dataField: "royalty_reference"
                    },
                    {
                        dataField: "royalty_date"
                    },
                    {
                        dataField: "nama_pemegang_et"
                    },
                    {
                        dataField: "status_id"
                    },
                    {
                        dataField: "nomor_sk_iupk"
                    },
                    {
                        dataField: "nomor_et"
                    },
                    {
                        dataField: "destination_id"
                    },
                    {
                        dataField: "nomor_invoice"
                    },
                    {
                        dataField: "bl_date"
                    },
                    {
                        dataField: "delivery_term"
                    },
                    {
                        dataField: "currency_exchange_id"
                    },
                    {
                        dataField: "destination_country"
                    },
                    {
                        dataField: "status_buyer_id"
                    },
                    {
                        dataField: "buyer"
                    },
                    {
                        dataField: "address"
                    },
                    {
                        dataField: "loading_port"
                    },
                    {
                        dataField: "discharge_port"
                    },
                    {
                        dataField: "vessel"
                    },
                    {
                        dataField: "barge_id"
                    },
                    {
                        dataField: "imo_number"
                    },
                    {
                        dataField: "tpk_barge"
                    },
                    {
                        dataField: "vessel_flag"
                    },
                    {
                        dataField: "barge_flag"
                    },
                    {
                        dataField: "coal_origin_id"
                    },
                    {
                        dataField: "tug_id"
                    },
                    {
                        dataField: "permit_location"
                    },
                    {
                        dataField: "tpk_tug"
                    },
                    {
                        dataField: "notes"
                    },
                    {
                        dataField: "tug_flag"
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
        onInitNewRow: function (e) {
            e.data.nama_pemegang_et = "PT. ARUTMIN INDONESIA";
            e.data.nomor_sk_iupk = "221K/33/MEM/2020";
            e.data.nomor_et = "03.ET-04.14.0160";
            e.data.tpk_tug = "JAKARTA";
            e.data.tpk_barge = "JAKARTA";
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