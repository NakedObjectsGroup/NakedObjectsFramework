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
import each from 'lodash-es/each';
import filter from 'lodash-es/filter';
import find from 'lodash-es/find';
import map from 'lodash-es/map';
import reduce from 'lodash-es/reduce';
import uniqWith from 'lodash-es/uniqWith';
import { ILocalFilter } from '../mask.service';
import { ConfigService } from '../config.service';
import * as Validate from '../validate';

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

function removeDuplicateMenuNames(menus: { name: string, action: ActionViewModel }[]) {
    return uniqWith(menus,
        (m1, m2) => {
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

        // then collate submenus
        // don't know why need cast here - problem in lodash types ?
        const submenuActions = filter(avms, (a: ActionViewModel) => getMenuNameForLevel(a.menuPath, level) === menuSlot.name && getMenuNameForLevel(a.menuPath, level + 1)) as any;
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
}

function validateAgainstType(model: Models.IHasExtensions, modelValue: string | ChoiceViewModel | string[] | ChoiceViewModel[], viewValue: string, localFilter: ILocalFilter): string {
    // first check

    const mandatory = Validate.validateMandatory(model, viewValue);

    if (mandatory) {
        return mandatory;
    }

    // if optional but empty always valid
    if (modelValue == null || modelValue === "") {
        return "";
    }

    return Validate.validateMandatoryAgainstType(model, viewValue, localFilter);
}

export function validate(rep: Models.IHasExtensions, vm: FieldViewModel, modelValue: string | ChoiceViewModel | string[] | ChoiceViewModel[], viewValue: string, mandatoryOnly: boolean) {
    const message = mandatoryOnly ? Validate.validateMandatory(rep, viewValue) : validateAgainstType(rep, modelValue, viewValue, vm.localFilter);

    if (message !== Msg.mandatory) {
        vm.setMessage(message);
    } else {
        vm.resetMessage();
    }

    vm.clientValid = !message;
    return vm.clientValid;
}

export function setScalarValueInView(vm: { value: string | number | boolean | Date | null }, propertyRep: Models.PropertyMember, value: Models.Value) {
    if (Models.isDate(propertyRep)) {
        const date = Models.toUtcDate(value);
        vm.value = date ? Models.toDateString(date) : "";
    } else if (Models.isDateTime(propertyRep)) {
        const date = Models.toUtcDate(value);
        vm.value = date ? Models.toDateTimeString(date) : "";
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
    if (requiredFieldsMissing) { msg = `${msg} Please complete REQUIRED fields. `; }
    if (fieldValidationErrors) { msg = `${msg} See field validation message(s). `; }

    if (!msg) { msg = err.warningMessage; }
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
