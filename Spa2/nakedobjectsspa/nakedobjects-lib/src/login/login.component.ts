import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { ConfigService } from '../config.service';
import { ContextService } from '../context.service';

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
