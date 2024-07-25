$(function () {

    var token = $.cookie("Token");
    var areaName = "Sales";
    var entityName = "DespatchOrderDocument";
    var url = "/api/" + areaName + "/" + entityName;
    let is_vessel_geared = false;
    var despatchOrderData = null;

    var grid = $("#grid").dxDataGrid({
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
                dataField: "despatch_order_number",
                dataType: "string",
                caption: "Shipping Order Number",
                sortOrder: "asc",
                allowEditing: false
            },
            {
                dataField: "filename",
                dataType: "string",
                caption: "File Name",
                formItem: {
                    colSpan: 2
                },
                cellTemplate: function (container, options) {
                    let attachmentUrl = options.value
                    let attachmentName = /[^\\]*$/.exec(attachmentUrl)[0] // Get only the file name and its extension

                    $(`<span><i class="fas fa-file mr-2"></i>${attachmentName}</span>`).appendTo(container)
                }
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
                        let attachment = e.row.data
                        let attachmentName = /[^\\]*$/.exec(attachment.filename)[0]

                        let xhr = new XMLHttpRequest()
                        xhr.open("GET", "/api/Sales/DespatchOrderDocument/DownloadDocument/" + attachment.id, true)
                        xhr.responseType = "blob"
                        xhr.setRequestHeader("Authorization", "Bearer " + token)

                        xhr.onload = function (e) {
                            let blobURL = window.webkitURL.createObjectURL(xhr.response)

                            let a = document.createElement("a")
                            a.href = blobURL
                            a.download = attachmentName
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
                /*buttons: ["delete"]*/
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
        masterDetail: {
            enabled: false,
            template: function (container, options) {
                var currentRecord = options.data;
                ////console.log(currentRecord)
                despatchOrderData = currentRecord;

                // Despatch Order Information container
                renderDespatchOrderInformation(currentRecord, container)

                // Despatch Order Product Specification
                renderDespatchOrderProductSpecification(currentRecord, container)

                renderDespatchOrderDocument(currentRecord, container)
                renderTranshipment(currentRecord, container)
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
            //if (e.parentType === "dataRow" && e.dataField == "sales_contract_id") {

            //    let standardHandler = e.editorOptions.onValueChanged
            //    let index = e.row.rowIndex
            //    let grid = e.component

            //    e.editorOptions.onValueChanged = function (e) { // Overiding the standard handler

            //        // Get its value (Id) on value changed
            //        let salesContract = e.value

            //        // Get another data from API after getting the Id
            //        $.ajax({
            //            url: '/api/Sales/SalesContract/DataDetail?Id=' + salesContract,
            //            type: 'GET',
            //            contentType: "application/json",
            //            beforeSend: function (xhr) {
            //                xhr.setRequestHeader("Authorization", "Bearer " + token);
            //            },
            //            success: function (response) {
            //                let salesContractData = response.data[0]

            //                ////console.log("SalesContract/DataDetail: ", response);

            //                // Set its corresponded field's value
            //                grid.cellValue(index, "customer_id", salesContractData.customer_id)
            //                grid.cellValue(index, "seller_id", salesContractData.seller_id)
            //                //grid.cellValue(index, "ship_to", salesContractData.end_user_id)


            //                $.ajax({
            //                    url: '/api/Sales/Customer/CreditLimitAlert?customer_id=' + salesContractData.customer_id,
            //                    type: 'GET',
            //                    contentType: "application/json",
            //                    beforeSend: function (xhr) {
            //                        xhr.setRequestHeader("Authorization", "Bearer " + token);
            //                    },
            //                    success: function (response) {
            //                        let creditLimitAlert = response[0]

            //                        // Set its corresponded field's value
            //                        grid.cellValue(index, "credit_limit_alert", creditLimitAlert.persentase + '%')
            //                        grid.cellValue(index, "credit_limit_color", creditLimitAlert.color)
            //                    }
            //                })

            //            }
            //        })

            //        standardHandler(e) // Calling the standard handler to save the edited value
            //    }
            //}

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
            pageSize: 20
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
            //form: {
            //    colCount: 2,
            //    items: [
            //        {
            //            dataField: "despatch_order_number",
            //            colSpan: 2
            //        },
            //        {
            //            dataField: "filename",
            //            colSpan: 2
            //        },
            //    ],
            //    //customizeItem: function (item) {
            //    //    if (item.dataField === 'credit_limit_alert') {
            //    //        item.visible = grid.__creditLimitAlertIsVisible;
            //    //    }
            //    //}
            //}
        },
        grouping: {
            contextMenuEnabled: true,
            autoExpandAll: false
        },
        //onContentReady: function (e) {
        //    let grid = e.component
        //    let queryString = window.location.search
        //    let params = new URLSearchParams(queryString)

        //    let despatchOrderId = params.get("Id")

        //    if (despatchOrderId) {
        //        grid.filter(["id", "=", despatchOrderId])

        //        /* Open edit form */
        //        if (params.get("openEditingForm") == "true") {
        //            let rowIndex = grid.getRowIndexByKey(despatchOrderId)

        //            grid.editRow(rowIndex)
        //        }
        //    }
        //},
        onInitNewRow: function (e) {
            //e.component.__creditLimitAlertIsVisible = true
        },
        onEditingStart: function (e) {
            //e.component.__creditLimitAlertIsVisible = false
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
    }).dxDataGrid("instance");

    // Despatch Order Information Container
    const renderDespatchOrderInformation = (currentRecord, container) => {
        let billOfLadingDateFormatted = currentRecord.bill_of_lading_date ? moment(currentRecord.bill_of_lading_date).format("D MMM YYYY") : '-'
        let draftSurveyBillOfLadingDateFormatted = currentRecord.draft_survey_bill_lading_date ? moment(currentRecord.draft_survey_bill_lading_date).format("D MMM YYYY") : '-'
        let laycanStartFormatted = currentRecord.laycan_start ? moment(currentRecord.laycan_start).format("D MMM YYYY") : '-'
        let laycanEndFormatted = currentRecord.laycan_end ? moment(currentRecord.laycan_end).format("D MMM YYYY") : '-'
        let etaPlanFormatted = currentRecord.laycan_end ? moment(currentRecord.eta_plan).format("D MMM YYYY") : '-'
        let orderRefDateFormatted = currentRecord.despatch_order_date ? moment(currentRecord.despatch_order_date).format("D MMM YYYY") : '-'

        //console.log(currentRecord)

        $(`
            <div>
                <div class="row align-items-center mb-3">
                    <div class="col-md-6">
                        <h5>Shipping Order</h5>
                    </div>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Overview</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-6 pr-0">
                                    <div class="headline-title-container">
                                        <small class="font-weight-normal d-block mb-1">Despatch Order Number</small>
                                        <h4 class="headline-title font-weight-bold">${(currentRecord.despatch_order_number ? currentRecord.despatch_order_number : "-")}</h4>
                                    </div>
                                </div>
                                <div class="col-md-6 pl-0">
                                    <div class="headline-detail-container">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-list fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Contract Term</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.contract_term_name ? currentRecord.contract_term_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-building fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Seller</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.seller_name ? currentRecord.seller_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start mb-3">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-user fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Buyer</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.customer_name ? currentRecord.customer_name : "-")}</h5>
                                                    </div>
                                                </div>
                                                <div class="d-flex align-items-start">
                                                    <div class="d-inline-block mr-3">
                                                        <div class="icon-circle">
                                                            <i class="fas fa-ship fa-sm"></i>
                                                        </div>
                                                    </div>
                                                    <div class="d-inline-block">
                                                        <small class="font-weight-normal text-muted d-block mb-1">Ship To</small>
                                                        <h5 class="font-weight-bold">${(currentRecord.ship_to_name ? currentRecord.ship_to_name : "-")}</h5>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Order Information</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="headline-detail-container">
                                        <div class="row mb-2">
                                            <div class="col-md-12">
                                                <small class="font-weight-normal text-muted d-block mb-1">Shipping Order Number</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.despatch_order_number ? currentRecord.despatch_order_number : "-")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Shipping Order Date</small>
                                                <p class="font-weight-bold m-0">${orderRefDateFormatted}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Shipping Plan</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.despatch_plan_name ? currentRecord.despatch_plan_name : "-")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">COW</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.draft_survey_number ? currentRecord.draft_survey_number : "-")}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">COW Quantity</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.draft_survey_quantity ? currentRecord.draft_survey_quantity : "-")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Bill of Lading Date</small>
                                                <p class="font-weight-bold m-0">${draftSurveyBillOfLadingDateFormatted}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Bill of Lading Number</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.draft_survey_bill_lading_number ? currentRecord.draft_survey_bill_lading_number : "-")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">COA</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.quality_sampling_number ? currentRecord.quality_sampling_number : "-")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Despatch Fulfilment Type</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.fulfilment_type_name ? currentRecord.fulfilment_type_name : "-")}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Contract Product</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.sales_contract_product_name ? currentRecord.sales_contract_product_name : "-")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Delivery Term</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.delivery_term_name ? currentRecord.delivery_term_name : "-")}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Unit</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.uom_name ? currentRecord.uom_name : "-")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Letter of Credit</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.letter_of_credit ? currentRecord.letter_of_credit : "-")}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Notes</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.notes ? currentRecord.notes : "-")}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row mb-5">
                    <div class="col-md-6">
                        <div class="master-detail-caption mb-2">Shipment Information</div>
                        <div class="card card-mcs card-headline">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="headline-detail-container">
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Laycan Start</small>
                                                <p class="font-weight-bold m-0">${laycanStartFormatted}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Laycan End</small>
                                                <p class="font-weight-bold m-0">${laycanEndFormatted}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">ETA Plan</small>
                                                <p class="font-weight-bold m-0">${(etaPlanFormatted)}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Bill of Lading Date</small>
                                                <p class="font-weight-bold m-0">${billOfLadingDateFormatted}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Laycan Commmitted</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.laycan_committed == true ? "Yes" : "No")}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">ETA Committed</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.eta_committed == true ? "Yes" : "No")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Vessel</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.vehicle_name ? currentRecord.vehicle_name : "-")}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Vessel Committed</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.vessel_committed == true ? "Yes" : "No")}</p>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Loading Port</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.loading_port ? currentRecord.loading_port : "-")}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <small class="font-weight-normal text-muted d-block mb-1">Discharge Port</small>
                                                <p class="font-weight-bold m-0">${(currentRecord.discharge_port == true ? currentRecord.discharge_port : "-")}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `).appendTo(container)
    }

    // Despatch Order Product Specification 
    const renderDespatchOrderProductSpecification = (currentRecord, container) => {
        var urlDetail = "/api/StockpileManagement/QualitySamplingAnalyte";

        $("<div>")
            .addClass("master-detail-caption")
            .text("COA Result")
            .appendTo(container);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: urlDetail + "/ByDespatchOrderId/" + encodeURIComponent(currentRecord.id),
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
                        dataField: "analyte_id",
                        dataType: "text",
                        caption: "Analyte",
                        validationRules: [{
                            type: "required",
                            message: "The field is required."
                        }],
                        lookup: {
                            dataSource: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: urlDetail + "/AnalyteIdLookup",
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
                        dataField: "uom_id",
                        dataType: "text",
                        caption: "Unit",
                        validationRules: [{
                            type: "required",
                            message: "The field is required."
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
                        }
                    },
                    {
                        dataField: "analyte_value",
                        dataType: "number",
                        caption: "Value",
                        allowEditing: false
                    },
                    {
                        dataField: "target",
                        dataType: "number",
                        caption: "Target",
                        allowEditing: false
                    },
                    {
                        dataField: "minimum",
                        dataType: "number",
                        caption: "Minimum",
                        allowEditing: false
                    },
                    {
                        dataField: "maximum",
                        dataType: "number",
                        caption: "Maximum",
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
                    form: {
                        colCount: 2,
                        items: [
                            {
                                dataField: "analyte_id",
                            },
                            {
                                dataField: "uom_id",
                            },
                            {
                                dataField: "analyte_value"
                            },
                        ],
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
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailName + '.xlsx');
                        });
                    });
                    e.cancel = true;
                }
            }).appendTo(container);
    }

    const renderTranshipment = (currentRecord, container) => {
        var urlDetail = "/api/Sales/SalesInvoiceTranshipment";

        $("<div>")
            .addClass("master-detail-caption")
            .text("Transhipment")
            .appendTo(container);

        $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: urlDetail + "/ByDespatchOrderId/" + encodeURIComponent(currentRecord.id),
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
                        dataField: "transaction_number",
                        dataType: "string",
                        caption: "Transaction Number",
                        sortOrder: "asc",
                        allowEditing: false
                    },
                    {
                        dataField: "start_datetime",
                        dataType: "datetime",
                        sortOrder: "asc",
                        caption: "Commenced Loading DateTime",
                        allowEditing: false
                    },
                    {
                        dataField: "end_datetime",
                        dataType: "datetime",
                        sortOrder: "asc",
                        caption: "Completed Loading DateTime",
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
                    form: {
                        colCount: 2,
                        items: [
                            {
                                dataField: "transaction_number",
                            },
                            {
                                dataField: "start_datetime",
                            },
                            {
                                dataField: "end_datetime",
                            },
                           
                        ],
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
                onInitNewRow: function (e) {
                    e.data.start_datetime = currentRecord.start_datetime;
                    e.data.end_datetime = currentRecord.end_datetime;
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
            }).appendTo(container);
    }


    // Initial Information Popup
    let initialInformationPopupOptions = {
        title: "Initial Information",
        height: "'auto'",
        hideOnOutsideClick: true,
        contentTemplate: function () {

            var initialInformationForm = $("<div>").dxForm({
                formData: {
                    despatch_order_id: "",
                    vessel_name: "",
                    eta_plan: "",
                    loading_port: "",
                    seller: "",
                    customer_name: "",
                    customer_address: "",
                    customer_additional_info: "",
                    contract_product_name: "",
                    status: "",
                },
                colCount: 2,
                readOnly: true,
                items: [
                    {
                        dataField: "despatch_order_id",
                        label: {
                            text: "Shipping Order Id"
                        },
                        visible: false
                    },
                    {
                        dataField: "seller",
                        label: {
                            text: "Seller Name",
                        },
                    },
                    {
                        dataField: "customer_name",
                        label: {
                            text: "Customer Name",
                        },
                    },
                    {
                        dataField: "customer_address",
                        label: {
                            text: "Customer Primary Address",
                        },
                        editorType: "dxTextArea",
                    },
                    {
                        dataField: "customer_additional_info",
                        label: {
                            text: "Customer Additional Information",
                        },
                        editorType: "dxTextArea",
                    },
                    {
                        dataField: "contract_product_name",
                        label: {
                            text: "Contract Product Name"
                        },
                    },
                    {
                        dataField: "eta_plan",
                        label: {
                            text: "ETA Plan"
                        },
                        editorType: "dxDateBox",
                    },
                    {
                        dataField: "vessel_name",
                        label: {
                            text: "Vessel Name"
                        },
                    },
                    {
                        dataField: "loading_port",
                        label: {
                            text: "Loading Port"
                        },
                    },

                    {
                        dataField: "status",
                        label: {
                            text: "Status"
                        },
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
                                let data = initialInformationForm.dxForm("instance").option("formData");
                                let formData = new FormData()
                                formData.append("key", data.despatch_order_id)
                                formData.append("values", JSON.stringify(data))

                                saveInitialInformationForm(formData)
                            }
                        }
                    }
                ],
                onInitialized: () => {
                    $.ajax({
                        type: "GET",
                        url: "/api/Sales/InitialInformation/GetByDespatchOrderId/" + encodeURIComponent(despatchOrderData.id),
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            // Update form formData with response from api
                            if (response) {
                                initialInformationForm.dxForm("instance").option("formData", response)
                            }
                        }
                    })
                }
            })

            return initialInformationForm;
        }
    }
    var initialInformationPopup = $("#initial-information-popup").dxPopup(initialInformationPopupOptions).dxPopup("instance")

    const showInitialInformationPopup = function () {
        initialInformationPopup.option("contentTemplate", initialInformationPopupOptions.contentTemplate.bind(this));
        initialInformationPopup.show()
    }

    const saveInitialInformationForm = (formData) => {
        $.ajax({
            type: "POST",
            url: "/api/Sales/InitialInformation/InsertData",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {

                    // Show successfuly saved popup
                    let successPopup = $("<div>").dxPopup({
                        width: 300,
                        height: "auto",
                        dragEnabled: false,
                        hideOnOutsideClick: true,
                        showTitle: true,
                        title: "Success",
                        contentTemplate: function () {
                            return $(`<p class="text-center">Initial Information saved.</p>`)
                        }
                    }).appendTo("body").dxPopup("instance")

                    successPopup.show();
                }

            }
        })
    }

    var calculateLaytimeAllowed = function (cargo_quantity, loading_rate) {
        //console.log("cargo_quantity", cargo_quantity);
        //console.log("loading_rate", loading_rate);
        var result = {
            value: 0,
            text: ""
        }
        if (cargo_quantity === undefined || loading_rate === undefined || cargo_quantity === 0 || loading_rate === 0) {
            return result
        } 
        result.value = parseInt((parseFloat(cargo_quantity) / parseFloat(loading_rate)) * 86400);
        result.text = secondsToDhms(result.value);
        return result;
    }

    function secondsToDhms(seconds) {
        seconds = Number(seconds);
        var d = Math.floor(seconds / (3600 * 24));
        var h = Math.floor(seconds % (3600 * 24) / 3600);
        var m = Math.floor(seconds % 3600 / 60);
        var s = Math.floor(seconds % 60);

        var dDisplay = d > 0 ? d + (d == 1 ? " Day " : " Days ") : "";
        var hDisplay = h > 0 ? h + (h == 1 ? " Hour " : " Hours ") : "";
        var mDisplay = m > 0 ? m + (m == 1 ? " Minute " : " Minutes ") : "";
        return dDisplay + hDisplay + mDisplay;
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
                url: "/api/Sales/DespatchOrder/UploadDocument",
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

    /**
     * =========================
     * Attachments Grid
     * =========================
     */

    const maxFileSize = 52428800;

    let despatchOrderDocumentDataGrid
    const renderDespatchOrderDocument = function (currentRecord, container) {
        let attachmentUrlDetail = "/api/Sales/DespatchOrderDocument";
        let despatchOrderDocumentContainer = $("<div class='mb-5'>")
        despatchOrderDocumentContainer.appendTo(container)

        let titleContainer = $(`
            <div class="row mb-3 mt-6 align-items-center">
                <div class="col-md-6">
                    <div class="master-detail-caption">File Attachments</div>
                </div>
                <div class="col-md-6 btn-container float-right">
                </div>
            </div>
        `).appendTo(despatchOrderDocumentContainer)

        $("<div>")
            .dxButton({
                stylingMode: "contained",
                icon: "plus",
                text: "Add Attachments",
                type: "normal",
                width: "'auto'",
                onClick: function () {
                    openAddAttachmentPopup()
                },
                elementAttr: {
                    class: "float-right"
                }
            }).appendTo(titleContainer.find(".btn-container")[0])

        despatchOrderDocumentDataGrid = $("<div>")
            .dxDataGrid({
                dataSource: DevExpress.data.AspNet.createStore({
                    key: "id",
                    loadUrl: attachmentUrlDetail + "/DataGrid?Id=" + encodeURIComponent(currentRecord.id),
                    insertUrl: attachmentUrlDetail + "/InsertData",
                    updateUrl: attachmentUrlDetail + "/UpdateData",
                    deleteUrl: attachmentUrlDetail + "/DeleteData",
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
                        dataField: "despatch_order_id",
                        dataType: "string",
                        allowEditing: false,
                        visible: false,
                        calculateCellValue: function () {
                            return currentRecord.id;
                        }
                    },
                    {
                        dataField: "filename",
                        dataType: "string",
                        caption: "File name",
                        formItem: {
                            colSpan: 2
                        },
                        cellTemplate: function (container, options) {
                            let attachmentUrl = options.value
                            let attachmentName = /[^\\]*$/.exec(attachmentUrl)[0] // Get only the file name and its extension

                            $(`<span><i class="fas fa-file mr-2"></i>${attachmentName}</span>`).appendTo(container)
                        }

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
                                let attachment = e.row.data
                                let attachmentName = /[^\\]*$/.exec(attachment.filename)[0]

                                let xhr = new XMLHttpRequest()
                                xhr.open("GET", "/api/Sales/DespatchOrderDocument/DownloadDocument/" + attachment.id, true)
                                xhr.responseType = "blob"
                                xhr.setRequestHeader("Authorization", "Bearer " + token)

                                xhr.onload = function (e) {
                                    let blobURL = window.webkitURL.createObjectURL(xhr.response)

                                    let a = document.createElement("a")
                                    a.href = blobURL
                                    a.download = attachmentName
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
                    e.data.business_partner_id = currentRecord.id;
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
            }).appendTo(despatchOrderDocumentContainer).dxDataGrid("instance");
    }

    const addAttachmentPopupOptions = {
        width: 500,
        height: "auto",
        showTitle: true,
        title: "Add Attachment",
        visible: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        contentTemplate: function () {
            let despatchOrderIdInput =
                $("<div>")
                    .dxTextBox({
                        name: "despatch_order_id",
                        value: despatchOrderData.id,
                        readOnly: true,
                        visible: false
                    })

            let attachmentInput =
                $("<div class='mb-5 dx-fileuploader-mcs'>")
                    .dxFileUploader({
                        uploadMode: "useForm",
                        multiple: false
                    })

            let submitButton =
                $("<div>")
                    .dxButton({
                        text: "Submit",
                        onClick: function (e) {
                            let despatchOrderId = despatchOrderIdInput.dxTextBox("instance").option("value")
                            let file = attachmentInput.dxFileUploader("instance").option("value")[0]

                            var reader = new FileReader();
                            reader.readAsDataURL(file);
                            reader.onload = function () {
                                let fileName = file.name
                                let fileSize = file.size
                                let data = reader.result.split(',')[1]

                                if (fileSize == 0) {
                                    alert("File content is empty.")
                                    return;
                                }
                                if (fileSize >= maxFileSize) {
                                    alert("File size exceeds 50 MB.");
                                    return;
                                }

                                let formData = {
                                    "despatchOrderId": despatchOrderId,
                                    "fileName": fileName,
                                    "fileSize": fileSize,
                                    "data": data
                                }

                                $.ajax({
                                    url: "/api/Sales/DespatchOrderDocument/InsertData",
                                    data: JSON.stringify(formData),
                                    type: "POST",
                                    contentType: "application/json",
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {
                                        addAttachmentPopup.hide()
                                        despatchOrderDocumentDataGrid.refresh()
                                    }
                                })
                            }
                        }
                    })

            let formContainer = $("<form enctype='multipart/form-data'>")
                .append(despatchOrderIdInput, attachmentInput, submitButton)

            return formContainer;
        }
    }

    const addAttachmentPopup = $("<div>")
        .dxPopup(addAttachmentPopupOptions).appendTo("body").dxPopup("instance")

    const openAddAttachmentPopup = function () {
        addAttachmentPopup.option("contentTemplate", addAttachmentPopupOptions.contentTemplate.bind(this));
        addAttachmentPopup.show()
    }

    //******* new popup
    const documentPopupOptions = {
        width: "70%",
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
                    despatchOrderId: "",
                    filename: ""
                },
                colCount: 2,
                items: [
                    {
                        dataField: "despatch_order_id",
                        editorType: "dxSelectBox",
                        label: {
                            text: "Shipping Order Number"
                        },
                        editorOptions: {
                            dataSource: new DevExpress.data.DataSource({
                                store: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: "/api/Sales/DespatchOrder/DespatchOrderIdLookup",
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                }),
                            }),
                            searchEnabled: true,
                            valueExpr: "value",
                            displayExpr: "text"
                        },
                    },
                    {
                        dataField: "filename",
                        name: "filename",
                        label: {
                            text: "File"
                        },
                        template: function (data, itemElement) {
                            itemElement.append($("<div>").attr("id", "filename").dxFileUploader({
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
                                let formData = formContainer.dxForm("instance").option('formData');
                                let file = formData.filename[0];

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
                                        "despatchOrderId": formData.despatch_order_id,
                                        "fileName": fileName,
                                        "fileSize": fileSize,
                                        "data": data
                                    }

                                    $.ajax({
                                        url: `/api/Sales/DespatchOrderDocument/InsertData`,
                                        data: JSON.stringify(newFormData),
                                        type: "POST",
                                        contentType: "application/json",
                                        beforeSend: function (xhr) {
                                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                                        },
                                        success: function (response) {
                                            documentPopup.hide();
                                            grid.refresh();
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

});