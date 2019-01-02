import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CollectionViewState } from '@nakedobjects/services';
import { CollectionViewModel } from '@nakedobjects/view-models';
import { PaneRouteData } from '@nakedobjects/services';
import { UrlManagerService } from '@nakedobjects/services';
import { ItemViewModel } from '@nakedobjects/view-models'; // needed for declarations compile
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from '../helpers-components';

type State = 'summary' | 'list' | 'table';

@Component({
    selector: 'nof-collection',
    templateUrl: 'collection.component.html',
    styleUrls: ['collection.component.css']
})
export class CollectionComponent implements OnInit, OnDestroy {

    constructor(private readonly urlManager: UrlManagerService) { }

    @Input()
    collection: CollectionViewModel;

    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData: PaneRouteData;
    private currentOid: string;

    selectedDialogId: string;

    get currentState() {
        return this.collection.currentState;
    }

    get state() {
        return CollectionViewState[this.currentState].toString().toLowerCase() as State;
    }

    get title() {
        return this.collection.title;
    }

    get details() {
        return this.collection.details;
    }

    get mayHaveItems() {
        return this.collection.mayHaveItems;
    }

    get header() {
        return this.collection.header;
    }

    get items(): ItemViewModel[] {
        return this.collection.items;
    }

    get message() {
        return this.collection.getMessage();
    }

    private isSummary = () => this.collection.currentState === CollectionViewState.Summary;

    private isList = () => this.collection.currentState === CollectionViewState.List;

    private isTable = () => this.collection.currentState === CollectionViewState.Table;

    showActions = () => !this.disableActions() && (this.isTable() || this.isList());
    showSummary = () => (this.mayHaveItems || !this.disableActions()) && (this.isList() || this.isTable());
    showList = () => (this.mayHaveItems || !this.disableActions()) && (this.isTable() || this.isSummary());
    showTable = () => this.mayHaveItems && (this.isList() || this.isSummary());

    doSummary = () => this.collection.doSummary();
    doList = () => this.collection.doList();
    doTable = () => this.collection.doTable();
    disableActions = () => this.collection.noActions();

    hasTableData = () => this.collection.hasTableData();

    ngOnInit(): void {

        this.paneRouteDataSub = this.urlManager.getPaneRouteDataObservable(this.collection.onPaneId)
            .subscribe((paneRouteData: PaneRouteData) => {
                if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
                    this.lastPaneRouteData = paneRouteData;
                    this.currentOid = this.currentOid || paneRouteData.objectId;

                    // ignore if different object
                    if (this.currentOid === paneRouteData.objectId) {
                        this.collection.reset(paneRouteData, false);
                        this.collection.resetMessage();
                    }
                    this.selectedDialogId = paneRouteData.dialogId;
                }
            });
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.paneRouteDataSub);
    }
}
