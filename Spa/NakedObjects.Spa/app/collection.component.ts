import { Component, Input } from '@angular/core';
import * as ViewModels from "./nakedobjects.viewmodels";
import {CollectionViewState} from './nakedobjects.routedata';

@Component({
    selector: 'collection',
    templateUrl: 'app/collection.component.html',
    styleUrls: ['app/collection.component.css']
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