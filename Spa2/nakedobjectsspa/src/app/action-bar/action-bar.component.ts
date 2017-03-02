import { Component, OnInit, Input } from '@angular/core';
import { IActionHolder } from '../action/action.component';
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
    set menuHolder (mh : IMenuHolderViewModel) {

        // todo DRY 

        const menuItems = mh.menuItems;
        const acts = _.flatten(_.map(menuItems, (mi: MenuItemViewModel) => mi.actions!));

        this.actions = _.map(acts,
            a => ({
                value: a.title,
                doClick: () => a.doInvoke(),
                doRightClick: () => a.doInvoke(true),
                show: () => true,
                disabled: () => a.disabled() ? true : null,
                tempDisabled: () => a.tempDisabled(),
                title: () => a.description,
                accesskey: null
            })) as IActionHolder[];
    }
}
