import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';
import * as ViewModels  from './view-models';
import { Validator, AbstractControl } from '@angular/forms';
import {NG_VALIDATORS } from '@angular/forms';
import * as _ from "lodash";

@Directive({
    selector: '[geminiValidate]',
    providers: [{ provide: NG_VALIDATORS, useExisting: GeminiValidateDirective, multi: true }]
})
export class GeminiValidateDirective implements Validator {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
        this.mandatoryOnly = false;
    }

    model: ViewModels.ValueViewModel;

    @Input('geminiValidate')
    set viewModel(vm: ViewModels.ValueViewModel) {
        this.model = vm;
    }

    @Input()
    mandatoryOnly : boolean;

    isValid(viewValue : any): boolean {

        let val: string;

        if (viewValue instanceof ViewModels.ChoiceViewModel) {
            val = viewValue.getValue().toValueString();
        } else if (viewValue instanceof Array) {
            if (viewValue.length) {
                  return _.every(viewValue as (string | ViewModels.ChoiceViewModel)[], (v: any) => this.isValid(v));
            }
            val = "";
        } else {
            val = viewValue as string;
        }

        return this.model.validate(viewValue, val, this.mandatoryOnly);
    };

    validate(c: AbstractControl): { [index: string]: any; } {
        const viewValue = c.value;
        const isvalid = this.isValid(viewValue);
        return isvalid ? null : { invalid: "invalid entry" };
    };
}