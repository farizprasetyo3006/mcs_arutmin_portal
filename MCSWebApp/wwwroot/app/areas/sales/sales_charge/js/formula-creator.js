class FormulaCreator {
    constructor(params) {
        this.formula = params.formula
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
        this.operatorsData = ["+", "-", "*", "/", "=", "<", ">", "<=", ">=", "(", ")"],
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
                    colCount: 4,
                    items: [
                        {
                            dataField: "formula",
                            label: {
                                text: "Formula"
                            },
                            colSpan: 3,
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 175
                            },
                        },
                        {
                            name: "numerical",
                            itemType: "group",
                            colCount: 3,
                            colSpan: 1,
                            items: this.renderNumericalButtons()
                        },
                        {
                            dataField: "variable",
                            editorType: "dxList",
                            label: {
                                text: "Variables"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "reservedW",
                                    loadUrl: "/api/Sales/SalesInvoice/GetReservedWords",
                                    onBeforeSend: (method, ajaxOptions) => {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = (request) => {
                                            request.setRequestHeader("Authorization", "Bearer " + this.token);
                                        };
                                    }
                                }),
                                valueExpr: "reservedW",
                                displayExpr: "reservedW",
                                showScrollbar: "always",
                                height: 200,
                                searchEnabled: true,
                                searchExpr: ["reservedW", "description"],
                                selectionMode: "single",
                                onItemClick: (e) => {
                                    var component = e.component,
                                        prevClickTime = component.lastClickTime,
                                        prevItemIndex = component.lastItemIndex
                                    component.lastClickTime = new Date();
                                    component.lastItemIndex = e.itemIndex.toString()

                                    // Check first if item that previously clicked is the same with the 2nd click
                                    // Ref: https://supportcenter.devexpress.com/ticket/details/t143438/datagrid-provide-the-row-double-click-event#
                                    if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                                        // Double click code 
                                        if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                                            this.addToFormula(this.formContainer, e.itemData.reservedW)
                                        }
                                        else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                                            DevExpress.ui.notify({
                                                message: "Double click to insert",
                                                type: "info",
                                                displayTime: 1000,
                                                maxWidth: "30%"
                                            });
                                        }
                                    }

                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.description)
                                }
                            },
                        },
                        {
                            dataField: "function",
                            editorType: "dxList",
                            label: {
                                text: "Function"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "reservedW",
                                    loadUrl: "/api/Sales/SalesInvoice/GetReservedFunctions",
                                    onBeforeSend: (method, ajaxOptions) => {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = (request) => {
                                            request.setRequestHeader("Authorization", "Bearer " + this.token);
                                        };
                                    }
                                }),
                                valueExpr: "reservedW",
                                displayExpr: "reservedW",
                                showScrollbar: "always",
                                height: 200,
                                searchEnabled: true,
                                searchExpr: ["reservedW", "description"],
                                selectionMode: "single",
                                onItemClick: (e) => {
                                    var component = e.component,
                                        prevClickTime = component.lastClickTime,
                                        prevItemIndex = component.lastItemIndex
                                    component.lastClickTime = new Date();
                                    component.lastItemIndex = e.itemIndex.toString()

                                    // Check first if item that previously clicked is the same with the 2nd click
                                    if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                                        // Double click code 
                                        if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                                            this.addToFormula(this.formContainer, e.itemData.reservedW)
                                        }
                                        else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                                            DevExpress.ui.notify({
                                                message: "Double click to insert",
                                                type: "info",
                                                displayTime: 1000,
                                                maxWidth: "30%"
                                            });
                                        }
                                    }

                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.description)
                                }
                            },
                        },

                        {
                            dataField: "royalty",
                            editorType: "dxList",
                            label: {
                                text: "Royalty"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: DevExpress.data.AspNet.createStore({
                                    key: "reservedW",
                                    loadUrl: "/api/Sales/Royalty/Royalty/GetRoyaltyAnalytes",
                                    onBeforeSend: (method, ajaxOptions) => {
                                        ajaxOptions.xhrFields = { withCredentials: true };
                                        ajaxOptions.beforeSend = (request) => {
                                            request.setRequestHeader("Authorization", "Bearer " + this.token);
                                        };
                                    }
                                }),
                                valueExpr: "reservedW",
                                displayExpr: "reservedW",
                                showScrollbar: "always",
                                height: 200,
                                searchEnabled: true,
                                searchExpr: ["reservedW", "description"],
                                selectionMode: "single",
                                onItemClick: (e) => {
                                    var component = e.component,
                                        prevClickTime = component.lastClickTime,
                                        prevItemIndex = component.lastItemIndex
                                    component.lastClickTime = new Date();
                                    component.lastItemIndex = e.itemIndex.toString()

                                    // Check first if item that previously clicked is the same with the 2nd click
                                    // Ref: https://supportcenter.devexpress.com/ticket/details/t143438/datagrid-provide-the-row-double-click-event#
                                    if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                                        // Double click code 
                                        if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                                            this.addToFormula(this.formContainer, e.itemData.reservedW)
                                        }
                                        else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                                            DevExpress.ui.notify({
                                                message: "Double click to insert",
                                                type: "info",
                                                displayTime: 1000,
                                                maxWidth: "30%"
                                            });
                                        }
                                    }

                                    // Show description
                                    this.formContainer.dxForm("instance").option("formData.description", e.itemData.description)
                                }
                            },
                        },

                        {
                            dataField: "operators",
                            editorType: "dxList",
                            label: {
                                text: "Operators"
                            },
                            colSpan: 1,
                            editorOptions: {
                                dataSource: this.operatorsData,
                                showScrollbar: "always",
                                height: 200,
                                searchEnabled: true,
                                selectionMode: "single",
                                onItemClick: (e) => {
                                    var component = e.component,
                                        prevClickTime = component.lastClickTime,
                                        prevItemIndex = component.lastItemIndex
                                    component.lastClickTime = new Date();
                                    component.lastItemIndex = e.itemIndex.toString()

                                    // Check first if item that previously clicked is the same with the 2nd click
                                    if (prevItemIndex && (prevItemIndex == component.lastItemIndex)) {
                                        // Double click code 
                                        if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
                                            this.addToFormula(this.formContainer, e.itemData)
                                        }
                                        else if (prevClickTime && (component.lastClickTime - prevClickTime < 1500)) {
                                            DevExpress.ui.notify({
                                                message: "Double click to insert",
                                                type: "info",
                                                displayTime: 1000,
                                                maxWidth: "30%"
                                            });
                                        }
                                    }

                                    // Delete description
                                    this.formContainer.dxForm("instance").option("formData.description", "")
                                }
                            },
                        },
                        {
                            dataField: "description",
                            label: {
                                text: "Description"
                            },
                            colSpan: 3,
                            editorType: "dxTextArea",
                            editorOptions: {
                                readOnly: true,
                                height: 50
                            },
                        },
                        {
                            dataField: "syntax_status",
                            label: {
                                text: "Syntax Status"
                            },
                            editorOptions: {
                                readOnly: true
                            },
                            dataType: "string",
                            colSpan: 1
                        },
                        {
                            itemType: "button",
                            horizontalAlignment: "left",
                            buttonOptions: {
                                text: "Check Syntax",
                                type: "secondary",
                                useSubmitBehavior: true,
                                onClick: () => {
                                    let formData = this.formContainer.dxForm("instance").option("formData")
                                    this.checkSyntax(formData.formula)
                                }
                            }
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

                                    params.saveFormulaCallback(formData.formula)
                                    this.close()
                                }
                            }
                        }
                    ],
                })
                e.append(this.formContainer)
            }
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
        let data = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "."]

        data.forEach((i) => {
            let colSpan = (i == "0" ? 2 : 1)

            buttons.push({
                itemType: "button",
                colSpan: colSpan,
                buttonOptions: {
                    text: i,
                    width: "100%",
                    type: "primary",
                    onClick: () => {
                        this.addToFormula(this.formContainer, i)

                        // Delete description
                        this.formContainer.dxForm("instance").option("formData.description", "")
                    }
                }
            })
        })

        return buttons
    }

    addToFormula(formContainer, string) {
        let form = formContainer.dxForm("instance")
        let formula = form.option("formData.formula")

        let newFormula = ''

        if (formula) {
            // Get formula textarea cursor position
            let formulaCaret = this.getFormulaCaret(form)

            let formulaBeforeCaret = formula.substring(0, formulaCaret)
            let formulaAfterCaret = formula.substring(formulaCaret, formula.length)

            // Combine function to the formula based on caret position
            newFormula = formulaBeforeCaret + string + formulaAfterCaret

            // Update new caret position after formulaBeforeCaret.length + new content length
            this.setFormulaCaret(form, formulaBeforeCaret.length + string.length)
        }
        else {
            newFormula = string

            // Update new caret position after formulaBeforeCaret.length + new content length
            this.setFormulaCaret(form, string.length)
        }


        // Add new string to formula
        form.option("formData.formula", newFormula)
    }

    checkSyntax(formula) {
        let formContainer = this.formContainer;
        var cekStatus = null;

        $.ajax({
            url: '/Api/Sales/SalesInvoice/CheckSyntaxFormula?tsyntax=' + formula,
            type: 'GET',
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + $.cookie("Token"));
            },
            success: function (response) {
                //formContainer.dxForm("instance").option("formData.syntax_status", response.status);
                cekStatus = response.status;

                if (cekStatus == "OK") {
                    formContainer.dxForm("instance").option("formData.syntax_status", cekStatus);
                }
                else {
                    $.ajax({
                        url: '/Api/Sales/Royalty/Royalty/CheckSyntaxFormula?tsyntax=' + encodeURIComponent(formula),
                        type: 'GET',
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie("Token"));
                        },
                        success: function (response) {
                            formContainer.dxForm("instance").option("formData.syntax_status", response.status)
                        }
                    });

                }
            }
        });
    }
}
