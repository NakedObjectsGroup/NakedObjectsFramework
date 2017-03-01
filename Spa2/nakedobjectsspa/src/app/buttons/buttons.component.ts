import { Component, OnInit, Input } from '@angular/core';
import { IButton } from '../action/action.component';
import * as Actioncomponent from '../action/action.component';

@Component({
    selector: 'nof-buttons',
    template: require('./buttons.component.html'),
    styles: [require('./buttons.component.css')]
})
export class ButtonsComponent {

    @Input()
    buttons: Actioncomponent.IButton[];
}
