$(function () {
    var token = $.cookie("Token");
    var entityName = "Pricing";
    var royaltyId = document.querySelector("[name=royalty_id]").value;
    var fobPrice = 0;
    var freightCost = 0;
    var totalSellingPrice = 0;

    $('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        if (target == "#pricing-container") {
            getRoyaltyHeader();
        }
    });

    const getRoyaltyHeader = () => {
        $.ajax({
            type: "GET",
            url: "/api/Sales/Royalty/Pricing/Detail/" + encodeURIComponent(royaltyId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {

                if (response) {
                    let royaltyHeaderData = response;
                    pricingHeaderForm.option("formData", royaltyHeaderData);
                    sellingPriceForm.option("formData", royaltyHeaderData);
                    hbaForm.option("formData", royaltyHeaderData);
                }
            }
        })
    }

    let pricingHeaderForm = $("#pricing-form").dxForm({
        formData: {
            royalty_id: royaltyId,
            volume_loading: "",
            bl_date: "",
        },
        colCount: 2,
        items: [
            {
                dataField: "despatch_order_number",
                label: {
                    text: "DO Number"
                },
                editorOptions: {
                    readOnly: true
                },
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
                    },
                    readOnly: true
                },
            },
            {
                dataField: "bl_date",
                dataType: "date",
                editorType: "dxDateBox",
                label: {
                    text: "BL Date"
                },
                editorOptions: {
                    readOnly: true
                },
            },
            {
                dataField: "destination_id",
                label: {
                    text: "Destination"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/General/MasterList/MasterListIdLookupByItemGroup?itemGroup=royalty-destination",
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
                    readOnly: true
                },
            },
        ],
        onInitialized: function (e) {
            //Get royalty header if has royaltyId
            //if (royaltyId) {
            //    getRoyaltyHeader()
            //}
        },
        onFieldDataChanged: function (data) {
        }
    }).dxForm("instance");


    let sellingPriceForm = $("#selling-price").dxForm({
        formData: {
            fob_price: "",
            freight_cost: "",
            total_selling_price: "",
            total_amount: "",
        },
        colCount: 1,
        items: [
            {
                dataField: "fob_price",
                label: {
                    text: "FOB Price"
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
                dataField: "total_selling_price",
                label: {
                    text: "Total Selling Price"
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
                dataField: "total_amount",
                label: {
                    text: "Total Amount"
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
            if (data.dataField == "fob_price") {
                fobPrice = data.value;
                totalSellingPrice = fobPrice + freightCost;

                this.updateData("total_selling_price", totalSellingPrice);
            };
            if (data.dataField == "freight_cost") {
                freightCost = data.value;
                totalSellingPrice = fobPrice + freightCost;

                this.updateData("total_selling_price", totalSellingPrice);
            };
        }
    }).dxForm("instance");


    let hbaForm = $("#hba-form").dxForm({
        formData: {
            hba_0: "",
            hba_type: "",
            hba_value: "",
            formula: "",
            hpb_vessel: "",
            hpb_barge: "",
        },
        colCount: 1,
        items: [
            {
                //id: "hbax",
                dataField: "hba_0",
                label: {
                    text: "HBA 0"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 2

                    },
                    readOnly: true
                },
            },
            {
                dataField: "hba_type",
                label: {
                    text: "HBA Type"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Sales/Royalty/Pricing/HBALookup",
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
                dataField: "hba_value",
                label: {
                    text: "HBA Value"
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
                dataField: "formula",
                label: {
                    text: "Formula"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        //loadUrl: "/api/Sales/SalesCharge/SalesChargeIdLookup",
                        loadUrl: "/api/Sales/Royalty/Royalty/SalesChargeIdLookup",
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
                dataField: "hpb_vessel",
                label: {
                    text: "HPB Vessel"
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
                dataField: "hpb_barge",
                label: {
                    text: "HPB Barge"
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
        onFieldDataChanged: async function (data) {
            if (data.dataField == "formula") {
                let salesChargeId = data.value;
                let hpbVessel = 0;

                let formData = hbaForm.option("formData");
                let hbaValue = formData.hba_value;

                await $.ajax({
                    type: "GET",
                    url: "/api/Sales/Royalty/Royalty/CalculateFormulaById/" + encodeURIComponent(royaltyId) + "/" + encodeURIComponent(salesChargeId)
                        + "/" + encodeURIComponent(hbaValue),
                    contentType: "application/json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    },
                    success: function (response) {
                        if (response.status == "OK") {
                            hpbVessel = response.value;
                        }
                    }
                });
                this.updateData("hpb_vessel", hpbVessel);

                var totalJoinCost = 0;
                await $.ajax({
                    type: "GET",
                    url: "/api/Sales/Royalty/Cost/Detail/" + encodeURIComponent(royaltyId),
                    contentType: "application/json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    },
                    success: function (response) {
                        if (response != null) {
                            totalJoinCost = response.total_join_cost ?? 0;
                        }
                    }
                });

                if (totalJoinCost == null) totalJoinCost = 0;
                let hpbBarge = hpbVessel - totalJoinCost;
                this.updateData("hpb_barge", hpbBarge);
            };
            if (data.dataField == "hba_type") {
                hbaType = data.value;
                let hbaValue = 0;
                await $.ajax({
                    type: "GET",
                    url: "/api/Sales/Royalty/Pricing/HBACalculate/" + encodeURIComponent(royaltyId) + "/" + encodeURIComponent(hbaType),
                    contentType: "application/json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    },
                    success: function (response) {
                        if (response) {
                            hbaValue = response;
                        }
                    }
                });
                this.updateData("hba_value", hbaValue);
            };
        }
    }).dxForm("instance");

    $("#btnSavePricing").click(function () {
        let formData = new FormData();
        formData.append("key", royaltyId);

        let data = pricingHeaderForm.option("formData");
        formData.append("values", JSON.stringify(data));

        $.ajax({
            type: "POST",
            url: "/api/Sales/Royalty/Pricing/SaveData",
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
                    }).appendTo("#pricing-form").dxPopup("instance");

                    successPopup.show();
                }

            }
        })
    });

    $("#btnRecalcPricing").click(function () {
        //hbaForm.option("formData", { hba_0: 321 } );

        $.ajax({
            type: "GET",
            url: "/api/Sales/Royalty/Pricing/RecalculatePricing/" + encodeURIComponent(royaltyId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response != null) {
                    let pricingData = response;
                    hbaForm.option("formData", pricingData);
                }
            }
        });
    });

});