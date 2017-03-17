/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />

namespace NakedObjects {
    import getOtherPane = Models.getOtherPane;

    export interface IClickHandler {
        pane(currentPane: number, right?: boolean): number;
    }

    app.service("clickHandler", function() {
        const clickHandler = <IClickHandler>this;

        function leftRightClickHandler(currentPane: number, right = false): number {
            return right ? 2 : 1;
        }

        function sameOtherClickHandler(currentPane: number, right = false): number {
            return right ? getOtherPane(currentPane) : currentPane;
        }

        clickHandler.pane = sameOtherClickHandler;

    });
}