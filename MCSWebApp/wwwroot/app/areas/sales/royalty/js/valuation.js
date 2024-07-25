$(function () {
    var token = $.cookie("Token");
    var entityName = "Valuation";
    var royaltyId = document.querySelector("[name=royalty_id]").value;
    var DHPB = 0, BMN = 0, Royalty = 0, PHT = 0;

    $('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        if (target == "#valuation-container") {
            getRoyaltyHeader();
        }
    });

    const getRoyaltyHeader = () => {
        $.ajax({
            type: "GET",
            url: "/api/Sales/Royalty/Valuation/Detail1/" + encodeURIComponent(royaltyId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {

                if (response) {
                    let royaltyHeaderData = response;
                    valuationHeaderForm.option("formData", royaltyHeaderData);
                    valuationMiddleForm.option("formData", royaltyHeaderData);
                    valuationBottomForm.option("formData", royaltyHeaderData);
                }
            }
        })
    }

    let valuationHeaderForm = $("#valuation-header-form").dxForm({
        formData: {
            base_price_royalty: "",
            volume_loading: "",
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
                dataField: "base_price_royalty",
                label: {
                    text: "Base Price Royalty"
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
        ],
        onInitialized: function (e) {
            //if (royaltyId) {
            //    getRoyaltyHeader()
            //}
        },
        onFieldDataChanged: function (data) {
            if (data.dataField == "base_price_royalty" || data.dataField == "volume_loading") {
                let formData1 = valuationMiddleForm.option("formData");
                let formData2 = valuationBottomForm.option("formData");

                valuationBottomForm.updateData("royalty_calc", formData1.base_price_royalty * formData1.volume_loading * formData1.royalty);
                valuationBottomForm.updateData("bmn_calc", formData1.base_price_royalty * formData1.volume_loading * formData1.bmn);
                valuationBottomForm.updateData("pht_calc", formData1.base_price_royalty * formData1.volume_loading * formData1.pht);
            }
        }
    }).dxForm("instance");


    let valuationMiddleForm = $("#valuation-middle-form").dxForm({
        formData: {
            dhpb: "",
            bmn: "",
            royalty: "",
            pht: "",
        },
        colCount: 2,
        items: [
            {
                dataField: "dhpb",
                label: {
                    text: "DHPB"
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
                dataField: "bmn",
                label: {
                    text: "BMN"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4
                    }
                },
            },
            {
                dataField: "royalty",
                label: {
                    text: "Royalty"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4
                    }
                },
            },
            {
                dataField: "pht",
                label: {
                    text: "PHT"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4
                    },
                    readOnly: true
                },
            },
        ],
        onInitialized: function (e) {
        },
        onFieldDataChanged: function (data) {
            if (data.dataField == "dhpb") {
                //DHPB = data.value;
                //PHT = DHPB - Royalty - BMN;
                //this.updateData("pht", PHT);

                let formData = valuationMiddleForm.option("formData");
                valuationMiddleForm.updateData("pht", formData.dhpb - formData.royalty - formData.bmn);
            }

            if (data.dataField == "royalty") {
                let formData = valuationMiddleForm.option("formData");
                valuationMiddleForm.updateData("pht", formData.dhpb - formData.royalty - formData.bmn);
                valuationBottomForm.updateData("royalty_calc", formData.base_price_royalty * formData.volume_loading * formData.royalty);
            }

            if (data.dataField == "bmn") {
                let formData1 = valuationMiddleForm.option("formData");
                let formData2 = valuationBottomForm.option("formData");
                valuationMiddleForm.updateData("pht", formData1.dhpb - formData1.royalty - formData1.bmn);
                valuationBottomForm.updateData("bmn_calc", formData1.base_price_royalty * formData1.volume_loading * formData1.bmn);
            }

            if (data.dataField == "pht") {
                let formData1 = valuationMiddleForm.option("formData");
                let formData2 = valuationBottomForm.option("formData");
                valuationBottomForm.updateData("pht_calc", formData1.base_price_royalty * formData1.volume_loading * formData1.pht);
            }
        }
    }).dxForm("instance");


    var valuationBottomForm = $("#valuation-bottom-form").dxForm({
        formData: {
            royalty_calc: "",
            royalty_awal: "",
            royalty_value: "",
            bmn_calc: "",
            bmn_awal: "",
            bmn_value: "",
            pht_calc: "",
            pht_awal: "",
            pht_value: "",
            dhpb_final_calc: "",
            dhpb_final_awal: "",
            dhpb_final_value: ""
        },
        colCount: 3,
        items: [
            {
                dataField: "royalty_calc",
                label: {
                    text: "Royalty Calc"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: false
                },
            },

            {
                dataField: "royalty_awal",
                label: {
                    text: "Royalty Awal"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                },
            },
            {
                dataField: "royalty_value",
                label: {
                    text: "Royalty"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: true
                },
            },
            {
                dataField: "bmn_calc",
                label: {
                    text: "BMN Calc"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: false
                },
            },
            {
                dataField: "bmn_awal",
                label: {
                    text: "BMN Awal"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                },
            },
            {
                dataField: "bmn_value",
                label: {
                    text: "BMN"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: true
                },
            },
            {
                dataField: "pht_calc",
                label: {
                    text: "PHT Calc"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: false
                },
            },
            {
                dataField: "pht_awal",
                label: {
                    text: "PHT Awal"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                },
            },
            {
                dataField: "pht_value",
                label: {
                    text: "PHT"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: true
                },
            },
            {
                dataField: "dhpb_final_calc",
                label: {
                    text: "DHPB Final Calc"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: false
                },
            },
            {
                dataField: "dhpb_final_awal",
                label: {
                    text: "DHPB Final Awal"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                },
            },
            {
                dataField: "dhpb_final_value",
                label: {
                    text: "DHPB Final"
                },
                editorType: "dxNumberBox",
                editorOptions: {
                    step: 0,
                    format: {
                        type: "fixedPoint",
                        precision: 4,
                    },
                    readOnly: true
                },
            },
        ],
        onInitialized: function (e) {
        },
        onFieldDataChanged: function (data) {
            //    if (data.dataField == "royalty_value" || data.dataField == "bmn_value" || data.dataField == "pht_value") {
            //        let formData = valuationMiddleForm.option("formData");
            //        this.updateData("dhpb_final_calc", formData.royalty_value + formData.bmn_value + formData.pht_value);
            //    }
            if (data.dataField == "royalty_calc" || data.dataField == "bmn_calc" || data.dataField == "pht_calc") {
                let formData = valuationMiddleForm.option("formData");
                this.updateData("dhpb_final_calc", formData.royalty_calc + formData.bmn_calc + formData.pht_calc);
            }

            if (data.dataField == "royalty_calc" || data.dataField == "royalty_awal") {
                let formData = valuationMiddleForm.option("formData");
                let royaltyValue = formData.royalty_calc - formData.royalty_awal;

                this.updateData("royalty_value", royaltyValue);
            }

            if (data.dataField == "bmn_calc" || data.dataField == "bmn_awal") {
                let formData = valuationMiddleForm.option("formData");
                let bmnValue = formData.bmn_calc - formData.bmn_awal;

                this.updateData("bmn_value", bmnValue);
            }

            if (data.dataField == "pht_calc" || data.dataField == "pht_awal") {
                let formData = valuationMiddleForm.option("formData");
                let phtValue = formData.pht_calc - formData.pht_awal;

                this.updateData("pht_value", phtValue);
            }

            if (data.dataField == "dhpb_final_calc" || data.dataField == "dhpb_final_awal") {
                let formData = valuationMiddleForm.option("formData");
                let dhpbFinalValue = formData.dhpb_final_calc - formData.dhpb_final_awal;

                this.updateData("dhpb_final_value", dhpbFinalValue);
            }
        }
    }).dxForm("instance");


    $("#btnRecalcValuation").click(function () {
        let formData = new FormData();
        formData.append("key", royaltyId);

        let data = valuationHeaderForm.option("formData");
        formData.append("values", JSON.stringify(data));

        $.ajax({
            type: "POST",
            url: "/api/Sales/Royalty/Valuation/Recalculate",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let royaltyValuationData = response;

                    valuationMiddleForm.option("formData", royaltyValuationData);
                    valuationBottomForm.option("formData", royaltyValuationData);
                }
            }
        });
    });

    $("#btnSaveValuation").click(function () {
        let formData = new FormData();
        formData.append("key", royaltyId);

        let data = valuationHeaderForm.option("formData");
        formData.append("values", JSON.stringify(data));

        let data2 = valuationMiddleForm.option("formData");
        formData.append("values2", JSON.stringify(data2));

        let data3 = valuationBottomForm.option("formData");
        formData.append("values3", JSON.stringify(data3));

        $.ajax({
            type: "POST",
            url: "/api/Sales/Royalty/Valuation/SaveData",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let royaltyValuationData = response;

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
                    }).appendTo("#valuation-header-form").dxPopup("instance");

                    successPopup.show();
                }
            }
        })
    });

});