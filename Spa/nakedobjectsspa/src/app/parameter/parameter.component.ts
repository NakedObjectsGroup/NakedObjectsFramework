import { Component, Input, OnInit } from '@angular/core';
import { NG_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { ViewModelFactoryService } from "../view-model-factory.service";
import { UrlManagerService } from "../url-manager.service";
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';

@Component({
    selector: 'parameter',
    templateUrl: './parameter.component.html',
    styleUrls: ['./parameter.component.css']
})
export class ParameterComponent extends FieldComponent implements OnInit {

    constructor(private viewModelFactory: ViewModelFactoryService, private urlManager: UrlManagerService) {
        super();
    }

    parm: ViewModels.ParameterViewModel;

    _form: FormGroup;

    message: string;

    onValueChanged(data?: any) {
        // clear previous error message (if any)
        this.message = '';

        if (this.parm) {
            const control = this._form.get(this.parm.id);
            if (control && control.dirty && !control.valid) {
                this.message = this.parm.getMessage();
            }
            super.onChange();
        }
    }

    @Input()
    set form(fm: FormGroup) {
        this._form = fm;
        this.form.valueChanges.subscribe(data => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation messages now
    }

    get form() {
        return this._form;
    }


    @Input()
    parent: ViewModels.DialogViewModel;

    @Input()
    set parameter(value: ViewModels.ParameterViewModel) {
        this.parm = value;
        this.droppable = value as ViewModels.IFieldViewModel;
    }

    get parameter() {
        return this.parm;
    }

    classes(): string {
        return `${this.parm.color}${this.canDrop ? " candrop" : ""}`;
    }

    ngOnInit(): void {
        super.init(this.parent, this.parameter, this.form.controls[this.parm.id]);
    }

}