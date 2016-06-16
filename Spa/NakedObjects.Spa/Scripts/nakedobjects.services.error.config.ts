/// <reference path="typings/lodash/lodash.d.ts" />


module NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import HttpStatusCode = Models.HttpStatusCode;
    app.run((error: IError, $rootScope: ng.IRootScopeService) => {


        // hansdle concurrency by triggering a concurrency event which will reload object and display message
        error.setErrorPreprocessor((reject: ErrorWrapper) => {

            if (reject.category === ErrorCategory.HttpClientError &&
                reject.httpErrorCode === HttpStatusCode.PreconditionFailed) {
                $rootScope.$broadcast(geminiConcurrencyEvent, new Models.ErrorMap({}, 0, concurrencyMessage));
                reject.handled = true;
            }
        });


        // set as many display handlers as you want. They are called in order
        // if no template is set tyhen the default error template will be used. 
        error.setErrorDisplayHandler(($scope: INakedObjectsScope) => {
            if ($scope.error.isConcurrencyError) {
                $scope.errorTemplate = concurrencyTemplate;
            }
        });

    });
}