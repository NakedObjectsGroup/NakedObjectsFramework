import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';
import * as ViewModels  from './nakedobjects.viewmodels';
import { Validator, AbstractControl } from '@angular/forms';

@Directive({ selector: '[geminiFieldValidate]' })
export class GeminiFieldValidateDirective implements Validator {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    model: ViewModels.IFieldViewModel;

    @Input('geminiFieldValidate')
    set viewModel(vm: ViewModels.IFieldViewModel) {
        this.model = vm;
    }

    //require: "ngModel",
    //link(scope: ng.IScope, elm: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) {
    //    ctrl.$validators.geminiFieldvalidate = (modelValue: any, viewValue: string) => {
    //        const parent = scope.$parent as IPropertyOrParameterScope;
    //        const viewModel = parent.parameter || parent.property;
    //        return viewModel.validate(modelValue, viewValue, false);
    //    };
    //}

    validate(c: AbstractControl): { [index: string]: any; } {

    
        let val: string;
        const viewValue = c.value;

        if (viewValue instanceof ViewModels.ChoiceViewModel) {
            val = viewValue.getValue().toValueString();
        } else if (viewValue instanceof Array) {
            if (viewValue.length) {
                //  return _.every(viewValue as (string | ViewModels.ChoiceViewModel)[], (v: any) => ctrl.$validators.geminiFieldmandatorycheck(v, v));
            }
            val = "";
        } else {
            val = viewValue as string;
        }



        return this.model.validate(viewValue, val, true) ? null : { invalid: "" };
    };
}