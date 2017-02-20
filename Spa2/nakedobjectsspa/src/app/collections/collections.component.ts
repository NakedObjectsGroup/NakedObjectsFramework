import { Component, Input } from '@angular/core';
import { CollectionViewModel } from '../view-models/collection-view-model';

@Component({
    selector: 'nof-collections',
    template: require('./collections.component.html'),
    styles: [require('./collections.component.css')]
})
export class CollectionsComponent {

    @Input()
    collections : CollectionViewModel[];
}