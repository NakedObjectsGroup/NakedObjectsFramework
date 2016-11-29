import { MessageViewModel } from './message-view-model';
import * as Models from '../models';
import * as Msg from '../user-messages';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { ContextService } from '../context.service';
import { ItemViewModel } from './item-view-model';
import { ActionViewModel } from './action-view-model';
import { ParameterViewModel } from './parameter-view-model';
import * as _ from "lodash";
import * as Helpers from './helpers-view-models';
import * as Menuitemviewmodel from './menu-item-view-model';
import * as Routedata from '../route-data';

export abstract class ContributedActionParentViewModel extends MessageViewModel {

    constructor(protected context: ContextService,
        protected viewModelFactory: ViewModelFactoryService,
        protected urlManager: UrlManagerService,
        protected error: ErrorService
    ) {
        super();
    }

    onPaneId: number;
    allSelected = () => _.every(this.items, item => item.selected);
    items: ItemViewModel[];
    actions: ActionViewModel[];
    menuItems: Menuitemviewmodel.MenuItemViewModel[];

    private isLocallyContributed(action: Models.IInvokableAction) {
        return _.some(action.parameters(), p => p.isCollectionContributed());
    }

    setActions(actions: _.Dictionary<Models.ActionMember>, routeData: Routedata.PaneRouteData) {
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
        this.menuItems = Helpers.createMenuItems(this.actions);
        _.forEach(this.actions, a => this.decorate(a));
    }


    protected collectionContributedActionDecorator(actionViewModel: ActionViewModel) {
        const wrappedInvoke = actionViewModel.execute;
        actionViewModel.execute = (pps: ParameterViewModel[], right?: boolean) => {

            const selected = _.filter(this.items, i => i.selected);


            const rejectAsNeedSelection = (action: Models.IInvokableAction) => {
                if (this.isLocallyContributed(action)) {

                    if (selected.length === 0) {

                        const em = new Models.ErrorMap({}, 0, Msg.noItemsSelected);
                        const rp = new Models.ErrorWrapper(Models.ErrorCategory.HttpClientError, Models.HttpStatusCode.UnprocessableEntity, em);

                        return rp;
                    }
                }
                return null;
            }


            const getParms = (action: Models.IInvokableAction) => {

                const parms = _.values(action.parameters()) as Models.Parameter[];
                const contribParm = _.find(parms, p => p.isCollectionContributed());

                if (contribParm) {
                    const parmValue = new Models.Value(_.map(selected, i => i.link));
                    const collectionParmVm = this.viewModelFactory.parameterViewModel(contribParm, parmValue, this.onPaneId);

                    const allpps = _.clone(pps);
                    allpps.push(collectionParmVm);
                    return allpps;
                }
                return pps;
            }

            if (actionViewModel.invokableActionRep) {
                const rp = rejectAsNeedSelection(actionViewModel.invokableActionRep);
                return rp ? Promise.reject(rp) : wrappedInvoke(getParms(actionViewModel.invokableActionRep), right).
                    then(result => {
                        // clear selected items on void actions 
                        this.clearSelected(result);
                        return result;
                    });
            }

            return this.context.getActionDetails(actionViewModel.actionRep as Models.ActionMember).
                then((details: Models.ActionRepresentation) => {
                    const rp = rejectAsNeedSelection(details);
                    if (rp) {
                        return Promise.reject(rp);
                    }
                    return wrappedInvoke(getParms(details), right);
                }).
                then(result => {
                    // clear selected items on void actions 
                    this.clearSelected(result);
                    return result;
                });
        }
    }

    protected collectionContributedInvokeDecorator(actionViewModel: ActionViewModel) {

        const showDialog = () =>
            this.context.getInvokableAction(actionViewModel.actionRep as Models.ActionMember).
                then(invokableAction => {
                    const keyCount = _.keys(invokableAction.parameters()).length;
                    return keyCount > 1 || keyCount === 1 && !_.toArray(invokableAction.parameters())[0].isCollectionContributed();
                });

        // make sure not null while waiting for promise to assign correct function 
        actionViewModel.doInvoke = () => { };

        const invokeWithDialog = (right?: boolean) => {
            this.context.clearDialogValues(this.onPaneId);
            //this.focusManager.focusOverrideOff();
            this.urlManager.setDialogOrMultiLineDialog(actionViewModel.actionRep, this.onPaneId);
        };

        const invokeWithoutDialog = (right?: boolean) =>
            actionViewModel.execute([], right).
                then(result => {
                    this.setMessage(result.shouldExpectResult() ? result.warningsOrMessages() || Msg.noResultMessage : "");
                    // clear selected items on void actions 
                    this.clearSelected(result);
                }).
                catch((reject: Models.ErrorWrapper) => {
                    const display = (em: Models.ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
                    this.error.handleErrorAndDisplayMessages(reject, display);
                });

        showDialog().
            then(show => actionViewModel.doInvoke = show ? invokeWithDialog : invokeWithoutDialog).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }

    protected decorate(actionViewModel: ActionViewModel) {
        this.collectionContributedActionDecorator(actionViewModel);
        this.collectionContributedInvokeDecorator(actionViewModel);
    }

    private setItems(newValue: boolean) {
        _.each(this.items, item => item.selected = newValue);
    }

    protected clearSelected(result: Models.ActionResultRepresentation) {
        if (result.resultType() === "void") {
           this.setItems(false);
        }
    }

    selectAll = () => {
        const newState = !this.allSelected();
        this.setItems(newState);
    };

}