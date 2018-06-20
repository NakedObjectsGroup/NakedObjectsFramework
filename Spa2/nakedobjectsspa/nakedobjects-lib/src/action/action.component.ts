import { Component, Input, ElementRef, Renderer, ViewChildren, QueryList } from '@angular/core';
import { ActionViewModel } from '../view-models/action-view-model';
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
        accesskey: null
    };
}

@Component({
    selector: 'nof-action',
    templateUrl: 'action.component.html',
    styleUrls: ['action.component.css']
})
export class ActionComponent {

    @Input()
    action: IActionHolder;

    @ViewChildren("focus")
    focusList: QueryList<ElementRef>;

    constructor(
        private readonly renderer: Renderer
    ) { }

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
        if (this.disabled()) {
            return false;
        }
        return !!(this.focusList && this.focusList.first) && focus(this.renderer, this.focusList.first);
    }
}
