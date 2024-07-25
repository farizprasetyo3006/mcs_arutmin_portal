$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "ShipmentPlan";
    var url = "/api/" + areaName + "/" + entityName;
    var selectedIds = null;

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("wasteRemovalDate1");
    var tgl2 = sessionStorage.getItem("wasteRemovalDate2");

    var date = new Date(), y = date.getFullYear(), m = date.getMonth(), d = date.getDate();
    var firstDay = new Date(y, m, d - 1);
    var lastDay = new Date(y, m, d, 23, 59, 59);

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
            sessionStorage.setItem("wasteRemovalDate1", formatTanggal(firstDay));
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
            sessionStorage.setItem("wasteRemovalDate2", formatTanggal(lastDay));
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
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "shipping_program_number",
                dataType: "string",
                caption: "Shipping Program Number",
                visible: false,
                formItem: {
                    colSpan: 2
                },
                allowEditing: false
            },
            {
                dataField: "lineup_number",
                dataType: "string",
                caption: "Line Up Code",
                allowEditing: false
            },
            {
                dataField: "customer_id",
                dataType: "string",
                caption: "Buyer",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/Customer/CustomerIdLookup",
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
                    rowData.customer_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                sortOrder: "asc",
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "sales_contract_id",
                dataType: "string",
                caption: "Contract Term Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        var _url = "/API/Planning/ShippingProgram/SalesContractIdLookup";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.customer_id !== undefined
                                    && options.data.customer_id !== null) {
                                    _url += "?CustomerId=" + encodeURIComponent(options.data.customer_id);
                                }
                            }
                        }
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: _url,
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
                setCellValue: function (rowData, value) {
                    rowData.sales_contract_id = value;
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
                dataField: "end_user",
                dataType: "string",
                caption: "End Buyer",
                //validationRules: [{
                //    type: "required",
                //    message: "This field is required."
                //}],
                visible: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/Customer/CustomerIdLookup",
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
                    rowData.end_user = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                sortOrder: "asc",
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "shipment_year",
                dataType: "string",
                caption: "Year",
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "month_id",
                dataType: "number",
                caption: "Month",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/MonthIndexLookup",
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
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "product_id",
                dataType: "string",
                caption: "Product",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                visible: false,
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
                }
            },
            {
                dataField: "destination",
                dataType: "string",
                caption: "Port of Discharge"
            },
            {
                dataField: "shipment_number",
                dataType: "string",
                caption: "Shipment Number",
                visible: false
            },
            {
                dataField: "incoterm",
                caption: "Incoterm",
                dataType: "string",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
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
                            filter: ["item_group", "=", "delivery-term"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                setCellValue: function (rowData, value) {
                    rowData.incoterm = value;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "transport_id",
                dataType: "string",
                caption: "Transport Name",
                lookup: {
                    dataSource: function (options) {
                        /*  var _url = url;
                          if (options.data && options.data.incoterm) {
                              _url += "/TransportByIncoterm?Incoterm=" + options.data.incoterm;
                          } else if (options.data === null || options.data === undefined) {
                              _url += "/TransportIdLookup";
                          }*/
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                //loadUrl: _url,
                                loadUrl: "/api/Planning/SalesPlanCustomer/VesselIdBargeIdLookup",
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
                setCellValue: function (rowData, value) {
                    rowData.transport_id = value
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
                dataField: "vessel_id",
                dataType: "string",
                caption: "Transport Type",
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
                            filter: ["item_group", "=", "tipe-penjualan"]
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
                setCellValue: function (rowData, value) {
                    rowData.vessel_id = value
                }
            },
            {
                dataField: "laycan_start",
                dataType: "date",
                caption: "Laycan Start",
            },
            {
                dataField: "laycan_end",
                dataType: "date",
                caption: "Laycan End",
            },
            {
                dataField: "laycan_status",
                dataType: "string",
                caption: "Laycan Status",
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
                            filter: ["item_group", "=", "time-status"]
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
                dataField: "invoice_no",
                dataType: "string",
                caption: "Invoice No",
                visible: false
            },
            {
                dataField: "royalti",
                dataType: "string",
                caption: "Royalty",
                visible: false
            },
            {
                dataField: "eta",
                dataType: "datetime",
                caption: "ETA",
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                dataField: "eta_status",
                dataType: "string",
                caption: "ETA Status",
                visible: false,
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
                            filter: ["item_group", "=", "time-status"]
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
                dataField: "qty_sp",
                dataType: "number",
                caption: "Contracted Tonnage",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },
            },
            {
                dataField: "stow_plan",
                dataType: "number",
                caption: "Stow Plan",
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                visible: false,
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
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "remarks",
                dataType: "string",
                caption: "Remark",
                cellTemplate: function (container, options) {
                    let value = options.data.remark ? options.data.remark : ''
                    if (options.data.id) {
                        $(`<div class="text-left">
                            <i class="fas fa-edit" style="color: #a1a1a1"></i>
                            <span>${value}</span>
                        </div>`).appendTo(container)
                    }
                    else {
                        container.append(value)
                    }
                },
                visible: false
            },
            {
                dataField: "fc_provider_id",
                dataType: "text",
                caption: "Fc Provider",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Organisation/Contractor/ContractorIdLookup",
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
                dataField: "transport_provider_id",
                dataType: "text",
                caption: "Transport Provider",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Organisation/Contractor/ContractorIdLookup",
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
                dataField: "loading_rate",
                dataType: "number",
                caption: "Loadrate Contracted",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                visible: false,
                allowEditing: true
            },
            {
                dataField: "loading_standart",
                dataType: "number",
                caption: "Loadrate Standard",
                visible: false,
                setCellValue: function (rowData, value) {
                    rowData.loading_standart = value
                }
            },
            {
                dataField: "despatch_demurrage_rate",
                dataType: "number",
                caption: "Dem USD/Day",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                visible: false,
                allowEditing: true
            },
            {
                dataField: "hpb_forecast",
                dataType: "number",
                caption: "HPB Forecast",
                editorType: "dxNumberBox",
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
                visible: false
            },
            /* {
                 dataField: "traffic_officer_id",
                 dataType: "text",
                 caption: "Traffic Officer",
                 lookup: {
                     dataSource: DevExpress.data.AspNet.createStore({
                         key: "value",
                         loadUrl: "/api/General/Employee/EmployeeIdLookup",
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
             },*/
            {
                dataField: "declared_month_id",
                dataType: "string",
                caption: "PLN Declared Month",
                visible: false,
                //validationRules: [{
                //    type: "required",
                //    message: "The field is required."
                //}],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Planning/SalesPlanDetail/MonthYearIndexLookup",
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
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "loadport_agent",
                dataType: "string",
                caption: "LoadPort Agent",
                visible: false,
            },
            {
                dataField: "certain",
                dataType: "boolean",
                caption: "Confirmation",
            },
            {
                dataField: "nora",
                dataType: "datetime",
                caption: "Nora",
                visible: false,
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                dataField: "etb",
                dataType: "datetime",
                caption: "ETB",
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                dataField: "etc",
                dataType: "datetime",
                caption: "ETC",
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                dataField: "pln_schedule",
                dataType: "string",
                caption: "PLN Schedule",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/MonthIndexLookup",
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "original_schedule",
                dataType: "string",
                caption: "Original Schedule",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/MonthIndexLookup",
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                }
            },
            {
                dataField: "loading_port",
                dataType: "string",
                caption: "Loading Port",
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
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                setCellValue: function (rowData, value) {
                    rowData.loading_port = value
                }
            },
            {
                dataField: "eta_disc",
                dataType: "datetime",
                caption: "ETA Disc",
                visible: false,
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                dataField: "etb_disc",
                dataType: "datetime",
                caption: "ETB Disc",
                visible: false,
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                dataField: "etcommence_disc",
                dataType: "datetime",
                caption: "ETCommence Disc",
                visible: false,
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                dataField: "etcompleted_disc",
                dataType: "datetime",
                caption: "ETCompleted Disc",
                visible: false,
                format: "MM/dd/yyyy, HH:mm"
            },
            {
                caption: "Split Data",
                type: "buttons",
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    text: "Split",
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
        summary: {
            totalItems: [
                {
                    column: 'stow_plan',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                },
                {
                    column: 'qty_sp',
                    summaryType: 'sum',
                    valueFormat: ',##0.###'
                }
            ],
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
            allowAdding: false,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 1,
                items: [
                    {
                        itemType: "group",
                        caption: "Shipment",
                        colCount: 2,
                        items: [
                            {
                                dataField: "shipping_program_number"
                            },
                            {
                                dataField: "lineup_number"
                            },
                            {
                                dataField: "customer_id"
                            },
                            {
                                dataField: "sales_contract_id"
                            },
                            {
                                dataField: "end_user"
                            },
                            {
                                dataField: "shipment_year"
                            },
                            {
                                dataField: "month_id"
                            },
                            {
                                dataField: "product_id"
                            },
                            {
                                dataField: "destination"
                            },
                            {
                                dataField: "shipment_number"
                            },
                            {
                                dataField: "incoterm"
                            },
                            {
                                dataField: "transport_id"
                            },
                            {
                                dataField: "vessel_id"
                            },
                            {
                                dataField: "invoice_no"
                            },
                            {
                                dataField: "stow_plan"
                            },
                            {
                                dataField: "qty_sp"
                            },
                            {
                                dataField: "royalti"
                            },
                            {
                                dataField: "business_unit_id"
                            },
                            {
                                dataField: "loading_rate"
                            },
                            {
                                dataField: "loading_standart"
                            },
                            {
                                dataField: "despatch_demurrage_rate"
                            },
                            /*{
                                dataField: "traffic_officer_id"
                            },*/
                            {
                                dataField: "declared_month_id"
                            },
                            {
                                dataField: "certain"
                            },
                            {
                                dataField: "pln_schedule"
                            },
                            {
                                dataField: "original_schedule"
                            },
                            {
                                dataField: "loading_port"
                            },
                            {
                                dataField: "remarks"
                            },
                            {
                                dataField: "fc_provider_id"
                            },
                            {
                                dataField: "transport_provider_id"
                            },
                            {
                                dataField: "loadport_agent"
                            },
                            {
                                dataField: "hpb_forecast"
                            }
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "Schedule",
                        colCount: 2,
                        items: [
                            {
                                dataField: "laycan_start"
                            },
                            {
                                dataField: "laycan_end"
                            },
                            {
                                dataField: "laycan_status"
                            },
                            {
                                dataField: "eta"
                            },
                            {
                                dataField: "eta_status"
                            },
                            {
                                dataField: "nora"
                            },
                            {
                                dataField: "etb"
                            },
                            {
                                dataField: "etc"
                            },
                            {
                                dataField: "eta_disc"
                            },
                            {
                                dataField: "etb_disc"
                            },
                            {
                                dataField: "etcommence_disc"
                            },
                            {
                                dataField: "etcompleted_disc"
                            }
                        ]
                    }
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
            e.data.certain = false;
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
            $("#btn-fetch1").remove();
            var $customButton = $('<div id="btn-fetch1">').dxButton({
                icon: 'refresh',
                text: "Fetch",
                onClick: function () {
                    $customButton.dxButton("instance").option("text", "Fetching");
                    $customButton.dxButton("instance").option("disabled", true);

                    // Add loading icon
                   // var $loadingIcon = $('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');
                    //s$customButton.append($loadingIcon);
                    //console.log(masterDetailData);
                    //$("#grid-shipping-transaction-detail").dxDataGrid("getDataSource").reload();
                    $.ajax({
                        url: '/api/Planning/ShipmentPlan/FetchShippingProgramToShipmentPlan/',
                        type: 'GET',
                        contentType: "application/json",
                        headers: {
                            "Authorization": "Bearer " + token
                        },
                    }).done(function (result) {
                        if (result.status.success) {
                            Swal.fire("Success!", "Fetching Data successfully.", "success");
                            $("#grid").dxDataGrid("getDataSource").reload();
                            // $("#grid-shipping-transaction-detail").dxDataGrid("getDataSource").reload();
                        } else {
                            Swal.fire("Error !", result.message, "error");
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        Swal.fire("Failed !", textStatus, "error");
                    });
                }
            })

            e.element.find('.dx-datagrid-header-panel').append($customButton)
        },
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            //if (e.parentType === "dataRow") {
            //    if (e.dataField == "accounting_period_name" || e.dataField == "is_closed" || e.dataField == "aktif") {
            //        e.editorOptions.disabled = e.row.data.accounting_period_name != null
            //    }
            //}
            if (e.parentType === "dataRow" && e.dataField == "laycan_start") {
                e.editorOptions.disabled = e.row.data.laycan_committed;

                let standardHandler = e.editorOptions.onValueChanged

                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

                    let laycanStart = e.value
                    ////console.log("laycanStart: ", laycanStart)

                    var laycanEndDate = new Date(laycanStart)
                    laycanEndDate.setDate(laycanEndDate.getDate() + 9)
                    grid.cellValue(index, "laycan_start", e.value);
                    grid.cellValue(index, "laycan_end", laycanEndDate);

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
            if (e.parentType === "dataRow" && e.dataField == "laycan_end") {
                e.editorOptions.disabled = e.row.data.laycan_committed;
            }

            if (e.parentType === "dataRow" && e.dataField === "sales_contract_id") {
                let myhandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                //grid.beginCustomLoading();
                e.editorOptions.onValueChanged = async function (e) {
                    let salesContract = e.value;
                    if (salesContract !== null || salesContract !== "") {
                        grid.cellValue(index, "certain", true);
                    }
                    else {
                        grid.cellValue(index, "certain", false);
                    }
                    myhandler(e);
                }
            }

            if (e.parentType === "dataRow" && e.dataField === "incoterm") {
                let myhandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;
                //grid.beginCustomLoading();
                e.editorOptions.onValueChanged = async function (e) {
                    let selectedIncoterm = e.value;
                    grid.cellValue(index, "transport_id", null); // Reset nilai transport_id saat incoterm berubah

                    $.ajax({
                        url: '/TransportByIncoterm?Incoterm=' + selectedIncoterm,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let transportData = response;
                            //grid.beginUpdate();
                            grid.columnOption("transport_id", "lookup.dataSource", transportData); // Memperbarui dataSource untuk dropdown transport_id
                            //grid.endUpdate();
                        },
                        error: function (error) {
                            console.error("Error fetching transport data:", error);
                            grid.endCustomLoading();
                        }
                    });
                    //grid.endCustomLoading();
                    /*setTimeout(() => {
                        grid.endCustomLoading()
                    }, 100)*/
                    myhandler(e); // Panggil myhandler untuk menyimpan nilai incoterm yang dipilih
                };
            }

            if (e.parentType === "dataRow" && e.dataField == "sales_plan_customer_id") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                    let salesPlanCustomerId = e.value
                    await $.ajax({
                        url: url + '/GetSalesPlanCustomer?id=' + salesPlanCustomerId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let salesPlanData = response;
                            // Set its corresponded field's value
                            grid.cellValue(index, "month_id", salesPlanData.month_id);
                            grid.cellValue(index, "customer_id", salesPlanData.customer_id);
                            grid.cellValue(index, "transport_id", salesPlanData.vessel_id);
                            grid.cellValue(index, "laycan_start", salesPlanData.laycan_start);
                            grid.cellValue(index, "laycan_end", salesPlanData.laycan_end);
                            grid.cellValue(index, "eta", salesPlanData.eta_date);

                        }
                    })

                    await $.ajax({
                        url: url + '/GetSalesPlanCustomerList?id=' + salesPlanCustomerId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let salesPlanData = response;
                            grid.cellValue(index, "sales_contract_id", salesPlanData.sales_contract_id);
                            grid.cellValue(index, "shipment_year", salesPlanData.plan_name);
                        }
                    })

                    await $.ajax({
                        url: url + '/GetCoalBrand?id=' + salesPlanCustomerId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let contractProduct = response;
                            grid.cellValue(index, "contract_product_id", contractProduct.product_id);
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }

            }

            if (e.parentType === "dataRow" && e.dataField == "loading_standart") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data
                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    
                    let salesContractTerm = rowData.sales_contract_id;
                    let transportTypeId = rowData.vessel_id;
                    let loadingPortId = rowData.loading_port;
                    await $.ajax({
                        url: '/api/planning/shipmentplan/getloadratecontracted/' + encodeURIComponent(salesContractTerm)
                            + '/' + encodeURIComponent(transportTypeId) + '/' + encodeURIComponent(loadingPortId),
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            // Set its corresponded field's value
                            grid.cellValue(index, "loading_rate", response);

                        }
                    });
                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }
        },
        onEditingStart: function (e) {
            //alert(e);
            // alert(e.data.shipment_code);
            // e.component.columnOption('shipment_code').formItem.visible = false;
            //if (e.data.shipment_code) {
            //    e.component
            //}
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
                    /* {
                         //itemType: "label",
                         label: {
                             text: "Are Your Sure?",
                         },
                         colSpan: 2,
                         
                     },*/
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
                                ////console.log(data);
                                let formData = new FormData();
                                //formData.append("key", data.id);
                                formData.append("key", data.id);
                                formData.append("number", data.number);

                                formData.append("values", JSON.stringify(data));

                                saveApprovalForm(formData);
                            }
                        }
                    },

                ],
                onInitialized: () => {
                    $.ajax({
                        type: "GET",
                        url: "/api/Mining/CHLS/GetCHLS/" + encodeURIComponent(salesInvoiceApprovalData.id),
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
    var approvalPopup = $("#split-popup").dxPopup(approvalPopupOptions).dxPopup("instance")

    const showApprovalPopup = function () {
        approvalPopup.option("contentTemplate", approvalPopupOptions.contentTemplate.bind(this));
        approvalPopup.show()
    }

    const saveApprovalForm = (formData) => {
        $.ajax({
            type: "POST",
            url: "/api/Planning/ShippingProgram/SplitQuantity",
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

    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;

            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');

            $.ajax({
                url: "/Planning/ShipmentPlan/ExcelExport",
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
                    a.download = "Shipment_Plan.xlsx"; // Set the appropriate file name here
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

    $('#btnDownloadLineup').on('click', function () {
       
        $('#btnDownloadLineup')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');

        $.ajax({
            url: "/Planning/ShipmentPlan/ExcelExportLineupRawData",
            type: 'POST',
            cache: false,
            contentType: "application/json",
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
                a.download = "Shipment_Lineup_Raw_Data.xlsx"; // Set the appropriate file name here
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                toastr["success"]("File downloaded successfully.");
                //  $("#modal-download-selected").modal('hide');////

            } else {
                toastr["error"]("File download failed.");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            toastr["error"]("Action failed.");
        }).always(function () {
            $('#btnDownloadLineup').html('Download');
        });
    });

    $('#btnRecalculate').on('click', function () {

        $('#btnRecalculate')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Calculating, please wait ...');

        $.ajax({
            url: "/api/Sales/SalesInvoice/RecalculateHPB",
            type: 'GET',
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result) {
                if (result.success) {
                    $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Successfuly Calculate " + result.value + " out of " + result.total + " data(s)", "success");
                    $("#modal-recalculate").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.status, "error");
                    $("#modal-recalculate").modal('hide');
                }
            }
        }).fail(function (result, jqXHR, textStatus, errorThrown) {
            $("#modal-recalculate-quality").modal('hide');
            Swal.fire("Error !", result.responseJSON.message, "error");
        }).always(function () {
            $('#btnRecalculate').html('Calculate');
        });
        
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
                url: "/api/Planning/ShipmentPlan/UploadDocument",
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
});