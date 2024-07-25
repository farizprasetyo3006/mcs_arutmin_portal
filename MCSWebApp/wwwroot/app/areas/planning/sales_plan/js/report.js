$(document).ready(function () {
    var token = $.cookie("Token");
    var sampleSnapshotData = {
        monthly: {
            data: [
                {
                    month_index: "1",
                    month_name: "January",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93181
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 99915
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 63493
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 99124,
                    target: 1000000,
                    carryOver: 0,
                    carryForward: 109232
                },
                {
                    month_index: "2",
                    month_name: "February",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 93842
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 94223
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 96783
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 97493
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 68313
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 60924
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 98323,
                    target: 1000000,
                    carryOver: 109232,
                    carryForward: 93925
                },
                {
                    month_index: "3",
                    month_name: "March",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 94422
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93312
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90024
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 59121
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 93012,
                    target: 1000000,
                    carryOver: 93925,
                    carryForward: 90302
                },
                {
                    month_index: "4",
                    month_name: "April",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 93842
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 94223
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 96783
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 97493
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 68313
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 60924
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 98323,
                    target: 1000000,
                    carryOver: 109232,
                    carryForward: 93925
                },
                {
                    month_index: "5",
                    month_name: "May",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93181
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 99915
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 63493
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 98012,
                    target: 1000000,
                    carryOver: 0,
                    carryForward: 90123
                },
                {
                    month_index: "6",
                    month_name: "June",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 94422
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93312
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90024
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 59121
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 93012,
                    target: 1000000,
                    carryOver: 93925,
                    carryForward: 90302
                },
                {
                    month_index: "7",
                    month_name: "July",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93181
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 99915
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 63493
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 99124,
                    target: 1000000,
                    carryOver: 0,
                    carryForward: 109232
                },
                {
                    month_index: "8",
                    month_name: "August",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 93842
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 94223
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 96783
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 97493
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 68313
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 60924
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 98323,
                    target: 1000000,
                    carryOver: 109232,
                    carryForward: 93925
                },
                {
                    month_index: "9",
                    month_name: "September",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 94422
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93312
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90024
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 59121
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 93012,
                    target: 1000000,
                    carryOver: 93925,
                    carryForward: 90302
                },
                {
                    month_index: "10",
                    month_name: "October",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 93842
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 94223
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 96783
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 97493
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 68313
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 60924
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 98323,
                    target: 1000000,
                    carryOver: 109232,
                    carryForward: 93925
                },
                {
                    month_index: "11",
                    month_name: "November",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93181
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 99915
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 63493
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 98012,
                    target: 1000000,
                    carryOver: 0,
                    carryForward: 90123
                },
                {
                    month_index: "12",
                    month_name: "December",
                    contracts: [
                        {
                            name: "Sales Contract A",
                            type: "Export",
                            planned: 100000,
                            actual: 94422
                        },
                        {
                            name: "Sales Contract B",
                            type: "Export",
                            planned: 100000,
                            actual: 93317
                        },
                        {
                            name: "Sales Contract C",
                            type: "Export",
                            planned: 100000,
                            actual: 93312
                        },
                        {
                            name: "Sales Contract D",
                            type: "Export",
                            planned: 100000,
                            actual: 90900
                        },
                        {
                            name: "Sales Contract E",
                            type: "Export",
                            planned: 100000,
                            actual: 90024
                        },
                        {
                            name: "Sales Contract F",
                            type: "Export",
                            planned: 100000,
                            actual: 99352
                        },
                        {
                            name: "Sales Contract PLN A",
                            type: "Domestic",
                            planned: 64000,
                            actual: 67389
                        },
                        {
                            name: "Sales Contract PLN B",
                            type: "Domestic",
                            planned: 64000,
                            actual: 59121
                        },
                    ],
                    totalPlanned: 800000,
                    totalActual: 93012,
                    target: 1000000,
                    carryOver: 93925,
                    carryForward: 90302
                },
            ]
        },
        total: {
            data: {
                contracts: [
                    {
                        name: "Sales Contract A",
                        type: "Export",
                        planned: 2250122,
                        actual: 1182323
                    },
                    {
                        name: "Sales Contract B",
                        type: "Export",
                        planned: 2250122,
                        actual: 1143677
                    },
                    {
                        name: "Sales Contract C",
                        type: "Export",
                        planned: 2358134,
                        actual: 1286493
                    },
                    {
                        name: "Sales Contract D",
                        type: "Export",
                        planned: 2335784,
                        actual: 909030
                    },
                    {
                        name: "Sales Contract E",
                        type: "Export",
                        planned: 2250122,
                        actual: 1286493
                    },
                    {
                        name: "Sales Contract F",
                        type: "Export",
                        planned: 2394782,
                        actual: 1143677
                    },
                    {
                        name: "Sales Contract PLN A",
                        type: "Domestic",
                        planned: 2250122,
                        actual: 1211389
                    },
                    {
                        name: "Sales Contract PLN B",
                        type: "Domestic",
                        planned: 2358134,
                        actual: 1286493
                    },
                ],
                totalPlanned: 11040000,
                totalActual: 10733282,
                target: 1200000,
            }
        }
        
    }
    var salesPlanId = null

    let salesPlanReportForm = $("#sales-plan-report-form").dxForm({
        formData: {
            sales_plan_id: "",
            data: ""
        },
        colCount: 2,
        items: [
            {
                dataField: "sales_plan_id",
                label: {
                    text: "Sales Plan"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "value",
                        loadUrl: "/api/Planning/SalesPlan/SalesPlanIdLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                            ajaxOptions.beforeSend = function (request) {
                                request.setRequestHeader("Authorization", "Bearer " + token);
                            };
                        }
                    }),
                    searchEnabled: true,
                    valueExpr: "value",
                    displayExpr: "text",
                },
            },
            {
                dataField: "sales_plan_link",
                label: {
                    text: "Sales Plan"
                },
                editorType: "dxButton",
                editorOptions: {
                    text: "See Sales Plan Detail",
                    disabled: true,
                    onClick: function (e) {
                        let salesPlanId = salesPlanReportForm.getEditor("sales_plan_id").option("value")
                        window.open("/Planning/SalesPlan/Index?Id=" + salesPlanId + "&openEditingForm=true", "_blank")
                    }
                }
            }
        ],
        onContentReady: function (e) {
            // If salesPlanId exist, enabled sales_plan_link
            if (salesPlanId) {
                let form = e.component
                form.getEditor("sales_plan_link").option("disabled", false)
            }
        },
        onFieldDataChanged: function (e) {
            if (e.dataField == "sales_plan_id") {
                salesPlanId = e.value

                getSalesPlanReport()
            }
        },
        onInitialized: async function (e) {
            let form = e.component
            let queryString = window.location.search
            let params = new URLSearchParams(queryString)

            salesPlanId = params.get("salesPlanId")

            if (salesPlanId) {
                // Set sales_plan_id value form salesPlanId
                form.updateData({ "sales_plan_id": salesPlanId })

                // Get sales plan report
                await $.ajax({
                    method: "GET",
                    url: "/Api/Planning/SalesPlanReport/GenerateReport/" + salesPlanId,
                    cache: false,
                    contentType: "application/json",
                    headers: {
                        "Authorization": "Bearer " + token
                    }
                }).done((response) => {
                    salesPlanReportData = response
                    salesPlanReportForm.updateData({ "data": JSON.stringify(response) })
                })

                renderReportContainer()

                renderMonthlyData(salesPlanReportData)
                renderTotalData(salesPlanReportData)
            }
        }
    }).dxForm("instance");

    // Save snapshot button on click
    window.saveSnapshot = () => {
        salesPlanSnapshotPopup.show()
    }

    let salesPlanSnapshotPopup = $("<div>").dxPopup({
        width: "50%",
        height: "auto",
        contentTemplate: function () {
            let salesPlanSnapshotForm = $("<div>").dxForm({
                formData: {
                    sales_plan_id: salesPlanReportForm.option("formData").sales_plan_id,
                    snapshot_name: "",
                    notes: "",
                    data: salesPlanReportForm.option("formData").data
                },
                items: [
                    {
                        dataField: "sales_plan_id",
                        label: {
                            text: "Sales Plan"
                        },
                        visible: false
                    },
                    {
                        dataField: "snapshot_name",
                        label: {
                            text: "Snapshot Name"
                        },
                    },
                    {
                        dataField: "notes",
                        label: {
                            text: "Notes"
                        },
                        colSpan: 2,
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 50
                        },
                    },
                    {
                        itemType: "button",
                        colSpan: 2,
                        horizontalAlignment: "right",
                        buttonOptions: {
                            text: "Save",
                            type: "secondary",
                            useSubmitBehavior: true,
                            onClick: function () {
                                let data = salesPlanSnapshotForm.dxForm("instance").option("formData");
                                //console.log(data)
                                let formData = new FormData()
                                formData.append("values", JSON.stringify(data))

                                $.ajax({
                                    method: "POST",
                                    url: "/Api/Planning/SalesPlanSnapshot/InsertData/",
                                    data: formData,
                                    processData: false,
                                    contentType: false,
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                                    },
                                    success: function (response) {

                                        // Redirect to SalesPlanSnapshot detail
                                        window.location = "/Planning/SalesPlanSnapshot/Detail/" + response.id
                                    }
                                })

                            }
                        }
                    }
                ],
            })

            return salesPlanSnapshotForm
        },
        showTitle: true,
        title: "Save Sales Plan Snapshot",
        hideOnOutsideClick: true,
        dragEnable: false
    }).appendTo("body").dxPopup("instance")

    /**
     * Get sales plan report
     * */
    let getSalesPlanReport = async () => {
        let salesPlanReportData = null
        await $.ajax({
            method: "GET",
            url: "/Api/Planning/SalesPlanReport/GenerateReport/" + salesPlanId,
            cache: false,
            contentType: "application/json",
            headers: {
                "Authorization": "Bearer " + token
            }
        }).done((response) => {
            salesPlanReportData = response
            salesPlanReportForm.updateData({ "data": JSON.stringify(response) })
        })

        renderReportContainer()

        renderMonthlyData(salesPlanReportData)
        renderTotalData(salesPlanReportData)
    }

    /* renderReportContainer
     * render container for monthly and total report
     */
    const renderReportContainer = () => {
        let container = $("#report-container")
        container.empty()

        $(`<div class="row">
                <div class="col">
                    <ul class="nav nav-mcs nav-pills pills-blue my-3" role="tablist">
                        <li class="nav-item"><a class="nav-link active" data-toggle="pill" href="#monthly-container">Monthly</a></li>
                        <li class="nav-item"><a class="nav-link" data-toggle="pill" href="#total-container">Total</a></li>
                    </ul>
                </div>
                <div class="col d-flex align-items-center justify-content-end">
                    <button class="btn btn-round btn-primary" id="save-snapshot-btn" onclick="saveSnapshot(this)">Save Snapshot</button>
                </div>
            </div>

            <div class="row">
                <div class="col-xl-12">

                    <div class="tab-content py-3">
                        <!-- Monthly -->
                        <div class="tab-pane fade show active" id="monthly-container" role="tabpanel">
                        </div>

                        <!-- Year (Total) -->
                        <div class="tab-pane fade" id="total-container" role="tabpanel">
                        </div>
                    </div>
                </div>
            </div>`).appendTo(container)

    }

    /* renderMonthlyData
     * render all monthly data and tables
     */
    const renderMonthlyData = (data) => {
        //console.log(data)
        let monthlyData = data.salesPlanReportMonths

        let i = 1;
        monthlyData.forEach(month => {

            let monthlyItem = $(`<div class="monthly-item mb-3">
                            <div class="card card-mcs">
                                <div class="card-body">

                                    <div class="item-head">
                                        <div class="row" data-toggle="collapse" data-target="#monthly-item-${month.month}" aria-expanded="true" aria-controls="collapseExample">
                                            <div class="col-md-6">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">${month.month < 10 ? "0"+month.month : month.month}</h6>
                                                <h4 class="font-weight-bold">${month.monthName}</h4>
                                            </div>
                                            <div class="col-md-2">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">Target</h6>
                                                <h4 class="font-weight-bold">${formatNumber(month.salesPlanTarget)}</h4>
                                            </div>
                                            <div class="col-md-2">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">Total Planned</h6>
                                                <h4 class="font-weight-bold">${formatNumber(month.salesPlanMonthTotal)}</h4>
                                            </div>
                                            <div class="col-md-2">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">Total Actual</h6>
                                                <h4 class="font-weight-bold">${formatNumber(month.salesActualMonthTotal)}</h4>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="collapse ${i == 1 && 'show'}" id="monthly-item-${month.month}">
                                        <div class="item-body mt-3">

                                            <div class="row">
                                                <div class="col-md-12">
                                                    <table class="table table-striped table-bordered">
                                                        <thead>
                                                            <tr class="table-active">
                                                                <th class="small text-uppercase font-weight-bold text-muted">Contracted</th>
                                                                <th class="small text-uppercase font-weight-bold text-muted text-center">Contract Type</th>
                                                                <th class="small text-uppercase font-weight-bold text-muted text-center text-success">Plan</th>
                                                                <th class="small text-uppercase font-weight-bold text-muted text-center text-warning">Actual</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            
                                                            <tr>
                                                                <td colspan="2" class="small text-uppercase font-weight-bold text-muted mt-2">Total</td>
                                                                <td class="text-center font-weight-bold text-success">${formatNumber(month.salesPlanMonthTotal)}</td>
                                                                <td class="text-center font-weight-bold text-warning">${formatNumber(month.salesActualMonthTotal)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" class="small text-uppercase font-weight-bold text-muted mt-2">Target</td>
                                                                <td class="text-center font-weight-bold">${formatNumber(month.salesPlanTarget)}</td>
                                                                <td></td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>

                                            <div class="row border-top pt-4">
                                                <div class="col-md-12">
                                                    <div class="row">
                                                        <div class="col px-4">
                                                            <div class="d-flex align-items-center">
                                                                <div class="d-inline-block mr-3">
                                                                    <div class="icon-circle">
                                                                        <i class="fas fa-chevron-right fa-sm"></i>
                                                                    </div>
                                                                </div>
                                                                <div class="d-inline-block">
                                                                    <h6 class="small text-uppercase font-weight-bold text-muted">Carry Over</h6>
                                                                    <h4 class="font-weight-bold m-0">${formatNumber(month.carryOver)}</h4>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col px-4 text-right">
                                                            <div class="d-flex align-items-center justify-content-end">
                                                                <div class="d-inline-block">
                                                                    <h6 class="small text-uppercase font-weight-bold text-muted">Carry Forward</h6>
                                                                    <h4 class="font-weight-bold m-0">${formatNumber(month.carryForward)}</h4>
                                                                </div>
                                                                <div class="d-inline-block ml-3">
                                                                    <div class="icon-circle">
                                                                        <i class="fas fa-chevron-right fa-sm"></i>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>`)

            // Contracts
            let monthlyItemContractContainer = monthlyItem.find("table tbody")
            let monthlyItemContracts = $(document.createDocumentFragment())
            month.salesContracts.forEach(contract => {
                $(`<tr>
                    <td class="font-weight-bold">${contract.salesContractName}</td>
                    <td class="text-center">${contract.type || "-" }</td>
                    <td class="text-center text-success">${formatNumber(contract.salesContractsPlanTotal)}</td>
                    <td class="text-center text-warning">${formatNumber(contract.salesContractsActualTotal)}</td>
                </tr>`).appendTo(monthlyItemContracts)
            })
            monthlyItemContracts.prependTo(monthlyItemContractContainer)


            // Append monthly-item to monthly-container
            monthlyItem.appendTo("#monthly-container")

            i++
        })

        
    }

    /* renderTotalData
     * render total (year) tables
     */
    const renderTotalData = (data) => {
        let totalData = data.salesPlanReportSalesContractTotals

        let total = $(`<div class="total mb-3">
            <div class="card card-mcs">
                <div class="card-body">

                    <div class="row">
                        <div class="col-md-6">
                            <h6 class="text-uppercase font-weight-bold small text-muted">2021</h6>
                            <h4 class="font-weight-bold">Januari - Desember</h4>
                        </div>
                    </div>


                    <div class="row mt-3">
                        <div class="col-md-12">
                            <table class="table table-striped table-bordered">
                                <thead>
                                    <tr class="table-active">
                                        <th class="small text-uppercase font-weight-bold text-muted">Contracted</th>
                                        <th class="small text-uppercase font-weight-bold text-muted text-center">Contract Type</th>
                                        <th class="small text-uppercase font-weight-bold text-muted text-center text-success">Plan (1 year)</th>
                                        <th class="small text-uppercase font-weight-bold text-muted text-center text-warning">Actual (1 year)</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="2" class="small text-uppercase font-weight-bold text-muted mt-2">Total</td>
                                        <td class="text-center font-weight-bold text-success">${formatNumber(data.totalPlanYearly)}</td>
                                        <td class="text-center font-weight-bold text-warning">${formatNumber(data.totalActualYearly)}</td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="small text-uppercase font-weight-bold text-muted mt-2">Target</td>
                                        <td class="text-center font-weight-bold">${formatNumber(data.totalTargetYearly)}</td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                </div>
            </div>
        </div>`)

        // Contracts
        let totalContractContainer = total.find("table tbody")
        let totalContracts = $(document.createDocumentFragment())
        totalData.forEach(contract => {
            $(`<tr>
                <td class="font-weight-bold">${contract.salesContractName}</td>
                <td class="text-center">${contract.type || "-" }</td>
                <td class="text-center text-success">${formatNumber(contract.salesContractsPlanTotal)}</td>
                <td class="text-center text-warning">${formatNumber(contract.salesContractsActualTotal)}</td>
            </tr>`).appendTo(totalContracts)
        })
        totalContracts.prependTo(totalContractContainer)


        // Append monthly-item to monthly-container
        total.appendTo("#total-container")

    }
});