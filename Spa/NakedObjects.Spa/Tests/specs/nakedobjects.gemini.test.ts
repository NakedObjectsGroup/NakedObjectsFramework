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
    import IHandlers = Angular.Gemini.IHandlers;
    import INakedObjectsScope = Angular.INakedObjectsScope;
    import PaneRouteData = Angular.Gemini.PaneRouteData;
    import MenusViewModel = Angular.Gemini.MenusViewModel;
    import FocusTarget = Angular.Gemini.FocusTarget;
    import ActionViewModel = NakedObjects.Angular.Gemini.ActionViewModel;
    import DialogViewModel = NakedObjects.Angular.Gemini.DialogViewModel;
    describe("nakedobjects.gemini.tests", () => {

        beforeEach(angular.mock.module("app"));

        let $httpBackend: ng.IHttpBackendService;

        beforeEach(inject($injector => {
            // Set up the mock http service responses
            $httpBackend = $injector.get("$httpBackend");
            Helpers.setupBackend($httpBackend);
        }));

        describe("Go to Home Page", () => {
            let testRouteData: PaneRouteData;
            let testScope: INakedObjectsScope;
            let testEventSpy: jasmine.Spy;
            let timeout: ng.ITimeoutService;

            function setupBasePaneRouteData(paneId: number) {
                const trd = new PaneRouteData(paneId);

                trd.collections = {};
                trd.parms = {};
                trd.props = {};
                trd.fields = {};
                return trd;
            }

            function executeHandleHome(handlers: IHandlers) {
                handlers.handleHome(testScope, testRouteData);
                $httpBackend.flush();
                timeout.flush();
            }

            function verifyBaseHomePageState(ts: INakedObjectsScope) {
                expect(ts.homeTemplate).toBe(Angular.homeTemplate);
                const menusViewModel = ts.menus as MenusViewModel;
                expect(menusViewModel.items.length).toBe(10);
                expect(testEventSpy).toHaveBeenCalled();
            }

            function verifyOpenMenuHomePageState(ts: INakedObjectsScope) {
                expect(ts.actionsTemplate).toBe(Angular.actionsTemplate);
                expect((<{actions : ActionViewModel[]}>ts.object).actions.length).toBe(4);
            }

            function verifyOpenDialogHomePageState(ts: INakedObjectsScope) {
                expect(ts.dialogTemplate).toBe(Angular.dialogTemplate);
                const dialogViewModel = ts.dialog as DialogViewModel;
                expect(dialogViewModel.parameters.length).toBe(1);
                expect(dialogViewModel.title).toBe("Find Vendor By Account Number");
            }


            function getEventTestFunc (t: FocusTarget, i: number, p: number, c: number) {              
                return (e, target: FocusTarget, index: number, paneId: number, count: number) => {
                    expect(e.name).toBe("geminiFocuson");
                    expect(target).toBe(t);
                    expect(index).toBe(i);
                    expect(paneId).toBe(p);
                    expect(count).toBe(c);
                }
            }

            function setupEventSpy(ts : INakedObjectsScope,  target: FocusTarget, index: number, paneId: number, count: number) {
                const tes = jasmine.createSpy("event", getEventTestFunc(target, index, paneId, count));
                tes.and.callThrough();
                testScope.$on("geminiFocuson", tes);
                return tes;
            }

            beforeEach(inject(($rootScope: ng.IRootScopeService, $timeout: ng.ITimeoutService) => {
                testScope = $rootScope.$new() as INakedObjectsScope;
                testRouteData = setupBasePaneRouteData(1);
                timeout = $timeout;
            }));

          
            describe("Without open menu or dialog", () => {

                beforeEach(inject((handlers: IHandlers) => {
                    testEventSpy = setupEventSpy(testScope, FocusTarget.Menu, 0, 1, 1);
                    executeHandleHome(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseHomePageState(testScope);
                });
            });

            describe("With open menu", () => {

                beforeEach(inject((handlers: IHandlers) => {
                    testRouteData.menuId = "VendorRepository";
                    testEventSpy = setupEventSpy(testScope, FocusTarget.SubAction, 0, 1, 1);
                    executeHandleHome(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseHomePageState(testScope);
                    verifyOpenMenuHomePageState(testScope);
                });
            });

            describe("With open menu and dialog", () => {

                beforeEach(inject((handlers: IHandlers) => {
                    testRouteData.menuId = "VendorRepository";
                    testRouteData.dialogId = "FindVendorByAccountNumber";
                    testEventSpy = setupEventSpy(testScope, FocusTarget.Dialog, 0, 1, 1);
                    executeHandleHome(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseHomePageState(testScope);
                    verifyOpenMenuHomePageState(testScope);
                    verifyOpenDialogHomePageState(testScope);
                });
            });


        });

    });
}