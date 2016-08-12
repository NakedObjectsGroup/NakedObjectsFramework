import { Component } from '@angular/core';
import { HomeComponent } from "./home.component";

@Component({
    selector: 'split-home',
    template: '<div class="split"><home [paneId]="1"></home></div><div class="split"><home [paneId]="2"></home></div>',
    directives: [HomeComponent]
})

export class SplitHomeComponent {
    id: string;
}