import { Component, Input, ViewChildren, QueryList } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { EditPropertyComponent } from '../edit-property/edit-property.component';
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import some from 'lodash-es/some';

@Component({
    selector: 'nof-properties',
    templateUrl: 'properties.component.html',
    styleUrls: ['properties.component.css']
})
export class PropertiesComponent {

    @Input()
    parent: DomainObjectViewModel;

    @Input()
    form: FormGroup;

    @Input()
    properties: PropertyViewModel[];

    @ViewChildren(EditPropertyComponent)
    propComponents: QueryList<EditPropertyComponent>;

    focus() {
        const prop = this.propComponents;
        if (prop && prop.length > 0) {
            // until first element returns true
            return some(prop.toArray(), i => i.focus());
        }
        return false;
    }
}
