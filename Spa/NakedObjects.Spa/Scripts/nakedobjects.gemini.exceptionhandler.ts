
module NakedObjects.Angular.Gemini {

    app.provider({
        $exceptionHandler() {


            this.$get = $injector => (exception) => {
                const ulrManager: IUrlManager = $injector.get("urlManager");
                const context: IContext = $injector.get("context");
                const error = ErrorRepresentation.create(exception.message, exception.stack.split("\n"));

                const rp = new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.SoftwareError, "", error);
     
                context.setError(rp);
                ulrManager.setError(ErrorCategory.ClientError, ClientErrorCode.SoftwareError);

            };
        }
    });
}