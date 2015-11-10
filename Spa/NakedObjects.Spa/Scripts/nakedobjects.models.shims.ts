//  Copyright 2013-2014 Naked Objects Group Ltd
//  Licensed under the Apache License, Version 2.0(the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

// ABOUT THIS FILE:
// nakedobjects.models defines a set of classes that correspond directly to the JSON representations returned by Restful Objects
// resources.  These classes provide convenient methods for navigating the contents of those representations, and for
// following links to other resources.

// null  version of shims 

module NakedObjects {

    export class ModelShim {
        attributes: any;

        constructor(object? : any) {
            this.attributes = object;
        }
        url(): string {
            return "";
        }
        get(attributeName: string): any {
            return this.attributes[attributeName];
        }
        set(attributeName?: any, value?: any, options?: any) {
            this.attributes[attributeName] = value;
        }
    }

    // base class for all representations that can be directly loaded from the server 
    export class HateoasModelBaseShim extends ModelShim {
        constructor(object? : any) {
            super(object);
        }
        hateoasUrl: string = "";
        method: string = "GET";
        suffix: string = "";
        url(): string {
            return (this.hateoasUrl || super.url()) + this.suffix;
        }
        onError(map: Object, statusCode: string, warnings: string) { }
        preFetch() { }

        urlParms : _.Dictionary<string>;
    }

    export class ArgumentMap extends HateoasModelBaseShim {
        constructor(map: Object, parent: any, public id: string) {
            super(map);
        }
        onChange() { }
        onError(map: Object, statusCode: string, warnings: string) { }
    }

    export class CollectionShim {
        constructor(object? : any) { }

        url(): any { }
        models: any[];
        model: new (any : any) => any;

        add(models: any, options?: any) {
            this.models = this.models || [];

            for (var i = 0; i < models.length; i++) {
                var m = new this.model(models[i]);
                this.models.push(m);
            }
        }
    }
}
