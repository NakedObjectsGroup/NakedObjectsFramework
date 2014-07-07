/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />

module Spiro.Angular {

    export interface INavigation {
        back()
        forward();
        push();
    }

    app.service('navigation', function($location: ng.ILocationService, $routeParams: ISpiroRouteParams) {

        var nav = <INavigation>this;

        nav.back = () => {
            
            if ($routeParams.resultObject || $routeParams.resultCollection) {
                // looking at an action result = so go back two 
                parent.history.back(2);
            } else {
                parent.history.back(1);
            }
        };

        nav.forward = () => {
            parent.history.forward(1);
        };

        nav.push = () => {

        };

    });
}