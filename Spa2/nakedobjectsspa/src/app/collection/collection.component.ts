import { Component, Input } from '@angular/core';
import * as ViewModels from "../view-models";
import { CollectionViewState } from '../route-data';
import { CollectionViewModel } from '../view-models/collection-view-model';


@Component({
    selector: 'collection',
    templateUrl: './collection.component.html',
    styleUrls: ['./collection.component.css']
})
export class CollectionComponent {

    coll: CollectionViewModel;

    @Input()
    set collection(value: CollectionViewModel) {
        this.coll = value;
        this.state = CollectionViewState[this.coll.currentState].toString().toLowerCase();
    }

    get collection() {
        return this.coll;
    }

    state: string;
}