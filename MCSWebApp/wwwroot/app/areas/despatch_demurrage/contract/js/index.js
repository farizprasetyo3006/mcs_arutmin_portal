$(function () {

    var token = $.cookie("Token");
    var areaName = "DespatchDemurrage";
    var entityName = "Contract";
    var url = "/api/" + areaName + "/" + entityName;

    $("#sales-contract-grid").dxDataGrid({
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
                dataField: "contract_name",
                dataType: "string",
                caption: "DesDem Contract Name",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Contract Start Date",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "Contract End Date",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
            },
            {
                dataField: "contractor_id",
                dataType: "text",
                caption: "Contractor",
                visible: true,
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Organisation/Contractor/ContractorIdLookupByIsEquipmentOwner?isEquipmentOwner=true",
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
            e.data.is_draft_survey = false;
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
            items: [{
                title: "Despatch/Demurrage Term",
                template: createDesDemTermTab(masterDetailOptions.data)
            }]
        });
    }

    function createDesDemTermTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "ContractDetail";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/DetailByDesDemId/" + encodeURIComponent(currentRecord.id),
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
                            dataField: "term_name",
                            dataType: "text",
                            caption: "DesDem Term Name",
                        },
                        {
                            dataField: "port_location",
                            dataType: "text",
                            caption: "Location",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Location/PortLocation/PortLocationIdLookup",
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
                            dataField: "despatch_percent",
                            caption: "Despatch %",
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            },
                        },
                        {
                            dataField: "loading_rate",
                            caption: "Loading Rate (ton/day)",
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            },
                        },
                        //{
                        //    dataField: "loading_rate_unit",
                        //    caption: "Loading Rate Unit",
                        //    editorType: "dxSelectBox",
                        //    editorOptions: {
                        //        dataSource: DevExpress.data.AspNet.createStore({
                        //            key: "value",
                        //            loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
                        //    visible: false
                        //},
                        {
                            dataField: "turn_time",
                            caption: "Turn Time (hour)",
                            editorType: "dxNumberBox",
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            },
                        },
                        //{
                        //    dataField: "turn_time_unit",
                        //    caption: "Turn Time Unit",
                        //    editorType: "dxSelectBox",
                        //    editorOptions: {
                        //        dataSource: DevExpress.data.AspNet.createStore({
                        //            key: "value",
                        //            loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
                        //    visible: false
                        //},
                        //{
                        //    dataField: "laytime_commenced",
                        //    caption: "Laytime Commenced",
                        //    editorType: "dxDateBox",
                        //    editorOptions: {
                        //        format: "fixedPoint",
                        //    },
                        //    visible: false
                        //},
                        //{
                        //    dataField: "actual_commenced",
                        //    label: {
                        //        text: "Actual Commenced"
                        //    },
                        //    editorType: "dxDateBox",
                        //    editorOptions: {
                        //        format: "fixedPoint",
                        //    },
                        //},
                        //{
                        //    dataField: "laytime_completed",
                        //    caption: "Laytime Completed",
                        //    editorType: "dxDateBox",
                        //    editorOptions: {
                        //        format: "fixedPoint",
                        //    },
                        //    visible: false
                        //},
                        //{
                        //    dataField: "actual_completed",
                        //    caption: "Actual Completed",
                        //    editorType: "dxDateBox",
                        //    editorOptions: {
                        //        format: "fixedPoint",
                        //    },
                        //    visible: false
                        //},
                        //{
                        //    dataField: "sof_id",
                        //    caption: "Statement of Fact",
                        //    colSpan: 2,
                        //    editorType: "dxSelectBox",
                        //    editorOptions: {
                        //        dataSource: DevExpress.data.AspNet.createStore({
                        //            key: "value",
                        //            loadUrl: "/api/Port/StatementOfFact/StatemenfOfFactIdLookup",
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
                            dataField: "currency_id",
                            caption: "Currency",
                            colSpan: 2,
                            editorType: "dxSelectBox",
                            editorOptions: {
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
                                searchEnabled: true,
                                valueExpr: "value",
                                displayExpr: "text"
                            },
                            visible: false
                        },
                        {
                            dataField: "rate",
                            caption: "Rate",
                            editorType: "dxNumberBox",
                            colSpan: 2,
                            editorOptions: {
                                format: "fixedPoint",
                                step: 0
                            },
                            visible: false
                        }
                    ],
                    masterDetail: {
                        enabled: true,
                        template: DesDemDelayTemplate
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
                    onInitNewRow: function (e) {
                        e.data.despatch_demurrage_id = currentRecord.id;
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

    function DesDemDelayTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [{
                title: "Despatch/Demurrage Delay",
                template: createDesDemDelayTab(masterDetailOptions.data)
            }]
        });
    }

    function createDesDemDelayTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "DespatchDemurrageDelay";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByDespatchDemurrageId/" + encodeURIComponent(currentRecord.despatch_demurrage_id),
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
                            dataField: "event_category_id",
                            caption: "Event Definition",
                            dataType: "text",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/EventCategoryIdLookup",
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
                            visible: true
                        },
                        {
                            dataField: "demurrage_percent",
                            caption: "Demurrage Applicable %",
                            dataType: "number",
                        },
                        {
                            dataField: "despatch_percent",
                            dataType: "number",
                            caption: "Despatch Applicable %",
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
                        e.data.despatch_demurrage_id = currentRecord.despatch_demurrage_id
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
                url: "/api/Planning/BlendingPlan/UploadDocument",
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