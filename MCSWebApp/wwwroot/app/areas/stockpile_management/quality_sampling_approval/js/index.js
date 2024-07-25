$(async function () {

    var token = $.cookie("Token");
    var areaName = "StockpileManagement";
    var entityName = "QualitySamplingApproval";
    var url = "/api/" + areaName + "/" + entityName;
    const maxFileSize = 52428800;
    var HeaderData = null;

    function formatTanggal(x) {
        theDate = new Date(x);
        formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1).toString().padStart(2, "0")
            + "-" + theDate.getDate().toString().padStart(2, "0") + " " + theDate.getHours().toString().padStart(2, "0")
            + ":" + theDate.getMinutes().toString().padStart(2, "0");
        return formatted_date;
    }

    var tgl1 = sessionStorage.getItem("qsaDate1");
    var tgl2 = sessionStorage.getItem("qsaDate2");
    var samplingTypeID = sessionStorage.getItem("qsaSamplingType");

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
            sessionStorage.setItem("qsaDate1", formatTanggal(firstDay));
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
            sessionStorage.setItem("qsaDate2", formatTanggal(lastDay));
            _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                + "/" + encodeURIComponent(formatTanggal(lastDay));
        }
    });

    var _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
        + "/" + encodeURIComponent(formatTanggal(lastDay));
    if (samplingTypeID != null) _loadUrl = _loadUrl + "/" + samplingTypeID;

    var samplingTypeData;
    await $.ajax({
        type: "GET",
        url: url + "/SamplingTypeIdLookup",
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (response) {
            if (response) {
                samplingTypeData = response;
            }
        }
    });

    $('#sampling-type').dxSelectBox({
        dataSource: samplingTypeData,
        valueExpr: "value",
        displayExpr: "text",
        value: samplingTypeID,
        placeholder: 'Filter by Sampling Type',
        showClearButton: true,
        onValueChanged: (e) => {
            if (e.value != null) {
                samplingTypeID = e.value;
                sessionStorage.setItem("qsaSamplingType", samplingTypeID);
                _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                    + "/" + encodeURIComponent(formatTanggal(lastDay)) + "/" + encodeURIComponent(samplingTypeID);
                location.reload();
            }
            else {
                samplingTypeID = null;
                sessionStorage.removeItem("qsaSamplingType");
                _loadUrl = url + "/DataGrid/" + encodeURIComponent(formatTanggal(firstDay))
                    + "/" + encodeURIComponent(formatTanggal(lastDay));
                location.reload();
            }
        }
    });

    $('#btnView').on('click', function () {
        location.reload();
    });

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
                dataField: "sampling_number",
                dataType: "string",
                caption: "Sampling Number",
                sortIndex: 0,
                sortOrder: "asc",
                allowEditing: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
            },
            {
                dataField: "sampling_datetime",
                dataType: "datetime",
                caption: "Sampling DateTime",
                format: "yyyy-MM-dd HH:mm:ss",
                allowEditing: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "surveyor_id",
                dataType: "text",
                caption: "Surveyor",
                allowEditing: false,
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
            },
            {
                dataField: "stock_location_id",
                dataType: "text",
                caption: "Sampling Location",
                allowEditing: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/StockpileLocationIdLookup",
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
            },
            {
                dataField: "product_id",
                dataType: "text",
                caption: "Product",
                allowEditing: false,
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
            },
            {
                dataField: "sampling_template_id",
                dataType: "text",
                caption: "Sampling Template",
                allowEditing: false,
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }],
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/SamplingTemplateIdLookup",
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
            },
            {
                dataField: "despatch_order_id",
                dataType: "text",
                caption: "Shipping Order",
                visible: true,
                allowEditing: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Sales/DespatchOrder/DespatchOrderIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                setCellValue: function (rowData, value) {
                    rowData.despatch_order_id = value;
                },
				formItem: {
					editorOptions: {
						showClearButton: true
					}
				},
            },
            {
                dataField: "despatch_order_link",
                dataType: "string",
                caption: "DO Detail",
                visible: false,
                allowEditing: false,
                allowFiltering: false
            },
            {
                dataField: "is_adjust",
                dataType: "boolean",
                caption: "Is Adjust",
                visible: false,
                allowEditing: false,
            },
            {
                dataField: "is_draft",
                dataType: "boolean",
                caption: "Draft",
                visible: false,
                allowEditing: false,
            },
            {
                dataField: "non_commercial",
                dataType: "boolean",
                caption: "Non Commercial",
                visible: false,
                allowEditing: false,
            },
            {
                dataField: "sampling_type_id",
                dataType: "string",
                caption: "Sampling Type",
                allowEditing: false,
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/StockpileManagement/SamplingType/SamplingTypeIdLookup",
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
            },
            {
                dataField: "shift_id",
                dataType: "text",
                caption: "Shift",
                allowEditing: false,
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Shift/Shift/ShiftIdLookup",
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
                }
            },
            {
                caption: "Approve",
                type: "buttons",
                buttons: [{
                    cssClass: "btn-dxdatagrid",
                    text: "Approve",
                    onClick: function (e) {
                        HeaderData = e.row.data;
                        showApprovalPopup();
                    }
                }]
            },
            {
                type: "buttons",
                buttons: ["edit"]
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
            allowAdding: false,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                itemType: "group",
                items: [
                    {
                        dataField: "sampling_number",
                    },
                    {
                        dataField: "sampling_datetime",
                    },
                    {
                        dataField: "despatch_order_id",
                    },
                    {
                        dataField: "despatch_order_link",
                        editorType: "dxButton",
                        editorOptions: {
                            text: "See Shipping Order Detail",
                            disabled: true
                        }
                    },
                    {
                        dataField: "surveyor_id",
                    },
                    {
                        dataField: "stock_location_id",
                    },
                    {
                        dataField: "product_id",
                    },
                    {
                        dataField: "sampling_template_id",
                    },
                    {
                        dataField: "non_commercial",
                    },
                    {
                        dataField: "is_adjust",
                    },
                    {
                        dataField: "sampling_type_id",
                    },
                    {
                        dataField: "is_draft",
                    },
                    {
                        dataField: "shift_id",
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
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "select") {
                var dataGridOptions = $("#grid").dxDataGrid("option");
                var judul;
                if (e.row.data.is_approved == null || e.row.data.is_approved == false) {
                    judul = "Approve";
                    dataGridOptions.columns[13].buttons[0].disabled = false;
                }
                else {
                    judul = "Approved";
                    dataGridOptions.columns[13].buttons[0].disabled = true;
                }

                dataGridOptions.columns[13].buttons[0].text = judul;
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
            if (e.parentType === "dataRow" && e.dataField == "despatch_order_link") {
                if (e.row.data.despatch_order_id) {
                    let despatchOrderId = e.row.data.despatch_order_id

                    e.editorOptions.onClick = function (e) {
                        window.open("/Sales/DespatchOrder/Index?Id=" + despatchOrderId + "&openEditingForm=true", "_blank")
                    }
                    e.editorOptions.disabled = false
                }
            }

            if (e.parentType === "dataRow" && e.dataField == "despatch_order_id") {
                let standardHandler = e.editorOptions.onValueChanged
                let index = e.row.rowIndex
                let grid = e.component
                let rowData = e.row.data

                e.editorOptions.onValueChanged = async function (e) { // Overiding the standard handler                    

                    // Get its value (Id) on value changed
                    let despatchOrderId = e.value

                    grid.beginCustomLoading()

                    // Get another data from API after getting the Id
                    $.ajax({
                        url: '/api/Sales/DespatchOrder/DataDetail?Id=' + despatchOrderId,
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            let despatchOrderData = response.data;

                            if (despatchOrderData) {
                                grid.beginUpdate()
                                grid.cellValue(index, "surveyor_id", despatchOrderData.surveyor_id)
                                grid.cellValue(index, "product_id", despatchOrderData.product_id)
                                grid.cellValue(index, "stock_location_id", despatchOrderData.vessel_id)
                                grid.endUpdate()
                            }
                        }
                    })

                    setTimeout(() => {
                        grid.endCustomLoading()
                    }, 500)

                    standardHandler(e) // Calling the standard handler to save the edited value
                }
            }

        },
        //onEditingStart: function (e) {
        //    if (e.data !== null && e.data.approved_on !== null) {
        //        e.cancel = true;
        //    }
        //},
        onContentReady: function (e) {
            // hide Save button
            var $buttonSave = e.element.find(".dx-datagrid-form-buttons-container div[aria-label='Save']");
            var showButton = false;
            if ($buttonSave.length == 1 && showButton !== true) {
                $buttonSave.hide();
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

    //******* Approval popup
    let approvalPopupOptions = {
        title: "Approval",
        height: "'auto'",
        width: 400,
        hideOnOutsideClick: true,
        contentTemplate: function () {

            var approvalForm = $("<div>").dxForm({
                formData: {
                    id: "",
                    is_approved: null,
                },
                colCount: 2,
                items: [
                    {
                        dataField: "id",
                        visible: false,
                    },
                    {
                        dataField: "is_approved",
                        editorType: "dxCheckBox",
                        label: {
                            text: "Approve"
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
                                let data = approvalForm.dxForm("instance").option("formData");

                                let formData = new FormData();
                                formData.append("key", data.id);

                                formData.append("values", JSON.stringify(data));

                                saveApprovalForm(formData);
                            }
                        }
                    }
                ],
                onInitialized: () => {
                    $.ajax({
                        type: "GET",
                        url: "/api/StockpileManagement/QualitySamplingApproval/GetQualitySamplingApproval/" + encodeURIComponent(HeaderData.id),
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + token);
                        },
                        success: function (response) {
                            // Update form formData with response from api
                            if (response) {
                                approvalForm.dxForm("instance").option("formData", response)
                            }
                        }
                    })
                }
            })

            return approvalForm;
        }
    }

    var approvalPopup = $("#approval-popup").dxPopup(approvalPopupOptions).dxPopup("instance")

    const showApprovalPopup = function () {
        approvalPopup.option("contentTemplate", approvalPopupOptions.contentTemplate.bind(this));
        approvalPopup.show()
    }

    const saveApprovalForm = (formData) => {
        $.ajax({
            type: "POST",
            url: "/api/StockpileManagement/QualitySamplingApproval/GiveApproval",
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
                            return $(`<p class="text-center">Data saved.</p>`)
                        }
                    }).appendTo("body").dxPopup("instance");
                    approvalPopup.hide();
                    successPopup.show();
                    $("#grid").dxDataGrid("getDataSource").reload();
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            approvalPopup.hide();
            Swal.fire("Failed !", jqXHR.responseText, "error");
        });
    }
    //********

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Analytes",
                    template: createAnalytesTabTemplate(masterDetailOptions.data)
                },
                //{
                //    title: "Documents",
                //    template: createDocumentsTab(masterDetailOptions.data)
                //}
            ]
        });
    }

    function createAnalytesTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "QualitySamplingAnalyte";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByQualitySamplingId/" + encodeURIComponent(currentRecord.id),
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
                            dataField: "quality_sampling_id",
                            caption: "Quality Sampling Id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            },
                            formItem: {
                                visible: false,
                            }
                        },
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
                            validationRules: [{
                                type: "required",
                                message: "The field is required."
                            }]
                        },
                        {
                            dataField: "order",
                            caption: "Order",
                            dataType: "string",
                            sortOrder: "asc"
                        },
                        {
                            width: "130px",
                            type: "buttons",
                            buttons: [
                                //{
                                //    hint: "Move up",
                                //    icon: "arrowup",
                                //    onClick: function (e) {
                                //        let index = e.row.rowIndex
                                //        debugger;
                                //        //console.log(e);
                                //        if (index == 0) {
                                //            alert("First data cannot be moved up")
                                //            return false
                                //        }


                                //        let formData = new FormData();
                                //        formData.append("key", e.row.data.id)
                                //        formData.append("id", currentRecord.id)
                                //        formData.append("type", -1)

                                //        $.ajax({
                                //            type: "PUT",
                                //            url: urlDetail + "/UpdateOrderData",
                                //            data: formData,
                                //            processData: false,
                                //            contentType: false,
                                //            beforeSend: function (xhr) {
                                //                xhr.setRequestHeader("Authorization", "Bearer " + token);
                                //            },
                                //            success: function (response) {
                                //                if (response) {
                                //                    var qualitySamplingGrid = $("#grid").dxDataGrid("instance");
                                //                    var detailRowIndex = qualitySamplingGrid.getRowIndexByKey(e.row.data.quality_sampling_id) + 1
                                //                    var detailGrid = qualitySamplingGrid.getRowElement(detailRowIndex).find(".dx-datagrid").first().parent().dxDataGrid("instance");
                                //                    detailGrid.refresh()
                                //                }

                                //            }
                                //        })

                                //    }
                                //},
                                //{
                                //    hint: "Move down",
                                //    icon: "arrowdown",
                                //    onClick: function (e) {
                                //        let index = e.row.rowIndex
                                //        debugger;
                                //        var qualitySamplingGrid = $("#grid").dxDataGrid("instance");
                                //        var detailRowIndex = qualitySamplingGrid.getRowIndexByKey(e.row.data.quality_sampling_id) + 1
                                //        var detailGrid = qualitySamplingGrid.getRowElement(detailRowIndex).find(".dx-datagrid").first().parent().dxDataGrid("instance");

                                //        let lastIndex = detailGrid.totalCount() - 1

                                //        if (index == lastIndex) {
                                //            alert("Last data cannot be moved down")
                                //            return false
                                //        }


                                //        let formData = new FormData();
                                //        formData.append("key", e.row.data.id)
                                //        formData.append("id", currentRecord.id)
                                //        formData.append("type", 1)

                                //        $.ajax({
                                //            type: "PUT",
                                //            url: urlDetail + "/UpdateOrderData",
                                //            data: formData,
                                //            processData: false,
                                //            contentType: false,
                                //            beforeSend: function (xhr) {
                                //                xhr.setRequestHeader("Authorization", "Bearer " + token);
                                //            },
                                //            success: function (response) {
                                //                if (response) {
                                //                    var qualitySamplingGrid = $("#grid").dxDataGrid("instance");
                                //                    var detailRowIndex = qualitySamplingGrid.getRowIndexByKey(e.row.data.quality_sampling_id) + 1
                                //                    var detailGrid = qualitySamplingGrid.getRowElement(detailRowIndex).find(".dx-datagrid").first().parent().dxDataGrid("instance");
                                //                    detailGrid.refresh()
                                //                }

                                //            }
                                //        })
                                //    }
                                //},
                                //"edit",
                                //"delete"
                            ]
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
                    onInitNewRow: function (e) {
                        e.data.quality_sampling_id = currentRecord.id;
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
    function createDocumentsTab(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "QualitySamplingDocument";
            let urlDetail = "/api/" + areaName + "/" + detailName;
            HeaderData = currentRecord;

            documentDataGrid = $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByHeaderId/" + encodeURIComponent(currentRecord.id),
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
                            dataField: "quality_sampling_id",
                            allowEditing: false,
                            visible: false,
                            calculateCellValue: function () {
                                return currentRecord.id;
                            }
                        },
                        {
                            dataField: "filename",
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
                                    let documentName = /[^\\]*$/.exec(documentData.filename)[0]

                                    let xhr = new XMLHttpRequest()
                                    xhr.open("GET", "/api/StockpileManagement/QualitySamplingDocument/DownloadDocument/" + documentData.id, true)
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
                        e.data.quality_sampling_id = currentRecord.id;
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
                    quality_sampling_id: HeaderData.id,
                    file: "",
                },
                colCount: 2,
                items: [
                    {
                        dataField: "quality_sampling_id",
                        visible: false
                    },
                    {
                        dataField: "file",
                        name: "file",
                        label: {
                            text: "File"
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
                                let file = formData.file[0]

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

                                    let newFormData = {
                                        "quality_sampling_id": formData.quality_sampling_id,
                                        "fileName": fileName,
                                        "fileSize": fileSize,
                                        "data": data
                                    }

                                    $.ajax({
                                        url: `/api/${areaName}/QualitySamplingDocument/InsertData`,
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

});