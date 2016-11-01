import { Component, Input, ElementRef , OnInit} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';

@Component({
    host: {
        '(document:click)': 'handleClick($event)'
    },
    selector: 'property',
    templateUrl: './property.component.html',
    styleUrls: ['./property.component.css']
})
export class PropertyComponent extends FieldComponent implements OnInit {

    constructor(myElement: ElementRef) {
        super(myElement);
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

    @Input()
    set form(fm: FormGroup) {
        this.formGroup = fm;
    }

    get form() {
        return this.formGroup;
    }

    ngOnInit(): void {
        super.init(this.parent, this.property, this.form.controls[this.prop.id]);
    }
}