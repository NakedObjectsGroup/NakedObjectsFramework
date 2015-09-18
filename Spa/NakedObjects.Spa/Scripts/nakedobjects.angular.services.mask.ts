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

/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular {

    export interface ILocalFilter {
        name: string;
        mask: string;
    }

    export interface IMaskMap {
        [index: string]: ILocalFilter;
    }


    export interface IMask {
        toLocalFilter(remoteMask: string): ILocalFilter;
        defaultLocalFilter(format : string) : ILocalFilter;

        setMaskMap(map : IMaskMap);
    }

    app.service('mask', function () {

        var mask = <IMask>this;
        var maskMap: IMaskMap = {};

        mask.toLocalFilter = (remoteMask: string) => {
            return maskMap ? maskMap[remoteMask] : null;
        }

        mask.setMaskMap = (map: IMaskMap) => {
            maskMap = map; 
        }

        mask.defaultLocalFilter = (format: string) => {
            switch (format) {
                case ("string"):
                    return null;
                case ("date-time"):
                    return { name: "date", mask: "d MMM yyyy hh:mm:ss" };
                case ("date"):
                    return { name: "date", mask: "d MMM yyyy" };
                case ("time"):
                    return { name: "date", mask: "hh:mm:ss"};
                case ("utc-millisec"):
                    return null;
                case ("big-integer"):
                    return { name: "number", mask: "" };
                case ("big-decimal"):
                    return { name: "number", mask: "" };
                case ("blob"):
                    return null;
                case ("clob"):
                    return null;
                case ("decimal"):
                    return null;
                    //return { name: "currency", mask: "$" };
                case ("int"):
                    return { name: "number", mask: "" };
                default:
                    return null;
            }
        }


    });
}