import { Component, Input, ElementRef, OnInit, OnDestroy } from '@angular/core';
import { ActionViewModel } from '../view-models/action-view-model';
import { ContextService} from '../context.service';
import { ISubscription } from 'rxjs/Subscription';
import * as Models from '../models';
import { IButton } from '../button/button.component';

@Component({
    selector: 'nof-action',
    template: require('./action.component.html'),
    styles: [require('./action.component.css')]
})
export class ActionComponent {

    constructor(private readonly myElement: ElementRef,
                private readonly context: ContextService) {
    }

    private avm : ActionViewModel;

    @Input()
    set action(act: ActionViewModel) {
        this.avm = act;
        this.button = {
            value: this.friendlyName,
            doClick: () => this.doInvoke(),
            doRightClick: () => this.doInvoke(true),
            show: () => true,
            disabled: () => this.disabled(),
            title: () => this.description,
            accesskey: null
        }
    }

    get action(): ActionViewModel {
        return this.avm;
    }

    get description() {
        return this.action.description;
    }

    get friendlyName() {
        return this.action.title;
    }

    get contextClass() {
        if (this.isObjectContext()) {
            return "objectContext";
        }
        if (this.isCollectionContext()) {
            return "collectionContext";
        }
        if (this.isHomeContext()) {
            return "homeContext";
        }
        return "";
    }

    button: IButton;

    disabled() {
        return this.action.disabled() ? true : null;
    }

    // todo DRY this across actions and action 
    displayClass() {
        return ({
            tempdisabled: this.tempDisabled(),
            objectContext: this.isObjectContext(),
            collectionContext: this.isCollectionContext(),
            homeContext: this.isHomeContext()
        });
    }

    isObjectContext() {
        return this.action.actionRep.parent instanceof Models.DomainObjectRepresentation;
    }

    isCollectionContext() {
        return this.action.actionRep.parent instanceof Models.CollectionMember;
    }

    isHomeContext() {
        return this.action.actionRep.parent instanceof Models.MenuRepresentation;
    }

    tempDisabled(): boolean {
        return this.action.invokableActionRep &&
            this.action.invokableActionRep.isPotent() &&
            this.context.isPendingPotentActionOrReload(this.action.paneId);
    }

    doInvoke(right?: boolean) {
        if (this.action.disabled() || this.tempDisabled()) {
            return;
        }
        this.action.doInvoke(right);
    }

    focus() {
         this.myElement.nativeElement.children[0].focus();
    }
}