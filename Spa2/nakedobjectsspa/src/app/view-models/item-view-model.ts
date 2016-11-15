import { LinkViewModel} from './link-view-model';
import * as Viewmodels from '../view-models';
import { TableRowViewModel } from './table-row-view-model';

export class ItemViewModel extends LinkViewModel {
    tableRowViewModel: TableRowViewModel;

    _selected: boolean;

    set selected(v: boolean) {
        this._selected = v;
        this.selectionChange();
    }

    get selected() {
        return this._selected;
    }

    selectionChange: () => void;
}
