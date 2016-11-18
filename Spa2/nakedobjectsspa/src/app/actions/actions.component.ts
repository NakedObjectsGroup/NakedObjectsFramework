import { Component, Input, ViewChildren, QueryList, ElementRef, AfterViewInit } from '@angular/core';
import { ActionComponent } from "../action/action.component";
import { MenuItemViewModel} from '../view-models/menu-item-view-model';

@Component({
    selector: 'actions',
    templateUrl: './actions.component.html',
    styleUrls: ['./actions.component.css']
})
export class ActionsComponent implements AfterViewInit {

    @Input()
    menuVm: { menuItems: MenuItemViewModel[] };

    get items() {
        return this.menuVm.menuItems;
    }

    menuName = (menu: MenuItemViewModel) => menu.name;

    menuItems = (menu: MenuItemViewModel) => menu.menuItems;

    menuActions = (menu: MenuItemViewModel) => menu.actions;

    toggleCollapsed = (menu: MenuItemViewModel) => menu.toggleCollapsed();

    navCollapsed = (menu: MenuItemViewModel) => menu.navCollapsed;

    displayClass = (menu: MenuItemViewModel) =>  ({ collapsed: menu.navCollapsed, open: !menu.navCollapsed, rootMenu: !menu.name });  


    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions && actions.first) {
            actions.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }
}