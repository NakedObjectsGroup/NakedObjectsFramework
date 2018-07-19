import { Injectable } from '@angular/core';
import * as Models from './models';
import * as Ro from './ro-interfaces';
import { Subject ,  Observable } from 'rxjs';
import { ConfigService } from './config.service';
import { SimpleLruCache } from './simple-lru-cache';

import { Dictionary } from 'lodash';
import each from 'lodash-es/each';
import reduce from 'lodash-es/reduce';
import { HttpClient, HttpRequest, HttpHeaders, HttpParams, HttpResponse, HttpErrorResponse } from '@angular/common/http';

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

    static fromMap(map: Models.IHateoasModel, digest?: string | null) {
        return new RequestOptions(map.getUrl(), map.method, map.getBody(), digest);
    }

    static fromLink(link: Models.Link, parms?: Dictionary<Object>) {
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

    private handleInvalidResponse(rc: Models.ErrorCategory) {
        const rr = new Models.ErrorWrapper(rc,
            Models.ClientErrorCode.ConnectionProblem,
            'The response from the client was not parseable as a RestfulObject json Representation ');

        return Promise.reject(rr);
    }

    private isObjectUrl(url: string) {
        const segments = url.split('/');
        return segments.length >= 4 && segments[3] === 'objects';
    }

    private handleError(response: HttpErrorResponse, originalUrl: string) {
        let category: Models.ErrorCategory;
        let error: Models.ErrorRepresentation | Models.ErrorMap | string;

        if (response.status === Models.HttpStatusCode.InternalServerError) {
            // this error should contain an error representatation
            const errorRep = new Models.ErrorRepresentation();
                if (Models.isErrorRepresentation(response.error)) {
                errorRep.populate(response.error as Ro.IErrorRepresentation);
                category = Models.ErrorCategory.HttpServerError;
                error = errorRep;
            } else {
                return this.handleInvalidResponse(Models.ErrorCategory.HttpServerError);
            }
        } else if (response.status <= 0) {
            // failed to connect
            category = Models.ErrorCategory.ClientError;
            error = `Failed to connect to server: ${response.url || 'unknown'}`;
        } else {
            category = Models.ErrorCategory.HttpClientError;
            const message = (response.headers && response.headers.get('warning')) || 'Unknown client HTTP error';

            if (response.status === Models.HttpStatusCode.BadRequest ||
                response.status === Models.HttpStatusCode.UnprocessableEntity) {
                // these errors should contain a map
                error = new Models.ErrorMap(response.error as Ro.IValueMap | Ro.IObjectOfType,
                    response.status,
                    message);
            } else if (response.status === Models.HttpStatusCode.NotFound && this.isObjectUrl(originalUrl)) {
                // were looking for an object an got not found - object may be deleted
                // treat as http problem.
                category = Models.ErrorCategory.HttpClientError;
                error = `Failed to connect to server: ${response.url || 'unknown'}`;
            } else if (response.status === Models.HttpStatusCode.NotFound) {
                // general not found other than object - assume client programming error
                category = Models.ErrorCategory.ClientError;
                error = `Failed to connect to server: ${response.url || 'unknown'}`;
            } else {
                error = message;
            }
        }

        const rr = new Models.ErrorWrapper(category, response.status as Models.HttpStatusCode, error, originalUrl);

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
    private handleRedirectedObject(response: Models.IHateoasModel, data: Ro.IRepresentation) {

        if (response instanceof Models.ActionResultRepresentation && Models.isIDomainObjectRepresentation(data)) {
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
        return Models.isResourceRepresentation(data);
    }

    private httpPopulate(options: RequestOptions, ignoreCache: boolean, response: Models.IHateoasModel): Promise<Models.IHateoasModel> {

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
                    return this.handleInvalidResponse(Models.ErrorCategory.ClientError);
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

    populate = <T extends Models.IHateoasModel>(model: Models.IHateoasModel, ignoreCache?: boolean): Promise<T> => {
        const response = model;
        const options = RequestOptions.fromMap(model);
        return this.httpPopulate(options, !!ignoreCache, response) as Promise<T>;
    }

    retrieve = <T extends Models.IHateoasModel>(map: Models.IHateoasModel, rc: { new (): Models.IHateoasModel }, digest?: string | null): Promise<T> => {
        const response = new rc();
        const options = RequestOptions.fromMap(map, digest);
        return this.httpPopulate(options, true, response) as Promise<T>;
    }

    validate = (map: Models.IHateoasModel, digest?: string): Promise<boolean> => {
        const options = RequestOptions.fromMap(map, digest);
        return this.httpValidate(options);
    }

    retrieveFromLink = <T extends Models.IHateoasModel>(link: Models.Link | null, parms?: Dictionary<Object>): Promise<T> => {

        if (link) {
            const response = link.getTarget();
            const options = RequestOptions.fromLink(link, parms);
            return this.httpPopulate(options, true, response) as Promise<T>;
        }
        return Promise.reject('link must not be null');
    }

    invoke = (action: Models.ActionRepresentation | Models.InvokableActionMember, parms: Dictionary<Models.Value>, urlParms: Dictionary<Object>): Promise<Models.ActionResultRepresentation> => {
        const invokeMap = action.getInvokeMap();
        if (invokeMap) {
            each(urlParms, (v, k) => invokeMap.setUrlParameter(k!, v));
            each(parms, (v, k) => invokeMap.setParameter(k!, v));
            return this.retrieve(invokeMap, Models.ActionResultRepresentation);
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
