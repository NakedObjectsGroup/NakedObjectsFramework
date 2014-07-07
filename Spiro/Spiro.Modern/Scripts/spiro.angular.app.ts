/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />

module Spiro.Angular {

    /* Declare app level module */
   
    export var app = angular.module('app', ['ngRoute', 'ngTouch']);

    export interface ISpiroRouteParams extends ng.route.IRouteParamsService {
        action: string;
        property: string;
        collectionItem: string;
        resultObject: string; 
        resultCollection: string; 
        collection: string; 
        editMode: string; 
        tableMode: string; 
        dt: string; 
        id: string; 
        sid: string; 
    }
}