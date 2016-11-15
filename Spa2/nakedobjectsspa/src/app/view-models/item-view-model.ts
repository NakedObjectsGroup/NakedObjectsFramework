import { LinkViewModel} from './link-view-model';
import * as Viewmodels from '../view-models';


export class ItemViewModel extends LinkViewModel {
    tableRowViewModel: Viewmodels.TableRowViewModel;

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
