$(document).ready(function () {
    'use strict';
    var token = $.cookie("Token");
    var $recordId = $("input[name=recordId]");
    //var $recordId = $('input[id=recordId]');
    //var $form = $("#formAfe")

    var pageFunction = function () {
        var getDesDemContract = function (){
            $.ajax({
                type: "GET",
                url: "/api/DespatchDemurrage/Invoice/DataDetail?Id=" + encodeURIComponent($recordId.val()),
                contentType: "application/json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                },
                success: function (r) {
                    //console.log(r);
                    if (r) {
                        let record = r.data[0];
                        $("#despatch-order-number").text(record.despatch_order_number);
                        $("#invoice-number").text(record.invoice_number);
                        $("#invoice-type").text(record.contract_type);
                        $("#vessel-name").text(record.vessel_name);
                        $("#invoice-date").text(record.invoice_date);
                        $("#invoice-currency").text(record.currency_code);
                        $("#contractor_name").text(record.contractor_name);
                       

                        if (r.data.length > 0) {
                            var html = "";
                            var number = 1;
                            var totalAmount = 0;
                            var total = 0;
                            $.each(r.data, function (key, value) {
                                total = value.amount;
                                html += `<tr>
                                            <td class="text-center fw-700">`+ number +`</td>
                                            <td class="text-left strong">` + value.contract_name +  `</td>
                                            <td class="text-left">` + value.sof_number + `</td>
                                            <td class="text-left">` + thousandSeparator(value.allowed_time) + `</td>
                                            <td class="text-left">` + thousandSeparator(value.actual_time) + `</td>
                                            <td class="text-left">` + thousandSeparator(value.rate) + `</td>
                                            <td class="text-right">` + thousandSeparator(total) + `</td>
                                        </tr>`;
                                number++;
                                totalAmount = total;
                            });

                            $("#total-amount").html(thousandSeparator(totalAmount));
                            $("#table-invoice > tbody").html("");
                            $("#table-invoice > tbody").append(html);
                        }

                        
                    }
                }
            })
        }

        var thousandSeparator = function(nStr) {
            nStr += '';
            var x = nStr.split('.');
            var x1 = x[0];
            var x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        }

        return {
            init: function () {
                getDesDemContract();
            }
        }
    }();


    $(document).ready(function () {
        pageFunction.init();
    });
});