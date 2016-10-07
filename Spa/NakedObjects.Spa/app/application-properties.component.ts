import { Component, OnInit } from '@angular/core';
import { Context } from "./context.service";
import { ViewModelFactory } from "./view-model-factory.service";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Models from './models';
import * as Nakedobjectsconfig from './nakedobjects.config';
import { Error } from "./error.service";

@Component({
    selector: 'recent',
    templateUrl: 'app/application-properties.component.html'
})
export class ApplicationPropertiesComponent implements OnInit {

    constructor(private context: Context, private error: Error) {
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
