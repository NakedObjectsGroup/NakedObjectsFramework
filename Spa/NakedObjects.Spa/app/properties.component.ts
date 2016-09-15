import { Component, Input } from '@angular/core';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import { PropertyComponent } from "./property.component";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Objectcomponent from './object.component';

@Component({
    selector: 'properties',
    templateUrl: 'app/properties.component.html',
    styleUrls: ['app/properties.component.css']
})

export class PropertiesComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) { }

    props: ViewModels.PropertyViewModel[];

    @Input()
    edit: boolean;

    @Input()
    parent: ViewModels.DomainObjectViewModel;

    @Input()
    set properties(value: ViewModels.PropertyViewModel[]) {
        this.props = value;
    }

    get properties() {
        return this.props;
    }
}