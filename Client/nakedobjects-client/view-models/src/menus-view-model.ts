import * as Ro from '@nakedobjects/restful-objects';
import { Pane } from '@nakedobjects/services';
import map from 'lodash-es/map';
import { LinkViewModel } from './link-view-model';
import { ViewModelFactoryService } from './view-model-factory.service';

export class MenusViewModel {
    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly menusRep: Ro.MenusRepresentation,
        onPaneId: Pane
    ) {
        this.items = map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, onPaneId));
    }

    readonly items: LinkViewModel[];
}
