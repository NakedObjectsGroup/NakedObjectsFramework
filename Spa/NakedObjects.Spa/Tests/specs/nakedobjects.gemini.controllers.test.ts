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

describe("nakedobjects.gemini.controllers", () => {
	let $scope, ctrl;

    beforeEach(angular.mock.module("app"));
    
    describe("HomeController", () => {
        let handleHome: jasmine.Spy;
        let getRouteData: jasmine.Spy;
        let testRouteData: NakedObjects.Angular.Gemini.RouteData;

        beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
            $scope = $rootScope.$new();
            handleHome = spyOn(handlers, "handleHome");
            testRouteData = new NakedObjects.Angular.Gemini.RouteData();
            getRouteData = spyOn(urlManager, "getRouteData");
            getRouteData.and.returnValue(testRouteData);
        }));

        describe("on Pane 1", () => {

            beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
                ctrl = $controller("Pane1HomeController", { $scope: $scope, handlers: handlers, urlManager: urlManager });
            }));

            it("should call getRouteData", () => {
                expect(getRouteData).toHaveBeenCalled();
            });

            it("should call the handler for the correct pane", () => {
                expect(handleHome).toHaveBeenCalledWith($scope, testRouteData.pane1);
            });
        });

        describe("on Pane 2", () => {

            beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
                ctrl = $controller("Pane2HomeController", { $scope: $scope, handlers: handlers, urlManager: urlManager });
            }));

            it("should call getRouteData", () => {
                expect(getRouteData).toHaveBeenCalled();
            });

            it("should call the handler for the correct pane", () => {
                expect(handleHome).toHaveBeenCalledWith($scope, testRouteData.pane2);
            });
        });
    });

    describe("ObjectController", () => {
        let handleObject: jasmine.Spy;
        let getRouteData: jasmine.Spy;
        let testRouteData: NakedObjects.Angular.Gemini.RouteData;

        beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
            $scope = $rootScope.$new();
            handleObject = spyOn(handlers, "handleObject");
            testRouteData = new NakedObjects.Angular.Gemini.RouteData();
            getRouteData = spyOn(urlManager, "getRouteData");
            getRouteData.and.returnValue(testRouteData);
        }));

        describe("on Pane 1", () => {

            beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
                ctrl = $controller("Pane1ObjectController", { $scope: $scope, handlers: handlers, urlManager: urlManager });
            }));

            it("should call getRouteData", () => {
                expect(getRouteData).toHaveBeenCalled();
            });

            it("should call the handler for the correct pane", () => {
                expect(handleObject).toHaveBeenCalledWith($scope, testRouteData.pane1);
            });
        });

        describe("on Pane 2", () => {

            beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
                ctrl = $controller("Pane2ObjectController", { $scope: $scope, handlers: handlers, urlManager: urlManager });
            }));

            it("should call getRouteData", () => {
                expect(getRouteData).toHaveBeenCalled();
            });

            it("should call the handler for the correct pane", () => {
                expect(handleObject).toHaveBeenCalledWith($scope, testRouteData.pane2);
            });
        });
    });

    describe("Pane1ListController", () => {
        let handleList: jasmine.Spy;
        let getRouteData: jasmine.Spy;
        let testRouteData: NakedObjects.Angular.Gemini.RouteData;

        beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
            $scope = $rootScope.$new();
            handleList = spyOn(handlers, "handleList");
            testRouteData = new NakedObjects.Angular.Gemini.RouteData();
            getRouteData = spyOn(urlManager, "getRouteData");
            getRouteData.and.returnValue(testRouteData);
        }));

        describe("on Pane 1", () => {

            beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
                ctrl = $controller("Pane1ListController", { $scope: $scope, handlers: handlers, urlManager: urlManager });
            }));

            it("should call getRouteData", () => {
                expect(getRouteData).toHaveBeenCalled();
            });

            it("should call the handler for the correct pane", () => {
                expect(handleList).toHaveBeenCalledWith($scope, testRouteData.pane1);
            });
        });

        describe("on Pane 2", () => {

            beforeEach(inject(($rootScope, $controller, handlers, urlManager) => {
                ctrl = $controller("Pane2ListController", { $scope: $scope, handlers: handlers, urlManager: urlManager });
            }));

            it("should call getRouteData", () => {
                expect(getRouteData).toHaveBeenCalled();
            });

            it("should call the handler for the correct pane", () => {
                expect(handleList).toHaveBeenCalledWith($scope, testRouteData.pane2);
            });
        });
    });



	describe("BackgroundController", () => {
		var handleBackground;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleBackground = spyOn(handlers, "handleBackground");
			ctrl = $controller("BackgroundController", { $scope: $scope, handlers: handlers });
		}));

		it("should call the handler", () => {
			expect(handleBackground).toHaveBeenCalledWith($scope);
		});  
	});

    describe("ErrorController", () => {

        var handleError;

        beforeEach(inject(($rootScope, $controller, handlers) => {
            $scope = $rootScope.$new();
            handleError = spyOn(handlers, "handleError");
            ctrl = $controller("ErrorController", { $scope: $scope, handlers: handlers });
        }));

        it("should call the handler", () => {
            expect(handleError).toHaveBeenCalledWith($scope);
        });

    });

	describe("ToolBarController", () => {

		var handleAppBar;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleAppBar = spyOn(handlers, "handleToolBar");
			ctrl = $controller("ToolBarController", { $scope: $scope, handlers: handlers });
		}));

		it("should call the handler", () => {
			expect(handleAppBar).toHaveBeenCalledWith($scope);
		});

	});

});