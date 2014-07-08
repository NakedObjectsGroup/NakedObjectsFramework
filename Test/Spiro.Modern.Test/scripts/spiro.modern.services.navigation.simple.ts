/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />


module Spiro.Angular.Modern {

    export interface INavigation {
        back()
        forward();
        push();
    }

    app.service('navigation', function ($location: ng.ILocationService) {

        var nav = <INavigation>this;
        var history = [];
        var index = -1; 
        var navigating = false; 

        nav.back = () => {
            if ((index - 1) >= 0) {
                index--;
                navigating = true; 
                $location.url(history[index]);
            }

        };
        nav.forward = () => {
            if ((index + 1) <= (history.length - 1)) {
                index++;
                navigating = true;
                $location.url(history[index]);
            }
        };
        nav.push = () => {
            if (!navigating) {
                var newUrl = $location.url();
                var curUrl = history[history.length - 1];
                var isActionUrl = newUrl.indexOf("?action") > 0; 

                if (!isActionUrl && newUrl != curUrl) {
                    history.push($location.url());
                }
                      
                index = history.length - 1;
            }
            navigating = false;
        };

    });
}