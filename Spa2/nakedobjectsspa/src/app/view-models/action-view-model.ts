import { ParameterViewModel } from './parameter-view-model';
import * as Models from '../models';
import * as Contextservice from '../context.service';
import * as Routedata from '../route-data';
import * as Urlmanagerservice from '../url-manager.service';
import * as Errorservice from '../error.service';
import * as _ from "lodash";
import * as Msg from "../user-messages";
import * as Imessageviewmodel from './imessage-view-model';
import * as Clickhandlerservice from '../click-handler.service';
import * as Viewmodelfactoryservice from '../view-model-factory.service';

export class ActionViewModel {

    constructor(
        private viewModelFactory: Viewmodelfactoryservice.ViewModelFactoryService,
        private context: Contextservice.ContextService,
        private urlManager: Urlmanagerservice.UrlManagerService,
        private error: Errorservice.ErrorService,
        private clickHandler: Clickhandlerservice.ClickHandlerService,
        public actionRep: Models.ActionMember | Models.ActionRepresentation,
        private vm: Imessageviewmodel.IMessageViewModel,
        private routeData: Routedata.PaneRouteData
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

    private paneId : number;
    invokableActionRep: Models.IInvokableAction;
    menuPath: string;
    title: string;
    description: string;
    presentationHint: string;

    // form actions should never show dialogs
    private showDialog = () => this.actionRep.extensions().hasParams() && (this.routeData.interactionMode !== Routedata.InteractionMode.Form);

    // open dialog on current pane always - invoke action goes to pane indicated by click
    doInvoke = this.showDialog()
        ? (right?: boolean) => {

            // clear any previous dialog so we don't pick up values from it
            this.context.clearDialogValues(this.paneId);
            this.urlManager.setDialog(this.actionRep.actionId(), this.paneId);
        }
        : (right?: boolean) => {
            const pps = this.parameters();
            this.execute(pps, right)
                .then((actionResult: Models.ActionResultRepresentation) => {
                    // if expect result and no warning from server generate one here
                    if (actionResult.shouldExpectResult() && !actionResult.warningsOrMessages()) {
                        this.context.broadcastWarning(Msg.noResultMessage);
                    }
                })
                .catch((reject: Models.ErrorWrapper) => {
                    const display = (em: Models.ErrorMap) => this.vm.setMessage(em.invalidReason() || em.warningMessage);
                    this.error.handleErrorAndDisplayMessages(reject, display);
                });
        };


    
    execute = (pps: ParameterViewModel[], right?: boolean): Promise<Models.ActionResultRepresentation> => {
        const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
        _.forEach(pps, p => this.urlManager.setParameterValue(this.actionRep.actionId(), p.parameterRep, p.getValue(), this.paneId));
        return this.context.getInvokableAction(this.actionRep)
            .then((details: Models.IInvokableAction) => this.context.invokeAction(details, parmMap, this.paneId, this.clickHandler.pane(this.paneId, right)));
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