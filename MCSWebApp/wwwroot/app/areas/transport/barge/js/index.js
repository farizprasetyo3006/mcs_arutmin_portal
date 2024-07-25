﻿$(function () {

    var token = $.cookie("Token");
    var areaName = "Transport";
    var entityName = "Barge";
    var url = "/api/" + areaName + "/" + entityName;
    var isInserting = true;

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
        selection: {
            mode: "multiple"
        },
        remoteOperations: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "business_area_id",
                dataType: "text",
                caption: "Business Area",
                width: "220px",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/BusinessAreaIdLookup",
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
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
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
                }
            },
            {
                dataField: "vehicle_name",
                dataType: "string",
                caption: "Barge Name",
                width: "150px",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                editorType: "dxTextBox",
                editorOptions: {
                    onInitialized: function (e) {
                        if (isInserting) { // Assuming isInserting is a boolean flag indicating whether it's a new row
                            e.component.option("value", "BG. ");
                        }
                    },
                    onInput: function (e) {
                        if (!e.component.option("value").startsWith("BG. ")) {
                            e.component.option("value", "BG. " + e.event.originalEvent.data);
                        }
                    }
                },
                sortOrder: "asc"
            },
            {
                dataField: "vehicle_id",
                dataType: "string",
                caption: "Barge Code",
                validationRules: [{
                    type: "required",
                    message: "The Capacity UoM field is required."
                }],
            },
            //{
            //    dataField: "vehicle_id",
            //    dataType: "string",
            //    caption: "Barge Code",
            //    visible: false,
            //},
            {
                dataField: "capacity",
                dataType: "number",
                caption: "Capacity",
                validationRules: [{
                    type: "required",
                    message: "The Capacity field is required."
                }],
            },
            {
                dataField: "capacity_uom_id",
                dataType: "text",
                caption: "Capacity Unit",
                validationRules: [{
                    type: "required",
                    message: "The Capacity UoM field is required."
                }],
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
            },
            {
                dataField: "vendor_id",
                dataType: "text",
                caption: "Owner",
                width: "130px",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/VendorIdLookup",
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
                dataField: "vehicle_make",
                dataType: "string",
                caption: "Make",
                visible: false,
            },
            {
                dataField: "vehicle_model",
                dataType: "string",
                caption: "Barge Size",
                visible: false,
            },
            {
                dataField: "vehicle_model_year",
                dataType: "number",
                caption: "Model Year",
            },
            {
                dataField: "vehicle_manufactured_year",
                dataType: "number",
                caption: "Manufactured Year",
                visible: false,
            },

            {
                dataField: "tug_id",
                dataType: "text",
                caption: "T u g",
                width: "130px",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/TugIdLookup",
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
            /*{
                dataField: "trip_plan",
                dataType: "number",
                caption: "Trip Plan",
            },*/

            //{
            //    dataField: "is_floating_stockpile",
            //    dataType: "boolean",
            //    caption: "Is Floating Stockpile",
            //    hidingPriority: 7
            //},
            //{
            //    dataField: "barge_status",
            //    dataType: "number",
            //    caption: "Barge Status",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: url + "/BargeStatusLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    }
            //}
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
        onEditingStart: function (e) {
            isInserting = false;
        },
        onInitNewRow: function (e) {
            isInserting = true
        },
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0) {
                selectedIds = $.map(data, function (value) {
                    return value.id;
                }).join(",");
                $("#dropdown-download-selected").removeClass("disabled");
            }
            else {
                $("#dropdown-download-selected").addClass("disabled");
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
            if (e.parentType === "dataRow" && e.dataField == "vehicle_id") {
                e.editorOptions.disabled = e.row.data.vehicle_id != "" && e.row.data.vehicle_id != undefined
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

    $('#btnDownloadSelectedRow').on('click', function () {
        if (selectedIds != null && selectedIds != '') {
            let payload = {};
            payload.selectedIds = selectedIds;
            $('#btnDownloadSelectedRow')
                .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Downloading, please wait ...');
            $.ajax({
                url: "/Transport/Barge/ExcelExport",
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
                if (data instanceof Blob) {
                    var a = document.createElement('a');
                    var url = window.URL.createObjectURL(data);
                    a.href = url;
                    a.download = "Barge.xlsx"; // Set the appropriate file name here
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                    toastr["success"]("File downloaded successfully.");
                } else {
                    toastr["error"]("File download failed.");
                }
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
                url: "/api/Transport/Barge/UploadDocument",
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