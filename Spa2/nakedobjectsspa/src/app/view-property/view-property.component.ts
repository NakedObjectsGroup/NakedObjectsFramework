import { Component, Input, ElementRef, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';

@Component({
    selector: 'view-property',
    templateUrl: './view-property.component.html',
    styleUrls: ['./view-property.component.css']
})
export class ViewPropertyComponent {

    @Input()
    property : ViewModels.PropertyViewModel

    classes(): string {
        return `${this.property.color}`;
    }
}