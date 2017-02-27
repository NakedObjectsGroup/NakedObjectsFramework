import { ErrorHandler } from '@angular/core';
import { UrlManagerService } from './url-manager.service';
import { ContextService } from './context.service';
import * as Models from './models';

export class GeminiErrorHandler implements ErrorHandler {
    handleError(error: any) {

        // todo make safer 
        const ec = error.context || error.rejection.context;

        if (ec) {

            const urlManager: UrlManagerService = ec.injector.get(UrlManagerService);
            const context: ContextService = ec.injector.get(ContextService);

            const rp = new Models.ErrorWrapper(Models.ErrorCategory.ClientError, Models.ClientErrorCode.SoftwareError, error.message);
            rp.stackTrace = error.stack.split("\n");

            context.setError(rp);
            urlManager.setError(Models.ErrorCategory.ClientError, Models.ClientErrorCode.SoftwareError);
        } else {
            throw error;
        }
    }
}