import { Component, Input } from '@angular/core';
import * as ViewModels from "../view-models";
import {CollectionViewState} from '../route-data';

@Component({
    selector: 'collection',
    templateUrl: './collection.component.html',
    styleUrls: ['./collection.component.css']
})
export class CollectionComponent {

    coll: ViewModels.CollectionViewModel;

    @Input()
    set collection(value: ViewModels.CollectionViewModel) {
        this.coll = value;
        this.state = CollectionViewState[this.coll.requestedState].toString().toLowerCase();
    }

    get collection() {
        return this.coll;
    }

    state: string;
}