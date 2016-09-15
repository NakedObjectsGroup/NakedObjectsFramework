import { Component, Input } from '@angular/core';
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'collections',
    templateUrl: 'app/collections.component.html',
    styleUrls: ['app/collections.component.css']
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