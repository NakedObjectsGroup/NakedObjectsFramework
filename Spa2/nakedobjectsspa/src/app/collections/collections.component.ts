import { Component, Input } from '@angular/core';
import { CollectionViewModel } from '../view-models/collection-view-model';

@Component({
    selector: 'nof-collections',
    templateUrl: 'collections.component.html',
    styleUrls: ['collections.component.css']
})
export class CollectionsComponent {

    @Input()
    collections: CollectionViewModel[];
}
