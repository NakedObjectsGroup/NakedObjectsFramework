import { Component, OnInit, Input } from '@angular/core';
import { IButton } from '../button/button.component';

@Component({
    selector: 'nof-buttons',
    template: require('./buttons.component.html'),
    styles: [require('./buttons.component.css')]
})
export class ButtonsComponent {

    @Input()
    buttons: IButton[];
}
