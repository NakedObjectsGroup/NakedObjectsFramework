export enum ErrorCategory {
    HttpClientError,
    HttpServerError,
    ClientError
}

export enum HttpStatusCode {
    NoContent = 204,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    NotAcceptable = 406,
    PreconditionFailed = 412,
    UnprocessableEntity = 422,
    PreconditionRequired = 428,
    InternalServerError = 500
}

// updated by build do not update manually or change name or regex may not match
export const clientVersion = '10.0.0';
