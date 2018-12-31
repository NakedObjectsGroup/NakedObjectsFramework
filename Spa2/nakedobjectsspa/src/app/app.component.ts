import { Component } from '@angular/core';
import { AuthService } from '@nakedobjects/services';
import { UrlManagerService } from '@nakedobjects/services';
import { ConfigService } from '@nakedobjects/services';

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
        auth.handleAuthentication();
     }

    isGemini = () =>  this.urlManager.isGemini();
}
