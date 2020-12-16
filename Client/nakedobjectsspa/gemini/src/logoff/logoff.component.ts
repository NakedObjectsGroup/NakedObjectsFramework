﻿import { Location } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService, ConfigService, ContextService } from '@nakedobjects/services';

@Component({
    selector: 'nof-logoff',
    templateUrl: 'logoff.component.html',
    styleUrls: ['logoff.component.css']
})
export class LogoffComponent implements OnInit {

    constructor(
        private readonly context: ContextService,
        private readonly authService: AuthService,
        readonly configService: ConfigService,
        private readonly http: HttpClient,
        private readonly location: Location,
    ) { }

    userId: string;

    isActive = true;

    userIsLoggedIn() {
        return this.authService.isAuthenticated();
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

            const args = {
                withCredentials: true
            };

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
        this.context.getUser().then(u => this.userId = u.userName() || 'Unknown');
    }
}
