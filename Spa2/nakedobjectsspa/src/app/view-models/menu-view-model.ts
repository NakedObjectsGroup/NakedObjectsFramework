

import * as Messageviewmodel from './message-view-model';
import * as Actionviewmodel from './action-view-model';
import * as Menuitemviewmodel from './menu-item-view-model';
import * as Models from '../models';

export class MenuViewModel extends Messageviewmodel.MessageViewModel {
    id: string;
    title: string;
    actions: Actionviewmodel.ActionViewModel[];
    menuItems: Menuitemviewmodel.MenuItemViewModel[];
    menuRep: Models.MenuRepresentation;
}