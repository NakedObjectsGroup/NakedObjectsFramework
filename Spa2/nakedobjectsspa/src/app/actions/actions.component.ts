import { Component, Input, ViewChildren, QueryList, ElementRef, AfterViewInit, OnInit } from '@angular/core';
import { ActionComponent } from '../action/action.component';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { ActionViewModel } from '../view-models/action-view-model'; // needed for declarations compile 
import * as Models from '../models';

@Component({
    selector: 'nof-actions',
    template: require('./actions.component.html'),
    styles: [require('./actions.component.css')]
})
export class ActionsComponent implements OnInit, AfterViewInit {
   

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

    displayContextClass() {
        return ({
            objectContext: this.isObjectContext(),
            collectionContext: this.isCollectionContext(),
            homeContext : this.isHomeContext()
        });
    }

    firstAction : ActionViewModel;

    isObjectContext() {
        return this.firstAction && this.firstAction.actionRep.parent instanceof Models.DomainObjectRepresentation;
    }

    isCollectionContext() {
        return this.firstAction && this.firstAction.actionRep.parent instanceof Models.CollectionMember;
    }

    isHomeContext() {
        return this.firstAction && this.firstAction.actionRep.parent instanceof Models.MenuRepresentation;
    }

    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions && actions.first) {
            actions.first.focus();
        }
    }

    findFirstAction(menuItems: MenuItemViewModel[]) : ActionViewModel | null {
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