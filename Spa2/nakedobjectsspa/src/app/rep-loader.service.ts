import { Injectable } from '@angular/core';
import { Http, Response, Request, RequestOptions, Headers, RequestMethod, ResponseContentType } from '@angular/http';
import './rxjs-extensions';
import * as Models from './models';
import * as Constants from './constants';
import * as Ro from './ro-interfaces';
import * as _ from 'lodash';
import { Subject } from 'rxjs/Subject';
import { ConfigService } from './config.service';
import { Observable } from 'rxjs'; // for declaration compile

type cachableTypes = Ro.IRepresentation | Blob;

class LruNode {

    constructor(public readonly key: string, public value: cachableTypes) { }

    next: LruNode | null = null;
    previous: LruNode | null = null;
}


class SimpleLruCache {

    constructor(private readonly depth: number) { }

    private cache: _.Dictionary<LruNode> = {};
    private count = 0;
    private head: LruNode | null = null;
    private tail: LruNode | null = null;

    private unlinkNode(node: LruNode) {
        const nodePrevious = node.previous;
        const nodeNext = node.next;

        if (nodePrevious) {
            nodePrevious.next = nodeNext;
        } else {
            // was head
            this.head = nodeNext;
        }

        if (nodeNext) {
            nodeNext.previous = nodePrevious;
        } else {
            //was tail
            this.tail = nodePrevious;
        }
        this.count--;
    }

    private moveNodeToHead(node: LruNode) {
        const existingHead = this.head;
        node.previous = null;
        node.next = existingHead;

        if (existingHead) {
            existingHead.previous = node;
        } else {
            // no existing head so this is also tail
            this.tail = node;
        }
        this.head = node;
        this.count++;
    }

    add(key: string, value: cachableTypes) {

        if (this.cache[key]) {
            this.updateExistingEntry(key, value);
        } else {
            this.addNewEntry(key, value);
        }
    }

    remove(key: string) {
        const node = this.cache[key];

        if (node) {
            this.unlinkNode(node);
            delete this.cache[key];
        }
    }

    removeAll() {
        this.head = this.tail = null;
        this.cache = {};
        this.count = 0;
    }

    private getNode(key: string): LruNode | null {
        const node = this.cache[key];

        if (node) {
            this.unlinkNode(node);
            this.moveNodeToHead(node);
        }
        return node;
    }

    get(key: string): cachableTypes | null {
        const node = this.getNode(key);
        return node ? node.value : null;
    }

    private updateExistingEntry(key: string, value: cachableTypes): any {
        const node = this.getNode(key) !;
        node.value = value;
    }

    private addNewEntry(key: string, value: cachableTypes): any {
        const newNode = new LruNode(key, value);
        this.cache[key] = newNode;
        this.moveNodeToHead(newNode);
        this.trimCache();
    }

    private trimCache() {
        while (this.count > this.depth) {
            const tail = this.tail;
            if (tail) {
                this.unlinkNode(tail);
                delete this.cache[tail.key];
            }
        }
    }
}


@Injectable()
export class RepLoaderService {

    constructor(
        private readonly http: Http,
        private readonly configService: ConfigService
    ) { }

    private loadingCount = 0;

    private loadingCountSource = new Subject<number>();

    loadingCount$ = this.loadingCountSource.asObservable();

    // use our own LRU cache 
    private cache = new SimpleLruCache(this.configService.config.httpCacheDepth);

    private addIfMatchHeader(config: RequestOptions, digest?: string | null) {
        if (digest && (config.method === RequestMethod.Post || config.method === RequestMethod.Put || config.method === RequestMethod.Delete)) {
            config.headers = new Headers({ "If-Match": digest });
        }
    }

    private handleInvalidResponse(rc: Models.ErrorCategory) {
        const rr = new Models.ErrorWrapper(rc,
            Models.ClientErrorCode.ConnectionProblem,
            "The response from the client was not parseable as a RestfulObject json Representation ");

        return Promise.reject(rr);
    }

    private handleError(response: Response, originalUrl : string) {
        let category: Models.ErrorCategory;
        let error: Models.ErrorRepresentation | Models.ErrorMap | string;

        if (response.status === Models.HttpStatusCode.InternalServerError) {
            // this error should contain an error representatation 
            if (Models.isErrorRepresentation(response.json())) {
                const errorRep = new Models.ErrorRepresentation();
                errorRep.populate(response.json() as Ro.IErrorRepresentation);
                category = Models.ErrorCategory.HttpServerError;
                error = errorRep;
            } else {
                return this.handleInvalidResponse(Models.ErrorCategory.HttpServerError);
            }
        } else if (response.status <= 0) {
            // failed to connect
            category = Models.ErrorCategory.ClientError;
            error = `Failed to connect to server: ${response.url || "unknown"}`;
        } else {
            category = Models.ErrorCategory.HttpClientError;
            const message = response.headers.get("warning") || "Unknown client HTTP error";

            if (response.status === Models.HttpStatusCode.BadRequest ||
                response.status === Models.HttpStatusCode.UnprocessableEntity) {
                // these errors should contain a map          
                error = new Models.ErrorMap(response.json() as Ro.IValueMap | Ro.IObjectOfType,
                    response.status,
                    message);
            } else if (response.status === Models.HttpStatusCode.NotFound) {
                category = Models.ErrorCategory.ClientError;
                error = `Failed to connect to server: ${response.url || "unknown"}`;
            }
            else {
                error = message;
            }
        }

        const rr = new Models.ErrorWrapper(category, response.status as Models.HttpStatusCode, error, originalUrl);

        return Promise.reject(rr);
    }


    private httpValidate(config: RequestOptions): Promise<boolean> {
        this.loadingCountSource.next(++(this.loadingCount));

        return this.http.request(new Request(config))
            .toPromise()
            .then(() => {
                this.loadingCountSource.next(--(this.loadingCount));
                return Promise.resolve(true);
            })
            .catch((r: Response) => {
                this.loadingCountSource.next(--(this.loadingCount));
                r.url = r.url || config.url;
                return this.handleError(r, config.url);
            });
    }

    // special handler for case where we receive a redirected object back from server 
    // instead of an actionresult. Wrap the object in an actionresult and then handle normally
    private handleRedirectedObject(response: Models.IHateoasModel, data: Ro.IRepresentation) {

        if (response instanceof Models.ActionResultRepresentation && Models.isIDomainObjectRepresentation(data)) {
            const actionResult: Ro.IActionInvokeRepresentation = {
                resultType: "object",
                result: data,
                links: [],
                extensions: {}
            }
            return actionResult;
        }

        return data;
    }

    private isValidResponse(data: any) {
        return Models.isResourceRepresentation(data);
    }


    private httpPopulate(config: RequestOptions, ignoreCache: boolean, response: Models.IHateoasModel): Promise<Models.IHateoasModel> {

        if (ignoreCache) {
            // clear cache of existing values
            this.cache.remove(config.url);
        } else {
            const cachedValue = this.cache.get(config.url);

            if (cachedValue) {
                response.populate(cachedValue);
                //response.etagDigest = r.headers.get("ETag");
                return Promise.resolve(response);
            }
        }

        this.loadingCountSource.next(++(this.loadingCount));

        return this.http.request(new Request(config))
            .toPromise()
            .then((r: Response) => {
                this.loadingCountSource.next(--(this.loadingCount));
                const asJson = r.json();
                if (!this.isValidResponse(asJson)) {
                    return this.handleInvalidResponse(Models.ErrorCategory.ClientError);
                }

                const representation = this.handleRedirectedObject(response, asJson);
                this.cache.add(config.url, representation);
                response.populate(representation);
                response.etagDigest = r.headers.get("ETag");
                return Promise.resolve(response);
            })
            .catch((r: Response) => {
                this.loadingCountSource.next(--(this.loadingCount));
                r.url = r.url || config.url;
                return this.handleError(r, config.url);
            });
    }

    populate = <T extends Models.IHateoasModel>(model: Models.IHateoasModel, ignoreCache?: boolean): Promise<T> => {

        const response = model;

        const config = new RequestOptions({
            withCredentials: true,
            url: model.getUrl(),
            method: model.method,
            body: model.getBody()
        });

        return this.httpPopulate(config, !!ignoreCache, response);
    }

    setConfigFromMap(map: Models.IHateoasModel, digest?: string | null) {

        const config = new RequestOptions({
            withCredentials: true,
            url: map.getUrl(),
            method: map.method,
            body: map.getBody()
        });

        this.addIfMatchHeader(config, digest);
        return config;
    }


    retrieve = <T extends Models.IHateoasModel>(map: Models.IHateoasModel,
        rc: { new (): Models.IHateoasModel },
        digest?: string | null): Promise<T> => {
        const response = new rc();
        const config = this.setConfigFromMap(map, digest);
        return this.httpPopulate(config, true, response);
    }

    validate = (map: Models.IHateoasModel, digest?: string): Promise<boolean> => {
        const config = this.setConfigFromMap(map, digest);
        return this.httpValidate(config);
    }

    retrieveFromLink = <T extends Models.IHateoasModel>(link: Models.Link | null, parms?: _.Dictionary<Object>): Promise<T> => {

        if (link) {
            const response = link.getTarget();
            let urlParms = "";

            if (parms) {
                const urlParmString = _.reduce(parms, (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n, "");
                urlParms = urlParmString !== "" ? `?${urlParmString}` : "";
            }

            const config = new RequestOptions({
                method: link.method(),
                url: link.href() + urlParms,
                withCredentials: true
                //headers: new Headers({ "Accept": mt })
            });

            return this.httpPopulate(config, true, response);
        }
        return Promise.reject("link must not be null");
    }


    invoke = (action: Models.IInvokableAction, parms: _.Dictionary<Models.Value>, urlParms: _.Dictionary<Object>): Promise<Models.ActionResultRepresentation> => {
        const invokeMap = action.getInvokeMap();
        if (invokeMap) {
            _.each(urlParms, (v, k) => invokeMap.setUrlParameter(k!, v));
            _.each(parms, (v, k) => invokeMap.setParameter(k!, v));
            return this.retrieve(invokeMap, Models.ActionResultRepresentation);
        }
        return Promise.reject(`attempting to invoke uninvokable action ${action.actionId()}`);
    }

    clearCache = (url: string) => {
        this.cache.remove(url);
    }

    addToCache = (url: string, m: Ro.IResourceRepresentation) => {
        this.cache.add(url, m);
    };

    getFile = (url: string, mt: string, ignoreCache: boolean): Promise<Blob> => {

        if (ignoreCache) {
            // clear cache of existing values
            this.cache.remove(url);
        } else {
            const blob = this.cache.get(url);
            if (blob) {
                return Promise.resolve(blob);
            }
        }

        const config = new RequestOptions({
            method: "GET",
            url: url,
            responseType: ResponseContentType.Blob,
            headers: new Headers({ "Accept": mt })
        });

        const request = new Request(config);

        return this.http.request(request)
            .toPromise()
            .then((r: Response) => {
                const blob = r.blob();
                this.cache.add(config.url, blob);
                return blob;
            })
            .catch((r: Response) => {
                r.url = r.url || config.url;
                return this.handleError(r, config.url);
            });
    }

    uploadFile = (url: string, mt: string, file: Blob): Promise<boolean> => {

        const config = new RequestOptions({
            method: "POST",
            url: url,
            body: file,
            headers: new Headers({ "Content-Type": mt })
        });

        const request = new Request(config);

        return this.http.request(request)
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