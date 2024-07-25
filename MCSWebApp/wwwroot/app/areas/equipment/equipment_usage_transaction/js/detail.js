$(document).ready(function () {

    var token = $.cookie("Token");
    var basePath = "/Equipment/EquipmentUsageTransactionDetail";
    var apiPath = "/api" + basePath;

    var recordId = $('input[name=equipment_usage_transaction_id]');
    var $btnSave = $("#btn-save");

    var request = {};
    var equipment_name = "";
    var event_category_name = "";

    function resultTemplate(obj) {
        var html = "";
        if (obj.data) {
            html = `
            <div>
                <span class="mb-0">Equipment Code: ` + obj.text + `<span> <br />
                <span class="mb-0" style="margin-bottom: 0px;">Event: ` + obj.data.event_category_name + `<span>
            <div/>
        `;
        }
        return html;
    };

    var lookup = function () {
        $('#equipment_usage_transaction_available').select2({
            ajax: {
                url: "/api/Equipment/EquipmentUsageTransaction/select2",
                dataType: 'json',
                headers: {
                    "Authorization": "Bearer " + token
                },
                dataType: 'json',
                delay: 250,
                cache: true,
                data: function (params) {
                    return {
                        q: params.term, // search term
                        page: params.page
                    };
                },
            },
            escapeMarkup: function (markup) {
                return markup;
            },
            templateResult: function (data) {
                return resultTemplate(data);
            },
            templateSelection: function (data) {
                return data.text;
            },
        }).on('select2:select', function (e) {
            var item = e.params.data;
            //console.log(item);

            equipment_name = item.equipment_name;
            event_category_name = item.event_category_name;
            request = {
                id: "",
                cn_unit_id: item.data.equipment_id,
                event_category_id: item.data.event_category_id,
                date: item.data.timesheet_date,
                duration: item.data.duration,
                equipment_usage_transaction_id: recordId.val(),
            }
        });
    }

    $btnSave.on("click", function () {
        save($btnSave);
    });

    var save = function (e) {
        if (request.id == "") {
            e.attr("disabled", "disabled");
            e.text("Loading");

            $.ajax({
                url: apiPath + '/Save',
                type: 'POST',
                contentType: "application/json",
                headers: {
                    "Authorization": "Bearer " + token
                },
                data: JSON.stringify(request)
            }).done(function (result) {
                //console.log(result);
                if (result.status.success) {
                    $('#equipment_usage_transaction_available').val(null).trigger('change');
                    getAll();
                    toastr["success"](result.message ?? "Success");
                } else {

                    if ((result.status.message).includes("duplicate key")) {
                        toastr["error"]("Equipment already exist");
                    } else {
                        toastr["error"](result.message ?? "Error");
                    }
                }

                e.removeAttr("disabled");
                e.text("Save");
            }).fail(function (jqXHR, textStatus, errorThrown) {
                ////console.log(textStatus);
                Swal.fire("Failed !", textStatus, "error");
                e.removeAttr("disabled");
                e.text("Save");
            });
        }
    }

    var getAll = function () {
        $.ajax({
            url: apiPath + '/GetByEquipmentUsageTransactionId?recordId=' + recordId.val(),
            type: 'GET',
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            },
        }).done(function (result) {
            //console.log(result);
            if (result.status.success) {
                if (result.data.length > 0) {
                    $("#tbl-list > tbody").html("");
                    $("#tbl-list > tbody").removeClass("d-none");
                    $("#tbl-list > tfoot").addClass("d-none");
                    $.each(result.data, function (index, value) {
                        var unit_name = value.equipment_code != null ? value.equipment_code : value.vehicle_name;
                        var html = `<tr>
                            <th scope="row">` + unit_name + `</th>
                            <td>` + moment(value.date).format('DD MMM YYYY') + `</td>
                            <td>` + value.duration.toFixed(2) + `</td>
                            <td>` + value.event_category_name + `</td>
                            <td>
                                <button type="button" class="btn btn-danger btn-sm btn-remove" data-id="` + value.id + `">Remove</button>
                            </td>
                        </tr>`;
                        $("#tbl-list > tbody").append(html);
                    });

                    $(".btn-remove").on("click", function () {
                        var id = $(this).data("id");
                        remove(id);
                    });
                } else {
                    $("#tbl-list > tbody").addClass("d-none");
                    $("#tbl-list > tfoot").removeClass("d-none");
                }
            }
            request = {};
            equipment_name = "";
            event_category_name = "";
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Swal.fire("Failed !", textStatus, "error");
        });
    }

    var remove = function (id) {
        Swal.fire({
            title: 'Are you sure want to delete?',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
        }).then((result) => {
            if (result) {
                $.ajax({
                    url: apiPath + '/DeleteById?recordId=' + id,
                    type: 'DELETE',
                    contentType: "application/json",
                    headers: {
                        "Authorization": "Bearer " + token
                    },
                    data: JSON.stringify(request)
                }).done(function (result) {
                    //console.log(result);
                    if (result.status.success) {
                        getAll();
                        toastr["success"](result.message ?? "Success");
                    } else {
                        toastr["error"](result.message ?? "Error");
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    ////console.log(textStatus);
                    Swal.fire("Failed !", textStatus, "error");
                });
            }
        })
    }

    var loadRecord = function (recordId) {
        //console.log(recordId);
        lookup();
        getAll();
    }

    // Form OnLoad
    if (!!recordId.val()) {
        loadRecord(recordId.val());
    }
});