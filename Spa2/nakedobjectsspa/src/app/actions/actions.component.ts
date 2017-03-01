import { Component, Input, ViewChildren, QueryList, ElementRef, AfterViewInit, OnInit } from '@angular/core';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { ActionViewModel } from '../view-models/action-view-model'; // needed for declarations compile 
import * as Models from '../models';
import { IButton } from '../action/action.component';
import * as Actioncomponent from '../action/action.component';

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

    //private actionButtons : _.Dictionary<IButton[]> = {};

    private getActionButtons(menuItem: MenuItemViewModel) {
        // todo DRY this clone from ObjectComponent
        //if (!this.actionButtons[menuItem.name]) {
        //const menuItems = this.menuItems() !;
        //const actions = _.flatten(_.map(menuItems, (mi: MenuItemViewModel) => mi.actions!));
        const actions = menuItem.actions;

        // todo investigate caching this 
        return _.map(actions,
            a => ({
                value: a.title,
                doClick: () =>
                    a.doInvoke(),
                doRightClick: () =>
                    a.doInvoke(true),
                show: () => true,
                disabled: () => a.disabled() ? true : null,
                tempDisabled: () => a.tempDisabled(),
                title: () => a.description,
                accesskey: null
            })) as IButton[];
        //}

        //return this.actionButtons[menuItem.name];
    }


    menuName = (menuItem: MenuItemViewModel) => menuItem.name;

    menuItems = (menuItem: MenuItemViewModel) => menuItem.menuItems;

    menuActions = (menuItem: MenuItemViewModel) => menuItem.actions;

    menuButtons = (menuItem: MenuItemViewModel) => this.getActionButtons(menuItem);

    toggleCollapsed = (menuItem: MenuItemViewModel) => menuItem.toggleCollapsed();

    navCollapsed = (menuItem: MenuItemViewModel) => menuItem.navCollapsed;

    displayClass = (menuItem: MenuItemViewModel) => ({ collapsed: menuItem.navCollapsed, open: !menuItem.navCollapsed, rootMenu: !menuItem.name });

    displayContextClass() {
        return ({
            objectContext: this.isObjectContext(),
            collectionContext: this.isCollectionContext(),
            homeContext: this.isHomeContext()
        });
    }

    firstAction: ActionViewModel;

    isObjectContext() {
        return this.firstAction && this.firstAction.actionRep.parent instanceof Models.DomainObjectRepresentation;
    }

    isCollectionContext() {
        return this.firstAction && this.firstAction.actionRep.parent instanceof Models.CollectionMember;
    }

    isHomeContext() {
        return this.firstAction && this.firstAction.actionRep.parent instanceof Models.MenuRepresentation;
    }

    @ViewChildren(Actioncomponent.ActionComponent)
    actionChildren: QueryList<Actioncomponent.ActionComponent>;

    focusOnFirstAction(actions: QueryList<Actioncomponent.ActionComponent>) {
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
        this.actionChildren.changes.subscribe((ql: QueryList<Actioncomponent.ActionComponent>) => this.focusOnFirstAction(ql));
    }
}