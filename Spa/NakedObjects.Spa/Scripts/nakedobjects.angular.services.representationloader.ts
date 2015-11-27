/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular {

    export interface IRepLoader {
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean, r?: IHateoasModel) => ng.IPromise<T>;
        invoke: (action: ActionMember, parms: _.Dictionary<Value>, urlParms : _.Dictionary<string>) => ng.IPromise<ActionResultRepresentation>;
    }

    // TODO investigate using transformations to transform results 
    app.service("repLoader", function ($http : ng.IHttpService, $q : ng.IQService, $rootScope : ng.IRootScopeService) {

        const repLoader = this; 
        repLoader.loadingCount = 0; 

        // todo this should be on model ! 
        function getUrl(model: IHateoasModel): string {
            const url = model.url();
            const attrAsJson = _.clone((<any>model).attributes);

            if (_.keys(attrAsJson).length > 0 && (model.method === "GET" || model.method === "DELETE")) {
                
                const urlParmsAsJson = _.clone(model.urlParms);
                const asJson = _.merge(attrAsJson, urlParmsAsJson);
                if (_.keys(asJson).length > 0) {
                    const map = JSON.stringify(asJson);
                    const parmString = encodeURI(map);
                    return url + "?" + parmString;
                }
                return url;
            }

            const urlParmString = _.reduce(model.urlParms || {},  (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n, "");
                  
            return urlParmString !== "" ? url + "?" + urlParmString : url;
        }

         // todo this should be on model too ! 
        function getData(model: IHateoasModel): Object {

            let data = {};

            if (model.method === "POST" || model.method === "PUT") {
                data = _.clone((<any>model).attributes);
            }

            return data;
        }

        repLoader.populate = <T>(model: IHateoasModel, ignoreCache?: boolean, expected?: IHateoasModel): ng.IPromise<T> => {

            const response = expected || model;
            const useCache = !ignoreCache;

            const config = {
                url: getUrl(model),
                method: model.method,
                cache: useCache,
                data: model.getBody()
            };

            $rootScope.$broadcast("ajax-change", ++this.loadingCount); 

            return $http(config).
                then(function (promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IResourceRepresentation>) {
                    response.populate(promiseCallback.data);
                    $rootScope.$broadcast("ajax-change", --this.loadingCount);
                    return $q.when(response);
                }).
                catch(function (promiseCallback: ng.IHttpPromiseCallbackArg<RoInterfaces.IRepresentation>) {

                    let reason: ErrorRepresentation | ErrorMap | string; 

                    if (promiseCallback.status === 500) {
                        const error  = new ErrorRepresentation();  
                        error.populate(promiseCallback.data as RoInterfaces.IResourceRepresentation);
                        reason = error;
                    }
                    else if (promiseCallback.status === 400 || promiseCallback.status === 422) {
                        reason = new ErrorMap(promiseCallback.data as RoInterfaces.IValueMap | RoInterfaces.IObjectOfType, promiseCallback.status, promiseCallback.headers("warning"));
                    }
                    else {
                        reason = promiseCallback.headers("warning");
                    }
                    $rootScope.$broadcast("ajax-change", --this.loadingCount);
                    return $q.reject(reason);
                });
        };

       

        repLoader.invoke = (action: ActionMember, parms: _.Dictionary<Value>, urlParms: _.Dictionary<string>): ng.IPromise < ActionResultRepresentation > => {
            const invoke = action.getInvoke(); 
            _.each(urlParms, (v, k) => invoke.setUrlParameter(k, v));                                      
            _.each(parms, (v, k) => invoke.setParameter(k, v));
            return this.populate (invoke, true);
        }

    });

}