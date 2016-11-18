import { Component, Input } from '@angular/core';
import { CollectionViewState } from '../route-data';
import { CollectionViewModel } from '../view-models/collection-view-model';
import {ItemViewModel} from '../view-models/item-view-model';
import * as Propertyviewmodel from '../view-models/property-view-model';

@Component({
    selector: 'collection',
    templateUrl: './collection.component.html',
    styleUrls: ['./collection.component.css']
})
export class CollectionComponent {

   
    @Input()
    collection : CollectionViewModel;

    // todo why not genericise lazy ? 
    private lazyState : string;
    get state() {
        if (!this.lazyState) {
           this.lazyState = CollectionViewState[this.collection.currentState].toString().toLowerCase();
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

    doSummary = () => this.collection.doSummary();
    doList = () => this.collection.doList();
    doTable = () => this.collection.doTable();
    disableActions = () => this.collection.disableActions();
    noItems = () => this.collection.items.length === 0;
    allSelected = () => this.collection.allSelected();
    selectAll = () => this.collection.selectAll();
    hasTableData = () => this.collection.hasTableData(); 

    itemColor = (item: ItemViewModel) => item.color;
    itemTitle = (item: ItemViewModel) => item.title;
    doItemClick = (item: ItemViewModel, right?: boolean) => item.doClick(right);

    itemId = (i: number) => `item${this.collection.onPaneId}-${i}`;
    itemSelected = (item: ItemViewModel) => item.selected;

    itemTableTitle =  (item : ItemViewModel) => item.tableRowViewModel.title;
    itemHasTableTitle = (item: ItemViewModel) => item.tableRowViewModel.hasTitle;
    itemTableProperties = (item: ItemViewModel) => item.tableRowViewModel.properties;


    propertyType = (property: Propertyviewmodel.PropertyViewModel) => property.type; 
    propertyValue = (property: Propertyviewmodel.PropertyViewModel) => property.value; 
    propertyFormattedValue = (property: Propertyviewmodel.PropertyViewModel) => property.formattedValue; 
    propertyReturnType = (property: Propertyviewmodel.PropertyViewModel) => property.returnType; 
}