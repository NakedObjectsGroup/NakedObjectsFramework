/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />


namespace NakedObjects {

    export interface INavigation {
        back() : void;
        forward(): void;
        push(): void;
    }

    app.service("navigation", function($location: ng.ILocationService) {
        const nav = <INavigation>this;
        const history : any[]  = [];
        let index = -1;
        let navigating = false;

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
                const newUrl = $location.url();
                const curUrl = history[history.length - 1];
                const isActionUrl = newUrl.indexOf("?action") > 0;
                if (!isActionUrl && newUrl !== curUrl) {
                    history.push($location.url());
                }

                index = history.length - 1;
            }
            navigating = false;
        };

    });
}