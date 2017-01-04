import { Component, OnInit, Input } from '@angular/core';
import { IButton } from '../button/button.component';

@Component({
    selector: 'nof-buttons',
    templateUrl: './buttons.component.html',
    styleUrls: ['./buttons.component.css']
})
export class ButtonsComponent {

    @Input()
    buttons: IButton[];
}
