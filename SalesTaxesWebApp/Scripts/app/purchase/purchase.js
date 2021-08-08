function purchaseViewModel(endpoints, confirmDeleteMessage, isFormPosted, debug, addItemMessage) {
    var self = this;
    self.endpoints = endpoints;
    self.debug = ((debug || false) && typeof console !== "undefined");
    self.confirmDeleteMessage = confirmDeleteMessage;
    self.isFormPosted = isFormPosted;
    self.hasAddedItems = isFormPosted;
    self.addItemMessage = addItemMessage;

    self.addItem = function (callBackFunctionAfterAjax) {
        var lineItem = {};
        lineItem.ItemName = $("#newItemName").val();
        lineItem.ListPrice = $("#newListPrice").val();
        lineItem.Quantity = $("#newQuantity").val();
        lineItem.IsImported = $("#isImportedNewSwitch").is(":checked");
        lineItem.IsTaxExempted = $("#isTaxExemptedNewSwitch").is(":checked");

        if (lineItem.ItemName) {
            $("#newItemName").parent().removeClass("has-error");
        } else {
            $("#newItemName").parent().addClass("has-error");
        }

        if (isNaN(parseFloat(lineItem.ListPrice)) || (parseFloat(lineItem.ListPrice)) === 0.0) {
            $("#newListPrice").parent().addClass("has-error");
        } else {
            $("#newListPrice").parent().removeClass("has-error");
        }

        if (isNaN(parseInt(lineItem.Quantity)) || (parseInt(lineItem.Quantity)) === 0) {
            $("#newQuantity").parent().addClass("has-error");
        } else {
            $("#newQuantity").parent().removeClass("has-error");
        }

        if ($(".new-line .has-error").length === 0) {
            $.ajax({
                type: "POST",
                url: self.endpoints.addItem,
                data: lineItem,
                dataType: "html",
                cache: false
            }).then(function (view) {
                $("#tblAddItems tbody").append(view);
                $('.expected-response-switch').bootstrapSwitch({
                    onText: "Yes",
                    offText: "No",
                    size: "small"
                });
                $("#newItemName").val("please enter name");
                $("#newListPrice").val(0.0001);
                $("#newQuantity").val(1);
                self.hasAddedItems = true;
                reorderItemTable("tblAddItems", "Items");
                $("#tblAddItems tbody").find(".itemName:last").focus();
                $("#tblAddItems").trigger("lineAdded");

                determinePlaceOrderClickable();
            }).then(callBackFunctionAfterAjax);
        } else if (typeof callBackFunctionAfterAjax !== "undefined") {
            callBackFunctionAfterAjax();

        }
    };

    self.placeOrder = function () {
        if (self.hasAddedItems) {          
            if ($("#formPurchaseInfo").valid()) {
                $("#loadingIcon").show();
                $("#formPurchaseInfo").submit();
            }
        } else {
            alert(addItemMessage);
        }
    };

    self.deleteItem = function (el) {
        var confirmDelete = confirm(self.confirmDeleteMessage);
        if (confirmDelete) {
            var row = el.parents("tr:first");
            row.remove();
            if (self.debug) {
                console.log("Removed new item");
            }
            reorderItemTable("tblAddItems", "Items");
            if (self.isFormPosted) {
                self.placeOrder();
            }

            if ($("#tblAddItems").find(".line-item").length === 0 && !self.isFormPosted) {
                self.hasAddedItems = false;
            }

            determinePlaceOrderClickable();
        }

    };

    self.clearOrder = function () {
        $("#DoClearModel").val("True");
        $("#formPurchaseInfo").submit();

    };


    self.rebindLineEvents = function () {
        if (this.debug) {
            console.log("rebindLineEvents");
        }

        $(".btnItem-Remove").off("click").on("click", function () {
            self.deleteItem($(this));
        });

        if (self.isFormPosted) {
            self.placeOrder();
        }
    };

    self.init = function () {
        $('.expected-response-switch').bootstrapSwitch({
            onText: "Yes",
            offText: "No",
            size: "small"
        });
        $(".new-expected-response-switch").bootstrapSwitch({
            onText: "Yes",
            offText: "No",
            size: "small"
        });

        $("#btnAddItem").off().on("click", function () {
            self.addItem();
        });

        $("#btnPlaceOrder").off().on("click", function (e) {
            if ($('tr.line-item').length < 1) {
                e.preventDefault();
                self.addItem(function () {
                    if ($("#btnPlaceOrder").hasClass('disabled')) {
                        $('#btnAddItem').addClass('shake');
                        setTimeout(function () {
                            $('#btnAddItem').removeClass('shake');
                        }, 1000);

                        return;
                    }

                    self.placeOrder();
                });


            } else {
                self.placeOrder();
            }

        });

        $("#btnClearOrder").off().on("click", function () {
            self.clearOrder();
        });

        $("input.itemName").change(function () {
            if (self.isFormPosted) {
                self.placeOrder();
            }
        });

        $("input.listPrice").change(function () {
            if (self.isFormPosted) {
                self.placeOrder();
            }
        });

        $("input.quantity").change(function () {
            if (self.isFormPosted) {
                self.placeOrder();
            }
        });

        $(".expected-response-switch").on("change.bootstrapSwitch", function (e, state) {
            if (self.isFormPosted) {
                self.placeOrder();
            }
        });

        $(".btnItem-Remove").off("click").on("click", function () {
            self.deleteItem($(this));
        });

        $("#tblAddItems").bind("lineAdded", function () {
            self.rebindLineEvents();
        });
    }

};

function reorderItemTable(tableId, viewModelItemPrefix) {
    $("#" + tableId + " tbody tr").not(".new-line").each(function (rowIndex, rowObject) {
        $(rowObject).find("input, select").each(function () {
            var field = $(this).attr("name");
            var newField = viewModelItemPrefix + "[" + rowIndex + "]" + field.substr(field.lastIndexOf("]") + 1);
            $(this).attr({ name: newField, id: newField.replace(/\./g, "_") });
        });
        $(rowObject).find("button").each(function () {
            var buttonPrefix = "btnItemRemove_";
            var newButtonId = buttonPrefix + rowIndex;
            $(this).attr("id", newButtonId);
        });
    });
}


function determinePlaceOrderClickable() {
    if ($('tr.line-item').length < 1) {
        $('#btnPlaceOrder').addClass("disabled");
    } else {
        $('#btnPlaceOrder').removeClass('disabled');
    }
}