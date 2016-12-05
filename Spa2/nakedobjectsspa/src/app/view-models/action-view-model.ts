import { ParameterViewModel } from './parameter-view-model';
import { ContextService } from '../context.service';
import { PaneRouteData, InteractionMode } from '../route-data';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { IMessageViewModel } from './imessage-view-model';
import { ClickHandlerService } from '../click-handler.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import * as Models from '../models';
import * as Msg from "../user-messages";
import * as _ from "lodash";

export class ActionViewModel {

    constructor(
        private viewModelFactory: ViewModelFactoryService,
        private context: ContextService,
        private urlManager: UrlManagerService,
        private error: ErrorService,
        private clickHandler: ClickHandlerService,
        public actionRep: Models.ActionMember | Models.ActionRepresentation,
        private vm: IMessageViewModel,
        private routeData: PaneRouteData
    ) {

        if (actionRep instanceof Models.ActionRepresentation || actionRep instanceof Models.InvokableActionMember) {
            this.invokableActionRep = actionRep;
        }

        this.paneId = routeData.paneId;
        this.title = actionRep.extensions().friendlyName();
        this.presentationHint = actionRep.extensions().presentationHint();
        this.menuPath = actionRep.extensions().menuPath() || "";
        this.description = this.disabled() ? actionRep.disabledReason() : actionRep.extensions().description();
    }

    paneId: number;
    invokableActionRep: Models.IInvokableAction;
    menuPath: string;
    title: string;
    description: string;
    presentationHint: string;
    gotoResult = true;

    // form actions should never show dialogs
    private showDialog = () => this.actionRep.extensions().hasParams() && (this.routeData.interactionMode !== InteractionMode.Form);

    private incrementPendingPotentAction() {
        if (this.invokableActionRep.isPotent()) {
            this.context.incrementPendingPotentAction(this.paneId);
        }
    }

    private decrementPendingPotentAction() {
        if (this.invokableActionRep.isPotent()) {
            this.context.decrementPendingPotentAction(this.paneId);
        }
    }

    // open dialog on current pane always - invoke action goes to pane indicated by click
    doInvoke = this.showDialog()
        ? (right?: boolean) => {

            // clear any previous dialog so we don't pick up values from it
            this.context.clearDialogValues(this.paneId);
            this.urlManager.setDialogOrMultiLineDialog(this.actionRep, this.paneId);    
        }
        : (right?: boolean) => {
            const pps = this.parameters();
            this.incrementPendingPotentAction();
            this.execute(pps, right)
                .then((actionResult: Models.ActionResultRepresentation) => {
                    this.decrementPendingPotentAction();
                    // if expect result and no warning from server generate one here
                    if (actionResult.shouldExpectResult() && !actionResult.warningsOrMessages()) {
                        this.context.broadcastWarning(Msg.noResultMessage);
                    }
                })
                .catch((reject: Models.ErrorWrapper) => {
                    this.decrementPendingPotentAction();
                    const display = (em: Models.ErrorMap) => this.vm.setMessage(em.invalidReason() || em.warningMessage);
                    this.error.handleErrorAndDisplayMessages(reject, display);
                });
        };



    execute = (pps: ParameterViewModel[], right?: boolean): Promise<Models.ActionResultRepresentation> => {
        const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
        _.forEach(pps, p => this.urlManager.setParameterValue(this.actionRep.actionId(), p.parameterRep, p.getValue(), this.paneId));
        // todo is this necessary - should we always be invokable by now ?
        return this.context.getInvokableAction(this.actionRep)
            .then((details: Models.IInvokableAction) => this.context.invokeAction(details, parmMap, this.paneId, this.clickHandler.pane(this.paneId, right), this.gotoResult));
    };


    disabled = () => !!this.actionRep.disabledReason();

    parameters = () => {
        // don't use actionRep directly as it may change and we've closed around the original value
        const parameters = _.pickBy(this.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Models.Parameter>;
        const parms = this.routeData.actionParams;
        return _.map(parameters, parm => this.viewModelFactory.parameterViewModel(parm, parms[parm.id()], this.paneId));
    };

    makeInvokable = (details: Models.IInvokableAction) => this.invokableActionRep = details;
}