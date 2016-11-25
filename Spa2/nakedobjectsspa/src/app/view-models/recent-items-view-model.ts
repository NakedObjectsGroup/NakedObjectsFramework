import { RecentItemViewModel } from './recent-item-view-model';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import * as _ from "lodash";

export class RecentItemsViewModel {

    constructor(
        private viewModelFactory: ViewModelFactoryService,
        private context: ContextService,
        private onPaneId: number
    ) {
        const items = _.map(this.context.getRecentlyViewed(), (o, i) => ({ obj: o, link: o.updateSelfLinkWithTitle(), index: i }));
        this.items = _.map(items, i => viewModelFactory.recentItemViewModel(i.obj, i.link, onPaneId, false, i.index));
    }

    items: RecentItemViewModel[];
}