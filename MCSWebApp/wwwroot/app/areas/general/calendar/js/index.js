$(function () {

    var token = $.cookie("Token");
    var areaName = "General";
    var entityName = "Calendar";
    var url = "/api/" + areaName + "/" + entityName;

    const days = [
        {
            text: "Monday",
            value: "0"
        },
        {
            text: "Tuesday",
            value: "1"
        },
        {
            text: "Wednesday",
            value: "2"
        },
        {
            text: "Thursday",
            value: "3"
        },
        {
            text: "Friday",
            value: "4"
        },
        {
            text: "Saturday",
            value: "5"
        },
        {
            text: "Sunday",
            value: "6"
        },

    ]
    var tagBox

    $("#grid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "id",
            loadUrl: url + "/DataGrid",
            insertUrl: url + "/InsertData",
            updateUrl: url + "/UpdateData",
            deleteUrl: url + "/DeleteData",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
                ajaxOptions.beforeSend = function (request) {
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
                dataField: "calendar_name",
                dataType: "string",
                caption: "Calendar Name",
                validationRules: [{
                    type: "required",
                    message: "The Calendar Name field is required."
                }],
                sortOrder: "asc"
            },
            {
                dataField: "start_date",
                dataType: "date",
                caption: "Start Date",
                validationRules: [{
                    type: "required",
                    message: "The Start Date field is required."
                }]
            },
            {
                dataField: "end_date",
                dataType: "date",
                caption: "End Date",
                validationRules: [{
                    type: "required",
                    message: "The End Date field is required."
                }]
            },
            {
                dataField: "days",
                dataType: "string",
                caption: "Business Days",
                editCellTemplate: function (cellElement, cellInfo) {
                    $tagBox = $("<div>").dxTagBox({
                        items: days,
                        displayExpr: "text",
                        valueExpr: "value",
                        showSelectionControls: true,
                        value: (cellInfo.value ? cellInfo.value.split(",") : []),
                        onValueChanged: function (e) {
                            cellInfo.setValue(e.value.join(","))
                        }
                    }).appendTo(cellElement);
                },
                customizeText: (cellInfo) => {
                    if (!cellInfo.value) {
                        return ""
                    }

                    let daysIndex = cellInfo.value.split(",")
                    let selectedDays = ""

                    let i = 1
                    let length = daysIndex.length
                    daysIndex.forEach(index => {
                        selectedDays += days[index].text

                        if (i < length)
                            selectedDays += ", "

                        i = i + 1
                    })

                    return selectedDays
                }
            }
        ],
        onInitNewRow: function (e) {
            e.data.days =  "0,1,2,3,4"
        },
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
            pageSize: 10
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
        masterDetail: {
            enabled: true,
            template: function (container, options) {
                var currentRecord = options.data;

                // National Holiday Container
                renderNationalHoliday(currentRecord, container)
            }
        },
        editing: {
            mode: "form",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            form: {
                colCount: 2,
                items: [
                    {
                        dataField: "calendar_name",
                        colSpan: 2
                    },
                    {
                        dataField: "start_date"
                    },
                    {
                        dataField: "end_date"
                    },
                    {
                        dataField: "days",
                    }
                ]
            }
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


    const renderNationalHoliday = function (currentRecord, container) {
        var urlNationalHoliday = "/api/General/NationalHoliday";

        let nationalHolidayContainer = $("<div>")
        nationalHolidayContainer.appendTo(container)

        $("<div>")
            .addClass("master-detail-caption mb-2")
            .text("National Holiday")
            .appendTo(nationalHolidayContainer);

        $("<div>").dxDataGrid({
            dataSource: DevExpress.data.AspNet.createStore({
                key: "id",
                loadUrl: urlNationalHoliday + "/ByCalendarId/" + encodeURIComponent(currentRecord.id),
                insertUrl: urlNationalHoliday + "/InsertData",
                updateUrl: urlNationalHoliday + "/UpdateData",
                deleteUrl: urlNationalHoliday + "/DeleteData",
                onBeforeSend: function (method, ajaxOptions) {
                    ajaxOptions.xhrFields = { withCredentials: true };
                    ajaxOptions.beforeSend = function (request) {
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
                    dataField: "calendar_id",
                    dataType: "text",
                    caption: "Calendar",
                    validationRules: [{
                        type: "required",
                        message: "The Calendar field is required."
                    }],
                    visible: false
                },
                {
                    dataField: "date",
                    dataType: "date",
                    caption: "Date",
                    validationRules: [{
                        type: "required",
                        message: "The Date field is required."
                    }]
                },
                {
                    dataField: "description",
                    dataType: "string",
                    caption: "Description",
                }
            ],
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
                pageSize: 10
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
                allowAdding: true,
                allowUpdating: true,
                allowDeleting: true,
                useIcons: true,
                form: {
                    colCount: 1,
                    items: [
                        {
                            dataField: "date"
                        },
                        {
                            dataField: "description",
                            editorType: "dxTextArea",
                            height: 20,
                        },
                    ]
                }
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
            onInitNewRow: function (e) {
                e.data.calendar_id = currentRecord.id;
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
        }).appendTo(container);
    }

    $('#btnUpload').on('click', function () {
        var f = $("#fUpload")[0].files;
        var filename = $('#fUpload').val();

        if (f.length == 0) {
            alert("Please select a file.");
            return false;
        }
        else {
            var fileExtension = ['xlsx', 'xlsm', 'xls'];
            var extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert("Please select only Excel files.");
                return false;
            }
        }

        $('#btnUpload')
            .html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Uploading ...');

        var reader = new FileReader();
        reader.readAsDataURL(f[0]);
        reader.onload = function () {
            var formData = {
                "filename": f[0].name,
                "filesize": f[0].size,
                "data": reader.result.split(',')[1]
            };
            $.ajax({
                url: "/api/General/Calendar/UploadDocument",
                type: 'POST',
                cache: false,
                contentType: "application/json",
                data: JSON.stringify(formData),
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                alert('File berhasil di-upload!');
                //location.reload();
                $("#modal-upload-file").modal('hide');
                $("#grid").dxDataGrid("refresh");
            }).fail(function (jqXHR, textStatus, errorThrown) {
                window.location = '/General/General/UploadError';
                alert('File gagal di-upload!');
            }).always(function () {
                $('#btnUpload').html('Upload');
            });
        };
        reader.onerror = function (error) {
            alert('Error: ' + error);
        };
    });

});