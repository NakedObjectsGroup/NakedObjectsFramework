import { FieldViewModel } from './field-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { ActionViewModel } from './action-view-model';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import { IDraggableViewModel } from './idraggable-view-model';
import { ChoiceViewModel } from './choice-view-model';
import { IMessageViewModel } from './imessage-view-model';
import * as Models from '../models';
import * as Msg from '../user-messages';
import { Dictionary } from 'lodash';
import { Pane } from '../route-data';
import each from 'lodash/each';
import filter from 'lodash/filter';
import find from 'lodash/find';
import map from 'lodash/map';
import reduce from 'lodash/reduce';
import uniqWith from 'lodash/uniqWith';
import { ILocalFilter } from '../mask.service';
import * as moment from 'moment'; 
import { ConfigService } from '../config.service';
import * as Constants from '../constants';

export function getDate(val: string): Date | null {
    const dt1 = moment(val, Constants.fixedDateFormat, true);
    return dt1.isValid() ? dt1.toDate() : null;
}

export function copy(event: KeyboardEvent, item: IDraggableViewModel, context: ContextService) {
    const cKeyCode = 67;
    if (event && (event.keyCode === cKeyCode && event.ctrlKey)) {
        context.setCopyViewModel(item);
        event.preventDefault();
    }
}

export function tooltip(onWhat: { clientValid: () => boolean }, fields: FieldViewModel[]): string {
    if (onWhat.clientValid()) {
        return "";
    }

    const missingMandatoryFields = filter(fields, p => !p.clientValid && !p.getMessage());

    if (missingMandatoryFields.length > 0) {
        return reduce(missingMandatoryFields, (s, t) => s + t.title + "; ", Msg.mandatoryFieldsPrefix);
    }

    const invalidFields = filter(fields, p => !p.clientValid);

    if (invalidFields.length > 0) {
        return reduce(invalidFields, (s, t) => s + t.title + "; ", Msg.invalidFieldsPrefix);
    }

    return "";
}

function getMenuNameForLevel(menupath: string, level: number) {
    let menu = "";

    if (menupath && menupath.length > 0) {
        const menus = menupath.split("_");

        if (menus.length > level) {
            menu = menus[level];
        }
    }

    return menu || "";
}

function removeDuplicateMenuNames(menus: { name: string }[]) {
    return uniqWith(menus,
        (m1: { name: string }, m2: { name: string }) => {
            if (m1.name && m2.name) {
                return m1.name === m2.name;
            }
            return false;
        });
}

export function createSubmenuItems(avms: ActionViewModel[], menuSlot: { name: string, action: ActionViewModel }, level: number): MenuItemViewModel {
    // if not root menu aggregate all actions with same name
    let menuActions: ActionViewModel[];
    let menuItems: MenuItemViewModel[] | null;

    if (menuSlot.name) {
        const actions = filter(avms, a => getMenuNameForLevel(a.menuPath, level) === menuSlot.name && !getMenuNameForLevel(a.menuPath, level + 1));
        menuActions = actions;

        //then collate submenus 

        const submenuActions = filter(avms, (a: ActionViewModel) => getMenuNameForLevel(a.menuPath, level) === menuSlot.name && getMenuNameForLevel(a.menuPath, level + 1));
        let menuSubSlots =  map(submenuActions, a => ({ name: getMenuNameForLevel(a.menuPath, level + 1), action: a }));
      
        menuSubSlots = removeDuplicateMenuNames(menuSubSlots);

        menuItems = map(menuSubSlots, slot => createSubmenuItems(submenuActions, slot, level + 1));
    } else {
        menuActions = [menuSlot.action];
        menuItems = null;
    }

    return new MenuItemViewModel(menuSlot.name, menuActions, menuItems);
}

export function createMenuItems(avms: ActionViewModel[]) {

    // first create a top level menu for each action 
    // note at top level we leave 'un-menued' actions
    // use an anonymous object locally so we can construct objects with readonly properties  
    let menuSlots = map(avms, a => ({ name: getMenuNameForLevel(a.menuPath, 0), action: a }));

    // remove non unique submenus 
    menuSlots = removeDuplicateMenuNames(menuSlots);

    // update submenus with all actions under same submenu
    return map(menuSlots, slot => createSubmenuItems(avms, slot, 0));
}

export function actionsTooltip(onWhat: { noActions: () => boolean }, actionsOpen: boolean) {
    if (actionsOpen) {
        return Msg.closeActions;
    }
    return onWhat.noActions() ? Msg.noActions : Msg.openActions;
}

export function getCollectionDetails(count: number | null) {
    if (count == null) {
        return Msg.unknownCollectionSize;
    }

    if (count === 0) {
        return Msg.emptyCollectionSize;
    }

    const postfix = count === 1 ? "Item" : "Items";

    return `${count} ${postfix}`;
}

export function drop(context: ContextService, error: ErrorService, vm: FieldViewModel, newValue: IDraggableViewModel) {
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

function isInteger(value: number) {
    return typeof value === "number" && isFinite(value) && Math.floor(value) === value;
}

function validateNumber(model: Models.IHasExtensions, newValue: number, filter: ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
    case ("int"):
        if (!isInteger(newValue)) {
            return "Not an integer";
        }
    }

    const range = model.extensions().range();

    if (range) {
        const min = range.min;
        const max = range.max;

        if (min && newValue < min) {
            return Msg.outOfRange(newValue, min, max, filter);
        }

        if (max && newValue > max) {
            return Msg.outOfRange(newValue, min, max, filter);
        }
    }

    return "";
}

function validateStringFormat(model: Models.IHasExtensions, newValue: string): string {

    const maxLength = model.extensions().maxLength();
    const pattern = model.extensions().pattern();
    const len = newValue ? newValue.length : 0;

    if (maxLength && len > maxLength) {
        return Msg.tooLong;
    }

    if (pattern) {
        const regex = new RegExp(pattern);
        return regex.test(newValue) ? "" : Msg.noPatternMatch;
    }
    return "";
}

function validateDateTimeFormat(model: Models.IHasExtensions, newValue: Date): string {
    return "";
}



function validateDateFormat(model: Models.IHasExtensions, newValue: Date | string, filter: ILocalFilter): string {
    const range = model.extensions().range();
    const newDate = (newValue instanceof Date) ? newValue : getDate(newValue);

    if (range && newDate) {
        const min = range.min ? getDate(range.min as string) : null;
        const max = range.max ? getDate(range.max as string) : null;

        if (min && newDate < min) {
            return Msg.outOfRange(Models.toDateString(newDate), Models.getUtcDate(range.min as string), Models.getUtcDate(range.max as string), filter);
        }

        if (max && newDate > max) {
            return Msg.outOfRange(Models.toDateString(newDate), Models.getUtcDate(range.min as string), Models.getUtcDate(range.max as string), filter);
        }
    }

    return "";
}

function validateTimeFormat(model: Models.IHasExtensions, newValue: Date): string {
    return "";
}

function validateString(model: Models.IHasExtensions, newValue: any, filter: ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
    case ("string"):
        return validateStringFormat(model, newValue as string);
    case ("date-time"):
        return validateDateTimeFormat(model, newValue as Date);
    case ("date"):
        return validateDateFormat(model, newValue as Date | string, filter);
    case ("time"):
        return validateTimeFormat(model, newValue as Date);
    default:
        return "";
    }
}


function validateMandatory(model: Models.IHasExtensions, viewValue: string): string {
    // first check 
    const isMandatory = !model.extensions().optional();

    if (isMandatory && (viewValue === "" || viewValue == null)) {
        return Msg.mandatory;
    }

    return "";
}


function validateAgainstType(model: Models.IHasExtensions, modelValue: string | ChoiceViewModel | string[] | ChoiceViewModel[], viewValue: string, filter: ILocalFilter): string {
    // first check 

    const mandatory = validateMandatory(model, viewValue);

    if (mandatory) {
        return mandatory;
    }

    // if optional but empty always valid 
    if (modelValue == null || modelValue === "") {
        return "";
    }

    // check type 
    const returnType = model.extensions().returnType();

    switch (returnType) {
    case ("number"):
        const valueAsNumber = parseFloat(viewValue);
        if (Number.isFinite(valueAsNumber)) {
            return validateNumber(model, valueAsNumber, filter);
        }
        return Msg.notANumber;
    case ("string"):
        return validateString(model, viewValue, filter);
    case ("boolean"):
        return "";
    default:
        return "";
    }
}

export function validate(rep: Models.IHasExtensions, vm: FieldViewModel, modelValue: string | ChoiceViewModel | string[] | ChoiceViewModel[], viewValue: string, mandatoryOnly: boolean) {
    const message = mandatoryOnly ? validateMandatory(rep, viewValue) : validateAgainstType(rep, modelValue, viewValue, vm.localFilter);

    if (message !== Msg.mandatory) {
        vm.setMessage(message);
    } else {
        vm.resetMessage();
    }

    vm.clientValid = !message;
    return vm.clientValid;
};

export function setScalarValueInView(vm: { value: string | number | boolean | Date | null }, propertyRep: Models.PropertyMember, value: Models.Value) {
    if (Models.isDateOrDateTime(propertyRep)) {
        const date = Models.toUtcDate(value);
        vm.value = date ? Models.toDateString(date) : "";
    } else if (Models.isTime(propertyRep)) {
        const time = Models.toTime(value);
        vm.value = time ? Models.toTimeString(time) : "";
    } else {
        vm.value = value.scalar();
    }
}


export function dirtyMarker(context: ContextService, configService: ConfigService, oid: Models.ObjectIdWrapper) {
    return (configService.config.showDirtyFlag && context.getIsDirty(oid)) ? "*" : "";
}


export function createChoiceViewModels(id: string, searchTerm: string, choices: Dictionary<Models.Value>) {
    return Promise.resolve(map(choices, (v, k) => new ChoiceViewModel(v, id, k, searchTerm)));
}

export function handleErrorResponse(err: Models.ErrorMap, messageViewModel: IMessageViewModel, valueViewModels: FieldViewModel[]) {

    let requiredFieldsMissing = false; // only show warning message if we have nothing else 
    let fieldValidationErrors = false;
    let contributedParameterErrorMsg = "";

    each(err.valuesMap(), (errorValue, k) => {

        const valueViewModel = find(valueViewModels, vvm => vvm.id === k);

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
            contributedParameterErrorMsg = errorValue.invalidReason || "";
        }
    });

    let msg = contributedParameterErrorMsg || err.invalidReason() || "";
    if (requiredFieldsMissing) msg = `${msg} Please complete REQUIRED fields. `;
    if (fieldValidationErrors) msg = `${msg} See field validation message(s). `;

    if (!msg) msg = err.warningMessage;
    messageViewModel.setMessage(msg);
}


export function incrementPendingPotentAction(context: ContextService, invokableaction: Models.ActionRepresentation | Models.InvokableActionMember, paneId: Pane) {
    if (invokableaction.isPotent()) {
        context.incPendingPotentActionOrReload(paneId);
    }
}

export function decrementPendingPotentAction(context: ContextService, invokableaction: Models.ActionRepresentation | Models.InvokableActionMember, paneId: Pane) {
    if (invokableaction.isPotent()) {
        context.decPendingPotentActionOrReload(paneId);
    }
}
