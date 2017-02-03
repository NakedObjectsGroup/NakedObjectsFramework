import { UrlManagerService } from "./url-manager.service";
import { ContextService } from "./context.service";
import * as Models from "./models";

export type errorPreprocessor = (reject: Models.ErrorWrapper) => void;
//export type errorDisplayHandler = ($scope: INakedObjectsScope) => void;
import { Injectable } from '@angular/core';

@Injectable()
export class ErrorService {

    constructor(
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService
    ) { }

    private preProcessors: errorPreprocessor[] = [];
    // todo later should we reimplement ?
    //const displayHandlers: errorDisplayHandler[] = [];

    private handleHttpServerError(reject: Models.ErrorWrapper) {
        this.urlManager.setError(Models.ErrorCategory.HttpServerError);
    }

    private handleHttpClientError(reject: Models.ErrorWrapper,
        displayMessages: (em: Models.ErrorMap) => void) {
        switch (reject.httpErrorCode) {
            case (Models.HttpStatusCode.UnprocessableEntity):
                displayMessages(reject.error as Models.ErrorMap);
                break;
            default:
                this.urlManager.setError(Models.ErrorCategory.HttpClientError, reject.httpErrorCode);
        }
    }

    private handleClientError(reject: Models.ErrorWrapper) {
        this.urlManager.setError(Models.ErrorCategory.ClientError, reject.clientErrorCode);
    }

    handleError (reject: Models.ErrorWrapper)  {
        this.handleErrorAndDisplayMessages(reject, () => { });
    };

    handleErrorAndDisplayMessages (reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void)  {
        this.preProcessors.forEach(p => p(reject));

        if (reject.handled) {
            return;
        }
        reject.handled = true;

        this.context.setError(reject);
        switch (reject.category) {
            case (Models.ErrorCategory.HttpServerError):
                this.handleHttpServerError(reject);
                break;
            case (Models.ErrorCategory.HttpClientError):
                this.handleHttpClientError(reject, displayMessages);
                break;
            case (Models.ErrorCategory.ClientError):
                this.handleClientError(reject);
                break;
        }
    };

    setErrorPreprocessor (handler: errorPreprocessor)  {
        this.preProcessors.push(handler);
    };

    //setErrorDisplayHandler = (handler: errorDisplayHandler) => {
    //    //this.displayHandlers.push(handler);
    //};

    //displayError = ($scope: INakedObjectsScope, routeData: Nakedobjectsroutedata.PaneRouteData) => {
    //    // first allow handlers to set error template, if none does then default 
    //    //displayHandlers.forEach(h => h($scope));

    //    //if (!$scope.errorTemplate) {
    //    //    $scope.errorTemplate = Nakedobjectsconstants.errorTemplate;
    //    //}
    //};

    // initialise error preprocessor with some concurrency handling code
    //setErrorPreprocessor((reject: Models.ErrorWrapper) : void {

    //    //if (reject.category === ErrorCategory.HttpClientError &&
    //    //    reject.httpErrorCode === HttpStatusCode.PreconditionFailed) {
    //    //    $rootScope.$broadcast(geminiConcurrencyEvent, new Models.ErrorMap({}, 0, concurrencyMessage));
    //    //    reject.handled = true;
    //    //}
    //}
}
