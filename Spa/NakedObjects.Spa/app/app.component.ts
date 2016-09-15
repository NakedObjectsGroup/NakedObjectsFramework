import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'my-app',
    templateUrl: 'app/app.component.html'
})
export class AppComponent implements OnInit {

    backgroundColor: string;

    ngOnInit(): void {
        // todo
        this.backgroundColor = "object-color0";
    }
}