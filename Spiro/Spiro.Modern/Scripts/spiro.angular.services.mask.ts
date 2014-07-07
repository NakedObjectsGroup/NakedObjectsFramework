/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />


module Spiro.Angular {

    export interface ILocalFilter {
        name: string;
        mask: string;
    }

    export interface IMaskMap {
        [index: string]: ILocalFilter;
    }


    export interface IMask {
        toLocalFilter(remoteMask: string): ILocalFilter;
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
    });
}