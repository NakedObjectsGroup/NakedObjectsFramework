import * as Messageviewmodel from './message-view-model';
import * as Colorservice from '../color.service';
import * as Contextservice from '../context.service';
import * as Viewmodelfactoryservice from '../view-model-factory.service';
import * as Urlmanagerservice from '../url-manager.service';
import * as Errorservice from '../error.service';
import * as Actionviewmodel from './action-view-model';
import * as Parameterviewmodel from './parameter-view-model';
import * as Routedata from '../route-data';
import * as Models from '../models';
import * as Usermessages from '../user-messages';
import { Subject } from 'rxjs/Subject';
import * as Helpersviewmodels from './helpers-view-models';
import * as _ from "lodash";
import { ISubscription } from 'rxjs/Subscription';

export class DialogViewModel extends Messageviewmodel.MessageViewModel {
    constructor(private color: Colorservice.ColorService,
        private context: Contextservice.ContextService,
        private viewModelFactory: Viewmodelfactoryservice.ViewModelFactoryService,
        private urlManager: Urlmanagerservice.UrlManagerService,
        private error: Errorservice.ErrorService) {
        super();
    }

    private onPaneId: number;
    private isQueryOnly: boolean;

    private actionMember = () => this.actionViewModel.actionRep;

    private execute = (right?: boolean) => {

        const pps = this.parameters;
        this.context.updateValues();
        return this.actionViewModel.execute(pps, right);
    };

    actionViewModel: Actionviewmodel.ActionViewModel;
    title: string;
    id: string;
    parameters: Parameterviewmodel.ParameterViewModel[];

    reset(actionViewModel: Actionviewmodel.ActionViewModel, routeData: Routedata.PaneRouteData) {
        this.actionViewModel = actionViewModel;
        this.onPaneId = routeData.paneId;

        const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);

        const parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Models.Parameter>;
        this.parameters = _.map(parameters, p => this.viewModelFactory.parameterViewModel(p, fields[p.id()], this.onPaneId));
        this.listenToParameters();

        this.title = this.actionMember().extensions().friendlyName();
        this.isQueryOnly = actionViewModel.invokableActionRep.invokeLink().method() === "GET";
        this.resetMessage();
        this.id = actionViewModel.actionRep.actionId();
    }

    refresh() {
        const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);
        _.forEach(this.parameters, p => p.refresh(fields[p.id]));
    }

    deregister: () => void;

    clientValid = () => _.every(this.parameters, p => p.clientValid);

    tooltip = () => Helpersviewmodels.tooltip(this, this.parameters);

    setParms = () => _.forEach(this.parameters, p => this.context.setFieldValue(this.actionMember().actionId(), p.parameterRep.id(), p.getValue(), this.onPaneId));


    doInvoke = (right?: boolean) =>
        this.execute(right).
            then((actionResult: Models.ActionResultRepresentation) => {
                if (actionResult.shouldExpectResult()) {
                    this.setMessage(actionResult.warningsOrMessages() || Usermessages.noResultMessage);
                } else if (actionResult.resultType() === "void") {
                    // dialog staying on same page so treat as cancel 
                    // for url replacing purposes
                    this.doCloseReplaceHistory();
                }
                else if (!this.isQueryOnly) {
                    // not query only - always close
                    //this.doCloseReplaceHistory();
                }
                else if (!right) {
                    // query only going to new page close dialog and keep history
                    //this.doCloseKeepHistory();
                }
                // else query only going to other tab leave dialog open
            }).
            catch((reject: Models.ErrorWrapper) => {
                const display = (em: Models.ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.parameters);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });

    doCloseKeepHistory = () => {
        //this.deregister();
        this.urlManager.closeDialogKeepHistory(this.onPaneId);
    }

    doCloseReplaceHistory = () => {
        //this.deregister();
        this.urlManager.closeDialogReplaceHistory(this.onPaneId);
    }

    clearMessages = () => {
        this.resetMessage();
        _.each(this.actionViewModel.parameters, parm => parm.clearMessage());
    };

    parameterChanged() {
        this.parameterChangedSource.next(true);
        this.parameterChangedSource.next(false);
    }

    private parameterChangedSource = new Subject<boolean>();

    parameterChanged$ = this.parameterChangedSource.asObservable();

    private validChangedSource = new Subject<boolean>();

    validChanged$ = this.validChangedSource.asObservable();

    private parmSubs: ISubscription[] = [];

    private listenToParameters() {
        _.forEach(this.parameters, p => {
            this.parmSubs.push(p.validChanged$.subscribe(changed => {
                if (changed) {
                    this.validChangedSource.next(true);
                    this.validChangedSource.next(false);
                }
            }));
        });
    }

}