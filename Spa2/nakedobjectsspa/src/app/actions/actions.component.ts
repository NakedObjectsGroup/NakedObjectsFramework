import { Component, Input, ViewChildren, QueryList, ElementRef, AfterViewInit } from '@angular/core';
import { ActionComponent } from '../action/action.component';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { ActionViewModel } from '../view-models/action-view-model'; // needed for declarations compile 

@Component({
    selector: 'nof-actions',
    template: require('./actions.component.html'),
    styles: [require('./actions.component.css')]
})
export class ActionsComponent implements AfterViewInit {

    @Input()
    menu: { menuItems: MenuItemViewModel[] };

    get items() {
        return this.menu.menuItems;
    }

    menuName = (menuItem: MenuItemViewModel) => menuItem.name;

    menuItems = (menuItem: MenuItemViewModel) => menuItem.menuItems;

    menuActions = (menuItem: MenuItemViewModel) => menuItem.actions;

    toggleCollapsed = (menuItem: MenuItemViewModel) => menuItem.toggleCollapsed();

    navCollapsed = (menuItem: MenuItemViewModel) => menuItem.navCollapsed;

    displayClass = (menuItem: MenuItemViewModel) => ({ collapsed: menuItem.navCollapsed, open: !menuItem.navCollapsed, rootMenu: !menuItem.name });

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