import { Component, OnInit } from '@angular/core';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';

@Component({
    selector: 'nof-error',
    templateUrl: 'error.component.html',
    styleUrls: ['error.component.css']
})
export class ErrorComponent implements OnInit {

    constructor(
        private readonly context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService
    ) { }

    // template API

    title: string;
    message: string;
    errorCode: string;
    description: string;
    stackTrace: string[] | null;

    ngOnInit(): void {
        // expect dynamic-error to  have checked if the context has an error
        const errorWrapper = this.context.getError();
        const error = this.viewModelFactory.errorViewModel(errorWrapper);

        this.title = error.title;
        this.message = error.message;
        this.errorCode = error.errorCode;
        this.description = error.description;
        this.stackTrace = error.stackTrace;
    }
}
