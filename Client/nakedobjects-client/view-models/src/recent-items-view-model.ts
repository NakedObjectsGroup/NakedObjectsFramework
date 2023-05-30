import * as Ro from '@nakedobjects/restful-objects';
import { ContextService, Pane, PaneRouteData, UrlManagerService } from '@nakedobjects/services';
import every from 'lodash-es/every';
import map from 'lodash-es/map';
import { RecentItemViewModel } from './recent-item-view-model';
import { ViewModelFactoryService } from './view-model-factory.service';
import * as Msg from './user-messages';

export enum SortType { ByUsage, ByType, ByTitle }

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

    private recentItems: RecentItemViewModel[] = [];
    private recentlyViewed: Ro.DomainObjectRepresentation[];
    private sortType = SortType.ByUsage; 

    public get currentSortType() {
        return this.sortType;
    }

    title = Msg.recentTitle;
    clearMessage = Msg.clear;

    private refreshItems() {
        const items = map(this.recentlyViewed, (o, i) => ({ obj: o, link: o.updateSelfLinkWithTitle(), index: i }));
        const itemViewModels = map(items, i => this.viewModelFactory.recentItemViewModel(i.obj, i.link!, this.onPaneId, false, i.index));

        if (this.currentSortType === SortType.ByTitle) {
            this.recentItems = itemViewModels.sort((r1, r2) => r1.title.localeCompare(r2.title));
        }
        else if (this.currentSortType === SortType.ByType) {
            this.recentItems = itemViewModels.sort((r1, r2) => r1.friendlyName.localeCompare(r2.friendlyName));
        }
        else {
            this.recentItems = itemViewModels;
        }
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
    };

    readonly clearSelected = (routeData?: PaneRouteData) => {
        const toClear = routeData?.selectedCollectionItems[''] || [];
        this.context.clearSelectedRecentlyViewed(toClear);
        this.urlManager.setAllItemsSelected(false, '', this.onPaneId);
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl(this.onPaneId);
    };


    sort = (sortBy: SortType) => {
        if (sortBy !== this.currentSortType) {
            this.sortType = sortBy;
            this.refreshItems();
        }
    };

    getRecentMessage(disabled: boolean) {
        return disabled ? Msg.recentDisabledMessage : Msg.recentMessage;
    }
}
