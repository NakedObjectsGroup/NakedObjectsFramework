import { Component } from '@angular/core';
import { HomeComponent } from "./home.component";

@Component({
    selector: 'single-home',
    template: '<div class="single"><home [paneId]="1"></home></div>',
    directives: [HomeComponent]
})
export class SingleHomeComponent {
    id: string;
}