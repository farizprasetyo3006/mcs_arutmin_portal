$(function () {
    var token = $.cookie("Token");
    var areaName = "Port";
    var entityName = "ShippingClosing";
    var url = "/api/" + areaName + "/" + entityName;    

    var from_date = null;
    var to_date = null;

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
            },
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "transaction_number",
                dataType: "string",
                caption: "Transaction Number",
                validationRules: [{
                    type: "required",
                    message: "Transaction Number is required."
                }],
                formItem: {
                    colSpan: 2
                },
                sortOrder: "asc"
            },
            {
                dataField: "transaction_date",
                dataType: "date",
                caption: "Transaction Date",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "accounting_period_id",
                dataType: "text",
                caption: "Accounting Period",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Accounting/AccountingPeriod/AccountingPeriodIdLookup",
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
            },
            {
                dataField: "advance_contract_id",
                dataType: "text",
                caption: "Advance Contract",
                formItem: {
                    colSpan: 2
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/ContractManagement/AdvanceContract/AdvanceContractIdLookup",
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
                    rowData.advance_contract_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "advance_contract_reference_id",
                dataType: "text",
                caption: "Advance Contract Reference",
                formItem: {
                    colSpan: 2
                },
                lookup: {
                    dataSource: function (options) {
                        var advId = "";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.advance_contract_id !== undefined
                                    && options.data.advance_contract_id !== null) {
                                    advId = options.data.advance_contract_id
                                }
                            }
                        }
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/ContractManagement/AdvanceContractReference/AdvanceContractReferenceIdLookupByAdvanceContractId?advance_contract_id=" + advId,
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
                visible: false
            },
            {
                dataField: "from_date",
                dataType: "datetime",
                caption: "From Date",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "to_date",
                dataType: "datetime",
                caption: "To Date",
                format: "yyyy-MM-dd HH:mm"
            },
            {
                dataField: "volume",
                dataType: "number",
                caption: "Volume",
                allowEditing: false,
            },
            {
                dataField: "distance",
                dataType: "number",
                caption: "Distance (meter)"
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
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {
                dataField: "note",
                dataType: "string",
                caption: "Note",
                formItem: {
                    colSpan: 2
                },
            }
        ],

        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentRecord = options.data;

                $("<div>")
                    .addClass("master-detail-caption")
                    .text("Detail")
                    .appendTo(container);

                $("<div>")
                    .dxDataGrid({
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "id",
                            loadUrl: url + "/DataGridItem?Id=" + encodeURIComponent(currentRecord.id),
                            insertUrl: url + "/InsertItemData",
                            updateUrl: url + "/UpdateItemData",
                            deleteUrl: url + "/DeleteItemData",
                            onBeforeSend: function (method, ajaxOptions) {
                                ajaxOptions.xhrFields = { withCredentials: true };
                                ajaxOptions.beforeSend = function (request) {
                                    request.setRequestHeader("Authorization", "Bearer " + token);
                                };
                            },
                        }),
                        remoteOperations: true,
                        allowColumnResizing: true,
                        columnResizingMode: "widget",
                        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                        columns: [
                            {
                                dataField: "transaction_item_date",
                                dataType: "date",
                                caption: "Loading Date"
                                //validationRules: [{
                                //    type: "required",
                                //    message: "This field is required."
                                //}]
                            },
                            {
                                dataField: "business_area_id",
                                dataType: "text",
                                caption: "Business Area",
                                //validationRules: [{
                                //    type: "required",
                                //    message: "This field is required."
                                //}],
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
                                    displayExpr: "text"
                                },
                                setCellValue: function (rowData, value) {
                                    rowData.business_area_id = value;
                                },
                                calculateSortValue: function (data) {
                                    var value = this.calculateCellValue(data);
                                    return this.lookup.calculateCellValue(value);
                                }
                            },
                            {
                                dataField: "mine_location_id",
                                dataType: "text",
                                caption: "Mine Location",
                                //validationRules: [{
                                //    type: "required",
                                //    message: "This field is required."
                                //}],
                                lookup: {
                                    dataSource: function (options) {
                                        return {
                                            store: DevExpress.data.AspNet.createStore({
                                                key: "value",
                                                loadUrl: "/api/Mining/WasteRemoval/SourceLocationIdLookup",
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
                                }
                            },
                            {
                                dataField: "quantity_item",
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
                                validationRules: [{
                                    type: "required",
                                    message: "This field is required."
                                }]
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
                            e.data.shipping_closing_id = currentRecord.id;
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
                    }).appendTo(container);
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
            if (e.parentType === "dataRow" && e.dataField === "from_date") {

                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    if (e.value !== null && e.value !== undefined) {
                        from_date = new Date(e.value);
                    };

                    if (from_date !== null && to_date !== null) {
                        let cari = "/api/Port/ShippingClosing/ProductionTotal"
                            + "?FromDate=" + encodeURIComponent(from_date.toISOString())
                            + "&ToDate=" + encodeURIComponent(to_date.toISOString());
                        $.ajax({
                            url: cari,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                ////console.log(response.data);
                                let record = response.data[0];
                                // Set its corresponded field's value
                                grid.cellValue(index, "from_date", from_date);
                                grid.cellValue(index, "to_date", to_date)
                                grid.cellValue(index, "volume", record.quantity);
                                grid.cellValue(index, "distance", record.distance)
                            }
                        });
                    };

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            };

            if (e.parentType === "dataRow" && e.dataField === "to_date") {

                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    if (e.value !== null && e.value !== undefined) {
                        to_date = new Date(e.value);
                    };

                    if (from_date !== null && to_date !== null) {
                        let cari = "/api/Port/ShippingClosing/ProductionTotal"
                            + "?FromDate=" + encodeURIComponent(from_date.toISOString())
                            + "&ToDate=" + encodeURIComponent(to_date.toISOString());
                        $.ajax({
                            url: cari,
                            type: 'GET',
                            contentType: "application/json",
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", "Bearer " + token);
                            },
                            success: function (response) {
                                let record = response.data[0];
                                // Set its corresponded field's value
                                grid.cellValue(index, "from_date", from_date);
                                grid.cellValue(index, "to_date", to_date)
                                grid.cellValue(index, "volume", record.unloading_quantity);
                                grid.cellValue(index, "distance", record.distance)
                            }
                        });
                    };

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            };
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
                url: "/api/Mining/ShippingClosing/UploadDocument",
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