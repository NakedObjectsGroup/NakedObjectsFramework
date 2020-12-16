import * as Ro from '@nakedobjects/restful-objects';
import { PaneRouteData } from '@nakedobjects/services';
import map from 'lodash-es/map';
import { ActionViewModel } from './action-view-model';
import * as Helpers from './helpers-view-models';
import { IMenuHolderViewModel } from './imenu-holder-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { MessageViewModel } from './message-view-model';
import { ViewModelFactoryService } from './view-model-factory.service';

export class MenuViewModel extends MessageViewModel implements IMenuHolderViewModel {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        public readonly menuRep: Ro.MenuRepresentation,
        public readonly routeData: PaneRouteData
    ) {
        super();

        this.id = menuRep.menuId();

        const actions = menuRep.actionMembers();
        this.title = menuRep.title();
        this.actions = map(actions, action => viewModelFactory.actionViewModel(action, this, routeData)).filter(avm => !avm.returnsScalar());
        this.menuItems = Helpers.createMenuItems(this.actions);
    }

    private readonly id: string;
    private readonly title: string;
    private readonly actions: ActionViewModel[];
    readonly menuItems: MenuItemViewModel[];
}
