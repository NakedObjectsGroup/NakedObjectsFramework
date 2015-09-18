//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular {

    export interface ILoaderWithTransform {
        loadAndTransform<T extends ResourceRepresentation>(url: string, repType: { new (any): T }) : ng.IPromise<any>;
    }

    app.service('loaderWithTransform', function (repLoader: NakedObjects.Angular.IRepLoader, $q: ng.IQService, transformStrategy : ITransformStrategy) {

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