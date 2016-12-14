import { MessageViewModel } from './message-view-model';
import { ColorService } from '../color.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { ActionViewModel } from './action-view-model';
import { ParameterViewModel } from './parameter-view-model';
import { PaneRouteData } from '../route-data';
import * as Models from '../models';
import * as Msg from '../user-messages';
import * as Helpers from './helpers-view-models';
import * as _ from "lodash";

export class DialogViewModel extends MessageViewModel {
    constructor(
        private color: ColorService,
        private context: ContextService,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private error: ErrorService,
        private routeData: PaneRouteData,
        action: Models.IInvokableAction,
        public actionViewModel: ActionViewModel,
        public isMultiLineDialogRow: boolean
    ) {
        super();

        // todo not happy with the whole invokable action thing here casting is horrid.
        this.actionViewModel = actionViewModel ||
            this.viewModelFactory.actionViewModel(action as Models.ActionMember | Models.ActionRepresentation,
                this,
                routeData);

        this.actionViewModel.makeInvokable(action);

        this.onPaneId = routeData.paneId;

        const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);

        const parameters = _.pickBy(this.actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Models.Parameter>;
        this.parameters = _.map(parameters, p => this.viewModelFactory.parameterViewModel(p, fields[p.id()], this.onPaneId));


        this.title = this.actionMember().extensions().friendlyName();
        this.isQueryOnly = this.actionViewModel.invokableActionRep.isQueryOnly();
        this.resetMessage();
        this.id = this.actionViewModel.actionRep.actionId();

        // todo use subscribe ? 
        this.context.setParmUpdater(this.setParms, routeData.paneId);

        this.incrementPendingPotentAction();
    }

    private incrementPendingPotentAction() {
        if (!this.isMultiLineDialogRow) {
            Helpers.incrementPendingPotentAction(this.context, this.actionViewModel.invokableActionRep, this.onPaneId);
        }
    }

    private decrementPendingPotentAction() {
        if (!this.isMultiLineDialogRow && !this.closed) {
            Helpers.decrementPendingPotentAction(this.context, this.actionViewModel.invokableActionRep, this.onPaneId);
        }
        this.closed = true;
    }

    private onPaneId: number;
    private isQueryOnly: boolean;

    private actionMember = () => this.actionViewModel.actionRep;

    private execute = (right?: boolean) => {

        const pps = this.parameters;
        this.context.updateValues();
        return this.actionViewModel.execute(pps, right);
    };

    title: string;
    id: string;
    parameters: ParameterViewModel[];
    submitted = false;
    closed = false; // make sure we never close more than once 

    refresh() {
        const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);
        _.forEach(this.parameters, p => p.refresh(fields[p.id]));
    }

    deregister = () => this.context.clearParmUpdater(this.onPaneId);

    clientValid = () => _.every(this.parameters, p => p.clientValid);

    tooltip = () => Helpers.tooltip(this, this.parameters);

    setParms = () => _.forEach(this.parameters, p => this.context.setFieldValue(this.actionMember().actionId(), p.parameterRep.id(), p.getValue(), this.onPaneId));

    doInvoke = (right?: boolean) =>
        this.execute(right)
            .then((actionResult: Models.ActionResultRepresentation) => {
                if (actionResult.shouldExpectResult()) {
                    this.setMessage(actionResult.warningsOrMessages() || Msg.noResultMessage);
                } else if (actionResult.resultType() === "void") {
                    // dialog staying on same page so treat as cancel 
                    // for url replacing purposes
                    this.doCloseReplaceHistory();

                } else if (!this.isQueryOnly) {
                    // not query only - always close

                    this.doClose();
                } else if (!right) {
                    // query only going to new page close dialog and keep history

                    this.doClose();
                }
                // else query only going to other tab leave dialog open
                this.doCompleteButLeaveOpen();
            })
            .catch((reject: Models.ErrorWrapper) => {
                const display = (em: Models.ErrorMap) => Helpers.handleErrorResponse(em, this, this.parameters);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });

    // todo tidy and rework these getting confusing 
    doCloseKeepHistory = () => {

        this.deregister();
        this.urlManager.closeDialogKeepHistory(this.id, this.onPaneId);
        this.decrementPendingPotentAction();
    };

    doCloseReplaceHistory = () => {
        this.deregister();
        this.urlManager.closeDialogReplaceHistory(this.id, this.onPaneId);
        this.decrementPendingPotentAction();
    };

    doClose() {
        this.decrementPendingPotentAction();
    }

    doCompleteButLeaveOpen = () => {
    }

    clearMessages = () => {
        this.resetMessage();
        _.each(this.actionViewModel.parameters, parm => parm.clearMessage());
    };
}