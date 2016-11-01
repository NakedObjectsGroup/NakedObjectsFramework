import { Component, Input } from '@angular/core';
import * as ViewModels from "../view-models";

@Component({
    selector: 'actions',
    templateUrl: './actions.component.html',
    styleUrls: ['./actions.component.css']
})
export class ActionsComponent {

    @Input()
    menuVm: { menuItems: ViewModels.IMenuItemViewModel[] };
}