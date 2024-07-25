$(function () {

    var token = $.cookie("Token");
    var areaName = "StockpileManagement";
    var entityName = "StockpileMutation";
    var url = "/api/" + areaName + "/" + entityName;

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("stockpileMutationDate1");
    var tgl2 = sessionStorage.getItem("stockpileMutationDate2");

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
            sessionStorage.setItem("stockpileMutationDate1", formatTanggal(firstDay));
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
            sessionStorage.setItem("stockpileMutationDate2", formatTanggal(lastDay));
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
            //key: "id",
            key: "row_num",
            loadUrl: _loadUrl,
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
                dataField: "stock_location_name",
                dataType: "string",
                caption: "Stockpile",
                allowEditing: false,
            },
            {
                dataField: "entity",
                dataType: "string",
                caption: "Entity",
                allowEditing: false,
            },
            {
                dataField: "stock_location_description",
                dataType: "string",
                caption: "Location",
                allowEditing: false,
                groupIndex: 0,
                sortOrder: "asc",
            },
            {
                dataField: "trans_date",
                dataType: "datetime",
                caption: "Transaction Datetime",
                format: "yyyy-MM-dd HH:mm",
                allowEditing: false,
                sortIndex: 1,
                sortOrder: "desc"
            },
            {
                dataField: "sampling_number",
                dataType: "string",
                caption: "Sampling Number",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "sampling_datetime",
                dataType: "datetime",
                caption: "Sampling Datetime",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "trans_no",
                dataType: "string",
                caption: "Transaction Number",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "opening",
                dataType: "number",
                caption: "Opening",
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
                dataField: "trans_in",
                dataType: "number",
                caption: "In",
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
                dataField: "trans_out",
                dataType: "number",
                caption: "Out",
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
            //{
            //    dataField: "adjustment",
            //    dataType: "number",
            //    caption: "Adjustment",
            //    format: {
            //        type: "fixedPoint",
            //        precision: 2
            //    },
            //    formItem: {
            //        editorType: "dxNumberBox",
            //        editorOptions: {
            //            format: {
            //                type: "fixedPoint",
            //                precision: 2
            //            }
            //        }
            //    },
            //    allowEditing: false
            //},
            {
                dataField: "survey",
                dataType: "number",
                caption: "Survey",
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
                dataField: "closing",
                dataType: "number",
                caption: "Closing",
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
                caption: "Print",
                type: "buttons",
                width: 80,
                visible: false,
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    hint: "Print",
                    text: "Print",
                    onClick: function (e) {
                        applicationEntityId = e.row.data.entity_id;
                        recordId = e.row.data.id;

                        $("#print-out-list").select2({
                            dropdownParent: $("#print-out-modal .modal-body"),
                            ajax:
                            {
                                url: "/api/Report/ReportTemplate/PrintOutListSelect2/" +
                                    encodeURIComponent(applicationEntityId),
                                headers: {
                                    "Authorization": "Bearer " + token
                                },
                                dataType: 'json',
                                delay: 250,
                                data: function (params) {
                                    return {
                                        q: params.term, // search term
                                        page: params.page
                                    };
                                },
                                cache: true
                            },
                            allowClear: true,
                            minimumInputLength: 0
                        }).on('select2:select', function (e) {
                            var data = e.params.data;
                            reportTemplateId = data.id;

                            $("#print-out-btn").on("click", function () {
                                let printPage = "/Report/PrintOutViewer/Index/"
                                    + "?Id=" + encodeURIComponent(recordId)
                                    + "&reportTemplateId=" + encodeURIComponent(reportTemplateId);
                                window.open(printPage);
                            });
                        }).on('select2:clear', function (e) {
                            reportTemplateId = "";
                        });

                        $("#print-out-modal").modal("show");
                    }
                }]
            },
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
        height: 900,
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

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Details",
                    template: createDetailsTab(masterDetailOptions.data)
                },
            ]
        });
    }

    function createDetailsTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "row_num",
                        //loadUrl: url + "/DataDetail?StockLocationId=" + encodeURIComponent(currentRecord.stock_location_id)
                        //    + "&tanggal=" + encodeURIComponent(formatTanggal(currentRecord.trans_date)),
                        loadUrl: url + "/DataDetail?StockLocationId=" + encodeURIComponent(currentRecord.stock_location_id)
                            + "&tanggal=" + encodeURIComponent(formatTanggal(currentRecord.trans_date))
                            + "&rowNumber=" + encodeURIComponent(currentRecord.row_num),
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
                        //{
                        //    dataField: "analyte_id",
                        //    dataType: "string",
                        //    caption: "Analyte",
                        //    allowEditing: false,
                        //    lookup: {
                        //        dataSource: DevExpress.data.AspNet.createStore({
                        //            key: "value",
                        //            loadUrl: "/api/Quality/Analyte/AnalyteIdLookup",
                        //            onBeforeSend: function (method, ajaxOptions) {
                        //                ajaxOptions.xhrFields = { withCredentials: true };
                        //                ajaxOptions.beforeSend = function (request) {
                        //                    request.setRequestHeader("Authorization", "Bearer " + token);
                        //                };
                        //            }
                        //        }),
                        //        searchEnabled: true,
                        //        valueExpr: "value",
                        //        displayExpr: "text"
                        //    },
                        //},
                        {
                            dataField: "analyte_symbol",
                            dataType: "string",
                            caption: "Analyte",
                            allowEditing: false
                        },
                        {
                            dataField: "uom_name",
                            dataType: "string",
                            caption: "Unit",
                            allowEditing: false
                        },
                        {
                            dataField: "analyte_opening",
                            dataType: "number",
                            caption: "Opening",
                            format: {
                                type: "fixedPoint",
                                precision: 2
                            },
                            allowEditing: false
                        },
                        {
                            dataField: "analyte_in",
                            dataType: "number",
                            caption: "In",
                            format: {
                                type: "fixedPoint",
                                precision: 2
                            },
                            allowEditing: false
                        },
                        {
                            dataField: "analyte_closing",
                            dataType: "number",
                            caption: "Closing",
                            format: {
                                type: "fixedPoint",
                                precision: 2
                            },
                            allowEditing: false
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
                        allowAdding: false,
                        allowUpdating: false,
                        allowDeleting: false,
                        useIcons: true,
                    },
                    grouping: {
                        contextMenuEnabled: true,
                        autoExpandAll: false
                    },
                    rowAlternationEnabled: true,
                    export: {
                        enabled: false,
                        allowExportSelectedData: true
                    },
                    onInitNewRow: function (e) {
                        //e.data.stock_state_id = currentRecord.row_num;
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

    $('#btnRecalculateQuality').on('click', function () {

        $('#btnRecalculateQuality')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Recalculating ...');

        $.ajax({
            url: "/api/StockpileManagement/StockpileMutation/RefreshQuality2", 
            type: 'PUT',
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result) {
                if (result.success) {
                    $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Recalculate Data successfully.", "success");
                    $("#modal-recalculate-quality").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                    $("#modal-recalculate-quality").modal('hide');
                }
            }
        }).fail(function (result, jqXHR, textStatus, errorThrown) {
            $("#modal-recalculate-quality").modal('hide');
            Swal.fire("Error !", result.responseJSON.message, "error");
        }).always(function () {
            $('#btnRecalculateQuality').html('Recalculate');
        });
    });

    $('#btnRecalculateQuantity').on('click', function () {

        $('#btnRecalculateQuantity')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Recalculating ...');

        $.ajax({
            url: "/api/StockpileManagement/StockpileMutation/RefreshView2",
            type: 'PUT',
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result) {
                if (result.success) {
                    $("#grid").dxDataGrid("refresh");
                    Swal.fire("Success!", "Recalculate Data successfully.", "success");
                    $("#modal-recalculate-quantity").modal('hide');
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                    $("#modal-recalculate-quantity").modal('hide');
                }
            }
        }).fail(function (result, jqXHR, textStatus, errorThrown) {
            $("#modal-recalculate-quantity").modal('hide');
            Swal.fire("Error !", result.responseJSON.message, "error");
        }).always(function () {
            $('#btnRecalculateQuantity').html('Recalculate');
        });
    });

    $('#refreshMV').on('click', function () {
        $.ajax({
            url: "/api/StockpileManagement/StockpileMutation/RefreshView",
            type: 'PUT',
            cache: false,
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            alert(result);
            location.reload();
        });
    });

    $('#refreshQuality').on('click', function () {
        $.ajax({
            url: "/api/StockpileManagement/StockpileMutation/RefreshQuality",
            type: 'PUT',
            cache: false,
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            alert(result);
            location.reload();
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
                url: "/api/StockpileManagement/StockpileState/UploadDocument",
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