$(function () {

    var token = $.cookie("Token");
    var areaName = "SurveyManagement";
    var entityName = "JointSurvey";
    var url = "/api/" + areaName + "/" + entityName;
    var advance_contract_id = "";

    const maxFileSize = 52428800;
    var jointSurveyData

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
                dataField: "join_survey_number",
                dataType: "string",
                caption: "Join Survey Number",
                width: "19%",
            },
            {
                dataField: "join_survey_date",
                dataType: "datetime",
                caption: "Join Survey Date",
                format: "yyyy-MM-dd HH:mm",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                width: "145px",
            },
            {
                dataField: "advance_contract_id",
                dataType: "text",
                caption: "Advance Contract",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        //loadUrl: url + "/AdvanceContractIdLookup",
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
                    advance_contract_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false,
            },
            {
                dataField: "advance_contract_reference_id",
                dataType: "text",
                caption: "Advance Contract Reference",
                visible: false,
                lookup: {
                    dataSource: function (options) {
                        var _url = url + "/AdvanceContractReferenceIdLookup";

                        if (options !== undefined && options !== null) {
                            if (options.data !== undefined && options.data !== null) {
                                if (options.data.advance_contract_id !== undefined
                                    && options.data.advance_contract_id !== null) {
                                    _url += "?AdvanceContractId=" + encodeURIComponent(options.data.advance_contract_id);
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
                setCellValue: function (rowData, value) {
                    rowData.advance_contract_reference_id = value;
                },
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "surveyor_id",
                dataType: "text",
                caption: "Surveyor",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/SurveyorIdLookup",
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
                formItem: {
                    editorOptions: {
                        showClearButton: true
                    }
                },
                visible: false,
            },
            
            //{
            //    dataField: "mine_location_id",
            //    dataType: "text",
            //    caption: "Mine Location",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/Location/MineLocation/MineLocationIdLookup",
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
            //    },
            //},
            //{
            //    dataField: "stockpile_location_id",
            //    dataType: "text",
            //    caption: "Stockpile Location",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/Location/StockpileLocation/StockpileLocationIdLookup",
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
            //    },
            //},
            //{
            //    dataField: "port_location_id",
            //    dataType: "text",
            //    caption: "Port Location",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/Location/PortLocation/PortLocationIdLookup",
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
            //    },
            //},
            //{
            //    dataField: "waste_location_id",
            //    dataType: "text",
            //    caption: "Waste Location",
            //    lookup: {
            //        dataSource: DevExpress.data.AspNet.createStore({
            //            key: "value",
            //            loadUrl: "/api/Location/WasteLocation/WasteLocationIdLookup",
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
            //    },
            //},
            
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                allowEditing: true,
                validationRules: [{
                    type: "required",
                    message: "The Business Unit Field is Required",
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
                    displayExpr: "text"
                }
            },
            {
                dataField: "note",
                dataType: "string",
                caption: "Note",
                visible: false
            },
            //{
            //    dataField: "approver_name",
            //    dataType: "string",
            //    caption: "Approver",                
            //    allowEditing: false,
            //    visible: false
            //},
            //{
            //    dataField: "approved_on",
            //    dataType: "datetime",
            //    caption: "Approved On",
            //    allowEditing: false
            //}, 
            {
                type: "buttons",
                width: 110,
                buttons: [
                    "edit",
                    {
                        hint: "Approve",
                        icon: "fas fa-calendar-check",
                        visible: true,
                        onClick: function (e) {
                            $('#joint_survey_id').val(e.row.data.id);
                            $("#modal-approval").modal("show");
                            e.event.preventDefault();
                        }
                    },
                    "delete"
                ],
                allowSearch: false,
                showInColumnChooser: false
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
                        dataField: "business_unit_id",
                    },
                    {
                        dataField: "join_survey_number",
                    },
                    {
                        dataField: "join_survey_date",
                    },
                    {
                        dataField: "advance_contract_id",
                        colSpan: 2
                    },
                    {
                        dataField: "advance_contract_reference_id",
                        colSpan: 2
                    },
                    {
                        dataField: "surveyor_id",
                        colSpan: 2
                    },
                    /*{
                        dataField: "quantity",
                    },
                    {
                        dataField: "quantity_carry_over",
                    },
                    {
                        dataField: "uom_id",
                    },
                    {

                    },
                    {
                        dataField: "distance",
                    },
                    {
                        dataField: "distance_carry_over",
                    },
                    {
                        dataField: "elevation",
                    },
                    {
                        dataField: "elevation_carry_over",
                    },
                    {
                        dataField: "accounting_period_id",
                    },
                    {
                        dataField: "transport_model",
                    },*/
                    //{
                    //    dataField: "mine_location_id",
                    //},
                    //{
                    //    dataField: "stockpile_location_id",
                    //},
                    //{
                    //    dataField: "port_location_id",
                    //},
                    //{
                    //    dataField: "waste_location_id",
                    //},
                    /*{
                        dataField: "business_process_id",
                    },
                    {
                        dataField: "location_id",
                    },*/
                    {
                        dataField: "note",
                        colSpan: 2
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
            //e.editorOptions.onValueChanged = function (args) {
            //    if (e.parentType === "dataRow" && e.row.data.approved_on !== null) {
            //        //toastr["error"]("This record has been approved and cannot be change" ?? "Error");
            //        //e.element.find("[aria-label='Save']").dxButton("instance").option("disabled", true);
            //        //e.setValue(args.value); // Updates the cell value  
            //    }
            //}
            
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
            if (e.parentType === "dataRow" && e.dataField == "advance_contract_reference_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component

                e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler
                    // Get its value (Id) on value changed
                    let advanceContractReferenceId = e.value

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/ContractManagement/AdvanceContractReference/DataDetail?Id=' + advanceContractReferenceId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let record = response.data[0];
                            //console.log(record);
                            // Set its corresponded field's value
                            grid.cellValue(index, "advance_contract_reference_id", record.id)
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

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Attachments",
                    template: createAttachmentsTab(masterDetailOptions.data)
                },
                {
                    title: "Detail",
                    template: createDetailTabTemplate(masterDetailOptions.data)
                },
                {
                    title: "LQ",
                    template: createLQTabTemplate(masterDetailOptions.data)
                }
            ]
        });
    }

    function createLQTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "ShippingDetail";
            let urlDetail = "/api/port" + "/" + detailName;

            return $("<div id='grid-shipping-transaction-lq'>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: url + "/LQ/DataGrid?id=" + encodeURIComponent(currentRecord.id),
                        insertUrl: url + "/lq/InsertItemData",
                        updateUrl: url + "/lq/UpdateitemData",
                        deleteUrl: url + "/lq/DeleteItemData",
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
                            dataField: "business_unit_id",
                            dataType: "text",
                            caption: "Business Unit",
                            allowEditing: true,
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
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
                                searchExpr: ["search", "text"]
                            }
                        },
                        {
                            dataField: "product_id",
                            dataType: "text",
                            caption: "Product",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Material/Product/ProductIdLookup",
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                }),
                                valueExpr: "value",
                                displayExpr: "text",
                                searchExpr: ["search", "text"]
                            },
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            },
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
                                displayExpr: "text",
                                searchExpr: ["search", "text"]
                            }
                        },
                        {
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity",
                            allowEditing: true,
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "presentage",
                            dataType: "number",
                            caption: "Presentage",
                            allowEditing: false,
                            format: {
                                type: "fixedPoint"
                            },
                            formItem: {
                                editorType: "dxNumberBox",
                                editorOptions: {
                                    step: 0,
                                    format: {
                                        type: "fixedPoint"
                                    }
                                }
                            }
                        },
                        {
                            dataField: "is_return",
                            dataType: "boolean",
                            caption: "Is Return Cargo",
                            format: {
                                type: "fixedPoint"
                            },
                            
                        },
                    ],
                    summary: {
                        totalItems: [
                            {
                                column: 'quantity',
                                summaryType: 'sum',
                                valueFormat: ',##0.###'
                            },
                        ],
                    },
                    filterRow: {
                        visible: false
                    },
                    headerFilter: {
                        visible: false
                    },
                    groupPanel: {
                        visible: false
                    },
                    searchPanel: {
                        visible: true,
                        width: 240,
                        placeholder: "Search..."
                    },
                    filterPanel: {
                        visible: false
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
                        useIcons: true,
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
                        e.data.header_id = currentRecord.id;
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

    let documentDataGrid
    function createAttachmentsTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "JointSurveyAttachment";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            jointSurveyData = currentRecord

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByJointSurveyId/" + encodeURIComponent(currentRecord.id),
                        //updateUrl: urlDetail + "/Loading/UpdateData",
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
                            dataField: "file_name",
                            dataType: "string",
                            caption: "File Name"
                        },
                        {
                            caption: "Download",
                            type: "buttons",
                            width: 100,
                            buttons: [{
                                cssClass: "btn-dxdatagrid",
                                hint: "Download attachment",
                                text: "Download",
                                onClick: function (e) {
                                    // Download file from Ajax. Ref: https://stackoverflow.com/a/9970672
                                    let documentData = e.row.data
                                    let documentName = /[^\\]*$/.exec(documentData.file_name)[0]

                                    let xhr = new XMLHttpRequest()
                                    xhr.open("GET", "/api/SurveyManagement/JointSurveyAttachment/DownloadDocument/" + documentData.id, true)
                                    xhr.responseType = "blob"
                                    xhr.setRequestHeader("Authorization", "Bearer " + token)

                                    xhr.onload = function (e) {
                                        let blobURL = window.webkitURL.createObjectURL(xhr.response)

                                        let a = document.createElement("a")
                                        a.href = blobURL
                                        a.download = documentName
                                        document.body.appendChild(a)
                                        a.click()
                                    };

                                    xhr.send()
                                }
                            }]
                        },
                        {
                            type: "buttons",
                            buttons: ["edit", "delete"]
                        }
                    ],
                    onToolbarPreparing: function (e) {
                        let toolbarItems = e.toolbarOptions.items;

                        // Modifies an existing item
                        toolbarItems.forEach(function (item) {
                            if (item.name === "addRowButton") {
                                item.options = {
                                    icon: "plus",
                                    onClick: function (e) {
                                        openDocumentPopup()
                                    }
                                }
                            }

                            if (item.name === "editRowButton") {
                                item.options = {
                                    icon: "edit",
                                    onClick: function (e) {
                                        openDocumentPopup()
                                    }
                                }
                            }
                        });
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
                        allowUpdating: false,
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
                        e.data.joint_survey_id = currentRecord.id;
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

            return documentDataGrid
        }
    }

    const documentPopupOptions = {
        width: "80%",
        height: "auto",
        showTitle: true,
        title: "Add Attachment",
        visible: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        contentTemplate: function (e) {
            let formContainer = $("<div>")
            formContainer.dxForm({
                formData: {
                    id: "",
                    joint_survey_id: jointSurveyData.id,
                    file_name: ""
                },
                colCount: 2,
                items: [
                    {
                        dataField: "joint_survey_id",
                        label: {
                            text: "Joint Survey Id"
                        },
                        validationRules: [{
                            type: "required"
                        }],
                        visible: false
                    },
                    {
                        dataField: "file_name",
                        name: "file",
                        label: {
                            text: "File Name"
                        },
                        template: function (data, itemElement) {
                            itemElement.append($("<div>").attr("id", "file").dxFileUploader({
                                uploadMode: "useForm",
                                multiple: false,
                                maxFileSize: maxFileSize,
                                invalidMaxFileSizeMessage: "Max. file size is 50 Mb",
                                onValueChanged: function (e) {
                                    data.component.updateData(data.dataField, e.value)
                                }
                            }));
                        },
                        validationRules: [{
                            type: "required"
                        }],
                        colSpan: 2
                    },
                    {
                        itemType: "button",
                        colSpan: 2,
                        horizontalAlignment: "right",
                        buttonOptions: {
                            text: "Save",
                            type: "secondary",
                            useSubmitBehavior: true,
                            onClick: function () {
                                let formData = formContainer.dxForm("instance").option('formData')
                                let file = formData.file_name[0]
                                ////console.log(file)

                                var reader = new FileReader();
                                reader.readAsDataURL(file);
                                reader.onload = function () {
                                    let fileName = file.name
                                    let fileSize = file.size
                                    let data = reader.result.split(',')[1]

                                    if (fileSize >= maxFileSize) {
                                        return;
                                    }

                                    let newFormData = {
                                        "jointSurveyId": formData.joint_survey_id,
                                        "fileName": fileName,
                                        "fileSize": fileSize,
                                        "data": data
                                    }

                                    /*//console.log(newFormData)*/

                                    $.ajax({
                                        url: `/api/${areaName}/JointSurveyAttachment/InsertData`,
                                        data: JSON.stringify(newFormData),
                                        type: "POST",
                                        contentType: "application/json",
                                        beforeSend: function (xhr) {
                                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                                        },
                                        success: function (response) {
                                            documentPopup.hide()
                                            documentDataGrid.dxDataGrid("instance").refresh()
                                        }
                                    })
                                }
                            }
                        }
                    }
                ]
            })
            e.append(formContainer)
        }
    }

    const documentPopup = $("<div>")
        .dxPopup(documentPopupOptions).appendTo("body").dxPopup("instance")

    const openDocumentPopup = function () {
        documentPopup.option("contentTemplate", documentPopupOptions.contentTemplate.bind(this));
        documentPopup.show()
    }

    function createDetailTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "JointSurvey";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/GetItemsById?Id=" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/InsertItemData",
                        updateUrl: urlDetail + "/UpdateItemData",
                        deleteUrl: urlDetail + "/DeleteItemData",

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
                            dataField: "quantity",
                            dataType: "number",
                            caption: "Quantity",
                            width: "15%",
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
                            dataField: "quantity_carry_over",
                            dataType: "number",
                            caption: "Quantity Carry Over",
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
                            visible: false,
                        },
                        {
                            dataField: "uom_id",
                            dataType: "text",
                            caption: "Qty Unit",
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
                            calculateSortValue: function (data) {
                                var value = this.calculateCellValue(data);
                                return this.lookup.calculateCellValue(value);
                            },
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                            visible: false,
                        },
                        {
                            dataField: "distance",
                            dataType: "number",
                            caption: "Distance (meter)",
                            visible: false
                        },
                        {
                            dataField: "distance_carry_over",
                            dataType: "number",
                            caption: "Distance Carry Over (meter)",
                            visible: false
                        },
                        {
                            dataField: "elevation",
                            dataType: "number",
                            caption: "Elevation",
                            visible: false,
                        },
                        {
                            dataField: "elevation_carry_over",
                            dataType: "number",
                            caption: "Elevation Carry Over",
                            visible: false
                        },
                        {
                            dataField: "accounting_period_id",
                            dataType: "text",
                            caption: "Accounting Period",
                            width: "100px",
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
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                        },
                        {
                            dataField: "transport_model",
                            dataType: "text",
                            caption: "Transportation Model",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/TransportModelIdLookup",
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
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                            visible: false,
                        },
                        {
                            dataField: "location_id",
                            dataType: "string",
                            caption: "Location",
                            //width: "30%",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: url + "/LocationIDLookup",
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                }),
                                valueExpr: "value",
                                displayExpr: "text",
                            },
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                        },
                        {
                            dataField: "business_process_id",
                            dataType: "string",
                            caption: "Business Process",
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=business-process",
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
                            formItem: {
                                editorOptions: {
                                    showClearButton: true
                                }
                            },
                            visible: false
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
                        enabled: false,
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
                        mode: 'popup',
                        allowUpdating: true,
                        allowAdding: true,
                        allowDeleting: true
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
                        e.data.joint_survey_id = currentRecord.id;
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
                url: url + "/UploadDocument",
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

    $('#btnApprove').on('click', function () {
        var teks = $('#approval_confirmation').val().toUpperCase();
        var JointSurveyId = $('#joint_survey_id').val();

        if (teks == "APPROVE") {
            $('#modal-approval').modal("hide");

            $.ajax({
                url: url + "/Approve?Id=" + encodeURIComponent(JointSurveyId),
                type: 'POST',
                cache: false,
                contentType: "application/json",
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                ////console.log(result);
                if (result) {
                    if (result.id != null) {
                        $("#grid").dxDataGrid("refresh");
                        toastr["success"](result.message ?? "Success");
                    }
                    else {
                        toastr["error"](result.message ?? "Error");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Action failed.");
            });
        }
    });

});