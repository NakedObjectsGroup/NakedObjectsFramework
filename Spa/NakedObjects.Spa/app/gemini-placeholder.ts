import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiPlaceholder]' })
export class GeminiPlaceholderDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    //const fn = $parse(attrs.geminiPlaceholder);
    //element.attr("placeholder", fn(scope));

}