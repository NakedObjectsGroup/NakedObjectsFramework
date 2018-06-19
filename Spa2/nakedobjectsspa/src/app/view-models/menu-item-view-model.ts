import { ActionViewModel } from './action-view-model';
import { IMenuHolderViewModel } from './imenu-holder-view-model';

export class MenuItemViewModel implements IMenuHolderViewModel {
    constructor(
        public readonly name: string,
        public readonly actions: ActionViewModel[],
        public readonly menuItems: MenuItemViewModel[] | null
    ) { }

    navCollapsed = !!this.name;
    readonly toggleCollapsed: () => void = () => this.navCollapsed = !this.navCollapsed;
}
