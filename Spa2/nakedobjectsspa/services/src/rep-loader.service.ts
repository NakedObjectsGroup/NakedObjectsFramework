import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';
import each from 'lodash-es/each';
import reduce from 'lodash-es/reduce';
import { Subject } from 'rxjs';
import { ConfigService } from './config.service';
import { ErrorWrapper, ClientErrorCode, ErrorCategory, HttpStatusCode } from './error.wrapper';
import { SimpleLruCache } from './simple-lru-cache';

class RequestOptions {

    private constructor(url: string, method: 'DELETE' | 'GET' | 'POST' | 'PUT', body?: object, digest?: string | null) {
        this.url = url;
        this.method = method;
        this.init = {
            withCredentials: true,
        };
        if (body) {
            this.body = body;
        }
        if (digest && this.isPotent()) {
            this.init.headers = new HttpHeaders({ 'If-Match': digest });
        }
    }

    method: 'DELETE' | 'GET' | 'POST' | 'PUT';
    url: string;
    body?: object;
    init: {
        headers?: HttpHeaders;
        reportProgress?: boolean;
        params?: HttpParams;
        responseType?: 'arraybuffer' | 'blob' | 'json' | 'text';
        withCredentials?: boolean;
    };

    static fromMap(map: Ro.IHateoasModel, digest?: string | null) {
        return new RequestOptions(map.getUrl(), map.method, map.getBody(), digest);
    }

    static fromLink(link: Ro.Link, parms?: Dictionary<Object>) {
        let urlParms = '';

        if (parms) {
            const urlParmString = reduce(parms, (result, n, key) => (result === '' ? '' : result + '&') + key + '=' + n, '');
            urlParms = urlParmString !== '' ? `?${urlParmString}` : '';
        }

        return new RequestOptions(link.href() + urlParms, link.method());
    }

    static fromFile(url: string, method: 'GET' | 'POST', mt: string, body?: object) {
        const options = new RequestOptions(url, method, body);

        options.init.responseType = 'blob';

        if (method === 'GET') {
            options.init.headers = new HttpHeaders({ 'Accept': mt });
        }
        if (method === 'POST') {
            options.init.headers = new HttpHeaders({ 'Content-Type': mt });
        }

        delete options.init.withCredentials;
        return options;
    }

    isPotent() {
        return this.method === 'POST' || this.method === 'PUT' || this.method === 'DELETE';
    }

    toHttpRequest() {
        return new HttpRequest(this.method, this.url, this.body, this.init);
    }
}

@Injectable()
export class RepLoaderService {

    constructor(
        private readonly http: HttpClient,
        private readonly configService: ConfigService
    ) { }

    private loadingCount = 0;

    private loadingCountSource = new Subject<number>();

    loadingCount$ = this.loadingCountSource.asObservable();

    // use our own LRU cache
    private cache = new SimpleLruCache(this.configService.config.httpCacheDepth);

    private handleInvalidResponse(rc: ErrorCategory) {
        const rr = new ErrorWrapper(rc,
            ClientErrorCode.ConnectionProblem,
            'The response from the client was not parseable as a RestfulObject json Representation ');

        return Promise.reject(rr);
    }

    private isObjectUrl(url: string) {
        const segments = url.split('/');
        return segments.length >= 4 && segments[3] === 'objects';
    }

    private handleError(response: HttpErrorResponse, originalUrl: string) {
        let category: ErrorCategory;
        let error: Ro.ErrorRepresentation | Ro.ErrorMap | string;

        if (response.status === HttpStatusCode.InternalServerError) {
            // this error should contain an error representatation
            const errorRep = new Ro.ErrorRepresentation();
            if (Ro.isErrorRepresentation(response.error)) {
                errorRep.populate(response.error as Ro.IErrorRepresentation);
                category = ErrorCategory.HttpServerError;
                error = errorRep;
            } else {
                return this.handleInvalidResponse(ErrorCategory.HttpServerError);
            }
        } else if (response.status <= 0) {
            // failed to connect
            category = ErrorCategory.ClientError;
            error = `Failed to connect to server: ${response.url || 'unknown'}`;
        } else {
            category = ErrorCategory.HttpClientError;
            const message = (response.headers && response.headers.get('warning')) || 'Unknown client HTTP error';

            if (response.status === HttpStatusCode.BadRequest ||
                response.status === HttpStatusCode.UnprocessableEntity) {
                // these errors should contain a map
                error = new Ro.ErrorMap(response.error as Ro.IValueMap | Ro.IObjectOfType,
                    response.status,
                    message);
            } else if (response.status === HttpStatusCode.NotFound && this.isObjectUrl(originalUrl)) {
                // were looking for an object an got not found - object may be deleted
                // treat as http problem.
                category = ErrorCategory.HttpClientError;
                error = `Failed to connect to server: ${response.url || 'unknown'}`;
            } else if (response.status === HttpStatusCode.NotFound) {
                // general not found other than object - assume client programming error
                category = ErrorCategory.ClientError;
                error = `Failed to connect to server: ${response.url || 'unknown'}`;
            } else {
                error = message;
            }
        }

        const rr = new ErrorWrapper(category, response.status as HttpStatusCode, error, originalUrl);

        return Promise.reject(rr);
    }

    private httpValidate(options: RequestOptions): Promise<boolean> {
        this.loadingCountSource.next(++(this.loadingCount));

        return this.http.request(options.toHttpRequest())
            .toPromise()
            .then(() => {
                this.loadingCountSource.next(--(this.loadingCount));
                return Promise.resolve(true);
            })
            .catch((r: HttpErrorResponse) => {
                this.loadingCountSource.next(--(this.loadingCount));
                const originalUrl = options.url || 'Unknown url';
                return this.handleError(r, originalUrl);
            });
    }

    // special handler for case where we receive a redirected object back from server
    // instead of an actionresult. Wrap the object in an actionresult and then handle normally
    private handleRedirectedObject(response: Ro.IHateoasModel, data: Ro.IRepresentation) {

        if (response instanceof Ro.ActionResultRepresentation && Ro.isIDomainObjectRepresentation(data)) {
            const actionResult: Ro.IActionInvokeRepresentation = {
                resultType: 'object',
                result: data,
                links: [],
                extensions: {}
            };
            return actionResult;
        }

        return data;
    }

    private isValidResponse(data: any) {
        return Ro.isResourceRepresentation(data);
    }

    private httpPopulate(options: RequestOptions, ignoreCache: boolean, response: Ro.IHateoasModel): Promise<Ro.IHateoasModel> {

        if (!options.url) {
            throw new Error('Request must have a URL');
        }

        const requestUrl = options.url;
        const clearCache = this.configService.config.clearCacheOnChange && options.isPotent();

        if (clearCache) {
            this.cache.removeAll();
        } else if (ignoreCache) {
            // clear cache of existing values
            this.cache.remove(requestUrl);
        } else {
            const cachedValue = this.cache.get(requestUrl);

            if (cachedValue) {
                response.populate(cachedValue);
                response.keySeparator = this.configService.config.keySeparator;
                return Promise.resolve(response);
            }
        }

        this.loadingCountSource.next(++(this.loadingCount));

        return this.http.request(options.toHttpRequest())
            .toPromise()
            .then((r: HttpResponse<Ro.IRepresentation>) => {
                this.loadingCountSource.next(--(this.loadingCount));

                if (!this.isValidResponse(r.body)) {
                    return this.handleInvalidResponse(ErrorCategory.ClientError);
                }

                const representation = this.handleRedirectedObject(response, r.body!);
                this.cache.add(requestUrl, representation);
                response.populate(representation);
                response.etagDigest = (r.headers && r.headers.get('ETag')) || '';
                response.keySeparator = this.configService.config.keySeparator;
                return Promise.resolve(response);
            })
            .catch((r: HttpErrorResponse) => {
                this.loadingCountSource.next(--(this.loadingCount));
                return this.handleError(r, requestUrl);
            });
    }

    populate = <T extends Ro.IHateoasModel>(model: Ro.IHateoasModel, ignoreCache?: boolean): Promise<T> => {
        const response = model;
        const options = RequestOptions.fromMap(model);
        return this.httpPopulate(options, !!ignoreCache, response) as Promise<T>;
    }

    retrieve = <T extends Ro.IHateoasModel>(map: Ro.IHateoasModel, rc: { new(): Ro.IHateoasModel }, digest?: string | null): Promise<T> => {
        const response = new rc();
        const options = RequestOptions.fromMap(map, digest);
        return this.httpPopulate(options, true, response) as Promise<T>;
    }

    validate = (map: Ro.IHateoasModel, digest?: string): Promise<boolean> => {
        const options = RequestOptions.fromMap(map, digest);
        return this.httpValidate(options);
    }

    retrieveFromLink = <T extends Ro.IHateoasModel>(link: Ro.Link | null, parms?: Dictionary<Object>): Promise<T> => {

        if (link) {
            const response = link.getTarget();
            const options = RequestOptions.fromLink(link, parms);
            return this.httpPopulate(options, true, response) as Promise<T>;
        }
        return Promise.reject('link must not be null');
    }

    invoke = (action: Ro.ActionRepresentation | Ro.InvokableActionMember, parms: Dictionary<Ro.Value>, urlParms: Dictionary<Object>): Promise<Ro.ActionResultRepresentation> => {
        const invokeMap = action.getInvokeMap();
        if (invokeMap) {
            each(urlParms, (v, k) => invokeMap.setUrlParameter(k!, v));
            each(parms, (v, k) => invokeMap.setParameter(k!, v));
            return this.retrieve(invokeMap, Ro.ActionResultRepresentation);
        }
        return Promise.reject(`attempting to invoke uninvokable action ${action.actionId()}`);
    }

    clearCache = (url: string) => {
        this.cache.remove(url);
    }

    addToCache = (url: string, m: Ro.IResourceRepresentation) => {
        this.cache.add(url, m);
    }

    getFile = (url: string, mt: string, ignoreCache: boolean): Promise<Blob> => {

        if (ignoreCache) {
            // clear cache of existing values
            this.cache.remove(url);
        } else {
            const blob = this.cache.get(url) as Blob;
            if (blob) {
                return Promise.resolve(blob);
            }
        }

        const options = RequestOptions.fromFile(url, 'GET', mt);

        return this.http.request(options.toHttpRequest())
            .toPromise()
            .then((r: HttpResponse<Blob>) => {
                const blob = r.body!;
                this.cache.add(options.url!, blob);
                return blob;
            })
            .catch((r: HttpErrorResponse) => {
                const originalUrl = options.url || 'Unknown url';
                return this.handleError(r, originalUrl);
            });
    }

    uploadFile = (url: string, mt: string, file: Blob): Promise<boolean> => {

        const options = RequestOptions.fromFile(url, 'POST', mt, file);

        return this.http.request(options.toHttpRequest())
            .toPromise()
            .then(() => {
                return Promise.resolve(true);
            })
            .catch(() => {
                return Promise.resolve(false);
            });
    }

    private logoff() {
        this.cache.removeAll();
    }
}
