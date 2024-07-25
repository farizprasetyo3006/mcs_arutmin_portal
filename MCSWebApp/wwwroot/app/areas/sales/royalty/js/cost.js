$(function () {
    var token = $.cookie("Token");
    var entityName = "RoyaltyCost";
    var royaltyId = document.querySelector("[name=royalty_id]").value;
    var royaltyCostData = null;
    var bargingCost = 0;
    var transhipmentCost = 0;
    var totalJoinCost = 0;

    const getRoyaltyHeader = () => {
        $.ajax({
            type: "GET",
            url: "/api/Sales/Royalty/Cost/Detail/" + encodeURIComponent(royaltyId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    royaltyCostData = response;
                    if (royaltyCostData.status_code == 'AKH') {
                        if (royaltyCostData.transhipment_cost == null)
                            royaltyCostData.transhipment_cost = 4;
                    }

                    costHeaderForm.option("formData", royaltyCostData);
                    costBottomForm.option("formData", royaltyCostData);
                }
            }
        })
    }

    let costHeaderForm = $("#cost-form").dxForm({
        formData: {
            despatch_order_id: "",
            barge_name: ""
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
                dataField: "barge_name",
                label: {
                    text: "Barge Name"
                },
                editorOptions: {
                    readOnly: true,
                }
            },
            {
                dataField: "barge_size",
                label: {
                    text: "Barge Size"
                },
                editorOptions: {
                    readOnly: true,
                }
            },
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


    let costBottomForm = $("#cost-bottom-form").dxForm({
        formData: {
            port_load_id: "",
            discharge_port_id: "",
            dist_barge_to_anchorage: "",
            barging_cost: "",
            freight_cost: "",
            transhipment_cost: "",
            total_join_cost: ""
        },
        colCount: 2,
        items: [
            {
                dataField: "port_load_id",
                label: {
                    text: "Port Load Out"
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
                    displayExpr: "text",
                },
            },
            {
                dataField: "discharge_port_id",
                label: {
                    text: "Discharge Port"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        //loadUrl: "/api/Planning/ShipmentPlan/ShipmentPlanIdLookup",
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
                dataField: "dist_barge_to_anchorage",
                label: {
                    text: "Dist. Barge to Anchorage"
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
                dataField: "barging_cost",
                label: {
                    text: "Barging Cost"
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
                dataField: "freight_cost",
                label: {
                    text: "Freight Cost"
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
                dataField: "transhipment_cost",
                label: {
                    text: "Transhipment Cost"
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

            },
            {
                dataField: "total_join_cost",
                label: {
                    text: "Total Join Cost"
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
        ],
        onInitialized: function (e) {
        },
        onFieldDataChanged: function (data) {
            if (data.dataField == "barging_cost") {
                bargingCost = data.value;
                totalJoinCost = bargingCost + transhipmentCost;

                this.updateData("total_join_cost", totalJoinCost);
            };

            if (data.dataField == "transhipment_cost") {
                transhipmentCost = data.value;
                totalJoinCost = bargingCost + transhipmentCost;

                this.updateData("total_join_cost", totalJoinCost);
            };

            if (data.dataField == "dist_barge_to_anchorage") {
                let distBarge = data.value;

                if (distBarge != null && royaltyCostData.status_code == 'AWL') {
                    if (royaltyCostData.barge_size < 270)
                        bargingCost = (0.0221 * distBarge) + 3.7406;
                    else if (royaltyCostData.barge_size >= 270 && royaltyCostData.barge_size <= 330)
                        bargingCost = (0.0184 * distBarge) + 3.1172;
                    else 
                        bargingCost = (0.0154 * distBarge) + 2.6022;

                    this.updateData("barging_cost", bargingCost);
                };
            };
        }
    }).dxForm("instance");

    $("#btnSaveCost").click(function () {
        let formData = new FormData();
        let data = costBottomForm.option("formData");

        formData.append("key", royaltyId);
        formData.append("values", JSON.stringify(data));

        $.ajax({
            type: "POST",
            url: "/api/Sales/Royalty/Cost/SaveData",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let royaltyPricingData = response;

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
                    }).appendTo("#cost-bottom-form").dxPopup("instance");

                    successPopup.show();
                }

            }
        })
    });

});