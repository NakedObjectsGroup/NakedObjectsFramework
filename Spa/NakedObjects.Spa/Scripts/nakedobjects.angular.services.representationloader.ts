/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular {

    export interface IRepLoader {
        retrieve : <T extends IHateoasModel>(map: IHateoasModel, rc: { new (): IHateoasModel }) => ng.IPromise<T>;
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean) => ng.IPromise<T>;
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

        repLoader.retrieve = <T extends IHateoasModel > (map: IHateoasModel, rc : { new () : IHateoasModel }): ng.IPromise<T> => {

            const response = new rc();

            const config = {
                withCredentials: true,
                url: map.getUrl(),
                method: map.method,
                cache: false,
                data: map.getBody()
            };

            return httpPopulate(config, true, response);
        };

       
        repLoader.invoke = (action: ActionMember, parms: _.Dictionary<Value>, urlParms: _.Dictionary<string>): ng.IPromise < ActionResultRepresentation > => {
            const invokeMap = action.getInvoke().getInvokeMap();
            _.each(urlParms, (v, k) => invokeMap.setUrlParameter(k, v));                                      
            _.each(parms, (v, k) => invokeMap.setParameter(k, v));
            return this.retrieve (invokeMap, ActionResultRepresentation);
        }
    });

}