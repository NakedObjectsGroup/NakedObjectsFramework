import { Component, Input } from '@angular/core';
import * as Models from "./models";
import { Observable } from 'rxjs/Observable';
import {NgClass} from '@angular/common';
import {formatType, scalarValueType} from './nakedobjects.rointerfaces'

import * as _ from "lodash";
import * as Nakedobjectsroutedata from "./nakedobjects.routedata";
import * as Nakedobjectsconstants from "./nakedobjects.constants";
import * as Usermessagesconfig from "./user-messages.config";
import * as Nakedobjectsconfig from "./nakedobjects.config";
import {Color} from "./color.service";
import {Context} from  "./context.service";
import {Error} from  "./error.service";
import * as Urlmanagerservice from "./urlmanager.service";
import * as Clickhandlerservice from "./click-handler.service";
import * as Nakedobjectsviewmodels from "./nakedobjects.viewmodels";
import * as Geminiclickdirective from "./gemini-click.directive";

@Component({
    selector: 'action',
    templateUrl: 'app/action.component.html',
    directives: [Geminiclickdirective.GeminiClickDirective]
})


export class ActionComponent {

    constructor(private context : Context) {  }

    private actionVm : Nakedobjectsviewmodels.ActionViewModel;

    disabled() { return false; }

    doInvoke(right?: boolean) {
        this.actionVm.doInvoke(right);
    }

    @Input()
    set action(value: Nakedobjectsviewmodels.ActionViewModel) {

        this.actionVm = value;

        this.description = this.actionVm.description;
        this.friendlyName = this.actionVm.title;
    }

    description: string;
    friendlyName: string;
}