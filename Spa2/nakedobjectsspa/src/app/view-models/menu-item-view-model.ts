import { ActionViewModel } from './action-view-model';
import { IMenuHolderViewModel } from './imenu-holder-view-model';

export class MenuItemViewModel implements IMenuHolderViewModel {
    constructor(
        public readonly name: string,
        public readonly actions: ActionViewModel[],
        public readonly menuItems: MenuItemViewModel[] | null
    ) { }

    readonly toggleCollapsed: () => void = () => this.navCollapsed = !this.navCollapsed;

    navCollapsed = !!this.name;
}