import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'property',
    templateUrl: 'app/property.component.html'
})
export class PropertyComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) {}

    prop: any;

    @Input()
    set property(value: ViewModels.PropertyViewModel) {
        this.prop = value;
    }

    get property() {
        return this.prop;
    }
}