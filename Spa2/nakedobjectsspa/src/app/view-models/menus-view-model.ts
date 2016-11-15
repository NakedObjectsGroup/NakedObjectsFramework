import * as Viewmodelfactoryservice from '../view-model-factory.service';
import * as Models from '../models';
import * as Routedata from '../route-data';
import * as Linkviewmodel from './link-view-model';
import * as _ from "lodash";

export class MenusViewModel {
    constructor(private viewModelFactory: Viewmodelfactoryservice.ViewModelFactoryService) {}

    reset(menusRep: Models.MenusRepresentation, routeData: Routedata.PaneRouteData) {
        this.menusRep = menusRep;
        this.onPaneId = routeData.paneId;
        this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
        return this;
    }

    menusRep: Models.MenusRepresentation;
    onPaneId: number;
    items: Linkviewmodel.LinkViewModel[];
}