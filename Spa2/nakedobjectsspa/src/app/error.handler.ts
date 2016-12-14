import { ErrorHandler } from '@angular/core';
import { UrlManagerService } from './url-manager.service';
import { ContextService } from './context.service';
import * as Models from './models';

export class GeminiErrorHandler implements ErrorHandler {
    handleError(error: any) {
       
        const urlManager: UrlManagerService = error.context.injector.get(UrlManagerService);
        const context: ContextService = error.context.injector.get(ContextService);

        const rp = new Models.ErrorWrapper(Models.ErrorCategory.ClientError, Models.ClientErrorCode.SoftwareError, error.message);
        rp.stackTrace = error.stack.split("\n");

        context.setError(rp);
        urlManager.setError(Models.ErrorCategory.ClientError, Models.ClientErrorCode.SoftwareError);
    }
}