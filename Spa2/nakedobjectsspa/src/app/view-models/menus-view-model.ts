import { ViewModelFactoryService } from '../view-model-factory.service';
import { PaneRouteData } from '../route-data';
import { LinkViewModel } from './link-view-model';
import * as Models from '../models';
import * as _ from "lodash";

export class MenusViewModel {
    constructor(
        private viewModelFactory: ViewModelFactoryService,
        private menusRep: Models.MenusRepresentation,
        routeData: PaneRouteData
    ) {
        this.onPaneId = routeData.paneId;
        this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
    }

    onPaneId: number;
    items: LinkViewModel[];
}