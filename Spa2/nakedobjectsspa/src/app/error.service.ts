import { UrlManagerService } from './url-manager.service';
import { ContextService } from './context.service';
import * as Models from './models';
import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';

type ErrorPreprocessor = (reject: Models.ErrorWrapper) => void;

@Injectable()
export class ErrorService {

    constructor(
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly configService: ConfigService
    ) {

        this.setErrorPreprocessor(reject => {
            if (reject.category === Models.ErrorCategory.HttpClientError && reject.httpErrorCode === Models.HttpStatusCode.PreconditionFailed) {

                if (reject.originalUrl) {
                    const oid = Models.ObjectIdWrapper.fromHref(reject.originalUrl, configService.config.keySeparator);
                    this.context.setConcurrencyError(oid);
                    reject.handled = true;
                }
            }
        });
    }

    private preProcessors: ErrorPreprocessor[] = [];

    private handleHttpServerError(reject: Models.ErrorWrapper) {
        this.urlManager.setError(Models.ErrorCategory.HttpServerError);
    }

    private handleHttpClientError(reject: Models.ErrorWrapper,
        displayMessages: (em: Models.ErrorMap) => void) {
        switch (reject.httpErrorCode) {
            case (Models.HttpStatusCode.UnprocessableEntity):
                displayMessages(reject.error as Models.ErrorMap);
                break;
            default:
                this.urlManager.setError(Models.ErrorCategory.HttpClientError, reject.httpErrorCode);
        }
    }

    private handleClientError(reject: Models.ErrorWrapper) {
        this.urlManager.setError(Models.ErrorCategory.ClientError, reject.clientErrorCode);
    }

    handleError(reject: Models.ErrorWrapper) {
        this.handleErrorAndDisplayMessages(reject, () => { });
    }

    handleErrorAndDisplayMessages(reject: Models.ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void) {
        this.preProcessors.forEach(p => p(reject));

        if (reject.handled) {
            return;
        }
        reject.handled = true;

        this.context.setError(reject);
        switch (reject.category) {
            case (Models.ErrorCategory.HttpServerError):
                this.handleHttpServerError(reject);
                break;
            case (Models.ErrorCategory.HttpClientError):
                this.handleHttpClientError(reject, displayMessages);
                break;
            case (Models.ErrorCategory.ClientError):
                this.handleClientError(reject);
                break;
        }
    }

    private setErrorPreprocessor(handler: ErrorPreprocessor) {
        this.preProcessors.push(handler);
    }
}
