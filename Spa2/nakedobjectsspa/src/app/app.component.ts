import { Component } from '@angular/core';
import { AuthService } from './auth.service';
import { UrlManagerService } from './url-manager.service';
import { ConfigService } from './config.service';

@Component({
    // tslint:disable-next-line:component-selector
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})

export class AppComponent {
    constructor(public readonly auth: AuthService,
                private readonly urlManager: UrlManagerService,
                public readonly config: ConfigService) {
        auth.handleAuthenticationWithHash();
     }

    isGemini = () =>  this.urlManager.isGemini();
}
