import { Component } from '@angular/core';
import { Context } from "./context.service";
import { ViewModelFactory } from "./view-model-factory.service";
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'recent',
    templateUrl: 'app/recent.component.html'
})
export class RecentComponent {

    constructor(private context : Context, private viewModelFactory : ViewModelFactory) {  }

    error : ViewModels.ErrorViewModel;

    ngOnInit(): void {
        const errorWrapper = this.context.getError();
        this.error = this.viewModelFactory.errorViewModel(errorWrapper);
    }
}
