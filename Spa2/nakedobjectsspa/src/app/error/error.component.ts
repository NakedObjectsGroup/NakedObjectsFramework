import { Component } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ErrorViewModel } from '../view-models/error-view-model';

@Component({
    selector: 'error',
    templateUrl: './error.component.html',
    styleUrls: ['./error.component.css']
})
export class ErrorComponent {

    constructor(private context: ContextService, private viewModelFactory: ViewModelFactoryService) { }

    // template API 

    title: string;
    message: string;
    errorCode: string;
    description: string;
    stackTrace: string[];

    ngOnInit(): void {
        // todo if no error perhaps just go home ? 
        // this would happen on error page reload 

        // todo - in case it gets missed - need to fix angular 2 error handling so it displays this page 
        // rather than just logs to console. 

        const errorWrapper = this.context.getError();
        const error = this.viewModelFactory.errorViewModel(errorWrapper);

        this.title = error.title;
        this.message = error.message;
        this.errorCode = error.errorCode;
        this.description = error.description;
        this.stackTrace = error.stackTrace;
    }
}