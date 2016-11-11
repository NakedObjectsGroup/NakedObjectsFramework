import { Component, Input } from '@angular/core';
import * as ViewModels from "../view-models";

@Component({
    selector: 'collections',
    templateUrl: './collections.component.html',
    styleUrls: ['./collections.component.css']
})
export class CollectionsComponent {

    colls: any;

    @Input()
    set collections(value: ViewModels.CollectionViewModel[]) {
        this.colls = value;
    }

    get collections() {
        return this.colls;
    }
}