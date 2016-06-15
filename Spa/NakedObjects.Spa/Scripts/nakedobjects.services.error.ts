/// <reference path="typings/lodash/lodash.d.ts" />


module NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ErrorMap = Models.ErrorMap;
    import HttpStatusCode = Models.HttpStatusCode;
   

    export interface IServerErrorHandler {
        handle(reject : Models.ErrorWrapper) : void;
    }


    export interface IError {

        handleError(reject: Models.ErrorWrapper): void;

        handleErrorAndDisplayMessages(reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void): void;

        handleWrappedError(reject: Models.ErrorWrapper,
            toReload: Models.DomainObjectRepresentation,
            onReload: (updatedObject: Models.DomainObjectRepresentation) => void,
            displayMessages: (em: Models.ErrorMap) => void): void;
    }

    app.service("error",
        function(urlManager: IUrlManager, context: IContext) {
            const errorService = <IError>this;

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

            errorService.handleWrappedError = (reject: ErrorWrapper,
                                               toReload: DomainObjectRepresentation,
                                               onReload: (updatedObject: DomainObjectRepresentation) => void,
                                               displayMessages: (em: ErrorMap) => void) => {
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
                errorService.handleWrappedError(reject, null, () => {}, () => {});
            };

            errorService.handleErrorAndDisplayMessages = (reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void) => {
                errorService.handleWrappedError(reject, null, () => {}, displayMessages);
            };
        });
}