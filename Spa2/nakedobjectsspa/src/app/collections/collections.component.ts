import { Component, Input } from '@angular/core';
import * as ViewModels from "../view-models";
import { CollectionViewModel } from '../view-models/collection-view-model';


@Component({
    selector: 'collections',
    templateUrl: './collections.component.html',
    styleUrls: ['./collections.component.css']
})
export class CollectionsComponent {

    colls: any;

    @Input()
    set collections(value: CollectionViewModel[]) {
        this.colls = value;
    }

    get collections() {
        return this.colls;
    }
}