import * as Models from './models';
import * as Msg from './user-messages';
import { ErrorCategory, HttpStatusCode, ClientErrorCode } from './constants';

export class ErrorWrapper {
    constructor(
        readonly category: ErrorCategory,
        code: HttpStatusCode | ClientErrorCode,
        err: string | Models.ErrorMap | Models.ErrorRepresentation,
        readonly originalUrl?: string
    ) {

        if (category === ErrorCategory.ClientError) {
            this.clientErrorCode = code as ClientErrorCode;
            this.errorCode = ClientErrorCode[this.clientErrorCode];
            let description = Msg.errorUnknown;

            switch (this.clientErrorCode) {
                case ClientErrorCode.ExpiredTransient:
                    description = Msg.errorExpiredTransient;
                    break;
                case ClientErrorCode.WrongType:
                    description = Msg.errorWrongType;
                    break;
                case ClientErrorCode.NotImplemented:
                    description = Msg.errorNotImplemented;
                    break;
                case ClientErrorCode.SoftwareError:
                    description = Msg.errorSoftware;
                    break;
                case ClientErrorCode.ConnectionProblem:
                    description = Msg.errorConnection;
                    break;
            }

            this.description = description;
            this.title = Msg.errorClient;
        }

        if (category === ErrorCategory.HttpClientError || category === ErrorCategory.HttpServerError) {
            this.httpErrorCode = code as HttpStatusCode;
            this.errorCode = `${HttpStatusCode[this.httpErrorCode]}(${this.httpErrorCode})`;

            this.description = category === ErrorCategory.HttpServerError
                ? 'A software error has occurred on the server'
                : 'An HTTP error code has been received from the server\n' +
                'You can look up the meaning of this code in the Restful Objects specification.';

            this.title = 'Error message received from server';
        }

        if (err instanceof Models.ErrorMap) {
            const em = err as Models.ErrorMap;
            this.message = em.invalidReason() || em.warningMessage;
            this.error = em;
            this.stackTrace = [];
        } else if (err instanceof Models.ErrorRepresentation) {
            const er = err as Models.ErrorRepresentation;
            this.message = er.message();
            this.error = er;
            this.stackTrace = err.stackTrace();
        } else {
            this.message = (err as string);
            this.error = null;
            this.stackTrace = [];
        }
    }

    title: string;
    description: string;
    errorCode: string;
    httpErrorCode: HttpStatusCode;
    clientErrorCode: ClientErrorCode;

    message: string;
    error: Models.ErrorMap | Models.ErrorRepresentation | null;

    stackTrace: string[];

    handled = false;
}
