﻿import { Component, Input } from '@angular/core';
import { CollectionViewState } from '@nakedobjects/services';
import { CollectionViewModel, ListViewModel } from '@nakedobjects/view-models';

@Component({
    // tslint:disable-next-line:component-selector
    selector: '[nof-header]',
    templateUrl: 'header.component.html',
    styleUrls: ['header.component.css']
})
export class HeaderComponent {

    @Input()
    collection: CollectionViewModel | ListViewModel;

    @Input()
    state: CollectionViewState;

    allSelected = () => this.collection.allSelected();
    selectAll = () => this.collection.selectAll();

    itemId = () => `${this.collection.name}${this.collection.onPaneId}-all`;

    private noItems() {
        return !this.collection.items || this.collection.items.length === 0;
    }

    showAllCheckbox = () => !(this.collection.noActions() || this.noItems());

    get header() {
        return this.state === CollectionViewState.Table ? this.collection.header : null;
    }
}
