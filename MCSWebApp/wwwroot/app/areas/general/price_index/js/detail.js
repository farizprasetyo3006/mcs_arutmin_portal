$(function () {
    var $recordId = document.querySelector("[name=price_index_id]").value
    var token = $.cookie("Token");
    var areaName = "General";
    var entityName = "PriceIndexMap";
    var url = "/api/" + areaName + "/" + entityName;

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: url + "/DataGrid?recordId=" + encodeURIComponent($recordId),
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
                dataField: "name",
                dataType: "string",
                caption: "Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                sortOrder: "asc"
            },
        ],
        masterDetail: {
            enabled: true,
            template: masterItemTemplate
        },
        onInitNewRow: function (e) {
            e.data.price_index_id = $recordId;
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
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            if (e.dataField == "note" && e.parentType === "dataRow") {
                const defaultValueChangeHandler = e.editorOptions.onValueChanged;
                e.editorName = "dxTextArea"; // Change the editor's type
                e.editorOptions.onValueChanged = function (args) {  // Override the default handler
                    // ...
                    // Custom commands go here
                    // ...
                    // If you want to modify the editor value, call the setValue function:
                    // e.setValue(newValue);
                    // Otherwise, call the default handler:
                    defaultValueChangeHandler(args);
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "contractor_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let recordId = e.value
                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/ContractManagement/AdvanceContract/ContractorById?Id=' + recordId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];
                            grid.cellValue(index, "contractor_name", record.business_partner_name);

                        }
                    })
                    standardHandler(e) // Calling the standard handler to save the edited value
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
    });

    function masterItemTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [{
                title: "Price Index Mapping",
                template: createPriceIndexMapTabTemplate(masterDetailOptions.data)
            }]
        });
    };

    function createPriceIndexMapTabTemplate(masterItem) {
        return function () {
            let currentRecord = masterItem;
            //console.log("currentRecord");
            //console.log(currentRecord);
            //console.log(currentRecord.price_index_id);
            let detailName = "PriceIndexMapDetail";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/DataGrid?recordId=" + encodeURIComponent(currentRecord.id),
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
                            dataField: "start_range",
                            dataType: "number",
                            caption: "Start Range",
                        },
                        {
                            dataField: "end_range",
                            dataType: "number",
                            caption: "End Range",
                        },
                        {
                            dataField: "value",
                            dataType: "number",
                            caption: "Value",
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint",
                                        precision: 3
                                    }
                                }
                            }
                        },
                        {
                            type: "buttons",
                            buttons: ["edit", "delete"]
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
                        e.data.price_index_map_id = currentRecord.id;
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
                    },
                    onEditorPreparing: function (e) {
                        if (e.parentType === 'searchPanel') {
                            e.editorOptions.onValueChanged = function (arg) {
                                if (arg.value.length == 0 || arg.value.length > 2) {
                                    e.component.searchByText(arg.value);
                                }
                            }
                        }
                        if (e.parentType === "dataRow") {

                        }
                        //if (e.parentType === "dataRow" && e.dataField == "advance_contract_reference_detail_id") {
                        //    const defaultValueChangeHandler = e.editorOptions.onValueChanged;
                        //    let index = e.row.rowIndex;
                        //    let grid = e.component;
                        //    e.editorName = "dxSelectBox"; // Change the editor's type
                        //    e.editorOptions.onValueChanged = function (args) {  // Override the default handler
                        //        //if ($advanceContractId != "" || ($advanceContractId != args.value)) {

                        //        //}
                        //        //console.log("args.value");
                        //        //console.log(args.value);

                        //        grid.cellValue(index, "advance_contract_reference_detail_id", args.value);

                        //        $.ajax({
                        //            //url: "/api/ContractManagement/AdvanceContractDetail/GetByAdvanceContractId?AdvanceContractId=" + encodeURIComponent($advanceContractId),
                        //            url: "/api/" + areaName + "/AdvanceContractCharge/CalculateFormulaById?advance_contract_id=" + currentRecord.advance_contract_id,
                        //            type: 'GET',
                        //            contentType: "application/json",
                        //            beforeSend: function (xhr) {
                        //                xhr.setRequestHeader("Authorization", "Bearer " + token);
                        //            },
                        //            success: function (r) {
                        //                //console.log("CalculateFormulaById: ", r);
                        //                //if (r.data.length > 0) {


                        //                //}

                        //                if (r.success) {
                        //                    grid.cellValue(index, "advance_contract_charge_id", args.value);
                        //                    grid.cellValue(index, "value", r.value.toFixed(2));
                        //                }
                        //            }
                        //        });

                        //        //grid.cellValue(index, "formula", "");
                        //        //grid.cellValue(index, "advance_contract_id", args.value);
                        //        //$advanceContractId = args.value;
                        //        //getAdvanceContractDetailByAdvanceContractId();
                        //    }
                        //}
                    },
                });
        }
    };

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
                url: "/api/DailyRecord/Rainfall/UploadDocument",
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