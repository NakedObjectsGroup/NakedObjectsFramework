import * as Ro from '@nakedobjects/restful-objects';
import { ContextService, Pane, UrlManagerService } from '@nakedobjects/services';
import every from 'lodash-es/every';
import map from 'lodash-es/map';
import { RecentItemViewModel } from './recent-item-view-model';
import { ViewModelFactoryService } from './view-model-factory.service';

export class RecentItemsViewModel {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly context: ContextService,
        private readonly urlManager: UrlManagerService,
        private readonly onPaneId: Pane
    ) {
        this.recentlyViewed = this.context.getRecentlyViewed();
        this.refreshItems();
    }

    private recentItems: RecentItemViewModel[];
    private recentlyViewed: Ro.DomainObjectRepresentation[];

    private refreshItems() {
        const items = map(this.recentlyViewed, (o, i) => ({ obj: o, link: o.updateSelfLinkWithTitle(), index: i }));
        this.recentItems = map(items, i => this.viewModelFactory.recentItemViewModel(i.obj, i.link!, this.onPaneId, false, i.index));
    }

    private itemsHaveChanged() {
        const currentRecentlyViewed = this.context.getRecentlyViewed();

        const same = this.recentlyViewed.length === currentRecentlyViewed.length &&
            every(this.recentlyViewed, (v, i) => v.id() === currentRecentlyViewed[i].id());

        if (!same) {
            this.recentlyViewed = currentRecentlyViewed;
        }

        return !same;
    }

    get items(): RecentItemViewModel[] {
        if (this.itemsHaveChanged()) {
            this.refreshItems();
        }
        return this.recentItems;
    }

    readonly clear = () => {
        this.context.clearRecentlyViewed();
        this.recentItems = [];
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl(this.onPaneId);
    }
}
