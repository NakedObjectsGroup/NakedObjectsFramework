import { UrlManagerService } from './url-manager.service';
import { ContextService } from './context.service';
import * as Models from '@nakedobjects/restful-objects';
import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';
import { ErrorCategory, HttpStatusCode } from './constants';
import { ErrorWrapper } from './error.wrapper';

type ErrorPreprocessor = (reject: ErrorWrapper) => void;

@Injectable()
export class ErrorService {

    constructor(
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly configService: ConfigService
    ) {

        this.setErrorPreprocessor(reject => {
            if (reject.category === ErrorCategory.HttpClientError && reject.httpErrorCode === HttpStatusCode.PreconditionFailed) {

                if (reject.originalUrl) {
                    const oid = Models.ObjectIdWrapper.fromHref(reject.originalUrl, configService.config.keySeparator);
                    this.context.setConcurrencyError(oid);
                    reject.handled = true;
                }
            }
        });
    }

    private preProcessors: ErrorPreprocessor[] = [];

    private handleHttpServerError(reject: ErrorWrapper) {
        this.urlManager.setError(ErrorCategory.HttpServerError);
    }

    private handleHttpClientError(reject: ErrorWrapper,
        displayMessages: (em: Models.ErrorMap) => void) {
        switch (reject.httpErrorCode) {
            case (HttpStatusCode.UnprocessableEntity):
                displayMessages(reject.error as Models.ErrorMap);
                break;
            default:
                this.urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
        }
    }

    private handleClientError(reject: ErrorWrapper) {
        this.urlManager.setError(ErrorCategory.ClientError, reject.clientErrorCode);
    }

    handleError(reject: ErrorWrapper) {
        this.handleErrorAndDisplayMessages(reject, () => { });
    }

    handleErrorAndDisplayMessages(reject: ErrorWrapper, displayMessages: (em: Models.ErrorMap) => void) {
        this.preProcessors.forEach(p => p(reject));

        if (reject.handled) {
            return;
        }
        reject.handled = true;

        this.context.setError(reject);
        switch (reject.category) {
            case (ErrorCategory.HttpServerError):
                this.handleHttpServerError(reject);
                break;
            case (ErrorCategory.HttpClientError):
                this.handleHttpClientError(reject, displayMessages);
                break;
            case (ErrorCategory.ClientError):
                this.handleClientError(reject);
                break;
        }
    }

    private setErrorPreprocessor(handler: ErrorPreprocessor) {
        this.preProcessors.push(handler);
    }
}
