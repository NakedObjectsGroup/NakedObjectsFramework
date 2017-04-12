import { DialogViewModel } from './dialog-view-model';
import { CollectionViewModel } from './collection-view-model';
import { PaneRouteData } from '../route-data';
import { ColorService } from '../color.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import * as Models from '../models';
import * as _ from 'lodash';

export class MultiLineDialogViewModel {

    constructor(
        private readonly color: ColorService,
        private readonly context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly urlManager: UrlManagerService,
        private readonly error: ErrorService,
        private readonly routeData: PaneRouteData,
        private readonly action: Models.IInvokableAction,
        holder: Models.MenuRepresentation | Models.DomainObjectRepresentation | CollectionViewModel
    ) {

        if (holder instanceof Models.DomainObjectRepresentation) {
            this.objectTitle = holder.title();
            this.objectFriendlyName = holder.extensions().friendlyName();
        }

        const initialCount = action.extensions().multipleLines() || 1;

        this.dialogs = _.map(_.range(initialCount), i => this.createRow(i));
        this.title = this.dialogs[0].title;
        this.action.parent.etagDigest = "*";
    }

    private readonly createRow = (i: number) => {
        const dialogViewModel = this.viewModelFactory.dialogViewModel(this.routeData, this.action as Models.IInvokableAction, null, true);

        dialogViewModel.actionViewModel.gotoResult = false;
        dialogViewModel.doCloseKeepHistory = () => dialogViewModel.submitted = true;
        dialogViewModel.doCloseReplaceHistory = () => dialogViewModel.submitted = true;
        dialogViewModel.doCompleteButLeaveOpen = () => dialogViewModel.submitted = true;
        dialogViewModel.parameters.forEach(p => p.setAsRow(i));
        return dialogViewModel;
    }

    readonly objectFriendlyName : string = "";
    readonly objectTitle : string = "";
    readonly title: string;
    readonly dialogs: DialogViewModel[];

    readonly header = () => this.dialogs.length === 0 ? [] : _.map(this.dialogs[0].parameters, p => p.title);

    readonly invokeAndAdd = (index: number) => {
        this.dialogs[index].doInvoke();
        this.context.clearDialogCachedValues();
        return this.add(index);
    }

    private readonly pushNewDialog = () => this.dialogs.push(this.createRow(this.dialogs.length)) - 1;

    private readonly add = (index: number) => {
        if (index === this.dialogs.length - 1) {
            // if this is last dialog always add another
            return this.pushNewDialog();
        }
        else if (_.takeRight(this.dialogs)[0].submitted) {
            // if the last dialog is submitted add another 
            return this.pushNewDialog();
        }
        return 0;
    }

    readonly submittedCount = () => _.filter(this.dialogs, d => d.submitted).length;
}