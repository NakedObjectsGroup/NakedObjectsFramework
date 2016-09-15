import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';
import { Validator, AbstractControl } from '@angular/forms';
import * as ViewModels from "./nakedobjects.viewmodels";

@Directive({ selector: '[geminiFieldMandatoryCheck]' })
export class GeminiFieldMandatoryCheckDirective implements Validator {
    private el: HTMLElement;

    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    model: ViewModels.IFieldViewModel;

    @Input('geminiFieldMandatoryCheck')
    set viewModel(vm: ViewModels.IFieldViewModel) {
        this.model = vm;
    }


    //require: "ngModel",
    //link(scope: ng.IScope, elm: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) {
    //    ctrl.$validators.geminiFieldmandatorycheck = (modelValue: any, viewValue: string | ChoiceViewModel | string[] | ChoiceViewModel[]) => {
    //        const parent = scope.$parent as IPropertyOrParameterScope;
    //        const viewModel = parent.parameter || parent.property;
    //        let val: string;

    //        if (viewValue instanceof ChoiceViewModel) {
    //            val = viewValue.getValue().toValueString();
    //        }
    //        else if (viewValue instanceof Array) {
    //            if (viewValue.length) {
    //                return _.every(viewValue as (string | ChoiceViewModel)[], (v: any) => ctrl.$validators.geminiFieldmandatorycheck(v, v));
    //            }
    //            val = "";
    //        }
    //        else {
    //            val = viewValue as string;
    //        }

    //        return viewModel.validate(modelValue, val, true);
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

        return this.model.validate(viewValue, val, true) ? null : { required: "" };
    };
}