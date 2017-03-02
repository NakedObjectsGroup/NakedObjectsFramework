import { Component, OnInit, Input, ElementRef } from '@angular/core';

export interface IAction {
    doClick: () => void;
    doRightClick?: () => void;
    show: () => boolean;
    disabled: () => boolean | null;
    tempDisabled: () => boolean | null;
    value: string;
    title: () => string;
    accesskey: string | null;
}

@Component({
    selector: 'nof-action',
    template: require('./action.component.html'),
    styles: [require('./action.component.css')]
})
export class ActionComponent {

    @Input()
    action: IAction;

    constructor(private readonly myElement: ElementRef) { }

    private canClick() {
        return !(this.disabled() || this.tempDisabled());
    }

    doClick() {
        if (this.canClick()) {
            this.action.doClick();
        }
    }

    doRightClick() {
        if (this.canClick() && this.action.doRightClick) {
            this.action.doRightClick();
        }
    }

    class() {
        return ({
            tempdisabled: this.tempDisabled()
        });
    }

    show() {
        return this.action && this.action.show();
    }

    disabled() {
        return this.action.disabled();
    }

    tempDisabled() {
        return this.action.tempDisabled();
    }

    get value() {
        return this.action.value;
    }

    get title() {
        return this.action.title();
    }

    focus() {
        // todo breaks stuff !
        //this.myElement.nativeElement.children[0].focus();
    }
}
