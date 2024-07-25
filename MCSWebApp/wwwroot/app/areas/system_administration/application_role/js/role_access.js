$(function () {
    var token = $.cookie("Token");
    var areaName = "SystemAdministration";
    var entityName = "ApplicationRole";
    var url = "/api/" + areaName + "/" + entityName;    

    //var MAX_ROLE_COUNT = 10000;
    var ACCESS_NO_ACCESS = 0;
    var ACCESS_OWN_RECORD = 1;
    //var ACCESS_BUSINESS_UNIT = MAX_ROLE_COUNT;
    //var ACCESS_DEEP_BUSINESS_UNIT = ACCESS_BUSINESS_UNIT * MAX_ROLE_COUNT;
    //var ACCESS_ORGANIZATION = ACCESS_DEEP_BUSINESS_UNIT * MAX_ROLE_COUNT;
    var ACCESS_BUSINESS_UNIT = 10;
    var ACCESS_DEEP_BUSINESS_UNIT = 100;
    var ACCESS_ORGANIZATION = 1000;

    var applicationRoleId = $('#ApplicationRoleId').val();

    // Column Definitions
    var columnSet = [
        {
            title: "Id",
            id: "id",
            data: "id",
            type: "readonly",
            visible: false,
            searchable: false
        },
        {
            title: "Entity",
            id: "display_name",
            data: "display_name",
            type: "text"
        },
        {
            title: "Create",
            id: "access_create",
            data: "access_create",
            class: "text-center", 
            type: "text",
            render: function (data, type, row) {
                let d = ' <button data-id="' + row.id + '" data-field="access_create" data-value="' + data + '" ';
                d += ' class="btn btn-sm btn-icon btn-circle" > ';

                if (data <= ACCESS_NO_ACCESS) {
                    d += ' <pie class="ten icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_OWN_RECORD) {
                    d += ' <pie class="twentyfive icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_BUSINESS_UNIT) {
                    d += ' <pie class="fifty icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_DEEP_BUSINESS_UNIT) {
                    d += ' <pie class="seventyfive icon-sm entity-pie"></pie> ';
                }
                else {
                    d += ' <pie class="onehundred icon-sm entity-pie"></pie> ';
                }

                d += ' </button> ';
                return d;
            }
        },
        {
            title: "Read",
            id: "access_read",
            data: "access_read",
            class: "text-center",
            type: "text",
            render: function (data, type, row) {
                let d = ' <button data-id="' + row.id + '" data-field="access_read" data-value="' + data + '" ';
                d += ' class="btn btn-sm btn-icon btn-circle" > ';

                if (data <= ACCESS_NO_ACCESS) {
                    d += ' <pie class="ten icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_OWN_RECORD) {
                    d += ' <pie class="twentyfive icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_BUSINESS_UNIT) {
                    d += ' <pie class="fifty icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_DEEP_BUSINESS_UNIT) {
                    d += ' <pie class="seventyfive icon-sm entity-pie"></pie> ';
                }
                else {
                    d += ' <pie class="onehundred icon-sm entity-pie"></pie> ';
                }

                d += ' </button> ';
                return d;
            }
        },
        {
            title: "Update",
            id: "access_update",
            data: "access_update",
            class: "text-center",
            type: "text",
            render: function (data, type, row) {
                let d = ' <button data-id="' + row.id + '" data-field="access_update" data-value="' + data + '" ';
                d += ' class="btn btn-sm btn-icon btn-circle" > ';

                if (data <= ACCESS_NO_ACCESS) {
                    d += ' <pie class="ten icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_OWN_RECORD) {
                    d += ' <pie class="twentyfive icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_BUSINESS_UNIT) {
                    d += ' <pie class="fifty icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_DEEP_BUSINESS_UNIT) {
                    d += ' <pie class="seventyfive icon-sm entity-pie"></pie> ';
                }
                else {
                    d += ' <pie class="onehundred icon-sm entity-pie"></pie> ';
                }

                d += '</button>';
                return d;
            }
        },
        {
            title: "Delete",
            id: "access_delete",
            data: "access_delete",
            class: "text-center",
            type: "text",
            render: function (data, type, row) {
                let d = ' <button data-id="' + row.id + '" data-field="access_delete" data-value="' + data + '" ';                
                d += ' class="btn btn-sm btn-icon btn-circle" > ';

                if (data <= ACCESS_NO_ACCESS) {
                    d += ' <pie class="ten icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_OWN_RECORD) {
                    d += ' <pie class="twentyfive icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_BUSINESS_UNIT) {
                    d += ' <pie class="fifty icon-sm entity-pie"></pie> ';
                }
                else if (data <= ACCESS_DEEP_BUSINESS_UNIT) {
                    d += ' <pie class="seventyfive icon-sm entity-pie"></pie> ';
                }
                else {
                    d += ' <pie class="onehundred icon-sm entity-pie"></pie> ';
                }

                d += ' </button> ';
                return d;
            }
        }];

    /* buttons uses classes from bootstrap, see buttons page for more details*/
    var buttonSet = [];

    function accessTemplate(id, field, value) {
        //'<input  type="text" name="' + field + '" data-id="' + data.id + '" value="' + data[field] + '" class="entity-pie form-control form-control-sm m-input" />';

        let d = '<button data-id="' + id + '" data-field="' + field + '"' + '" data-value="' + value + '"'
            + ' class="btn btn-sm btn-icon btn-circle entity-pie">';
        if (value <= ACCESS_NO_ACCESS)
            d = + '<pie class="twentyfive icon-sm"></pie>';
        else if (value <= ACCESS_OWN_RECORD)
            d = + '<pie class="twentyfive icon-sm"></pie>';
        else if (value <= ACCESS_BUSINESS_UNIT)
            d = + '<pie class="fifty icon-sm"></pie>';
        else if (value <= ACCESS_BUSINESS_UNIT)
            d = + '<pie class="seventyfive icon-sm"></pie>';
        else d = + '<pie class="onehundred icon-sm"></pie>';
        d += '</button>';
        return d;
    }

    var updateAccess = function (domObj) {
        let roleAccessId = domObj.parent().data('id');
        let fieldName = domObj.parent().attr('data-field');
        let currentValue = domObj.parent().attr('data-value');

        let obj = {
            id: roleAccessId,
            field: fieldName,
            value: currentValue
        };
        //console.log(obj);

        $.ajax({
            url: '/api/SystemAdministration/RoleAccess/Update',
            type: 'POST',
            headers: {
                "Authorization": "Bearer " + token
            },
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(obj)
        }).done(function (result) {
            if (result && result.success && result.data) {
                if (domObj.hasClass('ten') && result.data.value === ACCESS_OWN_RECORD) {
                    domObj.removeClass('ten');
                    domObj.addClass('twentyfive icon-sm entity-pie');
                    domObj.parent().attr('data-value', ACCESS_OWN_RECORD);
                }
                else if (domObj.hasClass('twentyfive') && result.data.value === ACCESS_BUSINESS_UNIT) {
                    domObj.removeClass('twentyfive');
                    domObj.addClass('fifty icon-sm entity-pie');
                    domObj.parent().attr('data-value', ACCESS_BUSINESS_UNIT);
                }
                else if (domObj.hasClass('fifty') && result.data.value === ACCESS_DEEP_BUSINESS_UNIT) {
                    domObj.removeClass('fifty');
                    domObj.addClass('seventyfive icon-sm entity-pie');
                    domObj.parent().attr('data-value', ACCESS_DEEP_BUSINESS_UNIT);
                }
                else if (domObj.hasClass('seventyfive') && result.data.value === ACCESS_ORGANIZATION) {
                    domObj.removeClass('seventyfive');
                    domObj.addClass('onehundred icon-sm entity-pie');
                    domObj.parent().attr('data-value', ACCESS_ORGANIZATION);
                }
                else if (domObj.hasClass('onehundred') && result.data.value === ACCESS_NO_ACCESS) {
                    domObj.removeClass('onehundred');
                    domObj.addClass('ten icon-sm entity-pie');
                    domObj.parent().attr('data-value', ACCESS_NO_ACCESS);
                }
            }

            //if (result && result.success && result.data) {
            //    if (domObj.hasClass('ten') && result.data.value === ACCESS_ORGANIZATION) {
            //        domObj.removeClass('ten');
            //        domObj.addClass('onehundred icon-sm entity-pie');
            //        domObj.parent().attr('data-value', ACCESS_ORGANIZATION);
            //    }
            //    else {
            //        domObj.removeClass('onehundred');
            //        domObj.addClass('ten icon-sm entity-pie');
            //        domObj.parent().attr('data-value', ACCESS_NO_ACCESS);
            //    }
            //}

        });
    };

    $('.entity-pie').on('click', function () {
        let _id = $(this).data('id');
        //console.log(_id);
        alert(_id);
    });

    var loadApplicationRole = function () {
        if (applicationRoleId !== null && applicationRoleId !== '') {
            $.ajax({
                url: url + '/Detail/' + encodeURIComponent(applicationRoleId),
                type: 'GET',
                headers: {
                    "Authorization": "Bearer " + token
                }
            }).done(function (result) {
                ////console.log(result.data)
                //if (result && result.data) {
                if (result) {
                    //$('#role_name').text('Role :: ' + result.data.role_name);
                    $('#role_name').text('Role :: ' + result.role_name);

                    let dtUrl = "/api/SystemAdministration/RoleAccess/DataTables/" + encodeURIComponent(applicationRoleId);
                    //console.log(dtUrl);

                    /* start data table */
                    let dtMain = $('#dt-main').dataTable(
                        {
                            /*	--- Layout Structure
                                --- Options
                                l	-	length changing input control
                                f	-	filtering input
                                t	-	The table!
                                i	-	Table information summary
                                p	-	pagination control
                                r	-	processing display element
                                B	-	buttons
                                R	-	ColReorder
                                S	-	Select
                
                                --- Markup
                                < and >				- div element
                                <"class" and >		- div with a class
                                <"#id" and >		- div with an ID
                                <"#id.class" and >	- div with an ID and a class
                
                                --- Further reading
                                https://datatables.net/reference/option/dom
                                --------------------------------------
                             */
                            dom: "<'row mb-3'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'B>>" +
                                "<'row'<'col-sm-12'tr>>" +
                                "<'row'<'col-sm-12 col-md-4'l><'col-sm-12 col-md-4'i><'col-sm-12 col-md-4'p>>",
                            ajax: {
                                url: dtUrl,
                                type: "POST",
                                headers: {
                                    "Authorization": "Bearer " + token
                                }
                            },
                            columns: columnSet,
                            responsive: true,
                            //select: true,
                            buttons: buttonSet,
                            order: [[1, "asc"]],
                            drawCallback: function () {
                                $('.entity-pie').on('click', function () {
                                    let _this = $(this);
                                    updateAccess(_this);
                                });
                            }
                        });
                }
            });
        }
    }

    loadApplicationRole();
});