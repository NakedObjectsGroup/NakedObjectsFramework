import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CollectionViewState } from '../route-data';
import { CollectionViewModel } from '../view-models/collection-view-model';
import { ItemViewModel } from '../view-models/item-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';
import { PaneRouteData } from '../route-data';
import { UrlManagerService } from '../url-manager.service';
import { ISubscription } from 'rxjs/Subscription';
import { TableRowColumnViewModel } from '../view-models/table-row-column-view-model'; // needed for declarations compile 

type state = "summary" | "list" | "table";

@Component({
    selector: 'nof-collection',
    template: require('./collection.component.html'),
    styles: [require('./collection.component.css')]
})
export class CollectionComponent implements OnInit, OnDestroy {

    constructor(private readonly urlManager: UrlManagerService) {}

    @Input()
    collection: CollectionViewModel;

    get state() {
        return CollectionViewState[this.collection.currentState].toString().toLowerCase() as state;
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

    get items() {
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
    disableActions = () => this.collection.disableActions();
    noItems = () => !this.collection.items || this.collection.items.length === 0;
    allSelected = () => this.collection.allSelected();
    selectAll = () => this.collection.selectAll();
    hasTableData = () => this.collection.hasTableData();

    itemColor = (item: ItemViewModel) => item.color;
    itemTitle = (item: ItemViewModel) => item.title;
    doItemClick = (item: ItemViewModel, right?: boolean) => item.doClick(right);

    itemId = (i: number | string) => `${this.collection.id}${this.collection.onPaneId}-${i}`;

    itemTableTitle = (item: ItemViewModel) => item.tableRowViewModel.title;
    itemHasTableTitle = (item: ItemViewModel) => item.tableRowViewModel.showTitle;
    itemTableProperties = (item: ItemViewModel) => item.tableRowViewModel.properties;

    propertyType = (property: PropertyViewModel) => property.type;
    propertyValue = (property: PropertyViewModel) => property.value;
    propertyFormattedValue = (property: PropertyViewModel) => property.formattedValue;
    propertyReturnType = (property: PropertyViewModel) => property.returnType;

    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData : PaneRouteData;

    private currentOid: string;

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

        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    }

    selectedDialogId: string;
}