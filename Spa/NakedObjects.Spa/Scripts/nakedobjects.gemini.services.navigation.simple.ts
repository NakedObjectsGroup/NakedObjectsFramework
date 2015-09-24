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


module NakedObjects.Angular.Gemini {

    export interface INavigation {
        back();
        forward();
        push();
    }

    app.service("navigation", function ($location: ng.ILocationService) {
        const nav = <INavigation>this;
        const history = [];
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