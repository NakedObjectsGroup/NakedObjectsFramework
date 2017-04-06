import { Component } from '@angular/core';
import { AuthService } from './auth.service';

@Component({
    selector: 'app-root',
    template: require('./app.component.html'),
    styles: [require('./app.component.css')]
})

export class AppComponent {
    constructor(private auth: AuthService) { }
}