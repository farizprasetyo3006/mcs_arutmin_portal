$(document).ready(function () {

    var basePath = "/Authentication/Login";
    var apiPath = "/api" + basePath;

    $('#btn-reset-password').on('click', function () {
        var id = $('#Id').val();
        var newPassword = $('#NewPassword').val();
        var confirmPassword = $('#ConfirmPassword').val();

        if (newPassword !== null && newPassword !== '' && newPassword === confirmPassword) {
            $.ajax({
                url: apiPath + '/ResetPassword?Id=' + encodeURIComponent(id) + '&NewPassword=' + encodeURIComponent(newPassword),
            }).done(function (result) {
                if (result !== null) {
                    if (result.success) {
                        Swal.fire("Success !", "New password is set", "success")
                            .then(function () {
                                window.location = basePath;
                            });
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
        }
    });
});