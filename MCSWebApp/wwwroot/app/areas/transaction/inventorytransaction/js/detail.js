$(document).ready(function () {

    var token = $.cookie("Token");
    var basePath = "/Transaction/InventoryTransaction";
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

    $("#SourceLocation").select2({
        ajax:
        {
            url: "/api/Location/StockLocation/select2source",
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
        $('#source_location_id').val(data.id);
        $('#source_location_name').val(data.text);
    });

    $("#DestinationLocation").select2({
        ajax:
        {
            url: "/api/Location/StockLocation/select2destination",
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
        $('#destination_location_id').val(data.id);
        $('#destination_location_name').val(data.text);
    });

    $("#AccountingPeriod").select2({
        ajax:
        {
            url: "/api/Organisation/AccountingPeriod/select2",
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
        $('#accounting_period_id').val(data.id);
        $('#accounting_period_name').val(data.text);
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
                        $('#transaction_datetime').val(result.data.transaction_datetime);
                        $('#transaction_number').val(result.data.transaction_number);
                        $('#quantity').val(result.data.quantity);
                        $('#product_id').val(result.data.product_id);
                        $('#product_name').val(result.data.product_name);                        
                        $('#uom_id').val(result.data.uom_id);
                        $('#uom_name').val(result.data.uom_name);
                        $('#source_location_id').val(result.data.source_location_id);
                        $('#source_location_name').val(result.data.source_location_name);
                        $('#destination_location_id').val(result.data.destination_location_id);
                        $('#destination_location_name').val(result.data.destination_location_name);
                        $('#accounting_period_id').val(result.data.accounting_period_id);
                        $('#accounting_period_name').val(result.data.accounting_period_name);

                        $('#Product').append(new Option(
                            $('#product_name').val(),
                            $('#product_id').val(), true, true)
                        ).trigger('change');

                        $('#UOM').append(new Option(
                            $('#uom_name').val(),
                            $('#uom_id').val(), true, true)
                        ).trigger('change');

                        $('#SourceLocation').append(new Option(
                            $('#source_location_name').val(),
                            $('#source_location_id').val(), true, true)
                        ).trigger('change');

                        $('#DestinationLocation').append(new Option(
                            $('#destination_location_name').val(),
                            $('#destination_location_id').val(), true, true)
                        ).trigger('change');

                        $('#AccountingPeriod').append(new Option(
                            $('#accounting_period_name').val(),
                            $('#accounting_period_id').val(), true, true)
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