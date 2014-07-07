/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />

module Spiro.Angular {

    export interface ILoaderWithTransform {
        loadAndTransform<T extends ResourceRepresentation>(url: string, repType: { new (any): T }) : ng.IPromise<any>;
    }

    app.service('loaderWithTransform', function (repLoader: Spiro.Angular.IRepLoader, $q: ng.IQService, transformStrategy : ITransformStrategy) {

        var transformer = <ILoaderWithTransform>this;

        transformer.loadAndTransform = function tt<T extends ResourceRepresentation>(url: string, repType: {new( any ) : T}) {

            var obj = new repType({});
            obj.hateoasUrl = url;
            return  repLoader.populate(obj).then((resource: T) => {
                return transformStrategy.transform(resource, transformer); 
            });
        };
    });
}