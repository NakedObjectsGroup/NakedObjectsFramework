/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export interface INavigation {
        back();
        forward();
        push();
    }

    app.service("navigation", function () {
        const nav = <INavigation>this;
        nav.back = () => parent.history.back(1);
        nav.forward = () => parent.history.forward(1);
        nav.push = () => { };
    });
}