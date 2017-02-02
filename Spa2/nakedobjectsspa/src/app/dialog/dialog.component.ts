import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { ViewModelFactoryService } from "../view-model-factory.service";
import { UrlManagerService } from "../url-manager.service";
import * as _ from "lodash";
import * as Models from "../models";
import { ActivatedRoute, Data } from '@angular/router';
import "../rxjs-extensions";
import { PaneRouteData, RouteData, ViewType } from '../route-data';
import { ISubscription } from 'rxjs/Subscription';
import { ContextService } from '../context.service';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { FormBuilder, FormGroup, FormControl, AbstractControl } from '@angular/forms';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { ActionViewModel } from '../view-models/action-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import { ListViewModel } from '../view-models/list-view-model';
import { MenuViewModel } from '../view-models/menu-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { CollectionViewModel } from '../view-models/collection-view-model';
import * as Configservice from '../config.service';

@Component({
    selector: 'nof-dialog',
    templateUrl: './dialog.component.html',
    styleUrls: ['./dialog.component.css']
})
export class DialogComponent {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly urlManager: UrlManagerService,
        private readonly activatedRoute: ActivatedRoute,
        private readonly error: ErrorService,
        private readonly context: ContextService,
        private readonly configService: Configservice.ConfigService,
        private readonly formBuilder: FormBuilder) {
    }

    private parentViewModel: MenuViewModel | DomainObjectViewModel | ListViewModel | CollectionViewModel;

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
        this.getDialog();
    }

    get selectedDialogId(): string {
        return this.currentDialogId;
    }

    dialog: DialogViewModel | null;

    form: FormGroup;

    get title() {
        const dialog = this.dialog;
        return dialog ? dialog.title : "";
    }

    get message() {
        const dialog = this.dialog;
        return dialog ? dialog.getMessage() : "";
    }

    get parameters() {
        const dialog = this.dialog;
        return dialog ? dialog.parameters : "";
    }

    get tooltip(): string {
        const dialog = this.dialog;
        return dialog ? dialog.tooltip() : "";
    }

    onSubmit(right?: boolean) {
        if (this.dialog) {
            _.forEach(this.parms,
                (p, k) => {
                    const newValue = this.form.value[p.id];
                    p.setValueFromControl(newValue);
                });
            this.dialog.doInvoke(right);
        }
    }

    close = () => {
        if (this.dialog) {
            this.dialog.doCloseReplaceHistory();
        }
    };

    private parms: _.Dictionary<ParameterViewModel>;

    private createForm(dialog: DialogViewModel) {
        const pps = dialog.parameters;
        this.parms = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p)) as _.Dictionary<ParameterViewModel>;
        const controls = _.mapValues(this.parms, p => [p.getValueForControl(), (a: AbstractControl) => p.validator(a)]);
        this.form = this.formBuilder.group(controls);

        this.form.valueChanges.subscribe((data: any) => {
            if (this.dialog) {
                // cache parm values
                _.forEach(data, (v, k) => this.parms[k!].setValueFromControl(v));
                this.dialog.setParms();
            }
        });
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
            let action: Models.ActionMember | Models.ActionRepresentation | null = null;
            let actionViewModel: ActionViewModel | null = null;

            if (p instanceof MenuViewModel) {
                action = p.menuRep.actionMember(this.currentDialogId, this.configService.config.keySeparator);
            }

            if (p instanceof DomainObjectViewModel && p.domainObject.hasActionMember(this.currentDialogId)) {
                action = p.domainObject.actionMember(this.currentDialogId, this.configService.config.keySeparator);
            }

            if (p instanceof ListViewModel) {
                action = p.actionMember(this.currentDialogId);
                actionViewModel = _.find(p.actions, a => a.actionRep.actionId() === this.currentDialogId) || null;
            }

            if (p instanceof CollectionViewModel && p.hasMatchingLocallyContributedAction(this.currentDialogId)) {
                action = p.actionMember(this.currentDialogId, this.configService.config.keySeparator);
                actionViewModel = _.find(p.actions, a => a.actionRep.actionId() === this.currentDialogId) || null;
            }

            if (action) {
                this.context.getInvokableAction(action)
                    .then(details => {
                        // only if we still have a dialog (may have beenn removed while getting invokable action)

                        if (this.currentDialogId) {

                            const dialogViewModel = this.viewModelFactory.dialogViewModel(this.parent.routeData, details, actionViewModel, false);
                            this.createForm(dialogViewModel);

                            // must be a change 
                            this.closeExistingDialog();
                            this.dialog = dialogViewModel;
                        }
                    })
                    .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            } else {
                this.closeExistingDialog();
            }

        } else {
            this.closeExistingDialog();
        }
    }
}