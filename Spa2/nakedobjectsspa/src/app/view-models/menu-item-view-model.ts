import * as Actionviewmodel from './action-view-model';
import * as Viewmodels from '../view-models';

export class MenuItemViewModel {
    constructor(public name: string,
        public actions: Actionviewmodel.ActionViewModel[],
        public menuItems: MenuItemViewModel[]) { }

    toggleCollapsed() {
        this.navCollapsed = !this.navCollapsed;
    }

    navCollapsed = !!this.name;
}