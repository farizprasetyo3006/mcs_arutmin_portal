$(document).ready(function () {

    var token = $.cookie("Token");
    var basePath = "/Equipment/EquipmentType";
    var apiPath = "/api" + basePath;

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
                        $('#equipment_type_name').val(result.data.equipment_type_name);                        
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