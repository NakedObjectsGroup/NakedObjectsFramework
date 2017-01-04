import { ActionViewModel } from './action-view-model';

export class MenuItemViewModel {
    constructor(
        public readonly name: string,
        public actions: ActionViewModel[], // modified todo 
        public menuItems: MenuItemViewModel[] // modified todo 
    ) {
    }

    readonly toggleCollapsed : () => void = () => this.navCollapsed = !this.navCollapsed;
    
    navCollapsed = !!this.name;
}