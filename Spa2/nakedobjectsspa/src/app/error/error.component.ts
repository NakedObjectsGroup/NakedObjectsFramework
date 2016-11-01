import { Component } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import * as ViewModels from "../view-models";

@Component({
    selector: 'error',
    templateUrl: './error.component.html'
})
export class ErrorComponent {

    constructor(private context : ContextService, private viewModelFactory : ViewModelFactoryService) {  }

    error : ViewModels.ErrorViewModel;

    ngOnInit(): void {
        const errorWrapper = this.context.getError();
        this.error = this.viewModelFactory.errorViewModel(errorWrapper);
    }
}
