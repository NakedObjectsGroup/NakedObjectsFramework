import { Component, Input, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FormGroup } from '@angular/forms';
import {EditPropertyComponent} from "../edit-property/edit-property.component"

@Component({
    selector: 'properties',
    templateUrl: './properties.component.html',
    styleUrls: ['./properties.component.css']
})

export class PropertiesComponent implements AfterViewInit {

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

    @ViewChildren(EditPropertyComponent) propComponents: QueryList<EditPropertyComponent>;

    focusOnFirstProperty(prop: QueryList<EditPropertyComponent>) {
        if (prop && prop.first) {
            prop.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstProperty(this.propComponents);
        this.propComponents.changes.subscribe((ql: QueryList<EditPropertyComponent>) => this.focusOnFirstProperty(ql));
    }
}