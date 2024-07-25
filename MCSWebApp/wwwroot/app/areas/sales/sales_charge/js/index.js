$(function () {

    var token = $.cookie("Token");
    var areaName = "Sales";
    var entityName = "SalesCharge";
    var url = "/api/" + areaName + "/" + entityName;

    let dataGrid = $("#grid").dxDataGrid({
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
                dataField: "sales_charge_name",
                dataType: "string",
                caption: "Sales Charge Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                formItem: {
                    colSpan: 2
                },
                sortOrder: "asc"
            },
            {
                dataField: "charge_type_id",
                dataType: "string",
                caption: "Charge Type",
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
                            filter: ["item_group", "=", "charge-type"]
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
                    colSpan: 2
                }
            },
            {
                dataField: "prerequisite",
                dataType: "string",
                caption: "Prerequisite",
                editorType: "dxTextArea",
                formItem: {
                    colSpan: 2
                }
            },
            {
                dataField: "charge_formula",
                dataType: "string",
                caption: "Formula",
                editorType: "dxTextArea",
                allowEditing: false,
                formItem: {
                    colSpan: 2
                }
            },
            {
                dataField: "description",
                dataType: "string",
                caption: "Description",
                editorType: "dxTextArea",
                formItem: {
                    colSpan: 2
                }
            },
            {
                dataField: "formula_creator_btn",
                caption: "Edit Formula",
                dataType: "string",
                visible: false,
                allowSearch: false
            },
            {
                dataField: "industry_type_id",
                dataType: "string",
                caption: "Industry Type",
                visible: true,
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
                                loadUrl: "/api/General/MasterList/MasterListIdLookup",
                                onBeforeSend: function (method, ajaxOptions) {
                                    ajaxOptions.xhrFields = { withCredentials: true };
                                    ajaxOptions.beforeSend = function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + token);
                                    };
                                }
                            }),
                            filter: ["item_group", "=", "industry-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "created_on",
                caption: "Created On",
                dataType: "string",
                visible: false,
                sortOrder: "asc"
            },
            {
                type: "buttons",
                buttons: ["edit", "delete"]
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
        height: 800,
        showBorders: true,
        editing: {
            mode: "form",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 1,
                items: [
                    {
                        dataField: "sales_charge_name",
                    },
                    {
                        dataField: "charge_type_id",
                    },
                    {
                        dataField: "prerequisite",
                    },
                    {
                        dataField: "charge_formula",
                        height: 100
                    }, 
                    {
                        dataField: "description",
                    },
                    {
                        dataField: "formula_creator_btn",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "Open Formula Editor",
                        },
                        horizontalAlignment: "right",
                    },
                    {
                        dataField: "industry_type_id",
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
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            // Set Formula Creator onchange handler
            if (e.parentType === "dataRow" && e.dataField == "formula_creator_btn") {
                let formula = e.row.data.charge_formula
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onClick = function (e) {
                    let formulaCreator = new FormulaCreator({
                        formula: formula,
                        saveFormulaCallback: function (value) {
                            grid.cellValue(index, "charge_formula", value)
                        }
                    })
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
    }).dxDataGrid("instance");

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "HPB Detail",
                    template: createDetailTabTemplate(masterDetailOptions.data)
                },
            ]
        });
    }

    function createDetailTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "PriceAdjustmentDetail";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/DataGrid/" + encodeURIComponent(currentRecord.id),
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
                    columnMinWidth: 100,
                    columnResizingMode: "widget",
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        {
                            dataField: "sales_contract_product_id",
                            caption: "Contract Product",
                            allowEditing: false,
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Sales/SalesContract/SalesContractIdLookup",
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
                            visible: false,
                        },
                        {
                            dataField: "analyte_id",
                            caption: "Analyte Definition",
                            dataType: "string",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Quality/Analyte/AnalyteIdLookup",
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
                            dataField: "analyte_standard_id",
                            dataType: "string",
                            caption: "Standard",
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
                                        filter: ["item_group", "=", "analyte-standard"]
                                    }
                                },
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                        },
                        //{
                        //    dataField: "value",
                        //    dataType: "number",
                        //    caption: "Value",
                        //    validationRules: [{
                        //        type: "required",
                        //        message: "The field is required."
                        //    }]
                        //},
                        {
                            dataField: "target",
                            dataType: "number",
                            caption: "Typical",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }],
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint",
                                        precision: 2
                                    }
                                }
                            }
                        },
                        {
                            dataField: "minimum",
                            dataType: "number",
                            caption: "Minimum",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint",
                                        precision: 2
                                    }
                                }
                            }
                        },
                        {
                            dataField: "maximum",
                            dataType: "number",
                            caption: "Maximum",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint",
                                        precision: 2
                                    }
                                }
                            }
                        },
                        {
                            dataField: "uom_id",
                            dataType: "string",
                            caption: "Unit",
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
                            dataField: "created_on",
                            caption: "Created On",
                            dataType: "string",
                            visible: false,
                            sortOrder: "desc"
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
                    pager: {
                        allowedPageSizes: [10, 20, 50, 100],
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        showInfo: true,
                        visible: true
                    },
                    height: 450,
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
                                {
                                    dataField: "analyte_id",
                                },
                                {
                                    dataField: "analyte_standard_id",
                                },
                                //{
                                //    dataField: "value",
                                //},
                                {
                                    dataField: "target",
                                    editorType: "dxNumberBox",
                                    editorOptions: {
                                        format: "fixedPoint",
                                        step: 0
                                    }
                                },
                                {
                                    dataField: "minimum",
                                    editorType: "dxNumberBox",
                                    editorOptions: {
                                        format: "fixedPoint",
                                        step: 0
                                    }
                                },
                                {
                                    dataField: "maximum",
                                    editorType: "dxNumberBox",
                                    editorOptions: {
                                        format: "fixedPoint",
                                        step: 0
                                    }
                                },
                                {
                                    dataField: "uom_id",
                                },
                            ]
                        }
                    },
                    onInitNewRow: function (e) {
                        e.data.price_adjustment_id = currentRecord.id
                        e.data.uom_id = "90f4b279a62e4efdb33b8e2d6c292e7d" // Percent Uom Id
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
                });
        }
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