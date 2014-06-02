/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.app.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Spiro;
(function (Spiro) {
    (function (Angular) {
        var AttachmentViewModel = (function () {
            function AttachmentViewModel() {
            }
            AttachmentViewModel.create = function (href, mimeType, title) {
                var attachmentViewModel = new AttachmentViewModel();

                attachmentViewModel.href = href;
                attachmentViewModel.mimeType = mimeType;
                attachmentViewModel.title = title || "UnknownFile";

                return attachmentViewModel;
            };
            return AttachmentViewModel;
        })();
        Angular.AttachmentViewModel = AttachmentViewModel;

        var ChoiceViewModel = (function () {
            function ChoiceViewModel() {
            }
            ChoiceViewModel.create = function (value, id, name, searchTerm) {
                var choiceViewModel = new ChoiceViewModel();

                choiceViewModel.id = id;
                choiceViewModel.name = name || value.toString();
                choiceViewModel.value = value.isReference() ? value.link().href() : value.toValueString();
                choiceViewModel.search = searchTerm || choiceViewModel.name;

                choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.value);
                return choiceViewModel;
            };

            ChoiceViewModel.prototype.equals = function (other) {
                return this.id === other.id && this.name === other.name && this.value === other.value;
            };

            ChoiceViewModel.prototype.match = function (other) {
                if (this.isEnum) {
                    return this.value.trim() === other.value.trim();
                }
                return this.search.trim() == other.search.trim();
            };
            return ChoiceViewModel;
        })();
        Angular.ChoiceViewModel = ChoiceViewModel;

        var ErrorViewModel = (function () {
            function ErrorViewModel() {
            }
            return ErrorViewModel;
        })();
        Angular.ErrorViewModel = ErrorViewModel;

        var LinkViewModel = (function () {
            function LinkViewModel() {
            }
            return LinkViewModel;
        })();
        Angular.LinkViewModel = LinkViewModel;

        var ItemViewModel = (function () {
            function ItemViewModel() {
            }
            return ItemViewModel;
        })();
        Angular.ItemViewModel = ItemViewModel;

        var MessageViewModel = (function () {
            function MessageViewModel() {
            }
            MessageViewModel.prototype.clearMessage = function () {
                this.message = "";
            };
            return MessageViewModel;
        })();
        Angular.MessageViewModel = MessageViewModel;

        var ValueViewModel = (function (_super) {
            __extends(ValueViewModel, _super);
            function ValueViewModel() {
                _super.apply(this, arguments);
            }
            ValueViewModel.prototype.setSelectedChoice = function () {
            };

            ValueViewModel.prototype.prompt = function (searchTerm) {
                return null;
            };

            ValueViewModel.prototype.conditionalChoices = function (args) {
                return null;
            };

            ValueViewModel.prototype.getMemento = function () {
                if (this.hasChoices) {
                    if (this.isMultipleChoices) {
                        var ss = _.map(this.multiChoices, function (c) {
                            return c.search;
                        });

                        return _.reduce(ss, function (m, s) {
                            return m + "-" + s;
                        });
                    } else {
                        return (this.choice && this.choice.search) ? this.choice.search : this.getValue().toString();
                    }
                }

                return this.getValue().toString();
            };

            ValueViewModel.prototype.getValue = function () {
                if (this.hasChoices || this.hasPrompt || this.hasConditionalChoices) {
                    if (this.isMultipleChoices) {
                        var selections = this.multiChoices || [];

                        if (this.type === "scalar") {
                            var selValues = _.map(selections, function (cvm) {
                                return cvm.value;
                            });
                            return new Spiro.Value(selValues);
                        }

                        var selRefs = _.map(selections, function (cvm) {
                            return { href: cvm.value, title: cvm.name };
                        });

                        // reference
                        return new Spiro.Value(selRefs);
                    }

                    if (this.type === "scalar") {
                        return new Spiro.Value(this.choice && this.choice.value != null ? this.choice.value : "");
                    }

                    // reference
                    return new Spiro.Value(this.choice && this.choice.value ? { href: this.choice.value, title: this.choice.name } : null);
                }

                if (this.type === "scalar") {
                    return new Spiro.Value(this.value == null ? "" : this.value);
                }

                // reference
                return new Spiro.Value(this.reference ? { href: this.reference } : null);
            };
            return ValueViewModel;
        })(MessageViewModel);
        Angular.ValueViewModel = ValueViewModel;

        var ParameterViewModel = (function (_super) {
            __extends(ParameterViewModel, _super);
            function ParameterViewModel() {
                _super.apply(this, arguments);
            }
            return ParameterViewModel;
        })(ValueViewModel);
        Angular.ParameterViewModel = ParameterViewModel;

        var ActionViewModel = (function () {
            function ActionViewModel() {
            }
            return ActionViewModel;
        })();
        Angular.ActionViewModel = ActionViewModel;

        var DialogViewModel = (function (_super) {
            __extends(DialogViewModel, _super);
            function DialogViewModel() {
                _super.apply(this, arguments);
            }
            DialogViewModel.prototype.doShow = function () {
            };
            DialogViewModel.prototype.doInvoke = function () {
            };

            DialogViewModel.prototype.clearMessages = function () {
                this.message = "";
                _.each(this.parameters, function (parm) {
                    return parm.clearMessage();
                });
            };
            return DialogViewModel;
        })(MessageViewModel);
        Angular.DialogViewModel = DialogViewModel;

        var PropertyViewModel = (function (_super) {
            __extends(PropertyViewModel, _super);
            function PropertyViewModel() {
                _super.apply(this, arguments);
            }
            return PropertyViewModel;
        })(ValueViewModel);
        Angular.PropertyViewModel = PropertyViewModel;

        var CollectionViewModel = (function () {
            function CollectionViewModel() {
            }
            return CollectionViewModel;
        })();
        Angular.CollectionViewModel = CollectionViewModel;

        var ServicesViewModel = (function () {
            function ServicesViewModel() {
            }
            return ServicesViewModel;
        })();
        Angular.ServicesViewModel = ServicesViewModel;

        var ServiceViewModel = (function () {
            function ServiceViewModel() {
            }
            return ServiceViewModel;
        })();
        Angular.ServiceViewModel = ServiceViewModel;

        var DomainObjectViewModel = (function (_super) {
            __extends(DomainObjectViewModel, _super);
            function DomainObjectViewModel() {
                _super.apply(this, arguments);
            }
            DomainObjectViewModel.prototype.doSave = function () {
            };
            return DomainObjectViewModel;
        })(MessageViewModel);
        Angular.DomainObjectViewModel = DomainObjectViewModel;
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.viewmodels.js.map
