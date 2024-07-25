$(function () {

    var token = $.cookie("Token");
    var areaName = "Planning";
    var entityName = "ShippingProgram";
    var url = "/api/" + areaName + "/" + entityName;
    var salesPlanDetailData = null;
    var CustomerId = ""
    var Quantity = 0;
    var data = [];
    let quantity = 0;
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

    /*function downloadTemplate() {

        $.ajax({
            url: "/Planning/ShippingProgram/ExcelExport?tanggal1=" + encodeURIComponent(formatTanggal(firstDay)) + "&tanggal2=" + encodeURIComponent(formatTanggal(lastDay)),
            type: 'GET',
            cache: false,
            data: JSON.stringify(),
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
                a.download = "Shipping_Program.xlsx"; // Set the appropriate file name here
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);

            } else {
                toastr["error"]("File download failed.");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            alert('Template gagal didownload!');
        }); 
    }*/

    $.ajax({
        type: "GET",
        url: "/api/Planning/ShippingProgram/ShippingProduct",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (result) {
            // Assuming the data returned is an array of shipping products
            var shippingProducts = result.data || [];

            // Now, you can use the shippingProducts array to dynamically create columns
            var dynamicColumns = shippingProducts.map(function (product, index) {
                if (index < 35) {
                    return {
                        dataField: "product_" + (index + 1),
                        dataType: "number",
                        caption: product,
                        format: "fixedPoint",
                        visible: false,
                        formItem: {
                            colSpan: 2,
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            }
                        },
                    };
                }
            });

            // Static column
            var staticColumn = [
                {
                dataField: "shipping_program_number",
                dataType: "string",
                caption: "Shipping Program Number",
                allowEditing: false,
                },
                {
                    dataField: "plan_year_id",
                    dataType: "string",
                    caption: "Year",
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
                        displayExpr: "text",
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
                    validationRules: [{
                        type: "required",
                        message: "The field is required."
                    }],
                    lookup: {
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "value",
                            loadUrl: "/api/planning/salesplandetail" + "/MonthIndexLookup",
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
                    dataField: "declared_month_id",
                    dataType: "string",
                    caption: "PLN Declared Month",
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
                    dataField: "product_category_id",
                    dataType: "text",
                    caption: "Brand",
                    validationRules: [{
                        type: "required",
                        message: "This field is required."
                    }],  
                    lookup: {
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "value",
                            loadUrl: "/api/Material/Product/ProductIdLookupWhereCategory",
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
                    dataField: "commitment_id",
                    dataType: "string",
                    caption: "Commitment",
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
                                filter: ["item_group", "=", "commitment"]
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
                    dataField: "customer_id",
                    dataType: "String",
                    caption: "Buyer",
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
                        CustomerId = value;
                    },
                    formItem: {
                        editorOptions: {
                            showClearButton: true
                        }
                    }
                },
                {
                    dataField: "sales_contract_id",
                    dataType: "string",
                    caption: "Sales Contract Term",
                    lookup: {
                        dataSource: function (options) {
                            return {
                                store: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/SalesContractIdLookup?CustomerId=" + encodeURIComponent(CustomerId),
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
                    //editorOptions: {
                    //    onOpened: function (e) {
                    //        renderAddNewButton("/Sales/Customer/Index")

                    //        // Always reload dataSource onOpenned to get new data
                    //        let lookup = e.component
                    //        lookup._dataSource.reload()
                    //    }
                    //},
                    setCellValue: function (rowData, value) {
                        rowData.sales_contract_id = value
                    },
                    formItem: {
                        editorOptions: {
                            showClearButton: true
                        }
                    }
                },
                {
                    dataField: "end_user_id",
                    dataType: "string",
                    caption: "End Buyer",
                    lookup: {
                        dataSource: function (options) {
                            return {
                                store: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/SalesContractEndUserIdLookup",
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
                    formItem: {
                        editorOptions: {
                            showClearButton: true
                        }
                    }
                },
                {
                    dataField: "tipe_penjualan_id",
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
                    }
                },
                {
                    dataField: "source_coal_id",
                    dataType: "String",
                    caption: "Loading Port",
                    //validationRules: [{
                    //    type: "required",
                    //    message: "This field is required."
                    //}],
                    lookup: {
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "value",
                            loadUrl: url + "/PortIdLookup",
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
                        rowData.source_coal_id = value;
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
                    dataField: "quantity",
                    dataType: "number",
                    caption: "Quantity",
                    format: "fixedPoint",
                    formItem: {
                        editorType: "dxNumberBox",
                        editorOptions: {
                            format: "fixedPoint",
                            readOnly: true,
                            step: 0
                        }
                    },
                },
            ];

            // Initialize the dxDataGrid with both dynamic and static columns
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
                columns: [...staticColumn, ...dynamicColumns], // Include both static and dynamic columns
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
                    if (e.parentType == "dataRow") {

                        // Disabled all columns/fields if is_locked is true
                        if (e.dataField !== "is_locked" && e.row.data.is_locked) {
                            e.editorOptions.disabled = true
                        }
                    }
                },
                onInitNewRow: function (e) {
                    e.data.is_locked = false
                    e.data.is_baseline = false
                },
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
                onExporting: function (e) {
                    var workbook = new ExcelJS.Workbook();
                    var worksheet = workbook.addWorksheet(entityName);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet: worksheet,
                        autoFilterEnabled: true,
                        customizeExcelExportData: function (columns, rows) {
                            // Get all columns, including invisible ones
                            var allColumns = grid.getVisibleColumns();

                            // Add dynamically generated columns to the exported columns
                            columns = columns.concat(dynamicColumns);

                            // Add all columns to the exported columns
                            columns = columns.concat(allColumns);

                            // Continue with the default data export
                            return { columns: columns, rows: rows };
                        }
                    }).then(function () {
                        // Writing to buffer and saving the file
                        workbook.xlsx.writeBuffer().then(function (buffer) {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), entityName + '.xlsx');
                        });
                    });

                    e.cancel = true;
                }




            });

        },
        error: function (error) {
            console.error("Error fetching shipping products:", error);
        }
    });

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Detail",
                    template: createDetailTabTemplate(masterDetailOptions.data)
                },
            ]
        });
    }

    /*dataSource: DevExpress.data.AspNet.createStore({
        key: "id",
        loadUrl: url + "/ByShippingProgramId?Id=" + currentRecord.id,
        insertUrl: urlDetail + "/InsertDetail",
        updateUrl: urlDetail + "/UpdateDetail",
        deleteUrl: urlDetail + "/DeleteDetail",
        onBeforeSend: function (method, ajaxOptions) {
            ajaxOptions.xhrFields = { withCredentials: true };
            ajaxOptions.beforeSend = function (request) {
                request.setRequestHeader("Authorization", "Bearer " + token);
            };
        }
    }),*/

    function createDetailTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            //let detailName = "BlendingPlanProduct";
            let urlDetail = "/api/" + areaName + "/" + entityName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/ByShippingProgramId?Id=" + currentRecord.id,
                        insertUrl: urlDetail + "/InsertDetail",
                        updateUrl: urlDetail + "/UpdateDetail",
                        deleteUrl: urlDetail + "/DeleteDetail",
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
                            dataField: "product_category_id",
                            dataType: "text",
                            caption: "Product Category",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Material/Product/ProductCategoryIdLookup",
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
                            },
                        },
                        {
                            dataField: "product_id",
                            dataType: "text",
                            caption: "Product",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Material/Product/ProductIdNPLCT",
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
                            },
                        },
                        {
                            dataField: "contractor_id",
                            dataType: "string",
                            caption: "Contractor",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Location/MineLocation/ContractorIdLookup",
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
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity",
                            allowEditing: true,
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
                        },
                    ],
                    summary: {
                        totalItems: [
                            {
                                column: 'quantity',
                                summaryType: 'sum',
                                valueFormat: ',##0.###'
                            },
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
                        e.data.shipping_program_id = currentRecord.id;
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
                                                        <h5 class="font-weight-bold">${createdOnFormatted}</h5>
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
                                                        <h5 class="font-weight-bold">${modifiedOnFormatted}</h5>
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
        var detailName = "SalesPlanDetail";
        var urlDetail = "/api/" + areaName + "/" + detailName;

        let salesPlanDetailsContainer = $("<div class='mb-5'>")
        salesPlanDetailsContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-2")
            .text("Monthly Details")
            .appendTo(salesPlanDetailsContainer);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: urlDetail + "/BySalesPlanId/" + encodeURIComponent(currentRecord.id),
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
                        dataField: "sales_plan_id",
                        allowEditing: false,
                        visible: false,
                        formItem: {
                            visible: false
                        },
                        calculateCellValue: function () {
                            return currentRecord.id;
                        }
                    },
                    {
                        dataField: "month_id",
                        dataType: "number",
                        caption: "Month",
                        validationRules: [{
                            type: "required",
                            message: "The field is required."
                        }],
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
                        caption: "Detail",
                        type: "buttons",
                        width: 200,
                        buttons: [{
                            cssClass: "btn-dxdatagrid",
                            hint: "See Customers",
                            text: "See Customers",
                            onClick: async function (e) {
                                salesPlanDetailData = e.row.data;

                                await $.ajax({
                                    url: "/api/Planning/SalesPlanCustomer/BySalesPlanCustomerId/" + salesPlanDetailData.id,
                                    type: 'GET',
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        Quantity = response.data[0].quantity;
                                    }
                                });

                                showSalesPlanMonthlyCustomerPopup();
                            }
                        }]
                    },
                    {
                        type: "buttons",
                        buttons: ["edit", "delete"]
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
                }
            }).appendTo(salesPlanDetailsContainer);
    }

    let popupOptions = {
        title: "Monthly Customers",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function () {
            let container = $("<div>")

            $(`<div class="mb-3">
                    <div class="row">
                        <div class="col-md-3">
                            <small class="font-weight-normal">Sales Plan</small>
                            <h3 class="font-weight-bold">`+ salesPlanDetailData.plan_name + `</h6>
                        </div>
                        <div class="col-md-2">
                            <small class="font-weight-normal">Month</small>
                            <h3 class="font-weight-bold">`+ salesPlanDetailData.month_name + `</h6>
                        </div>
                        <div class="col-md-3">
                            <small class="font-weight-normal">Quantity</small>
                            <!-- <h3 class="font-weight-bold">`+ formatNumber(salesPlanDetailData.quantity) + `</h3> -->
                            <h3 class="font-weight-bold">`+ formatNumber(Quantity) + `</h3>
                        </div>
                    </div>
                </div>
            `).appendTo(container)


            let url = "/api/Planning/SalesPlanCustomer";
            $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/BySalesPlanDetailId/" + salesPlanDetailData.id,
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
                            dataField: "sales_plan_detail_id",
                            dataType: "string",
                            caption: "Sales Plan Detail (Monthly) Id",
                            allowEditing: false,
                            visible: false,
                            formItem: {
                                visible: false
                            },
                            calculateCellValue: function () {
                                return salesPlanDetailData.id;
                            }
                        },
                        {
                            dataField: "customer_id",
                            dataType: "String",
                            caption: "Customer",
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
                                //CustomerId = value;
                            }
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
                            dataField: "sales_contract_id",
                            dataType: "string",
                            caption: "Sales Contract",
                            lookup: {
                                dataSource: function (options) {
                                    return {
                                        store: DevExpress.data.AspNet.createStore({
                                            key: "value",
                                            loadUrl: url + "/SalesContractIdLookup?CustomerId=" + encodeURIComponent(CustomerId),
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
                            //editorOptions: {
                            //    onOpened: function (e) {
                            //        renderAddNewButton("/Sales/Customer/Index")

                            //        // Always reload dataSource onOpenned to get new data
                            //        let lookup = e.component
                            //        lookup._dataSource.reload()
                            //    }
                            //},
                            setCellValue: function (rowData, value) {
                                rowData.sales_contract_id = value
                            }
                        },
                        {
                            dataField: "dmo",
                            dataType: "text",
                            caption: "DMO",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/DMOLookup",
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
                                rowData.dmo = value;
                            }
                        },
                        {
                            dataField: "electricity",
                            dataType: "text",
                            caption: "Electricity",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/ElectricityLookup",
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
                                rowData.electricity = value;
                            }
                        },
                        {
                            dataField: "monthly_sales_id",
                            dataType: "number",
                            caption: "Monthly sales",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "The field is required."
                            //}],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Planning/SalesPlanDetail/MonthIndexLookup",
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
                            dataField: "declared_month_id",
                            dataType: "number",
                            caption: "PLN Declared Month",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "The field is required."
                            //}],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Planning/SalesPlanDetail/MonthIndexLookup",
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
                            dataField: "schedule_id",
                            dataType: "string",
                            caption: "Schedule",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "The field is required."
                            //}],
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
                                        filter: ["item_group", "=", "schedule"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "area_id",
                            dataType: "string",
                            caption: "Area",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "The field is required."
                            //}],
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
                                        filter: ["item_group", "=", "area"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "country_id",
                            dataType: "string",
                            caption: "Country",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "The field is required."
                            //}],
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
                                        filter: ["item_group", "=", "country"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "product_id",
                            dataType: "text",
                            caption: "Product",
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
                            }],
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
                            }
                        },
                        {
                            dataField: "product_category_id",
                            dataType: "text",
                            caption: "Bit/Eco/Srg",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "This field is required."
                            //}],
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
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            }
                        },
                        {
                            dataField: "agents_id",
                            dataType: "String",
                            caption: "Agents",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "This field is required."
                            //}],
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
                                rowData.agents_id = value;
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "commitment_id",
                            dataType: "string",
                            caption: "Commitment",
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
                                        filter: ["item_group", "=", "commitment"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "pricing_id",
                            dataType: "string",
                            caption: "Pricing",
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
                                        filter: ["item_group", "=", "pricing-method"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "index_link_id",
                            dataType: "String",
                            caption: "Index Link",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "This field is required."
                            //}],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/General/PriceIndex/PriceIndexIdLookup",
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
                                rowData.index_link_id = value;
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            },
                            sortOrder: "asc"
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
                                            loadUrl: "/api/General/MasterList/MasterListIdLookup",
                                            onBeforeSend: function (method, ajaxOptions) {
                                                ajaxOptions.xhrFields = { withCredentials: true };
                                                ajaxOptions.beforeSend = function (request) {
                                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                                };
                                            }
                                        }),
                                        filter: ["item_group", "=", "status"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "incoterm_id",
                            dataType: "string",
                            caption: "Own",
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
                                        filter: ["item_group", "=", "delivery-term"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "ship_no_id",
                            dataType: "string",
                            caption: "Ship No."
                        },
                        {
                            dataField: "index_link_id",
                            dataType: "String",
                            caption: "Vessel",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "This field is required."
                            //}],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Planning/SalesPlanCustomer/VesselIdBargeIdLookup",
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
                                rowData.index_link_id = value;
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "tipe_penjualan_id",
                            dataType: "string",
                            caption: "Transport Type",
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
                                        filter: ["item_group", "=", "tipe-penjualan"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "source_coal_id",
                            dataType: "String",
                            caption: "Loading Port",
                            //validationRules: [{
                            //    type: "required",
                            //    message: "This field is required."
                            //}],
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
                            },
                            setCellValue: function (rowData, value) {
                                rowData.source_coal_id = value;
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            },
                            sortOrder: "asc"
                        },
                        {
                            dataField: "laycan_start",
                            dataType: "datetime",
                            caption: "Laycan Start"
                        },
                        {
                            dataField: "laycan_end",
                            dataType: "datetime",
                            caption: "Laycan End"
                        },
                        {
                            dataField: "eta_port",
                            dataType: "datetime",
                            caption: "ETA Port"
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
                        pageSize: 10
                    },
                    height: 500,
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
                        e.data.sales_plan_detail_id = salesPlanDetailData.id;
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

    const showSalesPlanMonthlyCustomerPopup = function () {
        popup.option("contentTemplate", popupOptions.contentTemplate.bind(this));
        popup.show()
    }
     // + encodeURIComponent(formatTanggal(firstDay)) + "/" + encodeURIComponent(formatTanggal(lastDay))

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
                url: "/Planning/ShippingProgram/ExcelExport",
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
                    a.download = "Shipping_Program.xlsx"; // Set the appropriate file name here
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

    /*$('#btnExport').on('click', function () {
        downloadTemplate();
    });*/

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
                url: "/api/Planning/ShippingProgram/UploadDocument",
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