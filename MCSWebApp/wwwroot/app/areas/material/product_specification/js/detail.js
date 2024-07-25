$(document).ready(function () {

    var token = $.cookie("Token");
    var basePath = "/Material/ProductSpecification";
    var apiPath = "/api" + basePath;

    $("#Product").select2({
        ajax:
        {
            url: "/api/Material/Product/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#product_id').val(data.id);
        $('#product_name').val(data.text);
    });

    $("#Analyte").select2({
        ajax:
        {
            url: "/api/Quality/Analyte/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#analyte_id').val(data.id);
        $('#analyte_name').val(data.text);
    });

    $("#UOM").select2({
        ajax:
        {
            url: "/api/General/UOM/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#uom_id').val(data.id);
        $('#uom_name').val(data.text);
    });

    $('#Save').on('click', function () {
        var record = $('#form-main').serializeToJSON();

        $.ajax({
            url: apiPath + '/save',
            type: 'POST',
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            },
            data: JSON.stringify(record)
        }).done(function (result) {
            if (result !== null) {
                if (result.success) {
                    Swal.fire("Success !", "Data is saved", "success")
                        .then(function () {
                            // Reload page
                            var url = basePath + "/Detail/" + encodeURIComponent(result.data.id);
                            window.location = url;
                        });
                }
                else {
                    Swal.fire("Error !", result.message, "error");
                }
            }
            else {
                Swal.fire("Error !", 'Server sent empty response', "error");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Swal.fire("Failed !", textStatus, "error");
        });
    });

    $('#Delete').on('click', function () {
        var recordId = $('#id').val();        

        Swal.fire(
            {
                title: "Are you sure?",
                text: "You won't be able to revert this!",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: "Yes, delete it!"
            }).then(function (result) {
                if (result.value) {
                    $.ajax({
                        url: apiPath + '/delete/' + encodeURIComponent(recordId),
                        type: 'GET',
                        headers: {
                            "Authorization": "Bearer " + token
                        }
                    }).done(function (result) {
                        if (result !== null) {
                            if (result.success) {
                                Swal.fire("Deleted !", "Record is successfully deleted.", "success")
                                    .then(function () {
                                        var url = basePath + "/Index";
                                        window.location = url;
                                    });
                            }
                            else {
                                Swal.fire("Error !", result.message, "error");
                            }
                        }
                        else {
                            Swal.fire("Error !", 'Server sent empty response', "error");
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        Swal.fire("Failed !", textStatus, "error");
                    });
                }
            });
    });

    var loadRecord = function (recordId) {
        if (recordId !== null && recordId !== '') {

            $.ajax({
                url: apiPath + '/getbyid/' + encodeURIComponent(recordId),
                type: 'GET',
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result !== null) {
                    if (result.success && result.data !== null) {
                        $('#id').val(result.data.id);
                        $('#product_id').val(result.data.product_id);
                        $('#product_name').val(result.data.product_name);
                        $('#analyte_id').val(result.data.analyte_id);
                        $('#analyte_name').val(result.data.analyte_name);
                        $('#uom_id').val(result.data.uom_id);
                        $('#uom_name').val(result.data.uom_name);
                        $('#target_value').val(result.data.target_value);
                        $('#minimum_value').val(result.data.minimum_value);
                        $('#maximum_value').val(result.data.maximum_value);
                        $('#applicable_date').val(result.data.applicable_date);

                        $('#Product').append(new Option(
                            $('#product_name').val(),
                            $('#product_id').val(), true, true)
                        ).trigger('change');

                        $('#Analyte').append(new Option(
                            $('#analyte_name').val(),
                            $('#analyte_id').val(), true, true)
                        ).trigger('change');

                        $('#UOM').append(new Option(
                            $('#uom_name').val(),
                            $('#uom_id').val(), true, true)
                        ).trigger('change');
                    }
                    else if (result.message !== null) {
                        Swal.fire("Error !", result.status.message, "error");
                    }
                    else {
                        Swal.fire("Error !", 'Server sent empty response', "error");
                    }
                }
                else {
                    Swal.fire("Error !", 'Server sent empty response', "error");
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                Swal.fire("Failed !", textStatus, "error");
            });
        }
    }

    // Form OnLoad
    var recordId = $('#id').val();
    if (!!recordId) {
        loadRecord(recordId);
    }
});