import * as Models from "./models";
import * as Nakedobjectsviewmodels from "./nakedobjects.viewmodels";
import * as Nakedobjectsconfig from "./nakedobjects.config";
import * as Focusmanagerservice from "./focus-manager.service";
import * as Nakedobjectsroutedata from "./nakedobjects.routedata";
import * as Nakedobjectsconstants from "./nakedobjects.constants";
import * as Usermessagesconfig from "./user-messages.config";
import * as Contextservice from "./context.service";
import * as Urlmanagerservice from "./urlmanager.service";
import * as Colorservice from "./color.service";
import * as Clickhandlerservice from "./click-handler.service";
import * as Errorservice from "./error.service";
import * as Maskservice from "./mask.service";
import { Injectable } from '@angular/core';
import * as _ from "lodash";

@Injectable()
export class ViewModelFactory {

    constructor(private context: Contextservice.Context,
        private urlManager: Urlmanagerservice.UrlManager,
        private color: Colorservice.Color,
        private error: Errorservice.Error,
        private clickHandler: Clickhandlerservice.ClickHandlerService,
        private focusManager: Focusmanagerservice.FocusManager,
        private mask : Maskservice.Mask
    ) { }


    errorViewModel = (error: Models.ErrorWrapper) => {
        const errorViewModel = new Nakedobjectsviewmodels.ErrorViewModel();

        errorViewModel.originalError = error;
        if (error) {
            errorViewModel.title = error.title;
            errorViewModel.description = error.description;
            errorViewModel.errorCode = error.errorCode;
            errorViewModel.message = error.message;
            const stackTrace = error.stackTrace;

            errorViewModel.stackTrace = stackTrace && stackTrace.length !== 0 ? stackTrace : null;

            errorViewModel.isConcurrencyError =
                error.category === Models.ErrorCategory.HttpClientError &&
                error.httpErrorCode === Models.HttpStatusCode.PreconditionFailed;
        }

        errorViewModel.description = errorViewModel.description || "No description available";
        errorViewModel.errorCode = errorViewModel.errorCode || "No code available";
        errorViewModel.message = errorViewModel.message || "No message available";
        errorViewModel.stackTrace = errorViewModel.stackTrace || ["No stack trace available"];

        return errorViewModel;
    };

    private initLinkViewModel(linkViewModel: Nakedobjectsviewmodels.LinkViewModel, linkRep: Models.Link) {
        linkViewModel.title = linkRep.title() + Models.dirtyMarker(this.context, linkRep.getOid());
        linkViewModel.link = linkRep;
        linkViewModel.domainType = linkRep.type().domainType;

        // for dropping 
        const value = new Models.Value(linkRep);

        linkViewModel.value = value.toString();
        linkViewModel.reference = value.toValueString();
        linkViewModel.selectedChoice = Nakedobjectsviewmodels.ChoiceViewModel.create(value, "");
        linkViewModel.draggableType = linkViewModel.domainType;

        this.color.toColorNumberFromHref(linkRep.href()).
            then(c => linkViewModel.color = `${Nakedobjectsconfig.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        linkViewModel.canDropOn = (targetType: string) => this.context.isSubTypeOf(linkViewModel.domainType, targetType);
    }

    private createChoiceViewModels = (id: string, searchTerm: string, choices: _.Dictionary<Models.Value>) =>
        Promise.resolve(_.map(choices, (v, k) => Nakedobjectsviewmodels.ChoiceViewModel.create(v, id, k, searchTerm)));

    attachmentViewModel = (propertyRep: Models.PropertyMember, paneId: number) => {
        const parent = propertyRep.parent as Models.DomainObjectRepresentation;
        const avm = Nakedobjectsviewmodels.AttachmentViewModel.create(propertyRep.attachmentLink(), parent, this.context, paneId);
        avm.doClick = (right?: boolean) => this.urlManager.setAttachment(avm.link, this.clickHandler.pane(paneId, right));

        return avm;
    };

    linkViewModel = (linkRep: Models.Link, paneId: number) => {
        const linkViewModel = new Nakedobjectsviewmodels.LinkViewModel();
        this.initLinkViewModel(linkViewModel, linkRep);

        linkViewModel.doClick = () => {
            // because may be clicking on menu already open so want to reset focus             
            this.urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
            this.focusManager.setCurrentPane(paneId);
            this.focusManager.focusOverrideOff();
            this.focusManager.focusOn(Focusmanagerservice.FocusTarget.SubAction, 0, paneId);
        };

        return linkViewModel as Nakedobjectsviewmodels.ILinkViewModel;
    };

    itemViewModel = (linkRep: Models.Link, paneId: number, selected: boolean) => {
        const itemViewModel = new Nakedobjectsviewmodels.ItemViewModel();
        this.initLinkViewModel(itemViewModel, linkRep);

        itemViewModel.selected = selected;

        itemViewModel.selectionChange = (index) => {
            this.context.updateValues();
            this.urlManager.setListItem(index, itemViewModel.selected, paneId);
            this.focusManager.focusOverrideOn(Focusmanagerservice.FocusTarget.CheckBox, index + 1, paneId);
        };

        itemViewModel.doClick = (right?: boolean) => {
            const currentPane = this.clickHandler.pane(paneId, right);
            this.focusManager.setCurrentPane(currentPane);
            this.urlManager.setItem(linkRep, currentPane);
        };

        const members = linkRep.members();

        if (members) {
            itemViewModel.tableRowViewModel = this.tableRowViewModel(members, paneId);
            itemViewModel.tableRowViewModel.title = itemViewModel.title;
        }

        return itemViewModel;
    };

    recentItemViewModel = (obj: Models.DomainObjectRepresentation, linkRep: Models.Link, paneId: number, selected: boolean) => {
        const recentItemViewModel = this.itemViewModel(linkRep, paneId, selected) as Nakedobjectsviewmodels.ILinkViewModel;
        (recentItemViewModel as Nakedobjectsviewmodels.IRecentItemViewModel).friendlyName = obj.extensions().friendlyName();
        return recentItemViewModel as Nakedobjectsviewmodels.IRecentItemViewModel;
    };

    actionViewModel = (actionRep: Models.ActionMember | Models.ActionRepresentation, vm: Nakedobjectsviewmodels.IMessageViewModel, routeData: Nakedobjectsroutedata.PaneRouteData) => {
        const actionViewModel = new Nakedobjectsviewmodels.ActionViewModel();

        const parms = routeData.actionParams;
        const paneId = routeData.paneId;

        actionViewModel.actionRep = actionRep;

        if (actionRep instanceof Models.ActionRepresentation || actionRep instanceof Models.InvokableActionMember) {
            actionViewModel.invokableActionRep = actionRep;
        }

        actionViewModel.title = actionRep.extensions().friendlyName();
        actionViewModel.presentationHint = actionRep.extensions().presentationHint();
        actionViewModel.menuPath = actionRep.extensions().menuPath() || "";
        actionViewModel.disabled = () => !!actionRep.disabledReason();
        actionViewModel.description = actionViewModel.disabled() ? actionRep.disabledReason() : actionRep.extensions().description();

        actionViewModel.parameters = () => {
            // don't use actionRep directly as it may change and we've closed around the original value
            const parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Models.Parameter>;
            return _.map(parameters, parm => this.parameterViewModel(parm, parms[parm.id()], paneId));
        };

        actionViewModel.execute = (pps: Nakedobjectsviewmodels.ParameterViewModel[], right?: boolean) => {
            const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
            _.forEach(pps, p => this.urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId));
            return this.context.getInvokableAction(actionViewModel.actionRep).then(details => this.context.invokeAction(details, parmMap, paneId, this.clickHandler.pane(paneId, right)));
        };

        // form actions should never show dialogs
        const showDialog = () => actionRep.extensions().hasParams() && (routeData.interactionMode !== Nakedobjectsroutedata.InteractionMode.Form);

        // open dialog on current pane always - invoke action goes to pane indicated by click
        actionViewModel.doInvoke = showDialog() ?
            (right?: boolean) => {
                this.focusManager.setCurrentPane(paneId);
                this.focusManager.focusOverrideOff();
                // clear any previous dialog so we don't pick up values from it
                this.context.clearDialogValues(paneId);
                this.urlManager.setDialog(actionRep.actionId(), paneId);
                this.focusManager.focusOn(Focusmanagerservice.FocusTarget.Dialog, 0, paneId); // in case dialog is already open
            } :
            (right?: boolean) => {
                this.focusManager.focusOverrideOff();
                const pps = actionViewModel.parameters();
                actionViewModel.execute(pps, right).
                    then((actionResult: Models.ActionResultRepresentation) => {
                        // if expect result and no warning from server generate one here
                        if (actionResult.shouldExpectResult() && !actionResult.warningsOrMessages()) {
                            //$rootScope.$broadcast(Nakedobjectsconstants.geminiWarningEvent, [Usermessagesconfig.noResultMessage]);
                        }
                    }).
                    catch((reject: Models.ErrorWrapper) => {
                        const display = (em: Models.ErrorMap) => vm.setMessage(em.invalidReason() || em.warningMessage);
                        this.error.handleErrorAndDisplayMessages(reject, display);
                    });
            };

        actionViewModel.makeInvokable = (details: Models.IInvokableAction) => actionViewModel.invokableActionRep = details;

        return actionViewModel as Nakedobjectsviewmodels.IActionViewModel;
    };


    handleErrorResponse = (err: Models.ErrorMap, messageViewModel: Nakedobjectsviewmodels.IMessageViewModel, valueViewModels: Nakedobjectsviewmodels.IFieldViewModel[]) => {

        let requiredFieldsMissing = false; // only show warning message if we have nothing else 
        let fieldValidationErrors = false;

        _.each(valueViewModels, valueViewModel => {
            const errorValue = err.valuesMap()[valueViewModel.id];

            if (errorValue) {
                const reason = errorValue.invalidReason;
                if (reason) {
                    if (reason === "Mandatory") {
                        const r = "REQUIRED";
                        requiredFieldsMissing = true;
                        valueViewModel.description = valueViewModel.description.indexOf(r) === 0 ? valueViewModel.description : `${r} ${valueViewModel.description}`;
                    } else {
                        valueViewModel.setMessage(reason);
                        fieldValidationErrors = true;
                    }
                }
            }
        });

        let msg = err.invalidReason() || "";
        if (requiredFieldsMissing) msg = `${msg} Please complete REQUIRED fields. `;
        if (fieldValidationErrors) msg = `${msg} See field validation message(s). `;

        if (!msg) msg = err.warningMessage;
        messageViewModel.setMessage(msg);
    };

    private drop(vm: Nakedobjectsviewmodels.IFieldViewModel, newValue: Nakedobjectsviewmodels.IDraggableViewModel) {
        this.context.isSubTypeOf(newValue.draggableType, vm.returnType).
            then((canDrop: boolean) => {
                if (canDrop) {
                    vm.setNewValue(newValue);
                }
            }).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    };

    private validate(rep: Models.IHasExtensions, vm: Nakedobjectsviewmodels.IFieldViewModel, modelValue: any, viewValue: string, mandatoryOnly: boolean) {
        const message = mandatoryOnly ? Models.validateMandatory(rep, viewValue) : Models.validate(rep, modelValue, viewValue, vm.localFilter);

        if (message !== Usermessagesconfig.mandatory) {
            vm.setMessage(message);
        } else {
            vm.resetMessage();
        }

        vm.clientValid = !message;
        return vm.clientValid;
    };

    private setupReference(vm: Nakedobjectsviewmodels.IPropertyViewModel, value: Models.Value, rep: Models.IHasExtensions) {
        vm.type = "ref";
        if (value.isNull()) {
            vm.reference = "";
            vm.value = vm.description;
            vm.formattedValue = "";
            vm.refType = "null";
        } else {
            vm.reference = value.link().href();
            vm.value = value.toString();
            vm.formattedValue = value.toString();
            vm.refType = rep.extensions().notNavigable() ? "notNavigable" : "navigable";
        }
        if (vm.entryType === Models.EntryType.FreeForm) {
            vm.description = vm.description || Usermessagesconfig.dropPrompt;
        }
    }

    private setScalarValueInView(vm: { value: string | number | boolean | Date }, propertyRep: Models.PropertyMember, value: Models.Value) {
        if (Models.isDateOrDateTime(propertyRep)) {
            vm.value = Models.toUtcDate(value);
        } else if (Models.isTime(propertyRep)) {
            vm.value = Models.toTime(value);
        } else {
            vm.value = value.scalar();
        }
    }

    private setupChoice(propertyViewModel: Nakedobjectsviewmodels.IPropertyViewModel, newValue: Models.Value) {
        if (propertyViewModel.entryType === Models.EntryType.Choices) {
            const propertyRep = propertyViewModel.propertyRep;
            const choices = propertyRep.choices();
            propertyViewModel.choices = _.map(choices, (v, n) => Nakedobjectsviewmodels.ChoiceViewModel.create(v, propertyViewModel.id, n));

            const currentChoice = Nakedobjectsviewmodels.ChoiceViewModel.create(newValue, propertyViewModel.id);
            propertyViewModel.selectedChoice = _.find(propertyViewModel.choices, c => c.valuesEqual(currentChoice));
        } else {
            propertyViewModel.selectedChoice = Nakedobjectsviewmodels.ChoiceViewModel.create(newValue, propertyViewModel.id);
        }
    }

    private setupScalarPropertyValue(propertyViewModel: Nakedobjectsviewmodels.IPropertyViewModel) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.type = "scalar";

        const remoteMask = propertyRep.extensions().mask();
        const localFilter = this.mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
        propertyViewModel.localFilter = localFilter;
        // formatting also happens in in directive - at least for dates - value is now date in that case

        propertyViewModel.refresh = (newValue: Models.Value) => this.callIfChanged(propertyViewModel, newValue, (value: Models.Value) => {

            this.setupChoice(propertyViewModel, value);
            this.setScalarValueInView(propertyViewModel, propertyRep, value);

            if (propertyRep.entryType() === Models.EntryType.Choices) {
                if (propertyViewModel.selectedChoice) {
                    propertyViewModel.value = propertyViewModel.selectedChoice.name;
                    propertyViewModel.formattedValue = propertyViewModel.selectedChoice.name;
                }
            } else if (propertyViewModel.password) {
                propertyViewModel.formattedValue = Usermessagesconfig.obscuredText;
            } else {
                propertyViewModel.formattedValue = localFilter.filter(propertyViewModel.value);
            }
        });
    }

    propertyTableViewModel = (propertyRep: Models.PropertyMember | Models.CollectionMember, id: string, paneId: number) => {
        const tableRowColumnViewModel = new Nakedobjectsviewmodels.TableRowColumnViewModel();

        tableRowColumnViewModel.title = propertyRep.extensions().friendlyName();

        if (propertyRep instanceof Models.CollectionMember) {
            const size = propertyRep.size();

            tableRowColumnViewModel.formattedValue = this.getCollectionDetails(size);
            tableRowColumnViewModel.value = "";
            tableRowColumnViewModel.type = "scalar";
            tableRowColumnViewModel.returnType = "string";
        }

        if (propertyRep instanceof Models.PropertyMember) {
            const isPassword = propertyRep.extensions().dataType() === "password";
            const value = propertyRep.value();
            tableRowColumnViewModel.returnType = propertyRep.extensions().returnType();

            if (propertyRep.isScalar()) {
                tableRowColumnViewModel.type = "scalar";
                this.setScalarValueInView(tableRowColumnViewModel, propertyRep, value);

                const remoteMask = propertyRep.extensions().mask();
                const localFilter = this.mask.toLocalFilter(remoteMask, propertyRep.extensions().format());

                if (propertyRep.entryType() === Models.EntryType.Choices) {
                    const currentChoice = Nakedobjectsviewmodels.ChoiceViewModel.create(value, id);
                    const choices = _.map(propertyRep.choices(), (v, n) => Nakedobjectsviewmodels.ChoiceViewModel.create(v, id, n));
                    const choice = _.find(choices, c => c.valuesEqual(currentChoice));

                    if (choice) {
                        tableRowColumnViewModel.value = choice.name;
                        tableRowColumnViewModel.formattedValue = choice.name;
                    }
                } else if (isPassword) {
                    tableRowColumnViewModel.formattedValue = Usermessagesconfig.obscuredText;
                } else {
                    tableRowColumnViewModel.formattedValue = localFilter.filter(tableRowColumnViewModel.value);
                }
            } else {
                // is reference   
                tableRowColumnViewModel.type = "ref";
                tableRowColumnViewModel.formattedValue = value.isNull() ? "" : value.toString();
            }
        }

        return tableRowColumnViewModel;
    };

    private getDigest(propertyRep: Models.PropertyMember) {
        const parent = propertyRep.parent;
        if (parent instanceof Models.DomainObjectRepresentation) {
            if (parent.isTransient()) {
                return parent.etagDigest;
            }
        }
        return null;
    }


    private setupPropertyAutocomplete(propertyViewModel: Nakedobjectsviewmodels.IPropertyViewModel, parentValues: () => _.Dictionary<Models.Value>) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.prompt = (searchTerm: string) => {
            const createcvm = _.partial(this.createChoiceViewModels, propertyViewModel.id, searchTerm);
            const digest = this.getDigest(propertyRep);

            return this.context.autoComplete(propertyRep, propertyViewModel.id, parentValues, searchTerm, digest).then(createcvm);
        };
        propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
        propertyViewModel.description = propertyViewModel.description || Usermessagesconfig.autoCompletePrompt;
    }

    private setupPropertyConditionalChoices(propertyViewModel: Nakedobjectsviewmodels.IPropertyViewModel) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(this.createChoiceViewModels, propertyViewModel.id, null);
            const digest = this.getDigest(propertyRep);
            return this.context.conditionalChoices(propertyRep, propertyViewModel.id, () => <_.Dictionary<Models.Value>>{}, args, digest).then(createcvm);
        };
        propertyViewModel.promptArguments = _.fromPairs(_.map(propertyRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private callIfChanged(propertyViewModel: Nakedobjectsviewmodels.IPropertyViewModel, newValue: Models.Value, doRefresh: (newValue: Models.Value) => void) {
        const propertyRep = propertyViewModel.propertyRep;
        const value = newValue || propertyRep.value();

        if (propertyViewModel.currentValue == null || value.toValueString() !== propertyViewModel.currentValue.toValueString()) {
            doRefresh(value);
            propertyViewModel.currentValue = value;
        }
    }

    private setupReferencePropertyValue(propertyViewModel: Nakedobjectsviewmodels.IPropertyViewModel) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.refresh = (newValue: Models.Value) => this.callIfChanged(propertyViewModel, newValue, (value: Models.Value) => {
            this.setupChoice(propertyViewModel, value);
            this.setupReference(propertyViewModel, value, propertyRep);
        });
    }

    propertyViewModel = (propertyRep: Models.PropertyMember, id: string, previousValue: Models.Value, paneId: number, parentValues: () => _.Dictionary<Models.Value>) => {
        const propertyViewModel = new Nakedobjectsviewmodels.PropertyViewModel(propertyRep, this.color, this.error);

        propertyViewModel.id = id;
        propertyViewModel.onPaneId = paneId;
        propertyViewModel.argId = `${id.toLowerCase()}`;
        propertyViewModel.paneArgId = `${propertyViewModel.argId}${paneId}`;


        if (propertyRep.attachmentLink() != null) {
            propertyViewModel.attachment = this.attachmentViewModel(propertyRep, paneId);
        }

        const fieldEntryType = propertyViewModel.entryType;

        if (fieldEntryType === Models.EntryType.AutoComplete) {
            this.setupPropertyAutocomplete(propertyViewModel, parentValues);
        }

        if (fieldEntryType === Models.EntryType.ConditionalChoices) {
            this.setupPropertyConditionalChoices(propertyViewModel);
        }

        if (propertyRep.isScalar()) {
            this.setupScalarPropertyValue(propertyViewModel);
        } else {
            // is reference
            this.setupReferencePropertyValue(propertyViewModel);
        }

        propertyViewModel.refresh(previousValue);

        if (!previousValue) {
            propertyViewModel.originalValue = propertyViewModel.getValue();
        }

        const required = propertyViewModel.optional ? "" : "* ";
        propertyViewModel.description = required + propertyViewModel.description;

        propertyViewModel.isDirty = () => !!previousValue || propertyViewModel.getValue().toValueString() !== propertyViewModel.originalValue.toValueString();
        propertyViewModel.validate = <any> _.partial(Models.validate, propertyRep, propertyViewModel) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        propertyViewModel.canDropOn = (targetType: string) => this.context.isSubTypeOf(propertyViewModel.returnType, targetType) as Promise<boolean>;
        propertyViewModel.drop = _.partial(this.drop, propertyViewModel);
        propertyViewModel.doClick = (right?: boolean) => this.urlManager.setProperty(propertyRep, this.clickHandler.pane(paneId, right));

        return propertyViewModel as Nakedobjectsviewmodels.IPropertyViewModel;
    };

    private setupParameterChoices(parmViewModel: Nakedobjectsviewmodels.IParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.choices = _.map(parmRep.choices(), (v, n) => Nakedobjectsviewmodels.ChoiceViewModel.create(v, parmRep.id(), n));
    }

    private setupParameterAutocomplete(parmViewModel: Nakedobjectsviewmodels.IParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.prompt = (searchTerm: string) => {
            const createcvm = _.partial(this.createChoiceViewModels, parmViewModel.id, searchTerm);
            return this.context.autoComplete(parmRep, parmViewModel.id, () => <_.Dictionary<Models.Value>>{}, searchTerm).
                then(createcvm);
        };
        parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
        parmViewModel.description = parmViewModel.description || Usermessagesconfig.autoCompletePrompt;
    }

    private setupParameterFreeformReference(parmViewModel: Nakedobjectsviewmodels.IParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.description = parmViewModel.description || Usermessagesconfig.dropPrompt;

        const val = previousValue && !previousValue.isNull() ? previousValue : parmRep.default();

        if (!val.isNull() && val.isReference()) {
            parmViewModel.reference = val.link().href();
            parmViewModel.selectedChoice = Nakedobjectsviewmodels.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
        }
    }

    private setupParameterConditionalChoices(parmViewModel: Nakedobjectsviewmodels.IParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(this.createChoiceViewModels, parmViewModel.id, null);
            return this.context.conditionalChoices(parmRep, parmViewModel.id, () => <_.Dictionary<Models.Value>>{}, args).
                then(createcvm);
        };
        parmViewModel.promptArguments = _.fromPairs(_.map(parmRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private setupParameterSelectedChoices(parmViewModel: Nakedobjectsviewmodels.IParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        const fieldEntryType = parmViewModel.entryType;
        function setCurrentChoices(vals: Models.Value) {

            const choicesToSet = _.map(vals.list(), val => Nakedobjectsviewmodels.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

            if (fieldEntryType === Models.EntryType.MultipleChoices) {
                parmViewModel.selectedMultiChoices = _.filter(parmViewModel.choices, c => _.some(choicesToSet, choiceToSet => c.valuesEqual(choiceToSet)));
            } else {
                parmViewModel.selectedMultiChoices = choicesToSet;
            }
        }

        function setCurrentChoice(val: Models.Value) {
            const choiceToSet = Nakedobjectsviewmodels.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

            if (fieldEntryType === Models.EntryType.Choices) {
                parmViewModel.selectedChoice = _.find(parmViewModel.choices, c => c.valuesEqual(choiceToSet));
            } else {
                if (!parmViewModel.selectedChoice || parmViewModel.selectedChoice.getValue().toValueString() !== choiceToSet.getValue().toValueString()) {
                    parmViewModel.selectedChoice = choiceToSet;
                }
            }
        }

        parmViewModel.refresh = (newValue: Models.Value) => {

            if (newValue || parmViewModel.dflt) {
                const toSet = newValue || parmRep.default();
                if (fieldEntryType === Models.EntryType.MultipleChoices || fieldEntryType === Models.EntryType.MultipleConditionalChoices ||
                    parmViewModel.isCollectionContributed) {
                    setCurrentChoices(toSet);
                } else {
                    setCurrentChoice(toSet);
                }
            }
        }

        parmViewModel.refresh(previousValue);

    }

    private setupParameterSelectedValue(parmViewModel: Nakedobjectsviewmodels.IParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        const returnType = parmRep.extensions().returnType();

        parmViewModel.refresh = (newValue: Models.Value) => {

            if (returnType === "boolean") {
                const valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                const bValueToSet = Nakedobjectsviewmodels.toTriStateBoolean(valueToSet);

                parmViewModel.value = bValueToSet;
            } else if (Models.isDateOrDateTime(parmRep)) {
                parmViewModel.value = Models.toUtcDate(newValue || new Models.Value(parmViewModel.dflt));
            } else if (Models.isTime(parmRep)) {
                parmViewModel.value = Models.toTime(newValue || new Models.Value(parmViewModel.dflt));
            } else {
                parmViewModel.value = (newValue ? newValue.toString() : null) || parmViewModel.dflt || "";
            }
        }

        parmViewModel.refresh(previousValue);
    }

    private getRequiredIndicator(parmViewModel: Nakedobjectsviewmodels.IParameterViewModel) {
        return parmViewModel.optional || typeof parmViewModel.value === "boolean" ? "" : "* ";
    }

    parameterViewModel = (parmRep: Models.Parameter, previousValue: Models.Value, paneId: number) => {
        const parmViewModel = new Nakedobjectsviewmodels.ParameterViewModel(parmRep, paneId, this.color, this.error);

        const fieldEntryType = parmViewModel.entryType;

        if (fieldEntryType === Models.EntryType.Choices || fieldEntryType === Models.EntryType.MultipleChoices) {
            this.setupParameterChoices(parmViewModel);
        }

        if (fieldEntryType === Models.EntryType.AutoComplete) {
            this.setupParameterAutocomplete(parmViewModel);
        }

        if (fieldEntryType === Models.EntryType.FreeForm && parmViewModel.type === "ref") {
            this.setupParameterFreeformReference(parmViewModel, previousValue);
        }

        if (fieldEntryType === Models.EntryType.ConditionalChoices || fieldEntryType === Models.EntryType.MultipleConditionalChoices) {
            this.setupParameterConditionalChoices(parmViewModel);
        }

        if (fieldEntryType !== Models.EntryType.FreeForm || parmViewModel.isCollectionContributed) {
            this.setupParameterSelectedChoices(parmViewModel, previousValue);
        } else {
            this.setupParameterSelectedValue(parmViewModel, previousValue);
        }

        const remoteMask = parmRep.extensions().mask();

        if (remoteMask && parmRep.isScalar()) {
            const localFilter = this.mask.toLocalFilter(remoteMask, parmRep.extensions().format());
            parmViewModel.localFilter = localFilter;
            // formatting also happens in in directive - at least for dates - value is now date in that case
            parmViewModel.formattedValue = parmViewModel.value ? localFilter.filter(parmViewModel.value.toString()) : "";
        }

        parmViewModel.description = this.getRequiredIndicator(parmViewModel) + parmViewModel.description;
        parmViewModel.validate = <any>_.partial(Models.validate, parmRep, parmViewModel) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        parmViewModel.drop = _.partial(this.drop, parmViewModel);

        return parmViewModel as Nakedobjectsviewmodels.IParameterViewModel;
    };

    getItems = (links: Models.Link[], tableView: boolean, routeData: Nakedobjectsroutedata.PaneRouteData, listViewModel: Nakedobjectsviewmodels.IListViewModel |
        Nakedobjectsviewmodels.
        ICollectionViewModel) => {
        const selectedItems = routeData.selectedItems;

        const items = _.map(links, (link, i) => this.itemViewModel(link, routeData.paneId, selectedItems[i]));

        if (tableView) {

            const getActionExtensions = routeData.objectId ?
                () => this.context.getActionExtensionsFromObject(routeData.paneId, Models.ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
                () => this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);

            const getExtensions = listViewModel instanceof Nakedobjectsviewmodels.CollectionViewModel ? () => Promise.resolve(listViewModel.collectionRep.extensions()) : getActionExtensions;

            // clear existing header 
            listViewModel.header = null;

            if (items.length > 0) {
                getExtensions().
                    then((ext: Models.Extensions) => {
                        _.forEach(items, itemViewModel => {
                            itemViewModel.tableRowViewModel.hasTitle = ext.tableViewTitle();
                            itemViewModel.tableRowViewModel.title = itemViewModel.title;
                        });

                        if (!listViewModel.header) {
                            const firstItem = items[0].tableRowViewModel;
                            const propertiesHeader = _.map(firstItem.properties, property => property.title);

                            listViewModel.header = firstItem.hasTitle ? [""].concat(propertiesHeader) : propertiesHeader;

                            this.focusManager.focusOverrideOff();
                            this.focusManager.focusOn(Focusmanagerservice.FocusTarget.TableItem, 0, routeData.paneId);
                        }
                    }).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            }
        }

        return items;
    };

    private getCollectionDetails(count: number) {
        if (count == null) {
            return Usermessagesconfig.unknownCollectionSize;
        }

        if (count === 0) {
            return Usermessagesconfig.emptyCollectionSize;
        }

        const postfix = count === 1 ? "Item" : "Items";

        return `${count} ${postfix}`;
    }

    private getDefaultTableState(exts: Models.Extensions) {
        if (exts.renderEagerly()) {
            return exts.tableViewColumns() || exts.tableViewTitle() ? Nakedobjectsroutedata.CollectionViewState.Table : Nakedobjectsroutedata.CollectionViewState.List;
        }
        return Nakedobjectsroutedata.CollectionViewState.Summary;
    }

    collectionViewModel = (collectionRep: Models.CollectionMember, routeData: Nakedobjectsroutedata.PaneRouteData) => {
        const collectionViewModel = new Nakedobjectsviewmodels.CollectionViewModel();

        const itemLinks = collectionRep.value();
        const paneId = routeData.paneId;
        const size = collectionRep.size();

        collectionViewModel.collectionRep = collectionRep;
        collectionViewModel.onPaneId = paneId;
        collectionViewModel.title = collectionRep.extensions().friendlyName();
        collectionViewModel.presentationHint = collectionRep.extensions().presentationHint();
        collectionViewModel.pluralName = collectionRep.extensions().pluralName();

        this.color.toColorNumberFromType(collectionRep.extensions().elementType()).
            then(c => collectionViewModel.color = `${Nakedobjectsconfig.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        collectionViewModel.refresh = (routeData: Nakedobjectsroutedata.PaneRouteData, resetting: boolean) => {

            let state = size === 0 ? Nakedobjectsroutedata.CollectionViewState.Summary : routeData.collections[collectionRep.collectionId()];

            if (state == null) {
                state = this.getDefaultTableState(collectionRep.extensions());
            }

            if (resetting || state !== collectionViewModel.currentState) {

                if (size > 0 || size == null) {
                    collectionViewModel.mayHaveItems = true;
                }
                collectionViewModel.details = this.getCollectionDetails(size);
                const getDetails = itemLinks == null || state === Nakedobjectsroutedata.CollectionViewState.Table;

                if (state === Nakedobjectsroutedata.CollectionViewState.Summary) {
                    collectionViewModel.items = [];
                } else if (getDetails) {
                    this.context.getCollectionDetails(collectionRep, state, resetting).
                        then(details => {
                            collectionViewModel.items = this.getItems(details.value(),
                                state === Nakedobjectsroutedata.CollectionViewState.Table,
                                routeData,
                                collectionViewModel);
                            collectionViewModel.details = this.getCollectionDetails(collectionViewModel.items.length);
                        }).
                        catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
                } else {
                    collectionViewModel.items = this.getItems(itemLinks, state === Nakedobjectsroutedata.CollectionViewState.Table, routeData, collectionViewModel);
                }

                switch (state) {
                    case Nakedobjectsroutedata.CollectionViewState.List:
                        collectionViewModel.template = Nakedobjectsconstants.collectionListTemplate;
                        break;
                    case Nakedobjectsroutedata.CollectionViewState.Table:
                        collectionViewModel.template = Nakedobjectsconstants.collectionTableTemplate;
                        break;
                    default:
                        collectionViewModel.template = Nakedobjectsconstants.collectionSummaryTemplate;
                }
                collectionViewModel.currentState = state;
            }
        }

        collectionViewModel.refresh(routeData, true);

        collectionViewModel.doSummary = () => this.urlManager.setCollectionMemberState(collectionRep.collectionId(), Nakedobjectsroutedata.CollectionViewState.Summary, paneId);
        collectionViewModel.doList = () => this.urlManager.setCollectionMemberState(collectionRep.collectionId(), Nakedobjectsroutedata.CollectionViewState.List, paneId);
        collectionViewModel.doTable = () => this.urlManager.setCollectionMemberState(collectionRep.collectionId(), Nakedobjectsroutedata.CollectionViewState.Table, paneId);

        return collectionViewModel;
    };


    listPlaceholderViewModel = (routeData: Nakedobjectsroutedata.PaneRouteData) => {
        const collectionPlaceholderViewModel = new Nakedobjectsviewmodels.CollectionPlaceholderViewModel();

        collectionPlaceholderViewModel.description = () => `Page ${routeData.page}`;

        const recreate = () =>
            routeData.objectId ?
                this.context.getListFromObject(routeData.paneId, routeData, routeData.page, routeData.pageSize) :
                this.context.getListFromMenu(routeData.paneId, routeData, routeData.page, routeData.pageSize);


        collectionPlaceholderViewModel.reload = () =>
            recreate().
                then(() => {
                    //$route.reload()
                }).
                catch((reject: Models.ErrorWrapper) => {
                    this.error.handleError(reject);
                });

        return collectionPlaceholderViewModel as Nakedobjectsviewmodels.ICollectionPlaceholderViewModel;
    };

    menuViewModel = (menuRep: Models.MenuRepresentation, routeData: Nakedobjectsroutedata.PaneRouteData) => {
        const menuViewModel = new Nakedobjectsviewmodels.MenuViewModel();

        menuViewModel.id = menuRep.menuId();
        menuViewModel.menuRep = menuRep;

        const actions = menuRep.actionMembers();
        menuViewModel.title = menuRep.title();
        menuViewModel.actions = _.map(actions, action => this.actionViewModel(action, menuViewModel, routeData));

        menuViewModel.menuItems = Nakedobjectsviewmodels.createMenuItems(menuViewModel.actions);


        return menuViewModel;
    };

    private selfLinkWithTitle(o: Models.DomainObjectRepresentation) {
        const link = o.selfLink();
        link.setTitle(o.title());
        return link;
    }

    recentItemsViewModel = (paneId: number) => {
        const recentItemsViewModel = new Nakedobjectsviewmodels.RecentItemsViewModel();
        recentItemsViewModel.onPaneId = paneId;
        const items = _.map(this.context.getRecentlyViewed(), o => ({ obj: o, link: this.selfLinkWithTitle(o) }));
        recentItemsViewModel.items = _.map(items, i => this.recentItemViewModel(i.obj, i.link, paneId, false));
        return recentItemsViewModel;
    };

    tableRowViewModel = (properties: _.Dictionary<Models.PropertyMember>, paneId: number): Nakedobjectsviewmodels.ITableRowViewModel => {
        const tableRowViewModel = new Nakedobjectsviewmodels.TableRowViewModel();
        tableRowViewModel.properties = _.map(properties, (property, id) => this.propertyTableViewModel(property, id, paneId));
        return tableRowViewModel;
    };


    private cachedToolBarViewModel: Nakedobjectsviewmodels.IToolBarViewModel;

    private getToolBarViewModel() {
        if (!this.cachedToolBarViewModel) {
            const tvm = new Nakedobjectsviewmodels.ToolBarViewModel();
            tvm.goHome = (right?: boolean) => {
                this.focusManager.focusOverrideOff();
                this.context.updateValues();
                this.urlManager.setHome(this.clickHandler.pane(1, right));
            };
            tvm.goBack = () => {
                this.focusManager.focusOverrideOff();
                this.context.updateValues();
                //navigation.back();
            };
            tvm.goForward = () => {
                this.focusManager.focusOverrideOff();
                this.context.updateValues();
                //navigation.forward();
            };
            tvm.swapPanes = () => {
               // $rootScope.$broadcast(Nakedobjectsconstants.geminiPaneSwapEvent);
                this.context.updateValues();
                this.context.swapCurrentObjects();
                this.urlManager.swapPanes();
            };
            tvm.singlePane = (right?: boolean) => {
                this.context.updateValues();
                this.urlManager.singlePane(this.clickHandler.pane(1, right));
                this.focusManager.refresh(1);
            };
            tvm.cicero = () => {
                this.context.updateValues();
                this.urlManager.singlePane(this.clickHandler.pane(1));
                this.urlManager.cicero();
            };

            tvm.recent = (right?: boolean) => {
                this.context.updateValues();
                this.focusManager.focusOverrideOff();
                this.urlManager.setRecent(this.clickHandler.pane(1, right));
            };

            tvm.logOff = () => {
                this.context.getUser().
                    then(u => {
                        if (window.confirm(Usermessagesconfig.logOffMessage(u.userName() || "Unknown"))) {
                            const config = {
                                withCredentials: true,
                                url: Nakedobjectsconfig.logoffUrl,
                                method: "POST",
                                cache: false
                            };

                            // logoff server
                            //$http(config);

                            // logoff client without waiting for server
                            //$rootScope.$broadcast(Nakedobjectsconstants.geminiLogoffEvent);
                            //$timeout(() => window.location.href = Nakedobjectsconfig.postLogoffUrl);
                        }
                    }).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            };

            tvm.applicationProperties = () => {
                this.context.updateValues();
                this.urlManager.applicationProperties();
            };


            tvm.template = Nakedobjectsconstants.appBarTemplate;
            tvm.footerTemplate = Nakedobjectsconstants.footerTemplate;

            //$rootScope.$on(Nakedobjectsconstants.geminiAjaxChangeEvent, (event, count) =>
            //    tvm.loading = count > 0 ? Usermessagesconfig.loadingMessage : "");

            //$rootScope.$on(Nakedobjectsconstants.geminiWarningEvent, (event, warnings) =>
            //    tvm.warnings = warnings);

            //$rootScope.$on(Nakedobjectsconstants.geminiMessageEvent, (event, messages) =>
            //    tvm.messages = messages);

            this.context.getUser().
                then(user => tvm.userName = user.userName()).
                catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

            this.cachedToolBarViewModel = tvm;
        }
        return this.cachedToolBarViewModel;
    }

    toolBarViewModel = () => this.getToolBarViewModel();

    private cvm: Nakedobjectsviewmodels.ICiceroViewModel = null;

    //ciceroViewModel = () => {
    //    if (cvm == null) {
    //        cvm = new Nakedobjectsviewmodels.CiceroViewModel();
    //        commandFactory.initialiseCommands(cvm);
    //        cvm.parseInput = (input: string) => {
    //            commandFactory.parseInput(input, cvm);
    //        };
    //        cvm.executeNextChainedCommandIfAny = () => {
    //            if (cvm.chainedCommands && cvm.chainedCommands.length > 0) {
    //                const next = cvm.popNextCommand();
    //                commandFactory.processSingleCommand(next, cvm, true);
    //            }
    //        };
    //        cvm.autoComplete = (input: string) => {
    //            commandFactory.autoComplete(input, cvm);
    //        };
    //        cvm.renderHome = _.partial(ciceroRenderer.renderHome, cvm) as (routeData: Nakedobjectsroutedata.PaneRouteData) => void;
    //        cvm.renderObject = _.partial(ciceroRenderer.renderObject, cvm) as (routeData: Nakedobjectsroutedata.PaneRouteData) => void;
    //        cvm.renderList = _.partial(ciceroRenderer.renderList, cvm) as (routeData: Nakedobjectsroutedata.PaneRouteData) => void;
    //        cvm.renderError = _.partial(ciceroRenderer.renderError, cvm);
    //    }
    //    return cvm;
    //};

    //private logoff() {
    //    cvm = null;
    //}

    //$rootScope.$on(geminiLogoffEvent, () => logoff());
}