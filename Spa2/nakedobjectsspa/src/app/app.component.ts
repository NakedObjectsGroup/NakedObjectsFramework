import { Component } from '@angular/core';
import { AuthService } from './auth.service';
import { UrlManagerService } from './url-manager.service';

@Component({
    selector: 'app-root',
    template: require('./app.component.html'),
    styles: [require('./app.component.css')]
})

export class AppComponent {
    constructor(public readonly auth: AuthService, private readonly urlManager : UrlManagerService) { }

    isGemini = () =>  this.urlManager.isGemini();
}