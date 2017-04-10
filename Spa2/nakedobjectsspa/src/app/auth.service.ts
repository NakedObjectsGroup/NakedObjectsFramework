import { Injectable } from '@angular/core';
import { tokenNotExpired } from 'angular2-jwt';
import { Router, NavigationStart, CanActivate } from '@angular/router';
import './rxjs-extensions';
import { UrlManagerService } from './url-manager.service';
import { LoggerService } from './logger.service';
import { ConfigService } from './config.service';


export abstract class AuthService {

    abstract login(): void;

    abstract authenticated(): boolean;

    abstract logout(): void;

    abstract canActivate(): boolean;
}


@Injectable()
export class Auth0AuthService extends AuthService implements CanActivate {

    // Configure Auth0
    // this is client id which is public 
    private readonly lock;

    //Store profile object in auth class
    private userProfile: any; // todo fix

    constructor(
        private readonly router: Router,
        private readonly urlManager: UrlManagerService,
        private readonly logger: LoggerService,
        private readonly configService: ConfigService
    ) {
        super();

        const options = {
            auth: {
                params: { scope: 'openid email' },
            }
        };

        this.lock = new Auth0Lock(configService.config.authClientId, configService.config.authDomain, options);

        // Set userProfile attribute of already saved profile
        this.userProfile = JSON.parse(localStorage.getItem('profile'));

        // Add callback for lock `authenticated` event
        this.lock.on("authenticated", (authResult) => {
            localStorage.setItem('id_token', authResult.idToken);
        });

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

    login() {
        // Call the show method to display the widget.
        this.lock.show();
    }

    authenticated() {
        // Check if there's an unexpired JWT
        // This searches for an item in localStorage with key == 'id_token'
        return tokenNotExpired();
    }

    logout() {
        // Remove token from localStorage
        // todo make keys consts 
        localStorage.removeItem('id_token');
        localStorage.removeItem('profile');
        this.userProfile = undefined;
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
