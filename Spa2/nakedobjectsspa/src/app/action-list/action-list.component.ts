import { Component, Input, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { IActionHolder, wrapAction } from '../action/action.component';
import { ActionComponent } from '../action/action.component';
import { IMenuHolderViewModel } from '../view-models/imenu-holder-view-model';
import * as _ from 'lodash';

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

    toggleCollapsed = (menuItem: MenuItemViewModel, index: number) => menuItem.toggleCollapsed();

    navCollapsed = (menuItem: MenuItemViewModel) => menuItem.navCollapsed;

    displayClass = (menuItem: MenuItemViewModel) => ({ collapsed: menuItem.navCollapsed, open: !menuItem.navCollapsed, rootMenu: !menuItem.name });

    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    previousActionChildrenNames: string[] = [];

    focusFromIndex(actions: QueryList<ActionComponent>, index = 0) {

        const toFocus = actions.toArray().slice(index);

        if (toFocus && toFocus.length > 0) {
            // until first element returns true
            _.some(toFocus, i => i.focus());
        }
    }


    focus(actions: QueryList<ActionComponent>) {
        if (actions && actions.length > 0) {
            const actionChildrenNames = _.map(actions.toArray(), a => a.action.value);
            const newActions = _.difference(actionChildrenNames, this.previousActionChildrenNames);
            let index : number = 0;

            if (newActions && newActions.length > 0) {
                const firstAction = _.first(newActions);
                index = _.findIndex(actions.toArray(), a => a.action.value === firstAction);
                index = index < 0 ? 0 : index; 
            }
            this.previousActionChildrenNames = actionChildrenNames;
            this.focusFromIndex(actions, index);
        }
    }


    ngAfterViewInit(): void {
        this.focus(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focus(ql));
    }
}