import { Component, Input } from '@angular/core';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import { PropertyComponent } from "./property.component";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'properties',
    templateUrl: 'app/properties.component.html',
    directives: [PropertyComponent]
})

export class PropertiesComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) { }

    props : any;
   
    @Input()
    set properties(value: ViewModels.PropertyViewModel[]) {
        this.props = value;
    }

    get properties() {
        return this.props;
    }
}