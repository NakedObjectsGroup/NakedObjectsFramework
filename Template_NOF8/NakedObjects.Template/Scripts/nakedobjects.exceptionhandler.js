/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var ErrorWrapper = NakedObjects.Models.ErrorWrapper;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    NakedObjects.app.provider({
        $exceptionHandler: function () {
            this.$get = function ($injector) { return function (exception) {
                var ulrManager = $injector.get("urlManager");
                var context = $injector.get("context");
                var rp = new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.SoftwareError, exception.message);
                rp.stackTrace = exception.stack.split("\n");
                context.setError(rp);
                ulrManager.setError(ErrorCategory.ClientError, ClientErrorCode.SoftwareError);
            }; };
        }
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.exceptionhandler.js.map