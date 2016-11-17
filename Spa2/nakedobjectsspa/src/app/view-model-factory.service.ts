import * as Models from "./models";
import * as Config from "./config";
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
import { MomentWrapperService } from "./moment-wrapper.service";
import { ChoiceViewModel } from './view-models/choice-view-model';
import { AttachmentViewModel } from './view-models/attachment-view-model';
import { ErrorViewModel } from './view-models/error-view-model';
import { IDraggableViewModel } from './view-models/idraggable-view-model';
import { IMessageViewModel } from './view-models/imessage-view-model';
import { LinkViewModel } from './view-models/link-view-model';
import { ItemViewModel } from './view-models/item-view-model';
import { RecentItemViewModel } from './view-models/recent-item-view-model';

import { TableRowColumnViewModel } from './view-models/table-row-column-view-model';
import { TableRowViewModel } from './view-models/table-row-view-model';
import { CollectionPlaceholderViewModel } from './view-models/collection-placeholder-view-model';
import { RecentItemsViewModel} from './view-models/recent-items-view-model';
import * as Ciceroviewmodel from './view-models/cicero-view-model';
import { FieldViewModel } from './view-models/field-view-model';
import { ParameterViewModel } from './view-models/parameter-view-model';
import { ActionViewModel } from './view-models/action-view-model';
import { PropertyViewModel } from './view-models/property-view-model';
import { CollectionViewModel } from './view-models/collection-view-model';
import { ListViewModel } from './view-models/list-view-model';
import * as Helpersviewmodels from './view-models/helpers-view-models';
import { MenuViewModel } from './view-models/menu-view-model';

@Injectable()
export class ViewModelFactoryService {

    constructor(private context: ContextService,
        private urlManager: UrlManagerService,
        private color: ColorService,
        private error: ErrorService,
        private clickHandler: ClickHandlerService,
        private mask: MaskService,
        private momentWrapperService: MomentWrapperService
    ) {}

    errorViewModel = (error: Models.ErrorWrapper) => {
        return new ErrorViewModel(error);
    };

    // todo move to helpers
    createChoiceViewModels = (id: string, searchTerm: string, choices: _.Dictionary<Models.Value>) =>
        Promise.resolve(_.map(choices, (v, k) => ChoiceViewModel.create(v, id, k, searchTerm)));

    attachmentViewModel = (propertyRep: Models.PropertyMember, paneId: number) => {
        const parent = propertyRep.parent as Models.DomainObjectRepresentation;
        return new AttachmentViewModel(propertyRep.attachmentLink(), parent, this.context, this.urlManager, this.clickHandler, paneId);
    };

    linkViewModel = (linkRep: Models.Link, paneId: number) => {
        return new LinkViewModel(this.context, this.color, this.error, this.urlManager, linkRep, paneId);
    };

    itemViewModel = (linkRep: Models.Link, paneId: number, selected: boolean, index: number) => {
        return new ItemViewModel(this.context, this.color, this.error, this.urlManager, linkRep, paneId, this.clickHandler, this, index, selected);    
    };

    recentItemViewModel = (obj: Models.DomainObjectRepresentation, linkRep: Models.Link, paneId: number, selected: boolean, index: number) => {
        return new RecentItemViewModel(this.context, this.color, this.error, this.urlManager, linkRep, paneId, this.clickHandler, this, index, selected, obj.extensions().friendlyName());    
    };

    actionViewModel = (actionRep: Models.ActionMember | Models.ActionRepresentation, vm: IMessageViewModel, routeData: PaneRouteData) => {
        return new ActionViewModel(this, this.context, this.urlManager, this.error, this.clickHandler, actionRep, vm, routeData);
    };

    handleErrorResponse = (err: Models.ErrorMap, messageViewModel: IMessageViewModel, valueViewModels: FieldViewModel[]) => {

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

    private drop(context: ContextService, error: ErrorService, vm: FieldViewModel, newValue: IDraggableViewModel) {
        return context.isSubTypeOf(newValue.draggableType, vm.returnType).
            then((canDrop: boolean) => {
                if (canDrop) {
                    vm.setNewValue(newValue);
                    return true;
                }
                return false;
            }).
            catch((reject: Models.ErrorWrapper) => error.handleError(reject));
    };

    private validate(rep: Models.IHasExtensions, vm: FieldViewModel, ms: MomentWrapperService, modelValue: any, viewValue: string, mandatoryOnly: boolean) {
        const message = mandatoryOnly ? Models.validateMandatory(rep, viewValue) : Models.validate(rep, modelValue, viewValue, vm.localFilter, ms);

        if (message !== Msg.mandatory) {
            vm.setMessage(message);
        } else {
            vm.resetMessage();
        }

        vm.clientValid = !message;
        return vm.clientValid;
    };

    private setupReference(vm: PropertyViewModel, value: Models.Value, rep: Models.IHasExtensions) {
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

    private setupChoice(propertyViewModel: PropertyViewModel, newValue: Models.Value) {
        const propertyRep = propertyViewModel.propertyRep;
        if (propertyViewModel.entryType === Models.EntryType.Choices) {

            const choices = propertyRep.choices();

            propertyViewModel.choices = _.map(choices, (v, n) => ChoiceViewModel.create(v, propertyViewModel.id, n));

            if (propertyViewModel.optional) {
                const emptyChoice = ChoiceViewModel.create(new Models.Value(""), propertyViewModel.id);
                propertyViewModel.choices = _.concat([emptyChoice], propertyViewModel.choices);
            }

            const currentChoice = ChoiceViewModel.create(newValue, propertyViewModel.id);
            propertyViewModel.selectedChoice = _.find(propertyViewModel.choices, c => c.valuesEqual(currentChoice));
        } else if (!propertyRep.isScalar()) {
            propertyViewModel.selectedChoice = ChoiceViewModel.create(newValue, propertyViewModel.id);
        }
    }

    private setupScalarPropertyValue(propertyViewModel: PropertyViewModel) {
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
                propertyViewModel.formattedValue = localFilter.filter(propertyViewModel.value);
            }
        });
    }

    propertyTableViewModel = (propertyRep: Models.PropertyMember | Models.CollectionMember, id: string, paneId: number) => {
        const tableRowColumnViewModel = new TableRowColumnViewModel();

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
                    const currentChoice = ChoiceViewModel.create(value, id);
                    const choices = _.map(propertyRep.choices(), (v, n) => ChoiceViewModel.create(v, id, n));
                    const choice = _.find(choices, c => c.valuesEqual(currentChoice));

                    if (choice) {
                        tableRowColumnViewModel.value = choice.name;
                        tableRowColumnViewModel.formattedValue = choice.name;
                    }
                } else if (isPassword) {
                    tableRowColumnViewModel.formattedValue = Msg.obscuredText;
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

    private setupPropertyAutocomplete(propertyViewModel: PropertyViewModel, parentValues: () => _.Dictionary<Models.Value>) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.prompt = (searchTerm: string) => {
            const createcvm = _.partial(this.createChoiceViewModels, propertyViewModel.id, searchTerm);
            const digest = this.getDigest(propertyRep);

            return this.context.autoComplete(propertyRep, propertyViewModel.id, parentValues, searchTerm, digest).then(createcvm);
        };
        propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
        propertyViewModel.description = propertyViewModel.description || Msg.autoCompletePrompt;
    }

    private setupPropertyConditionalChoices(propertyViewModel: PropertyViewModel) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(this.createChoiceViewModels, propertyViewModel.id, null);
            const digest = this.getDigest(propertyRep);
            return this.context.conditionalChoices(propertyRep, propertyViewModel.id, () => <_.Dictionary<Models.Value>>{}, args, digest).then(createcvm);
        };
        propertyViewModel.promptArguments = (<any>_.fromPairs)(_.map(propertyRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private callIfChanged(propertyViewModel: PropertyViewModel, newValue: Models.Value, doRefresh: (newValue: Models.Value) => void) {
        const propertyRep = propertyViewModel.propertyRep;
        const value = newValue || propertyRep.value();

        if (propertyViewModel.currentValue == null || value.toValueString() !== propertyViewModel.currentValue.toValueString()) {
            doRefresh(value);
            propertyViewModel.currentValue = value;
        }
    }

    private setupReferencePropertyValue(propertyViewModel: PropertyViewModel) {
        const propertyRep = propertyViewModel.propertyRep;
        propertyViewModel.refresh = (newValue: Models.Value) => this.callIfChanged(propertyViewModel, newValue, (value: Models.Value) => {
            this.setupChoice(propertyViewModel, value);
            this.setupReference(propertyViewModel, value, propertyRep);
        });
    }

    propertyViewModel = (propertyRep: Models.PropertyMember, id: string, previousValue: Models.Value, paneId: number, parentValues: () => _.Dictionary<Models.Value>) => {
        const propertyViewModel = new PropertyViewModel(propertyRep, this.color, this.error);

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
        propertyViewModel.validate = _.partial(this.validate, propertyRep, propertyViewModel, this.momentWrapperService) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        propertyViewModel.canDropOn = (targetType: string) => this.context.isSubTypeOf(propertyViewModel.returnType, targetType) as Promise<boolean>;
        propertyViewModel.drop = _.partial(this.drop, this.context, this.error, propertyViewModel);
        propertyViewModel.doClick = (right?: boolean) => this.urlManager.setProperty(propertyRep, this.clickHandler.pane(paneId, right));

        return propertyViewModel as PropertyViewModel;
    };

    private setupParameterChoices(parmViewModel: ParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.choices = _.map(parmRep.choices(), (v, n) => ChoiceViewModel.create(v, parmRep.id(), n));
    }

    private setupParameterAutocomplete(parmViewModel: ParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.prompt = (searchTerm: string) => {
            const createcvm = _.partial(this.createChoiceViewModels, parmViewModel.id, searchTerm);
            return this.context.autoComplete(parmRep, parmViewModel.id, () => <_.Dictionary<Models.Value>>{}, searchTerm).
                then(createcvm);
        };
        parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
        parmViewModel.description = parmViewModel.description || Msg.autoCompletePrompt;
    }

    private setupParameterFreeformReference(parmViewModel: ParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.description = parmViewModel.description || Msg.dropPrompt;

        const val = previousValue && !previousValue.isNull() ? previousValue : parmRep.default();

        if (!val.isNull() && val.isReference()) {
            parmViewModel.reference = val.link().href();
            parmViewModel.selectedChoice = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
        }
    }

    private setupParameterConditionalChoices(parmViewModel: ParameterViewModel) {
        const parmRep = parmViewModel.parameterRep;
        parmViewModel.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(this.createChoiceViewModels, parmViewModel.id, null);
            return this.context.conditionalChoices(parmRep, parmViewModel.id, () => <_.Dictionary<Models.Value>>{}, args).
                then(createcvm);
        };
        parmViewModel.promptArguments = (<any>_.fromPairs)(_.map(parmRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private setupParameterSelectedChoices(parmViewModel: ParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        const fieldEntryType = parmViewModel.entryType;
        function setCurrentChoices(vals: Models.Value) {

            const choicesToSet = _.map(vals.list(), val => ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

            if (fieldEntryType === Models.EntryType.MultipleChoices) {
                parmViewModel.selectedMultiChoices = _.filter(parmViewModel.choices, c => _.some(choicesToSet, choiceToSet => c.valuesEqual(choiceToSet)));
            } else {
                parmViewModel.selectedMultiChoices = choicesToSet;
            }
        }

        function setCurrentChoice(val: Models.Value) {
            const choiceToSet = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

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



    private toTriStateBoolean(valueToSet: string | boolean | number) {

        // looks stupid but note type checking
        if (valueToSet === true || valueToSet === "true") {
            return true;
        }
        if (valueToSet === false || valueToSet === "false") {
            return false;
        }
        return null;
    }


    private setupParameterSelectedValue(parmViewModel: ParameterViewModel, previousValue: Models.Value) {
        const parmRep = parmViewModel.parameterRep;
        const returnType = parmRep.extensions().returnType();

        parmViewModel.refresh = (newValue: Models.Value) => {

            if (returnType === "boolean") {
                const valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                const bValueToSet = this.toTriStateBoolean(valueToSet);

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

    private getRequiredIndicator(parmViewModel: ParameterViewModel) {
        return parmViewModel.optional || typeof parmViewModel.value === "boolean" ? "" : "* ";
    }

    parameterViewModel = (parmRep: Models.Parameter, previousValue: Models.Value, paneId: number) => {
        return new ParameterViewModel(parmRep, paneId, this.color, this.error, this.momentWrapperService, this.mask, previousValue, this, this.context);

        //const fieldEntryType = parmViewModel.entryType;

        //if (fieldEntryType === Models.EntryType.Choices || fieldEntryType === Models.EntryType.MultipleChoices) {
        //    this.setupParameterChoices(parmViewModel);
        //}

        //if (fieldEntryType === Models.EntryType.AutoComplete) {
        //    this.setupParameterAutocomplete(parmViewModel);
        //}

        //if (fieldEntryType === Models.EntryType.FreeForm && parmViewModel.type === "ref") {
        //    this.setupParameterFreeformReference(parmViewModel, previousValue);
        //}

        //if (fieldEntryType === Models.EntryType.ConditionalChoices || fieldEntryType === Models.EntryType.MultipleConditionalChoices) {
        //    this.setupParameterConditionalChoices(parmViewModel);
        //}

        //if (fieldEntryType !== Models.EntryType.FreeForm || parmViewModel.isCollectionContributed) {
        //    this.setupParameterSelectedChoices(parmViewModel, previousValue);
        //} else {
        //    this.setupParameterSelectedValue(parmViewModel, previousValue);
        //}

        //const remoteMask = parmRep.extensions().mask();

        //if (remoteMask && parmRep.isScalar()) {
        //    const localFilter = this.mask.toLocalFilter(remoteMask, parmRep.extensions().format());
        //    parmViewModel.localFilter = localFilter;
        //    // formatting also happens in in directive - at least for dates - value is now date in that case
        //    parmViewModel.formattedValue = localFilter.filter(parmViewModel.value.toString());
        //}

        //parmViewModel.description = this.getRequiredIndicator(parmViewModel) + parmViewModel.description;
        //parmViewModel.validate = <any>_.partial(this.validate, parmRep, parmViewModel, this.momentWrapperService) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        //parmViewModel.drop = _.partial(this.drop, this.context, this.error, parmViewModel);

        //return parmViewModel;
    };

    getItems = (links: Models.Link[], tableView: boolean, routeData: PaneRouteData, listViewModel: ListViewModel | CollectionViewModel) => {
        const selectedItems = routeData.selectedItems;

        const items = _.map(links, (link, i) => this.itemViewModel(link, routeData.paneId, selectedItems[i], i));

        if (tableView) {

            const getActionExtensions = routeData.objectId ?
                () => this.context.getActionExtensionsFromObject(routeData.paneId, Models.ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
                () => this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);

            const getExtensions = listViewModel instanceof CollectionViewModel ? () => Promise.resolve(listViewModel.collectionRep.extensions()) : getActionExtensions;

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

    collectionViewModel = (collectionRep: Models.CollectionMember, routeData: PaneRouteData) => {
        return new CollectionViewModel(this, this.color, this.error, this.context, this.urlManager, collectionRep, routeData );    
    };

    listPlaceholderViewModel = (routeData: PaneRouteData) => {
        return new CollectionPlaceholderViewModel(this.context, this.error, routeData);    
    };

    menuViewModel = (menuRep: Models.MenuRepresentation, routeData: PaneRouteData) => {
        return new MenuViewModel(this, menuRep, routeData);    
    };

    recentItemsViewModel = (paneId: number) => {
        return new RecentItemsViewModel(this, this.context, paneId);
    };

    tableRowViewModel = (properties: _.Dictionary<Models.PropertyMember>, paneId: number): TableRowViewModel => {
        return new TableRowViewModel(this, properties, paneId);
    };

    private cvm: Ciceroviewmodel.CiceroViewModel = null;

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
