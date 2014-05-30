/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />


module Spiro.Angular {

    export interface IUrlHelper {
        action(dvm?: DialogViewModel): string;
        actionParms(): string[];
        getOtherParms(excepts?: string[]): string;
        toAppUrl(href: string, toClose?: string[]): string;
        toActionUrl(href: string): string;
        toPropertyUrl(href: string): string;
        toCollectionUrl(href: string): string;
        toItemUrl(href: string, itemHref: string): string;

        toObjectPath(obj: DomainObjectRepresentation): string;
        toTransientObjectPath(obj: DomainObjectRepresentation): string;
        toErrorPath(): string;
        updateParms(resultObj: DomainObjectRepresentation, dvm?: DialogViewModel) : string;
        updateParms(resultList: ListRepresentation, dvm?: DialogViewModel) : string;
    }

    app.service('urlHelper', function ($routeParams : ISpiroRouteParams) {

        var helper = <IUrlHelper>this;

        helper.action = function (dvm?: DialogViewModel) {

            var pps = dvm && dvm.parameters.length > 0 ? _.reduce(dvm.parameters, (memo : string, parm : ParameterViewModel) => { return memo + "/" + parm.getMemento(); }, "") : "";

            return _.first($routeParams.action.split("/")) + encodeURIComponent(pps);
        }

        helper.actionParms = function () {
            return _.rest($routeParams.action.split("/"));
        }

        helper.getOtherParms = function (excepts?: string[]) {

            function include(parm) {
                return $routeParams[parm] && !_.any(excepts, (except) => parm === except);
            }

            function getParm(name: string) {
                return include(name) ? "&" + name + "=" + $routeParams[name] : "";
            }

            var actionParm = include("action") ? "&action=" + helper.action() : "";
            var collectionParm = include("collection") ? "&collection=" + $routeParams.collection : "";
            var collectionItemParm = include("collectionItem") ? "&collectionItem=" + $routeParams.collectionItem : "";
            var propertyParm = include("property") ? "&property=" + $routeParams.property : "";
            var resultObjectParm = include("resultObject") ? "&resultObject=" + $routeParams.resultObject : "";
            var resultCollectionParm = include("resultCollection") ? "&resultCollection=" + $routeParams.resultCollection : "";

            return actionParm + collectionParm + collectionItemParm + propertyParm + resultObjectParm + resultCollectionParm;
        }

        helper.toAppUrl = function (href: string, toClose?: string[]): string {
            var urlRegex = /(objects|services)\/(.*)/;
            var results = (urlRegex).exec(href);
            var parms = "";

            if (toClose) {
                parms = helper.getOtherParms(toClose);
                parms = parms ? "?" + parms.substr(1) : "";
            }

            return (results && results.length > 2) ? "#/" + results[1] + "/" + results[2] + parms : "";
        }


        helper.toActionUrl = function (href: string): string {
            var urlRegex = /(services|objects)\/([\w|\.]+(\/[\w|\.|-]+)?)\/actions\/([\w|\.]+)/;
            var results = (urlRegex).exec(href);
            return (results && results.length > 3) ? "#/" + results[1] + "/" + results[2] + "?action=" + results[4] + helper.getOtherParms(["action"]) : "";
        }

        helper.toPropertyUrl = function (href: string): string {
            var urlRegex = /(objects)\/([\w|\.]+)\/([\w|\.|-]+)\/(properties)\/([\w|\.]+)/;
            var results = (urlRegex).exec(href);
            return (results && results.length > 5) ? "#/" + results[1] + "/" + results[2] + "/" + results[3] + "?property=" + results[5] + helper.getOtherParms(["property", "collectionItem", "resultObject"]) : "";
        }

        helper.toCollectionUrl = function (href: string): string {
            var urlRegex = /(objects)\/([\w|\.]+)\/([\w|\.|-]+)\/(collections)\/([\w|\.]+)/;
            var results = (urlRegex).exec(href);
            return (results && results.length > 5) ? "#/" + results[1] + "/" + results[2] + "/" + results[3] + "?collection=" + results[5] + helper.getOtherParms(["collection", "resultCollection"]) : "";
        }

        helper.toItemUrl = function (href: string, itemHref: string): string {
            var parentUrlRegex = /(services|objects)\/([\w|\.]+(\/[\w|\.|-]+)?)/;
            var itemUrlRegex = /(objects)\/([\w|\.]+)\/([\w|\.|-]+)/;
            var parentResults = (parentUrlRegex).exec(href);
            var itemResults = (itemUrlRegex).exec(itemHref);
            return (parentResults && parentResults.length > 2) ? "#/" + parentResults[1] + "/" + parentResults[2] + "?collectionItem=" + itemResults[2] + "/" + itemResults[3] + helper.getOtherParms(["property", "collectionItem", "resultObject"]) : "";
        }

        helper.toTransientObjectPath = function (obj: DomainObjectRepresentation) {
            return "objects/" + obj.domainType(); 
        }

        helper.toObjectPath = function (obj : DomainObjectRepresentation) {
            return "objects/" + obj.domainType() + "/" + obj.instanceId();  
        }

        helper.toErrorPath = function () {
            return "error";
        }

        helper.updateParms = function (result: Object, dvm?: DialogViewModel) : string {

            var resultParm = "";
            var actionParm = "";

            function getActionParm() {
                if (dvm) {
                    return dvm.show ? "&action=" + helper.action(dvm) : "";
                }
                return ""; 
            }

            if (result instanceof DomainObjectRepresentation) {
                var obj = <DomainObjectRepresentation>result;
                resultParm = "resultObject=" + obj.domainType() + "-" + obj.instanceId();  // todo add some parm handling code 
                actionParm = getActionParm();
            }

            if (result instanceof ListRepresentation) {
                resultParm = "resultCollection=" + helper.action(dvm);
                actionParm = getActionParm();
            }

            return resultParm + actionParm;
        }
    });

}