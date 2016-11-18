import { TableRowColumnViewModel } from './table-row-column-view-model';
import { ViewModelFactoryService } from '../view-model-factory.service';
import * as _ from "lodash";
import * as Models from '../models';

export class TableRowViewModel {

    constructor(
        private viewModelFactory: ViewModelFactoryService,
        properties: _.Dictionary<Models.PropertyMember>,
        private paneId: number
    ) {
        this.properties = _.map(properties, (property, id) => viewModelFactory.propertyTableViewModel(id, property));
    }

    properties: TableRowColumnViewModel[];

    // todo these currently initialised outside constructor - smell ?
    title: string;
    hasTitle: boolean;

    getPlaceHolderTableRowColumnViewModel(id: string) {
        // no property so place holder for column 
        return this.viewModelFactory.propertyTableViewModel(id);
    }

    conformColumns(columns: string[]) {
        if (columns) {
            this.properties =
                _.map(columns, c => _.find(this.properties, tp => tp.id === c) || this.getPlaceHolderTableRowColumnViewModel(c));
        }
    }
}