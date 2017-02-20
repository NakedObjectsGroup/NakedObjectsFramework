import { Component, Input, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { EditPropertyComponent } from '../edit-property/edit-property.component'
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import * as Models from '../models';

@Component({
    selector: 'nof-properties',
    template: require('./properties.component.html'),
    styles: [require('./properties.component.css')]
})
export class PropertiesComponent implements AfterViewInit {

    @Input()
    parent: DomainObjectViewModel;

    @Input()
    form: FormGroup;

    @Input()
    properties: PropertyViewModel[];

    @ViewChildren(EditPropertyComponent)
    propComponents: QueryList<EditPropertyComponent>;

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