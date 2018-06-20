import { LinkViewModel } from './link-view-model';
import { TableRowViewModel } from './table-row-view-model';
import { ContextService } from '../context.service';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { UrlManagerService } from '../url-manager.service';
import { ClickHandlerService } from '../click-handler.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import * as Models from '../models';
import { ConfigService } from '../config.service';
import { Pane } from '../route-data';

export class ItemViewModel extends LinkViewModel {

    constructor(
        context: ContextService,
        colorService: ColorService,
        error: ErrorService,
        urlManager: UrlManagerService,
        configService: ConfigService,
        link: Models.Link,
        paneId: Pane,
        private readonly clickHandler: ClickHandlerService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly index: number,
        private isSelected: boolean,
        public readonly id: string
    ) {
        super(context, colorService, error, urlManager, configService, link, paneId);

        const members = link.members();

        if (members) {
            this.tableRowViewModel = this.viewModelFactory.tableRowViewModel(members, paneId, this.title);
        }
    }

    readonly tableRowViewModel: TableRowViewModel;

    readonly selectionChange = () => {
        this.urlManager.setItemSelected(this.index, this.selected, this.id, this.paneId);
    }

    readonly doClick = (right?: boolean) => {
        const currentPane = this.clickHandler.pane(this.paneId, right);
        this.urlManager.setItem(this.link, currentPane);
    }

    set selected(v: boolean) {
        this.isSelected = v;
        this.selectionChange();
    }

    get selected() {
        return this.isSelected;
    }

    silentSelect(v: boolean) {
        this.isSelected = v;
    }
}
