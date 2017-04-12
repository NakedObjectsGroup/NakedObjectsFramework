import { Directive, ElementRef, HostListener, Input, Renderer } from '@angular/core';
import { FieldViewModel } from './view-models/field-view-model';

@Directive({ selector: '[geminiBoolean]' })
export class GeminiBooleanDirective {

    constructor(
        private readonly el: ElementRef,
        private readonly renderer: Renderer
    ) {
    }

    model: FieldViewModel;

    @Input('geminiBoolean')
    set viewModel(vm: FieldViewModel) {
        this.model = vm;
        this.render();
    }

    render = () => {
        const nativeEl = this.el.nativeElement;

        switch (this.model.value) {
            case true:
                this.renderer.setElementProperty(nativeEl, "indeterminate", false);
                this.renderer.setElementProperty(nativeEl, "checked", true);
                break;
            case false:
                this.renderer.setElementProperty(nativeEl, "indeterminate", false);
                this.renderer.setElementProperty(nativeEl, "checked", false);
                break;
            default: // null
                this.renderer.setElementProperty(nativeEl, "indeterminate", true);
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