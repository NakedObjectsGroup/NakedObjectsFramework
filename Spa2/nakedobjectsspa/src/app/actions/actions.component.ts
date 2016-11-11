import { Component, Input, ViewChildren, QueryList, ElementRef } from '@angular/core';
import * as ViewModels from "../view-models";
import { ActionComponent } from "../action/action.component";

@Component({
    selector: 'actions',
    templateUrl: './actions.component.html',
    styleUrls: ['./actions.component.css']
})
export class ActionsComponent {

    @Input()
    menuVm: { menuItems: ViewModels.IMenuItemViewModel[] };

    @ViewChildren(ActionComponent) acts: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions && actions.first) {
            actions.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.acts);
        this.acts.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }
}