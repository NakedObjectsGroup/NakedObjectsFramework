/// <reference path="typings/lodash/lodash.d.ts" />


namespace NakedObjects {

    app.run((error: IError) => {

        // Set as many display handlers as you want. They are called in order.
        // If no template is set then the default error template will be used. 
        // ErrorViewModel is on $scope.error
        // Original error is available through ErrorViewModel:originalError;
        error.setErrorDisplayHandler(($scope: INakedObjectsScope) => {
            if ($scope.error.isConcurrencyError) {
                $scope.errorTemplate = concurrencyTemplate;
            }
        });
    });
}