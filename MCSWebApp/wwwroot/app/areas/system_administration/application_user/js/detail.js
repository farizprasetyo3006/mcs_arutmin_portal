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

    $("#ResetPassword").on("click", function () {
        let newPassword = $('#new_password').val();
        let confirmPassword = $('#confirm_password').val();
        if (newPassword !== null && newPassword !== '' && newPassword === confirmPassword) {
            $.ajax({
                url: '/api/SystemAdministration/ApplicationUser/ResetPassword?Id=' + encodeURIComponent(recordId)
                    + '&NewPassword=' + encodeURIComponent(newPassword),
                type: 'GET',
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result !== null) {
                    if (result.success) {
                        toastr["success"]("New password has been set");
                    }
                    else if (result.message !== null) {
                        toastr["error"](result.message);
                    }
                    else {
                        toastr["error"]("Server sent empty response");
                    }
                }
                else {
                    toastr["error"]("Server sent empty response");
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                toastr["error"]("Failed");
            });
        }
    });

    $("#DisableUser").on("click", function () {
        $.ajax({
            url: '/api/SystemAdministration/ApplicationUser/DisableUser?Id=' + encodeURIComponent(recordId),
            type: 'GET',
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result !== null) {
                if (result.success) {
                    toastr["success"]("User is disabled");
                }
                else if (result.message !== null) {
                    toastr["error"](result.message);
                }
                else {
                    toastr["error"]("Server sent empty response");
                }
            }
            else {
                toastr["error"]("Server sent empty response");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            toastr["error"]("Failed");
        });
    });

    $("#EnableUser").on("click", function () {
        $.ajax({
            url: '/api/SystemAdministration/ApplicationUser/EnableUser?Id=' + encodeURIComponent(recordId),
            type: 'GET',
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done(function (result) {
            if (result !== null) {
                if (result.success) {
                    toastr["success"]("User is enabled");
                }
                else if (result.message !== null) {
                    toastr["error"](result.message);
                }
                else {
                    toastr["error"]("Server sent empty response");
                }
            }
            else {
                toastr["error"]("Server sent empty response");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            toastr["error"]("Failed");
        });
    });

    $("#ResetPasswordUsingEmail").on("click", function () {
        let email = $('#email').val();
        if (email !== null && email !== "") {
            $.ajax({
                url: '/api/SystemAdministration/ApplicationUser/ResetPassword/' + encodeURIComponent(recordId),
                type: 'GET',
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                if (result !== null) {
                    if (result.success && result.data !== null) {
                        Swal.fire("Success !", "Reset password instruction is sent to user's email", "success");
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
    });

    $("#BusinessUnit").select2({
        ajax:
        {
            url: "/api/SystemAdministration/Team/Select2PrimaryTeam",
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
        $('#primary_team_id').val(data.id);
        $('#primary_team_name').val(data.text);
    }).on('select2:clear', function (e) {
        $('#primary_team_id').val('');
    });

    $('#cbIsActive').change(function () {
        $('#is_active').val(this.checked);
    });
    $('#cbIsSysAdmin').change(function () {
        $('#is_sysadmin').val(this.checked);
    });
    $('#cbUseLDAP').change(function () {
        $('#use_ldap').val(this.checked);
    });

    $('#Save').on('click', function () {
        var record = $('#form-main').serializeToJSON();

        $.ajax({
            url: '/api/SystemAdministration/ApplicationUser/save',
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
                            var url = "/SystemAdministration/ApplicationUser/Detail/" + encodeURIComponent(result.data.id);
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
                        url: '/api/SystemAdministration/ApplicationUser/delete/' + encodeURIComponent(recordId),
                        type: 'GET',
                        headers: {
                            "Authorization": "Bearer " + token
                        }
                    }).done(function (result) {
                        if (result !== null) {
                            if (result.success) {
                                Swal.fire("Deleted !", "Record is successfully deleted.", "success")
                                    .then(function () {
                                        var url = "/SystemAdministration/ApplicationUser/Index";
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
            //console.log('loading user = ' + recordId);

            $.ajax({
                url: '/api/SystemAdministration/ApplicationUser/getbyid/' + encodeURIComponent(recordId),
                type: 'GET',
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                //console.log(result);

                if (result !== null) {
                    if (result.success && result.data !== null) {
                        $('#id').val(result.data.id);
                        $('#application_username').val(result.data.application_username);
                        $('#fullname').val(result.data.fullname);
                        $('#email').val(result.data.email);
                        $('#primary_team_id').val(result.data.primary_team_id);
                        $('#primary_team_name').val(result.data.primary_team_name);

                        var isActive = result.data.is_active;
                        var isSysAdmin = result.data.is_sysadmin;
                        var useLDAP = result.data.use_ldap;

                        $('#cbIsActive').prop("checked", isActive);
                        $('#cbIsSysAdmin').prop("checked", isSysAdmin);
                        $('#cbUseLDAP').prop("checked", useLDAP);

                        $('#is_active').val(isActive);
                        $('#is_sysadmin').val(isSysAdmin);
                        $('#use_ldap').val(useLDAP);

                        //$('#BusinessUnit').append(new Option(
                        //    result.data.primary_team_name,
                        //    result.data.primary_team_id, true, true)
                        //).trigger('change');
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
    //console.log('user id = ' + recordId);
    if (!!recordId) {
        loadRecord(recordId);
    }
});