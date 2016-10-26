import { Component, Input, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ViewModelFactoryService } from "../view-model-factory.service";
import { UrlManagerService } from "../url-manager.service";
import * as _ from "lodash";
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { ActivatedRoute, Router, Data } from '@angular/router';
import "../rxjs-extensions";
import { Subject } from 'rxjs/Subject';
import { PaneRouteData, RouteData } from '../route-data';
import { ISubscription } from 'rxjs/Subscription';
import { ContextService } from '../context.service';
import { ColorService } from '../color.service';
import { FocusManagerService } from '../focus-manager.service';
import { ErrorService } from '../error.service';
import { FormBuilder, FormGroup, FormControl, AbstractControl } from '@angular/forms';

@Component({
    selector: 'app-dialog',
    templateUrl: './dialog.component.html',
    styleUrls: ['./dialog.component.css']
})
export class DialogComponent implements OnInit, OnDestroy {

    constructor(private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private activatedRoute: ActivatedRoute,
        private error: ErrorService,
        private color: ColorService,
        private focusManager: FocusManagerService,
        private context: ContextService,
        private changeDetectorRef: ChangeDetectorRef,
        private formBuilder: FormBuilder) { }


    paneId: number;

    @Input()
    parent: ViewModels.MenuViewModel | ViewModels.DomainObjectViewModel | ViewModels.ListViewModel;

    dialog: ViewModels.DialogViewModel;

    form: FormGroup;


    private currentDialogId: string;

    private parms: _.Dictionary<ViewModels.ParameterViewModel>;

    private createForm(dialog: ViewModels.DialogViewModel) {
        const pps = dialog.parameters;
        this.parms = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p)) as _.Dictionary<ViewModels.ParameterViewModel>

        const controls = _.mapValues(this.parms, p => [p.getValueForControl(), a => p.validator(a)]) as _.Dictionary<any>;
        this.form = this.formBuilder.group(controls);

        this.form.valueChanges.subscribe(data => {
            // cache parm values
            _.forEach(data, (v, k) => {
                const parm = this.parms[k];
                parm.setValueFromControl(v);
            });
            this.dialog.setParms();
        });
        // this.onValueChanged(); // (re)set validation messages now
    }


    getDialog(routeData: PaneRouteData) {

        if (this.parent && this.currentDialogId) {
            const p = this.parent;
            let action: Models.ActionMember | Models.ActionRepresentation = null;
            let actionViewModel: ViewModels.ActionViewModel = null;

            if (p instanceof ViewModels.MenuViewModel) {
                action = p.menuRep.actionMember(this.currentDialogId);
            }

            if (p instanceof ViewModels.DomainObjectViewModel) {
                action = p.domainObject.actionMember(this.currentDialogId);
            }

            if (p instanceof ViewModels.ListViewModel) {
                action = p.actionMember(this.currentDialogId);
                actionViewModel = _.find(p.actions, a => a.actionRep.actionId() === this.currentDialogId);
            }

            this.context.getInvokableAction(action)
                .then(details => {
                    // only if we still have a dialog (may have beenn removed while getting invokable action)
                    if (this.currentDialogId) {

                        //if (actionViewModel) {
                        //    actionViewModel.makeInvokable(details);
                        //}
                        //setDialog($scope, actionViewModel || details, routeData);
                        //this.focusManager.focusOn(Focusmanagerservice.FocusTarget.Dialog, 0, routeData.paneId);

                        this.context.clearParmUpdater(routeData.paneId);

                        //$scope.dialogTemplate = dialogTemplate;
                        const dialogViewModel = new ViewModels.DialogViewModel(this.color,
                            this.context,
                            this.viewModelFactory,
                            this.urlManager,
                            this.focusManager,
                            this.error);
                        //const isAlreadyViewModel = action instanceof Nakedobjectsviewmodels.ActionViewModel;
                        actionViewModel = actionViewModel ||
                            this.viewModelFactory.actionViewModel(action as Models.ActionMember | Models.ActionRepresentation,
                                dialogViewModel,
                                routeData);
                        //: action as Nakedobjectsviewmodels.IActionViewModel;

                        actionViewModel.makeInvokable(details);
                        dialogViewModel.reset(actionViewModel, routeData);
                        //$scope.dialog = dialogViewModel;

                        this.context.setParmUpdater(dialogViewModel.setParms, routeData.paneId);
                        dialogViewModel.deregister = () => this.context.clearParmUpdater(routeData.paneId);

                        this.createForm(dialogViewModel);

                        this.dialog = dialogViewModel;
                    }
                })
                .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        } else {
            this.dialog = null;
        }
    }

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;
    private dialogSub: ISubscription;


    ngOnInit(): void {

        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(data => {

            this.paneId = data["pane"];

            this.paneRouteDataSub = this.urlManager.getRouteDataObservable()
                .subscribe((rd: RouteData) => {
                    const paneRouteData = rd.pane()[this.paneId];
                    this.currentDialogId = paneRouteData.dialogId;
                    this.getDialog(paneRouteData);
                });
        });
     
    }

    ngOnDestroy(): void {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }

        if (this.dialogSub) {
            this.dialogSub.unsubscribe();
        }

    }

    get tooltip(): string {
        return this.dialog.tooltip();
    }

    onSubmit() {
        _.forEach(this.parms, (p, k) => {
            const newValue = this.form.value[p.id];
            p.setValueFromControl(newValue);
        });
        this.dialog.doInvoke();
    }

}
