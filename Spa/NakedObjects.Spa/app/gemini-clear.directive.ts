import { Directive, ElementRef, HostListener, Output, EventEmitter, Renderer, Input, OnInit } from '@angular/core';
import * as ViewModels from './nakedobjects.viewmodels';

@Directive({ selector: '[geminiClear]' })
export class GeminiClearDirective implements OnInit {
    private el: HTMLInputElement;

    constructor(el: ElementRef, private renderer: Renderer) {
        this.el = el.nativeElement;
    }

    model: ViewModels.IFieldViewModel;

    @Input('geminiClear')
    set viewModel(vm: ViewModels.IFieldViewModel) {
        this.model = vm;
    }

    ngOnInit(): void {
        this.onChange();
    }

    onChange() {
        this.el.classList.add("ng-clearable");

        if (this.el.value) {
            this.el.classList.add("ng-x");
        } else {
            this.el.classList.remove("ng-x");
        }
    }

    onMouseMove(event: MouseEvent) {
        if (this.el.classList.contains("ng-x")) {
            const onX = this.el.offsetWidth - 18 < event.clientX - this.el.getBoundingClientRect().left;
            if (onX) {
                this.el.classList.add("ng-onX");
            } else {
                this.el.classList.remove("ng-onX");
            }
        }
    }

    onClick(event: KeyboardEvent) {
        if (this.el.classList.contains("ng-onX")) {

            event.preventDefault();
            this.el.classList.remove("ng-x");
            this.el.classList.remove("ng-onX");

            this.model.clear();
        }
    }

    @HostListener("click", ['$event'])
    click(event: KeyboardEvent) {
        this.onClick(event);
    }

    @HostListener("touchstart", ['$event'])
    touchstart(event: KeyboardEvent) {
        this.onClick(event);
    }

    @HostListener("mousemove", ['$event'])
    mousemove(event: MouseEvent) {
        this.onMouseMove(event);
    }

    @HostListener("input")
    input() {
        this.onChange();
    }

    @HostListener("change")
    change() {
        this.onChange();
    }
}
