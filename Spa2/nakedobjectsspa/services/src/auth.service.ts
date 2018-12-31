import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import * as auth0 from 'auth0-js';
import { ConfigService } from './config.service';
import { LoggerService } from './logger.service';
import { UrlManagerService } from './url-manager.service';

(window as any).global = window;

@Injectable()
export class AuthService  implements CanActivate {

    getAuthorizationHeader(): string {
        const token = localStorage.getItem('id_token') || '';
        return `Bearer ${token}`;
    }

    private get authenticate() {
        return this.configService.config.authenticate;
    }

    private get auth0() {
        const clientID = this.configService.config.authClientId;
        const domain = this.configService.config.authDomain;
        const url = (window as any).location.origin;
        const redirectUrl = `${url}/gemini/callback`;

        return new auth0.WebAuth({
            clientID,
            domain,
            responseType: 'token id_token',
            audience: `https://${domain}/userinfo`,
            redirectUri: redirectUrl,
            scope: 'openid email profile'
        });
    }

    constructor(
        private readonly router: Router,
        private readonly urlManager: UrlManagerService,
        private readonly logger: LoggerService,
        private readonly configService: ConfigService
    ) {
    }

    public login(): void {
        this.auth0.authorize();
    }

    public handleAuthentication(): void {
        if (this.authenticate) {
            this.auth0.parseHash((err, authResult) => {
                if (authResult && authResult.accessToken && authResult.idToken) {
                  this.setSession(authResult);
                  this.urlManager.setHomeSinglePane();
                } else if (err) {
                    this.urlManager.setHomeSinglePane();
                    console.log(err);
                    alert(`Error: ${err.error}. Check the console for further details.`);
                }
              });
        }
    }

    private setSession(authResult: any): void {
        if (this.authenticate) {
            // Set the time that the access token will expire at
            const expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
            localStorage.setItem('access_token', authResult.accessToken);
            localStorage.setItem('id_token', authResult.idToken);
            localStorage.setItem('expires_at', expiresAt);
        }
    }

    logout() {
        if (this.authenticate) {
             // Remove tokens and expiry time from localStorage
            localStorage.removeItem('access_token');
            localStorage.removeItem('id_token');
            localStorage.removeItem('expires_at');
            // Go back to the home route
            this.urlManager.setHomeSinglePane();
        }
    }

    canActivate() {
        if (this.authenticate) {
            return this.isAuthenticated();
        }
        return true;
    }

    canDeactivate(component: { isActive: boolean }) {
        if (this.authenticate) {
            return !component.isActive;
        }
        return true;
    }

    public isAuthenticated(): boolean {
        // Check whether the current time is past the
        // access token's expiry time
        const expiresAt = JSON.parse(localStorage.getItem('expires_at') || '{}');
        return new Date().getTime() < expiresAt;
    }
}
