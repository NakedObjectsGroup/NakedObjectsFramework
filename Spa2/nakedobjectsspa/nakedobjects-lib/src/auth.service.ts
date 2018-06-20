import { LogoffComponent } from './logoff/logoff.component';
import { Injectable } from '@angular/core';
import { Router, NavigationStart, CanActivate } from '@angular/router';
import { UrlManagerService } from './url-manager.service';
import { LoggerService } from './logger.service';
import { ConfigService } from './config.service';
// import Auth0Lock from 'auth0-lock';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter } from 'rxjs/operators';

@Injectable()
export class AuthService  implements CanActivate {

    private pendingAuthenticate = false;
    // private lockInstance: Auth0LockStatic | undefined;
    private lockInstance = undefined;

    getAuthorizationHeader(): string {
        // todo
        // if (this.authenticated()){
        //     return new HttpHeaders({"Bearer": localStorage.getItem('id_token')})
        // }
        const token = localStorage.getItem('id_token') || "";
        return `Bearer ${token}`;
    }

    private get authenticate() {
        return this.configService.config.authenticate;
    }

    private get lock() {
        const clientId = this.configService.config.authClientId;
        const domain = this.configService.config.authDomain;

        if (this.authenticate && clientId && domain && !this.lockInstance) {

            // this.lockInstance = new Auth0Lock(clientId, domain, {
            //     oidcConformant: true,
            //     autoclose: true,
            //     auth: {
            //       // redirectUrl: 'http://',
            //       responseType: 'token id_token',
            //       audience: `https://${domain}/api/v2/`,
            //       params: {
            //         scope: 'openid email profile'
            //       }
            //     }
            //   });
        }
        return undefined; // this.lockInstance;
    }

    constructor(
        private readonly router: Router,
        private readonly urlManager: UrlManagerService,
        private readonly logger: LoggerService,
        private readonly configService: ConfigService
    ) {
    }

    public handleAuthenticationWithHash(): void {
        if (this.authenticate && this.lock) {
            this
                .router
                .events
                .pipe(
                  filter(event => event instanceof NavigationStart),
                  filter((event: NavigationStart) => (/access_token|id_token|error/).test(event.url)))
                .subscribe(() => {
                    this.lock!.resumeAuth(window.location.hash, (err, authResult) => {
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
                                this.urlManager.setHomeSinglePane();
                            });
                        }
                    });
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

    login() {
        // Call the show method to display the widget.
        if (this.lock) {
            this.lock.show();
        }
    }

    authenticated() {
        if (this.authenticate) {
            // Check whether the current time is past the
            // access token's expiry time
            const expiresAtItem = localStorage.getItem('expires_at');
            if (expiresAtItem) {
                const expiresAt = JSON.parse(expiresAtItem);
                return new Date().getTime() < expiresAt;
            }
            return false;
        }
        return true;
    }

    logout() {
        if (this.authenticate) {
            // Remove token from localStorage
            // Remove tokens and expiry time from localStorage
            localStorage.removeItem('access_token');
            localStorage.removeItem('id_token');
            localStorage.removeItem('expires_at');
            // Go back to the home route
            this.router.navigate(['/']);
        }
    }

    canActivate() {
        if (this.authenticate) {
            return !this.pendingAuthenticate && this.authenticated();
        }
        return true;
    }

    canDeactivate(component: LogoffComponent) {
        if (this.authenticate) {
            return !component.isActive;
        }
        return true;
    }

    userIsLoggedIn() {
        if (this.authenticate) {
            return this.authenticated();
        }
        return false;
    }
}

// @Injectable()
// export class NullAuthService extends AuthService implements CanActivate {
//     getAuthorizationHeader(): string {
//         throw new Error("Method not implemented.");
//     }

//     login() { }

//     authenticated() {
//         return true;
//     }

//     logout() { }

//     canActivate() {
//         return true;
//     }

//     canDeactivate(component: LogoffComponent) {
//         return true;
//     }

//     userIsLoggedIn() {
//         return false;
//     }

//     handleAuthenticationWithHash() { }
// }
