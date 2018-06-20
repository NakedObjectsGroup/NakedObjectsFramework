import { Directive, ElementRef, HostListener, Output, EventEmitter, Renderer, Input, OnInit, OnDestroy } from '@angular/core';
import { BehaviorSubject ,  SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from './helpers-components';

@Directive({ selector: '[nofClear]' })
export class ClearDirective implements OnInit, OnDestroy {

    private readonly nativeEl: HTMLInputElement;

    constructor(
        private readonly el: ElementRef,
        private readonly renderer: Renderer
    ) {
        this.nativeEl = this.el.nativeElement;
    }

    // tslint:disable-next-line:no-input-rename
    @Input('nofClear')
    subject: BehaviorSubject<any>;

    @Output()
    clear = new EventEmitter();

    private sub: ISubscription;

    ngOnInit(): void {
        this.onChange();
        this.sub = this.subject.subscribe(data => this.onChange());
    }

    // not need the ngClass directive on element even though it doesn't do anything
    // otherwise we lose all the classes added here
    onChange() {

        this.nativeEl.classList.add("ng-clearable");

        if (this.subject.getValue()) {
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
            this.clear.emit("event");
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

    ngOnDestroy() {
        safeUnsubscribe(this.sub);
    }

}
