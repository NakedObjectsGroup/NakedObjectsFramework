import { MessageViewModel } from './message-view-model';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { PaneRouteData } from '../route-data';
import * as Helpers from './helpers-view-models';
import * as Models from '../models';
import * as _ from 'lodash';

export class MenuViewModel extends MessageViewModel {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        public readonly menuRep: Models.MenuRepresentation,
        public readonly routeData: PaneRouteData
    ) {
        super();

        this.id = menuRep.menuId();

        const actions = menuRep.actionMembers();
        this.title = menuRep.title();
        this.actions = _.map(actions, action => viewModelFactory.actionViewModel(action, this, routeData));
        this.menuItems = Helpers.createMenuItems(this.actions);
    }

    private readonly id: string;
    private readonly title: string;
    private readonly actions: ActionViewModel[];
    private readonly menuItems: MenuItemViewModel[];
}