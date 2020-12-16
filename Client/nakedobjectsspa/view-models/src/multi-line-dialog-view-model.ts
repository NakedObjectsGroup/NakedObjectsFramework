﻿import * as Ro from '@nakedobjects/restful-objects';
import { ColorService, ContextService, ErrorService, PaneRouteData, UrlManagerService } from '@nakedobjects/services';
import filter from 'lodash-es/filter';
import map from 'lodash-es/map';
import range from 'lodash-es/range';
import takeRight from 'lodash-es/takeRight';
import { CollectionViewModel } from './collection-view-model';
import { DialogViewModel } from './dialog-view-model';
import { ViewModelFactoryService } from './view-model-factory.service';
import * as Msg from './user-messages';

export class MultiLineDialogViewModel {

    constructor(
        private readonly color: ColorService,
        private readonly context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly urlManager: UrlManagerService,
        private readonly error: ErrorService,
        private readonly routeData: PaneRouteData,
        private readonly action: Ro.ActionRepresentation | Ro.InvokableActionMember,
        holder: Ro.MenuRepresentation | Ro.DomainObjectRepresentation | CollectionViewModel
    ) {

        if (holder instanceof Ro.DomainObjectRepresentation) {
            this.objectTitle = holder.title();
            this.objectFriendlyName = holder.extensions().friendlyName();
        }

        const initialCount = action.extensions().multipleLines() || 1;

        this.dialogs = map(range(initialCount), i => this.createRow(i));
        this.title = this.dialogs[0].title;
        this.action.parent.etagDigest = '*';
    }

    readonly objectFriendlyName: string = '';
    readonly objectTitle: string = '';
    readonly title: string;
    readonly dialogs: DialogViewModel[];

    private readonly createRow = (i: number) => {
        return this.viewModelFactory.dialogViewModel(this.routeData, this.action as Ro.ActionRepresentation | Ro.InvokableActionMember, null, true, i);
    }

    readonly header = () => this.dialogs.length === 0 ? [] : map(this.dialogs[0].parameters, p => p.title);

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
        } else if (takeRight(this.dialogs)[0].submitted) {
            // if the last dialog is submitted add another
            return this.pushNewDialog();
        }
        return 0;
    }

    readonly submittedCount = () => filter(this.dialogs, d => d.submitted).length;

    readonly submittedCountMsg = () => {
        return Msg.submittedCount(this.submittedCount());
    }
}
