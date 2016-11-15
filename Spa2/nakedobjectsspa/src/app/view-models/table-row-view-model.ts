import { TableRowColumnViewModel } from './table-row-column-view-model';
import * as _ from "lodash";

export class TableRowViewModel {
    title: string;
    hasTitle: boolean;
    properties: TableRowColumnViewModel[];

    getPlaceHolderTableRowColumnViewModel(id: string) {
        const ph = new TableRowColumnViewModel();
        ph.id = id;
        ph.type = "scalar";
        ph.value = "";
        ph.formattedValue = "";
        ph.title = "";
        return ph;
    }

    conformColumns(columns: string[]) {
        if (columns) {
            this.properties =
                _.map(columns, c => _.find(this.properties, tp => tp.id === c) || this.getPlaceHolderTableRowColumnViewModel(c));
        }
    }
}