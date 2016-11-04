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
    selector: 'edit-property',
    templateUrl: './edit-property.component.html',
    styleUrls: ['./edit-property.component.css']
})
export class EditPropertyComponent extends FieldComponent implements OnInit {

    constructor(myElement: ElementRef) {
        super(myElement);
    }

    prop: ViewModels.PropertyViewModel;

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

    datePickerChanged(evt){
        const val = evt.currentTarget.value;

       // this.formGroup.value[this.property.id] = val;
        
    } 

}