import { ViewModelFactoryService } from '../view-model-factory.service';
import { PaneRouteData } from '../route-data';
import { LinkViewModel } from './link-view-model';
import * as Models from '../models';
import * as _ from 'lodash';

export class MenusViewModel {
    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly menusRep: Models.MenusRepresentation,
        onPaneId : number
    ) {
        this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, onPaneId));
    }

    readonly items: LinkViewModel[];
}