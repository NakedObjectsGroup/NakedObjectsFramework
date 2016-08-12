import { Component, Input } from '@angular/core';
import * as Models from "./models";
import { Observable } from 'rxjs/Observable';
import { formatType, scalarValueType } from './nakedobjects.rointerfaces'
import * as _ from "lodash";
import { Context } from "./context.service";
import * as ViewModels from "./nakedobjects.viewmodels";
import { GeminiClickDirective } from "./gemini-click.directive";

@Component({
    selector: 'action',
    templateUrl: 'app/action.component.html',
    directives: [GeminiClickDirective]
})
export class ActionComponent {

    constructor(private context: Context) {}

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