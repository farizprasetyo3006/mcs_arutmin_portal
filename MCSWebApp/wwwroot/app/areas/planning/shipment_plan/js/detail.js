$(document).ready(function () {
    /*var salesPlanSnapshotId = document.querySelector("[name=sales_plan_snapshot_id]").value*/
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

    let salesPlanSnapshotForm = $("#sales-plan-snapshot-form").dxForm({
        formData: {
            snapshot_name: "",
            sales_plan_id: "",
            notes: ""
        },
        colCount: 2,
        items: [
            {
                dataField: "snapshot_name",
                label: {
                    text: "Snapshot Name"
                },
                editorOptions: {
                    disabled: true
                }
            },
            {
                dataField: "sales_plan",
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
                    disabled: true
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
                    height: 50,
                    disabled: true
                },
            },
            {
                itemType: "empty"
            },
        ],
        onInitialized: function () {
            // Get sales plan snapshot data if has salesContractProductId
            /*if (salesPlanSnapshotId) {
                getSalesContractProductHeader()
            }*/
        }
    }).dxForm("instance");

    /* renderMonthlyData
     * render all monthly data and tables
     */
    const renderMonthlyData = () => {
        let monthlyData = sampleSnapshotData.monthly.data

        let i = 1;
        monthlyData.forEach(month => {

            let monthlyItem = $(`<div class="monthly-item mb-3">
                            <div class="card card-mcs">
                                <div class="card-body">

                                    <div class="item-head">
                                        <div class="row" data-toggle="collapse" data-target="#monthly-item-${month.month_index}" aria-expanded="true" aria-controls="collapseExample">
                                            <div class="col-md-6">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">${month.month_index < 10 ? "0" + month.month_index : month.month_index}</h6>
                                                <h4 class="font-weight-bold">${month.month_name}</h4>
                                            </div>
                                            <div class="col-md-2">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">Target</h6>
                                                <h4 class="font-weight-bold">${formatNumber(month.target)}</h4>
                                            </div>
                                            <div class="col-md-2">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">Total Planned</h6>
                                                <h4 class="font-weight-bold">${formatNumber(month.totalPlanned)}</h4>
                                            </div>
                                            <div class="col-md-2">
                                                <h6 class="text-uppercase font-weight-bold small text-muted">Total Actual</h6>
                                                <h4 class="font-weight-bold">${formatNumber(month.totalActual)}</h4>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="collapse ${i == 1 && 'show'}" id="monthly-item-${month.month_index}">
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
                                                                <td class="text-center font-weight-bold text-success">${formatNumber(month.totalPlanned)}</td>
                                                                <td class="text-center font-weight-bold text-warning">${formatNumber(month.totalActual)}</td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" class="small text-uppercase font-weight-bold text-muted mt-2">Target</td>
                                                                <td class="text-center font-weight-bold">${formatNumber(month.target)}</td>
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
            month.contracts.forEach(contract => {
                $(`<tr>
                    <td class="font-weight-bold">${contract.name}</td>
                    <td class="text-center">${contract.type}</td>
                    <td class="text-center text-success">${formatNumber(contract.planned)}</td>
                    <td class="text-center text-warning">${formatNumber(contract.actual)}</td>
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
    const renderTotalData = () => {
        let totalData = sampleSnapshotData.total.data

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
                                        <td class="text-center font-weight-bold text-success">11.040.000</td>
                                        <td class="text-center font-weight-bold text-warning">8.069.329</td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="small text-uppercase font-weight-bold text-muted mt-2">Target</td>
                                        <td class="text-center font-weight-bold">12.000.000</td>
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
        totalData.contracts.forEach(contract => {
            $(`<tr>
                <td class="font-weight-bold">${contract.name}</td>
                <td class="text-center">${contract.type}</td>
                <td class="text-center text-success">${formatNumber(contract.planned)}</td>
                <td class="text-center text-warning">${formatNumber(contract.actual)}</td>
            </tr>`).appendTo(totalContracts)
        })
        totalContracts.prependTo(totalContractContainer)


        // Append monthly-item to monthly-container
        total.appendTo("#total-container")

    }

    renderMonthlyData();
    renderTotalData();
});