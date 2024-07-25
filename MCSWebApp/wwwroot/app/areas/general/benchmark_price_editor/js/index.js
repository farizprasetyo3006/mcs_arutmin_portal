$(function () {

    var token = $.cookie("Token");
    var areaName = "General";
    var entityName = "BenchmarkPriceEditor";
    var url = "/api/" + areaName + "/" + entityName;

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
        remoteOperations: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "price_name",
                dataType: "text",
                caption: "Price Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                formItem: {
                    colSpan: 2
                }
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Start Date",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "End Date",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "reference_price_id",
                dataType: "text",
                caption: "Reference Price",
                formItem: {
                    colSpan: 2
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/ReferencePriceEditor/ReferencePriceSeriesIdLookup",
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
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
            },
            {
                dataField: "notes",
                caption: "Notes",
                formItem: {
                    colSpan: 2,
                },
                editorType: "dxTextArea",
                editorOptions: {
                    height: 50,
                },
                visible: false
            },
        ],
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                let currentRecord = options.data;

                // Benchmark PDetail Information Container
                renderBenchmarkPriceInformation(currentRecord, container)

                // Benchmark Price Another Informations Tabs
                renderBenchmarkPriceDetail(currentRecord, container)
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

    const renderBenchmarkPriceInformation = function (currentRecord, container) {
        let startDateFormatted = currentRecord.start_date ? moment(currentRecord.start_date.split('T')[0]).format("D MMM YYYY") : '-'
        let endDateFormatted = currentRecord.end_date ? moment(currentRecord.end_date.split('T')[0]).format("D MMM YYYY") : '-'

        $(`<div>
            <h5 class="mb-3">Benchmark Detail</h5>

            <div class="row mb-4">
                <div class="col-md-6">
                    <div class="master-detail-caption mb-2">Overview</div>
                    <div class="card card-mcs card-headline">
                        <div class="row">
                            <div class="col-md-6 pr-0">
                                <div class="headline-title-container">
                                    <small class="font-weight-normal d-block mb-1">Price Name</small>
                                    <h4 class="headline-title font-weight-bold">${(currentRecord.price_name ? currentRecord.price_name : "-")}</h4>
                                </div>
                            </div>
                            <div class="col-md-6 pl-0">
                                <div class="headline-detail-container">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="d-flex align-items-start mb-3">
                                                <div class="d-inline-block mr-3">
                                                    <div class="icon-circle">
                                                        <i class="fas fa-calendar-alt fa-sm"></i>
                                                    </div>
                                                </div>
                                                <div class="d-inline-block">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Start Date</small>
                                                    <h5 class="font-weight-bold">${ startDateFormatted }</h5>
                                                </div>
                                            </div>
                                            <div class="d-flex align-items-start">
                                                <div class="d-inline-block mr-3">
                                                    <div class="icon-circle">
                                                        <i class="fas fa-calendar-alt fa-sm"></i>
                                                    </div>
                                                </div>
                                                <div class="d-inline-block">
                                                    <small class="font-weight-normal text-muted d-block mb-1">End Date</small>
                                                    <h5 class="font-weight-bold">${ endDateFormatted }</h5>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="master-detail-caption mb-2">Other Information</div>
                    <div class="card card-mcs card-headline">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="headline-detail-container">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="d-flex align-items-start">
                                                <div class="d-inline-block mr-3">
                                                    <div class="icon-circle">
                                                        <i class="fas fa-file fa-sm"></i>
                                                    </div>
                                                </div>
                                                <div class="d-inline-block">
                                                    <small class="font-weight-normal text-muted d-block mb-1">Reference Price</small>
                                                    <h5 class="font-weight-bold">${(currentRecord.reference_price_name ? currentRecord.reference_price_name : "-")}</h5>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
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
        </div>`).appendTo(container)
    }

    const renderBenchmarkPriceDetail = function (currentRecord, container) {
        let BenchmarkPriceDetailUrl = "/api/General/BenchmarkPriceSeriesDetail";
        let benchmarkPriceDetailContainer = $("<div class='mb-5'>")
        benchmarkPriceDetailContainer.appendTo(container)

        let referencePriceId = currentRecord.reference_price_id
        let referencePrice

        // Get data from from selected Reference Price Editor
        // in Benchmark Price Editor (master) for the Benchmark Price detail
        $.ajax({
            url: '/api/General/ReferencePriceEditor/DataDetail?Id=' + referencePriceId,
            type: 'GET',
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                referencePrice = response.data[0]
            }
        })


        $("<div>")
            .addClass("master-detail-caption mb-3")
            .text("Benchmark Price Detail")
            .appendTo(benchmarkPriceDetailContainer);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: BenchmarkPriceDetailUrl + "/ByBenchmarkPriceSeriesId/" + encodeURIComponent(currentRecord.id),
                    insertUrl: BenchmarkPriceDetailUrl + "/InsertData",
                    updateUrl: BenchmarkPriceDetailUrl + "/UpdateData",
                    deleteUrl: BenchmarkPriceDetailUrl + "/DeleteData",
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
                        dataField: "brand_owner_id",
                        dataType: "string",
                        caption: "Company",
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
                                    filter: ["item_group", "=", "owner"]
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        },
                        visible: false,
                        setCellValue: function (rowData, value) {
                            rowData.brand_owner_id = value
                            rowData.brand_id = null
                        }
                    },
                    {
                        dataField: "brand_id",
                        dataType: "string",
                        caption: "Brand",
                        lookup: {
                            dataSource: function (options) {
                                return {
                                    store: DevExpress.data.AspNet.createStore({
                                        key: "value",
                                        loadUrl: "/api/General/BenchmarkPriceBrand/BenchmarkPriceBrandIdLookup",
                                        onBeforeSend: function (method, ajaxOptions) {
                                            ajaxOptions.xhrFields = { withCredentials: true };
                                            ajaxOptions.beforeSend = function (request) {
                                                request.setRequestHeader("Authorization", "Bearer " + token);
                                            };
                                        }
                                    }),
                                    filter: options.data ? ["brand_owner_id", "=", options.data.brand_owner_id] : null
                                }
                            },
                            valueExpr: "value",
                            displayExpr: "text"
                        },
                    },
                    {
                        dataField: "calori",
                        dataType: "number",
                        caption: "Calorie",
                        format: "fixedPoint",
                        formItem: {
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            }
                        },
                        setCellValue: function (rowData, value) {
                            rowData.calori = value
                            rowData.price = priceFormula(rowData, referencePrice)
                        }
                    },
                    {
                        dataField: "calori_uom_id",
                        dataType: "text",
                        caption: "Calorie Unit",
                        visible: false,
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
                        },
                        allowEditing: false,
                    },
                    {
                        dataField: "total_moisture",
                        dataType: "number",
                        caption: "Total Moisture",
                        format: "fixedPoint",
                        formItem: {
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            }
                        },
                        setCellValue: function (rowData, value) {
                            rowData.total_moisture = value
                            rowData.price = priceFormula(rowData, referencePrice)
                        }
                    },
                    {
                        dataField: "total_moisture_uom_id",
                        dataType: "text",
                        caption: "Moisture Unit",
                        visible: false,
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
                        },
                        allowEditing: false,
                    },
                    {
                        dataField: "total_sulphur",
                        dataType: "number",
                        caption: "Total Sulphur",
                        format: "fixedPoint",
                        formItem: {
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            }
                        },
                        setCellValue: function (rowData, value) {
                            rowData.total_sulphur = value
                            rowData.price = priceFormula(rowData, referencePrice)
                        }
                    },
                    {
                        dataField: "total_sulphur_uom_id",
                        dataType: "text",
                        caption: "Sulphur Unit",
                        visible: false,
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
                        },
                        allowEditing: false,
                    },
                    {
                        dataField: "ash",
                        dataType: "number",
                        caption: "Ash",
                        format: "fixedPoint",
                        formItem: {
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            }
                        },
                        setCellValue: function (rowData, value) {
                            rowData.ash = value
                            rowData.price = priceFormula(rowData, referencePrice)
                        }
                    },
                    {
                        dataField: "ash_uom_id",
                        dataType: "text",
                        caption: "Ash Unit",
                        visible: false,
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
                        },
                        allowEditing: false,
                    },
                    {
                        dataField: "price",
                        dataType: "number",
                        caption: "Price",
                        formItem: {
                            colSpan: 2,
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            }
                        },
                        allowEditing: false
                    },
                    {
                        dataField: "currency_id",
                        dataType: "text",
                        caption: "Currency",
                        visible: false,
                        lookup: {
                            dataSource: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/Currency/CurrencyIdLookup",
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
                        allowEditing: false
                    },
                    {
                        dataField: "price_uom_id",
                        dataType: "text",
                        caption: "Price Unit",
                        visible: false,
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
                        },
                        allowEditing: false
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
                    if (e.dataField === "brand_id" && e.parentType === "dataRow") {
                        e.editorOptions.disabled = !e.row.data.brand_id && !e.row.data.brand_owner_id || e.row.data.brand_owner_id === null;
                    }
                },
                onInitNewRow: function (e) {
                    e.data.benchmark_price_id = currentRecord.id

                    // Auto fill for calorie, total sulphur, total moisture, ash and currency uom, and price
                    e.data.calori_uom_id = referencePrice.calori_uom_id
                    e.data.total_sulphur_uom_id = referencePrice.total_sulphur_uom_id
                    e.data.total_moisture_uom_id = referencePrice.total_moisture_uom_id
                    e.data.ash_uom_id = referencePrice.ash_uom_id
                    e.data.currency_id = referencePrice.currency_id
                    e.data.price_uom_id = referencePrice.currency_uom_id

                    e.data.calori = 0
                    e.data.total_sulphur = 0
                    e.data.total_moisture = 0
                    e.data.ash = 0
                    e.data.price = 0
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
            }).appendTo(benchmarkPriceDetailContainer);
    }

    function priceFormula(benchmarkPrice, referencePrice) {
        benchmarkPriceCalori = benchmarkPrice.calori || 0
        benchmarkPriceTotalSulphur = benchmarkPrice.total_sulphur || 0
        benchmarkPriceTotalMoisture = benchmarkPrice.total_moisture || 0
        benchmarkPriceAsh = benchmarkPrice.ash || 0

        let formula1 = ((100 - referencePrice.total_moisture) / (100 - benchmarkPriceTotalMoisture) * benchmarkPriceTotalMoisture + 100 - referencePrice.total_moisture) / 100 //fka
        let formula2 = (benchmarkPriceTotalSulphur - referencePrice.total_sulphur) * 4 + (benchmarkPriceAsh - referencePrice.ash) * 0.4 // (b + u)
        let formula3 = referencePrice.price * benchmarkPriceCalori / referencePrice.calori * (100 - benchmarkPriceTotalMoisture) / (100 - referencePrice.total_moisture / formula1) // (hba * k * a)

        let price = formula3 + formula2
        //console.log(price)
        return price
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
                url: "/api/General/Currency/UploadDocument",
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