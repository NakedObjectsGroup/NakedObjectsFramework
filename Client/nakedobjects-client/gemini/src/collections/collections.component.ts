import { Component, Input } from '@angular/core';
import { CollectionViewModel } from '@nakedobjects/view-models';

@Component({
    selector: 'nof-collections',
    templateUrl: 'collections.component.html',
    styleUrls: ['collections.component.css'],
    standalone: false
})
export class CollectionsComponent {

    @Input({required: true})
    collections!: CollectionViewModel[];

    classes(coll: CollectionViewModel) {
        const hint = coll.presentationHint ?? '';
        return `collection ${hint}`.trim();
    }
}
