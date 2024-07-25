$(function () {
    var token = $.cookie("Token");
    var url = "/api/General/MasterList";

    //const itemGroups = [
    //    {
    //        value: "contract-basis",
    //        text: "Contract Basis"
    //    },
    //    {
    //        value: "commitment",
    //        text: "Commitment"
    //    },
    //    {
    //        value: "contract-status",
    //        text: "Contract Status"
    //    },
    //    {
    //        value: "quotation-type",
    //        text: "Quotation Type"
    //    },
    //    {
    //        value: "quotation-period",
    //        text: "Quotation Period"
    //    },
    //    {
    //        value: "pricing-method",
    //        text: "Pricing Method"
    //    },
    //    {
    //        value: "invoice-type",
    //        text: "Invoice Type"
    //    },
    //    {
    //        value: "payment-method",
    //        text: "Payment Method"
    //    },
    //    {
    //        value: "reference-date",
    //        text: "Reference Date"
    //    },
    //    {
    //        value: "days-type",
    //        text: "Days Type"
    //    },
    //    {
    //        value: "analyte-standard",
    //        text: "Analyte Standard"
    //    },
    //    {
    //        value: "fulfilment-type",
    //        text: "Fulfilment Type"
    //    },
    //    {
    //        value: "sales-type",
    //        text: "Sales Type"
    //    },
    //    {
    //        value: "invoice-target",
    //        text: "Invoice Target"
    //    },
    //    {
    //        value: "delivery-term",
    //        text: "Delivery Term"
    //    },
    //    {
    //        value: "frequency",
    //        text: "Frequency"
    //    },
    //    {
    //        value: "rounding-type",
    //        text: "Rounding Type"
    //    },
    //    {
    //        value: "charge-type",
    //        text: "Charge Type"
    //    },
    //    {
    //        value: "site",
    //        text: "Site"
    //    },
    //    {
    //        value: "owner",
    //        text: "Owner"
    //    },
    //    {
    //        value: "activity",
    //        text: "Activity"
    //    },
    //    {
    //        value: "document-type",
    //        text: "Document Type"
    //    },
    //    {
    //        value: "reserved-words",
    //        text: "Reserved Words"
    //    },
    //    {
    //        value: "reserved-functions",
    //        text: "Reserved Functions"
    //    },
    //    {
    //        value: "explosive-type",
    //        text: "Explosive Type"
    //    },
    //    {
    //        value: "explosive-accessory",
    //        text: "Explosive Accessory"
    //    },
    //    {
    //        value: "timesheet-activity",
    //        text: "Timesheet Activity"
    //    },
    //    {
    //        value: "transport-model",
    //        text: "Transportation Model"
    //    },
    //    {
    //        value: "desdem-invoice-status",
    //        text: "DesDem Invoice Status"
    //    },
    //    {
    //        value: "contract-item-class",
    //        text: "Contract Item Class"
    //    },
    //    {
    //        value: "day-work-event",
    //        text: "Day Work Event"
    //    },
    //    {
    //        value: "credit-limit-alert-range",
    //        text: "Range of Credit Limit Alert"
    //    },
    //    {
    //        value: "credit-limit-alert-value",
    //        text: "Value of Credit Limit Alert"
    //    },
    //    {
    //        value: "years",
    //        text: "Years"
    //    },
    //    //loading type
    //    {
    //        value: "loading-type",
    //        text: "Loading Type"
    //    },
    //    {
    //        value: "exchange-type",
    //        text: "Exchange Type"
    //    },
    //    //Shipping Instruction
    //    {
    //        value: "si-detail-jasa-pekerjaan",
    //        text: "SI Detail Jasa Pekerjaan"
    //    },
    //    {
    //        value: "si-detail-survey-dokumen",
    //        text: "SI Detail Survey Dokumen"
    //    },
    //    {
    //        value: "si-pekerjaan-shipping-agent",
    //        text: "SI Pekerjaan Shipping Agent"
    //    },
    //    {
    //        value: "si-dokumen-shipping-agent",
    //        text: "SI Dokumen Shipping Agent"
    //    },
    //    {
    //        value: "si-type",
    //        text: "SI Type"
    //    },
    //    {
    //        value: "index-calculation",
    //        text: "Index Calculation"
    //    },
    //    {
    //        value: "plan-type",
    //        text: "Plan Type"
    //    },
    //    {
    //        value: "sampling-type",
    //        text: "Sampling Type"
    //    },
    //    {
    //        value: "delay-information",
    //        text: "Delay Information"
    //    },
    //    {
    //        value: "business-process",
    //        text: "Business Process"
    //    },
    //    {
    //        value: "hauling-plan-type",
    //        text: "Hauling Plan Type"
    //    },
    //    {
    //        value: "barging-plan-type",
    //        text: "Barging Plan Type"
    //    },
    //    {
    //        value: "business-area-index",
    //        text: "Business Area Index"
    //    },
    //]

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
            //{
            //    dataField: "item_group",
            //    dataType: "string",
            //    caption: "Item Group",
            //    validationRules: [{
            //        type: "required",
            //        message: "The Item Group field is required."
            //    }],
            //    lookup: {
            //        dataSource: DevExpress.data.query(itemGroups).sortBy("text", false).toArray(),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    groupIndex: 0
            //},
            {
                dataField: "item_group",
                dataType: "string",
                caption: "Item Group",
                validationRules: [{
                    type: "required",
                    message: "The Item Group field is required."
                }],
                groupIndex: 0,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/General/ItemGroup/MasterListItemGroupLookup",
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
                setCellValue: async function (newData, value) {
                    this.defaultSetCellValue(newData, value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "item_name",
                dataType: "string",
                caption: "Item Name",
                validationRules: [{
                    type: "required",
                    message: "The Item Name field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "item_in_coding",
                dataType: "string",
                caption: "Item Code"
            },
            {
                dataField: "sort_number",
                dataType: "numeric",
                caption: "Sort Number"
            },
            {
                dataField: "notes",
                dataType: "string",
                caption: "Notes"
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
                        dataField: "item_group",
                    },
                    {
                        dataField: "item_name",
                    },
                    {
                        dataField: "item_in_coding",
                    },
                    {
                        dataField: "sort_number",
                    },
                    {
                        dataField: "notes",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 50
                        },
                        colSpan: 2
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
        onEditorPreparing: function (e) {
            if (e.parentType == 'dataRow' && e.dataField == 'item_in_coding') {
                e.editorOptions.maxLength = 20;
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
                url: "/api/General/MasterList/UploadDocument",
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