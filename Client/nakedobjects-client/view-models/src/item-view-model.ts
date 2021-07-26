import * as Ro from '@nakedobjects/restful-objects';
import { ClickHandlerService, ColorService, ConfigService, ContextService, ErrorService, Pane, UrlManagerService } from '@nakedobjects/services';
import { LinkViewModel } from './link-view-model';
import { TableRowViewModel } from './table-row-view-model';
import { ViewModelFactoryService } from './view-model-factory.service';

export class ItemViewModel extends LinkViewModel {

    constructor(
        context: ContextService,
        colorService: ColorService,
        error: ErrorService,
        urlManager: UrlManagerService,
        configService: ConfigService,
        link: Ro.Link,
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
