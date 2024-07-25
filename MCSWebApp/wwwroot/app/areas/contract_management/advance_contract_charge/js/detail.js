$(function () {
    var token = $.cookie("Token");
    var $recordId = document.querySelector("[name=advance_contract_charge_id]").value;
    var $advanceContractId = document.querySelector("[name=advance_contract_id]").value;
    var areaName = "ContractManagement";
    var entityName = "AdvanceContractChargeDetail";
    var gridUrl = "/api/" + areaName + "/" + entityName;

    var employeesList = ["John Heart", "Samantha Bright", "Arthur Miller", "Robert Reagan", "Greta Sims", "Brett Wade",
        "Sandra Johnson", "Ed Holmes", "Barb Banks", "Kevin Carter", "Cindy Stanwick", "Sammy Hill", "Davey Jones", "Victor Norris",
        "Mary Stern", "Robin Cosworth", "Kelly Rodriguez", "James Anderson", "Antony Remmen", "Olivia Peyton", "Taylor Riley",
        "Amelia Harper", "Wally Hobbs", "Brad Jameson", "Karen Goodson", "Marcus Orbison", "Sandy Bright", "Morgan Kennedy",
        "Violet Bailey", "Ken Samuelson", "Nat Maguiree", "Bart Arnaz", "Leah Simpson", "Arnie Schwartz", "Billy Zimmer", "Samantha Piper",
        "Maggie Boxter", "Terry Bradley", "Gabe Jones", "Lucy Ball", "Jim Packard", "Hannah Brookly", "Harv Mudd", "Clark Morgan",
        "Todd Hoffman", "Jackie Garmin", "Lincoln Bartlett", "Brad Farkus", "Jenny Hobbs", "Dallas Lou", "Stu Pizaro"];
    var employeesTasks = [{
        "ID": 1,
        "Assigned": "Mr. John Heart",
        "Subject": "Choose between PPO and HMO Health Plan"
    }, {
        "ID": 2,
        "Assigned": "Mr. John Heart",
        "Subject": "Google AdWords Strategy"
    }, {
        "ID": 3,
        "Assigned": "Mr. John Heart",
        "Subject": "New Brochures"
    }, {
        "ID": 4,
        "Assigned": "Mr. John Heart",
        "Subject": "Update NDA Agreement"
    }, {
        "ID": 5,
        "Assigned": "Mr. John Heart",
        "Subject": "Review Product Recall Report by Engineering Team"
    }, {
        "ID": 6,
        "Assigned": "Mrs. Olivia Peyton",
        "Subject": "Update Personnel Files"
    }, {
        "ID": 7,
        "Assigned": "Mrs. Olivia Peyton",
        "Subject": "Review Health Insurance Options Under the Affordable Care Act"
    }, {
        "ID": 8,
        "Assigned": "Mrs. Olivia Peyton",
        "Subject": "Non-Compete Agreements"
    }, {
        "ID": 9,
        "Assigned": "Mrs. Olivia Peyton",
        "Subject": "Give Final Approval for Refunds"
    }, {
        "ID": 10,
        "Assigned": "Mr. Robert Reagan",
        "Subject": "Deliver R&D Plans for 2013"
    }, {
        "ID": 11,
        "Assigned": "Mr. Robert Reagan",
        "Subject": "Decide on Mobile Devices to Use in the Field"
    }, {
        "ID": 12,
        "Assigned": "Mr. Robert Reagan",
        "Subject": "Try New Touch-Enabled WinForms Apps"
    }, {
        "ID": 13,
        "Assigned": "Mr. Robert Reagan",
        "Subject": "Approval on Converting to New HDMI Specification"
    }, {
        "ID": 14,
        "Assigned": "Ms. Greta Sims",
        "Subject": "Approve Hiring of John Jeffers"
    }, {
        "ID": 15,
        "Assigned": "Ms. Greta Sims",
        "Subject": "Update Employee Files with New NDA"
    }, {
        "ID": 16,
        "Assigned": "Ms. Greta Sims",
        "Subject": "Provide New Health Insurance Docs"
        }];

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
    }];

    var arrAdvanceContractDetail = [];
    var arrPriceIndexHistory = [];


    var ary = {
        data: { "a": "a", "b": "b"}
    };
    ary.tes = {};
    //ary.push({
    //productId: ""
    //});
    //console.log(ary);

    $.ajax({
        url: "/api/ContractManagement/AdvanceContractDetail/GetByAdvanceContractId?AdvanceContractId=" + encodeURIComponent($advanceContractId),
        type: 'GET',
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (r) {
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

    var getPriceIndex = function () {
        $.ajax({
            url: "/api/General/PriceIndex/GetPriceIndex",
            type: 'GET',
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (r) {
                //console.log("GetPriceIndexHistory", r);
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
                    });
                }
                datatable();
            }
        });
    }

    var datatable = function () {
        $("#dt-grid").dxDataGrid({
            dataSource: DevExpress.data.AspNet.createStore({
                key: "id",
                loadUrl: gridUrl + "/DataGrid?recordId=" + encodeURIComponent($recordId),
                insertUrl: gridUrl + "/InsertData",
                updateUrl: gridUrl + "/UpdateData",
                deleteUrl: gridUrl + "/DeleteData",
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
                    caption: "Variable",
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
                {
                    type: "buttons",
                    buttons: ["edit", "delete"]
                }
            ],
            onEditorPreparing: function (e) {
                if (e.parentType === 'searchPanel') {
                    e.editorOptions.onValueChanged = function (arg) {
                        if (arg.value.length == 0 || arg.value.length > 2) {
                            e.component.searchByText(arg.value);
                        }
                    }
                }
                // Set Formula Creator onchange handler
                if (e.parentType === "dataRow" && e.dataField == "formula_creator_btn") {
                    let formula = e.row.data.charge_formula
                    let index = e.row.rowIndex
                    let grid = e.component

                    e.editorOptions.onClick = function (e) {
                        let formulaCreator = new FormulaCreator({
                            formula: formula,
                            employeesList: employeesList,
                            employeesTasks: employeesTasks,
                            advanceContractDetailRecords: arrAdvanceContractDetail,
                            priceIndexHistoryRecords: arrPriceIndexHistory,
                            jointSurveyRecords: arrJointSurveyRecords,
                            saveFormulaCallback: function (value, advance_contract_detail_id, price_index_id) {
                                var regExp = /\(([^)]+)\)/;
                                //var matches = regExp.exec("Shabalu Sheedaa - Ali Gul Panara - Octa School - 10 (7)");
                                var matches = regExp.exec(value);

                                //matches[1] contains the value between the parentheses
                                ////console.log(matches[1])
                                
                                grid.cellValue(index, "price_index_id", price_index_id);
                                grid.cellValue(index, "advance_contract_detail_id", advance_contract_detail_id);
                                grid.cellValue(index, "formula", value);
                            }
                        })
                    }
                }
            },
            onInitNewRow: function (e) {
                e.data.advance_contract_charge_id = $recordId;
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
                useIcons: true,
                form: {
                    colCount: 2,
                    items: [
                        {
                            dataField: "variable",
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
});


