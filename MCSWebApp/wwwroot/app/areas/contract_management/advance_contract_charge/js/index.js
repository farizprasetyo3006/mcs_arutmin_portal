$(function () {

    var token = $.cookie("Token");
    var areaName = "ContractManagement";
    var entityName = "AdvanceContractCharge";
    var url = "/api/" + areaName + "/" + entityName;

    var $advanceContractId = "";
    var arrAdvanceContractDetail = [];
    var arrEquipmentUsageTransaction = [];
    var arrPriceIndexHistory = [];
    var arrAdditionals = [];

    var arrJointSurveyRecords = [{
        "ID": 1,
        "Text": "Quantity",
        "Value": 100
    }, {
        "ID": 2,
        "Text": "Distance",
        "Value": 200
    }, {
        "ID": 3,
        "Text": "Elevation",
        "Value": 70
    }, {
        "ID": 4,
        "Text": "QuantityCarryOver",
        "Value": 70
    }, {
        "ID": 5,
        "Text": "DistanceCarryOver",
        "Value": 70
    }, {
        "ID": 6,
        "Text": "ElevationCarryOver",
        "Value": 70
    }];
    var arrProdClosing = [{
        "ID": 1,
        "Text": "ProdVolume",
        "Value": "ProdVolume"
    }, {
        "ID": 2,
        "Text": "ProdDistance",
        "Value": "ProdDistance"
    }];
    var arrDaywork = [{
        "ID": 1,
        "Text": "DWTotalHM",
        "Value": "DWTotalHM"
    }, {
        "ID": 2,
        "Text": "DWTotalValue ",
        "Value": "DWTotalValue "
    }];
   

    var getAdvanceContractDetailByAdvanceContractId = function () {
        arrAdvanceContractDetail = [];
         $.ajax({
             url: "/api/ContractManagement/AdvanceContractItemDetail/GetByAdvanceContractItemDetailByAdvanceContractId?AdvanceContractId=" + encodeURIComponent($advanceContractId),
                type: 'GET',
                contentType: "application/json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                },
             success: function (r) {
                 //console.log("getAdvanceContractDetailByAdvanceContractId: ", r);
                    if (r.data.length > 0) {
                        $.each(r.data, function (index, value) {
                            //alert(index + ": " + value);
                            arrAdvanceContractDetail.push({
                                "ID": value.id,
                                "Assigned": "Ms. Greta Sims",
                                "Subject": value.variable,
                                "Amount": value.amount
                            });
                        });

                    }
                }
            });
    }

    var getByEquipmentUsageTransactionByAdvanceContractId = function () {
        arrEquipmentUsageTransaction = [];
        $.ajax({
            url: "/api/Equipment/EquipmentUsageTransaction/GetEquipmentTransactionByAdvanceContractId?AdvanceContractId=" + encodeURIComponent($advanceContractId),
            type: 'GET',
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (r) {
                //console.log("getByEquipmentUsageTransactionByAdvanceContractId: ", r);
                if (r.data.length > 0) {
                    $.each(r.data, function (index, value) {
                        arrEquipmentUsageTransaction.push({
                            "ID": value.id,
                            "Subject": value.equipment_usage_number,
                            "Amount": value.amount
                        });
                    });

                }
            }
        });
    }

    var getPriceIndex = function () {
        var fbp = "mops";

        $.ajax({
            url: "/api/General/PriceIndex/GetPriceIndex",
            type: 'GET',
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (r) {
                //console.log("GetPriceIndexHistory", r);
                var isMopsFound = false;
                if (r.data.length > 0) {
                    $.each(r.data, function (index, value) {
                        if (r.data.length - 1 > index &&
                            (r.data[index].price_index_code != r.data[index + 1].price_index_code) ||
                            (index == (r.data.length - 1))
                        ) {
                            arrPriceIndexHistory.push({
                                "ID": value.id,
                                "PriceIndexName": "AVG(ThisMonth)",
                                "PriceIndexCode": value.price_index_code,
                                "Type": "Value",
                                "PriceIndexDate": "",
                                "IndexValue": ""
                            });
                        }


                        if ((value.price_index_code.toLowerCase().indexOf(fbp)) >= 0) {
                            isMopsFound = true;
                        }
                    });
                }
                if (isMopsFound) {
                    arrAdditionals.push({
                        "ID": "",
                        "Subject": "AFP",
                        "Type": "Value",
                        "PriceIndexDate": "",
                        "IndexValue": ""
                    });
                    arrAdditionals.push({
                        "ID": "",
                        "Subject": "Raise&Fall",
                        "Type": "Value",
                        "PriceIndexDate": "",
                        "IndexValue": ""
                    });
                }
                arrAdditionals.push({
                        "ID": "",
                        "Subject": "BFP",
                        "Type": "Value",
                        "PriceIndexDate": "",
                        "IndexValue": ""
                    }, {
                        "ID": "",
                        "Subject": "FC",
                        "Type": "Value",
                        "PriceIndexDate": "",
                        "IndexValue": ""
                    }
                );
                datatable();
            }
        });
    }

    var getPriceIndexMapDetail = function () {
        $.ajax({
            url: "/api/General/PriceIndexMap/GetPriceIndexMapByBaseRate",
            type: 'GET',
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (r) {
                //console.log("GetPriceIndexMap", r);
                //if (r.data.length > 0) {
                //    $.each(r.data, function (index, value) {
                //        if (r.data.length - 1 > index &&
                //            (r.data[index].price_index_code != r.data[index + 1].price_index_code) ||
                //            (index == (r.data.length - 1))
                //        ) {
                //            arrPriceIndexHistory.push({
                //                "ID": value.id,
                //                "PriceIndexName": "AVG(ThisMonth)",
                //                "PriceIndexCode": value.price_index_code,
                //                "Type": "Value",
                //                "PriceIndexDate": "",
                //                "IndexValue": ""
                //            });
                //        }
                //    });
                //}
                //datatable();
            }
        });
    }

    

    var datatable = function () {
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
                    dataField: "variable",
                    dataType: "string",
                    caption: "Charge Code",
                    //validationRules: [{
                    //    type: "required",
                    //    message: "Charge Name is required."
                    //}],
                    sortOrder: "asc",

                    editorOptions: { readOnly: true }
                },
                {
                    dataField: "charge_name",
                    dataType: "string",
                    caption: "Charge Name"
                },
                {
                    dataField: "advance_contract_id",
                    dataType: "text",
                    caption: "Advance Contract",
                    groupIndex: 0,
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
                    calculateSortValue: function (data) {
                        var value = this.calculateCellValue(data);
                        return this.lookup.calculateCellValue(value);
                    },
                    setCellValue: function (rowData, value) {
                        rowData.advance_contract_id = value;
                    }
                },
                //{
                //    dataField: "advance_contract_item_id",
                //    dataType: "text",
                //    caption: "Advance Contract Item",
                //    validationRules: [{
                //        type: "required",
                //        message: "This field is required."
                //    }],
                //    lookup: {
                //        dataSource: DevExpress.data.AspNet.createStore({
                //            key: "value",
                //            loadUrl: url + "/AdvanceContractItemIdLookup?AdvanceContractId=" + record.advance_contract_id,
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
                    dataField: "item_name",
                    dataType: "string",
                    caption: "Advance Contract Item",
                },
                {
                    dataField: "advance_contract_item_id",
                    dataType: "text",
                    caption: "Advance Contract Item",
                    formItem: {
                        colSpan: 2
                    },
                    lookup: {
                        dataSource: function (options) {
                            var _url = url + "/AdvanceContractItemById";
                            if (options !== undefined && options !== null) {
                                if (options.data !== undefined && options.data !== null) {
                                    if (options.data.advance_contract_id !== undefined
                                        && options.data.advance_contract_id !== null) {
                                        _url += "?Id=" + encodeURIComponent(options.data.advance_contract_id);
                                    }
                                }
                            }

                            return {
                                store: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: _url,
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                })
                            }
                        },
                        valueExpr: "value",
                        displayExpr: "text"
                    },
                    //setCellValue: function (rowData, value) {
                    //    rowData.advance_contract_item_id = value;
                    //},
                    calculateSortValue: function (data) {
                        var value = this.calculateCellValue(data);
                        return this.lookup.calculateCellValue(value);
                    },
                    visible: false
                },
                {
                    dataField: "formula",
                    dataType: "string",
                    caption: "Formula",
                    editorOptions: { readOnly: true }
                },
                {
                    dataField: "advance_contract_detail_id",
                    dataType: "string",
                    caption: "Advance Contract Charge Detail",
                    editorOptions: { readOnly: true },
                    visible: false
                },
                {
                    dataField: "equipment_usage_transaction_id",
                    dataType: "string",
                    caption: "Equipment Usage Transaction",
                    editorOptions: { readOnly: true },
                    visible: false
                },
                {
                    dataField: "price_index_id",
                    dataType: "string",
                    caption: "Formula",
                    visible: false,
                    editorOptions: { readOnly: true }
                },
                {
                    dataField: "formula_creator_btn",
                    caption: "Edit Formula",
                    dataType: "string",
                    visible: false
                },
                //{
                //    dataField: "formula_creator_btn",
                //    caption: "Detail",
                //    type: "buttons",
                //    width: 150,
                //    buttons: [{
                //        cssClass: "btn-dxdatagrid",
                //        hint: "See Advance Contract Detail",
                //        text: "Open Detail",
                //        //onClick: function (e) {
                //        //    recordId = e.row.data.id
                //        //    window.location = "/ContractManagement/AdvanceContractCharge/Detail/" + recordId
                //        //}
                //    }]
                //},
                //{
                //    dataField: "decimal_places",
                //    dataType: "number",
                //    caption: "Decimal Places"
                //},

                {
                    dataField: "decimal_places",
                    dataType: "number",
                    caption: "Decimal Places",
                    editorType: "dxNumberBox",
                    editorOptions: {
                        format: "fixedPoint",
                        step: 0
                    }
                },

                {
                    dataField: "rounding_type_id",
                    dataType: "string",
                    caption: "Rounding Type",
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
                                filter: ["item_group", "=", "rounding-type"]
                            }
                        },
                        valueExpr: "value",
                        displayExpr: "text"
                    },
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
                {
                    dataField: "is_base_formula",
                    caption: "Base Formula",
                    dataType: "boolean",
                    visible: false
                },
                {
                    type: "buttons",
                    buttons: ["edit", "delete"]
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
                mode: "popup",
                allowAdding: true,
                allowUpdating: true,
                allowDeleting: true,
                useIcons: true,
                form: {
                    //colCount: 3,
                    items: [
                        {
                            dataField: "variable",
                            colSpan: 2
                        },
                        {
                            dataField: "charge_name",
                            colSpan: 2
                        },
                        {
                            dataField: "advance_contract_id",
                            colSpan: 2
                        },
                        {
                            dataField: "advance_contract_item_id",
                            colSpan: 2
                        },
                        {
                            dataField: "formula",
                            colSpan: 2
                        },
                        {
                            dataField: "formula_creator_btn",
                            editorType: "dxButton",
                            editorOptions: {
                                text: "Open Formula Editor",
                            },
                            horizontalAlignment: "right",
                        },
                        {
                            dataField: "decimal_places",
                            colSpan: 2
                        },
                        {
                            dataField: "rounding_type_id"
                        },
                        {
                            dataField: "is_base_formula"
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

                if (e.dataField == "advance_contract_id" && e.parentType === "dataRow") {
                    const defaultValueChangeHandler = e.editorOptions.onValueChanged;
                    let index = e.row.rowIndex;
                    let grid = e.component;
                    e.editorName = "dxSelectBox"; // Change the editor's type
                    e.editorOptions.onValueChanged = function (args) {  // Override the default handler
                        //if ($advanceContractId != "" || ($advanceContractId != args.value)) {
                            
                        //}
                        grid.cellValue(index, "formula", "");
                        grid.cellValue(index, "advance_contract_id", args.value);
                        $advanceContractId = args.value;
                        getAdvanceContractDetailByAdvanceContractId();
                    }
                }

                if (e.parentType === "dataRow" && e.dataField == "formula_creator_btn") {
                    let index = e.row.rowIndex
                    let grid = e.component
                    let formula = grid.cellValue(index, "formula");
                    //console.log(grid);
                    //console.log(grid.cellValue(index, "advance_contract_id"));
                    e.editorOptions.onClick = function (e) {
                        if ($advanceContractId == "" && grid.cellValue(index, "advance_contract_id") == "") {
                            alert("Please choose an Advance Contract first.");
                            return;
                        } else if (grid.cellValue(index, "advance_contract_id") != "") {
                            $advanceContractId = grid.cellValue(index, "advance_contract_id");
                        }

                        getAdvanceContractDetailByAdvanceContractId();
                        getByEquipmentUsageTransactionByAdvanceContractId();

                        let formulaCreator = new FormulaCreator({
                            advanceContractId: encodeURIComponent($advanceContractId),
                            formula: formula,
                            advanceContractDetailRecords: arrAdvanceContractDetail,
                            equipmentUsageTransactionRecords: arrEquipmentUsageTransaction,
                            priceIndexHistoryRecords: arrPriceIndexHistory,
                            jointSurveyRecords: arrJointSurveyRecords,
                            additionalRecords: arrAdditionals,
                            productionClosingRecords: arrProdClosing,
                            dayWorkRecords: arrDaywork,
                            saveFormulaCallback: function (value, advance_contract_detail_id, price_index_id, equipment_usage_transaction_id) {
                                var regExp = /\(([^)]+)\)/;
                                var matches = regExp.exec(value);;

                                grid.cellValue(index, "price_index_id", price_index_id);
                                grid.cellValue(index, "advance_contract_detail_id", advance_contract_detail_id);
                                grid.cellValue(index, "equipment_usage_transaction_id", equipment_usage_transaction_id);
                                grid.cellValue(index, "formula", value);
                            }
                        })
                    }
                }

                if (e.parentType == 'dataRow' && e.dataField == "decimal_places") {
                    e.editorOptions.min = 0;
                    e.editorOptions.max = 4;
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
    }

    getPriceIndex();
    //getPriceIndexMapDetail();

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