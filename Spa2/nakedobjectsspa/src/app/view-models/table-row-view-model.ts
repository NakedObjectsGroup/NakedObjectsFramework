import { TableRowColumnViewModel } from './table-row-column-view-model';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { Dictionary } from 'lodash';
import * as Models from '../models';
import { Pane } from '../route-data';
import find from 'lodash-es/find';
import map from 'lodash-es/map';

export class TableRowViewModel {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        properties: Dictionary<Models.PropertyMember | Models.CollectionMember>,
        private readonly paneId: Pane,
        public readonly title: string
    ) {
        this.properties = map(properties, (property, id) => viewModelFactory.propertyTableViewModel(id as string, property));
    }

    properties: TableRowColumnViewModel[];

    showTitle = false;

    readonly getPlaceHolderTableRowColumnViewModel = (id: string) => this.viewModelFactory.propertyTableViewModel(id); // no property so place holder for column

    readonly conformColumns = (showTitle: boolean, columns: string[]) => {
        this.showTitle = showTitle;
        if (columns) {
            this.properties = map(columns, c => find(this.properties, tp => tp.id === c) || this.getPlaceHolderTableRowColumnViewModel(c));
        }
    }
}
