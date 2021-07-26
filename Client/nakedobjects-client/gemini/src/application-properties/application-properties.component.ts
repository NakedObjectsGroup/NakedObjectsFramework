import { Component, OnInit } from '@angular/core';
import { ApplicationPropertiesViewModel, ViewModelFactoryService } from '@nakedobjects/view-models';
import { clientVersion } from '../version';

@Component({
    selector: 'nof-application-properties',
    templateUrl: 'application-properties.component.html',
    styleUrls: ['application-properties.component.css']
})
export class ApplicationPropertiesComponent implements OnInit {

    constructor(private readonly viewModelFactory: ViewModelFactoryService) { }

    get applicationName() {
        return this.applicationProperties.applicationName;
    }

    get userName() {
        return this.applicationProperties.userName;
    }

    get serverUrl() {
        return this.applicationProperties.serverUrl;
    }

    get implVersion() {
        return this.applicationProperties.serverVersion ? this.applicationProperties.serverVersion.implVersion : '';
    }

    get apiVersion() {
        return this.applicationProperties.serverVersion ? this.applicationProperties.serverVersion.specVersion : '';
    }

    get clientVersion() {
        return clientVersion;
    }

    private applicationProperties: ApplicationPropertiesViewModel;

    ngOnInit(): void {
        this.applicationProperties = this.viewModelFactory.applicationPropertiesViewModel();
    }
}
