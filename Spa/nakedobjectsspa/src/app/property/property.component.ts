import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { AbstractDroppableComponent } from '../abstract-droppable/abstract-droppable.component';

@Component({
    selector: 'property',
    templateUrl: './property.component.html',
    styleUrls: ['./property.component.css']
})
export class PropertyComponent extends AbstractDroppableComponent {

    constructor() {
        super();
    }

    prop: ViewModels.PropertyViewModel;

    @Input()
    edit: boolean;

    @Input()
    parent: ViewModels.DomainObjectViewModel;

    @Input()
    set property(value: ViewModels.PropertyViewModel) {
        this.prop = value;
        this.droppable = value as ViewModels.IFieldViewModel;
    }

    get property() {
        return this.prop;
    }

    classes(): string {
        return `${this.prop.color}${this.canDrop ? " candrop" : ""}`;
    }

}