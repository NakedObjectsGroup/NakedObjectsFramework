import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Nakedobjectsroutedata from './nakedobjects.routedata';

@Component({
    selector: 'collection',
    templateUrl: 'app/collection.component.html',
    styleUrls: ['app/collection.component.css']
})
export class CollectionComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) { }

    coll: ViewModels.CollectionViewModel;


    @Input()
    set collection(value: ViewModels.CollectionViewModel) {
        this.coll = value;
        this.state = Nakedobjectsroutedata.CollectionViewState[this.coll.currentState].toString().toLowerCase();

    }

    get collection() {
        return this.coll;
    }

    state: string;
}