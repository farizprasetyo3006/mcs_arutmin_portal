$(function () {

    var token = $.cookie("Token");
    var areaName = "General";
    var entityName = "CurrencyExchange";
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
                dataField: "source_currency_id",
                dataType: "string",
                caption: "Source Currency",
                validationRules: [{
                    type: "required",
                    message: "The Source Currency field is required."
                }],
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
                sortOrder: "asc"
            },
            {
                dataField: "target_currency_id",
                dataType: "string",
                caption: "Target Currency",
                validationRules: [{
                    type: "required",
                    message: "The Target Currency field is required."
                }],
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
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Start Date",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "End Date",
            },
            {
                dataField: "exchange_rate",
                dataType: "number",
                caption: "Exchange Rate",
                validationRules: [{
                    type: "required",
                    message: "The Exchange Rate field is required."
                }],
            },
            {
                dataField: "selling_rate",
                dataType: "number",
                caption: "Selling Rate",
                validationRules: [{
                    type: "required",
                    message: "The Selling Rate field is required."
                }],
            },
            {
                dataField: "buying_rate",
                dataType: "number",
                caption: "Buying Rate",
                validationRules: [{
                    type: "required",
                    message: "The Buying Rate field is required."
                }],
            },
            {
                dataField: "exchange_type_id",
                dataType: "string",
                caption: "Exchange Type",
                validationRules: [{
                    type: "required",
                    message: "The Exchange Type field is required."
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
                            filter: ["item_group", "=", "exchange-type"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
        ],
        onRowValidating: function (e) {
            if (!e.brokenRules.length)
                return false

            // Ref: https://supportcenter.devexpress.com/ticket/details/t512157/how-to-automatically-focus-the-first-non-validated-field
            e.brokenRules[0].validator.focus();
        },
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "selling_rate") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    

                    let sellingRateNominal = e.value

                    grid.beginUpdate()
                    // Set its corresponded field's value
                    grid.cellValue(index, "selling_rate", sellingRateNominal)
                    grid.cellValue(index, "exchange_rate", (sellingRateNominal + rowData.buying_rate)/2)
                    grid.endUpdate()

                    grid.beginCustomLoading()
                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500)
                    standardHandler(e) // Calling the standard handler to save the edited value
                }

            }

            if (e.parentType === "dataRow" && e.dataField == "buying_rate") {

                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    

                    let buyingRateNominal = e.value

                    grid.beginUpdate()
                    // Set its corresponded field's value
                    grid.cellValue(index, "buying_rate", buyingRateNominal)
                    grid.cellValue(index, "exchange_rate", (buyingRateNominal + rowData.selling_rate) / 2)
                    grid.endUpdate()

                    grid.beginCustomLoading()
                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500)
                    standardHandler(e) // Calling the standard handler to save the edited value
                }

            }

            // //console.log(e)

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
            useIcons: true,
            form: {
                colCount: 2,
                items: [
                    {
                        dataField: "source_currency_id",
                    },
                    {
                        dataField: "target_currency_id",
                    },
                    {
                        dataField: "start_date"
                    },
                    {
                        dataField: "end_date",
                    },
                    {
                        dataField: "selling_rate",
                        colSpan: 2
                    },
                    {
                        dataField: "buying_rate",
                        colSpan: 2
                    },
                    {
                        dataField: "exchange_rate",
                        colSpan: 2
                    },
                    {
                        dataField: "exchange_type_id",
                        colSpan: 2
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
                url: "/api/General/CurrencyExchange/UploadDocument",
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