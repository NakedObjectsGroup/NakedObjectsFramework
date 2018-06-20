import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[nofClick]' })
export class ClickDirective {
    private readonly el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    @Output() leftClick = new EventEmitter();
    @Output() rightClick = new EventEmitter();

    @HostListener('click') onClick() {

        this.leftClick.emit("event");
        return false;
    }

    handleKey(event: KeyboardEvent) {
        const enterKeyCode = 13;
        if (event.which === enterKeyCode) {
            const trigger = event.shiftKey ? this.rightClick : this.leftClick;
            trigger.emit("event");
            return false;
        }

        return true;
    }

    @HostListener('keydown', ['$event']) onEnter(event: KeyboardEvent) {
        return this.handleKey(event);
    }

    @HostListener('keypress', ['$event']) onEnter1(event: KeyboardEvent) {
        return this.handleKey(event);
    }

    @HostListener('contextmenu') onContextMenu() {
        this.rightClick.emit("event");
        return false;
    }
}
