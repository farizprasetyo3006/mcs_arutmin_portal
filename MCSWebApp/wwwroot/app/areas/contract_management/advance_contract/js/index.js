$(function () {

    var token = $.cookie("Token");
    var areaName = "ContractManagement";
    var entityName = "AdvanceContract";
    var url = "/api/" + areaName + "/" + entityName;

    var contractType = "";

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
                dataField: "advance_contract_number",
                dataType: "string",
                caption: "Contract Number",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "version",
                dataType: "number",
                caption: "Version",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                visible: false
            },
            {
                dataField: "note",
                dataType: "string",
                caption: "Description",
                visible: false
            },
            {
                dataField: "reference_number",
                dataType: "string",
                caption: "Reference Number",
                visible: false
            },
            {
                dataField: "contract_type",
                dataType: "text",
                caption: "Contract Type",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/ContractTypeLookup",
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
                    rowData.contract_type = value;
                    rowData.contractor_id = null;
                    contractType = value;
                }
            },
            //{
            //    dataField: "contractor_id",
            //    dataType: "text",
            //    caption: "Contract Target",
            //    formItem: {
            //        colSpan: 2
            //    },
            //    validationRules: [{
            //        type: "required",
            //        message: "This field is required."
            //    }],
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: url + "/ContractorLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    calculateSortValue: function (data) {
            //        var value = this.calculateCellValue(data);
            //        return this.lookup.calculateCellValue(value);
            //    }
            //},
            {
                dataField: "contractor_id",
                dataType: "text",
                caption: "Contract Target",
                validationRules: [{
                    type: "required",
                    message: "Contract Target is required."
                }],
                lookup: {
                    dataSource: function (options) {
                        var contractType = "";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.contract_type !== undefined && options.data.contract_type !== null) {
                                    contractType = options.data.contract_type
                                }
                            }
                        }
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: url + "/ContractorLookup?ContractType=" + encodeURIComponent(contractType),
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
                    rowData.contractor_id = value;
                    rowData.contract_type = contractType;
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false
            },
            {
                dataField: "contractor_name",
                dataType: "string",
                caption: "Name",
                groupIndex: 0,
                editorOptions: { readOnly: true }
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Start Date",                
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "End Date",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            //{
            //    dataField: "quantity",
            //    dataType: "number",
            //    caption: "Quantity",
            //    format: "fixedPoint",
            //    formItem: {
            //        editorType: "dxNumberBox",
            //        editorOptions: {
            //            format: "fixedPoint",
            //        }
            //    },
            //},
            //{
            //    dataField: "quantity_uom_id",
            //    dataType: "text",
            //    caption: "Qty Unit",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/UOM/UOM/UomIdLookup",
            //            onBeforeSend: function (method, ajaxOptions) {
            //                ajaxOptions.xhrFields = { withCredentials: true };
            //                ajaxOptions.beforeSend = function (request) {
            //                    request.setRequestHeader("Authorization", "Bearer " + token);
            //                };
            //            }
            //        }),
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //    calculateSortValue: function (data) {
            //        var value = this.calculateCellValue(data);
            //        return this.lookup.calculateCellValue(value);
            //    }
            //},
            {
                dataField: "contract_value",
                dataType: "number",
                caption: "Contract Value",
                format: "fixedPoint",
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                }
            },
            {
                dataField: "contract_currency_id",
                dataType: "text",
                caption: "Contract Currency",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                }
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                allowEditing: false,
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
        masterDetail: {
            enabled: true,
            template: masterItemTemplate
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
                title: "Advance Contract Item",
                template: createAdvanceContractItemTabTemplate(masterDetailOptions.data)
            }]
        });
    };

    function createAdvanceContractItemTabTemplate(masterItem) {
        return function () {
            let currentRecord = masterItem;
            let detailName = "AdvanceContractItem";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        //loadUrl: urlDetail + "/DataGrid/" + encodeURIComponent(currentRecord.id),
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
                            dataField: "item_name",
                            dataType: "string",
                            caption: "Item Name",
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
                            }],
                            sortOrder: "asc"
                        },
                        {
                            dataField: "description",
                            dataType: "string",
                            caption: "Description",
                            sortOrder: "asc"
                        },
                        {
                            dataField: "master_list_id",
                            dataType: "text",
                            caption: "Item Class",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=contract-item-class",
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
                        },
                        {
                            caption: "Detail",
                            type: "buttons",
                            width: 150,
                            buttons: [{
                                cssClass: "btn-dxdatagrid",
                                hint: "See Advance Contract Detail Item Detail",
                                text: "Open Detail",
                                onClick: function (e) {
                                    recordId = e.row.data.id
                                    window.location = "/ContractManagement/AdvanceContract/ItemDetail/" + recordId
                                }
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
                        e.data.advance_contract_id = currentRecord.id;
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
                                if (arg.value.length == 0 || arg.value.length > 1) {
                                    e.component.searchByText(arg.value);
                                }
                            }
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
                url: "/api/ContractManagement/AdvanceContract/UploadDocument",
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