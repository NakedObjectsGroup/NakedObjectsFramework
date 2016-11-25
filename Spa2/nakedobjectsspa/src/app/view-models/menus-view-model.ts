import { ViewModelFactoryService } from '../view-model-factory.service';
import { PaneRouteData } from '../route-data';
import { LinkViewModel } from './link-view-model';
import * as Models from '../models';
import * as _ from "lodash";

export class MenusViewModel {
    constructor(private viewModelFactory: ViewModelFactoryService) { }

    reset(menusRep: Models.MenusRepresentation, routeData: PaneRouteData) {
        this.menusRep = menusRep;
        this.onPaneId = routeData.paneId;
        this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
        return this;
    }

    menusRep: Models.MenusRepresentation;
    onPaneId: number;
    items: LinkViewModel[];
}