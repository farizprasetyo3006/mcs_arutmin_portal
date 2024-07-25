$(document).ready(function () {

    var basePath = "/Authentication/Login";
    var apiPath = "/api" + basePath;

    $('#btn-forgot-password').on('click', function () {
        var id = $('#lostaccount').val();
        //console.log("Lost account = " + id);

        $.ajax({
            url: apiPath + '/ForgotPassword?Id=' + encodeURIComponent(id),
        }).done(function (result) {
            if (result !== null) {
                if (result.success) {
                    Swal.fire("Success !", "An email has been sent", "success");
                }
                else {
                    Swal.fire("Error !", result.message ?? "Invalid account", "error");
                }
            }
            else {
                Swal.fire("Error !", 'Server sent empty response', "error");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Swal.fire("Failed !", textStatus, "error");
        });
    });
});