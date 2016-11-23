import { Injectable } from '@angular/core';
import { Http, Response, Request, RequestOptions, Headers, RequestMethod, ResponseContentType} from '@angular/http';
import "./rxjs-extensions";
import * as Models from "./models";
import * as Constants from "./constants";
import * as Ro from "./ro-interfaces";
import * as _ from "lodash";
import { Subject } from 'rxjs/Subject';


@Injectable()
export class RepLoaderService {

    constructor(private http: Http) { }

    private loadingCount = 0;

    private loadingCountSource = new Subject<number>();
  
    loadingCount$ = this.loadingCountSource.asObservable();

// use our own LRU cache 
//private cache = $cacheFactory("nof-cache", { capacity: httpCacheDepth });

    private addIfMatchHeader(config: RequestOptions, digest: string) {
        if (digest && (config.method === RequestMethod.Post || config.method === RequestMethod.Put || config.method === RequestMethod.Delete)) {
            config.headers = new Headers ({ "If-Match": digest });
        }
    }

    private handleInvalidResponse(rc: Models.ErrorCategory) {
        const rr = new Models.ErrorWrapper(rc,
            Models.ClientErrorCode.ConnectionProblem,
            "The response from the client was not parseable as a RestfulObject json Representation ");

        return Promise.reject(rr);
    }

    private handleError(response: Response) {
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
        } else if (response.status === -1) {
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
            } else {
                error = message;
            }
        }

        const rr = new Models.ErrorWrapper(category, response.status as Models.HttpStatusCode, error);

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
                return <any>this.handleError(r);
            });
          
    }

// special handler for case whwre we reciece a redirected object back from server 
// instead of an actionresult. Wrap the object in an actionresult and then handle normally
    private handleRedirectedObject(response: Models.IHateoasModel, data: Ro.IRepresentation) {

        //if (response instanceof Models.ActionResultRepresentation && Models.isIDomainObjectRepresentation(data)) {
        //    const actionResult: Models.ActionResultRepresentation = {
        //        resultType: "object",
        //        result: data,
        //        links: [],
        //        extensions: {}
        //    }
        //    return actionResult;
        //}

        return data;
    }

    private isValidResponse(data: any) {
        return Models.isResourceRepresentation(data);
    }


    private httpPopulate(config: RequestOptions, ignoreCache: boolean, response: Models.IHateoasModel): Promise<Models.IHateoasModel> {
        this.loadingCountSource.next(++(this.loadingCount));

        if (ignoreCache) {
            // clear cache of existing values
            //cache.remove(config.url);
        }

        return this.http.request(new Request(config))
            .toPromise()
            .then((r: Response) => {
                this.loadingCountSource.next(--(this.loadingCount));
                if (!this.isValidResponse(r.json())) {
                    return this.handleInvalidResponse(Models.ErrorCategory.ClientError);
                }

                const representation = this.handleRedirectedObject(response, r.json());
                response.populate(representation);
                response.etagDigest = r.headers.get("ETag");
                return <any>Promise.resolve(response);
            })
            .catch((r: Response) => {
                this.loadingCountSource.next(--(this.loadingCount));
                return <any>this.handleError(r);
            });         
    }

    populate = <T extends Models.IHateoasModel>(model: Models.IHateoasModel, ignoreCache?: boolean): Promise<T> => {

        const response = model;
        const useCache = !ignoreCache;

        //const config = {
        //    withCredentials: true,
        //    url: model.getUrl(),
        //    method: model.method,
        //    cache: useCache ? cache : false,
        //    data: model.getBody()
        //};

        const config = new RequestOptions({
         
                withCredentials: true,
                url: model.getUrl(),
                method: model.method,
                //cache: useCache ? cache : false,
                body: model.getBody()
        });

        return this.httpPopulate(config, ignoreCache, response);
    };

    setConfigFromMap(map: Models.IHateoasModel, digest?: string) {
        //const config = {
        //    withCredentials: true,
        //    url: map.getUrl(),
        //    method: map.method,
        //    cache: false,
        //    data: map.getBody()
        //};

        const config = new RequestOptions({
            withCredentials: true,
            url: map.getUrl(),
            method: map.method,
            //cache: false,
            body: map.getBody()
        });

        this.addIfMatchHeader(config, digest);
        return config;
    }


    retrieve = <T extends Models.IHateoasModel>(map: Models.IHateoasModel,
                                                rc: { new (): Models.IHateoasModel },
                                                digest?: string): Promise<T> => {
        const response = new rc();
        const config = this.setConfigFromMap(map, digest);
        return this.httpPopulate(config, true, response);
    };

    validate = (map: Models.IHateoasModel, digest?: string): Promise<boolean> => {
        const config = this.setConfigFromMap(map, digest);
        return this.httpValidate(config);
    };

    retrieveFromLink = <T extends Models.IHateoasModel>(link: Models.Link, parms?: _.Dictionary<Object>): Promise<T> => {

        const response = link.getTarget();
        let urlParms = "";

        if (parms) {
            const urlParmString = _.reduce(parms,
                (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n,
                "");
            urlParms = urlParmString !== "" ? `?${urlParmString}` : "";
        }

        //const config = {
        //    withCredentials: true,
        //    url: link.href() + urlParms,
        //    method: link.method(),
        //    cache: false
        //};

        const config = new RequestOptions({
            method: link.method(),
            url: link.href() + urlParms,
            withCredentials : true
            //headers: new Headers({ "Accept": mt })
        });

        const request = new Request(config);

        return this.httpPopulate(config, true, response);
    };


    invoke = (action: Models.IInvokableAction, parms: _.Dictionary<Models.Value>, urlParms: _.Dictionary<Object>): Promise<Models.ActionResultRepresentation> => {
        const invokeMap = action.getInvokeMap();
        _.each(urlParms, (v, k) => invokeMap.setUrlParameter(k, v));
        _.each(parms, (v, k) => invokeMap.setParameter(k, v));
        return this.retrieve(invokeMap, Models.ActionResultRepresentation);
    };

    clearCache = (url: string) => {
        //cache.remove(url);
    };

    addToCache = (url: string, m: Ro.IResourceRepresentation) => {
        //cache.put(url, m);
    };

    getFile = (url: string, mt: string, ignoreCache: boolean): Promise<Blob> => {

        if (ignoreCache) {
            // clear cache of existing values
            //cache.remove(url);
        }

        //const config: ng.IRequestConfig = {
        //    method: "GET",
        //    url: url,
        //    responseType: "blob",
        //    headers: { "Accept": mt },
        //    //cache: cache
        //};

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
                return r.blob();
            })
            .catch((r:Response) => {
                return this.handleError(r);
            });
    };

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
    };


    private logoff() {
        //cache.removeAll();
    }
}
        
