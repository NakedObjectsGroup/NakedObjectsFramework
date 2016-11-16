import * as Models from '../models';

export class ErrorViewModel {

    constructor(error: Models.ErrorWrapper) {
        this.originalError = error;
        if (error) {
            this.title = error.title;
            this.description = error.description;
            this.errorCode = error.errorCode;
            this.message = error.message;
            const stackTrace = error.stackTrace;

            this.stackTrace = stackTrace && stackTrace.length !== 0 ? stackTrace : null;

            this.isConcurrencyError =
                error.category === Models.ErrorCategory.HttpClientError &&
                error.httpErrorCode === Models.HttpStatusCode.PreconditionFailed;
        }

        this.description = this.description || "No description available";
        this.errorCode = this.errorCode || "No code available";
        this.message = this.message || "No message available";
        this.stackTrace = this.stackTrace || ["No stack trace available"];
    }

    originalError: Models.ErrorWrapper;
    title: string;
    message: string;
    stackTrace: string[];
    errorCode: string;
    description: string;
    isConcurrencyError: boolean;
}