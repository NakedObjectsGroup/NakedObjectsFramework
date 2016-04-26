/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />


module NakedObjects {
    import IHateoasModel = Models.IHateoasModel;
    import Link = Models.Link;
    import ActionMember = Models.ActionMember;
    import Value = Models.Value;
    import ActionResultRepresentation = Models.ActionResultRepresentation;
    import ErrorCategory = Models.ErrorCategory;
    import ErrorRepresentation = Models.ErrorRepresentation;
    import ErrorMap = Models.ErrorMap;
    import HttpStatusCode = Models.HttpStatusCode;
    import ErrorWrapper = Models.ErrorWrapper;
    import IInvokableAction = NakedObjects.Models.IInvokableAction;

    export interface IRepLoader {
        validate: (map: IHateoasModel, digest?: string) => ng.IPromise<boolean>;
        retrieve: <T extends IHateoasModel>(map: IHateoasModel, rc: { new(): IHateoasModel }, digest?: string) => ng.IPromise<T>;
        retrieveFromLink: <T extends IHateoasModel>(link: Link, parms?: _.Dictionary<Object>) => ng.IPromise<T>;
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean) => ng.IPromise<T>;
        invoke: (action: IInvokableAction, parms: _.Dictionary<Value>, urlParms: _.Dictionary<Object>) => ng.IPromise<ActionResultRepresentation>;
        getFile: (url: string, mt: string, ignoreCache: boolean) => ng.IPromise<Blob>;
        clearCache: (url : string) => void ; 
    }

    app.service("repLoader", function($http: ng.IHttpService, $q: ng.IQService, $rootScope: ng.IRootScopeService, $cacheFactory: ng.ICacheFactoryService) {

        const repLoader = this as IRepLoader;
        let loadingCount = 0;

        function addIfMatchHeader(config: ng.IRequestConfig, digest: string) {
            if (digest && (config.method === "POST" || config.method === "PUT" || config.method === "DELETE")) {
                config.headers = { "If-Match": digest };
            }
        }

        function handleError(promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) {
            let message: string;
            let category: ErrorCategory;
            let error: ErrorRepresentation | ErrorMap | string = null;

            if (promiseCallback.status === HttpStatusCode.InternalServerError) {
                // this error contains an error representatation 
                const errorRep = new ErrorRepresentation();
                errorRep.populate(promiseCallback.data as RoInterfaces.IErrorRepresentation);
                category = ErrorCategory.HttpServerError;
                error = errorRep;
            }
            else if (promiseCallback.status === -1) {
                // failed to connect
                category = ErrorCategory.ClientError;
                error = `Failed to connect to server: ${promiseCallback.config ? promiseCallback.config.url : "unknown"}`;
            } else {
                category = ErrorCategory.HttpClientError;
                message = promiseCallback.headers("warning") || "Unknown client HTTP error";

                if (promiseCallback.status === HttpStatusCode.BadRequest || promiseCallback.status === HttpStatusCode.UnprocessableEntity) {
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
            $rootScope.$broadcast("ajax-change", ++loadingCount);

            return $http(config).
                then(() => {
                    return $q.when(true);
                }).
                catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {                   
                    return handleError(promiseCallback);
                }).
                finally(() => {
                    $rootScope.$broadcast("ajax-change", --loadingCount);
                });
        }


        function httpPopulate(config: ng.IRequestConfig, ignoreCache: boolean, response: IHateoasModel): ng.IPromise<IHateoasModel> {
            $rootScope.$broadcast("ajax-change", ++loadingCount);

            if (ignoreCache) {
                // clear cache of existing values
                $cacheFactory.get("$http").remove(config.url);
            }

            return $http(config).
                then((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IResourceRepresentation>) => {
                    response.populate(promiseCallback.data);
                    response.etagDigest = promiseCallback.headers("ETag");
                    return $q.when(response);
                }).
                catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {
                    return handleError(promiseCallback);
                }).
                finally(() => {
                    $rootScope.$broadcast("ajax-change", --loadingCount);
                });
        }

        repLoader.populate = <T extends IHateoasModel>(model: IHateoasModel, ignoreCache?: boolean): ng.IPromise<T> => {

            const response = model;
            const useCache = !ignoreCache;

            const config = {
                withCredentials: true,
                url: model.getUrl(),
                method: model.method,
                cache: useCache,
                data: model.getBody()
            };

            return httpPopulate(config, ignoreCache, response);
        };

        function setConfigFromMap(map: IHateoasModel, digest? : string) {
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


        repLoader.retrieve = <T extends IHateoasModel>(map: IHateoasModel, rc: { new(): IHateoasModel }, digest?: string): ng.IPromise<T> => {
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
                const urlParmString = _.reduce(parms, (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n, "");
                urlParms = urlParmString !== "" ?  `?${urlParmString}` : "";
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
            $cacheFactory.get("$http").remove(url);
        }

        repLoader.getFile = (url: string, mt: string, ignoreCache: boolean): ng.IPromise<Blob> => {

            if (ignoreCache) {
                // clear cache of existing values
                $cacheFactory.get("$http").remove(url);
            }

            const config: ng.IRequestConfig = {
                method: "GET",
                url: url,
                responseType: "blob",
                headers: { "Accept": mt },
                cache: true
            };

            return $http(config).
                then((promiseCallback: ng.IHttpPromiseCallbackArg<Blob>) => {
                    return $q.when(promiseCallback.data);
                }).
                catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {
                    return handleError(promiseCallback);
                });
        }

    });

}