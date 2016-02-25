/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular {

    export interface IRepLoader {
        retrieve: <T extends IHateoasModel>(map: IHateoasModel, rc: { new (): IHateoasModel }, digest? : string) => ng.IPromise<T>;
        retrieveFromLink: <T extends IHateoasModel>(link : Link) => ng.IPromise<T>;
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean) => ng.IPromise<T>;
        invoke: (action: ActionMember, parms: _.Dictionary<Value>, urlParms : _.Dictionary<string>) => ng.IPromise<ActionResultRepresentation>;
    }

    app.service("repLoader", function ($http : ng.IHttpService, $q : ng.IQService, $rootScope : ng.IRootScopeService, $cacheFactory : ng.ICacheFactoryService ) {

        const repLoader = this as IRepLoader;
        let loadingCount = 0;

        function addIfMatchHeader(config: ng.IRequestConfig, digest : string) {
            if (digest && (config.method === "POST" || config.method === "PUT" || config.method === "DELETE")) {
                config.headers = { "If-Match": digest };
            }
        }

        function httpPopulate(config : ng.IRequestConfig, ignoreCache : boolean, response : IHateoasModel) : ng.IPromise<IHateoasModel> {
            $rootScope.$broadcast("ajax-change", ++loadingCount);

            if (ignoreCache) {
                // clear cache of existing values
                $cacheFactory.get("$http").remove(config.url);
            }

            return $http(config).
                then((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IResourceRepresentation>) => {
                    response.populate(promiseCallback.data);
                    response.etagDigest = promiseCallback.headers("ETag");
                    $rootScope.$broadcast("ajax-change", --loadingCount);
                    return $q.when(response);
                }).
                catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {
                    
                    let message: string;
                    let category: ErrorCategory;
                    let error: ErrorRepresentation | ErrorMap = null;

                    if (promiseCallback.status === HttpStatusCode.InternalServerError) {
                        // this error contains an error representatation 
                        const errorRep = new ErrorRepresentation();
                        errorRep.populate(promiseCallback.data as RoInterfaces.IErrorRepresentation);
                        category = ErrorCategory.HttpServerError;
                        error = errorRep;
                        message = promiseCallback.headers("warning") || "Unknown server HTTP error";
                    } else {
                        category = ErrorCategory.HttpClientError;
                        message = promiseCallback.headers("warning") || "Unknown client HTTP error";

                        if (promiseCallback.status === HttpStatusCode.BadRequest || promiseCallback.status === HttpStatusCode.UnprocessableEntity) {
                            // these errors should contain a map                            
                            error = new ErrorMap(promiseCallback.data as RoInterfaces.IValueMap | RoInterfaces.IObjectOfType, promiseCallback.status, message);
                        } 
                    }

                    $rootScope.$broadcast("ajax-change", --loadingCount);

                    const rr = new ErrorWrapper(category, promiseCallback.status, error);

                    return $q.reject(rr);
                });

        }

        repLoader.populate = <T extends IHateoasModel>(model: IHateoasModel, ignoreCache?: boolean): ng.IPromise<T> => {

            const response =  model;
            const useCache = !ignoreCache;

            const config = {
                withCredentials : true,
                url: model.getUrl(),
                method: model.method,
                cache: useCache,
                data: model.getBody()
            };

            return httpPopulate(config, ignoreCache, response);
        };

        repLoader.retrieve = <T extends IHateoasModel > (map: IHateoasModel, rc : { new () : IHateoasModel }, digest? : string): ng.IPromise<T> => {

            const response = new rc();

            const config = {
                withCredentials: true,
                url: map.getUrl(),
                method: map.method,
                cache: false,
                data: map.getBody()
            };

            addIfMatchHeader(config, digest);

            return httpPopulate(config, true, response);
        };

       
        repLoader.retrieveFromLink = <T extends IHateoasModel>(link : Link): ng.IPromise<T> => {

            const response = link.getTarget();

            const config = {
                withCredentials: true,
                url: link.href(),
                method: link.method(),
                cache: false
            };

            return httpPopulate(config, true, response);
        };



        repLoader.invoke = (action: ActionMember, parms: _.Dictionary<Value>, urlParms: _.Dictionary<string>): ng.IPromise < ActionResultRepresentation > => {
            const invokeMap = action.getInvokeMap();
            _.each(urlParms, (v, k) => invokeMap.setUrlParameter(k, v));                                      
            _.each(parms, (v, k) => invokeMap.setParameter(k, v));
            return this.retrieve (invokeMap, ActionResultRepresentation);
        }
    });

}