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
    button: IAction;

    constructor(private readonly myElement: ElementRef) { }

    private canClick() {
        return !(this.disabled() || this.tempDisabled());
    }

    doClick() {
        if (this.canClick()) {
            this.button.doClick();
        }
    }

    doRightClick() {
        if (this.canClick() && this.button.doRightClick) {
            this.button.doRightClick();
        }
    }

    class() {
        return ({
            tempdisabled: this.tempDisabled()
        });
    }

    show() {
        return this.button && this.button.show();
    }

    disabled() {
        return this.button.disabled();
    }

    tempDisabled() {
        return this.button.tempDisabled();
    }

    get value() {
        return this.button.value;
    }

    get title() {
        return this.button.title();
    }

    focus() {
        // todo breaks stuff !
        //this.myElement.nativeElement.children[0].focus();
    }
}
