import { RecentItemViewModel } from './recent-item-view-model';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import * as _ from 'lodash';
import { UrlManagerService } from '../url-manager.service';

export class RecentItemsViewModel {

    constructor(
        viewModelFactory: ViewModelFactoryService,
        private readonly context: ContextService,
        private readonly urlManager: UrlManagerService,
        onPaneId: number
    ) {
        const items = _.map(this.context.getRecentlyViewed(), (o, i) => ({ obj: o, link: o.updateSelfLinkWithTitle(), index: i }));
        this.items = _.map(items, i => viewModelFactory.recentItemViewModel(i.obj, i.link!, onPaneId, false, i.index));
    }

    readonly items: RecentItemViewModel[];

    readonly clear = () => {
        this.context.clearRecentlyViewed();
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
    }
}