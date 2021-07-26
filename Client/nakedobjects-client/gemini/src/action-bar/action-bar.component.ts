import { AfterViewInit, Component, Input, OnDestroy, QueryList, ViewChildren } from '@angular/core';
import { IMenuHolderViewModel, MenuItemViewModel } from '@nakedobjects/view-models';
import flatten from 'lodash-es/flatten';
import map from 'lodash-es/map';
import some from 'lodash-es/some';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { IActionHolder, wrapAction } from '../action/action.component';
import { ActionComponent } from '../action/action.component';
import { safeUnsubscribe } from '../helpers-components';

@Component({
    selector: 'nof-action-bar',
    templateUrl: 'action-bar.component.html',
    styleUrls: ['action-bar.component.css']
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

    private sub: ISubscription;

    classes(action: IActionHolder) {
        const hint = action.presentationHint ?? '';
        return hint.trim();
    }

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions) {
            // until first element returns true
            some(actions.toArray(), i => i.focus());
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.actionChildren);
        this.sub = this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.sub);
    }
}
