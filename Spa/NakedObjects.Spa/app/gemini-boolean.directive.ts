import { Directive, ElementRef, HostListener, Output, EventEmitter, Input, Renderer } from '@angular/core';
import * as ViewModels from "./nakedobjects.viewmodels";

@Directive({ selector: '[geminiBoolean]' })
export class GeminiBooleanDirective {
    private el: HTMLElement;

    constructor(el: ElementRef, private renderer : Renderer) {
        this.el = el.nativeElement;
    }

    model: ViewModels.IFieldViewModel;

    @Input('geminiBoolean')
    set viewModel(vm: ViewModels.IFieldViewModel) {
        this.model = vm;
        this.render();
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

    render = () => {

        switch (this.model.value) {
            case true:
                this.renderer.setElementProperty(this.el, "indeterminate", false);
                this.renderer.setElementProperty(this.el, "checked", true);
               
                break;
            case false:
                this.renderer.setElementProperty(this.el, "indeterminate", false);
                this.renderer.setElementProperty(this.el, "checked", false);
               
                break;
            default: // null
                this.renderer.setElementProperty(this.el, "indeterminate", true);
        }
    };


    triStateClick = () => {

        switch (this.model.value) {
        case false:
            this.model.value = true;
            break;
        case true:
            this.model.value = null;
            break;
        default: // null
            this.model.value = false;
        }

        this.render();
    };

    twoStateClick = () => {
        this.model.value = !this.model.value;
        this.render();
    };

    @HostListener('click')
    onClick() {
        const click = this.model.optional ? this.triStateClick : this.twoStateClick;
        click();
    }
}