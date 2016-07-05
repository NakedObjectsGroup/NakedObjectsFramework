/// <reference path="typings/lodash/lodash.d.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.run(function (error) {
        // Set as many display handlers as you want. They are called in order.
        // If no template is set then the default error template will be used. 
        // ErrorViewModel is on $scope.error
        // Original error is available through ErrorViewModel:originalError;
        error.setErrorDisplayHandler(function ($scope) {
            if ($scope.error.isConcurrencyError) {
                $scope.errorTemplate = NakedObjects.concurrencyTemplate;
            }
        });
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.error.config.js.map