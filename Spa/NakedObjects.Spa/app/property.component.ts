import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Objectcomponent from './object.component';

@Component({
    selector: 'property',
    templateUrl: 'app/property.component.html',
    styleUrls: ['app/property.component.css']
})
export class PropertyComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) {}

    prop: ViewModels.PropertyViewModel;

    @Input()
    edit: boolean;

    @Input()
    parent: ViewModels.DomainObjectViewModel;

    @Input()
    set property(value: ViewModels.PropertyViewModel) {
        this.prop = value;
    }

    get property() {
        return this.prop;
    }
}