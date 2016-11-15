import * as Actionviewmodel from './action-view-model';

export class MenuItemViewModel {
    constructor(public name: string,
        public actions: Actionviewmodel.ActionViewModel[],
        public menuItems: MenuItemViewModel[]) {
    }

    toggleCollapsed() {
        this.navCollapsed = !this.navCollapsed;
    }

    navCollapsed = !!this.name;
}