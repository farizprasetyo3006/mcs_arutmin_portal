$(function () {

    var token = $.cookie("Token");
    var areaName = "DailyRecord";
    var entityName = "FuelInventory";
    var url = "/api/" + areaName + "/" + entityName;

    var beginning = 0;
    var qty_in = 0;
    //var processing = 0;
    //var return_cargo = 0;
    var qty_out = 0;
    //var adjustment = 0;
    var ending = 0;

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("fuelInventoryDate1");
    var tgl2 = sessionStorage.getItem("fuelInventoryDate2");

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
            sessionStorage.setItem("fuelInventoryDate1", formatTanggal(firstDay));
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
            sessionStorage.setItem("fuelInventoryDate2", formatTanggal(lastDay));
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
                dataField: "date_time",
                dataType: "datetime",
                caption: "Date",
                validationRules: [{
                    type: "required",
                    message: "The Date field is required."
                }],
                formItem: {
                    colSpan: 2,
                },
                format: "yyyy-MM-dd",
                sortOrder: "asc"
            },
            {
                dataField: "beginning",
                dataType: "number",
                caption: "Beginning",
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
            {
                dataField: "qty_in",
                dataType: "number",
                caption: "In",
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
                    },
                },
            },
            {
                dataField: "qty_out",
                dataType: "number",
                caption: "Out",
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
            {
                dataField: "ending",
                dataType: "number",
                caption: "Ending",
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
                    },
                },
            },
            {
                dataField: "master_list_id",
                dataType: "string",
                caption: "Type",
                formItem: {
                    colSpan: 2,
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
                            filter: ["item_group", "=", "fuel-inventory"]
                        }
                    },
                    valueExpr: "value",
                    displayExpr: "text"
                },
                sortOrder: "asc"
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
                    displayExpr: "text"
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
        onEditorPreparing: function (e) {
            if (e.parentType === "dataRow" && e.dataField == "date_time") {
                let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;

                e.editorOptions.onValueChanged = function (e) {
                    var cargoDate = e.value.toISOString();

                    $.ajax({
                        url: url + "/GetLastEndingByDate/" + encodeURIComponent(cargoDate),
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            console.log(response);

                            if (response != null && response != undefined) {
                                let record = response;
                                grid.cellValue(index, "beginning", record.ending);
                                grid.cellValue(index, "ending", record.ending);
                            }
                        }
                    })

                    standardHandler(e);
                }
            }
            //if (e.parentType === "dataRow" && e.dataField == "cargo_date") {
            //    let standardHandler = e.editorOptions.onValueChanged;
            //    let index = e.row.rowIndex;
            //    let grid = e.component;

            //    e.editorOptions.onValueChanged = function (e) {
            //        var cargoDate = e.value.toISOString();

            //        $.ajax({
            //            url: url + "/GetLastEndingByDate/" + encodeURIComponent(cargoDate),
            //            type: 'GET',
            //            contentType: "application/json",
            //            beforeSend: function (xhr) {
            //                xhr.setRequestHeader("Authorization", "Bearer " + token);
            //            },
            //            success: function (response) {
            //                console.log(response);

            //                if (response != null && response != undefined) {
            //                    let record = response;
            //                    grid.cellValue(index, "beginning", record.ending);
            //                    grid.cellValue(index, "ending", record.ending);
            //                }
            //            }
            //        })

            //        standardHandler(e);
            //    }
            //}

            /*if (e.parentType === "dataRow" && (e.dataField == "beginning" || e.dataField == "hauling" || e.dataField == "processing" ||
                e.dataField == "return_cargo" || e.dataField == "loading_to_barge" || e.dataField == "adjustment")) {
                //let standardHandler = e.editorOptions.onValueChanged;
                let index = e.row.rowIndex;
                let grid = e.component;

                e.editorOptions.onValueChanged = function (inputan) {
                    var beginning = e.row.data.beginning ?? 0;
                    var hauling = e.row.data.hauling ?? 0;
                    var processing = e.row.data.processing ?? 0;
                    var return_cargo = e.row.data.return_cargo ?? 0;
                    var loading_to_barge = e.row.data.loading_to_barge ?? 0;
                    var adjustment = e.row.data.adjustment ?? 0;

                    switch (e.dataField) {
                        case "beginning":
                            beginning = inputan.value;
                            grid.cellValue(index, "beginning", beginning);
                            break;
                        case "hauling":
                            hauling = inputan.value;
                            grid.cellValue(index, "hauling", hauling);
                            break;
                        case "processing":
                            processing = inputan.value;
                            grid.cellValue(index, "processing", processing);
                            break;
                        case "return_cargo":
                            return_cargo = inputan.value;
                            grid.cellValue(index, "return_cargo", return_cargo);
                            break;
                        case "loading_to_barge":
                            loading_to_barge = inputan.value;
                            grid.cellValue(index, "loading_to_barge", loading_to_barge);
                            break;
                        case "adjustment":
                            adjustment = inputan.value;
                            grid.cellValue(index, "adjustment", adjustment);
                            break;
                    }

                    var ending = beginning + hauling + processing + return_cargo - loading_to_barge + adjustment;
                    grid.cellValue(index, "ending", ending);
                }
            } */

            if (e.parentType === "dataRow" && (e.dataField == "beginning" || e.dataField == "qty_in" || e.dataField == "qty_out")) {
                let index = e.row.rowIndex;
                let grid = e.component;

                e.editorOptions.onValueChanged = function (inputan) {
                    var beginning = e.row.data.beginning ?? 0;
                    var qty_in = e.row.data.qty_in ?? 0;
                    var qty_out = e.row.data.qty_out ?? 0;

                    switch (e.dataField) {
                        case "beginning":
                            beginning = inputan.value;
                            grid.cellValue(index, "beginning", beginning);
                            break;
                        case "qty_in":
                            qty_in = inputan.value;
                            grid.cellValue(index, "qty_in", qty_in);
                            break;
                        case "qty_out":
                            qty_out = inputan.value;
                            grid.cellValue(index, "qty_out", qty_out);
                            break;
                    }

                    var ending = beginning + qty_in - qty_out;
                    grid.cellValue(index, "ending", ending);
                }
            }

            //if (e.parentType === "dataRow" && (e.dataField == "hauling_from_date" || e.dataField == "hauling_to_date")) {
            //    let index = e.row.rowIndex;
            //    let grid = e.component;

            //    e.editorOptions.onValueChanged = function (inputan) {
            //        beginning = e.row.data.beginning ?? 0;
            //        hauling = e.row.data.hauling ?? 0;
            //        processing = e.row.data.processing ?? 0;
            //        return_cargo = e.row.data.return_cargo ?? 0;
            //        loading_to_barge = e.row.data.loading_to_barge ?? 0;
            //        adjustment = e.row.data.adjustment ?? 0;

            //        var fromDate = e.row.data.hauling_from_date ?? null;
            //        var toDate = e.row.data.hauling_to_date ?? null;

            //        if (e.dataField == "hauling_from_date") {
            //            fromDate = inputan.value?.toISOString() ?? null;
            //            //grid.cellValue(index, "hauling_from_date", fromDate);
            //            grid.cellValue(index, "hauling_from_date", inputan.value);
            //        }
            //        else {
            //            toDate = inputan.value?.toISOString() ?? null;
            //            //grid.cellValue(index, "hauling_to_date", toDate);
            //            grid.cellValue(index, "hauling_to_date", inputan.value);
            //        }

            //        if (fromDate != null && toDate != null) {
            //            $.ajax({
            //                url: url + "/GetHaulingByDate/" + encodeURIComponent(fromDate) + "/" + encodeURIComponent(toDate),
            //                type: 'GET',
            //                contentType: "application/json",
            //                beforeSend: function (xhr) {
            //                    xhr.setRequestHeader("Authorization", "Bearer " + token);
            //                },
            //                success: function (response) {
            //                    hauling = response?.data[0]?.loading_quantity ?? 0;
            //                    grid.cellValue(index, "hauling", hauling);

            //                    ending = beginning + hauling + processing + return_cargo - loading_to_barge + adjustment;
            //                    grid.cellValue(index, "ending", ending);
            //                }
            //            })
            //        }
            //    }
            //}

            //if (e.parentType === "dataRow" && (e.dataField == "barging_from_date" || e.dataField == "barging_to_date")) {
            //    let index = e.row.rowIndex;
            //    let grid = e.component;

            //    e.editorOptions.onValueChanged = function (inputan) {
            //        beginning = e.row.data.beginning ?? 0;
            //        hauling = e.row.data.hauling ?? 0;
            //        processing = e.row.data.processing ?? 0;
            //        return_cargo = e.row.data.return_cargo ?? 0;
            //        loading_to_barge = e.row.data.loading_to_barge ?? 0;
            //        adjustment = e.row.data.adjustment ?? 0;

            //        var fromDate = e.row.data.barging_from_date ?? null;
            //        var toDate = e.row.data.barging_to_date ?? null;

            //        if (e.dataField == "barging_from_date") {
            //            fromDate = inputan.value?.toISOString() ?? null;
            //            //grid.cellValue(index, "barging_from_date", fromDate);
            //            grid.cellValue(index, "barging_from_date", inputan.value);
            //        }
            //        else {
            //            toDate = inputan.value?.toISOString() ?? null;
            //            //grid.cellValue(index, "barging_to_date", toDate);
            //            grid.cellValue(index, "barging_to_date", inputan.value);
            //        }

            //        if (fromDate != null && toDate != null) {
            //            $.ajax({
            //                url: url + "/GetBargingByDate/" + encodeURIComponent(fromDate) + "/" + encodeURIComponent(toDate),
            //                type: 'GET',
            //                contentType: "application/json",
            //                beforeSend: function (xhr) {
            //                    xhr.setRequestHeader("Authorization", "Bearer " + token);
            //                },
            //                success: function (response) {
            //                    loading_to_barge = response?.data[0]?.quantity ?? 0;
            //                    grid.cellValue(index, "loading_to_barge", loading_to_barge);

            //                    ending = beginning + hauling + processing + return_cargo - loading_to_barge + adjustment;
            //                    grid.cellValue(index, "ending", ending);
            //                }
            //            });

            //            $.ajax({
            //                url: url + "/GetReturnCargoByDate/" + encodeURIComponent(fromDate) + "/" + encodeURIComponent(toDate),
            //                type: 'GET',
            //                contentType: "application/json",
            //                beforeSend: function (xhr) {
            //                    xhr.setRequestHeader("Authorization", "Bearer " + token);
            //                },
            //                success: function (response) {
            //                    return_cargo = response?.data[0]?.quantity ?? 0;
            //                    grid.cellValue(index, "return_cargo", return_cargo);

            //                    ending = beginning + hauling + processing + return_cargo - loading_to_barge + adjustment;
            //                    grid.cellValue(index, "ending", ending);
            //                }
            //            });

            //            $.ajax({
            //                url: url + "/GetAdjustmentByDate/" + encodeURIComponent(fromDate) + "/" + encodeURIComponent(toDate),
            //                type: 'GET',
            //                contentType: "application/json",
            //                beforeSend: function (xhr) {
            //                    xhr.setRequestHeader("Authorization", "Bearer " + token);
            //                },
            //                success: function (response) {
            //                    adjustment = response?.data[0]?.quantity ?? 0;
            //                    grid.cellValue(index, "adjustment", adjustment);

            //                    ending = beginning + hauling + processing + return_cargo - loading_to_barge + adjustment;
            //                    grid.cellValue(index, "ending", ending);
            //                }
            //            });
            //        }
            //    }
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
                url: "/api/DailyRecord/FuelInventory/UploadDocument",
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