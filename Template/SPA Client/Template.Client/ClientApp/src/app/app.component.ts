import { Component } from '@angular/core';
import { AuthService, ConfigService, UrlManagerService } from '@nakedobjects/services';

@Component({
   
    // eslint-disable-next-line @angular-eslint/component-selector
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css'],
    standalone: false
})

export class AppComponent {
    constructor(public readonly auth: AuthService,
                private readonly urlManager: UrlManagerService,
                public readonly config: ConfigService) {
        auth.handleAuthentication();
     }

    isGemini = () => this.urlManager.isGemini();
}
