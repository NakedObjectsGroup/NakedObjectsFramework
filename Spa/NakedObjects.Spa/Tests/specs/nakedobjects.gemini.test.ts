/// <reference path="../../Scripts/nakedobjects.config.ts" />
/// <reference path="../../Scripts/nakedobjects.models.helpers.ts" />
/// <reference path="../../Scripts/nakedobjects.models.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.app.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.controllers.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.directives.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.routedata.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.context.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.handlers.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.navigation.browser.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.navigation.simple.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.urlmanager.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.focusmanager.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.viewmodelfactory.ts" />

/// <reference path="nakedobjects.gemini.test.helpers.ts" />

module NakedObjects.Gemini.Test {
    import SetupCondition = Test.Helpers.SetupCondition;
    import IHandlers = Angular.Gemini.IHandlers;
    import INakedObjectsScope = Angular.INakedObjectsScope;
    import PaneRouteData = Angular.Gemini.PaneRouteData;

    describe("nakedobjects.gemini.tests", () => {
        beforeEach(angular.mock.module("app"));

        describe("Go to Home Page", () => {
            let testRouteData: PaneRouteData;
            let testScope: INakedObjectsScope;

            beforeEach(inject(($rootScope) => {
                testScope = $rootScope.$new();
                Helpers.setup(SetupCondition.Home, testRouteData);
            }));

            describe("Call HandleHome", () => {

                beforeEach(inject((handlers: IHandlers) => {
                    // execute 
                    handlers.handleHome(testScope, testRouteData);
                }));

                it("Verify state in scope", () => {
                    Helpers.verify(testScope);
                });
            });

        });

    });
}