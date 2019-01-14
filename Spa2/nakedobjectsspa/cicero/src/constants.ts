// routing constants

export const supportedDateFormats = ['D/M/YYYY', 'D/M/YY', 'D MMM YYYY', 'D MMMM YYYY', 'D MMM YY', 'D MMMM YY'];

export enum ErrorCategory {
    HttpClientError,
    HttpServerError,
    ClientError
}

export enum ClientErrorCode {
    ExpiredTransient,
    WrongType,
    NotImplemented,
    SoftwareError,
    ConnectionProblem = 0
}
