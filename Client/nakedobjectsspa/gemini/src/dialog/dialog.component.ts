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
import some from 'lodash-es/some';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { createForm, safeUnsubscribe } from '../helpers-components';
import { ParametersComponent } from '../parameters/parameters.component';

@Component({
    selector: 'nof-dialog',
    templateUrl: 'dialog.component.html',
    styleUrls: ['dialog.component.css']
})
export class DialogComponent implements AfterViewInit, OnDestroy, OnChanges {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly error: ErrorService,
        private readonly context: ContextService,
        private readonly formBuilder: FormBuilder) {
    }

    private parentViewModel: MenuViewModel | DomainObjectViewModel | ListViewModel | CollectionViewModel;
    private parms: Dictionary<ParameterViewModel>;
    private formSub: ISubscription;
    private sub: ISubscription;
    private createFormSub: ISubscription;

    @Input()
    set parent(parent: MenuViewModel | DomainObjectViewModel | ListViewModel | CollectionViewModel) {
        this.parentViewModel = parent;
    }

    get parent(): MenuViewModel | DomainObjectViewModel | ListViewModel | CollectionViewModel {
        return this.parentViewModel;
    }

    private currentDialogId: string;

    @Input()
    set selectedDialogId(id: string) {
        this.currentDialogId = id;
    }

    get selectedDialogId(): string {
        return this.currentDialogId;
    }

    dialog: DialogViewModel | null;

    form: FormGroup;

    @ViewChildren(ParametersComponent)
    parmComponents: QueryList<ParametersComponent>;

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
        return dialog ? dialog.parameters : '';
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
            this.dialog.doCloseKeepHistory();
            this.dialog = null;
        }
    }

    getDialog() {

        // if it's the same dialog just return

        if (this.parent && this.currentDialogId) {

            if (this.dialog && this.dialog.id === this.currentDialogId) {
                return;
            }

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
                    .catch((reject: ErrorWrapper) => this.error.handleError(reject));
            } else {
                this.closeExistingDialog();
            }

        } else {
            this.closeExistingDialog();
        }
    }

    focus(parms: QueryList<ParametersComponent>) {
        if (parms && parms.length > 0) {
            some(parms.toArray(), p => p.focus());
        }
    }

    ngAfterViewInit(): void {
        this.sub = this.parmComponents.changes.subscribe(ql => this.focus(ql));
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.createFormSub);
        safeUnsubscribe(this.formSub);
        safeUnsubscribe(this.sub);
    }

    ngOnChanges(): void {
        this.getDialog();
    }
}
