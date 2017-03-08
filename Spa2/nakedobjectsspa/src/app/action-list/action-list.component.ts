import { Component, Input, ViewChildren, QueryList, ElementRef, AfterViewInit, OnInit } from '@angular/core';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { ActionViewModel } from '../view-models/action-view-model'; // needed for declarations compile 
import * as Models from '../models';
import { IActionHolder, wrapAction } from '../action/action.component';
import { ActionComponent } from '../action/action.component';
import { IMenuHolderViewModel} from '../view-models/imenu-holder-view-model';

@Component({
    selector: 'nof-action-list',
    template: require('./action-list.component.html'),
    styles: [require('./action-list.component.css')]
})
export class ActionListComponent implements AfterViewInit {

    private holder: IMenuHolderViewModel;

    @Input()
    set menuHolder(mh: IMenuHolderViewModel) {
        this.holder = mh;
        this.actionHolders = []; // clear cache;
    }

    get menuHolder() {
        return this.holder;
    }

    get items() {
        return this.menuHolder.menuItems;
    }

    private actionHolders: IActionHolder[][] = [];

    private getActionHolders(menuItem: MenuItemViewModel) {
        return _.map(menuItem.actions, a => wrapAction(a));
    }

    hasActions = (menuItem: MenuItemViewModel) => {
        const actions = menuItem.actions;
        return actions && actions.length > 0;
    }

    hasItems = (menuItem: MenuItemViewModel) => {
        const items = menuItem.menuItems;
        return items && items.length > 0; 
    }

    menuName = (menuItem: MenuItemViewModel) => menuItem.name;

    menuItems = (menuItem: MenuItemViewModel) => menuItem.menuItems;

    menuActions = (menuItem: MenuItemViewModel, index: number) => {
        if (!this.actionHolders[index]) {
            this.actionHolders[index] = this.getActionHolders(menuItem);
        }
        return this.actionHolders[index];
    };

    toggleCollapsed = (menuItem: MenuItemViewModel) => menuItem.toggleCollapsed();

    navCollapsed = (menuItem: MenuItemViewModel) => menuItem.navCollapsed;

    displayClass = (menuItem: MenuItemViewModel) => ({ collapsed: menuItem.navCollapsed, open: !menuItem.navCollapsed, rootMenu: !menuItem.name });

    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions) {
            // until first element returns true
            _.every(actions.toArray(), i => !i.focus());
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }
}