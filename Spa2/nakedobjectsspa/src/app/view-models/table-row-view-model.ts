import { TableRowColumnViewModel } from './table-row-column-view-model';
import { ViewModelFactoryService } from '../view-model-factory.service';
import * as _ from 'lodash';
import * as Models from '../models';

export class TableRowViewModel {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        properties: _.Dictionary<Models.PropertyMember>,
        private readonly paneId: number,
        public readonly title: string
    ) {
        this.properties = _.map(properties, (property, id) => viewModelFactory.propertyTableViewModel(id as string, property));
    }

    properties: TableRowColumnViewModel[];

    showTitle = false;

    readonly getPlaceHolderTableRowColumnViewModel = (id: string) => this.viewModelFactory.propertyTableViewModel(id); // no property so place holder for column

    readonly conformColumns = (showTitle: boolean, columns: string[]) => {
        this.showTitle = showTitle;
        if (columns) {
            this.properties = _.map(columns, c => _.find(this.properties, tp => tp.id === c) || this.getPlaceHolderTableRowColumnViewModel(c));
        }
    }
}