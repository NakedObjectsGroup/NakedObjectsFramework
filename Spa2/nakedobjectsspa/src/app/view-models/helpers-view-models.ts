import * as Fieldviewmodel from './field-view-model';
import * as Msg from '../user-messages';
import * as _ from "lodash";
import * as Menuitemviewmodel from './menu-item-view-model';
import * as Actionviewmodel from './action-view-model';

export function tooltip(onWhat: { clientValid: () => boolean }, fields: Fieldviewmodel.FieldViewModel[]): string {
    if (onWhat.clientValid()) {
        return "";
    }

    const missingMandatoryFields = _.filter(fields, p => !p.clientValid && !p.getMessage());

    if (missingMandatoryFields.length > 0) {
        return _.reduce(missingMandatoryFields, (s, t) => s + t.title + "; ", Msg.mandatoryFieldsPrefix);
    }

    const invalidFields = _.filter(fields, p => !p.clientValid);

    if (invalidFields.length > 0) {
        return _.reduce(invalidFields, (s, t) => s + t.title + "; ", Msg.invalidFieldsPrefix);
    }

    return "";
}

function getMenuForLevel(menupath: string, level: number) {
    let menu = "";

    if (menupath && menupath.length > 0) {
        const menus = menupath.split("_");

        if (menus.length > level) {
            menu = menus[level];
        }
    }

    return menu || "";
}

function removeDuplicateMenus(menus: Menuitemviewmodel.MenuItemViewModel[]) {
    return _.uniqWith(menus, (m1: Menuitemviewmodel.MenuItemViewModel, m2: Menuitemviewmodel.MenuItemViewModel) => {
        if (m1.name && m2.name) {
            return m1.name === m2.name;
        }
        return false;
    });
}

export function createSubmenuItems(avms: Actionviewmodel.ActionViewModel[], menu: Menuitemviewmodel.MenuItemViewModel, level: number) {
    // if not root menu aggregate all actions with same name
    if (menu.name) {
        const actions = _.filter(avms, a => getMenuForLevel(a.menuPath, level) === menu.name && !getMenuForLevel(a.menuPath, level + 1));
        menu.actions = actions;

        //then collate submenus 

        const submenuActions = _.filter(avms, (a: Actionviewmodel.ActionViewModel) => getMenuForLevel(a.menuPath, level) === menu.name && getMenuForLevel(a.menuPath, level + 1));
        let menus = _
            .chain(submenuActions)
            .map(a => new Menuitemviewmodel.MenuItemViewModel(getMenuForLevel(a.menuPath, level + 1), null, null))
            .value();

        menus = removeDuplicateMenus(menus);

        menu.menuItems = _.map(menus, m => createSubmenuItems(submenuActions, m, level + 1));
    }
    return menu;
}

export function createMenuItems(avms: Actionviewmodel.ActionViewModel[]) {

    // first create a top level menu for each action 
    // note at top level we leave 'un-menued' actions
    let menus = _
        .chain(avms)
        .map(a => new Menuitemviewmodel.MenuItemViewModel(getMenuForLevel(a.menuPath, 0), [a], null))
        .value();

    // remove non unique submenus 
    menus = removeDuplicateMenus(menus);

    // update submenus with all actions under same submenu
    return _.map(menus, m => createSubmenuItems(avms, m, 0));
}

export function actionsTooltip(onWhat: { disableActions: () => boolean }, actionsOpen: boolean) {
    if (actionsOpen) {
        return Msg.closeActions;
    }
    return onWhat.disableActions() ? Msg.noActions : Msg.openActions;
}