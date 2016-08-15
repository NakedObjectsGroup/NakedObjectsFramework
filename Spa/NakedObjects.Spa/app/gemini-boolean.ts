import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiBoolean]' })
export class GeminiBooleanDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    //require: "?ngModel",
    //link(scope: ng.IScope, el: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) {

    //    const parent = scope.$parent as IPropertyOrParameterScope;
    //    const viewModel = parent.parameter || parent.property;

    //    ctrl.$formatters = [];
    //    ctrl.$parsers = [];

    //    ctrl.$validators.geminiBoolean = (modelValue: any, viewValue: string) => {
    //        return viewModel.validate(modelValue, viewValue, true);
    //    };

    //    ctrl.$render = () => {
    //        const d = ctrl.$viewValue as boolean;
    //        el.data("checked", d);
    //        switch (d) {
    //            case true:
    //                el.prop("indeterminate", false);
    //                el.prop("checked", true);
    //                break;
    //            case false:
    //                el.prop("indeterminate", false);
    //                el.prop("checked", false);
    //                break;
    //            default: // null
    //                el.prop("indeterminate", true);
    //        }
    //    };

    //    const triStateClick = () => {
    //        let d: boolean;
    //        const checkedBool: boolean = el.data("checked") as any;
    //        switch (checkedBool) {
    //            case false:
    //                d = true;
    //                break;
    //            case true:
    //                d = null;
    //                break;
    //            default: // null
    //                d = false;
    //        }
    //        ctrl.$setViewValue(d);
    //        scope.$apply(ctrl.$render);
    //    };

    //    const twoStateClick = () => {
    //        let d: boolean;
    //        const checkedBool: boolean = el.data("checked") as any;
    //        switch (checkedBool) {
    //            case true:
    //                d = false;
    //                break;
    //            default: // false or null
    //                d = true;
    //        }
    //        ctrl.$setViewValue(d);
    //        scope.$apply(ctrl.$render);
    //    };

    //    el.bind("click", viewModel.optional ? triStateClick : twoStateClick);
    //}
}