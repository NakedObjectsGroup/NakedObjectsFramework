﻿import { Component, ElementRef, Input, QueryList, ViewChildren } from '@angular/core';
import { ActionViewModel } from '@nakedobjects/view-models';
import { focus } from '../helpers-components';

export interface IActionHolder {
    doClick: () => void;
    doRightClick?: () => void;
    show: () => boolean;
    disabled: () => boolean | null;
    tempDisabled: () => boolean | null;
    value: string;
    title: () => string;
    accesskey: string | null;
    presentationHint: string;
    showDialog: () => boolean;
}

export function wrapAction(a: ActionViewModel): IActionHolder {
    return {
        value: a.title,
        doClick: () => a.doInvoke(),
        doRightClick: () => a.doInvoke(true),
        show: () => true,
        disabled: () => a.disabled() ? true : null,
        tempDisabled: () => a.tempDisabled(),
        title: () => a.description,
        accesskey: null,
        presentationHint: a.presentationHint,
        showDialog: () => a.showDialog()
    };
}

@Component({
    selector: 'nof-action',
    templateUrl: 'action.component.html',
    styleUrls: ['action.component.css'],
    standalone: false
})
export class ActionComponent {

    @Input({ required: true })
    action!: IActionHolder;

    @ViewChildren('focus')
    focusList?: QueryList<ElementRef>;

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
            tempdisabled: this.tempDisabled(),
            [this.dialogClass()]: true,
        });
    }

    show() {
        return this.action.show();
    }

    disabled() {
        return this.action.disabled();
    }

    tempDisabled() {
        return this.action.tempDisabled();
    }

    dialogClass() {
        return this.showDialog() ? 'has-params' : 'no-params';
    }

    showDialog() {
        return this.action.showDialog();
    }

    get value() {
        return this.action.value;
    }

    get title() {
        return this.action.title();
    }

    focus() {
        if (this.disabled()) {
            return false;
        }
        return !!(this.focusList && this.focusList.first) && focus(this.focusList.first);
    }
}
