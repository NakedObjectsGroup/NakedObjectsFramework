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

    export interface IClickHandler {
        pane(currentPane: number, right?: boolean): number;
    }

    app.service("clickHandler", function () {
        const clickHandler = <IClickHandler>this;

        function leftRightClickHandler(currentPane: number, right = false): number {
            return right ? 2 : 1;
        }

        function sameOtherClickHandler(currentPane: number, right = false): number {
            const otherPane = currentPane === 1 ? 2 : 1;
            return right ? otherPane : currentPane;
        }

        clickHandler.pane = sameOtherClickHandler;

    });
}