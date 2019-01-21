import { Injectable } from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import { ConfigService } from './config.service';
import { ContextService } from './context.service';
import { ErrorWrapper, ErrorCategory, HttpStatusCode } from './error.wrapper';
import { UrlManagerService } from './url-manager.service';

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
                    const oid = Ro.ObjectIdWrapper.fromHref(reject.originalUrl, configService.config.keySeparator);
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
        displayMessages: (em: Ro.ErrorMap) => void) {
        switch (reject.httpErrorCode) {
            case (HttpStatusCode.UnprocessableEntity):
                displayMessages(reject.error as Ro.ErrorMap);
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

    handleErrorAndDisplayMessages(reject: ErrorWrapper, displayMessages: (em: Ro.ErrorMap) => void) {
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
