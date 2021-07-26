import { ErrorHandler } from '@angular/core';
import { ContextService } from './context.service';
import { ErrorWrapper, ClientErrorCode, ErrorCategory } from './error.wrapper';
import { UrlManagerService } from './url-manager.service';

export class GeminiErrorHandler implements ErrorHandler {
    handleError(error: any) {

        const ec = (error && error.context) || (error && error.rejection && error.rejection.context);

        if (ec && ec.injector) {

            const urlManager: UrlManagerService = ec.injector.get(UrlManagerService);
            const context: ContextService = ec.injector.get(ContextService);

            const rp = new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.SoftwareError, error.message);
            rp.stackTrace = error.stack.split('\n');

            context.setError(rp);
            urlManager.setError(ErrorCategory.ClientError, ClientErrorCode.SoftwareError);
        } else {
            error = error || { message: 'null error', stack: '' };
            console.error(`${error.message}\n${error.stack}`);
            throw error;
        }
    }
}
