import { RecentItemViewModel } from './recent-item-view-model';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import map from 'lodash/map';
import every from 'lodash/map';
import * as Models from '../models';
import { UrlManagerService } from '../url-manager.service';
import { Pane } from '../route-data';

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

    private recentlyViewed : Models.DomainObjectRepresentation[];

    private refreshItems() {       
        const items = map(this.recentlyViewed, (o, i) => ({ obj: o, link: o.updateSelfLinkWithTitle(), index: i }));
        this._items = map(items, i => this.viewModelFactory.recentItemViewModel(i.obj, i.link!, this.onPaneId, false, i.index));
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

    private _items : RecentItemViewModel[];

    get items() : RecentItemViewModel[] {
        if (this.itemsHaveChanged()) {
            this.refreshItems();
        }
        return this._items;
    }

    readonly clear = () => {
        this.context.clearRecentlyViewed();
        this._items = [];
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl(this.onPaneId);
    }
}