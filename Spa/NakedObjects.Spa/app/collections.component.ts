import { Component, Input } from '@angular/core';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import { PropertyComponent } from "./property.component";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Collectioncomponent from './collection.component';

@Component({
    selector: 'collections',
    templateUrl: 'app/collections.component.html',
    directives: [Collectioncomponent.CollectionComponent],
    styleUrls: ['app/collections.component.css']
})

export class CollectionsComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) { }

    colls: any;

    @Input()
    set collections(value: ViewModels.CollectionViewModel[]) {
        this.colls = value;
    }

    get collections() {
        return this.colls;
    }
}