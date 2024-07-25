$(function () {
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
        "timeOut": 3000,
        "extendedTimeOut": 500,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    var token = $.cookie("Token");
    var areaName = "Report";
    var entityName = "ReportViewer";
    var url = "/api/" + areaName + "/" + entityName;    

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: url + "/DataGrid",
            insertUrl: url + "/InsertData",
            updateUrl: url + "/UpdateData",
            deleteUrl: url + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function(request){
                    request.setRequestHeader("Authorization", "Bearer " + token);
                };                
            }
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        columns: [
            {
                dataField: "report_name",
                dataType: "string",
                colSpan: 2,
                caption: "Report Name",
                validationRules: [{
                    type: "required",
                    message: "This field is required." 
                }],
                sortOrder: 'asc'
            },
            {
                type: "buttons",
                buttons: [
                    //{
                    //    text: "Upload",
                    //    icon: "upload",
                    //    hint: "Upload report template",
                    //    onClick: function (e) {
                    //            //console.log(e);
                    //            $("#recordkey").val(e.row.key);
                    //            $("#modal-upload-file").modal("show");
                    //    }
                    //},
                    //{
                    //    text: "Download",
                    //    icon: "download",
                    //    hint: "Download report template",
                    //    onClick: function (e) {
                    //        ////console.log(e);
                    //        let fileUrl = url + "/DownloadDocument?Id=" + encodeURIComponent(e.row.key);
                    //        //console.log(fileUrl);

                    //        $.ajax({
                    //            url: fileUrl,
                    //            type: 'GET',
                    //            cache: false,
                    //            headers: {
                    //                "Authorization": "Bearer " + token
                    //            }
                    //        }).done(function (data, textStatus, request) {
                    //            let blob = new Blob([data]);

                    //            // the file name from server.
                    //            let fileName = "";
                    //            let disposition = request.getResponseHeader('Content-Disposition');
                    //            if (disposition && (disposition.indexOf('attachment') !== -1
                    //                || disposition.indexOf('inline') !== -1)) {
                    //                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                    //                var matches = filenameRegex.exec(disposition);
                    //                if (matches != null && matches[1]) {
                    //                    fileName = matches[1].replace(/['"]/g, '');
                    //                }
                    //            }
                    //            if (fileName === "") {
                    //                fileName = e.row.data.report_name + ".frx";
                    //            }

                    //            if (window.navigator && window.navigator.msSaveOrOpenBlob) { // for IE
                    //                window.navigator.msSaveOrOpenBlob(blob, fileName);
                    //            } else { // for others
                    //                let url = window.URL.createObjectURL(blob);
                    //                const a = document.createElement('a');
                    //                a.style.display = 'none';
                    //                a.href = url;
                    //                a.download = fileName;
                    //                document.body.appendChild(a);
                    //                a.click();
                    //                window.URL.revokeObjectURL(url);
                    //            }
                    //        });
                    //    }
                    //},
                    {
                        text: "View",
                        icon: "datafield",
                        hint: "View report",
                        onClick: function (e) {
                            let urlView = "/" + areaName + "/" + entityName + "/Viewer"
                                + "/" + encodeURIComponent(e.row.key);
                            window.location = urlView;
                            //$("#recordkey").val(e.row.key);
                        }
                    }, //"edit", "delete"
                ]
            }],
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: true
        },
        groupPanel: {
            visible: true
        },
        searchPanel: {
            visible: true,
            width: 240,
            placeholder: "Search..."
        },
        filterPanel: {
            visible: true
        },
        filterBuilderPopup: {
            position: { of: window, at: "top", my: "top", offset: { y: 10 } },
        },
        columnChooser: {
            enabled: true,
            mode: "select"
        },
        paging: {
            pageSize: 50
        },
        pager: {
            allowedPageSizes: [10, 20, 50, 100],
            showNavigationButtons: true,
            showPageSizeSelector: true,
            showInfo: true,
            visible: true
        },
        height: 600,
        showBorders: true,        
        editing: {
            mode: "form",
            allowAdding: false,
            allowUpdating: false,
            allowDeleting: true,
            useIcons: true
        },
        grouping: {
            contextMenuEnabled: true,
            autoExpandAll: false
        },
        rowAlternationEnabled: true,        
        export: {
            enabled: true,
            allowExportSelectedData: true
        },
        onExporting: function (e) {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet(entityName);

            DevExpress.excelExporter.exportDataGrid({
                component: e.component,
                worksheet: worksheet,
                autoFilterEnabled: true
            }).then(function () {
                // https://github.com/exceljs/exceljs#writing-xlsx
                workbook.xlsx.writeBuffer().then(function (buffer) {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), entityName + '.xlsx');
                });
            });
            e.cancel = true;
        }
    });

    $('#btnUpload').on('click', function () {
        var f = $("#fUpload")[0].files;
        var filename = $('#fUpload').val();

        if (f.length == 0) {
            alert("Please select a file.");
            return false;
        }
        else {
            var fileExtension = ['frx'];
            var extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert("Please select only FastReport file.");
                return false;
            }
        }

        let btn = $(this);
        let reader = new FileReader();
        reader.readAsDataURL(f[0]);
        reader.onload = function () {
            $(btn).buttonLoader('start');

            var formData = {
                "recordkey": $("#recordkey").val(),
                "filename": f[0].name,
                "filesize": f[0].size,
                "data": reader.result.split(',')[1]
            };
            $.ajax({
                url: "/api/Report/Viewer/UploadDocument",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(formData),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                //$("#upload-result").attr("hidden", true);
                $(btn).buttonLoader('stop');
                if (result !== null && result !== undefined) {
                    if (result.success) {
                        toastr["success"]("File is uploaded.");
                    }
                    else if (result.message !== null && result.message !== undefined) {
                        toastr["error"](result.message);
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                $(btn).buttonLoader('stop');
                toastr["error"]("Failed to upload file.");
            });
        };
        reader.onerror = function (error) {
            alert('Error: ' + error);
            $(btn).buttonLoader('stop');
        };
    });
});