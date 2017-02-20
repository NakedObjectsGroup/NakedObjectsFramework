import { Component, OnInit, Input } from '@angular/core';

export interface IButton {
    doClick: () => void;
    doRightClick?: () => void;
    show: () => boolean;
    disabled: () => boolean | null;
    value: string;
    title: () => string;
    accesskey: string | null;
}

@Component({
    selector: 'nof-button',
    template: require('./button.component.html'),
    styles: [require('./button.component.css')]
})
export class ButtonComponent {

    @Input()
    button: IButton;

    doClick() {
        this.button.doClick();
    }

    doRightClick() {
        if (this.button.doRightClick) {
            this.button.doRightClick();
        }
    }

    show() {
        return this.button && this.button.show();
    }

    disabled() {
        return this.button.disabled();
    }

    get value() {
        return this.button.value;
    }

    get title() {
        return this.button.title();
    }

}
