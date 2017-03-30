import { Component, OnInit, Input } from '@angular/core';
import * as Collectionviewmodel from '../view-models/collection-view-model';
import * as Listviewmodel from '../view-models/list-view-model';
import { CollectionViewState } from '../route-data';

@Component({
    selector: '[nof-header]',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.css']
})
export class HeaderComponent {

    @Input()
    collection: Collectionviewmodel.CollectionViewModel | Listviewmodel.ListViewModel;

    @Input()
    state: CollectionViewState;

    allSelected = () => this.collection.allSelected();
    selectAll = () => this.collection.selectAll();

    itemId = () => `${this.collection.name}${this.collection.onPaneId}-all`;

    private noItems() {
        return !this.collection.items || this.collection.items.length === 0;
    }

    showAllCheckbox = () => !(this.collection.disableActions() || this.noItems());

    get header() {
        return this.state === CollectionViewState.Table ? this.collection.header : null;
    }
}
