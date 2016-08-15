import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiFieldValidate]' })
export class GeminiFieldValidateDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    //require: "ngModel",
    //link(scope: ng.IScope, elm: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) {
    //    ctrl.$validators.geminiFieldvalidate = (modelValue: any, viewValue: string) => {
    //        const parent = scope.$parent as IPropertyOrParameterScope;
    //        const viewModel = parent.parameter || parent.property;
    //        return viewModel.validate(modelValue, viewValue, false);
    //    };
    //}
}