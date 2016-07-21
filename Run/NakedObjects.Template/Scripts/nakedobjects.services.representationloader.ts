/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />


namespace NakedObjects {
    import IHateoasModel = Models.IHateoasModel;
    import Link = Models.Link;
    import Value = Models.Value;
    import ActionResultRepresentation = Models.ActionResultRepresentation;
    import ErrorCategory = Models.ErrorCategory;
    import ErrorRepresentation = Models.ErrorRepresentation;
    import ErrorMap = Models.ErrorMap;
    import HttpStatusCode = Models.HttpStatusCode;
    import ErrorWrapper = Models.ErrorWrapper;
    import IInvokableAction = Models.IInvokableAction;
    import IResourceRepresentation = RoInterfaces.IResourceRepresentation;
    import ActionInvokeRepresentation = RoInterfaces.IActionInvokeRepresentation;
    import isIDomainObjectRepresentation = Models.isIDomainObjectRepresentation;
    import isResourceRepresentation = Models.isResourceRepresentation;
    import ClientErrorCode = Models.ClientErrorCode;
    import isErrorRepresentation = Models.isErrorRepresentation;

    export interface IRepLoader {
        validate: (map: IHateoasModel, digest?: string) => ng.IPromise<boolean>;
        retrieve: <T extends IHateoasModel>(map: IHateoasModel, rc: { new(): IHateoasModel }, digest?: string) => ng.
        IPromise<T>;
        retrieveFromLink: <T extends IHateoasModel>(link: Link, parms?: _.Dictionary<Object>) => ng.IPromise<T>;
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean) => ng.IPromise<T>;
        invoke: (action: IInvokableAction, parms: _.Dictionary<Value>, urlParms: _.Dictionary<Object>) => ng.
        IPromise<ActionResultRepresentation>;
        getFile: (url: string, mt: string, ignoreCache: boolean) => ng.IPromise<Blob>;
        uploadFile : (url: string, mt: string, file: Blob) => ng.IPromise<boolean>;
        clearCache: (url: string) => void;
        addToCache: (url: string, m: IResourceRepresentation) => void;
    }

    app.service("repLoader",
        function($http: ng.IHttpService,
            $q: ng.IQService,
            $rootScope: ng.IRootScopeService,
            $cacheFactory: ng.ICacheFactoryService) {

            const repLoader = this as IRepLoader;
            let loadingCount = 0;

            // use our own LRU cache 
            const cache = $cacheFactory("nof-cache", { capacity: httpCacheDepth });

            function addIfMatchHeader(config: ng.IRequestConfig, digest: string) {
                if (digest && (config.method === "POST" || config.method === "PUT" || config.method === "DELETE")) {
                    config.headers = { "If-Match": digest };
                }
            }

            function handleInvalidResponse(rc : ErrorCategory) {
                const rr = new ErrorWrapper(rc,
                                            ClientErrorCode.ConnectionProblem,
                                            "The response from the client was not parseable as a RestfulObject json Representation ");

                return $q.reject(rr);
            }

            function handleError(promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>)   {
                let category: ErrorCategory;
                let error: ErrorRepresentation | ErrorMap | string;

                if (promiseCallback.status === HttpStatusCode.InternalServerError) {
                    // this error should contain an error representatation 
                    if (isErrorRepresentation(promiseCallback.data)) {
                        const errorRep = new ErrorRepresentation();
                        errorRep.populate(promiseCallback.data as RoInterfaces.IErrorRepresentation);
                        category = ErrorCategory.HttpServerError;
                        error = errorRep;
                    } else {
                        return handleInvalidResponse(ErrorCategory.HttpServerError);
                    }
                } else if (promiseCallback.status === -1) {
                    // failed to connect
                    category = ErrorCategory.ClientError;
                    error = `Failed to connect to server: ${promiseCallback.config ? promiseCallback.config.url : "unknown"}`;
                } else {
                    category = ErrorCategory.HttpClientError;
                    const message = promiseCallback.headers("warning") || "Unknown client HTTP error";

                    if (promiseCallback.status === HttpStatusCode.BadRequest ||
                        promiseCallback.status === HttpStatusCode.UnprocessableEntity) {
                        // these errors should contain a map          
                        error = new ErrorMap(promiseCallback.data as RoInterfaces.IValueMap | RoInterfaces.IObjectOfType,
                            promiseCallback.status,
                            message);
                    } else {
                        error = message;
                    }
                }

                const rr = new ErrorWrapper(category, promiseCallback.status, error);

                return $q.reject(rr);
            }


            function httpValidate(config: ng.IRequestConfig): ng.IPromise<boolean> {
                $rootScope.$broadcast(geminiAjaxChangeEvent, ++loadingCount);

                return $http(config)
                    .then(() => {
                        return $q.when(true);
                    })
                    .catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {
                        return handleError(promiseCallback);
                    })
                    .finally(() => {
                        $rootScope.$broadcast(geminiAjaxChangeEvent, --loadingCount);
                    });
            }

            // special handler for case whwre we reciece a redirected object back from server 
            // instead of an actionresult. Wrap the object in an actionresult and then handle normally
            function handleRedirectedObject(response : IHateoasModel,  data: RoInterfaces.IRepresentation) {

                if (response instanceof ActionResultRepresentation && isIDomainObjectRepresentation(data)) {
                    const actionResult: ActionInvokeRepresentation = {
                        resultType: "object",
                        result: data,
                        links: [],
                        extensions: {}
                    }
                    return actionResult;
                }

                return data;
            }

            function isValidResponse(data: any) {
                return isResourceRepresentation(data);
            }



            function httpPopulate(config: ng.IRequestConfig, ignoreCache: boolean, response: IHateoasModel): ng.IPromise<IHateoasModel> {
                $rootScope.$broadcast(geminiAjaxChangeEvent, ++loadingCount);

                if (ignoreCache) {
                    // clear cache of existing values
                    cache.remove(config.url);
                }


                if ((<any>config).doesnotexist) {
                    config.url = "http://www.google.co.uk";
                }


                return $http(config)
                    .then((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IResourceRepresentation>) => {
                        if (!isValidResponse(promiseCallback.data)) {
                            return handleInvalidResponse(ErrorCategory.ClientError);
                        }

                        const representation = handleRedirectedObject(response, promiseCallback.data);
                        response.populate(representation);
                        response.etagDigest = promiseCallback.headers("ETag");
                        return $q.when(response);
                    })
                    .catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {
                        return handleError(promiseCallback);
                    })
                    .finally(() => {
                        $rootScope.$broadcast(geminiAjaxChangeEvent, --loadingCount);
                    });
            }

            repLoader.populate = <T extends IHateoasModel>(model: IHateoasModel, ignoreCache?: boolean): ng.IPromise<T> => {

                const response = model;
                const useCache = !ignoreCache;

                const config = {
                    withCredentials: true,
                    url: model.getUrl(),
                    method: model.method,
                    cache: useCache ? cache : false,
                    data: model.getBody()
                };

                return httpPopulate(config, ignoreCache, response);
            };

            function setConfigFromMap(map: IHateoasModel, digest?: string) {
                const config = {
                    withCredentials: true,
                    url: map.getUrl(),
                    method: map.method,
                    cache: false,
                    data: map.getBody()
                };

                addIfMatchHeader(config, digest);
                return config;
            }


            repLoader.retrieve = <T extends IHateoasModel>(map: IHateoasModel,
                rc: { new(): IHateoasModel },
                digest?: string): ng.IPromise<T> => {
                const response = new rc();
                const config = setConfigFromMap(map, digest);
                return httpPopulate(config, true, response);
            };

            repLoader.validate = (map: IHateoasModel, digest?: string): ng.IPromise<boolean> => {
                const config = setConfigFromMap(map, digest);
                return httpValidate(config);
            };

            repLoader.retrieveFromLink = <T extends IHateoasModel>(link: Link, parms?: _.Dictionary<Object>): ng.IPromise<T> => {

                const response = link.getTarget();
                let urlParms = "";

                if (parms) {
                    const urlParmString = _.reduce(parms,
                        (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n,
                        "");
                    urlParms = urlParmString !== "" ? `?${urlParmString}` : "";
                }

                const config = {
                    withCredentials: true,
                    url: link.href() + urlParms,
                    method: link.method(),
                    cache: false
                };

                return httpPopulate(config, true, response);
            };


            repLoader.invoke = (action: IInvokableAction, parms: _.Dictionary<Value>, urlParms: _.Dictionary<Object>): ng.IPromise<ActionResultRepresentation> => {
                const invokeMap = action.getInvokeMap();
                _.each(urlParms, (v, k) => invokeMap.setUrlParameter(k, v));
                _.each(parms, (v, k) => invokeMap.setParameter(k, v));
                return this.retrieve(invokeMap, ActionResultRepresentation);
            };

            repLoader.clearCache = (url: string) => {
                cache.remove(url);
            };

            repLoader.addToCache = (url: string, m: IResourceRepresentation) => {                
                cache.put(url, m);
            };

            repLoader.getFile = (url: string, mt: string, ignoreCache: boolean): ng.IPromise<Blob> => {

                if (ignoreCache) {
                    // clear cache of existing values
                    cache.remove(url);
                }

                const config: ng.IRequestConfig = {
                    method: "GET",
                    url: url,
                    responseType: "blob",
                    headers: { "Accept": mt },
                    cache: cache
                };

                return $http(config)
                    .then((promiseCallback: ng.IHttpPromiseCallbackArg<Blob>) => {
                        return $q.when(promiseCallback.data);
                    })
                    .catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {
                        return handleError(promiseCallback);
                    });
            };

            repLoader.uploadFile = (url: string, mt: string, file: Blob): ng.IPromise<boolean> => {


                const config: ng.IRequestConfig = {
                    method: "POST",
                    url: url,
                    data : file,
                    headers: { "Content-Type": mt }
                };

                return $http(config)
                    .then((promiseCallback) => {
                        return $q.when(true);
                    })
                    .catch((promiseCallback) => {
                        return $q.when(false);
                    });
            };


            function logoff() {
                cache.removeAll();
            }

            $rootScope.$on(geminiLogoffEvent, () => logoff());
        });

}