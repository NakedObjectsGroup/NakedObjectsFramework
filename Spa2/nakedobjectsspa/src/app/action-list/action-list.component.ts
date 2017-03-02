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
export class ActionListComponent implements OnInit, AfterViewInit {


    @Input()
    menuHolder: IMenuHolderViewModel;

    get items() {
        return this.menuHolder.menuItems;
    }

    private getActionHolders(menuItem: MenuItemViewModel) {
        // todo investigate caching this 
        return _.map(menuItem.actions, a => wrapAction(a));
    }

    menuName = (menuItem: MenuItemViewModel) => menuItem.name;

    menuItems = (menuItem: MenuItemViewModel) => menuItem.menuItems;

    menuActions = (menuItem: MenuItemViewModel) => this.getActionHolders(menuItem);

    toggleCollapsed = (menuItem: MenuItemViewModel) => menuItem.toggleCollapsed();

    navCollapsed = (menuItem: MenuItemViewModel) => menuItem.navCollapsed;

    displayClass = (menuItem: MenuItemViewModel) => ({ collapsed: menuItem.navCollapsed, open: !menuItem.navCollapsed, rootMenu: !menuItem.name });

    firstAction: ActionViewModel;

    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions && actions.first) {
            actions.first.focus();
        }
    }

    findFirstAction(menuItems: MenuItemViewModel[]): ActionViewModel | null {
        for (const mi of menuItems) {
            if (mi.actions && mi.actions.length > 0) {
                return mi.actions[0];
            }
            return this.findFirstAction(mi.menuItems);
        }
        return null;
    }

    ngOnInit(): void {
        this.firstAction = this.findFirstAction(this.items);
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }
}