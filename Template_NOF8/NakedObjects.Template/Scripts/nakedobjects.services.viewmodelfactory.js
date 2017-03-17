/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.userMessages.config.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var CollectionMember = NakedObjects.Models.CollectionMember;
    var DomainObjectRepresentation = NakedObjects.Models.DomainObjectRepresentation;
    var Value = NakedObjects.Models.Value;
    var PropertyMember = NakedObjects.Models.PropertyMember;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var HttpStatusCode = NakedObjects.Models.HttpStatusCode;
    var EntryType = NakedObjects.Models.EntryType;
    var toUtcDate = NakedObjects.Models.toUtcDate;
    var isDateOrDateTime = NakedObjects.Models.isDateOrDateTime;
    var ActionRepresentation = NakedObjects.Models.ActionRepresentation;
    var dirtyMarker = NakedObjects.Models.dirtyMarker;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    var InvokableActionMember = NakedObjects.Models.InvokableActionMember;
    var isTime = NakedObjects.Models.isTime;
    var toTime = NakedObjects.Models.toTime;
    NakedObjects.app.service("viewModelFactory", function ($q, $timeout, $location, $filter, $cacheFactory, $rootScope, $route, $http, repLoader, color, context, mask, urlManager, focusManager, navigation, clickHandler, commandFactory, error, ciceroRenderer) {
        var viewModelFactory = this;
        viewModelFactory.errorViewModel = function (error) {
            var errorViewModel = new NakedObjects.ErrorViewModel();
            errorViewModel.originalError = error;
            if (error) {
                errorViewModel.title = error.title;
                errorViewModel.description = error.description;
                errorViewModel.errorCode = error.errorCode;
                errorViewModel.message = error.message;
                var stackTrace = error.stackTrace;
                errorViewModel.stackTrace = stackTrace && stackTrace.length !== 0 ? stackTrace : null;
                errorViewModel.isConcurrencyError =
                    error.category === ErrorCategory.HttpClientError &&
                        error.httpErrorCode === HttpStatusCode.PreconditionFailed;
            }
            errorViewModel.description = errorViewModel.description || "No description available";
            errorViewModel.errorCode = errorViewModel.errorCode || "No code available";
            errorViewModel.message = errorViewModel.message || "No message available";
            errorViewModel.stackTrace = errorViewModel.stackTrace || ["No stack trace available"];
            return errorViewModel;
        };
        function initLinkViewModel(linkViewModel, linkRep) {
            linkViewModel.title = linkRep.title() + dirtyMarker(context, linkRep.getOid());
            linkViewModel.link = linkRep;
            linkViewModel.domainType = linkRep.type().domainType;
            // for dropping 
            var value = new Value(linkRep);
            linkViewModel.value = value.toString();
            linkViewModel.reference = value.toValueString();
            linkViewModel.selectedChoice = NakedObjects.ChoiceViewModel.create(value, "");
            linkViewModel.draggableType = linkViewModel.domainType;
            color.toColorNumberFromHref(linkRep.href()).
                then(function (c) { return linkViewModel.color = "" + NakedObjects.linkColor + c; }).
                catch(function (reject) { return error.handleError(reject); });
            linkViewModel.canDropOn = function (targetType) { return context.isSubTypeOf(linkViewModel.domainType, targetType); };
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
            initLinkViewModel(linkViewModel, linkRep);
            linkViewModel.doClick = function () {
                // because may be clicking on menu already open so want to reset focus             
                urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
                focusManager.setCurrentPane(paneId);
                focusManager.focusOverrideOff();
                focusManager.focusOn(NakedObjects.FocusTarget.SubAction, 0, paneId);
            };
            return linkViewModel;
        };
        viewModelFactory.itemViewModel = function (linkRep, paneId, selected, id) {
            var itemViewModel = new NakedObjects.ItemViewModel();
            initLinkViewModel(itemViewModel, linkRep);
            itemViewModel.selected = selected;
            itemViewModel.selectionChange = function (index) {
                context.updateValues();
                urlManager.setItemSelected(index, itemViewModel.selected, id, paneId);
                focusManager.focusOverrideOn(NakedObjects.FocusTarget.CheckBox, index + 1, paneId);
            };
            itemViewModel.doClick = function (right) {
                var currentPane = clickHandler.pane(paneId, right);
                focusManager.setCurrentPane(currentPane);
                urlManager.setItem(linkRep, currentPane);
            };
            var members = linkRep.members();
            if (members) {
                itemViewModel.tableRowViewModel = viewModelFactory.tableRowViewModel(members, paneId);
                itemViewModel.tableRowViewModel.title = itemViewModel.title;
            }
            return itemViewModel;
        };
        viewModelFactory.recentItemViewModel = function (obj, linkRep, paneId, selected) {
            var recentItemViewModel = viewModelFactory.itemViewModel(linkRep, paneId, selected, "");
            recentItemViewModel.friendlyName = obj.extensions().friendlyName();
            return recentItemViewModel;
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
            actionViewModel.presentationHint = actionRep.extensions().presentationHint();
            actionViewModel.menuPath = actionRep.extensions().menuPath() || "";
            actionViewModel.disabled = function () { return !!actionRep.disabledReason(); };
            actionViewModel.description = actionViewModel.disabled() ? actionRep.disabledReason() : actionRep.extensions().description();
            actionViewModel.parameters = function () {
                // don't use actionRep directly as it may change and we've closed around the original value
                var parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), function (p) { return !p.isCollectionContributed(); });
                return _.map(parameters, function (parm) { return viewModelFactory.parameterViewModel(parm, parms[parm.id()], paneId); });
            };
            actionViewModel.execute = function (pps, right) {
                var parmMap = _.zipObject(_.map(pps, function (p) { return p.id; }), _.map(pps, function (p) { return p.getValue(); }));
                _.forEach(pps, function (p) { return urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId); });
                return context.getInvokableAction(actionViewModel.actionRep).then(function (details) { return context.invokeAction(details, parmMap, paneId, clickHandler.pane(paneId, right), actionViewModel.gotoResult); });
            };
            // form actions should never show dialogs
            var showDialog = function () { return actionRep.extensions().hasParams() && (routeData.interactionMode !== NakedObjects.InteractionMode.Form); };
            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = showDialog() ?
                function (right) {
                    focusManager.setCurrentPane(paneId);
                    focusManager.focusOverrideOff();
                    // clear any previous dialog so we don't pick up values from it
                    context.clearDialogValues(paneId);
                    urlManager.setDialogOrMultiLineDialog(actionRep, paneId);
                    focusManager.focusOn(NakedObjects.FocusTarget.Dialog, 0, paneId); // in case dialog is already open
                } :
                function (right) {
                    focusManager.focusOverrideOff();
                    var pps = actionViewModel.parameters();
                    actionViewModel.execute(pps, right).
                        then(function (actionResult) {
                        // if expect result and no warning from server generate one here
                        if (actionResult.shouldExpectResult() && !actionResult.warningsOrMessages()) {
                            $rootScope.$broadcast(NakedObjects.geminiWarningEvent, [NakedObjects.noResultMessage]);
                        }
                    }).
                        catch(function (reject) {
                        var display = function (em) { return vm.setMessage(em.invalidReason() || em.warningMessage); };
                        error.handleErrorAndDisplayMessages(reject, display);
                    });
                };
            actionViewModel.makeInvokable = function (details) { return actionViewModel.invokableActionRep = details; };
            return actionViewModel;
        };
        viewModelFactory.handleErrorResponse = function (err, messageViewModel, valueViewModels) {
            var requiredFieldsMissing = false; // only show warning message if we have nothing else 
            var fieldValidationErrors = false;
            var contributedParameterErrorMsg = "";
            _.each(err.valuesMap(), function (errorValue, k) {
                var valueViewModel = _.find(valueViewModels, function (vvm) { return vvm.id === k; });
                if (valueViewModel) {
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
                else {
                    // no matching parm for message - this can happen in contributed actions 
                    // make the message a dialog level warning.                               
                    contributedParameterErrorMsg = errorValue.invalidReason;
                }
            });
            var msg = contributedParameterErrorMsg || err.invalidReason() || "";
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
            }).
                catch(function (reject) { return error.handleError(reject); });
        }
        ;
        function validate(rep, vm, modelValue, viewValue, mandatoryOnly) {
            var message = mandatoryOnly ? NakedObjects.Models.validateMandatory(rep, viewValue) : NakedObjects.Models.validate(rep, modelValue, viewValue, vm.localFilter);
            if (message !== NakedObjects.mandatory) {
                vm.setMessage(message);
            }
            else {
                vm.resetMessage();
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
        function setScalarValueInView(vm, propertyRep, value) {
            if (isDateOrDateTime(propertyRep)) {
                vm.value = toUtcDate(value);
            }
            else if (isTime(propertyRep)) {
                vm.value = toTime(value);
            }
            else {
                vm.value = value.scalar();
            }
        }
        function setupChoice(propertyViewModel, newValue) {
            if (propertyViewModel.entryType === EntryType.Choices) {
                var propertyRep = propertyViewModel.propertyRep;
                var choices_1 = propertyRep.choices();
                propertyViewModel.choices = _.map(choices_1, function (v, n) { return NakedObjects.ChoiceViewModel.create(v, propertyViewModel.id, n); });
                var currentChoice_1 = NakedObjects.ChoiceViewModel.create(newValue, propertyViewModel.id);
                propertyViewModel.selectedChoice = _.find(propertyViewModel.choices, function (c) { return c.valuesEqual(currentChoice_1); });
            }
            else {
                propertyViewModel.selectedChoice = NakedObjects.ChoiceViewModel.create(newValue, propertyViewModel.id);
            }
        }
        function setupScalarPropertyValue(propertyViewModel) {
            var propertyRep = propertyViewModel.propertyRep;
            propertyViewModel.type = "scalar";
            var remoteMask = propertyRep.extensions().mask();
            var localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
            propertyViewModel.localFilter = localFilter;
            // formatting also happens in in directive - at least for dates - value is now date in that case
            propertyViewModel.refresh = function (newValue) { return callIfChanged(propertyViewModel, newValue, function (value) {
                setupChoice(propertyViewModel, value);
                setScalarValueInView(propertyViewModel, propertyRep, value);
                if (propertyRep.entryType() === EntryType.Choices) {
                    if (propertyViewModel.selectedChoice) {
                        propertyViewModel.value = propertyViewModel.selectedChoice.name;
                        propertyViewModel.formattedValue = propertyViewModel.selectedChoice.name;
                    }
                }
                else if (propertyViewModel.password) {
                    propertyViewModel.formattedValue = NakedObjects.obscuredText;
                }
                else {
                    propertyViewModel.formattedValue = localFilter.filter(propertyViewModel.value);
                }
            }); };
        }
        viewModelFactory.propertyTableViewModel = function (propertyRep, id, paneId) {
            var tableRowColumnViewModel = new NakedObjects.TableRowColumnViewModel();
            tableRowColumnViewModel.title = propertyRep.extensions().friendlyName();
            tableRowColumnViewModel.id = id;
            if (propertyRep instanceof CollectionMember) {
                var size = propertyRep.size();
                tableRowColumnViewModel.formattedValue = getCollectionDetails(size);
                tableRowColumnViewModel.value = "";
                tableRowColumnViewModel.type = "scalar";
                tableRowColumnViewModel.returnType = "string";
            }
            if (propertyRep instanceof PropertyMember) {
                var isPassword = propertyRep.extensions().dataType() === "password";
                var value = propertyRep.value();
                tableRowColumnViewModel.returnType = propertyRep.extensions().returnType();
                if (propertyRep.isScalar()) {
                    tableRowColumnViewModel.type = "scalar";
                    setScalarValueInView(tableRowColumnViewModel, propertyRep, value);
                    var remoteMask = propertyRep.extensions().mask();
                    var localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
                    if (propertyRep.entryType() === EntryType.Choices) {
                        var currentChoice_2 = NakedObjects.ChoiceViewModel.create(value, id);
                        var choices_2 = _.map(propertyRep.choices(), function (v, n) { return NakedObjects.ChoiceViewModel.create(v, id, n); });
                        var choice = _.find(choices_2, function (c) { return c.valuesEqual(currentChoice_2); });
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
        function getDigest(propertyRep) {
            var parent = propertyRep.parent;
            if (parent instanceof DomainObjectRepresentation) {
                if (parent.isTransient()) {
                    return parent.etagDigest;
                }
            }
            return null;
        }
        function setupPropertyAutocomplete(propertyViewModel, parentValues) {
            var propertyRep = propertyViewModel.propertyRep;
            propertyViewModel.prompt = function (searchTerm) {
                var createcvm = _.partial(createChoiceViewModels, propertyViewModel.id, searchTerm);
                var digest = getDigest(propertyRep);
                return context.autoComplete(propertyRep, propertyViewModel.id, parentValues, searchTerm, digest).then(createcvm);
            };
            propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
            propertyViewModel.description = propertyViewModel.description || NakedObjects.autoCompletePrompt;
        }
        function setupPropertyConditionalChoices(propertyViewModel) {
            var propertyRep = propertyViewModel.propertyRep;
            propertyViewModel.conditionalChoices = function (args) {
                var createcvm = _.partial(createChoiceViewModels, propertyViewModel.id, null);
                var digest = getDigest(propertyRep);
                return context.conditionalChoices(propertyRep, propertyViewModel.id, function () { return {}; }, args, digest).then(createcvm);
            };
            propertyViewModel.promptArguments = _.fromPairs(_.map(propertyRep.promptLink().arguments(), function (v, key) { return [key, new Value(v.value)]; }));
        }
        function callIfChanged(propertyViewModel, newValue, doRefresh) {
            var propertyRep = propertyViewModel.propertyRep;
            var value = newValue || propertyRep.value();
            if (propertyViewModel.currentValue == null || value.toValueString() !== propertyViewModel.currentValue.toValueString()) {
                doRefresh(value);
                propertyViewModel.currentValue = value;
            }
        }
        function setupReferencePropertyValue(propertyViewModel) {
            var propertyRep = propertyViewModel.propertyRep;
            propertyViewModel.refresh = function (newValue) { return callIfChanged(propertyViewModel, newValue, function (value) {
                setupChoice(propertyViewModel, value);
                setupReference(propertyViewModel, value, propertyRep);
            }); };
        }
        viewModelFactory.propertyViewModel = function (propertyRep, id, previousValue, paneId, parentValues) {
            var propertyViewModel = new NakedObjects.PropertyViewModel(propertyRep, color, error);
            propertyViewModel.id = id;
            propertyViewModel.onPaneId = paneId;
            propertyViewModel.argId = "" + id.toLowerCase();
            propertyViewModel.paneArgId = "" + propertyViewModel.argId + paneId;
            if (propertyRep.attachmentLink() != null) {
                propertyViewModel.attachment = viewModelFactory.attachmentViewModel(propertyRep, paneId);
            }
            var fieldEntryType = propertyViewModel.entryType;
            if (fieldEntryType === EntryType.AutoComplete) {
                setupPropertyAutocomplete(propertyViewModel, parentValues);
            }
            if (fieldEntryType === EntryType.ConditionalChoices) {
                setupPropertyConditionalChoices(propertyViewModel);
            }
            if (propertyRep.isScalar()) {
                setupScalarPropertyValue(propertyViewModel);
            }
            else {
                // is reference
                setupReferencePropertyValue(propertyViewModel);
            }
            propertyViewModel.refresh(previousValue);
            if (!previousValue) {
                propertyViewModel.originalValue = propertyViewModel.getValue();
            }
            var required = propertyViewModel.optional ? "" : "* ";
            propertyViewModel.description = required + propertyViewModel.description;
            propertyViewModel.isDirty = function () { return !!previousValue || propertyViewModel.getValue().toValueString() !== propertyViewModel.originalValue.toValueString(); };
            propertyViewModel.validate = _.partial(validate, propertyRep, propertyViewModel);
            propertyViewModel.canDropOn = function (targetType) { return context.isSubTypeOf(propertyViewModel.returnType, targetType); };
            propertyViewModel.drop = _.partial(drop, propertyViewModel);
            propertyViewModel.doClick = function (right) { return urlManager.setProperty(propertyRep, clickHandler.pane(paneId, right)); };
            return propertyViewModel;
        };
        function setupParameterChoices(parmViewModel) {
            var parmRep = parmViewModel.parameterRep;
            parmViewModel.choices = _.map(parmRep.choices(), function (v, n) { return NakedObjects.ChoiceViewModel.create(v, parmRep.id(), n); });
        }
        function setupParameterAutocomplete(parmViewModel) {
            var parmRep = parmViewModel.parameterRep;
            parmViewModel.prompt = function (searchTerm) {
                var createcvm = _.partial(createChoiceViewModels, parmViewModel.id, searchTerm);
                return context.autoComplete(parmRep, parmViewModel.id, function () { return {}; }, searchTerm).
                    then(createcvm);
            };
            parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
            parmViewModel.description = parmViewModel.description || NakedObjects.autoCompletePrompt;
        }
        function setupParameterFreeformReference(parmViewModel, previousValue) {
            var parmRep = parmViewModel.parameterRep;
            parmViewModel.description = parmViewModel.description || NakedObjects.dropPrompt;
            var val = previousValue && !previousValue.isNull() ? previousValue : parmRep.default();
            if (!val.isNull() && val.isReference()) {
                parmViewModel.reference = val.link().href();
                parmViewModel.selectedChoice = NakedObjects.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
            }
        }
        function setupParameterConditionalChoices(parmViewModel) {
            var parmRep = parmViewModel.parameterRep;
            parmViewModel.conditionalChoices = function (args) {
                var createcvm = _.partial(createChoiceViewModels, parmViewModel.id, null);
                return context.conditionalChoices(parmRep, parmViewModel.id, function () { return {}; }, args).
                    then(createcvm);
            };
            parmViewModel.promptArguments = _.fromPairs(_.map(parmRep.promptLink().arguments(), function (v, key) { return [key, new Value(v.value)]; }));
        }
        function setupParameterSelectedChoices(parmViewModel, previousValue) {
            var parmRep = parmViewModel.parameterRep;
            var fieldEntryType = parmViewModel.entryType;
            function setCurrentChoices(vals) {
                var choicesToSet = _.map(vals.list(), function (val) { return NakedObjects.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null); });
                if (fieldEntryType === EntryType.MultipleChoices) {
                    parmViewModel.selectedMultiChoices = _.filter(parmViewModel.choices, function (c) { return _.some(choicesToSet, function (choiceToSet) { return c.valuesEqual(choiceToSet); }); });
                }
                else {
                    parmViewModel.selectedMultiChoices = choicesToSet;
                }
            }
            function setCurrentChoice(val) {
                var choiceToSet = NakedObjects.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
                if (fieldEntryType === EntryType.Choices) {
                    parmViewModel.selectedChoice = _.find(parmViewModel.choices, function (c) { return c.valuesEqual(choiceToSet); });
                }
                else {
                    if (!parmViewModel.selectedChoice || parmViewModel.selectedChoice.getValue().toValueString() !== choiceToSet.getValue().toValueString()) {
                        parmViewModel.selectedChoice = choiceToSet;
                    }
                }
            }
            parmViewModel.refresh = function (newValue) {
                if (newValue || parmViewModel.dflt) {
                    var toSet = newValue || parmRep.default();
                    if (fieldEntryType === EntryType.MultipleChoices || fieldEntryType === EntryType.MultipleConditionalChoices ||
                        parmViewModel.isCollectionContributed) {
                        setCurrentChoices(toSet);
                    }
                    else {
                        setCurrentChoice(toSet);
                    }
                }
            };
            parmViewModel.refresh(previousValue);
        }
        function setupParameterSelectedValue(parmViewModel, previousValue) {
            var parmRep = parmViewModel.parameterRep;
            var returnType = parmRep.extensions().returnType();
            parmViewModel.refresh = function (newValue) {
                if (returnType === "boolean") {
                    var valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                    var bValueToSet = NakedObjects.toTriStateBoolean(valueToSet);
                    parmViewModel.value = bValueToSet;
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
            };
            parmViewModel.refresh(previousValue);
        }
        function getRequiredIndicator(parmViewModel) {
            return parmViewModel.optional || typeof parmViewModel.value === "boolean" ? "" : "* ";
        }
        viewModelFactory.parameterViewModel = function (parmRep, previousValue, paneId) {
            var parmViewModel = new NakedObjects.ParameterViewModel(parmRep, paneId, color, error);
            var remoteMask = parmRep.extensions().mask();
            var fieldEntryType = parmViewModel.entryType;
            if (fieldEntryType === EntryType.FreeForm && parmViewModel.type === "scalar") {
                var lf = void 0;
                if (remoteMask) {
                    lf = mask.toLocalFilter(remoteMask, parmRep.extensions().format()) || mask.defaultLocalFilter(parmRep.extensions().format());
                }
                else {
                    lf = mask.defaultLocalFilter(parmRep.extensions().format());
                }
                parmViewModel.localFilter = lf;
            }
            if (fieldEntryType === EntryType.Choices || fieldEntryType === EntryType.MultipleChoices) {
                setupParameterChoices(parmViewModel);
            }
            if (fieldEntryType === EntryType.AutoComplete) {
                setupParameterAutocomplete(parmViewModel);
            }
            if (fieldEntryType === EntryType.FreeForm && parmViewModel.type === "ref") {
                setupParameterFreeformReference(parmViewModel, previousValue);
            }
            if (fieldEntryType === EntryType.ConditionalChoices || fieldEntryType === EntryType.MultipleConditionalChoices) {
                setupParameterConditionalChoices(parmViewModel);
            }
            if (fieldEntryType !== EntryType.FreeForm || parmViewModel.isCollectionContributed) {
                setupParameterSelectedChoices(parmViewModel, previousValue);
            }
            else {
                setupParameterSelectedValue(parmViewModel, previousValue);
            }
            parmViewModel.description = getRequiredIndicator(parmViewModel) + parmViewModel.description;
            parmViewModel.validate = _.partial(validate, parmRep, parmViewModel);
            parmViewModel.drop = _.partial(drop, parmViewModel);
            return parmViewModel;
        };
        viewModelFactory.getItems = function (links, tableView, routeData, listViewModel) {
            var collection = listViewModel instanceof NakedObjects.CollectionViewModel ? listViewModel : null;
            var id = collection ? collection.id : "";
            var selectedItems = routeData.selectedCollectionItems[id];
            var items = _.map(links, function (link, i) { return viewModelFactory.itemViewModel(link, routeData.paneId, selectedItems && selectedItems[i], id); });
            if (tableView) {
                var getActionExtensions = routeData.objectId ?
                    function () { return context.getActionExtensionsFromObject(routeData.paneId, ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId); } :
                    function () { return context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId); };
                var getExtensions = listViewModel instanceof NakedObjects.CollectionViewModel ? function () { return $q.when(listViewModel.collectionRep.extensions()); } : getActionExtensions;
                // clear existing header 
                listViewModel.header = null;
                if (items.length > 0) {
                    getExtensions().
                        then(function (ext) {
                        _.forEach(items, function (itemViewModel) {
                            itemViewModel.tableRowViewModel.hasTitle = ext.tableViewTitle();
                            itemViewModel.tableRowViewModel.title = itemViewModel.title;
                            itemViewModel.tableRowViewModel.conformColumns(ext.tableViewColumns());
                        });
                        if (!listViewModel.header) {
                            var firstItem_1 = items[0].tableRowViewModel;
                            var propertiesHeader = _.map(firstItem_1.properties, function (p, i) {
                                var match = _.find(items, function (item) { return item.tableRowViewModel.properties[i].title; });
                                return match ? match.tableRowViewModel.properties[i].title : firstItem_1.properties[i].id;
                            });
                            listViewModel.header = firstItem_1.hasTitle ? [""].concat(propertiesHeader) : propertiesHeader;
                            focusManager.focusOverrideOff();
                            focusManager.focusOn(NakedObjects.FocusTarget.TableItem, 0, routeData.paneId);
                        }
                    }).
                        catch(function (reject) { return error.handleError(reject); });
                }
            }
            return items;
        };
        function getCollectionDetails(count) {
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
            var collectionViewModel = new NakedObjects.CollectionViewModel(context, viewModelFactory, urlManager, focusManager, error, $q);
            var itemLinks = collectionRep.value();
            var paneId = routeData.paneId;
            var size = collectionRep.size();
            collectionViewModel.collectionRep = collectionRep;
            collectionViewModel.onPaneId = paneId;
            collectionViewModel.title = collectionRep.extensions().friendlyName();
            collectionViewModel.presentationHint = collectionRep.extensions().presentationHint();
            collectionViewModel.pluralName = collectionRep.extensions().pluralName();
            collectionViewModel.id = collectionRep.collectionId().toLowerCase();
            color.toColorNumberFromType(collectionRep.extensions().elementType()).
                then(function (c) { return collectionViewModel.color = "" + NakedObjects.linkColor + c; }).
                catch(function (reject) { return error.handleError(reject); });
            collectionViewModel.refresh = function (routeData, resetting) {
                var state = routeData.collections[collectionRep.collectionId()];
                // collections are always shown as summary on transient 
                if (routeData.interactionMode === NakedObjects.InteractionMode.Transient) {
                    state = NakedObjects.CollectionViewState.Summary;
                }
                if (state == null) {
                    state = getDefaultTableState(collectionRep.extensions());
                }
                collectionViewModel.editing = routeData.interactionMode === NakedObjects.InteractionMode.Edit;
                // clear any previous messages
                collectionViewModel.resetMessage();
                if (resetting || state !== collectionViewModel.currentState) {
                    if (size > 0 || size == null) {
                        collectionViewModel.mayHaveItems = true;
                    }
                    collectionViewModel.details = getCollectionDetails(size);
                    var getDetails = itemLinks == null || state === NakedObjects.CollectionViewState.Table;
                    var actions = collectionRep.actionMembers();
                    collectionViewModel.setActions(actions, routeData);
                    if (resetting) {
                        // need to clear the cache so that next time we get details 
                        // we will get fresh 
                        context.clearCachedCollection(collectionRep);
                    }
                    if (state === NakedObjects.CollectionViewState.Summary) {
                        collectionViewModel.items = [];
                    }
                    else if (getDetails) {
                        context.getCollectionDetails(collectionRep, state, resetting)
                            .then(function (details) {
                            collectionViewModel.items = viewModelFactory.getItems(details.value(), state === NakedObjects.CollectionViewState.Table, routeData, collectionViewModel);
                            collectionViewModel.details = getCollectionDetails(collectionViewModel.items.length);
                            collectionViewModel.allSelected = _.every(collectionViewModel.items, function (item) { return item.selected; });
                        })
                            .catch(function (reject) { return error.handleError(reject); });
                    }
                    else {
                        collectionViewModel.items = viewModelFactory.getItems(itemLinks, state === NakedObjects.CollectionViewState.Table, routeData, collectionViewModel);
                        collectionViewModel.allSelected = _.every(collectionViewModel.items, function (item) { return item.selected; });
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
                else {
                    collectionViewModel.allSelected = _.every(collectionViewModel.items, function (item) { return item.selected; });
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
                    error.handleError(reject);
                });
            };
            return collectionPlaceholderViewModel;
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
            recentItemsViewModel.clear = function () {
                context.clearRecentlyViewed();
                urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
            };
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
                    context.updateValues();
                    var newPane = clickHandler.pane(1, right);
                    if (NakedObjects.leftClickHomeAlwaysGoesToSinglePane && newPane === 1) {
                        urlManager.setHome(1);
                        urlManager.singlePane(1);
                        focusManager.refresh(1);
                    }
                    else {
                        urlManager.setHome(newPane);
                    }
                };
                tvm_1.goBack = function () {
                    focusManager.focusOverrideOff();
                    context.updateValues();
                    navigation.back();
                };
                tvm_1.goForward = function () {
                    focusManager.focusOverrideOff();
                    context.updateValues();
                    navigation.forward();
                };
                tvm_1.swapPanes = function () {
                    if (!tvm_1.swapDisabled()) {
                        $rootScope.$broadcast(NakedObjects.geminiPaneSwapEvent);
                        context.updateValues();
                        context.swapCurrentObjects();
                        urlManager.swapPanes();
                    }
                };
                tvm_1.singlePane = function (right) {
                    context.updateValues();
                    urlManager.singlePane(clickHandler.pane(1, right));
                    focusManager.refresh(1);
                };
                tvm_1.cicero = function () {
                    context.updateValues();
                    urlManager.singlePane(clickHandler.pane(1));
                    urlManager.cicero();
                };
                tvm_1.recent = function (right) {
                    context.updateValues();
                    focusManager.focusOverrideOff();
                    urlManager.setRecent(clickHandler.pane(1, right));
                };
                tvm_1.swapDisabled = function () {
                    return urlManager.isMultiLineDialog();
                };
                tvm_1.logOff = function () {
                    context.getUser().
                        then(function (u) {
                        if (window.confirm(NakedObjects.logOffMessage(u.userName() || "Unknown"))) {
                            var config = {
                                withCredentials: true,
                                url: NakedObjects.logoffUrl,
                                method: "POST",
                                cache: false
                            };
                            // logoff server
                            $http(config);
                            // logoff client without waiting for server
                            $rootScope.$broadcast(NakedObjects.geminiLogoffEvent);
                            $timeout(function () { return window.location.href = NakedObjects.postLogoffUrl; });
                        }
                    }).
                        catch(function (reject) { return error.handleError(reject); });
                };
                tvm_1.applicationProperties = function () {
                    context.updateValues();
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
                context.getUser().
                    then(function (user) { return tvm_1.userName = user.userName(); }).
                    catch(function (reject) { return error.handleError(reject); });
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
                cvm.renderHome = _.partial(ciceroRenderer.renderHome, cvm);
                cvm.renderObject = _.partial(ciceroRenderer.renderObject, cvm);
                cvm.renderList = _.partial(ciceroRenderer.renderList, cvm);
                cvm.renderError = _.partial(ciceroRenderer.renderError, cvm);
            }
            return cvm;
        };
        function logoff() {
            cvm = null;
        }
        $rootScope.$on(NakedObjects.geminiLogoffEvent, function () { return logoff(); });
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.viewmodelfactory.js.map