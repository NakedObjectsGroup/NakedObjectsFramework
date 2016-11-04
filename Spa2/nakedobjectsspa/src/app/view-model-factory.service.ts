import * as Models from "./models";
import * as ViewModels from "./view-models";
import * as Config from "./config";
import { FocusManagerService, FocusTarget } from "./focus-manager.service";
import { PaneRouteData, CollectionViewState, InteractionMode } from "./route-data";
import * as Constants from "./constants";
import * as Msg from "./user-messages";
import { ContextService } from "./context.service";
import { UrlManagerService } from "./url-manager.service";
import { ColorService } from "./color.service";
import { ClickHandlerService } from "./click-handler.service";
import { ErrorService } from "./error.service";
import { MaskService } from "./mask.service";
import { Injectable } from '@angular/core';
import * as _ from "lodash";
import {MomentWrapperService} from "./moment-wrapper.service";

@Injectable()
export class ViewModelFactoryService {

    constructor(private context: ContextService,
        private urlManager: UrlManagerService,
        private color: ColorService,
        private error: ErrorService,
        private clickHandler: ClickHandlerService,
        private focusManager: FocusManagerService,
        private mask : MaskService,
        private momentWrapperService : MomentWrapperService
    ) {

     }


    errorViewModel = (error: Models.ErrorWrapper) => {
        const errorViewModel = new ViewModels.ErrorViewModel();

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

    private initLinkViewModel(linkViewModel: ViewModels.LinkViewModel, linkRep: Models.Link) {
        linkViewModel.title = linkRep.title() + Models.dirtyMarker(this.context, linkRep.getOid());
        linkViewModel.link = linkRep;
        linkViewModel.domainType = linkRep.type().domainType;

        // for dropping 
        const value = new Models.Value(linkRep);

        linkViewModel.value = value.toString();
        linkViewModel.reference = value.toValueString();
        linkViewModel.selectedChoice = ViewModels.ChoiceViewModel.create(value, "");
        linkViewModel.draggableType = linkViewModel.domainType;

        this.color.toColorNumberFromHref(linkRep.href()).
            then(c => linkViewModel.color = `${Config.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        linkViewModel.canDropOn = (targetType: string) => this.context.isSubTypeOf(linkViewModel.domainType, targetType);
    }

    private createChoiceViewModels = (id: string, searchTerm: string, choices: _.Dictionary<Models.Value>) =>
        Promise.resolve(_.map(choices, (v, k) => ViewModels.ChoiceViewModel.create(v, id, k, searchTerm)));

    attachmentViewModel = (propertyRep: Models.PropertyMember, paneId: number) => {
        const parent = propertyRep.parent as Models.DomainObjectRepresentation;
        const avm = ViewModels.AttachmentViewModel.create(propertyRep.attachmentLink(), parent, this.context, paneId);
        avm.doClick = (right?: boolean) => this.urlManager.setAttachment(avm.link, this.clickHandler.pane(paneId, right));

        return avm;
    };

    linkViewModel = (linkRep: Models.Link, paneId: number) => {
        const linkViewModel = new ViewModels.LinkViewModel();
        this.initLinkViewModel(linkViewModel, linkRep);

        linkViewModel.doClick = () => {
            // because may be clicking on menu already open so want to reset focus             
            this.urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
            this.focusManager.setCurrentPane(paneId);
            this.focusManager.focusOverrideOff();
            this.focusManager.focusOn(FocusTarget.SubAction, 0, paneId);
        };

        return linkViewModel as ViewModels.ILinkViewModel;
    };

    itemViewModel = (linkRep: Models.Link, paneId: number, selected: boolean, index : number) => {
        const itemViewModel = new ViewModels.ItemViewModel();
        this.initLinkViewModel(itemViewModel, linkRep);

        itemViewModel.selectionChange = () => {
            this.context.updateValues();
            this.urlManager.setListItem(index, itemViewModel.selected, paneId);
            this.focusManager.focusOverrideOn(FocusTarget.CheckBox, index + 1, paneId);
        };

        // avoid setter to avoid selectionChanged
        itemViewModel._selected = selected;

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

    recentItemViewModel = (obj: Models.DomainObjectRepresentation, linkRep: Models.Link, paneId: number, selected: boolean, index : number) => {
        const recentItemViewModel = this.itemViewModel(linkRep, paneId, selected, index) as ViewModels.ILinkViewModel;
        (recentItemViewModel as ViewModels.IRecentItemViewModel).friendlyName = obj.extensions().friendlyName();
        return recentItemViewModel as ViewModels.IRecentItemViewModel;
    };

    actionViewModel = (actionRep: Models.ActionMember | Models.ActionRepresentation, vm: ViewModels.IMessageViewModel, routeData: PaneRouteData) => {
        const actionViewModel = new ViewModels.ActionViewModel();

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

        actionViewModel.execute = (pps: ViewModels.ParameterViewModel[], right?: boolean) => {
            const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
            _.forEach(pps, p => this.urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId));
            return this.context.getInvokableAction(actionViewModel.actionRep).then(details => this.context.invokeAction(details, parmMap, paneId, this.clickHandler.pane(paneId, right)));
        };

        // form actions should never show dialogs
        const showDialog = () => actionRep.extensions().hasParams() && (routeData.interactionMode !== InteractionMode.Form);

        // open dialog on current pane always - invoke action goes to pane indicated by click
        actionViewModel.doInvoke = showDialog() ?
            (right?: boolean) => {
                this.focusManager.setCurrentPane(paneId);
                this.focusManager.focusOverrideOff();
                // clear any previous dialog so we don't pick up values from it
                this.context.clearDialogValues(paneId);
                this.urlManager.setDialog(actionRep.actionId(), paneId);
                this.focusManager.focusOn(FocusTarget.Dialog, 0, paneId); // in case dialog is already open
            } :
            (right?: boolean) => {
                this.focusManager.focusOverrideOff();
                const pps = actionViewModel.parameters();
                actionViewModel.execute(pps, right).
                    then((actionResult: Models.ActionResultRepresentation) => {
                        // if expect result and no warning from server generate one here
                        if (actionResult.shouldExpectResult() && !actionResult.warningsOrMessages()) {
                            this.context.broadcastWarning(Msg.noResultMessage);
                        }
                    }).
                    catch((reject: Models.ErrorWrapper) => {
                        const display = (em: Models.ErrorMap) => vm.setMessage(em.invalidReason() || em.warningMessage);
                        this.error.handleErrorAndDisplayMessages(reject, display);
                    });
            };

        actionViewModel.makeInvokable = (details: Models.IInvokableAction) => actionViewModel.invokableActionRep = details;

        return actionViewModel as ViewModels.IActionViewModel;
    };


    handleErrorResponse = (err: Models.ErrorMap, messageViewModel: ViewModels.IMessageViewModel, valueViewModels: ViewModels.IFieldViewModel[]) => {

        let requiredFieldsMissing = false; // only show warning message if we have nothing else 
        let fieldValidationErrors = false;
        let contributedParameterErrorMsg = "";

        _.each(err.valuesMap(), (errorValue, k) => {

            const valueViewModel = _.find(valueViewModels, vvm => vvm.id === k);

            if (valueViewModel) {
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
            } else {
                // no matching parm for message - this can happen in contributed actions 
                // make the message a dialog level warning.                               
                contributedParameterErrorMsg = errorValue.invalidReason;
            }
        });

        let msg = contributedParameterErrorMsg || err.invalidReason() || "";
        if (requiredFieldsMissing) msg = `${msg} Please complete REQUIRED fields. `;
        if (fieldValidationErrors) msg = `${msg} See field validation message(s). `;

        if (!msg) msg = err.warningMessage;
        messageViewModel.setMessage(msg);
    };

    private drop(context: ContextService, error: ErrorService, vm: ViewModels.IFieldViewModel, newValue: ViewModels.IDraggableViewModel) {
        context.isSubTypeOf(newValue.draggableType, vm.returnType).
            then((canDrop: boolean) => {
                if (canDrop) {
                    vm.setNewValue(newValue);
                }
            }).
            catch((reject: Models.ErrorWrapper) => error.handleError(reject));
    };

    private validate(rep: Models.IHasExtensions, vm: ViewModels.IFieldViewModel, ms : MomentWrapperService, modelValue: any, viewValue: string, mandatoryOnly: boolean) {
        const message = mandatoryOnly ? Models.validateMandatory(rep, viewValue) : Models.validate(rep, modelValue, viewValue, vm.localFilter, ms);

        if (message !== Msg.mandatory) {
            vm.setMessage(message);
        } else {
            vm.resetMessage();
        }

        vm.clientValid = !message;
        return vm.clientValid;
    };

    private setupReference(vm: ViewModels.IPropertyViewModel, value: Models.Value, rep: Models.IHasExtensions) {
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
            vm.description = vm.description || Msg.dropPrompt;
        }
    }

    private setScalarValueInView(vm: { value: string | number | boolean | Date }, propertyRep: Models.PropertyMember, value: Models.Value) {
        if (Models.isDateOrDateTime(propertyRep)) {
            //vm.value = Models.toUtcDate(value);
            const date = Models.toUtcDate(value);
            vm.value = date ? Models.toDateString(date) : "";
        } else if (Models.isTime(propertyRep)) {
            vm.value = Models.toTime(value);
        } else {
            vm.value = value.scalar();
        }
    }

    private setupChoice(propertyViewModel: ViewModels.IPropertyViewModel, newValue: Models.Value) {
        const propertyRep = propertyViewModel.propertyRep;
        if (propertyViewModel.entryType === Models.EntryType.Choices) {
            
            const choices = propertyRep.choices();

            propertyViewModel.choices = _.map(choices, (v, n) => ViewModels.ChoiceViewModel.create(v, propertyViewModel.id, n));

            if (propertyViewModel.optional) {
                const emptyChoice = ViewModels.ChoiceViewModel.create(new Models.Value(""), propertyViewModel.id);
                propertyViewModel.choices = _.concat([emptyChoice], propertyViewModel.choices);
            }

            const currentChoice = ViewModels.ChoiceViewModel.create(newValue, propertyViewModel.id);
            propertyViewModel.selectedChoice = _.find(propertyViewModel.choices, c => c.valuesEqual(currentChoice));
        } else if (!propertyRep.isScalar()) {
            propertyViewModel.selectedChoice = ViewModels.ChoiceViewModel.create(newValue, propertyViewModel.id);
        }
    }

    private setupScalarPropertyValue(propertyViewModel: ViewModels.IPropertyViewModel) {
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
                propertyViewModel.formattedValue = Msg.obscuredText;
            } else {
                propertyViewModel.formattedValue =  localFilter.filter(propertyViewModel.value);
            }
        });
    }

    propertyTableViewModel = (propertyRep: Models.PropertyMember | Models.CollectionMember, id: string, paneId: number) => {
        const tableRowColumnViewModel = new ViewModels.TableRowColumnViewModel();

        tableRowColumnViewModel.title = propertyRep.extensions().friendlyName();
        tableRowColumnViewModel.id = id;

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
                    const currentChoice = ViewModels.ChoiceViewModel.create(value, id);
                    const choices = _.map(propertyRep.choices(), (v, n) => ViewModels.ChoiceViewModel.create(v, id, n));
                    const choice = _.find(choices, c => c.valuesEqual(currentChoice));

                    if (choice) {
                        tableRowColumnViewModel.value = choice.name;
                        tableRowColumnViewModel.formattedValue = choice.name;
                    }
                } else if (isPassword) {
                    tableRowColumnViewModel.formattedValue = Msg.obscuredText;
                } else {
                    tableRowColumnViewModel.formattedValue =  localFilter.filter(tableRowColumnViewModel.value);
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

    private setupPropertyAutocomplete(propertyViewModel: ViewModels.IPropertyViewModel, parentValues: () => _.Dictionary<Models.Value>) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.prompt = (searchTerm: string) => {
            const createcvm = _.partial(this.createChoiceViewModels, propertyViewModel.id, searchTerm);
            const digest = this.getDigest(propertyRep);

            return this.context.autoComplete(propertyRep, propertyViewModel.id, parentValues, searchTerm, digest).then(createcvm);
        };
        propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
        propertyViewModel.description = propertyViewModel.description || Msg.autoCompletePrompt;
    }

    private setupPropertyConditionalChoices(propertyViewModel: ViewModels.IPropertyViewModel) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(this.createChoiceViewModels, propertyViewModel.id, null);
            const digest = this.getDigest(propertyRep);
            return this.context.conditionalChoices(propertyRep, propertyViewModel.id, () => <_.Dictionary<Models.Value>>{}, args, digest).then(createcvm);
        };
        propertyViewModel.promptArguments = (<any>_.fromPairs)(_.map(propertyRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private callIfChanged(propertyViewModel: ViewModels.IPropertyViewModel, newValue: Models.Value, doRefresh: (newValue: Models.Value) => void) {
        const propertyRep = propertyViewModel.propertyRep;
        const value = newValue || propertyRep.value();

        if (propertyViewModel.currentValue == null || value.toValueString() !== propertyViewModel.currentValue.toValueString()) {
            doRefresh(value);
            propertyViewModel.currentValue = value;
        }
    }

    private setupReferencePropertyValue(propertyViewModel: ViewModels.IPropertyViewModel) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.refresh = (newValue: Models.Value) => this.callIfChanged(propertyViewModel, newValue, (value: Models.Value) => {
            this.setupChoice(propertyViewModel, value);
            this.setupReference(propertyViewModel, value, propertyRep);
        });
    }

    propertyViewModel = (propertyRep: Models.PropertyMember, id: string, previousValue: Models.Value, paneId: number, parentValues: () => _.Dictionary<Models.Value>) => {
        const propertyViewModel = new ViewModels.PropertyViewModel(propertyRep, this.color, this.error);

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
        propertyViewModel.validate =  _.partial(this.validate, propertyRep, propertyViewModel, this.momentWrapperService) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        propertyViewModel.canDropOn = (targetType: string) => this.context.isSubTypeOf(propertyViewModel.returnType, targetType) as Promise<boolean>;
        propertyViewModel.drop = _.partial(this.drop, this.context, this.error, propertyViewModel);
        propertyViewModel.doClick = (right?: boolean) => this.urlManager.setProperty(propertyRep, this.clickHandler.pane(paneId, right));

        return propertyViewModel as ViewModels.IPropertyViewModel;
    };

    private setupParameterChoices(parmViewModel: ViewModels.IParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.choices = _.map(parmRep.choices(), (v, n) => ViewModels.ChoiceViewModel.create(v, parmRep.id(), n));
    }

    private setupParameterAutocomplete(parmViewModel: ViewModels.IParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.prompt = (searchTerm: string) => {
            const createcvm = _.partial(this.createChoiceViewModels, parmViewModel.id, searchTerm);
            return this.context.autoComplete(parmRep, parmViewModel.id, () => <_.Dictionary<Models.Value>>{}, searchTerm).
                then(createcvm);
        };
        parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
        parmViewModel.description = parmViewModel.description || Msg.autoCompletePrompt;
    }

    private setupParameterFreeformReference(parmViewModel: ViewModels.IParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.description = parmViewModel.description || Msg.dropPrompt;

        const val = previousValue && !previousValue.isNull() ? previousValue : parmRep.default();

        if (!val.isNull() && val.isReference()) {
            parmViewModel.reference = val.link().href();
            parmViewModel.selectedChoice = ViewModels.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
        }
    }

    private setupParameterConditionalChoices(parmViewModel: ViewModels.IParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(this.createChoiceViewModels, parmViewModel.id, null);
            return this.context.conditionalChoices(parmRep, parmViewModel.id, () => <_.Dictionary<Models.Value>>{}, args).
                then(createcvm);
        };
        parmViewModel.promptArguments = (<any>_.fromPairs)(_.map(parmRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private setupParameterSelectedChoices(parmViewModel: ViewModels.IParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        const fieldEntryType = parmViewModel.entryType;
        function setCurrentChoices(vals: Models.Value) {

            const choicesToSet = _.map(vals.list(), val => ViewModels.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

            if (fieldEntryType === Models.EntryType.MultipleChoices) {
                parmViewModel.selectedMultiChoices = _.filter(parmViewModel.choices, c => _.some(choicesToSet, choiceToSet => c.valuesEqual(choiceToSet)));
            } else {
                parmViewModel.selectedMultiChoices = choicesToSet;
            }
        }

        function setCurrentChoice(val: Models.Value) {
            const choiceToSet = ViewModels.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

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

    private setupParameterSelectedValue(parmViewModel: ViewModels.IParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        const returnType = parmRep.extensions().returnType();

        parmViewModel.refresh = (newValue: Models.Value) => {

            if (returnType === "boolean") {
                const valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                const bValueToSet = ViewModels.toTriStateBoolean(valueToSet);

                parmViewModel.value = bValueToSet;
            } else if (Models.isDateOrDateTime(parmRep)) {
                //parmViewModel.value = Models.toUtcDate(newValue || new Models.Value(parmViewModel.dflt));
                const date = Models.toUtcDate(newValue || new Models.Value(parmViewModel.dflt));
                parmViewModel.value = date ? Models.toDateString(date) : "";
            } else if (Models.isTime(parmRep)) {
                parmViewModel.value = Models.toTime(newValue || new Models.Value(parmViewModel.dflt));
            } else {
                parmViewModel.value = (newValue ? newValue.toString() : null) || parmViewModel.dflt || "";
            }
        }

        parmViewModel.refresh(previousValue);
    }

    private getRequiredIndicator(parmViewModel: ViewModels.IParameterViewModel) {
        return parmViewModel.optional || typeof parmViewModel.value === "boolean" ? "" : "* ";
    }

    parameterViewModel = (parmRep: Models.Parameter, previousValue: Models.Value, paneId: number) => {
        const parmViewModel = new ViewModels.ParameterViewModel(parmRep, paneId, this.color, this.error);

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
            parmViewModel.formattedValue =  localFilter.filter(parmViewModel.value.toString());
        }

        parmViewModel.description = this.getRequiredIndicator(parmViewModel) + parmViewModel.description;
        parmViewModel.validate = <any>_.partial(this.validate, parmRep, parmViewModel, this.momentWrapperService) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        parmViewModel.drop = _.partial(this.drop, this.context, this.error, parmViewModel);

        return parmViewModel as ViewModels.IParameterViewModel;
    };

    getItems = (links: Models.Link[], tableView: boolean, routeData: PaneRouteData, listViewModel: ViewModels.IListViewModel | ViewModels.ICollectionViewModel) => {
        const selectedItems = routeData.selectedItems;

        const items = _.map(links, (link, i) => this.itemViewModel(link, routeData.paneId, selectedItems[i], i));

        if (tableView) {

            const getActionExtensions = routeData.objectId ?
                () => this.context.getActionExtensionsFromObject(routeData.paneId, Models.ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
                () => this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);

            const getExtensions = listViewModel instanceof ViewModels.CollectionViewModel ? () => Promise.resolve(listViewModel.collectionRep.extensions()) : getActionExtensions;

            // clear existing header 
            listViewModel.header = null;

            if (items.length > 0) {
                getExtensions().
                    then((ext: Models.Extensions) => {
                        _.forEach(items, itemViewModel => {
                            itemViewModel.tableRowViewModel.hasTitle = ext.tableViewTitle();
                            itemViewModel.tableRowViewModel.title = itemViewModel.title;
                            itemViewModel.tableRowViewModel.conformColumns(ext.tableViewColumns());
                        });

                        if (!listViewModel.header) {
                            const firstItem = items[0].tableRowViewModel;

                            const propertiesHeader =
                                _.map(firstItem.properties, (p, i) => {
                                    const match = _.find(items, item => item.tableRowViewModel.properties[i].title);
                                    return match ? match.tableRowViewModel.properties[i].title : firstItem.properties[i].id;
                                });

                            listViewModel.header = firstItem.hasTitle ? [""].concat(propertiesHeader) : propertiesHeader;

                            this.focusManager.focusOverrideOff();
                            this.focusManager.focusOn(FocusTarget.TableItem, 0, routeData.paneId);
                        }
                    }).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            }
        }

        return items;
    };

    private getCollectionDetails(count: number) {
        if (count == null) {
            return Msg.unknownCollectionSize;
        }

        if (count === 0) {
            return Msg.emptyCollectionSize;
        }

        const postfix = count === 1 ? "Item" : "Items";

        return `${count} ${postfix}`;
    }

    private getDefaultTableState(exts: Models.Extensions) {
        if (exts.renderEagerly()) {
            return exts.tableViewColumns() || exts.tableViewTitle() ? CollectionViewState.Table : CollectionViewState.List;
        }
        return CollectionViewState.Summary;
    }

    collectionViewModel = (collectionRep: Models.CollectionMember, routeData: PaneRouteData) => {
        const collectionViewModel = new ViewModels.CollectionViewModel();

        const itemLinks = collectionRep.value();
        const paneId = routeData.paneId;
        const size = collectionRep.size();

        collectionViewModel.collectionRep = collectionRep;
        collectionViewModel.onPaneId = paneId;
        collectionViewModel.title = collectionRep.extensions().friendlyName();
        collectionViewModel.presentationHint = collectionRep.extensions().presentationHint();
        collectionViewModel.pluralName = collectionRep.extensions().pluralName();

        this.color.toColorNumberFromType(collectionRep.extensions().elementType()).
            then(c => collectionViewModel.color = `${Config.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        function setCurrentState(collectionViewModel: ViewModels.CollectionViewModel)
        {
            if (size === 0) {
                collectionViewModel.currentState = CollectionViewState.Summary;
            }
            else if (collectionRep.hasTableData()) {
                collectionViewModel.currentState = CollectionViewState.Table;
            } else {
                collectionViewModel.currentState = CollectionViewState.List;
            }
        }


        collectionViewModel.refresh = (routeData: PaneRouteData, resetting: boolean) => {

            setCurrentState(collectionViewModel);
            

            collectionViewModel.requestedState = routeData.collections[collectionRep.collectionId()] as CollectionViewState;

            if (collectionViewModel.requestedState == null) {
                collectionViewModel.requestedState = this.getDefaultTableState(collectionRep.extensions());
            }

            if (resetting || collectionViewModel.requestedState !== collectionViewModel.currentState) {

                if (size > 0 || size == null) {
                    collectionViewModel.mayHaveItems = true;
                }
                collectionViewModel.details = this.getCollectionDetails(size);
                const getDetails = itemLinks == null || collectionViewModel.requestedState === CollectionViewState.Table && collectionViewModel.currentState !== CollectionViewState.Table;

                if (collectionViewModel.requestedState === CollectionViewState.Summary) {
                    collectionViewModel.items = [];
                    setCurrentState(collectionViewModel);
                } else if (getDetails) {
                    this.context.getCollectionDetails(collectionRep, collectionViewModel.requestedState, resetting).
                        then(details => {
                            collectionViewModel.items = this.getItems(details.value(),
                                collectionViewModel.requestedState === CollectionViewState.Table,
                                routeData,
                                collectionViewModel);
                            collectionViewModel.details = this.getCollectionDetails(collectionViewModel.items.length);
                            setCurrentState(collectionViewModel);
                        }).
                        catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
                } else {
                    collectionViewModel.items = this.getItems(itemLinks, collectionViewModel.currentState === CollectionViewState.Table, routeData, collectionViewModel);
                    setCurrentState(collectionViewModel);
                }
            }
        }

        collectionViewModel.refresh(routeData, true);

        collectionViewModel.doSummary = () => this.urlManager.setCollectionMemberState(collectionRep.collectionId(), CollectionViewState.Summary, paneId);
        collectionViewModel.doList = () => this.urlManager.setCollectionMemberState(collectionRep.collectionId(), CollectionViewState.List, paneId);
        collectionViewModel.doTable = () => this.urlManager.setCollectionMemberState(collectionRep.collectionId(), CollectionViewState.Table, paneId);

        collectionViewModel.hasTableData = () => collectionViewModel.items && _.some(collectionViewModel.items, i => i.tableRowViewModel);

        return collectionViewModel;
    };


    listPlaceholderViewModel = (routeData: PaneRouteData) => {
        const collectionPlaceholderViewModel = new ViewModels.CollectionPlaceholderViewModel();

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

        return collectionPlaceholderViewModel as ViewModels.ICollectionPlaceholderViewModel;
    };

    menuViewModel = (menuRep: Models.MenuRepresentation, routeData: PaneRouteData) => {
        const menuViewModel = new ViewModels.MenuViewModel();

        menuViewModel.id = menuRep.menuId();
        menuViewModel.menuRep = menuRep;

        const actions = menuRep.actionMembers();
        menuViewModel.title = menuRep.title();
        menuViewModel.actions = _.map(actions, action => this.actionViewModel(action, menuViewModel, routeData));

        menuViewModel.menuItems = ViewModels.createMenuItems(menuViewModel.actions);


        return menuViewModel;
    };

    private selfLinkWithTitle(o: Models.DomainObjectRepresentation) {
        const link = o.selfLink();
        link.setTitle(o.title());
        return link;
    }

    recentItemsViewModel = (paneId: number) => {
        const recentItemsViewModel = new ViewModels.RecentItemsViewModel();
        recentItemsViewModel.onPaneId = paneId;
        const items = _.map(this.context.getRecentlyViewed(), (o, i) => ({ obj: o, link: this.selfLinkWithTitle(o), index : i }));
        recentItemsViewModel.items = _.map(items, i => this.recentItemViewModel(i.obj, i.link, paneId, false, i.index));
        return recentItemsViewModel;
    };

    tableRowViewModel = (properties: _.Dictionary<Models.PropertyMember>, paneId: number): ViewModels.ITableRowViewModel => {
        const tableRowViewModel = new ViewModels.TableRowViewModel();
        tableRowViewModel.properties = _.map(properties, (property, id) => this.propertyTableViewModel(property, id, paneId));
        return tableRowViewModel;
    };


    private cachedToolBarViewModel: ViewModels.IToolBarViewModel;

    private getToolBarViewModel() {
        if (!this.cachedToolBarViewModel) {
            const tvm = new ViewModels.ToolBarViewModel();
         
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
                        if (window.confirm(Msg.logOffMessage(u.userName() || "Unknown"))) {
                            const config = {
                                withCredentials: true,
                                url: Config.logoffUrl,
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


            //tvm.template = Constants.appBarTemplate;
            //tvm.footerTemplate = Constants.footerTemplate;

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

    private cvm: ViewModels.ICiceroViewModel = null;

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
    //        cvm.renderHome = _.partial(ciceroRenderer.renderHome, cvm) as (routeData: PaneRouteData) => void;
    //        cvm.renderObject = _.partial(ciceroRenderer.renderObject, cvm) as (routeData: PaneRouteData) => void;
    //        cvm.renderList = _.partial(ciceroRenderer.renderList, cvm) as (routeData: PaneRouteData) => void;
    //        cvm.renderError = _.partial(ciceroRenderer.renderError, cvm);
    //    }
    //    return cvm;
    //};

    //private logoff() {
    //    cvm = null;
    //}

    //$rootScope.$on(geminiLogoffEvent, () => logoff());
}
