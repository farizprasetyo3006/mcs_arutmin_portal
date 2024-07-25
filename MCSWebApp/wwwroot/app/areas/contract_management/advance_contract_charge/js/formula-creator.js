var totalLeftParentheses = 0;
var totalRightParentheses = 0;
var advance_contract_detail_id = "";
var price_index_id = "";
var equipment_usage_transaction_id = "";

var calculateParenthesis = function(str) {
    totalLeftParentheses = 0;
    totalRightParentheses = 0;
    for (let i = 0; i < str.length; i++) {
        if (str[i] == "(") {
            totalLeftParentheses++;
        }
        if (str[i] == ")") {
            totalRightParentheses++;
        }
    }
}

class FormulaCreator {
    constructor(params) {
        this.advance_contract_id = params.advanceContractId,
        this.formula = params.formula,
        this.employeesList = params.employeesList,
        this.employeesTasks = params.employeesTasks,
            this.advanceContractDetailRecords = params.advanceContractDetailRecords,
            this.equipmentUsageTransactionRecords = params.equipmentUsageTransactionRecords,
            this.priceIndexHistoryRecords = params.priceIndexHistoryRecords,
            this.jointSurveyRecords = params.jointSurveyRecords,
            this.additionalRecords = params.additionalRecords,
            this.productionClosingRecords = params.productionClosingRecords,
            this.dayWorkRecords = params.dayWorkRecords,
            this.functionsData = [
                {
                    id: 1,
                    value: "AVG()",
                    description: "Get the average value of given number \nAVG()"
                },
                {
                    id: 2,
                    value: "IF()",
                    description: "Conditional statement \nIF(logical_test, [value_if_true], [value_if_false])"
                },
                {
                    id: 3,
                    value: "Round()",
                    description: "Round a given number \nRound([number])"
                },
            
            ],
            this.variablesData = [
            {
                id: 4,
                value: "CurrentInvoiceUnitPrice()",
                description: "Get Current Invoice Unit Price \nCurrentInvoiceUnitPrice()"
            },
            {
                id: 5,
                value: "ash(adb).Value()",
                description: "Get Ash Adb value"
            },
            {
                id: 6,
                value: "ash(adb).Target()",
                description: "Get Ash Adb target"
            },
            {
                id: 7,
                value: "gcv(ar).Value()",
                description: "Get GCV AR value"
            },
            {
                id: 8,
                value: "gcv(ar).Target()",
                description: "Get GCV AR target"
            },
            {
                id: 9,
                value: "ts(adb).Value()",
                description: "Get TS Adb value"
            },
            {
                id: 10,
                value: "ts(adb).Target()",
                description: "Get TS Adb target"
            }
        ],
            this.operatorsData = ["+", "-", "*", "/" /*"=", "<", ">", "<=", ">=",*/],
            this.parenthesesData = ["()"],
            this.token = $.cookie("Token");

        this.formContainer
        this.popupOptions = {
            width: "80%",
            height: "auto",
            showTitle: true,
            title: "Formula Editor",
            visible: false,
            dragEnabled: false,
            hideOnOutsideClick: true,
            contentTemplate: function (e) {
                this.formContainer = $("<div>").dxForm({
                    formData: {
                        formula: this.formula,
                        description: ""
                    },
                    colCount: 3,
                    items: [
                        {
                            dataField: "formula",
                            label: {
                                text: "Formula"
                            },
                            colSpan: 2,
                            editorType: "dxTextArea",
                            editorOptions: {
                                readOnly: false,
                                height: 175,
                                onValueChanged: function (e) {
                                    //console.log("onValueChanged", e);
                                    //form.getEditor("firstName")
                                    //    .option("disabled", e.value);

                                    calculateParenthesis(e.value);
                                }
                            },
                        },
                        {
                            name: "numerical",
                            itemType: "group",
                            colCount: 4,
                            items: this.renderNumericalButtons()
                        },
                        {
                            dataField: "advance_contract_detail",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Advance Contract Detail"
                            },
                            colSpan: 1,
                            editorOptions: {
                                //dataSource: this.advanceContractDetailRecords,
                                //dataSource: new DevExpress.data.DataSource({
                                //    store: this.advanceContractDetailRecords,
                                //    key: "Subject",
                                //}),
                                dataSource: DevExpress.data.AspNet.createStore({
                                    //key: "reservedW",
                                    loadUrl: "/api/ContractManagement/AdvanceContractItemDetail/AdvanceContractItemDetailIdLookupByAdvanceContractId?advance_contract_id=" + this.advance_contract_id,
                                    onBeforeSend: (method, ajaxOptions) => {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = (request) => {
                                            request.setRequestHeader("Authorization", "Bearer " + this.token);
                                        };
                                    }
                                }),
                                //valueExpr: function (item) {
                                //    alert();
                                //    //console.log(item);
                                //    return item && item.Subject;
                                //},
                                displayExpr: function (item) {
                                    //console.log(item);
                                    return item && item.text;
                                },
                                searchExpr: ["variable"],
                                showScrollbar: "always",
                                //height: 200,
                                searchEnabled: true,
                                selectionMode: "single",
                                onItemClick: (e) => {
                                    var component = e.component,
                                        prevClickTime = component.lastClickTime,
                                        prevItemIndex = component.lastItemIndex
                                    component.lastClickTime = new Date();
                                    component.lastItemIndex = e.itemIndex.toString()

                                    //// Check first if item that previously clicked is the same with the 2nd click
                                    //if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                                    //    // Double click code 
                                    //    if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                                    //        //this.addToFormula(this.formContainer, e.itemData)
                                    //        this.addToFormula(this.formContainer, e.itemData.text, false, false);
                                    //        advance_contract_detail_id = e.itemData.value;
                                    //    }
                                    //    else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                                    //        DevExpress.ui.notify({
                                    //            message: "Double click to insert",
                                    //            type: "info",
                                    //            displayTime: 1000,
                                    //            maxWidth: "30%"
                                    //        });
                                    //    }
                                    //}

                                    this.addToFormula(this.formContainer, e.itemData.text, false, false);

                                    // Delete description
                                    this.formContainer.dxForm("instance").option("formData.description", "")
                                }
                            },
                        },
                        //{
                        //    dataField: "advance_contract_detail",
                        //    editorType: "dxSelectBox",
                        //    label: {
                        //        text: "Advance Contract Detail"
                        //    },
                        //    colSpan: 3,
                        //    editorOptions: {
                        //        dataSource: new DevExpress.data.DataSource({
                        //            store: this.advanceContractDetailRecords,
                        //            key: "ID",
                        //        }),
                        //        dropDownOptions: {
                        //            hideOnOutsideClick: true,
                        //            showTitle: false
                        //        },
                        //        displayExpr: function (item) {
                        //            return item && item.Subject;
                        //        },
                        //        onItemClick: (e) => {
                        //            this.addToFormula(this.formContainer, e.itemData.Subject, false);
                        //            advance_contract_detail_id = e.itemData.ID;
                        //            // Show description
                        //            this.formContainer.dxForm("instance").option("formData.description", e.itemData.Amount)
                        //        }
                        //    },
                        //},
                        {
                            dataField: "price_index",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Price Index"
                            },
                            colSpan: 3,
                            editorOptions: {
                                dataSource: new DevExpress.data.DataSource({
                                    store: this.priceIndexHistoryRecords,
                                    key: "ID",
                                    group: "PriceIndexCode"
                                }),
                                dropDownOptions: {
                                    hideOnOutsideClick: true,
                                    showTitle: false
                                },
                                grouped: true,
                                displayExpr: function (item) {
                                    return item && item.PriceIndexName;
                                },
                                onItemClick: (e) => {
                                    this.addToFormula(this.formContainer, e.itemData.PriceIndexName, false, false);
                                    price_index_id = e.itemData.ID;
                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.PriceIndexName)
                                }
                            },
                        },
                        
                        {
                            dataField: "advance_contract_detail",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Price Index Detail"
                            },
                            colSpan: 1,
                            editorOptions: {
                                //dataSource: this.advanceContractDetailRecords,
                                //dataSource: new DevExpress.data.DataSource({
                                //    store: this.advanceContractDetailRecords,
                                //    key: "Subject",
                                //}),
                                dataSource: DevExpress.data.AspNet.createStore({
                                    //key: "reservedW",
                                    loadUrl: "/api/General/PriceIndexMap/GetPriceIndexMapByBaseRate",
                                    onBeforeSend: (method, ajaxOptions) => {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = (request) => {
                                            request.setRequestHeader("Authorization", "Bearer " + this.token);
                                        };
                                    }
                                }),
                                displayExpr: function (item) {
                                    //console.log(item);
                                    return item && item.name;
                                },
                                searchExpr: ["variable"],
                                showScrollbar: "always",
                                //height: 200,
                                searchEnabled: true,
                                selectionMode: "single",
                                onItemClick: (e) => {
                                    var component = e.component,
                                        prevClickTime = component.lastClickTime,
                                        prevItemIndex = component.lastItemIndex
                                    component.lastClickTime = new Date();
                                    component.lastItemIndex = e.itemIndex.toString()

                                    //// Check first if item that previously clicked is the same with the 2nd click
                                    //if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                                    //    // Double click code 
                                    //    if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                                    //        //this.addToFormula(this.formContainer, e.itemData)
                                    //        this.addToFormula(this.formContainer, e.itemData.name, false, false);
                                    //    }
                                    //    else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                                    //        DevExpress.ui.notify({
                                    //            message: "Double click to insert",
                                    //            type: "info",
                                    //            displayTime: 1000,
                                    //            maxWidth: "30%"
                                    //        });
                                    //    }
                                    //}

                                    this.addToFormula(this.formContainer, e.itemData.name, false, false);
                                    // Delete description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.name)
                                }
                            },
                        },

                        {
                            dataField: "jointSurveyRecords",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Join Survey"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: new DevExpress.data.DataSource({
                                    store: this.jointSurveyRecords,
                                    key: "ID",
                                }),
                                dropDownOptions: {
                                    hideOnOutsideClick: true,
                                    showTitle: false
                                },
                                displayExpr: function (item) {
                                    return item && item.Text;
                                },
                                onItemClick: (e) => {
                                     
                                    this.addToFormula(this.formContainer, e.itemData.Text, false, false);
                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.Text)
                                }
                            },
                        },
                        {
                            dataField: "productionClosingRecords",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Production Closing"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: new DevExpress.data.DataSource({
                                    store: this.productionClosingRecords,
                                    key: "ID",
                                }),
                                dropDownOptions: {
                                    hideOnOutsideClick: true,
                                    showTitle: false
                                },
                                displayExpr: function (item) {
                                    return item && item.Text;
                                },
                                onItemClick: (e) => {
                                     
                                    this.addToFormula(this.formContainer, e.itemData.Text, false, false);
                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.Text)
                                }
                            },
                        },
                        {
                            dataField: "dayWorkRecords",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Day Work"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: new DevExpress.data.DataSource({
                                    store: this.dayWorkRecords,
                                    key: "ID",
                                }),
                                dropDownOptions: {
                                    hideOnOutsideClick: true,
                                    showTitle: false
                                },
                                displayExpr: function (item) {
                                    return item && item.Text;
                                },
                                onItemClick: (e) => {
                                     
                                    this.addToFormula(this.formContainer, e.itemData.Text, false, false);
                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.Text)
                                }
                            },
                        },
                        {
                            dataField: "equipment_usage_transaction_id",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Equipment Usage Transaction"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: new DevExpress.data.DataSource({
                                    store: this.equipmentUsageTransactionRecords,
                                    key: "ID",
                                }),
                                dropDownOptions: {
                                    hideOnOutsideClick: true,
                                    showTitle: false
                                },
                                displayExpr: function (item) {
                                    return item && item.Subject;
                                },
                                onItemClick: (e) => {
                                     
                                    this.addToFormula(this.formContainer, e.itemData.Subject, false, false);
                                    equipment_usage_transaction_id = e.itemData.ID;
                                    //console.log(equipment_usage_transaction_id);
                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.Subject)
                                }
                            },
                        },

                        {
                            dataField: "additional_formula",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Additional"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: new DevExpress.data.DataSource({
                                    store: this.additionalRecords,
                                    key: "ID",
                                }),
                                dropDownOptions: {
                                    hideOnOutsideClick: true,
                                    showTitle: false
                                },
                                displayExpr: function (item) {
                                    return item && item.Subject;
                                },
                                onItemClick: (e) => {
                                     
                                    this.addToFormula(this.formContainer, e.itemData.Subject, false, false);
                                    advance_contract_detail_id = e.itemData.ID;
                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.Amount)
                                }
                            },
                        },

                        {
                            dataField: "advance_contract_detail",
                            editorType: "dxSelectBox",
                            label: {
                                text: "Formula"
                            },
                            colSpan: 3,
                            editorOptions: {
                                //dataSource: this.advanceContractDetailRecords,
                                //dataSource: new DevExpress.data.DataSource({
                                //    store: this.advanceContractDetailRecords,
                                //    key: "Subject",
                                //}),
                                dataSource: DevExpress.data.AspNet.createStore({
                                    //key: "reservedW",
                                    /*loadUrl: "/api/General/PriceIndexMap/GetPriceIndexMapByBaseRate",*/
                                    loadUrl: "/api/ContractManagement/AdvanceContractCharge/GetBaseFormulaByContractID?advance_contract_id=" + this.advance_contract_id,
                                    onBeforeSend: (method, ajaxOptions) => {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = (request) => {
                                            request.setRequestHeader("Authorization", "Bearer " + this.token);
                                        };
                                    }
                                }),
                                displayExpr: function (item) {
                                    ////console.log(item);
                                    return item && '$'+item.charge_name;
                                },
                                searchExpr: ["variable"],
                                showScrollbar: "always",
                                //height: 200,
                                searchEnabled: true,
                                selectionMode: "single",
                                onItemClick: (e) => {
                                    var component = e.component,
                                        prevClickTime = component.lastClickTime,
                                        prevItemIndex = component.lastItemIndex
                                    component.lastClickTime = new Date();
                                    component.lastItemIndex = e.itemIndex.toString()

                                    //// Check first if item that previously clicked is the same with the 2nd click
                                    //if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                                    //    // Double click code 
                                    //    if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                                    //        //this.addToFormula(this.formContainer, e.itemData)
                                    //        this.addToFormula(this.formContainer, e.itemData.name, false, false);
                                    //    }
                                    //    else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                                    //        DevExpress.ui.notify({
                                    //            message: "Double click to insert",
                                    //            type: "info",
                                    //            displayTime: 1000,
                                    //            maxWidth: "30%"
                                    //        });
                                    //    }
                                    //}

                                    this.addToFormula(this.formContainer, '$'+e.itemData.charge_name, false, false);
                                    // Delete description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.name)
                                }
                            },
                        },

                        //{
                        //    dataField: "operators",
                        //    editorType: "dxList",
                        //    label: {
                        //        text: "Operators"
                        //    },
                        //    colSpan: 1,
                        //    editorOptions: {
                        //        dataSource: this.operatorsData,
                        //        showScrollbar: "always",
                        //        height: 200,
                        //        searchEnabled: true,
                        //        selectionMode: "single",
                        //        onItemClick: (e) => {
                        //            var component = e.component,
                        //                prevClickTime = component.lastClickTime,
                        //                prevItemIndex = component.lastItemIndex
                        //            component.lastClickTime = new Date();
                        //            component.lastItemIndex = e.itemIndex.toString()

                        //            // Check first if item that previously clicked is the same with the 2nd click
                        //            if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                        //                // Double click code 
                        //                if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                        //                    this.addToFormula(this.formContainer, e.itemData)
                        //                }
                        //                else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                        //                    DevExpress.ui.notify({
                        //                        message: "Double click to insert",
                        //                        type: "info",
                        //                        displayTime: 1000,
                        //                        maxWidth: "30%"
                        //                    });
                        //                }
                        //            }

                        //            // Delete description
                        //            this.formContainer.dxForm("instance").option("formData.description", "")
                        //        }
                        //    },
                        //},
                        //{
                        //    dataField: "parentheses",
                        //    editorType: "dxList",
                        //    label: {
                        //        text: "Parentheses"
                        //    },
                        //    colSpan: 1,
                        //    editorOptions: {
                        //        dataSource: this.parenthesesData,
                        //        showScrollbar: "always",
                        //        height: 200,
                        //        searchEnabled: true,
                        //        selectionMode: "single",
                        //        onItemClick: (e) => {
                        //            var component = e.component,
                        //                prevClickTime = component.lastClickTime,
                        //                prevItemIndex = component.lastItemIndex
                        //            component.lastClickTime = new Date();
                        //            component.lastItemIndex = e.itemIndex.toString()

                        //            // Check first if item that previously clicked is the same with the 2nd click
                        //            if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                        //                // Double click code 
                        //                if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                        //                    this.addToFormula(this.formContainer, e.itemData)
                        //                }
                        //                else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                        //                    DevExpress.ui.notify({
                        //                        message: "Double click to insert",
                        //                        type: "info",
                        //                        displayTime: 1000,
                        //                        maxWidth: "30%"
                        //                    });
                        //                }
                        //            }

                        //            // Delete description
                        //            this.formContainer.dxForm("instance").option("formData.description", "")
                        //        }
                        //    },
                        //},
                        {
                            dataField: "description",
                            label: {
                                text: "Description"
                            },
                            colSpan: 3,
                            editorType: "dxTextArea",
                            editorOptions: {
                                readOnly: true,
                                height: 50,
                            },
                        },
                        {
                            itemType: "button",
                            colSpan: 3,
                            horizontalAlignment: "right",
                            buttonOptions: {
                                text: "Save",
                                type: "secondary",
                                useSubmitBehavior: true,
                                onClick: () => {
                                    let formData = this.formContainer.dxForm("instance").option("formData")

                                    if (totalLeftParentheses != totalRightParentheses) {
                                        alert("Total Left Parentheses is not equal with Total Right Parenthesis");
                                        return;
                                    }
                                    params.saveFormulaCallback(formData.formula, advance_contract_detail_id, price_index_id, equipment_usage_transaction_id)
                                    this.close()
                                }
                            }
                        }
                    ],
                })
                e.append(this.formContainer)
            },
        }
        this.popup = $("<div>").dxPopup(this.popupOptions).appendTo("body").dxPopup("instance")
        this.open()
    }
    open() {
        this.popup.option("contentTemplate", this.popupOptions.contentTemplate.bind(this));
        this.popup.show()
    }
    close() {
        this.popup.hide()
    }
        getFormulaCaret(form) {
            let formulaContainerElement = form.getEditor('formula').element()
            let formulaTextArea = formulaContainerElement.find("textarea").get(0)

            return formulaTextArea.selectionStart
        }
    setFormulaCaret(form, position) {
        let formulaContainerElement = form.getEditor('formula').element()
        let formulaTextArea = formulaContainerElement.find("textarea").get(0)

        formulaTextArea.selectionEnd = position
        formulaTextArea.focus()
    }

    renderNumericalButtons() {
        let buttons = []
        let data = ["7", "8", "9", "x", "4", "5", "6", "/", "1", "2", "3", "+", ".", "0", "-", "(", ")", "CLEAR"]

        data.forEach((i) => {
            let colSpan = 1;
            if (i == "0") {
                colSpan = 2;
            } else if (i == "ROUND(X,Y)") {
                colSpan = 2;
            } else if (i == "CLEAR") {
                colSpan = 2;
            }
            //let colSpan = 1;

            buttons.push({
                itemType: "button",
                colSpan: colSpan,
                buttonOptions: {
                    text: i,
                    width: "100%",
                    type: "primary",
                    onClick: (e) => {
                        //
                        var res = i;
                        if (i == "x") {
                            res = "*";
                        }

                        var isClear = false;
                        if (i == "CLEAR") {
                            isClear = true;
                        }

                        var isBackspace = false;
                        if (i === "Del") {
                            isBackspace = true;
                        }


                        this.addToFormula(this.formContainer, res, isClear, isBackspace);

                        // Delete description
                        this.formContainer.dxForm("instance").option("formData.description", "")
                    }
                }
            })
        })

        return buttons
    }

    addToFormula(formContainer, string, isclear, isBackspace) {
        let form = formContainer.dxForm("instance");
        let formula = form.option("formData.formula");

        // Get formula textarea cursor position
        let formulaCaret = this.getFormulaCaret(form);

        let formulaBeforeCaret = "";
        let formulaAfterCaret = "";
        let newFormula = "";

        if (isclear) {
            form.option("formData.formula", newFormula)
            this.setFormulaCaret(form, newFormula.length)
        } else {
           
            if (typeof formula === "undefined") {
                newFormula = string + "";
            } else {
                formulaBeforeCaret = formula.substring(0, formulaCaret);
                formulaAfterCaret = formula.substring(formulaCaret, formula.length);
                newFormula = formulaBeforeCaret + string + formulaAfterCaret;
            }

            // Combine function to the formula based on caret position
            // Update new caret position after formulaBeforeCaret.length + new content length
            form.option("formData.formula", newFormula)
            if (typeof formula === "undefined") {
                this.setFormulaCaret(form, newFormula.length)
            } else {
                this.setFormulaCaret(form, formulaBeforeCaret.length + string.length)
            }
        }

    }
}
