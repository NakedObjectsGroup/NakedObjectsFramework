import { MessageViewModel } from './message-view-model';
import { ColorService } from '../color.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { ActionViewModel } from './action-view-model';
import { ParameterViewModel } from './parameter-view-model';
import { PaneRouteData, Pane } from '../route-data';
import * as Models from '../models';
import * as Msg from '../user-messages';
import * as Helpers from './helpers-view-models';
import each from 'lodash-es/each';
import every from 'lodash-es/every';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import { Dictionary } from 'lodash';
import pickBy from 'lodash-es/pickBy';

export class DialogViewModel extends MessageViewModel {
    constructor(
        private readonly color: ColorService,
        private readonly context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly urlManager: UrlManagerService,
        private readonly error: ErrorService,
        private readonly routeData: PaneRouteData,
        action: Models.ActionRepresentation | Models.InvokableActionMember,
        actionViewModel: ActionViewModel | null,
        public readonly isMultiLineDialogRow: boolean,
        row?: number
    ) {
        super();

        this.actionViewModel = actionViewModel || this.viewModelFactory.actionViewModel(action, this, routeData);

        this.onPaneId = routeData.paneId;

        const fields = this.context.getDialogCachedValues(this.actionMember().actionId(), this.onPaneId);

        const parameters = pickBy(this.actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as Dictionary<Models.Parameter>;
        this.parameters = map(parameters, p => this.viewModelFactory.parameterViewModel(p, fields[p.id()], this.onPaneId));

        this.title = this.actionMember().extensions().friendlyName();
        this.isQueryOnly = this.actionViewModel.invokableActionRep.isQueryOnly();
        this.resetMessage();
        this.id = this.actionViewModel.actionRep.actionId();

        this.incrementPendingPotentAction();

        if (this.isMultiLineDialogRow) {
            this.actionViewModel.gotoResult = false;
            this.parameters.forEach(p => p.setAsRow(row!));
        }
    }

    private readonly onPaneId: Pane;
    private readonly isQueryOnly: boolean;
    readonly title: string;
    readonly id: string;
    readonly parameters: ParameterViewModel[];
    submitted = false;
    closed = false; // make sure we never close more than once

    public readonly actionViewModel: ActionViewModel;

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

    private readonly actionMember = () => this.actionViewModel.actionRep;

    private readonly execute = (right?: boolean) => {
        const pps = this.parameters;
        return this.actionViewModel.execute(pps, right);
    }

    readonly refresh = () => {
        const fields = this.context.getDialogCachedValues(this.actionMember().actionId(), this.onPaneId);
        forEach(this.parameters, p => p.refresh(fields[p.id]));
    }

    readonly clientValid = () => every(this.parameters, p => p.clientValid);

    readonly tooltip = () => Helpers.tooltip(this, this.parameters);

    readonly setParms = () => forEach(this.parameters, p => this.context.cacheFieldValue(this.actionMember().actionId(), p.parameterRep.id(), p.getValue(), this.onPaneId));

    readonly doInvoke = (right?: boolean) =>
        this.execute(right)
            .then((actionResult: Models.ActionResultRepresentation) => {
                if (actionResult.shouldExpectResult()) {
                    this.setMessage(actionResult.warningsOrMessages() || Msg.noResultMessage);
                } else if (actionResult.resultType() === "void") {
                    // dialog staying on same page so treat as cancel
                    // for url replacing purposes and have to explicitly close
                    this.doCloseReplaceHistory();
                } else if (!this.isQueryOnly) {
                    // not query only and going to new page mark as complete
                    // and have to explicitly close
                    this.doCloseKeepHistory();
                } else {
                    // query only - if on same pane will be replaced if on other pane
                    // will stay open
                    this.doComplete();
                }
            })
            .catch((reject: Models.ErrorWrapper) => {
                const display = (em: Models.ErrorMap) => Helpers.handleErrorResponse(em, this, this.parameters);
                this.error.handleErrorAndDisplayMessages(reject, display);
            })

    private submit() {
        if (this.isMultiLineDialogRow) {
            this.submitted = true;
        }
    }

    doCloseKeepHistory = () => {
        this.urlManager.closeDialogKeepHistory(this.id, this.onPaneId);
        this.doComplete();
    }

    doCloseReplaceHistory = () => {
        this.urlManager.closeDialogReplaceHistory(this.id, this.onPaneId);
        this.doComplete();
    }

    private doComplete() {
        this.submit();
        this.decrementPendingPotentAction();
    }

    clearMessages = () => {
        this.resetMessage();
        each(this.actionViewModel.parameters, parm => parm.clearMessage());
    }
}
