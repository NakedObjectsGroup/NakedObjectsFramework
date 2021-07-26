import { AfterViewInit, Component, Input, OnDestroy, QueryList, ViewChildren, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import * as Ro from '@nakedobjects/restful-objects';
import { ContextService, ErrorService, ErrorWrapper } from '@nakedobjects/services';
import {
    ActionViewModel,
    CollectionViewModel,
    DialogViewModel,
    DomainObjectViewModel,
    ListViewModel,
    MenuViewModel,
    ParameterViewModel,
    ViewModelFactoryService
} from '@nakedobjects/view-models';
import { Dictionary } from 'lodash';
import find from 'lodash-es/find';
import forEach from 'lodash-es/forEach';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { createForm, safeUnsubscribe } from '../helpers-components';

export class BaseDialogComponent implements OnDestroy, OnChanges {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly error: ErrorService,
        private readonly context: ContextService,
        private readonly formBuilder: FormBuilder) {
    }

    private parentViewModel: MenuViewModel | DomainObjectViewModel | ListViewModel | CollectionViewModel;
    private parms: Dictionary<ParameterViewModel>;
    private formSub: ISubscription;
    protected sub: ISubscription;
    private createFormSub: ISubscription;

    @Input()
    set parent(parent: MenuViewModel | DomainObjectViewModel | ListViewModel | CollectionViewModel) {
        this.parentChanged = this.parentViewModel !== parent;
        this.parentViewModel = parent;
    }

    get parent(): MenuViewModel | DomainObjectViewModel | ListViewModel | CollectionViewModel {
        return this.parentViewModel;
    }

    private currentDialogId: string;
    private parentChanged: boolean;

    @Input()
    set selectedDialogId(id: string) {
        this.currentDialogId = id;
    }

    get selectedDialogId(): string {
        return this.currentDialogId;
    }

    dialog: DialogViewModel | null;

    form: FormGroup;

    get title() {
        const dialog = this.dialog;
        return dialog ? dialog.title : '';
    }

    get message() {
        const dialog = this.dialog;
        return dialog ? dialog.getMessage() : '';
    }

    get parameters() {
        const dialog = this.dialog;
        return dialog ? dialog.parameters : [];
    }

    get tooltip(): string {
        const dialog = this.dialog;
        return dialog ? dialog.tooltip() : '';
    }

    onSubmit(right?: boolean) {
        if (this.dialog) {
            forEach(this.parms,
                (p, _) => {
                    if (p.isEditable) {
                        const newValue = this.form.value[p.id];
                        p.setValueFromControl(newValue);
                    }
                });
            this.dialog.doInvoke(right);
        }
    }

    close = () => {
        if (this.dialog) {
            this.dialog.doCloseReplaceHistory();
            this.dialog = null;
        }
    }

    private createForm(dialog: DialogViewModel) {
        safeUnsubscribe(this.formSub);
        safeUnsubscribe(this.createFormSub);
        ({ form: this.form, dialog: this.dialog, parms: this.parms, sub: this.createFormSub } = createForm(dialog, this.formBuilder));
        this.formSub = this.form.valueChanges.subscribe((_) => this.onValueChanged());
    }

    onValueChanged() {
        if (this.dialog) {
            // clear messages if dialog changes
            this.dialog.resetMessage();
            this.context.clearMessages();
            this.context.clearWarnings();
        }
    }

    closeExistingDialog() {
        if (this.dialog) {
            if (this.dialog.id !== this.currentDialogId) {
                this.dialog.doCloseKeepHistory();
            } else {
                this.dialog.doCloseKeepUrl();
            }
            this.dialog = null;
        }
    }

    getDialog() {

        // if it's the same dialog just return

        if (this.parent && this.currentDialogId) {

            if (!this.parentChanged && this.dialog && this.dialog.id === this.currentDialogId) {
                return;
            }
            this.parentChanged = false;

            const p = this.parent;
            let action: Ro.ActionMember | Ro.ActionRepresentation | null = null;
            let actionViewModel: ActionViewModel | null = null;

            if (p instanceof MenuViewModel) {
                action = p.menuRep.actionMember(this.currentDialogId);
            }

            if (p instanceof DomainObjectViewModel && p.domainObject.hasActionMember(this.currentDialogId)) {
                action = p.domainObject.actionMember(this.currentDialogId);
            }

            if (p instanceof ListViewModel) {
                action = p.actionMember(this.currentDialogId)!;
                actionViewModel = find(p.actions, a => a.actionRep.actionId() === this.currentDialogId) || null;
            }

            if (p instanceof CollectionViewModel && p.hasMatchingLocallyContributedAction(this.currentDialogId)) {
                action = p.actionMember(this.currentDialogId)!;
                actionViewModel = find(p.actions, a => a.actionRep.actionId() === this.currentDialogId) || null;
            }

            if (action) {
                this.context.getInvokableAction(action)
                    .then(details => {
                        // only if we still have a dialog (may have beenn removed while getting invokable action)

                        if (this.currentDialogId) {
                            // must be a change
                            this.closeExistingDialog();
                            const dialogViewModel = this.viewModelFactory.dialogViewModel(this.parent.routeData, details, actionViewModel, false);
                            this.createForm(dialogViewModel);
                        }
                    })
                    .catch((reject: ErrorWrapper) => {
                        this.error.handleError(reject);
                    });
            } else {
                this.closeExistingDialog();
            }

        } else {
            this.closeExistingDialog();
        }
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.createFormSub);
        safeUnsubscribe(this.formSub);
        safeUnsubscribe(this.sub);
        this.closeExistingDialog();
    }

    ngOnChanges(): void {
        this.getDialog();
    }
}
