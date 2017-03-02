import { Component, OnInit, Input } from '@angular/core';
import { IActionHolder } from '../action/action.component';

@Component({
    selector: 'nof-action-bar',
    template: require('./action-bar.component.html'),
    styles: [require('./action-bar.component.css')]
})
export class ActionBarComponent {

    @Input()
    actions: IActionHolder[];

}
