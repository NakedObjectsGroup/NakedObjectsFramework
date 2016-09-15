import { Component, Input } from '@angular/core';
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'actions',
    templateUrl: 'app/actions.component.html',
    styleUrls: ['app/actions.component.css']
})
export class ActionsComponent {

    @Input()
    menuVm: { menuItems: ViewModels.IMenuItemViewModel[] };
}