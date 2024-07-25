$(function () {
    var token = $.cookie("Token");
    var $desDemContractId = $("input[name=desdem_contract_id]");
    var $despatchOrderId = $("input[name=despatch_order_id]");
    var $sofId = $("input[name=sof_id]");
    var $desDemDetailId = $("input[name=despatch_demurrage_detail_id]");

    var salesContractTermId = document.querySelector("[name=desdem_contract_id]").value

    /**
     * =========================
     * Despatch/Demurrage Term Grid
     * =========================
     */

    const getDesDemContract = () => {
        $.ajax({
            type: "GET",
            url: "/api/DespatchDemurrage/Contract/DataDetail?Id=" + encodeURIComponent($desDemContractId.val()),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let desDemContractData = response;
                }
            }
        })
    }

    const getDesDemContractDetail = () => {
        $.ajax({
            type: "GET",
            url: "/api/DespatchDemurrage/ContractDetail/DataDetailByDesDemId?desDemId=" + encodeURIComponent($desDemContractId.val()),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                // If salesContractDespatchDemurrageTerm 
                if (response) {
                    let DesDemContractDetailData = response
                    desDemContractDetailForm.option("formData", DesDemContractDetailData.data[0]);
                }
            }
        })
    }

    const saveDesDemContractDetail = (formData) => {
        $.ajax({
            type: "POST",
            url: "/api/DespatchDemurrage/ContractDetail/InsertData",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let DesDemContractDetailData = response
                    $desDemDetailId.val(DesDemContractDetailData.id);

                    // Show successfuly saved popup
                    let successPopup = $("<div>").dxPopup({
                        width: 300,
                        height: "auto",
                        dragEnabled: false,
                        hideOnOutsideClick: true,
                        showTitle: true,
                        title: "Success",
                        contentTemplate: function () {
                            return $(`<h5 class="text-center">All changes are saved.</h5>`)
                        }
                    }).appendTo("#despatch-demurrage-term-form").dxPopup("instance")

                    successPopup.show()

                    if (!$desDemContractId.val()) {
                        // Update sales contract despatch demurrage term grid
                        updateSalesContractDespatchDemurrageDelay(DesDemContractDetailData)
                    }
                }
            }
        })
    }

    const updateDesDemContractDetail = (formData) => {
        $.ajax({
            type: "PUT",
            url: "/api/DespatchDemurrage/ContractDetail/UpdateData?key=" + $desDemDetailId.val(),
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let DesDemContractDetailData = response

                    // Show successfuly saved popup
                    let successPopup = $("<div>").dxPopup({
                        width: 300,
                        height: "auto",
                        dragEnabled: false,
                        hideOnOutsideClick: true,
                        showTitle: true,
                        title: "Success",
                        contentTemplate: function () {
                            return $(`<h5 class="text-center">All changes are saved.</h5>`)
                        }
                    }).appendTo("#despatch-demurrage-term-form").dxPopup("instance")

                    successPopup.show()

                    if (!$desDemContractId.val()) {
                        // Update sales contract despatch demurrage term grid
                        updateSalesContractDespatchDemurrageDelay(DesDemContractDetailData)
                    }
                }
            }
        })
    }

    const updateSalesContractDespatchDemurrageDelay = (DesDemContractDetailData) => {
        /**
         * Update despatchDemurrageDelayGrid options
         * after getting salesContractDespatchDemurrageTerm data
         * Options that need to updated:
         * - editing.allowAdding : set true
         * - onInitNewRow : set sales_contract_despatch_demurrage_id
         * - dataSource : set params
         */
        despatchDemurrageDelayGrid.option("editing.allowAdding", true)
        despatchDemurrageDelayGrid.option("onInitNewRow", function (e) {
            e.data.sales_contract_despatch_demurrage_id = DesDemContractDetailData.id;
        })
        contractProductSpecificationGrid.option("dataSource", DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: desDemDelayUrl + "/DataGrid?termProductId=" + encodeURIComponent(DesDemContractDetailData.id),
            insertUrl: desDemDelayUrl + "/InsertData",
            updateUrl: desDemDelayUrl + "/UpdateData",
            deleteUrl: desDemDelayUrl + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };
            }
        }))
    }
    
    let desDemContractDetailForm = $("#despatch-demurrage-term-form").dxForm({
        formData: {
            id: "",
            despatch_demurrage_id: $desDemContractId.val(),
            port_location: "",
            despatch_percent: "",
            loading_rate: "",
            loading_rate_unit: "",
            turn_time: "",
            turn_time_unit: "",
        },
        colCount: 2,
        items: [
            {
                dataField: "port_location",
                label: {
                    text: "Location"
                },
                editorType: "dxSelectBox",
                editorOptions: {
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
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text"
                },
            },
            {
                dataField: "contract_name",
                dataType: "text",
                label: { text: "DesDem Term Name" },
                editorOptions: { readOnly: true }
            },
            {
                dataField: "despatch_percent",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                label: {
                    text: "Despatch %"
                },
            },
            {
                dataField: "loading_rate",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                label: {
                    text: "Loading Rate"
                },
            },
            {
                dataField: "loading_rate_unit",
                label: {
                    text: "Loading Rate Unit"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
            },
            {
                dataField: "turn_time",
                label: {
                    text: "Turn Time"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
            },
            {
                dataField: "turn_time_unit",
                label: {
                    text: "Turn Time Unit",
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/UOM/UOM/UOMIdLookup",
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
            },
            {
                dataField: "laytime_commenced",
                label: {
                    text: "Laytime Commenced"
                },
                editorType: "dxDateBox",
                editorOptions: {
                    format: "fixedPoint",
                },
            },
            {
                dataField: "actual_commenced",
                label: {
                    text: "Actual Commenced"
                },
                editorType: "dxDateBox",
                editorOptions: {
                    format: "fixedPoint",
                },
            },
            {
                dataField: "laytime_completed",
                label: {
                    text: "Laytime Completed"
                },
                editorType: "dxDateBox",
                editorOptions: {
                    format: "fixedPoint",
                },
            },
            {
                dataField: "actual_completed",
                label: {
                    text: "Actual Completed"
                },
                editorType: "dxDateBox",
                editorOptions: {
                    format: "fixedPoint",
                },
            },
            {
                dataField: "sof_id",
                label: {
                    text: "Laytime Calculation"
                },
                colSpan: 2,
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Port/StatementOfFact/StatemenfOfFactIdLookup",
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
            },

            {
                dataField: "currency_id",
                label: {
                    text: "Currency"
                },
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
            },
            {
                dataField: "rate",
                editorType: "dxNumberBox",
                colSpan: 2,
                editorOptions: {
                    format: "fixedPoint",
                    step: 0
                },
                label: {
                    text: "Rate"
                },
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
                        let data = desDemContractDetailForm.option("formData");
                        if ($desDemDetailId.val() != "") {
                            data.id = $desDemDetailId.val();
                        }
                        data.despatch_demurrage_id = $desDemContractId.val();
                        let formData = new FormData();
                        formData.append("values", JSON.stringify(data));

                        //console.log(data);
                        if ($desDemDetailId.val() == "") {
                            saveDesDemContractDetail(formData);
                        } else {
                            formData.append("key", $desDemDetailId.val());
                            updateDesDemContractDetail(formData);
                        }
                    }
                }
            }
        ],
        onFieldDataChanged: function (data) {
            if (data.dataField == "sof_id") {
                $sofId.val(data.value);
            }
        },
        onInitialized: function () {
            // Get Detail
            if ($desDemContractId.val()) {
                getDesDemContractDetail();
            }
        }
    }).dxForm("instance");

    let desDemDelayUrl = "/api/DespatchDemurrage/DespatchDemurrageDelay";
    let despatchDemurrageDelayGrid = $("#despatch-demurrage-delay-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: desDemDelayUrl + "/DataGridByDespatchDemurrageId?despatchDemurrageId=" + encodeURIComponent($desDemContractId.val()),
            insertUrl: desDemDelayUrl + "/InsertData",
            updateUrl: desDemDelayUrl + "/UpdateData",
            deleteUrl: desDemDelayUrl + "/DeleteData",
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
                dataField: "event_category_id",
                caption: "Event Definition",
                dataType: "text",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Port/StatementOfFactDetail/EventCategoryIdLookupBySofId?sof_id=" + $sofId.val(),
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
                //setCellValue: function (rowData, value) {
                //    rowData.incident_category_id = value
                //    rowData.incident_id = null
                //},
                visible: true
            },
            //{
            //    dataField: "incident_id",
            //    caption: "Event Definition",
            //    dataType: "text",
            //    lookup: {
            //        dataSource: function (options) {
            //            return {
            //                store: DevExpress.data.AspNet.createStore({
            //                    key: "value",
            //                    loadUrl: "/api/Incident/Incident/IncidentIdLookup",
            //                    onBeforeSend: function (method, ajaxOptions) {
            //                        ajaxOptions.xhrFields = { withCredentials: true };
            //                        ajaxOptions.beforeSend = function (request) {
            //                            request.setRequestHeader("Authorization", "Bearer " + token);
            //                        };
            //                    }
            //                }),
            //                filter: options.data ? ["incident_category_id", "=", options.data.incident_category_id] : null
            //            }
            //        },
            //        valueExpr: "value",
            //        displayExpr: "text"
            //    },
            //},
            {
                dataField: "demurrage_percent",
                caption: "Demurrage Applicable %",
                dataType: "number",
            },
            {
                dataField: "despatch_percent",
                dataType: "number",
                caption: "Despatch Applicable %",
            },
        ],
        onEditorPreparing: function (e) {
            if (e.parentType === 'searchPanel') {
                e.editorOptions.onValueChanged = function (arg) {
                    if (arg.value.length == 0 || arg.value.length > 2) {
                        e.component.searchByText(arg.value);
                    }
                }
            }
            if (e.dataField === "incident_id" && e.parentType === "dataRow") {
                e.editorOptions.disabled = !e.row.data.incident_id && (!e.row.data.incident_category_id || e.row.data.incident_category_id === null);
            }
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
            allowAdding: $desDemContractId.val() ? true : false,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 1,
                items: [
                    {
                        dataField: "event_category_id",
                    },
                    //{
                    //    dataField: "incident_id",
                    //},
                    {
                        dataField: "demurrage_percent",
                    },
                    {
                        dataField: "despatch_percent",
                    },
                ]
            }
        },
        onInitNewRow: function (e) {
            e.data.despatch_demurrage_id = $desDemContractId.val();
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
    }).dxDataGrid("instance");
});


