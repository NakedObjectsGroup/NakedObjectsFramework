import { MessageViewModel } from './message-view-model';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import * as Models from '../models';

export class MenuViewModel extends MessageViewModel {
    id: string;
    title: string;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    menuRep: Models.MenuRepresentation;
}