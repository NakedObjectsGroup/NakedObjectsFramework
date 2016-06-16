/// <reference path="typings/lodash/lodash.d.ts" />


module NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ErrorMap = Models.ErrorMap;
    import HttpStatusCode = Models.HttpStatusCode;

    export type errorPreprocessor = (reject: ErrorWrapper) => void;
    export type errorDisplayHandler = ($scope: INakedObjectsScope) => void;

    export interface IError {

        handleError(reject: Models.ErrorWrapper): void;

        handleErrorAndDisplayMessages(reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void): void;

        handleErrorWithReload(reject: Models.ErrorWrapper,
            toReload: Models.DomainObjectRepresentation,
            onReload: (updatedObject: Models.DomainObjectRepresentation) => void,
            displayMessages: (em: Models.ErrorMap) => void): void;

        setErrorPreprocessor(handler: errorPreprocessor): void;

        displayError($scope: INakedObjectsScope, routeData: PaneRouteData): void;

        setErrorDisplayHandler(handler: errorDisplayHandler): void;
    }

    app.service("error",
        function(urlManager: IUrlManager, context: IContext) {
            const errorService = <IError>this;

            const preProcessors: errorPreprocessor[] = [];
            const displayHandlers: errorDisplayHandler[] = [];

            function handleHttpServerError(reject: ErrorWrapper) {
                urlManager.setError(ErrorCategory.HttpServerError);
            }

            function handleHttpClientError(reject: ErrorWrapper,
                toReload: DomainObjectRepresentation,
                onReload: (updatedObject: DomainObjectRepresentation) => void,
                displayMessages: (em: ErrorMap) => void) {
                switch (reject.httpErrorCode) {
                case (HttpStatusCode.PreconditionFailed):

                    if (toReload.isTransient()) {
                        urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
                    } else {
                        context.reloadObject(1, toReload)
                            .then((updatedObject: DomainObjectRepresentation) => {
                                onReload(updatedObject);
                            });
                    }
                    break;
                case (HttpStatusCode.UnprocessableEntity):
                    displayMessages(reject.error as ErrorMap);
                    break;
                default:
                    urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
                }
            }

            function handleClientError(reject: ErrorWrapper) {
                urlManager.setError(ErrorCategory.ClientError, reject.clientErrorCode);
            }

            errorService.handleErrorWithReload = (reject: ErrorWrapper,
                toReload: DomainObjectRepresentation,
                onReload: (updatedObject: DomainObjectRepresentation) => void,
                displayMessages: (em: ErrorMap) => void) => {


                preProcessors.forEach(p => p(reject));

                if (reject.handled) {
                    return;
                }
                reject.handled = true;

                context.setError(reject);
                switch (reject.category) {
                case (ErrorCategory.HttpServerError):
                    handleHttpServerError(reject);
                    break;
                case (ErrorCategory.HttpClientError):
                    handleHttpClientError(reject, toReload, onReload, displayMessages);
                    break;
                case (ErrorCategory.ClientError):
                    handleClientError(reject);
                    break;
                }
            };

            errorService.handleError = (reject: ErrorWrapper) => {
                errorService.handleErrorWithReload(reject, null, () => {}, () => {});
            };

            errorService.handleErrorAndDisplayMessages = (reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void) => {
                errorService.handleErrorWithReload(reject, null, () => {}, displayMessages);
            };

            errorService.setErrorPreprocessor = (handler: errorPreprocessor) => {
                preProcessors.push(handler);
            };

            errorService.setErrorDisplayHandler = (handler: errorDisplayHandler) => {
                displayHandlers.push(handler);
            };

            errorService.displayError = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
                // first allow handlers to set error template, if none does then default 
                displayHandlers.forEach(h => h($scope));

                if (!$scope.errorTemplate) {
                    $scope.errorTemplate = errorTemplate;
                }
            };

        });
}