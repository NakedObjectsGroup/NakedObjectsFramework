/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular {

    export interface IRepLoader {
        populate: <T>(m: IHateoasModel, ignoreCache?: boolean, r?: IHateoasModel) => ng.IPromise<T>;
        invoke: (action : ActionMember, parms : {id : string, val : Value}[]) => ng.IPromise<ActionResultRepresentation>;

    }

    // TODO investigate using transformations to transform results 
    app.service("repLoader", function ($http, $q, $rootScope) {

        var repLoader = this; 
        repLoader.loadingCount = 0; 

        function getUrl(model: IHateoasModel): string {

            var url = model.url();

            if (model.method === "GET" || model.method === "DELETE") {
                var asJson = _.clone((<any>model).attributes);

                if (_.toArray(asJson).length > 0) {
                    var map = JSON.stringify(asJson);
                    var encodedMap = encodeURI(map);
                    url += `?${encodedMap}`;
                }
            }

            return url;
        }

        function getData(model: IHateoasModel): Object {

            var data = {};

            if (model.method === "POST" || model.method === "PUT") {
                data = _.clone((<any>model).attributes);
            }

            return data;
        }

        repLoader.populate = function <T>(model: IHateoasModel, ignoreCache?: boolean, expected?: IHateoasModel): ng.IPromise<T>
        {

            var response = expected || model;
            var useCache = !ignoreCache;

            var delay = $q.defer();

            var config = {
                url: getUrl(model),
                method: model.method,
                cache: useCache,
                data: getData(model)
            };

            $rootScope.$broadcast("ajax-change", ++repLoader.loadingCount); 
            $http(config).
                success(function (data, status, headers, config) {
                    (<any>response).attributes = data; // TODO make typed 
                    delay.resolve(response);
                    $rootScope.$broadcast("ajax-change", --repLoader.loadingCount); 
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
                    $rootScope.$broadcast("ajax-change", --repLoader.loadingCount); 
                });

            return delay.promise;
        };


        repLoader.invoke = function (action: ActionMember, parms: { id: string, val: Value }[]) : ng.IPromise < ActionResultRepresentation > {
            var invoke = action.getInvoke();                                          
            _.each(parms, (vp) => invoke.setParameter(vp.id, vp.val));
            return repLoader.populate (invoke, true);
        }

    });

}