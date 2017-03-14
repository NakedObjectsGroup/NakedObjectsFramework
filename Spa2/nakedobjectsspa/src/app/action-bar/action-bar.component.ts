import { Component, OnInit, Input, QueryList, ElementRef, AfterViewInit, ViewChildren } from '@angular/core';
import { IActionHolder, wrapAction } from '../action/action.component';
import { IMenuHolderViewModel } from '../view-models/imenu-holder-view-model';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { ActionComponent } from '../action/action.component';

@Component({
    selector: 'nof-action-bar',
    template: require('./action-bar.component.html'),
    styles: [require('./action-bar.component.css')]
})
export class ActionBarComponent {

    @Input()
    actions: IActionHolder[];

    @Input()
    set menuHolder(mhvm: IMenuHolderViewModel) {
        const menuItems = mhvm.menuItems;
        const avms = _.flatten(_.map(menuItems, (mi: MenuItemViewModel) => mi.actions!));
        this.actions = _.map(avms, a => wrapAction(a));
    }

    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions) {
            // until first element returns true
            _.some(actions.toArray(), i => i.focus());
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }
}
