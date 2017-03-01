import { Component } from '@angular/core';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from '../url-manager.service';
import { RouteData, PaneRouteData } from '../route-data';
import { RecentItemsViewModel } from '../view-models/recent-items-view-model';
import { RecentItemViewModel } from '../view-models/recent-item-view-model';
import { PaneComponent } from '../pane/pane';
import { IButton } from '../button/button.component';
import * as Msg from '../user-messages';

@Component({
    selector: 'nof-recent',
    template: require('./recent.component.html'),
    styles: [require('./recent.component.css')]
})
export class RecentComponent extends PaneComponent {

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        private readonly viewModelFactory: ViewModelFactoryService,
    ) {
        super(activatedRoute, urlManager);
    }

    // template API 

    title = Msg.recentTitle;
    items = () => this.recent.items;

    // todo again a smell - new child component ! 
    itemColor = (item: RecentItemViewModel) => item.color;
    itemTitle = (item: RecentItemViewModel) => item.title;
    itemFriendlyName = (item: RecentItemViewModel) => item.friendlyName;

    doItemClick = (item: RecentItemViewModel, right?: boolean) => item.doClick(right);

    recent: RecentItemsViewModel;

    private clearButton: IButton = {
        value: Msg.clear,
        doClick: () => this.clear(),
        show: () => true,
        disabled: () => this.clearDisabled(),
        title: () => this.clearDisabled() ? Msg.recentDisabledMessage : Msg.recentMessage,
        accesskey: "c"
    };

    get buttons() {
        return [this.clearButton];
    }

    clear() {
        this.recent.clear();
    }

    clearDisabled() {
        return this.recent.items.length === 0 ? true : null;
    }

    protected setup(routeData: PaneRouteData) {
        this.recent = this.viewModelFactory.recentItemsViewModel(this.paneId);
    }
}