$(document).ready(function () {

    var token = $.cookie("Token");
    var basePath = "/Location/BusinessArea";
    var apiPath = "/api" + basePath;

    $("#ParentBusinessArea").select2({
        ajax:
        {
            url: "/api/Location/BusinessArea/select2",
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
        $('#parent_business_area_id').val(data.id);
        $('#parent_business_area_name').val(data.text);
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
                        $('#business_area_name').val(result.data.business_area_name);                                        
                        $('#parent_business_area_id').val(result.data.parent_business_area_id);
                        $('#parent_business_area_name').val(result.data.parent_business_area_name);

                        $('#ParentBusinessArea').append(new Option(
                            $('#parent_business_area_name').val(),
                            $('#parent_business_area_id').val(), true, true)
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