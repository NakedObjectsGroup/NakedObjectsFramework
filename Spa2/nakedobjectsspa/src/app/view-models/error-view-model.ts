import * as Models from '../models';

export class ErrorViewModel {
    originalError: Models.ErrorWrapper;
    title: string;
    message: string;
    stackTrace: string[];
    errorCode: string;
    description: string;
    isConcurrencyError: boolean;
}