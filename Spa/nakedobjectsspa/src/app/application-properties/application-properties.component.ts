import { Component, OnInit } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import * as ViewModels from "../view-models";
import * as Models from '../models';
import * as Nakedobjectsconfig from '../config';
import { ErrorService } from "../error.service";

@Component({
    selector: 'recent',
    templateUrl: './application-properties.component.html',
    styleUrls: ['./application-properties.component.css']
})
export class ApplicationPropertiesComponent implements OnInit {

    constructor(private context: ContextService, private error: ErrorService) {
    }

    get userName() {
        return this.viewModel.user ? this.viewModel.user.userName : "";
    }

    get serverUrl() {
        return this.viewModel.serverUrl;
    }

    get implVersion() {
        return this.viewModel.serverVersion ? this.viewModel.serverVersion.implVersion : "";
    }

    get clientVersion() {
        return "todo";
    }

    viewModel: ViewModels.ApplicationPropertiesViewModel;

    ngOnInit(): void {

        this.viewModel = new ViewModels.ApplicationPropertiesViewModel();

        this.context.getUser().
            then(u => this.viewModel.user = u.wrapped()).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.context.getVersion().
            then(v => this.viewModel.serverVersion = v.wrapped()).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.viewModel.serverUrl = Nakedobjectsconfig.getAppPath();

        // apvm.clientVersion = (NakedObjects as any)["version"] || "Failed to write version";

    }
}
