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

@Component({
    selector: 'app-dialog',
    templateUrl: './dialog.component.html',
    styleUrls: ['./dialog.component.css']
})
export class DialogComponent implements OnInit, OnDestroy {

    constructor(
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private activatedRoute: ActivatedRoute,
        private error: ErrorService,
        private context: ContextService,
        private formBuilder: FormBuilder) {
    }

    @Input()
    parent: MenuViewModel | DomainObjectViewModel | ListViewModel;

    dialog: DialogViewModel;

    form: FormGroup;

    get title() {
        return this.dialog.title;
    }

    get message() {
        return this.dialog.getMessage();
    }

    get parameters() {
        return this.dialog.parameters;
    }

    get tooltip(): string {
        return this.dialog.tooltip();
    }

    paneId: number;

    onSubmit(right?: boolean) {
        _.forEach(this.parms,
            (p, k) => {
                const newValue = this.form.value[p.id];
                p.setValueFromControl(newValue);
            });
        this.dialog.doInvoke(right);
    }


    close = () => this.dialog.doCloseReplaceHistory();

    private currentDialogId: string;

    private parms: _.Dictionary<ParameterViewModel>;

    private createForm(dialog: DialogViewModel) {
        const pps = dialog.parameters;
        this.parms = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p)) as _.Dictionary<ParameterViewModel>;
        // todo fix types - no any 
        const controls = _.mapValues(this.parms, p => [p.getValueForControl(), a => p.validator(a)]) as _.Dictionary<any>;
        this.form = this.formBuilder.group(controls);

        this.form.valueChanges.subscribe((data: any) => {
            // cache parm values
            _.forEach(data,
                (v, k) => {
                    const parm = this.parms[k];
                    parm.setValueFromControl(v);
                });
            this.dialog.setParms();
        });
    }


    getDialog(routeData: PaneRouteData) {

        if (this.parent && this.currentDialogId) {
            const p = this.parent;
            let action: Models.ActionMember | Models.ActionRepresentation = null;
            let actionViewModel: ActionViewModel = null;

            if (p instanceof MenuViewModel) {
                action = p.menuRep.actionMember(this.currentDialogId);
            }

            if (p instanceof DomainObjectViewModel) {
                action = p.domainObject.actionMember(this.currentDialogId);
            }

            if (p instanceof ListViewModel) {
                action = p.actionMember(this.currentDialogId);
                actionViewModel = _.find(p.actions, a => a.actionRep.actionId() === this.currentDialogId);
            }

            this.context.getInvokableAction(action)
                .then(details => {
                    // only if we still have a dialog (may have beenn removed while getting invokable action)
                    if (this.currentDialogId) {

                        // todo fix this it's clunky
                        this.context.clearParmUpdater(routeData.paneId);
        
                        const dialogViewModel = this.viewModelFactory.dialogViewModel(routeData, details, actionViewModel, false);
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
   
    private routeDataMatchesParent(rd: PaneRouteData) {
        if (this.parent instanceof MenuViewModel) {
            return rd.location === ViewType.Home;
        }

        if (this.parent instanceof DomainObjectViewModel) {
            return rd.location === ViewType.Object;
        }

        if (this.parent instanceof ListViewModel) {
            return rd.location === ViewType.List;
        }
        return false;
    }


    ngOnInit(): void {

        this.activatedRouteDataSub = this.activatedRoute.data.subscribe((data: any) => {
            this.paneId = data["pane"];
        });

        this.paneRouteDataSub = this.urlManager.getRouteDataObservable()
            .subscribe((rd: RouteData) => {
                if (this.paneId) {
                    const paneRouteData = rd.pane()[this.paneId];
                    // check that the paneRouteData is teh same as the parent otherwise 
                    // we may get route data for a different page 
                    if (this.routeDataMatchesParent(paneRouteData)) {
                        this.currentDialogId = paneRouteData.dialogId;
                        this.getDialog(paneRouteData);
                    }
                }
            });
    }

    ngOnDestroy(): void {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    }
}