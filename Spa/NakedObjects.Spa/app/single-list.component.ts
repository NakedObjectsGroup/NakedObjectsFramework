import { Component } from '@angular/core';
import { HomeComponent } from "./home.component";
import { ListComponent } from "./list.component";

@Component({
    selector: 'single-object',
    template: '<div class="single"><list [paneId]="1"></list></div>',
    directives: [ListComponent]
})
export class SingleListComponent {
    id: string;
}