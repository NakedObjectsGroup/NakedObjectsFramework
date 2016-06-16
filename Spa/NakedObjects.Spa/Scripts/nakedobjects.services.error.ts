/// <reference path="typings/lodash/lodash.d.ts" />


module NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import ErrorMap = Models.ErrorMap;
    import HttpStatusCode = Models.HttpStatusCode;

    export type errorPreprocessor = (reject: ErrorWrapper) => void;
    export type errorDisplayHandler = ($scope: INakedObjectsScope) => void;

    export interface IError {

        handleError(reject: Models.ErrorWrapper): void;
        handleErrorAndDisplayMessages(reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void): void;
        displayError($scope: INakedObjectsScope, routeData: PaneRouteData): void;

        setErrorPreprocessor(handler: errorPreprocessor): void;
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
                                           displayMessages: (em: ErrorMap) => void) {
                switch (reject.httpErrorCode) {
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

            errorService.handleError = (reject: ErrorWrapper) => {
                errorService.handleErrorAndDisplayMessages(reject, () => {});
            };

            errorService.handleErrorAndDisplayMessages = (reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void) => {
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
                    handleHttpClientError(reject, displayMessages);
                    break;
                case (ErrorCategory.ClientError):
                    handleClientError(reject);
                    break;
                }
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