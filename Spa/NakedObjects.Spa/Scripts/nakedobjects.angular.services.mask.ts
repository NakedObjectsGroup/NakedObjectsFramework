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
        const mask = <IMask>this;
        let maskMap: IMaskMap = {};

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