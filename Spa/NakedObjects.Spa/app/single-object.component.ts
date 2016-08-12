import { Component } from '@angular/core';
import { HomeComponent } from "./home.component";
import { ObjectComponent } from "./object.component";

@Component({
    selector: 'single-object',
    template: '<div class="single"><object [paneId]="1"></object></div>',
    directives: [ObjectComponent]
})

export class SingleObjectComponent {
    id : string;
}