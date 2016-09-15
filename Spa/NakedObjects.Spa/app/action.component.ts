import { Component, Input } from '@angular/core';
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'action',
    templateUrl: 'app/action.component.html',
    styleUrls: ['app/action.component.css']
})
export class ActionComponent {

    private actionVm: ViewModels.ActionViewModel;

    disabled() { return false; }

    doInvoke(right?: boolean) {
        this.actionVm.doInvoke(right);
    }

    @Input()
    set action(value: ViewModels.ActionViewModel) {

        this.actionVm = value;

        this.description = this.actionVm.description;
        this.friendlyName = this.actionVm.title;
    }

    description: string;
    friendlyName: string;
}