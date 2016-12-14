import { DialogViewModel } from './dialog-view-model';
import { RouteData, PaneRouteData } from '../route-data';
import { ColorService } from '../color.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import * as Models from '../models';
import * as Ro from '../ro-interfaces';
import * as Config from '../config';
import * as Msg from '../user-messages';
import * as _ from "lodash";

export class MultiLineDialogViewModel {

    constructor(
        private color: ColorService,
        private context: ContextService,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private error: ErrorService,
        routeData: PaneRouteData,
        action: Models.IInvokableAction
    ) {
        this.reset(routeData, action);
    }


    private createRow(i: number) {
        const dialogViewModel = this.viewModelFactory.dialogViewModel(this.routeData, this.action as Models.IInvokableAction, null, true);

        dialogViewModel.actionViewModel.gotoResult = false;

        dialogViewModel.doCloseKeepHistory = () => {
            dialogViewModel.submitted = true;
        };

        dialogViewModel.doCloseReplaceHistory = () => {
            dialogViewModel.submitted = true;
        };

        dialogViewModel.doCompleteButLeaveOpen = () => {
            dialogViewModel.submitted = true;
        };

        dialogViewModel.parameters.forEach(p => p.setAsRow(i));

        return dialogViewModel;
    }

    objectFriendlyName: string;
    objectTitle: string;

    title: string = "";
    action: Models.IInvokableAction;
    routeData: PaneRouteData;

    reset(routeData: PaneRouteData, action: Models.IInvokableAction) {

        this.action = action;
        this.routeData = routeData;
        this.action.parent.etagDigest = "*";

        const initialCount = action.extensions().multipleLines() || 1;

        this.dialogs = _.map(_.range(initialCount), (i) => this.createRow(i));
        this.title = this.dialogs[0].title;
        return this;
    }

    dialogs: DialogViewModel[] = [];

    header() {
        return this.dialogs.length === 0 ? [] : _.map(this.dialogs[0].parameters, p => p.title);
    }

    clientValid() {
        return _.every(this.dialogs, d => d.clientValid());
    }

    tooltip() {
        const tooltips = _.map(this.dialogs, (d, i) => `row: ${i} ${d.tooltip() || "OK"}`);
        return _.reduce(tooltips, (s, t) => `${s}\n${t}`);
    }

    invokeAndAdd(index: number) {
        this.dialogs[index].doInvoke();
        this.context.clearDialogValues();
        return this.add(index);
        //this.focusManager.focusOn(FocusTarget.MultiLineDialogRow, 1, 1); 
    }

    add(index: number) {
        if (index === this.dialogs.length - 1) {
            // if this is last dialog always add another
            return this.dialogs.push(this.createRow(this.dialogs.length)) - 1;
        }
        else if (_.takeRight(this.dialogs)[0].submitted) {
            // if the last dialog is submitted add another 
            return this.dialogs.push(this.createRow(this.dialogs.length)) - 1;
        }
        return 0;
    }

    clear(index: number) {
        _.pullAt(this.dialogs, [index]);
    }

    submitAll() {
        if (this.clientValid()) {
            _.each(this.dialogs, d => {
                if (!d.submitted) {
                    d.doInvoke();
                }
            });
        }
    }

    submittedCount() {
        return (_.filter(this.dialogs, d => d.submitted)).length;
    }

    close() {
        this.urlManager.popUrlState();
    }

}