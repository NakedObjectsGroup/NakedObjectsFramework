import { MessageViewModel } from './message-view-model';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import * as Models from '../models';
import * as Viewmodelfactoryservice from '../view-model-factory.service';
import * as Routedata from '../route-data';
import * as _ from "lodash";
import * as Helpers from './helpers-view-models';

export class MenuViewModel extends MessageViewModel {

    constructor(private viewModelFactory: Viewmodelfactoryservice.ViewModelFactoryService,
        public menuRep: Models.MenuRepresentation,
        routeData: Routedata.PaneRouteData) {
        super();

        this.id = menuRep.menuId();


        const actions = menuRep.actionMembers();
        this.title = menuRep.title();
        this.actions = _.map(actions, action => viewModelFactory.actionViewModel(action, this, routeData));
        this.menuItems = Helpers.createMenuItems(this.actions);
    }


    id: string;
    title: string;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];

}