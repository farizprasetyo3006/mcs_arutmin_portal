$(function () {

    var token = $.cookie("Token");
    var entityName = "RoyaltyQuantityQuality";
    var royaltyId = document.querySelector("[name=royalty_id]").value;
    var royaltyDate = document.querySelector("[name=royalty_date]").value;

    /* =========================
     * Quantity & Quality Form
     * ========================= */

    const getRoyaltyHeader = () => {
        $.ajax({
            type: "GET",
            url: "/api/Sales/Royalty/Royalty/Detail/" + encodeURIComponent(royaltyId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let royaltyHeaderData = response;
                    qualityQuantityHeaderForm.option("formData", royaltyHeaderData);
                }
            }
        })
    }

    let qualityQuantityHeaderForm = $("#quantity-quality-header-form").dxForm({
        formData: {
            royalty_id: royaltyId,
            volume_loading: ""
        },
        colCount: 2,
        items: [
            {
                dataField: "despatch_order_id",
                label: {
                    text: "DO Number"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/DespatchOrder/DespatchOrderIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text",
                    readOnly: true,
                }
            },
            {
                dataField: "volume_loading",
                label: {
                    text: "Volume Loading"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
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
                        let data = qualityQuantityHeaderForm.option("formData");

                        var volumeLoading = data.volume_loading;

                        if ($.isNumeric(volumeLoading)) {
                            let formData = new FormData();
                            formData.append("key", royaltyId);
                            formData.append("volumeLoading", volumeLoading);

                            $.ajax({
                                type: "POST",
                                url: "/api/Sales/Royalty/Royalty/UpdateVolumeLoading",
                                data: formData,
                                processData: false,
                                contentType: false,
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                },
                                success: function (response) {
                                    if (response) {
                                        Swal.fire("Success!", "Data is updated", "success");
                                    }
                                }
                            });
                        }
                    }
                }
            }   
            //{
            //    itemType: "button",
            //    colSpan: 2,
            //    horizontalAlignment: "right",
            //    buttonOptions: {
            //        text: "Save",
            //        type: "secondary",
            //        useSubmitBehavior: true,
            //        onClick: function () {
            //            let data = qualityQuantityHeaderForm.option("formData");
            //            let formData = new FormData()
            //            formData.append("values", JSON.stringify(data))

            //            //saveSalesContractProductHeader(formData)
            //        }
            //    }
            //}
        ],
        onInitialized: function (e) {
            // Get royalty data if has royaltyId
            if (royaltyId) {
                getRoyaltyHeader()
            }

        },
        onFieldDataChanged: function (data) {

        }
    }).dxForm("instance");


    /* Quantity & Quality Grid
     * ========================= */

    let quantityQualityAnalyteUrl = "/api/Sales/Royalty/QuantityQuality";
    let quantityQualityAnalyteGrid = $("#quantity-quality-analyte-grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: quantityQualityAnalyteUrl + "/DataGrid?royaltyId=" + encodeURIComponent(royaltyId),
            insertUrl: quantityQualityAnalyteUrl + "/InsertData",
            updateUrl: quantityQualityAnalyteUrl + "/UpdateData",
            deleteUrl: quantityQualityAnalyteUrl + "/DeleteData",
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
                dataField: "analyte_id",
                dataType: "string",
                caption: "Analyte",
                lookup: {
                    dataSource: function (options) {
                        return {
                            store: DevExpress.data.AspNet.createStore({
                                key: "value",
                                loadUrl: "/api/Quality/Analyte/AnalyteIdLookup",
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
                calculateSortValue: function (data) {
                    var value = this.calculateCellValue(data);
                    return this.lookup.calculateCellValue(value);
                },
                formItem: {
                    editorOptions: {
                        readOnly: true,
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "uom_id",
                dataType: "string",
                caption: "Unit",
                validationRules: [{
                    type: "required",
                    message: "The field is required."
                }],
                lookup: {
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
                    valueExpr: "value",
                    displayExpr: "text"
                },
                formItem: {
                    editorOptions: {
                        readOnly: true,
                        showClearButton: true
                    }
                },
            },
            {
                dataField: "analyte_value",
                dataType: "number",
                caption: "Value",
                editorOptions: {
                    format: "fixedPoint",
                    precision: 3
                },
                formItem: {
                    editorType: "dxNumberBox",
                    editorOptions: {
                        step: 0,
                        format: {
                            type: "fixedPoint",
                            precision: 2
                        }
                    },
                },
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
            visible: false
        },
        height: 500,
        showBorders: true,
        editing: {
            mode: "form",
            allowAdding: false,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 3,
            }
        },
        onInitNewRow: function (e) {
            e.data.royalty_id = royaltyId
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
        onContentReady: function (e) {
            //$("#btn-fetch").remove();
            //var $customButton = $('<div id="btn-fetch">').dxButton({
            //    icon: 'refresh',
            //    text: "Fetch",
            //    onClick: function () {
            //        $.ajax({
            //            url: '/api/Sales/SalesContractTerm/FetchProductAnalyteIntoSalesContractProduct/' + royaltyId,
            //            type: 'GET',
            //            contentType: "application/json",
            //            headers: {
            //                "Authorization": "Bearer " + token
            //            },
            //        }).done(function (result) {
            //            if (result.status.success) {
            //                Swal.fire("Success!", "Fetching Data successfully.", "success");
            //                $("#contract-product-specification-grid").dxDataGrid("getDataSource").reload();
            //            } else {
            //                Swal.fire("Error !", result.message, "error");
            //            }
            //        }).fail(function (jqXHR, textStatus, errorThrown) {
            //            Swal.fire("Failed !", textStatus, "error");
            //        });
            //    }
            //})
        
            //e.element.find('.dx-datagrid-header-panel').append($customButton)
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
    //* ===== Quantity & Quality Grid


});