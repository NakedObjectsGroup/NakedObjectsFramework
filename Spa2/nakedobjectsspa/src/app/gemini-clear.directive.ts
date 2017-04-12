import { Directive, ElementRef, HostListener, Output, EventEmitter, Renderer, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldViewModel } from './view-models/field-view-model';

@Directive({ selector: '[geminiClear]' })
export class GeminiClearDirective implements OnInit {

    private readonly nativeEl: HTMLInputElement;

    constructor(
        private readonly el: ElementRef,
        private readonly renderer: Renderer
    ) {
        this.nativeEl = this.el.nativeElement;
    }

    model: FieldViewModel;
    formGroup: FormGroup;

    @Input('geminiClear')
    set viewModel(vm: FieldViewModel) {
        this.model = vm;
    }

    @Input()
    set form(fm: FormGroup) {
        this.formGroup = fm;
    }

    ngOnInit(): void {
        this.onChange();
        this.formGroup.controls[this.model.id].valueChanges.subscribe(data => this.onChange());
    }

    // not need the ngClass directive on element even though it doesn't do anything 
    // otherwise we lose all the classes added here 
    onChange() {

        this.nativeEl.classList.add("ng-clearable");

        if (this.formGroup.controls[this.model.id].value) {
            this.nativeEl.classList.add("ng-x");
        } else {
            this.nativeEl.classList.remove("ng-x");
        }
    }

    onMouseMove(event: MouseEvent) {
        if (this.nativeEl.classList.contains("ng-x")) {
            const onX = this.nativeEl.offsetWidth - 18 < event.clientX - this.nativeEl.getBoundingClientRect().left;
            if (onX) {
                this.nativeEl.classList.add("ng-onX");
            } else {
                this.nativeEl.classList.remove("ng-onX");
            }
        }
    }

    onClick(event: KeyboardEvent) {
        if (this.nativeEl.classList.contains("ng-onX")) {

            event.preventDefault();
            this.nativeEl.classList.remove("ng-x");
            this.nativeEl.classList.remove("ng-onX");

            this.model.clear();
            this.formGroup.controls[this.model.id].reset("");
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
