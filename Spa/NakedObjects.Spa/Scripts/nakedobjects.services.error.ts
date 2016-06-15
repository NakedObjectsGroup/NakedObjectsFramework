/// <reference path="typings/lodash/lodash.d.ts" />


module NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ErrorMap = Models.ErrorMap;
    import HttpStatusCode = Models.HttpStatusCode;
    import ClientErrorCode = Models.ClientErrorCode;

    export interface IError {
        handleWrappedError(reject: Models.ErrorWrapper,
            toReload: Models.DomainObjectRepresentation,
            onReload: (updatedObject: Models.DomainObjectRepresentation) => void,
            displayMessages: (em: Models.ErrorMap) => void,
            customClientHandler?: (ec: Models.ClientErrorCode) => boolean): void;
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

            function handleClientError(reject: ErrorWrapper, customClientHandler: (ec: ClientErrorCode) => boolean) {

                if (!customClientHandler(reject.clientErrorCode)) {
                    urlManager.setError(ErrorCategory.ClientError, reject.clientErrorCode);
                }
            }

            errorService.handleWrappedError = (reject: ErrorWrapper,
                toReload: DomainObjectRepresentation,
                onReload: (updatedObject: DomainObjectRepresentation) => void,
                displayMessages: (em: ErrorMap) => void,
                customClientHandler: (ec: ClientErrorCode) => boolean = () => false) => {
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
                    handleClientError(reject, customClientHandler);
                    break;
                }
            };


        });
}