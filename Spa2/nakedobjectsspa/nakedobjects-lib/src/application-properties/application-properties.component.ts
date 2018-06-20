import { Component, OnInit } from '@angular/core';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ApplicationPropertiesViewModel } from '../view-models/application-properties-view-model';
import * as Msg from '../user-messages';
import { AuthService } from '../auth.service';

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
        return this.applicationProperties.serverVersion ? this.applicationProperties.serverVersion.implVersion : "";
    }

    get clientVersion() {
        return this.applicationProperties.clientVersion;
    }

    private applicationProperties: ApplicationPropertiesViewModel;

    ngOnInit(): void {
        this.applicationProperties = this.viewModelFactory.applicationPropertiesViewModel();
    }
}
