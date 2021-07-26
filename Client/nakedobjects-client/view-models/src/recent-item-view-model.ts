import * as Models from '@nakedobjects/restful-objects';
import { ClickHandlerService, ColorService, ConfigService, ContextService, ErrorService, Pane, UrlManagerService } from '@nakedobjects/services';
import { ItemViewModel } from './item-view-model';
import { ViewModelFactoryService } from './view-model-factory.service';

export class RecentItemViewModel extends ItemViewModel {

    constructor(
        context: ContextService,
        colorService: ColorService,
        error: ErrorService,
        urlManager: UrlManagerService,
        configService: ConfigService,
        link: Models.Link,
        paneId: Pane,
        clickHandler: ClickHandlerService,
        viewModelFactory: ViewModelFactoryService,
        index: number,
        isSelected: boolean,
        public readonly friendlyName: string
    ) {
        super(context, colorService, error, urlManager, configService, link, paneId, clickHandler, viewModelFactory, index, isSelected, '');
    }
}
