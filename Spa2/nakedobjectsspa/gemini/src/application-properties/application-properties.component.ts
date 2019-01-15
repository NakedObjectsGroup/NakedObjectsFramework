import { Component, OnInit } from '@angular/core';
import { AuthService } from '@nakedobjects/services';
import { ApplicationPropertiesViewModel, ViewModelFactoryService } from '@nakedobjects/view-models';
import * as Msg from '../user-messages';

@Component({
    selector: 'nof-application-properties',
    templateUrl: 'application-properties.component.html',
    styleUrls: ['application-properties.component.css']
})
export class ApplicationPropertiesComponent implements OnInit {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly authService: AuthService
    ) {
    }

    get applicationName() {
        return this.applicationProperties.applicationName;
    }

    get userName() {
        return this.applicationProperties.user ? this.applicationProperties.user.userName : Msg.noUserMessage;
    }

    get serverUrl() {
        return this.applicationProperties.serverUrl;
    }

    get implVersion() {
        return this.applicationProperties.serverVersion ? this.applicationProperties.serverVersion.implVersion : '';
    }

    get clientVersion() {
        return this.applicationProperties.clientVersion;
    }

    private applicationProperties: ApplicationPropertiesViewModel;

    ngOnInit(): void {
        this.applicationProperties = this.viewModelFactory.applicationPropertiesViewModel();
    }
}
