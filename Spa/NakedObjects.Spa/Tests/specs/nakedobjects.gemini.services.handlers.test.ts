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

/// <reference path="../../Scripts/typings/karma-jasmine/karma-jasmine.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.handlers.ts" />

describe("nakedobjects.gemini.services.handlers", () => {

    var $scope;

    beforeEach(() => {
        angular.mock.module("app");        
    });
    
    describe("handleError", () => {

        beforeEach(inject(($rootScope, handlers: NakedObjects.Angular.Gemini.IHandlers, context) => {
            $scope = $rootScope.$new();

            spyOn(context, "getError").and.returnValue(new NakedObjects.ErrorRepresentation({ message: "", stacktrace: [] }));

            handlers.handleError($scope);
        }));


        it("should set a error data", () => {
            expect($scope.error).toBeDefined();
            expect($scope.errorTemplate).toEqual("Content/partials/error.html");
        });

    });

    describe("handleBackground", () => {

        let navService: NakedObjects.Angular.Gemini.INavigation;     
        let setError: jasmine.Spy;

        beforeEach(inject(($rootScope, handlers, $location,  color, navigation, context) => {
            $scope = $rootScope.$new();
            navService = navigation;

            spyOn(color, "toColorFromHref").and.returnValue("acolor");
            spyOn(navigation, "push");
           
            setError = spyOn(context, "setError");

        }));

        describe("if validation ok", () => {
            const testVersion = new NakedObjects.VersionRepresentation();
            beforeEach(inject(($q, $timeout, $rootScope, $location, $routeParams, handlers: NakedObjects.Angular.Gemini.IHandlers, context) => {
                testVersion.attributes = { specVersion: "1.1", optionalCapabilities: { domainModel: "selectable" } };
                spyOn(context, "getVersion").and.returnValue($q.when(testVersion));

                handlers.handleBackground($scope);
                $timeout.flush();
            }));

            it("should set scope variables", () => {
                expect($scope.backgroundColor).toEqual("acolor");
                expect(navService.push).toHaveBeenCalled();
                expect(setError).not.toHaveBeenCalled();
            });
        });

        describe("if validation fails version", () => {
            const testVersion = new NakedObjects.VersionRepresentation();
            beforeEach(inject(($q, $timeout, $rootScope, $location, $routeParams, handlers: NakedObjects.Angular.Gemini.IHandlers, context) => {
                testVersion.attributes = { specVersion: "1.0", optionalCapabilities: { domainModel: "selectable" } };
                spyOn(context, "getVersion").and.returnValue($q.when(testVersion));

                handlers.handleBackground($scope);
                $timeout.flush();
            }));

            it("sets error", () => {
                expect(setError).toHaveBeenCalled();
            });
        });

        describe("if validation fails domain model", () => {
            const testVersion = new NakedObjects.VersionRepresentation();
            beforeEach(inject(($q, $timeout, $rootScope, $location, $routeParams, handlers: NakedObjects.Angular.Gemini.IHandlers, context: NakedObjects.Angular.Gemini.IContext) => {
                testVersion.attributes = { specVersion: "1.1", optionalCapabilities: { domainModel: "formal" } };
                spyOn(context, "getVersion").and.returnValue($q.when(testVersion));

                handlers.handleBackground($scope);
                $timeout.flush();
            }));

            it("sets error", () => {
                expect(setError).toHaveBeenCalled();
            });
        });
    });

    describe("handleToolBar", () => {


        function expectToolBarData() {
            expect($scope.toolBar).toBeDefined();
            expect($scope.toolBar.goHome).toBeDefined();
            expect($scope.toolBar.template).toEqual("Content/partials/appbar.html");
            expect($scope.toolBar.footerTemplate).toEqual("Content/partials/footer.html");
            expect($scope.toolBar.goBack).toBeDefined();
            expect($scope.toolBar.goForward).toBeDefined();
        }

        describe("handleToolBar when not viewing an  object", () => {

            beforeEach(inject(($rootScope, handlers: NakedObjects.Angular.Gemini.IHandlers) => {
                $scope = $rootScope.$new();

                handlers.handleToolBar($scope);
            }));

            it("should set toolBar data", () => {
                expectToolBarData();
            });

        });

        describe("handleToolBar when viewing an editable object", () => {

            const testObject = new NakedObjects.DomainObjectRepresentation();
            const testMember = new NakedObjects.PropertyMember({}, testObject, "");
            const testVm = new NakedObjects.Angular.Gemini.DomainObjectViewModel();

            beforeEach(inject(($q, $timeout, $rootScope, $location, $routeParams, handlers: NakedObjects.Angular.Gemini.IHandlers, context: NakedObjects.Angular.Gemini.IContext) => {
                $scope = $rootScope.$new();
                spyOn(testVm, "showEdit").and.returnValue(true);

                $scope.$parent.object = testVm;

                $routeParams.dt = "test";
                $routeParams.id = "1";

                spyOn(context, "getObject").and.returnValue($q.when(testObject));
                spyOn(testObject, "propertyMembers").and.returnValue([testMember]);

                spyOn($location, "path").and.returnValue("aPath");
                
                handlers.handleToolBar($scope);
                $timeout.flush();
            }));

            it("should set toolBar data", () => {
                expectToolBarData();
            });


        });

        describe("handleToolBar when viewing a non editable object", () => {

            const testObject = new NakedObjects.DomainObjectRepresentation();
            const testMember = new NakedObjects.PropertyMember({}, testObject, "");

            beforeEach(inject(($q, $timeout, $rootScope, $location, $routeParams, handlers: NakedObjects.Angular.Gemini.IHandlers, context: NakedObjects.Angular.Gemini.IContext) => {
                $scope = $rootScope.$new();

                $routeParams.dt = "test";
                $routeParams.id = "1";

                spyOn(context, "getObject").and.returnValue($q.when(testObject));
                spyOn(testObject, "propertyMembers").and.returnValue([testMember]);
                spyOn(testMember, "disabledReason").and.returnValue("disabled");

                spyOn($location, "path").and.returnValue("aPath");

                handlers.handleToolBar($scope);
                $timeout.flush();
            }));

            it("should set toolBar data", () => {
                expectToolBarData();
            });
        });
    });

});