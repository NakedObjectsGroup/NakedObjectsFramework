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
}


@Injectable()
export class Auth0AuthService extends AuthService implements CanActivate {

    private readonly lock: Auth0LockStatic;

    constructor(
        private readonly router: Router,
        private readonly urlManager: UrlManagerService,
        private readonly logger: LoggerService,
        private readonly configService: ConfigService
    ) {
        super();

        const clientId = configService.config.authClientId;
        const domain = configService.config.authDomain;

        if (clientId && domain) {

            const options = {
                auth: {
                    params: { scope: 'openid email' },
                }
            };

            // Configure Auth0
            // this is client id which is public 
            this.lock = new Auth0Lock(clientId, domain, options);

            // Add callback for lock `authenticated` event
            this.lock.on("authenticated", authResult => localStorage.setItem('id_token', authResult.idToken));

            this
                .router
                .events
                .filter(event => event instanceof NavigationStart)
                .filter((event: NavigationStart) => (/access_token|id_token|error/).test(event.url))
                .subscribe(() => {
                    this.lock.resumeAuth(window.location.hash, (error: any, authResult: any) => {
                        if (error) {
                            logger.error(error);
                        }
                        else if (authResult && authResult.idToken) {
                            localStorage.setItem('id_token', authResult.idToken);
                            setTimeout(() => this.urlManager.setHomeSinglePane());
                        }
                    });
                });
        }
    }

    login() {
        // Call the show method to display the widget.
        if (this.lock) {
            this.lock.show();
        }
    }

    authenticated() {
        // Check if there's an unexpired JWT
        // This searches for an item in localStorage with key == 'id_token'
        return tokenNotExpired();
    }

    logout() {
        // Remove token from localStorage
        localStorage.removeItem('id_token');
    }

    canActivate() {
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
}
