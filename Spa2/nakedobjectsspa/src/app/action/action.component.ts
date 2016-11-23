import { Component, Input, ElementRef } from '@angular/core';
import { ActionViewModel } from '../view-models/action-view-model';

@Component({
    selector: 'action',
    templateUrl: './action.component.html',
    styleUrls: ['./action.component.css']
})
export class ActionComponent {

    constructor(private myElement: ElementRef) {
    }

    @Input()
    action: ActionViewModel;

    get description() {
        return this.action.description;
    }

    get friendlyName() {
        return this.action.title;
    }

    disabled() {
        return this.action.disabled() ? true : null;
    }

    doInvoke(right?: boolean) {
        this.action.doInvoke(right);
    }

    focus() {
        this.myElement.nativeElement.children[0].focus();
    }
}