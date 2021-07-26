import { MenuItemViewModel } from './menu-item-view-model';

export interface IMenuHolderViewModel {
    readonly menuItems: MenuItemViewModel[] | null;
}
