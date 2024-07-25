$(function () {

    var token = $.cookie("Token");
    var entityName = "Payment";
    var royaltyId = document.querySelector("[name=royalty_id]").value;
    var royaltyValue = 0;
    var bmnValue = 0;
    var phtValue = 0;
    var dhpbFinalValue = 0;

    $('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab
        if (target == "#royalty-payment-container") {
            getRoyaltyHeader();
        }
    });

    const getRoyaltyHeader = () => {
        $.ajax({
            type: "GET",
            url: "/api/Sales/Royalty/Payment/Detail/" + encodeURIComponent(royaltyId),
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let royaltyHeaderData = response;
                    paymentHeaderForm.option("formData", royaltyHeaderData);
                    paymentBottomForm.option("formData", royaltyHeaderData);
                }
            }
        })
    }

    let paymentHeaderForm = $("#payment-header-form").dxForm({
        formData: {
            royalty_id: royaltyId,
            volume_loading: "",
            bl_date: "",
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
                dataField: "royalty_value",
                label: {
                    text: "Royalty Value"
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
                dataField: "pht_value",
                label: {
                    text: "PHT Value"
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
                dataField: "bmn_value",
                label: {
                    text: "BMN Value"
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
                dataField: "dhpb_final_value",
                label: {
                    text: "DHPB Final"
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
                dataField: "billing_code",
                label: {
                    text: "Billing Code"
                }
            },
            {
                dataField: "ntb_ntp",
                label: {
                    text: "NTB/NTP"
                }
            },
            {
                dataField: "ntpn",
                label: {
                    text: "NTPN"
                }
            },
            {
                dataField: "payment_date",
                dataType: "date",
                editorType: "dxDateBox",
                label: {
                    text: "Payment Date"
                },
            },
        ],
        onInitialized: function (e) {
            // Get data if has royaltyId
            //if (royaltyId) {
            //    getRoyaltyHeader()
            //}
        },
        onFieldDataChanged: function (data) {
            if (data.dataField == "royalty_value") {
                royaltyValue = data.value.toFixed(2);
            }
            if (data.dataField == "pht_value") {
                phtValue = data.value.toFixed(2);
            }
            if (data.dataField == "bmn_value") {
                bmnValue = data.value.toFixed(2);
            }
            if (data.dataField == "dhpb_final_value") {
                dhpbFinalValue = data.value.toFixed(2);
            }
        }
    }).dxForm("instance");


    let paymentBottomForm = $("#payment-bottom-form").dxForm({
        formData: {
            fob_price: ""
        },
        colCount: 2,
        items: [
            {
                dataField: "royalty_paid_off",
                label: {
                    text: "Royalty Paid Off"
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
                dataField: "royalty_outstanding",
                label: {
                    text: "Royalty Outstanding"
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
                dataField: "bmn_paid_off",
                label: {
                    text: "BMN Paid Off"
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
                dataField: "bmn_outstanding",
                label: {
                    text: "BMN Outstanding"
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
                dataField: "pht_paid_off",
                label: {
                    text: "PHT Paid Off"
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
                dataField: "pht_outstanding",
                label: {
                    text: "PHT Outstanding"
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
                dataField: "dhpb_paid_off",
                label: {
                    text: "DHPB Paid Off"
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
                dataField: "dhpb_outstanding",
                label: {
                    text: "DHPB Outstanding"
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
            if (data.dataField == "royalty_paid_off") {
                var royaltyPaidOff = data.value.toFixed(2);
                var royaltyOutstanding = royaltyValue - royaltyPaidOff;
                this.updateData("royalty_outstanding", royaltyOutstanding);
            }
            if (data.dataField == "bmn_paid_off") {
                var bmnPaidOff = data.value.toFixed(2);
                var bmnOutstanding = bmnValue - bmnPaidOff;
                this.updateData("bmn_outstanding", bmnOutstanding);
            }
            if (data.dataField == "pht_paid_off") {
                var phtPaidOff = data.value.toFixed(2);
                var phtOutstanding = phtValue - phtPaidOff;
                this.updateData("pht_outstanding", phtOutstanding);
            }
            if (data.dataField == "dhpb_paid_off") {
                var dhpbPaidOff = data.value.toFixed(2);
                var dhpbOutstanding = dhpbFinalValue - dhpbPaidOff;
                this.updateData("dhpb_outstanding", dhpbOutstanding);
            }

        }
    }).dxForm("instance");

    $("#btnSavePayment").click(function () {
        let formData = new FormData();
        formData.append("key", royaltyId);

        let data = paymentHeaderForm.option("formData");
        formData.append("values", JSON.stringify(data));

        //data = paymentBottomForm.option("formData");
        //formData.append("values2", JSON.stringify(data));

        //data = hbaForm.option("formData");
        //formData.append("values3", JSON.stringify(data));

        $.ajax({
            type: "POST",
            url: "/api/Sales/Royalty/Payment/SaveData",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (response) {
                if (response) {
                    let salesContractProductHeaderData = response;

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
                    }).appendTo("#payment-header-form").dxPopup("instance");

                    successPopup.show();

                    //if (!salesContractProductId) {
                    //    // Update sales contract product specification grid
                    //    //updateSalesContractProductSpecification(salesContractProductHeaderData);
                    //}
                }
            }
        })

    });

});