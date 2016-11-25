import { LinkViewModel } from './link-view-model';
import { TableRowViewModel } from './table-row-view-model';
import { ContextService } from '../context.service';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { UrlManagerService } from '../url-manager.service';
import { ClickHandlerService } from '../click-handler.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import * as Ro from "../ro-interfaces";
import * as Models from "../models";
import * as Config from "../config";

export class ItemViewModel extends LinkViewModel {

    constructor(
        context: ContextService,
        colorService: ColorService,
        error: ErrorService,
        urlManager: UrlManagerService,
        link: Models.Link,
        paneId: number,
        private clickHandler: ClickHandlerService,
        private viewModelFactory: ViewModelFactoryService,
        private index: number,
        private isSelected: boolean
    ) {
        super(context, colorService, error, urlManager, link, paneId);

        const members = link.members();

        if (members) {
            this.tableRowViewModel = this.viewModelFactory.tableRowViewModel(members, paneId);
            this.tableRowViewModel.title = this.title;
        }
    }

    selectionChange = () => {
        this.context.updateValues();
        this.urlManager.setListItem(this.index, this.selected, this.paneId);
    };

    doClick = (right?: boolean) => {
        const currentPane = this.clickHandler.pane(this.paneId, right);
        this.urlManager.setItem(this.link, currentPane);
    };

    tableRowViewModel: TableRowViewModel;

    set selected(v: boolean) {
        this.isSelected = v;
        this.selectionChange();
    }

    get selected() {
        return this.isSelected;
    }
}