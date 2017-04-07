import { Injectable } from '@angular/core';
import { tokenNotExpired } from 'angular2-jwt';
import { Router, NavigationStart } from '@angular/router';
import './rxjs-extensions';
import { UrlManagerService } from './url-manager.service';
import {LoggerService} from './logger.service';

// todo just use auth0 type 
export interface IUserProfile {
    name: string;
    email?: string;
    nickname: string;
    created_at : string;
    updated_at : string;
}

@Injectable()
export class AuthService {


    private options = {
        auth: {
            params: { scope: 'email' },
        }
    };  

    // Configure Auth0
    // this is client id which is public 
    private readonly lock = new Auth0Lock('wXXvvJOtNZoycQtK1gwxAjYgXDzQv5K9', 'nakedobjects.eu.auth0.com', this.options);

    //Store profile object in auth class
    userProfile: IUserProfile;

    constructor(
        private readonly router: Router,
        private readonly urlManager: UrlManagerService,
        private readonly logger: LoggerService
    ) {
        // Set userProfile attribute of already saved profile
        this.userProfile = JSON.parse(localStorage.getItem('profile'));

        // Add callback for lock `authenticated` event
        this.lock.on("authenticated", (authResult) => {
            localStorage.setItem('id_token', authResult.idToken);

            // Fetch profile information
            this.lock.getProfile(authResult.idToken, (error, profile) => {
                if (error) {
                    logger.error(error);
                }
                else if (profile) {
                    localStorage.setItem('profile', JSON.stringify(profile));
                    this.userProfile = profile;
                }
            });
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
                    else if (authResult) {
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
}

@Injectable()
export class NullAuthService {

    userProfile: IUserProfile;

    login() { }

    authenticated() {
        return true;
    }

    logout() { }
}
