import { Component, Input } from '@angular/core';
import * as Models from "../models";
import * as ViewModels from "../view-models";

@Component({
    selector: 'properties',
    templateUrl: './properties.component.html',
    styleUrls: ['./properties.component.css']
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