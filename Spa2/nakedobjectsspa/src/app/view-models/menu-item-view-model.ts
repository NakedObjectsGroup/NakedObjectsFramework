import { ActionViewModel } from './action-view-model';
import { IMenuHolderViewModel } from './imenu-holder-view-model';

export class MenuItemViewModel implements IMenuHolderViewModel{
    constructor(
        public readonly name: string,
        public actions: ActionViewModel[] | null, // modified todo 
        public menuItems: MenuItemViewModel[] | null // modified todo 
    ) {
    }

    readonly toggleCollapsed : () => void = () => this.navCollapsed = !this.navCollapsed;
    
    navCollapsed = !!this.name;
}