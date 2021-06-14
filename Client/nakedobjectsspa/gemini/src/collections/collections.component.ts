import { Component, Input } from '@angular/core';
import { CollectionViewModel } from '@nakedobjects/view-models';

@Component({
    selector: 'nof-collections',
    templateUrl: 'collections.component.html',
    styleUrls: ['collections.component.css']
})
export class CollectionsComponent {

    @Input()
    collections: CollectionViewModel[];

    classes(coll: CollectionViewModel) {
        const hint = coll.presentationHint ?? '';
        return `collection ${hint}`.trim();
    }
}
