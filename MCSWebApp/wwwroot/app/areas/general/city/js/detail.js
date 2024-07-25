$(document).ready(function () {

    var token = $.cookie("Token");
    var basePath = "/General/City";
    var apiPath = "/api" + basePath;

    $("#Country").select2({
        ajax:
        {
            url: "/api/General/Country/select2",
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
        $('#country_id').val(data.id);
        $('#country_name').val(data.text);
    });

    $("#Province").select2({
        ajax:
        {
            url: "/api/General/Province/select2",
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page,
                    country_id: $('#country_id').val()
                };
            },
            cache: true
        },
        allowClear: true,
        minimumInputLength: 0
    }).on('select2:select', function (e) {
        var data = e.params.data;
        $('#province_id').val(data.id);
        $('#province_name').val(data.text);
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
                        $('#city_name').val(result.data.city_name);
                        $('#province_id').val(result.data.province_id);
                        $('#province_name').val(result.data.province_name);
                        $('#country_id').val(result.data.country_id);
                        $('#country_name').val(result.data.country_name);

                        $('#Country').append(new Option(
                            $('#country_name').val(),
                            $('#country_id').val(), true, true)
                        ).trigger('change');

                        $('#Province').append(new Option(
                            $('#province_name').val(),
                            $('#province_id').val(), true, true)
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