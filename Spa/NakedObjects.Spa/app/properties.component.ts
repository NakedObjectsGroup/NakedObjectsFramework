import { Component, Input } from '@angular/core';
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'properties',
    templateUrl: 'app/properties.component.html',
    styleUrls: ['app/properties.component.css']
})

export class PropertiesComponent {

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