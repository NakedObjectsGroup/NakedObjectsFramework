import { Component, Input, QueryList, ViewChildren, OnDestroy, AfterViewInit } from '@angular/core';
import { IActionHolder, wrapAction } from '../action/action.component';
import { IMenuHolderViewModel } from '../view-models/imenu-holder-view-model';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { ActionComponent } from '../action/action.component';
import { ISubscription } from 'rxjs/Subscription';
import flatten from 'lodash/flatten';
import map from 'lodash/map';
import some from 'lodash/map';

@Component({
    selector: 'nof-action-bar',
    template: require('./action-bar.component.html'),
    styles: [require('./action-bar.component.css')]
})
export class ActionBarComponent implements OnDestroy, AfterViewInit {
   
    @Input()
    actions: IActionHolder[];

    @Input()
    set menuHolder(mhvm: IMenuHolderViewModel) {
        const menuItems = mhvm.menuItems;
        const avms = flatten(map(menuItems || [], (mi: MenuItemViewModel) => mi.actions!));
        this.actions = map(avms, a => wrapAction(a));
    }

    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions) {
            // until first element returns true
            some(actions.toArray(), i => i.focus());
        }
    }

    private sub : ISubscription;

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.actionChildren);
        this.sub = this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }

    ngOnDestroy(): void {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}
