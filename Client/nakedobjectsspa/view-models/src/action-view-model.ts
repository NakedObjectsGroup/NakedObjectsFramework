import * as Ro from '@nakedobjects/restful-objects';
import {
    ClickHandlerService,
    ContextService,
    ErrorService,
    ErrorWrapper,
    InteractionMode,
    Pane,
    PaneRouteData,
    UrlManagerService
} from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import pickBy from 'lodash-es/pickBy';
import zipObject from 'lodash-es/zipObject';
import * as Helpers from './helpers-view-models';
import { IMessageViewModel } from './imessage-view-model';
import { ParameterViewModel } from './parameter-view-model';
import * as Msg from './user-messages';
import { ViewModelFactoryService } from './view-model-factory.service';

export class ActionViewModel {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly context: ContextService,
        private readonly urlManager: UrlManagerService,
        private readonly error: ErrorService,
        private readonly clickHandler: ClickHandlerService,
        public readonly actionRep: Ro.ActionMember | Ro.ActionRepresentation,
        private readonly vm: IMessageViewModel,
        private readonly routeData: PaneRouteData
    ) {

        if (actionRep instanceof Ro.ActionRepresentation || actionRep instanceof Ro.InvokableActionMember) {
            this.invokableActionRep = actionRep;
        }

        this.paneId = routeData.paneId;
        this.title = actionRep.extensions().friendlyName();
        this.presentationHint = actionRep.extensions().presentationHint();
        this.menuPath = actionRep.extensions().menuPath() || '';
        this.description = this.disabled() ? actionRep.disabledReason() : actionRep.extensions().description();
        this.editProperties = actionRep.extensions().editProperties()?.split(',') ?? [];
        this.createNewProperties = actionRep.extensions().createNewProperties()?.split(',') ?? [];
    }

    readonly paneId: Pane;
    readonly menuPath: string;
    readonly title: string;
    readonly description: string;
    readonly presentationHint: string;
    readonly editProperties: string[];
    readonly createNewProperties: string[];
    gotoResult = true;
    invokableActionRep: Ro.ActionRepresentation | Ro.InvokableActionMember;

    // form actions should never show dialogs
    readonly showDialog = () => this.actionRep.extensions().hasParams() && (this.routeData.interactionMode !== InteractionMode.Form);

    private readonly incrementPendingPotentAction = () => {
        Helpers.incrementPendingPotentAction(this.context, this.invokableActionRep, this.paneId);
    }

    private readonly decrementPendingPotentAction = () => {
        Helpers.decrementPendingPotentAction(this.context, this.invokableActionRep, this.paneId);
    }

    readonly invokeWithDialog = (right?: boolean) => {
        // clear any previous dialog so we don't pick up values from it
        this.context.clearDialogCachedValues(this.paneId);
        this.urlManager.setDialogOrMultiLineDialog(this.actionRep, this.paneId);
    }

    readonly invokeWithoutDialogWithParameters = (parameters: Promise<ParameterViewModel[]>, right?: boolean) => {
        return parameters
            .then((pps: ParameterViewModel[]) => {
                return this.execute(pps, right);
            });
    }

    private readonly invokeWithoutDialog = (right?: boolean) => {
        this.incrementPendingPotentAction();
        this.invokeWithoutDialogWithParameters(this.parameters(), right)
            .then((actionResult: Ro.ActionResultRepresentation) => {
                this.decrementPendingPotentAction();
                // if expect result and no warning from server generate one here
                if (actionResult.shouldExpectResult() && !actionResult.warningsOrMessages()) {
                    this.context.broadcastWarning(Msg.noResultMessage);
                }
            })
            .catch((reject: ErrorWrapper) => {
                this.decrementPendingPotentAction();
                const display = (em: Ro.ErrorMap) => this.vm.setMessage(em.invalidReason() || em.warningMessage);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });
    }

    // open dialog on current pane always - invoke action goes to pane indicated by click
    // note this can be modified by decorators
    // tslint:disable-next-line:member-ordering
    doInvoke = this.showDialog() ? this.invokeWithDialog : this.invokeWithoutDialog;

    private getInvokable() {
        if (this.invokableActionRep) {
            return Promise.resolve(this.invokableActionRep);
        }

        return this.context.getInvokableAction(this.actionRep)
            .then((details: Ro.ActionRepresentation | Ro.InvokableActionMember) => {
                this.invokableActionRep = details;
                return details;
            });
    }

    // note this is modified by decorators
    execute = (pps: ParameterViewModel[], right?: boolean): Promise<Ro.ActionResultRepresentation> => {
        const parmMap = zipObject(map(pps, p => p.id), map(pps, p => p.getValue())) as Dictionary<Ro.Value>;
        forEach(pps, p => this.urlManager.setParameterValue(this.actionRep.actionId(), p.parameterRep, p.getValue(), this.paneId));
        return this.getInvokable()
            .then((details: Ro.ActionRepresentation | Ro.InvokableActionMember) => this.context.invokeAction(details, parmMap, this.paneId, this.clickHandler.pane(this.paneId, right), this.gotoResult));
    }

    readonly disabled = () => !!this.actionRep.disabledReason();

    readonly returnsScalar = () => this.actionRep.returnsScalar();

    readonly tempDisabled = () => this.invokableActionRep &&
        this.invokableActionRep.isPotent() &&
        this.context.isPendingPotentActionOrReload(this.paneId)

    private getParameters(invokableAction: Ro.ActionRepresentation | Ro.InvokableActionMember) {
        const parameters = pickBy(invokableAction.parameters(), p => !p.isCollectionContributed()) as Dictionary<Ro.Parameter>;
        const parms = this.routeData.actionParams;
        return map(parameters, parm => this.viewModelFactory.parameterViewModel(parm, parms[parm.id()], this.paneId));
    }

    readonly parameters = () => {

        if (this.invokableActionRep) {
            const pps = this.getParameters(this.invokableActionRep);
            return Promise.resolve(pps);
        }

        return this.context.getInvokableAction(this.actionRep)
            .then((details: Ro.ActionRepresentation | Ro.InvokableActionMember) => {
                this.invokableActionRep = details;
                return this.getParameters(details);
            });
    }

    readonly makeInvokable = (details: Ro.ActionRepresentation | Ro.InvokableActionMember) => this.invokableActionRep = details;
}
