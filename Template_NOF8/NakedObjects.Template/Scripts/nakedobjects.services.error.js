/// <reference path="typings/lodash/lodash.d.ts" />
var NakedObjects;
(function (NakedObjects) {
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var HttpStatusCode = NakedObjects.Models.HttpStatusCode;
    NakedObjects.app.service("error", function (urlManager, context, $rootScope) {
        var errorService = this;
        var preProcessors = [];
        var displayHandlers = [];
        function handleHttpServerError() {
            urlManager.setError(ErrorCategory.HttpServerError);
        }
        function handleHttpClientError(reject, displayMessages) {
            switch (reject.httpErrorCode) {
                case (HttpStatusCode.UnprocessableEntity):
                    displayMessages(reject.error);
                    break;
                default:
                    urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
            }
        }
        function handleClientError(reject) {
            urlManager.setError(ErrorCategory.ClientError, reject.clientErrorCode);
        }
        errorService.handleError = function (reject) {
            errorService.handleErrorAndDisplayMessages(reject, function () { });
        };
        errorService.handleErrorAndDisplayMessages = function (reject, displayMessages) {
            preProcessors.forEach(function (p) { return p(reject); });
            if (reject.handled) {
                return;
            }
            reject.handled = true;
            context.setError(reject);
            switch (reject.category) {
                case (ErrorCategory.HttpServerError):
                    handleHttpServerError();
                    break;
                case (ErrorCategory.HttpClientError):
                    handleHttpClientError(reject, displayMessages);
                    break;
                case (ErrorCategory.ClientError):
                    handleClientError(reject);
                    break;
            }
        };
        errorService.setErrorPreprocessor = function (handler) {
            preProcessors.push(handler);
        };
        errorService.setErrorDisplayHandler = function (handler) {
            displayHandlers.push(handler);
        };
        errorService.displayError = function ($scope) {
            // first allow handlers to set error template, if none does then default 
            displayHandlers.forEach(function (h) { return h($scope); });
            if (!$scope.errorTemplate) {
                $scope.errorTemplate = NakedObjects.errorTemplate;
            }
        };
        // initialise error preprocessor with some concurrency handling code
        errorService.setErrorPreprocessor(function (reject) {
            if (reject.category === ErrorCategory.HttpClientError &&
                reject.httpErrorCode === HttpStatusCode.PreconditionFailed) {
                $rootScope.$broadcast(NakedObjects.geminiConcurrencyEvent, new NakedObjects.Models.ErrorMap({}, 0, NakedObjects.concurrencyMessage));
                reject.handled = true;
            }
        });
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.error.js.map