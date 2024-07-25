$(document).ready(function () {

    var token = $.cookie("Token");
    var basePath = "/Transaction/InventoryTransaction";
    var apiPath = "/api" + basePath;

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
            title: "Date Time",
            id: "transaction_datetime",
            data: "transaction_datetime",
            type: "text"
        },
        {
            title: "Transaction Number",
            id: "transaction_number",
            data: "transaction_number",
            type: "text"
        },
        {
            title: "Product Name",
            id: "product_name",
            data: "product_name",
            type: "text"
        },
        {
            title: "Source Location",
            id: "source_location_name",
            data: "source_location_name",
            type: "text"
        },
        {
            title: "Destination Location",
            id: "destination_location_name",
            data: "destination_location_name",
            type: "text"
        },
        {
            title: "Quantity",
            id: "quantity",
            data: "quantity",
            type: "text"
        },
        {
            title: "UOM",
            id: "uom_name",
            data: "uom_name",
            type: "text"
        }];

    /* buttons uses classes from bootstrap, see buttons page for more details*/
    var buttonSet = [];

    buttonSet.push({
        extend: 'selectRows',
        text: '<i class="fal fa-times mr-1"></i> Delete',
        name: 'delete',
        className: 'btn-danger btn-sm mr-1',
        action: function (e, dt, node, config) {
            var selectedRows = dt.rows('.selected').data();
            var recordId = selectedRows[0].id;
            var token = $.cookie("Token");

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
                            url: apiPath + '/Delete/' + encodeURIComponent(recordId),
                            type: 'GET',
                            headers: {
                                "Authorization": "Bearer " + token
                            }
                        }).done(function (result) {
                            if (result !== null) {
                                if (result.success) {
                                    dt.rows('.selected').remove();
                                    dt.ajax.reload();
                                    dt.draw();

                                    Swal.fire("Deleted !", "Record is successfully deleted.", "success");
                                }
                                else {
                                    Swal.fire("Error !", result.status.message, "error");
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
        }
    });

    buttonSet.push({
        extend: 'selectRows',
        text: '<i class="fal fa-edit mr-1"></i> Edit',
        name: 'edit',
        className: 'btn-primary btn-sm mr-1',
        action: function (e, dt, node, config) {
            var selectedRows = dt.rows('.selected').data();
            if (selectedRows !== null && selectedRows.length > 0) {
                var recordId = selectedRows[0].id;
                var url = basePath + "/Detail/" + encodeURIComponent(recordId);
                window.location = url;
            }
        }
    });

    buttonSet.push({
        text: '<i class="fal fa-plus mr-1"></i> Add',
        name: 'add',
        className: 'btn-success btn-sm mr-1',
        action: function (e, dt, node, config) {
            var url = basePath + "/Detail";
            window.location = url;
        }
    });

    /* start data table */
    var dtMain = $('#dt-main').dataTable(
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
                url: apiPath + "/datatables",
                type: "POST",
                headers: {
                    "Authorization": "Bearer " + token
                }
            },
            columns: columnSet,
            responsive: true,
            select: true,
            buttons: buttonSet,
            order: [[1, "asc"]]
        });
});