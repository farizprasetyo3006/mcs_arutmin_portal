$(function () {

    var token = $.cookie("Token");
    var areaName = "SystemAdministration";
    var entityName = "ApplicationUser";
    var url = "/api/" + areaName + "/" + entityName;

    let applicationUserGrid = $("#grid").dxDataGrid({
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
        columnAutoWidth: true,
        columns: [
            {
                dataField: "application_username",
                dataType: "string",
                caption: "User Name",
                formItem: {
                    colSpan: 2
                },
                sortIndex: 0,
                sortOrder: "asc",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "fullname",
                dataType: "string",
                caption: "Full Name",
                formItem: {
                    colSpan: 2
                },
                sortIndex: 1,
                sortOrder: "asc",
                validationRules: [{
                    type: "required",
                    message: "This field is required."
                }]
            },
            {
                dataField: "email",
                dataType: "string",
                caption: "Email",
                formItem: {
                    colSpan: 2
                },
                width: 120
            },
            {
                dataField: "business_unit_id",
                dataType: "text",
                caption: "Business Unit",
                formItem: {
                    colSpan: 2
                },
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: url + "/PrimaryTeamIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    valueExpr: "value",
                    displayExpr: "text"
                }
            },
            {   //*** ini tambahan kolom jika tdk pake detail page
                dataField: "is_active",
                dataType: "boolean",
                caption: "Is Active",
                formItem: {
                    colSpan: 2,
                },
                width: "110"
            },
            {
                dataField: "api_key",
                dataType: "text",
                caption: "API Key",
                formItem: {
                    visible: false
                },
                cellTemplate: function (container, options) {
                    let apiKeyContainer = $("<div class='d-flex'>")
                    let api_key = options.data.api_key || ''
                    let id = options.data.id

                    // if(!api_key) return false

                    apiKeyContainer.append("<span class='mr-auto'>" + api_key + "</span>")

                    $("<div class='api-key-refresh-btn-container d-inline-block ml-2'>").dxButton({
                        icon: "refresh",
                        hint: "Refresh API Key",
                        stylingMode: "text",
                        onClick: function (e) {

                            let formData = new FormData()
                            formData.append("Id", id)

                            $.ajax({
                                url: `/api/SystemAdministration/ApplicationUser/RefreshKey`,
                                type: "PUT",
                                data: formData,
                                processData: false,
                                contentType: false,
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                                },
                                success: function (response) {
                                    applicationUserGrid.refresh()
                                }
                            })
                        }
                    }).appendTo(apiKeyContainer);

                    container.append(apiKeyContainer)
                }
            },
            {
                dataField: "expired_date",
                dataType: "date",
                caption: "API Expired Date",
                formItem: {
                    visible: false
                }
            },
            {
                dataField: "use_ldap",
                dataType: "boolean",
                caption: "Use LDAP",
                formItem: {
                    colSpan: 2,
                    visible: true
                },
                width: "110"
            },
            {
                dataField: "is_sysadmin",
                dataType: "boolean",
                caption: "Is SysAdmin",
                formItem: {
                    colSpan: 2,
                },
                visible: true
            },
            {
                type: "buttons",
                buttons: [
                    {
                        hint: "Edit",
                        icon: "edit",
                        visible: true,
                        onClick: function (e) {
                            window.location = "/" + areaName + "/" + entityName + "/Detail/" + encodeURIComponent(e.row.data.id);
                            e.event.preventDefault();
                        }
                    },
                    "delete"
                ],
                showInColumnChooser: false
            },
        ],
        masterDetail: {
            enabled: true,
            template: masterDetailTemplate
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
        height: 800,
        showBorders: true,
        editing: {
            mode: "form",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },
        grouping: {
            contextMenuEnabled: true,
            autoExpandAll: false
        },
        rowAlternationEnabled: true,
        onEditorPreparing: function (e) {
            if (e.parentType === "dataRow" && e.dataField === "application_username") {
                e.editorOptions.disabled = e.row.data.application_username;
            }
        },
        export: {
            enabled: true,
            allowExportSelectedData: true
        },
        onInitNewRow: function (e) {
            e.data.is_active = true;
            e.data.use_ldap = false;
            e.data.is_sysadmin = false;
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
    }).dxDataGrid("instance");

    function masterDetailTemplate(_, masterDetailOptions) {
        return $("<div>").dxTabPanel({
            items: [
                {
                    title: "Teams",
                    template: createTeamsTabTemplate(masterDetailOptions.data)
                },
                {
                    title: "Roles",
                    template: createRolesTabTemplate(masterDetailOptions.data)
                }
            ]
        });
    }

    function createRolesTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            let detailName = "UserRole";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        loadUrl: urlDetail + "/ByApplicationUserId/" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/InsertData",
                        updateUrl: urlDetail + "/UpdateData",
                        deleteUrl: urlDetail + "/DeleteData",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    remoteOperations: true,
                    allowColumnResizing: true,
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        //{
                        //    dataField: "application_user_id",
                        //    caption: "User Id",
                        //    formItem: {
                        //        visible: false
                        //    },
                        //    allowEditing: false,
                        //    visible: false,
                        //    calculateCellValue: function () {
                        //        return currentRecord.id;
                        //    }
                        //},
                        {
                            dataField: "application_role_id",
                            dataType: "text",
                            caption: "Role",
                            formItem: {
                                colSpan: 2
                            },
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
                            }],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    loadUrl: urlDetail + "/ApplicationRoleIdLookup",
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                }),
                                valueExpr: "value",
                                displayExpr: "text"
                            }
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
                        enabled: false,
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
                    showBorders: true,
                    editing: {
                        mode: "form",
                        allowAdding: true,
                        allowUpdating: true,
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
                    onInitNewRow: function (e) {
                        e.data.application_user_id = currentRecord.id;
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
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailName + '.xlsx');
                            });
                        });
                        e.cancel = true;
                    }
                });
        }
    }

    function createTeamsTabTemplate(masterDetailData) {
        return function () {
            let currentRecord = masterDetailData;
            //let detailName = "Team";
            let detailName = "TeamMember";
            let urlDetail = "/api/" + areaName + "/" + detailName;

            return $("<div>")
                .dxDataGrid({
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "id",
                        //loadUrl: urlDetail + "/ByTeamId/" + encodeURIComponent(currentRecord.id),
                        loadUrl: urlDetail + "/ByApplicationUserId/" + encodeURIComponent(currentRecord.id),
                        insertUrl: urlDetail + "/InsertData",
                        updateUrl: urlDetail + "/UpdateData",
                        deleteUrl: urlDetail + "/DeleteData",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    remoteOperations: true,
                    allowColumnResizing: true,
                    dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
                    columns: [
                        //{
                        //    dataField: "application_user_id",
                        //    caption: "User Id",
                        //    formItem: {
                        //        visible: false
                        //    },
                        //    allowEditing: false,
                        //    visible: false,
                        //    calculateCellValue: function () {
                        //        return currentRecord.id;
                        //    }
                        //},
                        {
                            dataField: "team_id",
                            dataType: "text",
                            caption: "Team",
                            formItem: {
                                colSpan: 2
                            },
                            validationRules: [{
                                type: "required",
                                message: "This field is required."
                            }],
                            lookup: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "value",
                                    //loadUrl: urlDetail + "/TeamIdLookup",
                                    loadUrl: "/api/SystemAdministration/Team/TeamIdLookup",
                                    onBeforeSend: function (method, ajaxOptions) {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + token);
                                        };
                                    }
                                }),
                                valueExpr: "value",
                                displayExpr: "text"
                            }
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
                        enabled: false,
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
                    showBorders: true,
                    editing: {
                        mode: "form",
                        allowAdding: true,
                        allowUpdating: true,
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
                    onInitNewRow: function (e) {
                        e.data.application_user_id = currentRecord.id;
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
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), detailName + '.xlsx');
                            });
                        });
                        e.cancel = true;
                    }
                });
        }
    }
});