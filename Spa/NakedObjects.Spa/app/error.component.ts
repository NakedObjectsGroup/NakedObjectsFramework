import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { RepresentationsService } from './representations.service'
import { getAppPath } from "./nakedobjects.config";
import { Observable } from 'rxjs/Observable';
import { ISubscription } from 'rxjs/Subscription';
import { ActionsComponent } from "./actions.component";
import { ActivatedRoute, Router, Data } from '@angular/router';
import { UrlManager } from "./urlmanager.service";
import { Context } from "./context.service";
import { Error } from './error.service';
import { FocusManager } from "./focus-manager.service";
import { ViewModelFactory } from "./view-model-factory.service";
import { Color } from "./color.service";
import { DialogComponent } from "./dialog.component";
import { RouteData, PaneRouteData } from "./nakedobjects.routedata";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";


@Component({
    selector: 'error',
    templateUrl: 'app/error.component.html'
})
export class ErrorComponent {

    constructor(private context : Context, private viewModelFactory : ViewModelFactory) {  }

    error : ViewModels.ErrorViewModel;

    ngOnInit(): void {
        const errorWrapper = this.context.getError();
        this.error = this.viewModelFactory.errorViewModel(errorWrapper);
    }
}
