/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.config.ts" />

module Spiro.Angular {


    export interface IMask {
        toLocalFilter(remoteMask: string): ILocalFilter;
    }

    app.service('mask', function () {

        var color = <IMask>this;

        color.toLocalFilter = function (remoteMask: string) {
            if (maskMap) {
                return maskMap[remoteMask]; 
            }

            return null;
        }

    });
}