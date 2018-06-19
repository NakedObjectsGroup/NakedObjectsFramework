import { ContextService } from '../context.service';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { UrlManagerService } from '../url-manager.service';
import { ClickHandlerService } from '../click-handler.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ItemViewModel } from './item-view-model';
import * as Models from '../models';
import { ConfigService } from '../config.service';
import { Pane } from '../route-data';

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
        super(context, colorService, error, urlManager, configService, link, paneId, clickHandler, viewModelFactory, index, isSelected, "");
    }
}
