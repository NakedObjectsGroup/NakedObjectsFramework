import { Component, OnInit, Input } from '@angular/core';

export interface IButton {
    doClick: () => void;
    show: () => boolean;
    disabled: () => boolean;
    value: string;
    title: () => string;
    accesskey : string;
}

@Component({
    selector: 'nof-button',
    templateUrl: './button.component.html',
    styleUrls: ['./button.component.css']
})
export class ButtonComponent {

    @Input()
    button: IButton;

    doClick() {
        this.button.doClick();
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
