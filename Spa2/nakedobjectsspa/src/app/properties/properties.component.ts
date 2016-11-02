import { Component, Input } from '@angular/core';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FormGroup }                 from '@angular/forms';

@Component({
    selector: 'properties',
    templateUrl: './properties.component.html',
    styleUrls: ['./properties.component.css']
})

export class PropertiesComponent {

    props: ViewModels.PropertyViewModel[];

    @Input()
    parent: ViewModels.DomainObjectViewModel;

    @Input()
    form: FormGroup;

    @Input()
    set properties(value: ViewModels.PropertyViewModel[]) {
        this.props = value;
    }

    get properties() {
        return this.props;
    }
}