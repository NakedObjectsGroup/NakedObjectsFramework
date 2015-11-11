/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular {

    export interface IRepLoader {
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean, r?: IHateoasModel) => ng.IPromise<T>;
        invoke: (action: ActionMember, parms: _.Dictionary<Value>, urlParms : _.Dictionary<string>) => ng.IPromise<ActionResultRepresentation>;
    }

    // TODO investigate using transformations to transform results 
    app.service("repLoader", function ($http, $q, $rootScope) {

        const repLoader = this; 
        repLoader.loadingCount = 0; 

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

            const delay = $q.defer();

            const config = {
                url: getUrl(model),
                method: model.method,
                cache: useCache,
                data: getData(model)
            };

            $rootScope.$broadcast("ajax-change", ++this.loadingCount); 
            $http(config).
                success(function (data, status, headers, config) {
                    (<any>response).attributes = data; // TODO make typed 
                    delay.resolve(response);
                    $rootScope.$broadcast("ajax-change", --this.loadingCount); 
                }).
                error(function (data, status, headers, config) {

                    if (status === 500) {
                        var error = new ErrorRepresentation(data);
                        delay.reject(error);
                    }
                    else if (status === 400 || status === 422) {
                        var errorMap = new ErrorMap(data, status, headers().warning);
                        delay.reject(errorMap);
                    }
                    else {
                        delay.reject(headers().warning);
                    }
                    $rootScope.$broadcast("ajax-change", --this.loadingCount); 
                });

            return delay.promise;
        };


        repLoader.invoke = (action: ActionMember, parms: _.Dictionary<Value>, urlParms: _.Dictionary<string>): ng.IPromise < ActionResultRepresentation > => {
            const invoke = action.getInvoke(); 
            _.each(urlParms, (v, k) => invoke.setUrlParameter(k, v));                                      
            _.each(parms, (v, k) => invoke.setParameter(k, v));
            return this.populate (invoke, true);
        }

    });

}