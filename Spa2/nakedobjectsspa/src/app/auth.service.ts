import { LogoffComponent } from './logoff/logoff.component';
import { Injectable } from '@angular/core';
import { tokenNotExpired } from 'angular2-jwt';
import { Router, NavigationStart, CanActivate } from '@angular/router';
import { UrlManagerService } from './url-manager.service';
import { LoggerService } from './logger.service';
import { ConfigService } from './config.service';
import Auth0Lock from 'auth0-lock';


export abstract class AuthService {

    abstract login(): void;

    abstract authenticated(): boolean;

    abstract logout(): void;

    abstract canActivate(): boolean;

    abstract userIsLoggedIn() : boolean;

    abstract handleAuthenticationWithHash() : void;
}


@Injectable()
export class Auth0AuthService extends AuthService implements CanActivate {

    private readonly lock: Auth0LockStatic;

    private pendingAuthenticate : boolean = false;

    constructor(
        private readonly router: Router,
        private readonly urlManager: UrlManagerService,
        private readonly logger: LoggerService,
        private readonly configService: ConfigService
    ) {
        super();

        const clientId = configService.config.authClientId;
        const domain = configService.config.authDomain;
        const authenticate = configService.config.authenticate;

        if (authenticate && clientId && domain) {

            this.lock = new Auth0Lock(clientId, domain, {
                oidcConformant: true,
                autoclose: true,
                auth: {
                  //redirectUrl: 'http://',
                  responseType: 'token id_token',
                  audience: `https://${domain}/api/v2/`,
                  params: {
                    scope: 'openid email'
                  }
                }
              });
        }
    }

    public handleAuthenticationWithHash(): void {
        this
            .router
            .events
            .filter(event => event instanceof NavigationStart)
            .filter((event: NavigationStart) => (/access_token|id_token|error/).test(event.url))
            .subscribe(() => {
                this.lock.resumeAuth(window.location.hash, (err, authResult) => {
                    if (err) {
                        this.urlManager.setHomeSinglePane();
                        console.log(err);
                        alert(`Error: ${err.error}. Check the console for further details.`);
                        return;
                    }
                    if (authResult) {
                        // some sort of race here with token response navigating us to a page,
                        // we're making auth OK with token but app.component doesn't yet have router-outlet 
                        // so we see errors. Set the pending Authenticate flag which will make it look like 
                        // we're not authenticated and then clear and route home on next event loop.
                        this.setSession(authResult);
                        this.pendingAuthenticate = true;
                        setTimeout(() => {
                            this.pendingAuthenticate = false;
                            this.urlManager.setHomeSinglePane()
                        });
                    }
                });
            });
    }

    private setSession(authResult: any): void {
        // Set the time that the access token will expire at
        const expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
        localStorage.setItem('access_token', authResult.accessToken);
        localStorage.setItem('id_token', authResult.idToken);
        localStorage.setItem('expires_at', expiresAt);
    }

    login() {
        // Call the show method to display the widget.
        if (this.lock) {
            this.lock.show();
        }
    }

    authenticated() {
        // Check whether the current time is past the
        // access token's expiry time
        const expiresAtItem = localStorage.getItem('expires_at');
        if (expiresAtItem) {
            const expiresAt = JSON.parse(expiresAtItem);
            return new Date().getTime() < expiresAt;
        }
        return false;
    }

    logout() {
        // Remove token from localStorage
        // Remove tokens and expiry time from localStorage
        localStorage.removeItem('access_token');
        localStorage.removeItem('id_token');
        localStorage.removeItem('expires_at');
        // Go back to the home route
        this.router.navigate(['/']);
    }

    canActivate() {
        return !this.pendingAuthenticate && this.authenticated();
    }

    canDeactivate(component: LogoffComponent) {
        return !component.isActive;
    }

    userIsLoggedIn() {
        return this.authenticated();
    }
}

@Injectable()
export class NullAuthService extends AuthService implements CanActivate {

    login() { }

    authenticated() {
        return true;
    }

    logout() { }

    canActivate() {
        return true;
    }

    canDeactivate(component: LogoffComponent) {
        return true;
    }

    userIsLoggedIn() {
        return false;
    }

    handleAuthenticationWithHash() {}
}
