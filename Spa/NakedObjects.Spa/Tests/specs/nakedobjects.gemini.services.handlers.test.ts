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

            const rawError = { message: "", stacktrace: [] , links : [], extensions : {} };
            const er = new NakedObjects.ErrorRepresentation();
            er.populate(rawError);

            spyOn(context, "getError").and.returnValue(er);

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
                const opts: NakedObjects.RoInterfaces.IOptionalCapabilities = { domainModel: "selectable", blobsClobs: "", deleteObjects: "", protoPersistentObjects : "" , validateOnly : ""};
                const version: NakedObjects.RoInterfaces.IVersionRepresentation = { links: [], extensions: {}, specVersion: "1.1", optionalCapabilities: opts, implVersion: "" };
                testVersion.populate(version);
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
                const opts: NakedObjects.RoInterfaces.IOptionalCapabilities = { domainModel: "selectable", blobsClobs: "", deleteObjects: "", protoPersistentObjects: "", validateOnly: "" };

                const version = { specVersion: "1.0", optionalCapabilities: opts , links : [], extensions : {} , implVersion : ""};

                testVersion.populate(version);
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
                const opts: NakedObjects.RoInterfaces.IOptionalCapabilities = { domainModel: "formal", blobsClobs: "", deleteObjects: "", protoPersistentObjects: "", validateOnly: "" };

                const version = { specVersion: "1.1", optionalCapabilities: opts, links: [], extensions: {}, implVersion: ""};

                testVersion.populate(version);
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
            const testMember = new NakedObjects.PropertyMember({} as any, testObject, "");
            const testVm = new NakedObjects.Angular.Gemini.DomainObjectViewModel();

            beforeEach(inject(($q, $timeout, $rootScope, $location, $routeParams, handlers: NakedObjects.Angular.Gemini.IHandlers, context: NakedObjects.Angular.Gemini.IContext) => {
                $scope = $rootScope.$new();
                spyOn(testVm, "hideEdit").and.returnValue(false);

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
            const testMember = new NakedObjects.PropertyMember({} as any, testObject, "");

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