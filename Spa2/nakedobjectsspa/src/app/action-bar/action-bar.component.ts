import { Component, OnInit, Input } from '@angular/core';
import { IActionHolder, wrapAction } from '../action/action.component';
import { IMenuHolderViewModel} from '../view-models/imenu-holder-view-model';
import { MenuItemViewModel} from '../view-models/menu-item-view-model';

@Component({
    selector: 'nof-action-bar',
    template: require('./action-bar.component.html'),
    styles: [require('./action-bar.component.css')]
})
export class ActionBarComponent {

    @Input()
    actions: IActionHolder[];

    @Input()
    set menuHolder (mhvm : IMenuHolderViewModel) {
        const menuItems = mhvm.menuItems;
        const avms = _.flatten(_.map(menuItems, (mi: MenuItemViewModel) => mi.actions!));
        this.actions = _.map(avms, a => wrapAction(a));
    }
}
