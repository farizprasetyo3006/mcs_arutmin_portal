$(document).ready(function () {
    var token = $.cookie("Token");
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 100,
        "timeOut": 5000,
        "extendedTimeOut": 1000,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    $('#Save').on('click', function () {
        $('#email_content').val(editorInstance.option("value"));

        var record = $('#form-main').serializeToJSON();

        $.ajax({
            url: '/api/SystemAdministration/EmailNotification/Save',
            type: 'POST',
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            },
            data: JSON.stringify(record)
        }).done(function (result) {
            if (result !== null) {
                // Reload page
                var url = "/SystemAdministration/EmailNotification/Index";
                window.location = url;
            }
            else {
                Swal.fire("Error !", result.message, "error");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Swal.fire("Failed !", textStatus, "error");
        });
    });

    $('#Send').on('click', function () {
        $('#email_content').val(editorInstance.option("value"));

        var record = $('#form-main').serializeToJSON();

        $.ajax({
            url: '/api/SystemAdministration/EmailNotification/Send',
            type: 'POST',
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            },
            data: JSON.stringify(record)
        }).done(function (result) {
            if (result !== null) {
                // Reload page
                var url = "/SystemAdministration/EmailNotification/Index";
                window.location = url;
            }
            else {
                Swal.fire("Error !", result.message, "error");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Swal.fire("Failed !", textStatus, "error");
        });
    });


    var loadRecord = function (recordId) {
        ////console.log('loading user = ' + recordId);
        if (recordId !== null && recordId !== '') {

            $.ajax({
                url: '/api/SystemAdministration/EmailNotification/Detail/' + encodeURIComponent(recordId),
                type: 'GET',
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result !== null) {
                    $('#id').val(result.id);
                    $('#email_subject').val(result.email_subject);
                    $('#recipients').val(result.recipients);
                    $('#delivery_schedule').val(tanggal(result.delivery_schedule));
                    $('#table_name').val(result.table_name);
                    $('#fields').val(result.fields);
                    $('#criteria').val(result.criteria);
                    $('#email_code').val(result.email_code);
                    $('#attachment_file').val(result.attachment_file);

                    $("#date-box").dxDateBox({
                        type: "datetime",
                        displayFormat: 'dd MMM yyyy HH:mm',
                        value: result.delivery_schedule,
                        onValueChanged: function(e) {
                            $('#delivery_schedule').val(tanggal(e.value));
                        }
                    });

                    editorInstance.option("value", result.email_content);
                }
                else Swal.fire("Error !", 'Server sent empty response', "error");

            }).fail(function (jqXHR, textStatus, errorThrown) {
                Swal.fire("Failed !", textStatus, "error");
            });
        }
    }

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
                        url: '/api/SystemAdministration/EmailNotification/DeleteById/' + encodeURIComponent(recordId),
                        type: 'DELETE',
                        headers: {
                            "Authorization": "Bearer " + token
                        }
                    }).done(function (result) {
                        if (result !== null) {
                            Swal.fire("Deleted !", "Record is successfully deleted.", "success")
                                .then(function () {
                                    var url = "/SystemAdministration/EmailNotification/Index";
                                    window.location = url;
                                });
                        }
                        else {
                            Swal.fire("Error !", result.message, "error");
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        Swal.fire("Failed !", textStatus, "error");
                    });
                }
            });
    });

    var editorInstance = $("#html-editor").dxHtmlEditor({
        height: 500,
        toolbar: {
            items: [
                "undo", "redo", "separator",
                {
                    name: "size",
                    acceptedValues: ["8pt", "10pt", "12pt", "14pt", "18pt", "24pt", "36pt"]
                },
                {
                    name: "font",
                    acceptedValues: ["Arial", "Georgia", "Tahoma", "Times New Roman", "Verdana"]
                },
                "separator", "bold", "italic", "strike", "underline", "separator", "alignLeft", "alignCenter", "alignRight",
                "alignJustify", "separator", "orderedList", "bulletList", "separator",
                {
                    name: "header",
                    acceptedValues: [false, 1, 2, 3, 4, 5]
                },
                "separator", "color", "background", "separator", "link", "image", "separator", "clear", "codeBlock", "blockquote", "separator",
                "insertTable", "deleteTable", "insertRowAbove", "insertRowBelow", "deleteRow",
                "insertColumnLeft", "insertColumnRight", "deleteColumn"
            ],
            multiline: true

        },
        onValueChanged: function (e) {
            $(".value-content").text(e.component.option("value"));
        }
    }).dxHtmlEditor("instance");

    // Form OnLoad
    var recordId = $('#id').val();
    ////console.log('user id = ' + recordId);
    if (!!recordId) {
        loadRecord(recordId);
    } else {
        $('#delivery_schedule').val(tanggal(Date.now()));

        $("#date-box").dxDateBox({
            type: "datetime",
            displayFormat: 'dd MMM yyyy HH:mm',
            value: Date.now(),
            onValueChanged: function (e) {
                $('#delivery_schedule').val(tanggal(e.value));
            }
        });

    }

});

function tanggal(x) {
    theDate = new Date(x);
    formatted_date = theDate.getFullYear() + "-" + (theDate.getMonth() + 1) + "-" + theDate.getDate() + " "
        + theDate.getHours() + ":" + theDate.getMinutes() + ":" + theDate.getSeconds();
    return formatted_date;
}