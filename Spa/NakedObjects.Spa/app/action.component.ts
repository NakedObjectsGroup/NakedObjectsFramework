import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { formatType, scalarValueType } from './nakedobjects.rointerfaces'
import { Context } from "./context.service";
import { GeminiClickDirective } from "./gemini-click.directive";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as _ from "lodash";

@Component({
    selector: 'action',
    templateUrl: 'app/action.component.html',
    directives: [GeminiClickDirective],
    styleUrls: ['app/action.component.css']
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