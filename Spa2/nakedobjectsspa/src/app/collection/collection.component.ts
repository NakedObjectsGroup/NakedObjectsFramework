import { Component, Input } from '@angular/core';
import { CollectionViewState } from '../route-data';
import { CollectionViewModel } from '../view-models/collection-view-model';
import { ItemViewModel } from '../view-models/item-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';

type state = "summary" | "list" | "table";

@Component({
    selector: 'collection',
    templateUrl: './collection.component.html',
    styleUrls: ['./collection.component.css']
})
export class CollectionComponent {


    @Input()
    collection: CollectionViewModel;

    // todo why not genericise lazy ? 
    private lazyState: state;

    get state() {
        if (!this.lazyState) {
            this.lazyState = CollectionViewState[this.collection.currentState].toString().toLowerCase() as state;
        }
        return this.lazyState;
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

    matchingDialog = () => {
        //this.dialog.matchingCollectionId === this.collection.id;
    }

    showActions = () => {
        return !this.disableActions() && (this.state === "table" || this.state === "list");
    }

    showSummary = () => {
        return (this.mayHaveItems || !this.disableActions()) && (this.state === "table" || this.state === "list");
    }

    showList = () => {
        return (this.mayHaveItems || !this.disableActions()) && (this.state === 'table' || this.state === 'summary');
    }

    showTable = () => {
        return this.mayHaveItems &&  (this.state === 'list' || this.state === 'summary');
    }

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

    itemId = (i: number) => `item${this.collection.onPaneId}-${i}`;

    itemTableTitle = (item: ItemViewModel) => item.tableRowViewModel.title;
    itemHasTableTitle = (item: ItemViewModel) => item.tableRowViewModel.hasTitle;
    itemTableProperties = (item: ItemViewModel) => item.tableRowViewModel.properties;

    propertyType = (property: PropertyViewModel) => property.type;
    propertyValue = (property: PropertyViewModel) => property.value;
    propertyFormattedValue = (property: PropertyViewModel) => property.formattedValue;
    propertyReturnType = (property: PropertyViewModel) => property.returnType;
}