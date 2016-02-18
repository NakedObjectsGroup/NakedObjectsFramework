/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular {

    export interface IRepLoader {
        retrieve : <T extends IHateoasModel>(map: IHateoasModel, rc: { new (): IHateoasModel }, ignoreCache?: boolean) => ng.IPromise<T>;
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean, r?: IHateoasModel) => ng.IPromise<T>;
        invoke: (action: ActionMember, parms: _.Dictionary<Value>, urlParms : _.Dictionary<string>) => ng.IPromise<ActionResultRepresentation>;
    }

    // TODO investigate using transformations to transform results 
    app.service("repLoader", function ($http : ng.IHttpService, $q : ng.IQService, $rootScope : ng.IRootScopeService, $cacheFactory : ng.ICacheFactoryService ) {

        const repLoader = this as IRepLoader;
        let loadingCount = 0;

        function httpPopulate(config : ng.IRequestConfig, ignoreCache : boolean, response : IHateoasModel) : ng.IPromise<IHateoasModel> {
            $rootScope.$broadcast("ajax-change", ++loadingCount);

            if (ignoreCache) {
                // clear cache of existing values
                $cacheFactory.get("$http").remove(config.url);
            }

            return $http(config).
                then((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IResourceRepresentation>) => {
                    response.populate(promiseCallback.data);
                    $rootScope.$broadcast("ajax-change", --loadingCount);
                    return $q.when(response);
                }).
                catch((promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) => {

                    let reason: ErrorRepresentation | ErrorMap | string;

                    if (promiseCallback.status === 500) {
                        const error = new ErrorRepresentation();
                        error.populate(promiseCallback.data as RoInterfaces.IErrorRepresentation);
                        reason = error;
                    }
                    else if (promiseCallback.status === 400 || promiseCallback.status === 422) {
                        reason = new ErrorMap(promiseCallback.data as RoInterfaces.IValueMap | RoInterfaces.IObjectOfType, promiseCallback.status, promiseCallback.headers("warning"));
                    }
                    else {
                        reason = promiseCallback.headers("warning") || "unknown server error";
                    }
                    $rootScope.$broadcast("ajax-change", --loadingCount);
                    return $q.reject(reason);
                });

        }


        // todo change this - make T an IHateosModel create a new and return 
        repLoader.populate = <T extends IHateoasModel>(model: IHateoasModel, ignoreCache?: boolean, expected?: IHateoasModel): ng.IPromise<T> => {

            const response = expected || model;
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

        repLoader.retrieve = <T extends IHateoasModel > (map: IHateoasModel, rc : { new () : IHateoasModel }, ignoreCache?: boolean): ng.IPromise<T> => {

            const response = new rc();
            const useCache = !ignoreCache;

            const config = {
                withCredentials: true,
                url: map.getUrl(),
                method: map.method,
                cache: useCache,
                data: map.getBody()
            };

            return httpPopulate(config, ignoreCache, response);
        };

       
        repLoader.invoke = (action: ActionMember, parms: _.Dictionary<Value>, urlParms: _.Dictionary<string>): ng.IPromise < ActionResultRepresentation > => {
            const invoke = action.getInvoke();
            const invokeMap = invoke.getInvokeMap();
            _.each(urlParms, (v, k) => invokeMap.setUrlParameter(k, v));                                      
            _.each(parms, (v, k) => invokeMap.setParameter(k, v));
            return this.populate (invokeMap, true, invoke);
        }
    });

}