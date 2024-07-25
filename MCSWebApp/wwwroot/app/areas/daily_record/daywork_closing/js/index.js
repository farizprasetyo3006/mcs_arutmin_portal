$(function () {
    var token = $.cookie("Token");
    var areaName = "DailyRecord";
    var entityName = "DayworkClosing";
    var url = "/api/" + areaName + "/" + entityName;

    var from_date, to_date, customer_id, reference_number

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
             
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
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
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
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
                validationRules: [{
                    type: "required",
                    message: "Advance Contract Reference is required."
                }],
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
                dataField: "customer_id",
                dataType: "string",
                caption: "Customer",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/ContractorIdLookup",
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
                setCellValue: function (rowData, value) {
                    rowData.customer_id = value
                }
            },
            {
                dataField: "reference_number",
                dataType: "text",
                caption: "Reference Number",
                validationRules: [{
                    type: "required",
                    message: "Reference Number is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var customer_id = ""

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.customer_id !== undefined
                                    && options.data.customer_id !== null) {
                                    customer_id = options.data.customer_id
                                }
                            }
                        }
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/DayworkReferenceLookup?CustomerId=" + customer_id,
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
                format: "yyyy-MM-dd HH:mm:ss"
            },
            {
                dataField: "to_date",
                dataType: "datetime",
                caption: "To Date",
                format: "yyyy-MM-dd HH:mm:ss"
            },
            {
                dataField: "total_hm",
                dataType: "number",
                caption: "Total HM",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                allowEditing: false,
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                }
            },
            {
                dataField: "total_value",
                dataType: "number",
                caption: "Total Value",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                allowEditing: false,
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                }
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                allowEditing: true,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
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
                var detailName = "DayworkClosing";

                $("<div>")
                    .addClass("master-detail-caption")
                    .text("Detail")
                    .appendTo(container);

                $("<div>")
                    .dxDataGrid({
                        dataSource: DevExpress.data.AspNet.createStore({
                            key: "id",
                            loadUrl: url + "/DayworkLookupDetail?CustomerId=" + currentRecord.customer_id
                                + "&ReferenceNumber=" + currentRecord.reference_number
                                + "&FromDate=" + currentRecord.from_date + "&ToDate=" + currentRecord.to_date,
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
                        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                        columns: [
                            {
                                dataField: "transaction_date",
                                dataType: "date",
                                caption: "Transaction Date"
                            },
                            {
                                dataField: "equipment_code",
                                dataType: "string",
                                caption: "Equipment",
                            },
                            {
                                dataField: "accounting_period_id",
                                dataType: "text",
                                caption: "Accounting Period",
                                validationRules: [{
                                    type: "required",
                                    message: "This field is required."
                                }],
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
                                dataField: "reference_number",
                                dataType: "string",
                                caption: "Reference Number"
                            },
                            {
                                dataField: "hm_start",
                                dataType: "number",
                                caption: "HM Start",
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
                                    }
                                },
                                validationRules: [{
                                    type: "required",
                                    message: "This field is required."
                                }]
                            },
                            {
                                dataField: "hm_end",
                                dataType: "number",
                                caption: "HM End",
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
                                    }
                                },
                                validationRules: [{
                                    type: "required",
                                    message: "This field is required."
                                }]
                            },
                            {
                                dataField: "hm_duration",
                                dataType: "number",
                                caption: "HM Duration",
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
                                    }
                                },
                                allowEditing: false
                            },
                            {
                                dataField: "hm_rate",
                                dataType: "number",
                                caption: "Rate",
                                format: {
                                    type: "fixedPoint",
                                    precision: 2
                                },
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            },
                            {
                                dataField: "hm_value",
                                dataType: "number",
                                caption: "Value",
                                format: {
                                    type: "fixedPoint",
                                    precision: 2
                                },
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    format: "fixedPoint",
                                    step: 0
                                }
                            },
                            {
                                dataField: "note",
                                dataType: "string",
                                caption: "Note"
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
                            allowAdding: false,
                            allowUpdating: false,
                            allowDeleting: false,
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
            if (e.parentType === "dataRow" && e.dataField == "reference_number") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    sessionStorage.setItem("reference_number", e.value);
                    from_date = sessionStorage.getItem("from_date");
                    to_date = sessionStorage.getItem("to_date");
                    reference_number = sessionStorage.getItem("reference_number");
                    customer_id = sessionStorage.getItem("customer_id");

                    let cari = url + '/DayworkTotal?FromDate=' + from_date + '&ToDate=' + to_date + '&CustomerId=' + customer_id + '&ReferenceNumber=' + reference_number;
                    $.ajax({
                        url: cari,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            if (response == null || response == undefined) {
                                grid.cellValue(index, "total_hm", 0);
                                grid.cellValue(index, "total_value", 0);
                            }
                            else {
                                let record = response.data[0];
                                // Set its corresponded field's value
                                grid.cellValue(index, "total_hm", record.hm_duration);
                                grid.cellValue(index, "total_value", record.hm_value);
                            }
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "customer_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    sessionStorage.setItem("customer_id", e.value);

                    from_date = sessionStorage.getItem("from_date");
                    to_date = sessionStorage.getItem("to_date");
                    reference_number = sessionStorage.getItem("reference_number");
                    customer_id = sessionStorage.getItem("customer_id");

                    let cari = url + '/DayworkTotal?FromDate=' + from_date + '&ToDate=' + to_date + '&CustomerId=' + customer_id + '&ReferenceNumber=' + reference_number;
                    $.ajax({
                        url: cari,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            if (response == null || response == undefined) {
                                grid.cellValue(index, "total_hm", 0);
                                grid.cellValue(index, "total_value", 0);
                            }
                            else {
                                let record = response.data[0];
                                // Set its corresponded field's value
                                grid.cellValue(index, "total_hm", record.hm_duration);
                                grid.cellValue(index, "total_value", record.hm_value);
                            }
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "from_date") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    sessionStorage.setItem("from_date", e.value.toISOString());
                    sessionStorage.setItem("to_date", e.value.toISOString());

                    from_date = sessionStorage.getItem("from_date");
                    to_date = sessionStorage.getItem("to_date");
                    reference_number = sessionStorage.getItem("reference_number");
                    customer_id = sessionStorage.getItem("customer_id");

                    let cari = url + '/DayworkTotal?FromDate=' + from_date + '&ToDate=' + to_date + '&CustomerId=' + customer_id + '&ReferenceNumber=' + reference_number;
                    $.ajax({
                        url: cari,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            if (response == null || response == undefined) {
                                grid.cellValue(index, "total_hm", 0);
                                grid.cellValue(index, "total_value", 0);
                            }
                            else {
                                let record = response.data[0];
                                // Set its corresponded field's value
                                grid.cellValue(index, "total_hm", record.hm_duration);
                                grid.cellValue(index, "total_value", record.hm_value);
                            }
                        }
                    })

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "to_date") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    sessionStorage.setItem("to_date", e.value.toISOString());

                    from_date = sessionStorage.getItem("from_date");
                    to_date = sessionStorage.getItem("to_date");
                    reference_number = sessionStorage.getItem("reference_number");
                    customer_id = sessionStorage.getItem("customer_id");

                    let cari = url + '/DayworkTotal?FromDate=' + from_date + '&ToDate=' + to_date + '&CustomerId=' + customer_id + '&ReferenceNumber=' + reference_number;
                    $.ajax({
                        url: cari,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            if (response == null || response == undefined) {
                                grid.cellValue(index, "total_hm", 0);
                                grid.cellValue(index, "total_value", 0);
                            }
                            else {
                                let record = response.data[0];
                                // Set its corresponded field's value
                                grid.cellValue(index, "total_hm", record.hm_duration);
                                grid.cellValue(index, "total_value", record.hm_value);
                            }                        }
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
                url: "/api/Mining/CoalMinedClosing/UploadDocument",
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