
module NakedObjects.Angular.Gemini {

    app.provider({
        $exceptionHandler() {


            this.$get = $injector => (exception) => {
                const ulrManager: IUrlManager = $injector.get("urlManager");
                const context: IContext = $injector.get("context");
                const error = ErrorRepresentation.create(exception.message, exception.stack.split("\n"));

                context.setError(error);
                ulrManager.setError(ErrorType.Software);

            };
        }
    });
}