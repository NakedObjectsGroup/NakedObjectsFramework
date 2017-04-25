import { ContextService } from '../context.service';
import { Component, OnInit } from '@angular/core';
import { ConfigService } from '../config.service';
import { AuthService } from '../auth.service';
import { Http, RequestOptionsArgs } from '@angular/http';
import { UrlManagerService } from '../url-manager.service';
import { Location } from '@angular/common';

@Component({
    selector: 'nof-logoff',
    template: require('./logoff.component.html'),
    styles: [require('./logoff.component.css')]
})
export class LogoffComponent implements OnInit {

    constructor(
        private readonly context: ContextService,
        private readonly authService: AuthService,
        private readonly configService: ConfigService,
        private readonly http: Http,
        private readonly urlManager: UrlManagerService,
        private readonly location: Location,
    ) { }

    userId: string;

    isActive = true;

    userIsLoggedIn() {
        return this.authService.userIsLoggedIn();
    }

    cancel() {
        this.isActive = false;
        this.location.back();
    }

    logoff() {
        this.isActive = false;
        const serverLogoffUrl = this.configService.config.logoffUrl;
        const postLogoffUrl = this.configService.config.postLogoffUrl;

        if (serverLogoffUrl) {

            const args: RequestOptionsArgs = {
                withCredentials: true
            }

            this.http.post(this.configService.config.logoffUrl, args);
        }

        // logoff client without waiting for server
        this.authService.logout();

        // if set this will reload page and cause all cached data to be lost.  
        if (postLogoffUrl) {
            this.context.clearingDataFlag = true;
            window.location.href = postLogoffUrl;
        }
    }

    ngOnInit() {
        this.context.getUser().then(u => this.userId = u.userName() || "Unknown");
    }
}
