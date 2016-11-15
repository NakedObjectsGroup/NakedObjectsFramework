import { Component, Input, ElementRef } from '@angular/core';
import * as ViewModels from "../view-models";
import { ActionViewModel } from '../view-models/action-view-model';

@Component({
    selector: 'action',
    templateUrl: './action.component.html',
    styleUrls: ['./action.component.css']
})
export class ActionComponent {

    constructor(private myElement: ElementRef) {
    }

    private actionVm: ActionViewModel;

    description: string;
    friendlyName: string;

    @Input()
    set action(value: ActionViewModel) {
        this.actionVm = value;
        this.description = this.actionVm.description;
        this.friendlyName = this.actionVm.title;
    }

    disabled() { return this.actionVm.disabled(); }

    doInvoke(right?: boolean) {
        this.actionVm.doInvoke(right);
    }

    focus() {
        this.myElement.nativeElement.children[0].focus();
    }
}