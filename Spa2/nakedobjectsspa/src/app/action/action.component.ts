import { Component, Input, ElementRef } from '@angular/core';
import * as ViewModels from "../view-models";

@Component({
    selector: 'action',
    templateUrl: './action.component.html',
    styleUrls: ['./action.component.css']
})
export class ActionComponent {

    constructor(private myElement: ElementRef) {
    }

    private actionVm: ViewModels.ActionViewModel;

    description: string;
    friendlyName: string;

    @Input()
    set action(value: ViewModels.ActionViewModel) {
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