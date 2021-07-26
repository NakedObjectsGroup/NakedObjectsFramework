import { Component } from '@angular/core';
import { AuthService, ConfigService, ContextService } from '@nakedobjects/services';

@Component({
    selector: 'nof-login',
    templateUrl: 'login.component.html',
    styleUrls: ['login.component.css']
})
export class LoginComponent {

    constructor(
        public readonly context: ContextService,
        public readonly auth: AuthService,
        public readonly configService: ConfigService
    ) { }
}
