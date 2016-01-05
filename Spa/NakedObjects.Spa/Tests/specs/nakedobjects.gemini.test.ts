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
    import ActionViewModel = Angular.Gemini.ActionViewModel;
    import DialogViewModel = Angular.Gemini.DialogViewModel;
    import DomainObjectViewModel = NakedObjects.Angular.Gemini.DomainObjectViewModel;
    describe("nakedobjects.gemini.tests", () => {

        beforeEach(angular.mock.module("app"));

        let $httpBackend: ng.IHttpBackendService;
        let testRouteData: PaneRouteData;
        let testScope: INakedObjectsScope;
        let timeout: ng.ITimeoutService;
        let testEventSpy: jasmine.Spy;
        let cacheFactory: ng.ICacheFactoryService;

        function setupBasePaneRouteData(paneId: number) {
            const trd = new PaneRouteData(paneId);

            trd.collections = {};
            trd.parms = {};
            trd.props = {};
            trd.fields = {};
            return trd;
        }

        function getEventTestFunc(t: FocusTarget, i: number, p: number, c: number) {
            return (e, target: FocusTarget, index: number, paneId: number, count: number) => {
                expect(e.name).toBe("geminiFocuson");
                expect(target).toBe(t);
                expect(index).toBe(i);
                expect(paneId).toBe(p);
                expect(count).toBe(c);
            }
        }

        function setupEventSpy(ts: INakedObjectsScope, target: FocusTarget, index: number, paneId: number, count: number) {
            const tes = jasmine.createSpy("event", getEventTestFunc(target, index, paneId, count));
            tes.and.callThrough();
            testScope.$on("geminiFocuson", tes);
            return tes;
        }

        beforeEach(inject($injector => {
            // Set up the mock http service responses
            $httpBackend = $injector.get("$httpBackend");
            Helpers.setupBackend($httpBackend);
        }));

        beforeEach(inject(($rootScope: ng.IRootScopeService, $timeout: ng.ITimeoutService, $cacheFactory : ng.ICacheFactoryService) => {
            testScope = $rootScope.$new() as INakedObjectsScope;
            testRouteData = setupBasePaneRouteData(1);
            timeout = $timeout;
            cacheFactory = $cacheFactory;
        }));

        function verifyOpenDialogState(ts: INakedObjectsScope, parmCount : number, title : string) {
            expect(ts.dialogTemplate).toBe(Angular.dialogTemplate);
            const dialogViewModel = ts.dialog as DialogViewModel;
            expect(dialogViewModel.parameters.length).toBe(parmCount);
            expect(dialogViewModel.title).toBe(title);
        }


        describe("Go to Home Page", () => {
                                
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
                expect((<{ actions: ActionViewModel[] }>ts.object).actions.length).toBe(4);
            }

            function verifyOpenDialogHomePageState(ts: INakedObjectsScope) {          
                verifyOpenDialogState(ts, 1, "Find Vendor By Account Number");
            }

            describe("Without open menu or dialog", () => {

                beforeEach(inject(() => {
                    testEventSpy = setupEventSpy(testScope, FocusTarget.Menu, 0, 1, 1);
                }));

                beforeEach(inject((handlers: IHandlers) => {
                    executeHandleHome(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseHomePageState(testScope);
                });
            });

            describe("With open menu", () => {

                beforeEach(inject(() => {
                    testRouteData.menuId = "VendorRepository";
                    testEventSpy = setupEventSpy(testScope, FocusTarget.SubAction, 0, 1, 1);
                }));

                beforeEach(inject((handlers: IHandlers) => {
                    executeHandleHome(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseHomePageState(testScope);
                    verifyOpenMenuHomePageState(testScope);
                });
            });

            describe("With open menu and dialog", () => {

                beforeEach(inject(() => {
                    testRouteData.menuId = "VendorRepository";
                    testRouteData.dialogId = "FindVendorByAccountNumber";
                    testEventSpy = setupEventSpy(testScope, FocusTarget.Dialog, 0, 1, 1);
                }));

                beforeEach(inject((handlers: IHandlers) => {
                    executeHandleHome(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseHomePageState(testScope);
                    verifyOpenMenuHomePageState(testScope);
                    verifyOpenDialogHomePageState(testScope);
                });
            });
        });


        describe("Go to Object Page", () => {

            function executeHandleObject(handlers: IHandlers) {
                handlers.handleObject(testScope, testRouteData);
                $httpBackend.flush();
                timeout.flush();
            }

            function verifyBaseObjectViewPageState(ts: INakedObjectsScope) {
                expect(ts.objectTemplate).toBe(Angular.objectViewTemplate);
                expect(ts.collectionsTemplate).toBe(Angular.collectionsTemplate);
                const objectViewModel = ts.object as DomainObjectViewModel;
                expect(objectViewModel.properties.length).toBe(7);
                expect(testEventSpy).toHaveBeenCalled();
                expect(cacheFactory.get("recentlyViewed").get("AdventureWorksModel.Vendor")[objectViewModel.domainObject.selfLink().href()].name).toBe(objectViewModel.domainObject.title());
            }

            function verifyActionsOpenPageState(ts: INakedObjectsScope) {
                expect(ts.actionsTemplate).toBe(Angular.actionsTemplate);              
            }

            function verifyActionsClosedPageState(ts: INakedObjectsScope) {
                expect(ts.actionsTemplate).toBe(Angular.nullTemplate);
            }

            function verifyOpenDialogObjectPageState(ts: INakedObjectsScope) {
                verifyOpenDialogState(ts, 2, "List Purchase Orders");
            }

            beforeEach(inject(() => {
                testRouteData.objectId = "AdventureWorksModel.Vendor-1634";
            }));

            describe("View with closed actions no dialog", () => {

                beforeEach(inject(() => {
                    testEventSpy = setupEventSpy(testScope, FocusTarget.ObjectTitle, 0, 1, 1);
                }));

                beforeEach(inject((handlers: IHandlers) => {
                    executeHandleObject(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseObjectViewPageState(testScope);
                    verifyActionsClosedPageState(testScope);
                });
            });

            describe("View with open actions no dialog", () => {

                beforeEach(inject(() => {
                    testRouteData.actionsOpen = "true";
                    testEventSpy = setupEventSpy(testScope, FocusTarget.SubAction, 0, 1, 1);
                }));

                beforeEach(inject((handlers: IHandlers) => {
                    executeHandleObject(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseObjectViewPageState(testScope);
                    verifyActionsOpenPageState(testScope);
                });
            });

            describe("View with open actions and dialog", () => {

                beforeEach(inject(() => {
                    testRouteData.actionsOpen = "true";
                    testRouteData.dialogId = "ListPurchaseOrders";
                    testEventSpy = setupEventSpy(testScope, FocusTarget.Dialog, 0, 1, 1);
                }));

                beforeEach(inject((handlers: IHandlers) => {
                    executeHandleObject(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseObjectViewPageState(testScope);
                    verifyActionsOpenPageState(testScope);
                    verifyOpenDialogObjectPageState(testScope);
                });
            });

            describe("View with closed actions and dialog", () => {

                beforeEach(inject(() => {
                    testRouteData.dialogId = "ListPurchaseOrders";
                    testEventSpy = setupEventSpy(testScope, FocusTarget.Dialog, 0, 1, 1);
                }));

                beforeEach(inject((handlers: IHandlers) => {
                    executeHandleObject(handlers);
                }));

                it("Verify state in scope", () => {
                    verifyBaseObjectViewPageState(testScope);
                    verifyActionsClosedPageState(testScope);
                    verifyOpenDialogObjectPageState(testScope);
                });
            });


        });
    });
}