import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiFieldMandatoryCheck]' })
export class GeminiFieldMandatoryCheckDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
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
}