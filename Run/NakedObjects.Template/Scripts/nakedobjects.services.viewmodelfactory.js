/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.userMessages.config.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var CollectionMember = NakedObjects.Models.CollectionMember;
    var Value = NakedObjects.Models.Value;
    var PropertyMember = NakedObjects.Models.PropertyMember;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var HttpStatusCode = NakedObjects.Models.HttpStatusCode;
    var EntryType = NakedObjects.Models.EntryType;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    var TypePlusTitle = NakedObjects.Models.typePlusTitle;
    var toUtcDate = NakedObjects.Models.toUtcDate;
    var isDateOrDateTime = NakedObjects.Models.isDateOrDateTime;
    var PlusTitle = NakedObjects.Models.typePlusTitle;
    var Title = NakedObjects.Models.typePlusTitle;
    var FriendlyNameForProperty = NakedObjects.Models.friendlyNameForProperty;
    var FriendlyNameForParam = NakedObjects.Models.friendlyNameForParam;
    var ActionRepresentation = NakedObjects.Models.ActionRepresentation;
    var dirtyMarker = NakedObjects.Models.dirtyMarker;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    var InvokableActionMember = NakedObjects.Models.InvokableActionMember;
    var isTime = NakedObjects.Models.isTime;
    var toTime = NakedObjects.Models.toTime;
    NakedObjects.app.service("viewModelFactory", function ($q, $timeout, $location, $filter, $cacheFactory, repLoader, color, context, mask, urlManager, focusManager, navigation, clickHandler, commandFactory, $rootScope, $route, $http) {
        var _this = this;
        var viewModelFactory = this;
        viewModelFactory.errorViewModel = function (error) {
            var errorViewModel = new NakedObjects.ErrorViewModel();
            if (error) {
                errorViewModel.title = error.title;
                errorViewModel.description = error.description;
                errorViewModel.code = error.errorCode;
                errorViewModel.message = error.message;
                var stackTrace = error.stackTrace;
                errorViewModel.stackTrace = stackTrace && stackTrace.length !== 0 ? stackTrace : null;
                errorViewModel.isConcurrencyError =
                    error.category === ErrorCategory.HttpClientError &&
                        error.httpErrorCode === HttpStatusCode.PreconditionFailed;
            }
            errorViewModel.description = errorViewModel.description || "No description available";
            errorViewModel.code = errorViewModel.code || "No code available";
            errorViewModel.message = errorViewModel.message || "No message available";
            errorViewModel.stackTrace = errorViewModel.stackTrace || ["No stack trace available"];
            return errorViewModel;
        };
        function initLinkViewModel(linkViewModel, linkRep) {
            linkViewModel.title = linkRep.title();
            color.toColorNumberFromHref(linkRep.href()).then(function (c) { return linkViewModel.color = "" + NakedObjects.linkColor + c; });
            linkViewModel.link = linkRep;
            linkViewModel.domainType = linkRep.type().domainType;
            linkViewModel.draggableType = linkViewModel.domainType;
            // for dropping 
            var value = new Value(linkRep);
            linkViewModel.value = value.toString();
            linkViewModel.reference = value.toValueString();
            linkViewModel.choice = NakedObjects.ChoiceViewModel.create(value, "");
            linkViewModel.canDropOn = function (targetType) { return context.isSubTypeOf(linkViewModel.domainType, targetType); };
            linkViewModel.title = linkViewModel.title + dirtyMarker(context, linkRep.getOid());
        }
        var createChoiceViewModels = function (id, searchTerm, choices) {
            return $q.when(_.map(choices, function (v, k) { return NakedObjects.ChoiceViewModel.create(v, id, k, searchTerm); }));
        };
        viewModelFactory.attachmentViewModel = function (propertyRep, paneId) {
            var parent = propertyRep.parent;
            var avm = NakedObjects.AttachmentViewModel.create(propertyRep.attachmentLink(), parent, context, paneId);
            avm.doClick = function (right) { return urlManager.setAttachment(avm.link, clickHandler.pane(paneId, right)); };
            return avm;
        };
        viewModelFactory.linkViewModel = function (linkRep, paneId) {
            var linkViewModel = new NakedObjects.LinkViewModel();
            linkViewModel.doClick = function () {
                // because may be clicking on menu already open so want to reset focus             
                urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
                focusManager.setCurrentPane(paneId);
                focusManager.focusOverrideOff();
                focusManager.focusOn(NakedObjects.FocusTarget.SubAction, 0, paneId);
            };
            initLinkViewModel(linkViewModel, linkRep);
            return linkViewModel;
        };
        viewModelFactory.itemViewModel = function (linkRep, paneId, selected) {
            var itemViewModel = new NakedObjects.ItemViewModel();
            itemViewModel.doClick = function (right) {
                var currentPane = clickHandler.pane(paneId, right);
                focusManager.setCurrentPane(currentPane);
                urlManager.setItem(linkRep, currentPane);
            };
            initLinkViewModel(itemViewModel, linkRep);
            itemViewModel.selected = selected;
            itemViewModel.checkboxChange = function (index) {
                urlManager.setListItem(index, itemViewModel.selected, paneId);
                focusManager.focusOverrideOn(NakedObjects.FocusTarget.CheckBox, index + 1, paneId);
            };
            var members = linkRep.members();
            if (members) {
                itemViewModel.target = viewModelFactory.tableRowViewModel(members, paneId);
                itemViewModel.target.title = itemViewModel.title;
            }
            return itemViewModel;
        };
        viewModelFactory.recentItemViewModel = function (obj, linkRep, paneId, selected) {
            var recentItemViewModel = viewModelFactory.itemViewModel(linkRep, paneId, selected);
            recentItemViewModel.friendlyName = obj.extensions().friendlyName();
            return recentItemViewModel;
        };
        viewModelFactory.parameterViewModel = function (parmRep, previousValue, paneId) {
            var parmViewModel = new NakedObjects.ParameterViewModel();
            parmViewModel.parameterRep = parmRep;
            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";
            parmViewModel.dflt = parmRep.default().toString();
            parmViewModel.optional = parmRep.extensions().optional();
            var required = parmViewModel.optional ? "" : "* ";
            parmViewModel.description = parmRep.extensions().description();
            parmViewModel.setMessage("");
            parmViewModel.id = parmRep.id();
            parmViewModel.argId = "" + parmViewModel.id.toLowerCase();
            parmViewModel.paneArgId = "" + parmViewModel.argId + paneId;
            parmViewModel.reference = "";
            parmViewModel.mask = parmRep.extensions().mask();
            parmViewModel.title = parmRep.extensions().friendlyName();
            parmViewModel.returnType = parmRep.extensions().returnType();
            parmViewModel.format = parmRep.extensions().format();
            parmViewModel.isCollectionContributed = parmRep.isCollectionContributed();
            parmViewModel.onPaneId = paneId;
            parmViewModel.multipleLines = parmRep.extensions().multipleLines() || 1;
            parmViewModel.password = parmRep.extensions().dataType() === "password";
            parmViewModel.clientValid = true;
            var fieldEntryType = parmRep.entryType();
            parmViewModel.entryType = fieldEntryType;
            parmViewModel.choices = [];
            if (fieldEntryType === EntryType.Choices || fieldEntryType === EntryType.MultipleChoices) {
                parmViewModel.choices = _.map(parmRep.choices(), function (v, n) { return NakedObjects.ChoiceViewModel.create(v, parmRep.id(), n); });
            }
            if (fieldEntryType === EntryType.AutoComplete) {
                parmViewModel.prompt = function (searchTerm) {
                    var createcvm = _.partial(createChoiceViewModels, parmViewModel.id, searchTerm);
                    return context.autoComplete(parmRep, parmViewModel.id, function () { return {}; }, searchTerm).
                        then(createcvm);
                };
                parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
                parmViewModel.description = parmViewModel.description || NakedObjects.autoCompletePrompt;
            }
            if (fieldEntryType === EntryType.FreeForm && parmViewModel.type === "ref") {
                parmViewModel.description = parmViewModel.description || NakedObjects.dropPrompt;
                parmViewModel.refresh = function (newValue) {
                    var val = newValue && !newValue.isNull() ? newValue : parmRep.default();
                    if (!val.isNull() && val.isReference()) {
                        parmViewModel.reference = val.link().href();
                        parmViewModel.choice = NakedObjects.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
                    }
                    parmViewModel.setColor(color);
                };
                parmViewModel.refresh(previousValue);
            }
            if (fieldEntryType === EntryType.ConditionalChoices || fieldEntryType === EntryType.MultipleConditionalChoices) {
                parmViewModel.conditionalChoices = function (args) {
                    var createcvm = _.partial(createChoiceViewModels, parmViewModel.id, null);
                    return context.conditionalChoices(parmRep, parmViewModel.id, function () { return {}; }, args).
                        then(createcvm);
                };
                // fromPairs definition faulty
                parmViewModel.arguments = _.fromPairs(_.map(parmRep.promptLink().arguments(), function (v, key) { return [key, new Value(v.value)]; }));
            }
            if (fieldEntryType !== EntryType.FreeForm || parmViewModel.isCollectionContributed) {
                function setCurrentChoices(vals) {
                    var choicesToSet = _.map(vals.list(), function (val) { return NakedObjects.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null); });
                    if (fieldEntryType === EntryType.MultipleChoices) {
                        parmViewModel.multiChoices = _.filter(parmViewModel.choices, function (c) { return _.some(choicesToSet, function (choiceToSet) { return c.match(choiceToSet); }); });
                    }
                    else {
                        parmViewModel.multiChoices = choicesToSet;
                    }
                }
                function setCurrentChoice(val) {
                    var choiceToSet = NakedObjects.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
                    if (fieldEntryType === EntryType.Choices) {
                        parmViewModel.choice = _.find(parmViewModel.choices, function (c) { return c.match(choiceToSet); });
                    }
                    else {
                        parmViewModel.choice = choiceToSet;
                    }
                }
                parmViewModel.refresh = function (newValue) {
                    if (newValue || parmViewModel.dflt) {
                        var toSet = newValue || parmRep.default();
                        if (fieldEntryType === EntryType.MultipleChoices ||
                            fieldEntryType === EntryType.MultipleConditionalChoices ||
                            parmViewModel.isCollectionContributed) {
                            setCurrentChoices(toSet);
                        }
                        else {
                            setCurrentChoice(toSet);
                        }
                    }
                    parmViewModel.setColor(color);
                };
                parmViewModel.refresh(previousValue);
            }
            else {
                var returnType_1 = parmRep.extensions().returnType();
                parmViewModel.refresh = function (newValue) {
                    if (returnType_1 === "boolean") {
                        var valueToSet = (newValue ? newValue.toValueString() : null) ||
                            parmRep.default().scalar();
                        var bValueToSet = NakedObjects.toTriStateBoolean(valueToSet);
                        parmViewModel.value = bValueToSet;
                        if (bValueToSet !== null) {
                            // reset required indicator
                            required = "";
                        }
                    }
                    else if (isDateOrDateTime(parmRep)) {
                        parmViewModel.value = toUtcDate(newValue || new Value(parmViewModel.dflt));
                    }
                    else if (isTime(parmRep)) {
                        parmViewModel.value = toTime(newValue || new Value(parmViewModel.dflt));
                    }
                    else {
                        parmViewModel.value = (newValue ? newValue.toString() : null) || parmViewModel.dflt || "";
                    }
                    parmViewModel.setColor(color);
                };
                parmViewModel.refresh(previousValue);
            }
            var remoteMask = parmRep.extensions().mask();
            if (remoteMask && parmRep.isScalar()) {
                var localFilter = mask.toLocalFilter(remoteMask, parmRep.extensions().format());
                parmViewModel.localFilter = localFilter;
                // formatting also happens in in directive - at least for dates - value is now date in that case
                parmViewModel.formattedValue = parmViewModel.value ? localFilter.filter(parmViewModel.value.toString()) : "";
            }
            parmViewModel.setColor(color);
            parmViewModel.validate = _.partial(validate, parmRep, parmViewModel);
            parmViewModel.drop = _.partial(drop, parmViewModel);
            parmViewModel.description = required + parmViewModel.description;
            parmViewModel.refresh = parmViewModel.refresh || (function (newValue) { });
            return parmViewModel;
        };
        viewModelFactory.actionViewModel = function (actionRep, vm, routeData) {
            var actionViewModel = new NakedObjects.ActionViewModel();
            var parms = routeData.actionParams;
            var paneId = routeData.paneId;
            actionViewModel.actionRep = actionRep;
            if (actionRep instanceof ActionRepresentation || actionRep instanceof InvokableActionMember) {
                actionViewModel.invokableActionRep = actionRep;
            }
            actionViewModel.title = actionRep.extensions().friendlyName();
            actionViewModel.menuPath = actionRep.extensions().menuPath() || "";
            actionViewModel.disabled = function () { return !!actionRep.disabledReason(); };
            actionViewModel.description = actionViewModel.disabled() ? actionRep.disabledReason() : actionRep.extensions().description();
            actionViewModel.parameters = function () {
                // don't use actionRep directly as it may change and we've closed around the original value
                var parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), function (p) { return !p.isCollectionContributed(); });
                return _.map(parameters, function (parm) { return viewModelFactory.parameterViewModel(parm, parms[parm.id()], paneId); });
            };
            actionViewModel.executeInvoke = function (pps, right) {
                var parmMap = _.zipObject(_.map(pps, function (p) { return p.id; }), _.map(pps, function (p) { return p.getValue(); }));
                _.forEach(pps, function (p) { return urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId); });
                return context.getInvokableAction(actionViewModel.actionRep).then(function (details) { return context.invokeAction(details, clickHandler.pane(paneId, right), parmMap); });
            };
            // form actions should never show dialogs
            var showDialog = function () { return actionRep.extensions().hasParams() && (routeData.interactionMode !== NakedObjects.InteractionMode.Form); };
            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = showDialog() ?
                function (right) {
                    focusManager.setCurrentPane(paneId);
                    focusManager.focusOverrideOff();
                    urlManager.setDialog(actionRep.actionId(), paneId);
                    focusManager.focusOn(NakedObjects.FocusTarget.Dialog, 0, paneId); // in case dialog is already open
                } :
                function (right) {
                    focusManager.focusOverrideOff();
                    var pps = actionViewModel.parameters();
                    actionViewModel.executeInvoke(pps, right).
                        catch(function (reject) {
                        var parent = actionRep.parent;
                        var reset = function (updatedObject) { return _this.reset(updatedObject, urlManager.getRouteData().pane()[_this.onPaneId]); };
                        var display = function (em) { return vm.setMessage(em.invalidReason() || em.warningMessage); };
                        context.handleWrappedError(reject, parent, reset, display);
                    });
                };
            actionViewModel.makeInvokable = function (details) { return actionViewModel.invokableActionRep = details; };
            return actionViewModel;
        };
        viewModelFactory.handleErrorResponse = function (err, messageViewModel, valueViewModels) {
            var requiredFieldsMissing = false; // only show warning message if we have nothing else 
            var fieldValidationErrors = false;
            _.each(valueViewModels, function (valueViewModel) {
                var errorValue = err.valuesMap()[valueViewModel.id];
                if (errorValue) {
                    var reason = errorValue.invalidReason;
                    if (reason) {
                        if (reason === "Mandatory") {
                            var r = "REQUIRED";
                            requiredFieldsMissing = true;
                            valueViewModel.description = valueViewModel.description.indexOf(r) === 0 ? valueViewModel.description : r + " " + valueViewModel.description;
                        }
                        else {
                            valueViewModel.setMessage(reason);
                            fieldValidationErrors = true;
                        }
                    }
                }
            });
            var msg = err.invalidReason() || "";
            if (requiredFieldsMissing)
                msg = msg + " Please complete REQUIRED fields. ";
            if (fieldValidationErrors)
                msg = msg + " See field validation message(s). ";
            if (!msg)
                msg = err.warningMessage;
            messageViewModel.setMessage(msg);
        };
        function drop(vm, newValue) {
            context.isSubTypeOf(newValue.draggableType, vm.returnType).
                then(function (canDrop) {
                if (canDrop) {
                    vm.setNewValue(newValue);
                }
            });
        }
        ;
        function validate(rep, vm, modelValue, viewValue, mandatoryOnly) {
            var message = mandatoryOnly ? NakedObjects.Models.validateMandatory(rep, viewValue) : NakedObjects.Models.validate(rep, modelValue, viewValue, vm.localFilter);
            if (message !== NakedObjects.mandatory) {
                vm.setMessage(message);
            }
            vm.clientValid = !message;
            return vm.clientValid;
        }
        ;
        function setupReference(vm, value, rep) {
            vm.type = "ref";
            if (value.isNull()) {
                vm.reference = "";
                vm.value = vm.description;
                vm.formattedValue = "";
                vm.refType = "null";
            }
            else {
                vm.reference = value.link().href();
                vm.value = value.toString();
                vm.formattedValue = value.toString();
                vm.refType = rep.extensions().notNavigable() ? "notNavigable" : "navigable";
            }
            if (vm.entryType === EntryType.FreeForm) {
                vm.description = vm.description || NakedObjects.dropPrompt;
            }
        }
        viewModelFactory.propertyTableViewModel = function (propertyRep, id, paneId) {
            var tableRowColumnViewModel = new NakedObjects.TableRowColumnViewModel();
            tableRowColumnViewModel.title = propertyRep.extensions().friendlyName();
            if (propertyRep instanceof CollectionMember) {
                var size = propertyRep.size();
                tableRowColumnViewModel.formattedValue = getCollectionCount(size);
                tableRowColumnViewModel.value = "";
                tableRowColumnViewModel.type = "scalar";
                tableRowColumnViewModel.returnType = "string";
            }
            if (propertyRep instanceof PropertyMember) {
                var isPassword = propertyRep.extensions().dataType() === "password";
                var value = propertyRep.value();
                tableRowColumnViewModel.returnType = propertyRep.extensions().returnType();
                if (propertyRep.isScalar()) {
                    if (isDateOrDateTime(propertyRep)) {
                        tableRowColumnViewModel.value = toUtcDate(value);
                    }
                    else {
                        tableRowColumnViewModel.value = value.scalar();
                    }
                    tableRowColumnViewModel.type = "scalar";
                    var remoteMask = propertyRep.extensions().mask();
                    var localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
                    if (propertyRep.entryType() === EntryType.Choices) {
                        var currentChoice_1 = NakedObjects.ChoiceViewModel.create(value, id);
                        var choices = _.map(propertyRep.choices(), function (v, n) { return NakedObjects.ChoiceViewModel.create(v, id, n); });
                        var choice = _.find(choices, function (c) { return c.match(currentChoice_1); });
                        if (choice) {
                            tableRowColumnViewModel.value = choice.name;
                            tableRowColumnViewModel.formattedValue = choice.name;
                        }
                    }
                    else if (isPassword) {
                        tableRowColumnViewModel.formattedValue = NakedObjects.obscuredText;
                    }
                    else {
                        tableRowColumnViewModel.formattedValue = localFilter.filter(tableRowColumnViewModel.value);
                    }
                }
                else {
                    // is reference   
                    tableRowColumnViewModel.type = "ref";
                    tableRowColumnViewModel.formattedValue = value.isNull() ? "" : value.toString();
                }
            }
            return tableRowColumnViewModel;
        };
        viewModelFactory.propertyViewModel = function (propertyRep, id, previousValue, paneId, parentValues) {
            var propertyViewModel = new NakedObjects.PropertyViewModel();
            propertyViewModel.onPaneId = paneId;
            propertyViewModel.propertyRep = propertyRep;
            propertyViewModel.entryType = propertyRep.entryType();
            propertyViewModel.id = id;
            propertyViewModel.argId = "" + id.toLowerCase();
            propertyViewModel.paneArgId = "" + propertyViewModel.argId + paneId;
            propertyViewModel.isEditable = !propertyRep.disabledReason();
            propertyViewModel.title = propertyRep.extensions().friendlyName();
            propertyViewModel.optional = propertyRep.extensions().optional();
            propertyViewModel.returnType = propertyRep.extensions().returnType();
            propertyViewModel.draggableType = propertyRep.extensions().returnType();
            propertyViewModel.format = propertyRep.extensions().format();
            propertyViewModel.multipleLines = propertyRep.extensions().multipleLines() || 1;
            propertyViewModel.password = propertyRep.extensions().dataType() === "password";
            propertyViewModel.clientValid = true;
            var required = propertyViewModel.optional ? "" : "* ";
            propertyViewModel.description = propertyRep.extensions().description();
            if (propertyRep.attachmentLink() != null) {
                propertyViewModel.attachment = viewModelFactory.attachmentViewModel(propertyRep, paneId);
            }
            var setupChoice;
            if (propertyRep.entryType() === EntryType.Choices) {
                var choices = propertyRep.choices();
                propertyViewModel.choices = _.map(choices, function (v, n) { return NakedObjects.ChoiceViewModel.create(v, id, n); });
                setupChoice = function (newValue) {
                    var currentChoice = NakedObjects.ChoiceViewModel.create(newValue, id);
                    propertyViewModel.choice = _.find(propertyViewModel.choices, function (c) { return c.match(currentChoice); });
                };
            }
            else {
                // use choice for draggable/droppable references
                propertyViewModel.choices = [];
                setupChoice = function (newValue) { return propertyViewModel.choice = NakedObjects.ChoiceViewModel.create(newValue, id); };
            }
            if (propertyRep.entryType() === EntryType.AutoComplete) {
                propertyViewModel.prompt = function (searchTerm) {
                    var createcvm = _.partial(createChoiceViewModels, id, searchTerm);
                    return context.autoComplete(propertyRep, id, parentValues, searchTerm).then(createcvm);
                };
                propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
                propertyViewModel.description = propertyViewModel.description || NakedObjects.autoCompletePrompt;
            }
            if (propertyRep.entryType() === EntryType.ConditionalChoices) {
                propertyViewModel.conditionalChoices = function (args) {
                    var createcvm = _.partial(createChoiceViewModels, id, null);
                    return context.conditionalChoices(propertyRep, id, function () { return {}; }, args).then(createcvm);
                };
                // fromPairs definition faulty
                propertyViewModel.arguments = _.fromPairs(_.map(propertyRep.promptLink().arguments(), function (v, key) { return [key, new Value(v.value)]; }));
            }
            function callIfChanged(newValue, doRefresh) {
                var value = newValue || propertyRep.value();
                if (propertyViewModel.currentValue == null || value.toValueString() !== propertyViewModel.currentValue.toValueString()) {
                    doRefresh(value);
                    propertyViewModel.currentValue = value;
                }
            }
            if (propertyRep.isScalar()) {
                propertyViewModel.reference = "";
                propertyViewModel.type = "scalar";
                var remoteMask = propertyRep.extensions().mask();
                var localFilter_1 = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
                propertyViewModel.localFilter = localFilter_1;
                // formatting also happens in in directive - at least for dates - value is now date in that case
                propertyViewModel.refresh = function (newValue) { return callIfChanged(newValue, function (value) {
                    setupChoice(value);
                    if (isDateOrDateTime(propertyRep)) {
                        propertyViewModel.value = toUtcDate(value);
                    }
                    else if (isTime(propertyRep)) {
                        propertyViewModel.value = toTime(value);
                    }
                    else {
                        propertyViewModel.value = value.scalar();
                    }
                    if (propertyRep.entryType() === EntryType.Choices) {
                        if (propertyViewModel.choice) {
                            propertyViewModel.value = propertyViewModel.choice.name;
                            propertyViewModel.formattedValue = propertyViewModel.choice.name;
                        }
                    }
                    else if (propertyViewModel.password) {
                        propertyViewModel.formattedValue = NakedObjects.obscuredText;
                    }
                    else {
                        propertyViewModel.formattedValue = localFilter_1.filter(propertyViewModel.value);
                    }
                }); };
            }
            else {
                // is reference
                propertyViewModel.refresh = function (newValue) { return callIfChanged(newValue, function (value) {
                    setupChoice(value);
                    setupReference(propertyViewModel, value, propertyRep);
                }); };
            }
            propertyViewModel.refresh(previousValue);
            // only set color if has value 
            propertyViewModel.setColor(color);
            if (!previousValue) {
                propertyViewModel.originalValue = propertyViewModel.getValue();
            }
            propertyViewModel.description = required + propertyViewModel.description;
            propertyViewModel.isDirty = function () { return !!previousValue || propertyViewModel.getValue().toValueString() !== propertyViewModel.originalValue.toValueString(); };
            propertyViewModel.validate = _.partial(validate, propertyRep, propertyViewModel);
            propertyViewModel.canDropOn = function (targetType) { return context.isSubTypeOf(propertyViewModel.returnType, targetType); };
            propertyViewModel.drop = _.partial(drop, propertyViewModel);
            propertyViewModel.doClick = function (right) { return urlManager.setProperty(propertyRep, clickHandler.pane(paneId, right)); };
            return propertyViewModel;
        };
        viewModelFactory.getItems = function (links, tableView, routeData, listViewModel) {
            var selectedItems = routeData.selectedItems;
            var items = _.map(links, function (link, i) { return viewModelFactory.itemViewModel(link, routeData.paneId, selectedItems[i]); });
            if (tableView) {
                var getActionExtensions = routeData.objectId ?
                    function () { return context.getActionExtensionsFromObject(routeData.paneId, ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId); } :
                    function () { return context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId); };
                var getExtensions = listViewModel instanceof NakedObjects.CollectionViewModel ? function () { return $q.when(listViewModel.collectionRep.extensions()); } : getActionExtensions;
                // clear existing header 
                listViewModel.header = null;
                if (items.length > 0) {
                    getExtensions().then(function (ext) {
                        _.forEach(items, function (itemViewModel) {
                            itemViewModel.target.hasTitle = ext.tableViewTitle();
                            itemViewModel.target.title = itemViewModel.title;
                        });
                        if (!listViewModel.header) {
                            var firstItem = items[0].target;
                            var propertiesHeader = _.map(firstItem.properties, function (property) { return property.title; });
                            listViewModel.header = firstItem.hasTitle ? [""].concat(propertiesHeader) : propertiesHeader;
                            focusManager.focusOverrideOff();
                            focusManager.focusOn(NakedObjects.FocusTarget.TableItem, 0, routeData.paneId);
                        }
                    });
                }
            }
            return items;
        };
        function getCollectionCount(count) {
            if (count == null) {
                return NakedObjects.unknownCollectionSize;
            }
            if (count === 0) {
                return NakedObjects.emptyCollectionSize;
            }
            var postfix = count === 1 ? "Item" : "Items";
            return count + " " + postfix;
        }
        function getDefaultTableState(exts) {
            if (exts.renderEagerly()) {
                return exts.tableViewColumns() || exts.tableViewTitle() ? NakedObjects.CollectionViewState.Table : NakedObjects.CollectionViewState.List;
            }
            return NakedObjects.CollectionViewState.Summary;
        }
        viewModelFactory.collectionViewModel = function (collectionRep, routeData) {
            var collectionViewModel = new NakedObjects.CollectionViewModel();
            var itemLinks = collectionRep.value();
            var paneId = routeData.paneId;
            var size = collectionRep.size();
            collectionViewModel.collectionRep = collectionRep;
            collectionViewModel.onPaneId = paneId;
            collectionViewModel.title = collectionRep.extensions().friendlyName();
            collectionViewModel.pluralName = collectionRep.extensions().pluralName();
            color.toColorNumberFromType(collectionRep.extensions().elementType()).then(function (c) { return collectionViewModel.color = "" + NakedObjects.linkColor + c; });
            collectionViewModel.refresh = function (routeData, resetting) {
                var state = size === 0 ? NakedObjects.CollectionViewState.Summary : routeData.collections[collectionRep.collectionId()];
                if (state == null) {
                    state = getDefaultTableState(collectionRep.extensions());
                }
                if (resetting || state !== collectionViewModel.currentState) {
                    collectionViewModel.size = getCollectionCount(size);
                    var getDetails = itemLinks == null || state === NakedObjects.CollectionViewState.Table;
                    if (state === NakedObjects.CollectionViewState.Summary) {
                        collectionViewModel.items = [];
                    }
                    else if (getDetails) {
                        // TODO - there was a missing catch here make sure all top level promises have a catch !
                        context.getCollectionDetails(collectionRep, state, resetting)
                            .then(function (details) {
                            collectionViewModel.items = viewModelFactory.getItems(details.value(), state === NakedObjects.CollectionViewState.Table, routeData, collectionViewModel);
                            collectionViewModel.size = getCollectionCount(collectionViewModel.items.length);
                        })
                            .catch(function (reject) {
                            context.handleWrappedError(reject, null, function () { }, function () { });
                        });
                    }
                    else {
                        collectionViewModel.items = viewModelFactory.getItems(itemLinks, state === NakedObjects.CollectionViewState.Table, routeData, collectionViewModel);
                    }
                    switch (state) {
                        case NakedObjects.CollectionViewState.List:
                            collectionViewModel.template = NakedObjects.collectionListTemplate;
                            break;
                        case NakedObjects.CollectionViewState.Table:
                            collectionViewModel.template = NakedObjects.collectionTableTemplate;
                            break;
                        default:
                            collectionViewModel.template = NakedObjects.collectionSummaryTemplate;
                    }
                    collectionViewModel.currentState = state;
                }
            };
            collectionViewModel.refresh(routeData, true);
            collectionViewModel.doSummary = function () { return urlManager.setCollectionMemberState(collectionRep.collectionId(), NakedObjects.CollectionViewState.Summary, paneId); };
            collectionViewModel.doList = function () { return urlManager.setCollectionMemberState(collectionRep.collectionId(), NakedObjects.CollectionViewState.List, paneId); };
            collectionViewModel.doTable = function () { return urlManager.setCollectionMemberState(collectionRep.collectionId(), NakedObjects.CollectionViewState.Table, paneId); };
            return collectionViewModel;
        };
        viewModelFactory.listPlaceholderViewModel = function (routeData) {
            var collectionPlaceholderViewModel = new NakedObjects.CollectionPlaceholderViewModel();
            collectionPlaceholderViewModel.description = function () { return ("Page " + routeData.page); };
            var recreate = function () {
                return routeData.objectId ?
                    context.getListFromObject(routeData.paneId, routeData, routeData.page, routeData.pageSize) :
                    context.getListFromMenu(routeData.paneId, routeData, routeData.page, routeData.pageSize);
            };
            collectionPlaceholderViewModel.reload = function () {
                return recreate().
                    then(function () { return $route.reload(); }).
                    catch(function (reject) {
                    context.handleWrappedError(reject, null, function () { }, function () { });
                });
            };
            return collectionPlaceholderViewModel;
        };
        viewModelFactory.servicesViewModel = function (servicesRep) {
            var servicesViewModel = new NakedObjects.ServicesViewModel();
            // filter out contributed action services 
            var links = _.filter(servicesRep.value(), function (m) {
                var sid = m.rel().parms[0].value;
                return sid.indexOf("ContributedActions") === -1;
            });
            servicesViewModel.title = "Services";
            servicesViewModel.color = "bg-color-darkBlue";
            servicesViewModel.items = _.map(links, function (link) { return viewModelFactory.linkViewModel(link, 1); });
            return servicesViewModel;
        };
        viewModelFactory.serviceViewModel = function (serviceRep, routeData) {
            var serviceViewModel = new NakedObjects.ServiceViewModel();
            var actions = serviceRep.actionMembers();
            serviceViewModel.serviceId = serviceRep.serviceId();
            serviceViewModel.title = serviceRep.title();
            serviceViewModel.actions = _.map(actions, function (action) { return viewModelFactory.actionViewModel(action, serviceViewModel, routeData); });
            serviceViewModel.menuItems = NakedObjects.createMenuItems(serviceViewModel.actions);
            color.toColorNumberFromType(serviceRep.serviceId()).then(function (c) {
                serviceViewModel.color = "" + NakedObjects.objectColor + c;
            });
            return serviceViewModel;
        };
        viewModelFactory.menuViewModel = function (menuRep, routeData) {
            var menuViewModel = new NakedObjects.MenuViewModel();
            menuViewModel.id = menuRep.menuId();
            menuViewModel.menuRep = menuRep;
            var actions = menuRep.actionMembers();
            menuViewModel.title = menuRep.title();
            menuViewModel.actions = _.map(actions, function (action) { return viewModelFactory.actionViewModel(action, menuViewModel, routeData); });
            menuViewModel.menuItems = NakedObjects.createMenuItems(menuViewModel.actions);
            return menuViewModel;
        };
        function selfLinkWithTitle(o) {
            var link = o.selfLink();
            link.setTitle(o.title());
            return link;
        }
        viewModelFactory.recentItemsViewModel = function (paneId) {
            var recentItemsViewModel = new NakedObjects.RecentItemsViewModel();
            recentItemsViewModel.onPaneId = paneId;
            var items = _.map(context.getRecentlyViewed(), function (o) { return ({ obj: o, link: selfLinkWithTitle(o) }); });
            recentItemsViewModel.items = _.map(items, function (i) { return viewModelFactory.recentItemViewModel(i.obj, i.link, paneId, false); });
            return recentItemsViewModel;
        };
        viewModelFactory.tableRowViewModel = function (properties, paneId) {
            var tableRowViewModel = new NakedObjects.TableRowViewModel();
            tableRowViewModel.properties = _.map(properties, function (property, id) { return viewModelFactory.propertyTableViewModel(property, id, paneId); });
            return tableRowViewModel;
        };
        var cachedToolBarViewModel;
        function getToolBarViewModel() {
            if (!cachedToolBarViewModel) {
                var tvm_1 = new NakedObjects.ToolBarViewModel();
                tvm_1.goHome = function (right) {
                    focusManager.focusOverrideOff();
                    urlManager.setHome(clickHandler.pane(1, right));
                };
                tvm_1.goBack = function () {
                    focusManager.focusOverrideOff();
                    navigation.back();
                };
                tvm_1.goForward = function () {
                    focusManager.focusOverrideOff();
                    navigation.forward();
                };
                tvm_1.swapPanes = function () {
                    $rootScope.$broadcast(NakedObjects.geminiPaneSwapEvent);
                    context.swapCurrentObjects();
                    urlManager.swapPanes();
                };
                tvm_1.singlePane = function (right) {
                    urlManager.singlePane(clickHandler.pane(1, right));
                    focusManager.refresh(1);
                };
                tvm_1.cicero = function () {
                    urlManager.singlePane(clickHandler.pane(1));
                    urlManager.cicero();
                };
                tvm_1.recent = function (right) {
                    focusManager.focusOverrideOff();
                    urlManager.setRecent(clickHandler.pane(1, right));
                };
                tvm_1.logOff = function () {
                    context.getUser()
                        .then(function (u) {
                        if (window.confirm(NakedObjects.logOffMessage(u.userName() || "Unknown"))) {
                            var config = {
                                withCredentials: true,
                                url: NakedObjects.logoffUrl,
                                method: "POST",
                                cache: false
                            };
                            $http(config)
                                .finally(function () {
                                $rootScope.$broadcast(NakedObjects.geminiLogoffEvent);
                                $timeout(function () { return window.location.href = NakedObjects.postLogoffUrl; });
                            });
                        }
                    });
                };
                tvm_1.applicationProperties = function () {
                    urlManager.applicationProperties();
                };
                tvm_1.template = NakedObjects.appBarTemplate;
                tvm_1.footerTemplate = NakedObjects.footerTemplate;
                $rootScope.$on(NakedObjects.geminiAjaxChangeEvent, function (event, count) {
                    return tvm_1.loading = count > 0 ? NakedObjects.loadingMessage : "";
                });
                $rootScope.$on(NakedObjects.geminiWarningEvent, function (event, warnings) {
                    return tvm_1.warnings = warnings;
                });
                $rootScope.$on(NakedObjects.geminiMessageEvent, function (event, messages) {
                    return tvm_1.messages = messages;
                });
                context.getUser().then(function (user) { return tvm_1.userName = user.userName(); });
                cachedToolBarViewModel = tvm_1;
            }
            return cachedToolBarViewModel;
        }
        viewModelFactory.toolBarViewModel = function () { return getToolBarViewModel(); };
        var cvm = null;
        viewModelFactory.ciceroViewModel = function () {
            if (cvm == null) {
                cvm = new NakedObjects.CiceroViewModel();
                commandFactory.initialiseCommands(cvm);
                cvm.parseInput = function (input) {
                    commandFactory.parseInput(input, cvm);
                };
                cvm.executeNextChainedCommandIfAny = function () {
                    if (cvm.chainedCommands && cvm.chainedCommands.length > 0) {
                        var next = cvm.popNextCommand();
                        commandFactory.processSingleCommand(next, cvm, true);
                    }
                };
                cvm.autoComplete = function (input) {
                    commandFactory.autoComplete(input, cvm);
                };
                cvm.renderHome = function (routeData) {
                    //TODO: Could put this in a function passed into a render method on CVM
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    }
                    else {
                        var output_1 = "";
                        if (routeData.menuId) {
                            context.getMenu(routeData.menuId)
                                .then(function (menu) {
                                output_1 += menu.title() + " menu" + "\n";
                                return routeData.dialogId ? context.getInvokableAction(menu.actionMember(routeData.dialogId)) : $q.when(null);
                            }).then(function (details) {
                                if (details) {
                                    output_1 += renderActionDialog(details, routeData, mask);
                                }
                            }).finally(function () {
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output_1);
                            });
                        }
                        else {
                            cvm.clearInput();
                            cvm.output = "Welcome to Cicero. Type 'help' and the Enter key for more information.";
                        }
                    }
                };
                cvm.renderObject = function (routeData) {
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    }
                    else {
                        var oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
                        context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                            .then(function (obj) {
                            var output = "";
                            var openCollIds = openCollectionIds(routeData);
                            if (_.some(openCollIds)) {
                                var id = openCollIds[0];
                                var coll = obj.collectionMember(id);
                                output += "Collection: " + coll.extensions().friendlyName() + " on " + TypePlusTitle(obj) + "\n";
                                switch (coll.size()) {
                                    case 0:
                                        output += "empty";
                                        break;
                                    case 1:
                                        output += "1 item";
                                        break;
                                    default:
                                        output += coll.size() + " items";
                                }
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                            }
                            else {
                                if (obj.isTransient()) {
                                    output += "Unsaved ";
                                    output += obj.extensions().friendlyName() + "\n";
                                    output += renderModifiedProperties(obj, routeData, mask);
                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                }
                                else if (routeData.interactionMode === NakedObjects.InteractionMode.Edit ||
                                    routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                                    var output_2 = "Editing ";
                                    output_2 += PlusTitle(obj) + "\n";
                                    if (routeData.dialogId) {
                                        context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                            .then(function (details) {
                                            output_2 += renderActionDialog(details, routeData, mask);
                                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output_2);
                                        });
                                    }
                                    else {
                                        output_2 += renderModifiedProperties(obj, routeData, mask);
                                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output_2);
                                    }
                                }
                                else {
                                    var output_3 = Title(obj) + "\n";
                                    if (routeData.dialogId) {
                                        context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                            .then(function (details) {
                                            output_3 += renderActionDialog(details, routeData, mask);
                                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output_3);
                                        });
                                    }
                                    else {
                                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output_3);
                                    }
                                }
                            }
                        }).catch(function (reject) {
                            var custom = function (cc) {
                                if (cc === ClientErrorCode.ExpiredTransient) {
                                    cvm.output = "The requested view of unsaved object details has expired";
                                    return true;
                                }
                                return false;
                            };
                            context.handleWrappedError(reject, null, function () { }, function () { }, custom);
                        });
                    }
                };
                cvm.renderList = function (routeData) {
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    }
                    else {
                        var listPromise = context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
                        listPromise.then(function (list) {
                            context.getMenu(routeData.menuId).then(function (menu) {
                                var count = list.value().length;
                                var numPages = list.pagination().numPages;
                                var description;
                                if (numPages > 1) {
                                    var page = list.pagination().page;
                                    var totalCount = list.pagination().totalCount;
                                    description = "Page " + page + " of " + numPages + " containing " + count + " of " + totalCount + " items";
                                }
                                else {
                                    description = count + " items";
                                }
                                var actionMember = menu.actionMember(routeData.actionId);
                                var actionName = actionMember.extensions().friendlyName();
                                var output = "Result from " + actionName + ":\n" + description;
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                            });
                        });
                    }
                };
                cvm.renderError = function () {
                    var err = context.getError().error;
                    cvm.clearInput();
                    cvm.output = "Sorry, an application error has occurred. " + err.message();
                };
            }
            return cvm;
        };
        function logoff() {
            cvm = null;
        }
        $rootScope.$on(NakedObjects.geminiLogoffEvent, function () { return logoff(); });
    });
    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    function openCollectionIds(routeData) {
        return _.filter(_.keys(routeData.collections), function (k) { return routeData.collections[k] !== NakedObjects.CollectionViewState.Summary; });
    }
    NakedObjects.openCollectionIds = openCollectionIds;
    function renderModifiedProperties(obj, routeData, mask) {
        var output = "";
        if (_.keys(routeData.props).length > 0) {
            output += "Modified properties:\n";
            _.each(routeData.props, function (value, propId) {
                output += FriendlyNameForProperty(obj, propId) + ": ";
                var pm = obj.propertyMember(propId);
                output += renderFieldValue(pm, value, mask);
                output += "\n";
            });
        }
        return output;
    }
    //Handles empty values, and also enum conversion
    function renderFieldValue(field, value, mask) {
        if (!field.isScalar()) {
            return value.isNull() ? "empty" : value.toString();
        }
        //Rest is for scalar fields only:
        if (value.toString()) {
            //This is to handle an enum: render it as text, not a number:           
            if (field.entryType() === EntryType.Choices) {
                var inverted = _.invert(field.choices());
                return inverted[value.toValueString()];
            }
            else if (field.entryType() === EntryType.MultipleChoices && value.isList()) {
                var inverted_1 = _.invert(field.choices());
                var output_4 = "";
                var values = value.list();
                _.forEach(values, function (v) {
                    output_4 += inverted_1[v.toValueString()] + ",";
                });
                return output_4;
            }
        }
        var properScalarValue;
        if (isDateOrDateTime(field)) {
            properScalarValue = toUtcDate(value);
        }
        else {
            properScalarValue = value.scalar();
        }
        if (properScalarValue === "" || properScalarValue == null) {
            return "empty";
        }
        else {
            var remoteMask = field.extensions().mask();
            var format = field.extensions().format();
            return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        }
    }
    NakedObjects.renderFieldValue = renderFieldValue;
    function renderActionDialogIfOpen(repWithActions, routeData, mask) {
        var output = "";
        if (routeData.dialogId) {
            var actionMember_1 = repWithActions.actionMember(routeData.dialogId);
            var actionName = actionMember_1.extensions().friendlyName();
            output += "Action dialog: " + actionName + "\n";
            _.forEach(routeData.dialogFields, function (value, paramId) {
                output += FriendlyNameForParam(actionMember_1, paramId) + ": ";
                var param = actionMember_1.parameters()[paramId];
                output += renderFieldValue(param, value, mask);
                output += "\n";
            });
        }
        return output;
    }
    function renderActionDialog(invokable, routeData, mask) {
        var actionName = invokable.extensions().friendlyName();
        var output = "Action dialog: " + actionName + "\n";
        _.forEach(routeData.dialogFields, function (value, paramId) {
            output += FriendlyNameForParam(invokable, paramId) + ": ";
            var param = invokable.parameters()[paramId];
            output += renderFieldValue(param, value, mask);
            output += "\n";
        });
        return output;
    }
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.viewmodelfactory.js.map