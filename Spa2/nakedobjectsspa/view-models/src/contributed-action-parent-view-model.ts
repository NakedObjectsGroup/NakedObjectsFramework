import * as Ro from '@nakedobjects/restful-objects';
import {
    ContextService,
    ErrorCategory,
    ErrorService,
    ErrorWrapper,
    HttpStatusCode,
    Pane,
    PaneRouteData,
    UrlManagerService
    } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import clone from 'lodash-es/clone';
import each from 'lodash-es/each';
import every from 'lodash-es/every';
import filter from 'lodash-es/filter';
import find from 'lodash-es/find';
import first from 'lodash-es/first';
import forEach from 'lodash-es/forEach';
import keys from 'lodash-es/keys';
import map from 'lodash-es/map';
import some from 'lodash-es/some';
import toArray from 'lodash-es/toArray';
import values from 'lodash-es/values';
import { ActionViewModel } from './action-view-model';
import * as Helpers from './helpers-view-models';
import { ItemViewModel } from './item-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { MessageViewModel } from './message-view-model';
import { ParameterViewModel } from './parameter-view-model';
import * as Msg from './user-messages';
import { ViewModelFactoryService } from './view-model-factory.service';

export abstract class ContributedActionParentViewModel extends MessageViewModel {

    protected constructor(
        protected readonly context: ContextService,
        protected readonly viewModelFactory: ViewModelFactoryService,
        protected readonly urlManager: UrlManagerService,
        protected readonly error: ErrorService,
        public readonly onPaneId: Pane
    ) {
        super();
    }

    items: ItemViewModel[];
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    readonly allSelected = () => every(this.items, item => item.selected);

    private isLocallyContributed(action: Ro.ActionRepresentation | Ro.InvokableActionMember) {
        return some(action.parameters(), p => p.isCollectionContributed());
    }

    setActions(actions: Dictionary<Ro.ActionMember>, routeData: PaneRouteData) {
        this.actions = map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
        this.menuItems = Helpers.createMenuItems(this.actions);
        forEach(this.actions, a => this.decorate(a));
    }

    protected collectionContributedActionDecorator(actionViewModel: ActionViewModel) {
        const wrappedInvoke = actionViewModel.execute;
        actionViewModel.execute = (pps: ParameterViewModel[], right?: boolean) => {

            const selected = filter(this.items, i => i.selected);

            const rejectAsNeedSelection = (action: Ro.ActionRepresentation | Ro.InvokableActionMember): ErrorWrapper | null => {
                if (this.isLocallyContributed(action)) {
                    if (selected.length === 0) {
                        const em = new Ro.ErrorMap({}, 0, Msg.noItemsSelected);
                        const rp = new ErrorWrapper(ErrorCategory.HttpClientError, HttpStatusCode.UnprocessableEntity, em);
                        return rp;
                    }
                }
                return null;
            };

            const getParms = (action: Ro.ActionRepresentation | Ro.InvokableActionMember) => {

                const parms = values(action.parameters()) as Ro.Parameter[];
                const contribParm = find(parms, p => p.isCollectionContributed());

                if (contribParm) {
                    const parmValue = new Ro.Value(map(selected, i => i.link));
                    const collectionParmVm = this.viewModelFactory.parameterViewModel(contribParm, parmValue, this.onPaneId);

                    const allpps = clone(pps);
                    allpps.push(collectionParmVm);
                    return allpps;
                }
                return pps;
            };

            const detailsPromise = actionViewModel.invokableActionRep
                ? Promise.resolve(actionViewModel.invokableActionRep)
                : this.context.getActionDetails(actionViewModel.actionRep as Ro.ActionMember);

            return detailsPromise.
                then(details => {
                    const rp = rejectAsNeedSelection(details);
                    return rp ? Promise.reject(rp) : wrappedInvoke(getParms(details), right);
                }).
                then(result => {
                    // clear selected items on void actions
                    this.clearSelected(result);
                    return result;
                });
        };
    }

    protected collectionContributedInvokeDecorator(actionViewModel: ActionViewModel) {

        const showDialog = () =>
            this.context.getInvokableAction(actionViewModel.actionRep).
                then(invokableAction => {
                    actionViewModel.makeInvokable(invokableAction);
                    const keyCount = keys(invokableAction.parameters()).length;
                    return keyCount > 1 || keyCount === 1 && !toArray(invokableAction.parameters())[0].isCollectionContributed();
                });

        // make sure not invokable  while waiting for promise to assign correct function
        actionViewModel.doInvoke = () => { };

        const invokeWithoutDialog = (right?: boolean) =>
            actionViewModel.invokeWithoutDialogWithParameters(Promise.resolve([]), right).then((actionResult: Ro.ActionResultRepresentation) => {
                this.setMessage(actionResult.shouldExpectResult() ? actionResult.warningsOrMessages() || Msg.noResultMessage : '');
                // clear selected items on void actions
                this.clearSelected(actionResult);
            });

        showDialog().
            then(show => actionViewModel.doInvoke = show ? actionViewModel.invokeWithDialog : invokeWithoutDialog).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));
    }

    protected decorate(actionViewModel: ActionViewModel) {
        this.collectionContributedActionDecorator(actionViewModel);
        this.collectionContributedInvokeDecorator(actionViewModel);
    }

    private setItems(newValue: boolean) {
        each(this.items, item => item.silentSelect(newValue));
        // TODO fix "!"
        const id = first(this.items)!.id;
        this.urlManager.setAllItemsSelected(newValue, id, this.onPaneId);
    }

    protected clearSelected(result: Ro.ActionResultRepresentation) {
        if (result.resultType() === 'void') {
            this.setItems(false);
        }
    }

    readonly selectAll = () => {
        const newState = !this.allSelected();
        this.setItems(newState);
    }
}
