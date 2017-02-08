import { Component, OnInit } from '@angular/core';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ApplicationPropertiesViewModel } from '../view-models/application-properties-view-model';

@Component({
    selector: 'nof-application-properties',
    templateUrl: './application-properties.component.html',
    styleUrls: ['./application-properties.component.css']
})
export class ApplicationPropertiesComponent implements OnInit {

    constructor(
        private readonly viewModelFactory : ViewModelFactoryService) {
    }

    get userName() {
        return this.applicationProperties.user ? this.applicationProperties.user.userName : "'No user set'";
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