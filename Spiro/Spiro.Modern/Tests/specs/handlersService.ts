/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.angular.services.handlers.ts" />
/// <reference path="helpers.ts" />
describe('handlers Service', () => {

    var $scope;

    beforeEach(module('app'));

    describe('handleCollectionResult', () => {

        var getCollection;

        describe('if it finds collection', () => {


            var testObject = new Spiro.ListRepresentation();
            var testViewModel = { test: testObject };

            var collectionViewModel;


            beforeEach(inject(($rootScope, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory) => {
                $scope = $rootScope.$new();

                getCollection = spyOnPromise(context, 'getCollection', testObject);
                collectionViewModel = spyOn(viewModelFactory, 'collectionViewModel').andReturn(testViewModel);

                handlers.handleCollectionResult($scope);
            }));


            it('should update the scope', () => {
                expect(getCollection).toHaveBeenCalled();
                expect(collectionViewModel).toHaveBeenCalledWith(testObject);

                expect($scope.collection).toEqual(testViewModel);
                expect($scope.collectionTemplate).toEqual("Content/partials/nestedCollection.html");
            });
        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getCollection = spyOnPromiseFail(context, 'getCollection', testObject);
                setError = spyOn(context, 'setError');

                handlers.handleCollectionResult($scope);
            }));


            it('should update the context', () => {
                expect(getCollection).toHaveBeenCalled();
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.collection).toBeUndefined();
                expect($scope.collectionTemplate).toBeUndefined();
            });
        });
    });

    describe('handleCollection', () => {

        var getObject;

        describe('if it finds object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testMember = new Spiro.CollectionMember({}, testObject);
            var testDetails = new Spiro.CollectionRepresentation();
            var testViewModel = { test: testObject };


            var collectionMember;
            var collectionDetails;
            var populate;
            var collectionViewModel;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory, repLoader: Spiro.Angular.IRepLoader) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise(context, 'getObject', testObject);

                collectionMember = spyOn(testObject, "collectionMember").andReturn(testMember);
                collectionDetails = spyOn(testMember, "getDetails").andReturn(testDetails);
                populate = spyOnPromise(repLoader, "populate", testDetails);
                collectionViewModel = spyOn(viewModelFactory, 'collectionViewModel').andReturn(testViewModel);


                $routeParams.dt = "test";
                $routeParams.id = "1";
                $routeParams.collection = "aCollection";

                handlers.handleCollection($scope);
            }));


            it('should update the scope', () => {
                expect(getObject).toHaveBeenCalledWith("test", "1");
                expect(collectionMember).toHaveBeenCalledWith("aCollection");
                expect(collectionDetails).toHaveBeenCalled();
                expect(populate).toHaveBeenCalledWith(testDetails);
                expect(collectionViewModel).toHaveBeenCalledWith(testDetails);

                expect($scope.collection).toEqual(testViewModel);
                expect($scope.collectionTemplate).toEqual("Content/partials/nestedCollection.html");
            });

        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromiseNestedFail(context, 'getObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.dt = "test";
                $routeParams.id = "1";

                handlers.handleCollection($scope);
            }));


            it('should update the context', () => {
                expect(getObject).toHaveBeenCalledWith("test", "1");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.collection).toBeUndefined();
                expect($scope.collectionTemplate).toBeUndefined();
            });
        });

    });

    describe('handleActionDialog', () => {

        var getObject;

        describe('if it finds object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testMember = new Spiro.ActionMember({}, testObject);
            var testDetails = new Spiro.ActionRepresentation();
            var testViewModel = { test: testObject };

            var actionMember;
            var actionDetails;
            var populate;
            var dialogViewModel;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory, repLoader: Spiro.Angular.IRepLoader) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise(context, 'getObject', testObject);

                actionMember = spyOn(testObject, "actionMember").andReturn(testMember);
                actionDetails = spyOn(testMember, "getDetails").andReturn(testDetails);
                populate = spyOnPromise(repLoader, "populate", testDetails);
                dialogViewModel = spyOn(viewModelFactory, 'dialogViewModel').andReturn(testViewModel);

            }));

            describe('if it is a service', () => {

                beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers) => {
                    spyOn(testDetails, "extensions").andReturn({ hasParams: true });

                    $routeParams.sid = "testService";
                    $routeParams.action = "anAction";

                    handlers.handleActionDialog($scope);
                }));

                it('should update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("testService", undefined);
                    expect(actionMember).toHaveBeenCalledWith("anAction");
                    expect(actionDetails).toHaveBeenCalled();
                    expect(populate).toHaveBeenCalledWith(testDetails);
                    expect(dialogViewModel).toHaveBeenCalledWith(testDetails, jasmine.any(Function));

                    expect($scope.dialog).toEqual(testViewModel);
                    expect($scope.dialogTemplate).toEqual("Content/partials/dialog.html");
                });

            });


            describe('if it has params', () => {

                beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers) => {
                    spyOn(testDetails, "extensions").andReturn({ hasParams: true });
                    $routeParams.dt = "test";
                    $routeParams.id = "1";
                    $routeParams.action = "anAction";

                    handlers.handleActionDialog($scope);
                }));

                it('should update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(actionMember).toHaveBeenCalledWith("anAction");
                    expect(actionDetails).toHaveBeenCalled();
                    expect(populate).toHaveBeenCalledWith(testDetails);
                    expect(dialogViewModel).toHaveBeenCalledWith(testDetails, jasmine.any(Function));

                    expect($scope.dialog).toEqual(testViewModel);
                    expect($scope.dialogTemplate).toEqual("Content/partials/dialog.html");
                });

            });

            describe('if it has no params', () => {

                beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers) => {
                    spyOn(testDetails, "extensions").andReturn({ hasParams: false });
                    $routeParams.dt = "test";
                    $routeParams.id = "1";
                    $routeParams.action = "anAction";

                    handlers.handleActionDialog($scope);
                }));

                it('should update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(actionMember).toHaveBeenCalledWith("anAction");
                    expect(actionDetails).toHaveBeenCalled();
                    expect(populate).toHaveBeenCalledWith(testDetails);
                    expect(dialogViewModel).wasNotCalled();

                    expect($scope.dialog).toBeUndefined();
                    expect($scope.dialogTemplate).toBeUndefined();
                });

            });

        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromiseNestedFail(context, 'getObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.dt = "test";
                $routeParams.id = "1";

                handlers.handleActionDialog($scope);
            }));


            it('should update the context', () => {
                expect(getObject).toHaveBeenCalledWith("test", "1");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.dialog).toBeUndefined();
                expect($scope.dialogTemplate).toBeUndefined();
            });
        });

    });

    describe('handleActionResult', () => {

        var getObject;

        describe('if it finds object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testMember = new Spiro.ActionMember({}, testObject);
            var testDetails = new Spiro.ActionRepresentation();
            var testResult = new Spiro.ActionResultRepresentation();

            var actionMember;
            var actionDetails;
            var actionResult;
            var populate;
            var setResult;

            beforeEach(inject(($rootScope, $routeParams, repHandlers: Spiro.Angular.IRepHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory, repLoader: Spiro.Angular.IRepLoader) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise(context, 'getObject', testObject);

                actionMember = spyOn(testObject, "actionMember").andReturn(testMember);
                actionDetails = spyOn(testMember, "getDetails").andReturn(testDetails);
                actionResult = spyOn(testDetails, "getInvoke").andReturn(testResult);
                populate = spyOnPromiseConditional(repLoader, "populate", testDetails, testResult);

                setResult = spyOn(repHandlers, "setResult");
            }));

            describe('if it is a service', () => {

                beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers) => {
                    spyOn(testMember, "extensions").andReturn({ hasParams: false });

                    $routeParams.sid = "testService";
                    $routeParams.action = "anAction";

                    handlers.handleActionResult($scope);
                }));

                it('should update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("testService", undefined);
                    expect(actionMember).toHaveBeenCalledWith("anAction");
                    expect(actionDetails).toHaveBeenCalled();
                    expect(actionResult).toHaveBeenCalled();

                    expect(populate).toHaveBeenCalled();
                    expect(setResult).toHaveBeenCalled();
                });

            });

            describe('if it has no params', () => {

                beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers) => {
                    spyOn(testMember, "extensions").andReturn({ hasParams: false });
                    $routeParams.dt = "test";
                    $routeParams.id = "1";
                    $routeParams.action = "anAction";

                    handlers.handleActionResult($scope);
                }));

                it('should update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(actionMember).toHaveBeenCalledWith("anAction");
                    expect(actionDetails).toHaveBeenCalled();
                    expect(actionResult).toHaveBeenCalled();

                    expect(populate).toHaveBeenCalled();
                    expect(setResult).toHaveBeenCalled();
                });

            });

            describe('if it has params', () => {

                beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers) => {
                    spyOn(testMember, "extensions").andReturn({ hasParams: true });
                    $routeParams.dt = "test";
                    $routeParams.id = "1";
                    $routeParams.action = "anAction";

                    handlers.handleActionResult($scope);
                }));

                it('should not update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(actionMember).toHaveBeenCalledWith("anAction");
                    expect(actionDetails).wasNotCalled();
                    expect(actionResult).wasNotCalled();

                    expect(setResult).wasNotCalled();
                });

            });

        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise2NestedFail(context, 'getObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.dt = "test";
                $routeParams.id = "1";

                handlers.handleActionResult($scope);
            }));


            it('should update the context', () => {
                expect(getObject).toHaveBeenCalledWith("test", "1");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.dialog).toBeUndefined();
                expect($scope.dialogTemplate).toBeUndefined();
            });
        });

    });

    describe('handleProperty', () => {

        var getObject;

        describe('if it finds object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testMember = new Spiro.PropertyMember({}, testObject);
            var testDetails = new Spiro.PropertyRepresentation();
            var testValue = new Spiro.Value({});
            var testLink = new Spiro.Link();
            var testTarget = new Spiro.DomainObjectRepresentation();
            var testViewModel = { test: testTarget };

            var propertyMember;
            var propertyDetails;
            var setNestedObject;
            var objectViewModel;

            var populate;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory, repLoader: Spiro.Angular.IRepLoader) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise(context, 'getObject', testObject);

                propertyMember = spyOn(testObject, "propertyMember").andReturn(testMember);
                propertyDetails = spyOn(testMember, "getDetails").andReturn(testDetails);

                spyOn(testDetails, "value").andReturn(testValue);
                spyOn(testValue, "link").andReturn(testLink);
                spyOn(testLink, "getTarget").andReturn(testTarget);

                populate = spyOnPromiseConditional(repLoader, "populate", testDetails, testTarget);

                objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                setNestedObject = spyOn(context, 'setNestedObject');

                $routeParams.dt = "test";
                $routeParams.id = "1";
                $routeParams.property = "aProperty";

                handlers.handleProperty($scope);
            }));

            it('should update the scope', () => {
                expect(getObject).toHaveBeenCalledWith("test", "1");
                expect(propertyMember).toHaveBeenCalledWith("aProperty");
                expect(propertyDetails).toHaveBeenCalled();

                expect(populate).toHaveBeenCalled();

                expect(setNestedObject).toHaveBeenCalledWith(testTarget);

                expect($scope.result).toEqual(testViewModel);
                expect($scope.nestedTemplate).toEqual("Content/partials/nestedObject.html");
            });


        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise2NestedFail(context, 'getObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.dt = "test";
                $routeParams.id = "1";

                handlers.handleProperty($scope);
            }));


            it('should update the context', () => {
                expect(getObject).toHaveBeenCalledWith("test", "1");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.dialog).toBeUndefined();
                expect($scope.dialogTemplate).toBeUndefined();
            });
        });

    });


    describe('handleResult', () => {
        var getNestedObject;

        describe('if it finds object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testViewModel = { test: testObject };

            var objectViewModel;
            var setNestedObject;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory) => {
                $scope = $rootScope.$new();

                getNestedObject = spyOnPromise(context, 'getNestedObject', testObject);

                objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                setNestedObject = spyOn(context, 'setNestedObject');


                $routeParams.resultObject = "test-1";

                handlers.handleResult($scope);
            }));


            it('should update the scope', () => {
                expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                expect(objectViewModel).toHaveBeenCalledWith(testObject);
                expect(setNestedObject).toHaveBeenCalledWith(testObject);

                expect($scope.result).toEqual(testViewModel);
                expect($scope.nestedTemplate).toEqual("Content/partials/nestedObject.html");
            });

        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getNestedObject = spyOnPromiseFail(context, 'getNestedObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.resultObject = "test-1";

                handlers.handleResult($scope);
            }));


            it('should update the context', () => {
                expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.result).toBeUndefined();
                expect($scope.nestedTemplate).toBeUndefined();
            });
        });
    });

    describe('handleCollectionItem', () => {
        var getNestedObject;

        describe('if it finds object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testViewModel = { test: testObject };

            var objectViewModel;
            var setNestedObject;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory) => {
                $scope = $rootScope.$new();

                getNestedObject = spyOnPromise(context, 'getNestedObject', testObject);

                objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                setNestedObject = spyOn(context, 'setNestedObject');

                $routeParams.collectionItem = "test/1";

                handlers.handleCollectionItem($scope);
            }));


            it('should update the scope', () => {
                expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                expect(objectViewModel).toHaveBeenCalledWith(testObject);
                expect(setNestedObject).toHaveBeenCalledWith(testObject);

                expect($scope.result).toEqual(testViewModel);
                expect($scope.nestedTemplate).toEqual("Content/partials/nestedObject.html");
            });

        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getNestedObject = spyOnPromiseFail(context, 'getNestedObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.collectionItem = "test/1";

                handlers.handleCollectionItem($scope);
            }));


            it('should update the context', () => {
                expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.result).toBeUndefined();
                expect($scope.nestedTemplate).toBeUndefined();
            });
        });


    });


    describe('handleServices', () => {
        var getServices;

        describe('if it finds services', () => {

            var testObject = new Spiro.DomainServicesRepresentation();
            var testViewModel = { test: testObject };

            var servicesViewModel;
            var setObject;
            var setNestedObject;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory) => {
                $scope = $rootScope.$new();

                getServices = spyOnPromise(context, 'getServices', testObject);

                servicesViewModel = spyOn(viewModelFactory, 'servicesViewModel').andReturn(testViewModel);
                setNestedObject = spyOn(context, 'setNestedObject');
                setObject = spyOn(context, 'setObject');

                handlers.handleServices($scope);
            }));


            it('should update the scope', () => {
                expect(getServices).toHaveBeenCalled();
                expect(servicesViewModel).toHaveBeenCalledWith(testObject);
                expect(setObject).toHaveBeenCalledWith(null);
                expect(setNestedObject).toHaveBeenCalledWith(null);

                expect($scope.services).toEqual(testViewModel);
            });

        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getServices = spyOnPromiseFail(context, 'getServices', testObject);
                setError = spyOn(context, 'setError');

                handlers.handleServices($scope);
            }));


            it('should update the context', () => {
                expect(getServices).toHaveBeenCalled();
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.services).toBeUndefined();
            });
        });


    });

    describe('handleService', () => {
        var getObject;

        describe('if it finds service', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testViewModel = { test: testObject };

            var serviceViewModel;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise(context, 'getObject', testObject);

                serviceViewModel = spyOn(viewModelFactory, 'serviceViewModel').andReturn(testViewModel);

                $routeParams.sid = "test";

                handlers.handleService($scope);
            }));


            it('should update the scope', () => {
                expect(getObject).toHaveBeenCalledWith("test");
                expect(serviceViewModel).toHaveBeenCalledWith(testObject);

                expect($scope.object).toEqual(testViewModel);
            });

        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromiseFail(context, 'getObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.sid = "test";

                handlers.handleService($scope);
            }));


            it('should update the context', () => {
                expect(getObject).toHaveBeenCalledWith("test");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.object).toBeUndefined();
            });
        });
    });

    describe('handleObject', () => {
        var getObject;

        describe('if it finds object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testViewModel = { test: testObject };

            var objectViewModel;
            var setNestedObject;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromise(context, 'getObject', testObject);

                objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                setNestedObject = spyOn(context, 'setNestedObject');

                $routeParams.dt = "test";
                $routeParams.id = "1";
            }));

            describe('not in edit mode', () => {

                beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers) => {

                    handlers.handleObject($scope);
                }));

                it('should update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(objectViewModel).toHaveBeenCalledWith(testObject);
                    expect(setNestedObject).toHaveBeenCalledWith(null);

                    expect($scope.object).toEqual(testViewModel);
                    expect($scope.actionTemplate).toEqual("Content/partials/actions.html");
                    expect($scope.propertiesTemplate).toEqual("Content/partials/viewProperties.html");
                });
            });

            describe('in edit mode', () => {

                var propertyMem = new Spiro.PropertyMember({}, testObject);
                var propertyRep = new Spiro.PropertyRepresentation();

                var populate;

                beforeEach(inject(($rootScope, $q, $routeParams, repLoader: Spiro.Angular.IRepLoader, handlers: Spiro.Angular.IHandlers) => {

                    spyOn(testObject, 'propertyMembers').andReturn([propertyMem]);
                    spyOn(propertyMem, 'getDetails').andReturn(propertyRep);

                    spyOnPromise($q, 'all', [propertyRep]);


                    populate = spyOnPromise(repLoader, "populate", propertyRep);

                    $routeParams.editMode = "test";
                    handlers.handleEditObject($scope);
                }));

                it('should update the scope', () => {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(objectViewModel).toHaveBeenCalledWith(testObject, [propertyRep], jasmine.any(Function));
                    expect(setNestedObject).toHaveBeenCalledWith(null);

                    expect($scope.object).toEqual(testViewModel);
                    expect($scope.actionTemplate).toEqual("");
                    expect($scope.propertiesTemplate).toEqual("Content/partials/editProperties.html");
                });
            });
        });

        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getObject = spyOnPromiseFail(context, 'getObject', testObject);
                setError = spyOn(context, 'setError');

                $routeParams.dt = "test";
                $routeParams.id = "1";

                handlers.handleObject($scope);
            }));

            it('should update the context', () => {
                expect(getObject).toHaveBeenCalledWith("test", "1");
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.object).toBeUndefined();
                expect($scope.actionTemplate).toBeUndefined();
                expect($scope.propertiesTemplate).toBeUndefined();
            });
        });
    });

    describe('handleTransientObject', () => {
        var getTransientObject;

        describe('if transient is found', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testViewModel = { test: testObject };

            var objectViewModel;
            var setNestedObject;

            var propertyMem = new Spiro.PropertyMember({}, testObject);
            var propertyRep = new Spiro.PropertyRepresentation();

            var populate;

            beforeEach(inject(($rootScope, $routeParams, $q, repLoader: Spiro.Angular.IRepLoader, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, viewModelFactory: Spiro.Angular.IViewModelFactory) => {
                $scope = $rootScope.$new();

                getTransientObject = spyOnPromise(context, 'getTransientObject', testObject);

                objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                setNestedObject = spyOn(context, 'setNestedObject');

                spyOn(testObject, 'propertyMembers').andReturn([propertyMem]);
                spyOn(propertyMem, 'getDetails').andReturn(propertyRep);
                spyOn(testObject, 'domainType').andReturn("test");

                spyOnPromise($q, 'all', [propertyRep]);

                populate = spyOnPromise(repLoader, "populate", propertyRep);

                handlers.handleTransientObject($scope);
            }));


            it('should update the scope', () => {
                expect(getTransientObject).toHaveBeenCalled();
                expect(objectViewModel).toHaveBeenCalledWith(testObject, null, jasmine.any(Function));
                expect(setNestedObject).toHaveBeenCalledWith(null);

                expect($scope.object).toEqual(testViewModel);
                expect($scope.actionTemplate).toEqual("");
                expect($scope.propertiesTemplate).toEqual("Content/partials/editProperties.html");
            });


        });

        describe('if transient is not found', () => {

            var testObject = null;
            var nav;
         
            beforeEach(inject(($rootScope, $routeParams, $q, repLoader: Spiro.Angular.IRepLoader, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext, navigation: Spiro.Angular.INavigation) => {
                $scope = $rootScope.$new();

                getTransientObject = spyOnPromise(context, 'getTransientObject', testObject);
                nav = spyOn(navigation, 'back');
            
                handlers.handleTransientObject($scope);
            }));

            it('should navigate back', () => {
                expect(getTransientObject).toHaveBeenCalled();
                expect(nav).toHaveBeenCalled();
            });


        });


        describe('if it has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setError;

            beforeEach(inject(($rootScope, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                getTransientObject = spyOnPromiseFail(context, 'getTransientObject', testObject);
                setError = spyOn(context, 'setError');

                handlers.handleTransientObject($scope);
            }));

            it('should update the context', () => {
                expect(getTransientObject).toHaveBeenCalledWith();
                expect(setError).toHaveBeenCalledWith(testObject);

                expect($scope.object).toBeUndefined();
                expect($scope.actionTemplate).toBeUndefined();
                expect($scope.propertiesTemplate).toBeUndefined();
            });
        });
    });


    describe('handleError', () => {

        beforeEach(inject(($rootScope, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
            $scope = $rootScope.$new();

            spyOn(context, 'getError').andReturn(new Spiro.ErrorRepresentation({ message: "", stacktrace: [] }));

            handlers.handleError($scope);
        }));


        it('should set a error data', () => {
            expect($scope.error).toBeDefined();
            expect($scope.errorTemplate).toEqual("Content/partials/error.html");
        });

    });

    describe('handleBackground', () => {

        var navService: Spiro.Angular.INavigation;

        beforeEach(inject(($rootScope, handlers: Spiro.Angular.IHandlers, $location: ng.ILocationService, urlHelper: Spiro.Angular.IUrlHelper, color: Spiro.Angular.IColor, navigation: Spiro.Angular.INavigation) => {
            $scope = $rootScope.$new();
            navService = navigation;

            spyOn(color, 'toColorFromHref').andReturn("acolor");
            spyOn(urlHelper, 'toAppUrl').andReturn("aurl");
            spyOn(navigation, 'push');

            handlers.handleBackground($scope);
        }));


        it('should set scope variables', () => {
            expect($scope.backgroundColor).toEqual("acolor");
            expect($scope.closeNestedObject).toEqual("aurl");
            expect($scope.closeCollection).toEqual("aurl");
            expect(navService.push).toHaveBeenCalled();
        });


    });


    describe('handleAppBar', () => {


        function expectAppBarData() {
            expect($scope.appBar).toBeDefined();
            expect($scope.appBar.goHome).toEqual("#/");
            expect($scope.appBar.template).toEqual("Content/partials/appbar.html");
            expect($scope.appBar.goBack).toBeDefined();
            expect($scope.appBar.goForward).toBeDefined();
        }

        describe('handleAppBar when not viewing an  object', () => {

            beforeEach(inject(($rootScope, handlers: Spiro.Angular.IHandlers) => {
                $scope = $rootScope.$new();

                handlers.handleAppBar($scope);
            }));

            it('should set appBar data', () => {
                expectAppBarData();
            });

            it('should disable edit button', () => {
                expect($scope.appBar.hideEdit).toEqual(true);
                expect($scope.appBar.doEdit).toBeUndefined();
            });

        });

        describe('handleAppBar when viewing an editable object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testMember = new Spiro.PropertyMember({}, testObject);


            beforeEach(inject(($rootScope, $location, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                $routeParams.dt = "test";
                $routeParams.id = "1";

                spyOnPromise(context, 'getObject', testObject);
                spyOn(testObject, 'propertyMembers').andReturn([testMember]);

                spyOn($location, 'path').andReturn("aPath");

                handlers.handleAppBar($scope);
            }));

            it('should set appBar data', () => {
                expectAppBarData();
            });

            it('should enable edit button', () => {
                expect($scope.appBar.hideEdit).toBe(false);
                expect($scope.appBar.doEdit).toEqual("?editMode=true");
            });

        });

        describe('handleAppBar when viewing a non editable object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testMember = new Spiro.PropertyMember({}, testObject);


            beforeEach(inject(($rootScope, $location, $routeParams, handlers: Spiro.Angular.IHandlers, context: Spiro.Angular.IContext) => {
                $scope = $rootScope.$new();

                $routeParams.dt = "test";
                $routeParams.id = "1";

                spyOnPromise(context, 'getObject', testObject);
                spyOn(testObject, 'propertyMembers').andReturn([testMember]);
                spyOn(testMember, 'disabledReason').andReturn("disabled");

                spyOn($location, 'path').andReturn("aPath");

                handlers.handleAppBar($scope);
            }));

            it('should set appBar data', () => {
                expectAppBarData();
            });

            it('should disable edit button', () => {
                expect($scope.appBar.hideEdit).toBe(true);
            });

        });

    });

    describe('setResult helper', () => {

        var testActionResult = new Spiro.ActionResultRepresentation();
        var testViewModel = new Spiro.Angular.DialogViewModel();
        var location;

        beforeEach(inject($location => {
            location = $location;
        }));

        describe('result is null', () => {

            var testResult = new Spiro.Result(null, 'object');

            beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers) => {
                spyOn(testActionResult, 'result').andReturn(testResult);
                spyOn(testActionResult, 'resultType').andReturn("void");
                repHandlers.setResult(testActionResult, testViewModel);
            }));

            it('should not set view model error', () => {
                expect(testViewModel.message).toBeUndefined();
                expect(location.search()).toEqual({});
            });
        });


        describe('result is object', () => {

            var testObject = new Spiro.DomainObjectRepresentation();
            var testResult = new Spiro.Result({}, 'object');
            var setNestedObject;

            beforeEach(inject(($routeParams, context: Spiro.Angular.IContext) => {

                spyOn(testActionResult, 'result').andReturn(testResult);
                spyOn(testActionResult, 'resultType').andReturn('object');
                spyOn(testResult, 'object').andReturn(testObject);
                setNestedObject = spyOn(context, 'setNestedObject');

                spyOn(testObject, 'domainType').andReturn("test");
                spyOn(testObject, 'instanceId').andReturn("1");
                spyOn(testObject, 'persistLink').andReturn(null);

                $routeParams.action = "anAction";
            }));


            describe('with show flag', () => {

                beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers) => {
                    testViewModel.parameters = [];
                    testViewModel.show = true;

                    repHandlers.setResult(testActionResult, testViewModel);
                }));

                it('should set nested object and search', () => {
                    expect(setNestedObject).toHaveBeenCalledWith(testObject);
                    expect(location.search()).toEqual({ resultObject: 'test-1', action: 'anAction' });
                });

            });

            describe('without show flag', () => {

                beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers) => {

                    repHandlers.setResult(testActionResult);
                }));

                it('should set nested object and search', () => {
                    expect(setNestedObject).toHaveBeenCalledWith(testObject);
                    expect(location.search()).toEqual({ resultObject: 'test-1' });
                });

            });
        });

        describe('result is list', () => {

            var testList = new Spiro.ListRepresentation();
            var testResult = new Spiro.Result([], 'list');
            var testNullResult = new Spiro.Result(null, 'list');
            var setCollection;

            beforeEach(inject(($routeParams, context: Spiro.Angular.IContext) => {


                spyOn(testActionResult, 'resultType').andReturn('list');
                spyOn(testResult, 'list').andReturn(testList);
                setCollection = spyOn(context, 'setCollection');

                $routeParams.action = "anAction";
            }));

            describe('with show flag', () => {

                var testParameters = [new Spiro.Angular.ParameterViewModel(), new Spiro.Angular.ParameterViewModel()];
                testParameters[0].type = "scalar";
                testParameters[0].value = "1";
                testParameters[1].type = "scalar";
                testParameters[1].value = "2";

                beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers) => {
                    spyOn(testActionResult, 'result').andReturn(testResult);
                    testViewModel.parameters = testParameters;

                    repHandlers.setResult(testActionResult, testViewModel);
                }));

                it('should set collection and search', () => {
                    expect(setCollection).toHaveBeenCalledWith(testList);
                    expect(location.search()).toEqual({ resultCollection: 'anAction/1/2', action: 'anAction/1/2' });
                });
            });

            describe('without show flag', () => {

                beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers) => {
                    spyOn(testActionResult, 'result').andReturn(testResult);
                    repHandlers.setResult(testActionResult);
                }));

                it('should set collection and search', () => {
                    expect(setCollection).toHaveBeenCalledWith(testList);
                    expect(location.search()).toEqual({ resultCollection: 'anAction' });
                });

            });

            describe('result is null', () => {

                beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers) => {
                    spyOn(testActionResult, 'result').andReturn(testNullResult);
                    repHandlers.setResult(testActionResult, testViewModel);
                }));

                it('should set view model error', () => {
                    expect(testViewModel.message).toBe("no result found");
                    expect(location.search()).toEqual({});
                });
            });

        });
    });

    describe('invokeAction helper', () => {

        var testAction = new Spiro.ActionRepresentation();
        var testActionResult = new Spiro.ActionResultRepresentation();
        var testViewModel = new Spiro.Angular.DialogViewModel();

        var testParameters = [new Spiro.Angular.ParameterViewModel(), new Spiro.Angular.ParameterViewModel()];
        testParameters[0].value = "1";
        testParameters[1].value = "2";
        testParameters[0].id = "one";
        testParameters[1].id = "two";
        testParameters[0].type = "scalar";
        testParameters[1].type = "scalar";

        var populate;
        var clearMessages;
        var setParameter;

        beforeEach(inject($rootScope => {

            spyOn(testAction, 'getInvoke').andReturn(testActionResult);

            clearMessages = spyOn(testViewModel, 'clearMessages');
            setParameter = spyOn(testActionResult, 'setParameter');
            $scope = $rootScope.$new();
        }));

        describe('invoke is successful', () => {

            var setResult;


            beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers, repLoader: Spiro.Angular.IRepLoader) => {
                populate = spyOnPromise(repLoader, 'populate', testActionResult);
                setResult = spyOn(repHandlers, 'setResult');
                testViewModel.parameters = testParameters;
                testViewModel.show = true;

                repHandlers.invokeAction($scope, testAction, testViewModel);
            }));

            it('should set result', () => {
                expect(setParameter.callCount == 2).toBeTruthy();

                expect(setParameter.calls[0].args[0]).toBe("one");
                expect(setParameter.calls[0].args[1].scalar()).toBe("1");

                expect(setParameter.calls[1].args[0]).toBe("two");
                expect(setParameter.calls[1].args[1].scalar()).toBe("2");

                expect(clearMessages).toHaveBeenCalled();
                expect(populate).toHaveBeenCalledWith(testActionResult, true);

                expect(setResult).toHaveBeenCalledWith(testActionResult, testViewModel);
            });
        });

        describe('if invoke has an error', () => {

            var testObject = new Spiro.ErrorRepresentation();
            var setInvokeUpdateError;

            beforeEach(inject(($rootScope, $routeParams, repHandlers: Spiro.Angular.IRepHandlers, repLoader: Spiro.Angular.IRepLoader) => {
                populate = spyOnPromiseFail(repLoader, 'populate', testObject);
                setInvokeUpdateError = spyOn(repHandlers, 'setInvokeUpdateError');
                repHandlers.invokeAction($scope, testAction, testViewModel);
            }));

            it('should set the error', () => {
                expect(setInvokeUpdateError).toHaveBeenCalledWith($scope, testObject, testParameters, testViewModel);
            });
        });
    });

    describe('updateObject helper', () => {

        var testObject = new Spiro.DomainObjectRepresentation();
        var testUpdatedObject = new Spiro.DomainObjectRepresentation();
        var testUpdate = {};
        var testViewModel = new Spiro.Angular.DomainObjectViewModel();
        var testRawLinks = { testLinks: "" };

        var testProperties = [new Spiro.Angular.PropertyViewModel(), new Spiro.Angular.PropertyViewModel(), new Spiro.Angular.PropertyViewModel()];

        testProperties[0].id = "one";
        testProperties[0].value = "1";
        testProperties[0].isEditable = true;
        testProperties[0].type = 'scalar';

        testProperties[1].id = "two";
        testProperties[1].value = "2";
        testProperties[1].isEditable = true;
        testProperties[1].type = 'scalar';

        testProperties[2].id = "three";
        testProperties[2].value = "3";
        testProperties[2].isEditable = false;
        testProperties[2].type = 'scalar';


        var populate;
        var setProperty;
        var set;

        beforeEach(inject($rootScope => {

            (<any>testUpdate).setProperty = () => {};

            spyOn(testObject, 'getUpdateMap').andReturn(testUpdate);
            setProperty = spyOn(testUpdate, 'setProperty');
            testViewModel.properties = testProperties;

            spyOn(testObject, 'get').andReturn(testRawLinks);
            set = spyOn(testUpdatedObject, 'set');

            $scope = $rootScope.$new();
        }));

        describe('update is successful', () => {

            var setObject;

            var location;
            var cacheFactory;
            var testCache = {};
            var remove;

            beforeEach(inject(($location, $cacheFactory, repHandlers: Spiro.Angular.IRepHandlers, repLoader: Spiro.Angular.IRepLoader, context: Spiro.Angular.IContext) => {

                setObject = spyOn(context, 'setObject');

                location = $location;
                cacheFactory = $cacheFactory;
                (<any>testCache).remove = () => {};

                populate = spyOnPromise(repLoader, 'populate', testUpdatedObject);

                spyOn(cacheFactory, 'get').andReturn(testCache);
                remove = spyOn(testCache, 'remove');

                testUpdatedObject.hateoasUrl = "testUrl";

                repHandlers.updateObject($scope, testObject, testViewModel);
            }));

            it('should set result', () => {
                expect(setProperty.callCount == 2).toBeTruthy();

                expect(setProperty.calls[0].args[0]).toBe("one");
                expect(setProperty.calls[0].args[1].scalar()).toBe("1");

                expect(setProperty.calls[1].args[0]).toBe("two");
                expect(setProperty.calls[1].args[1].scalar()).toBe("2");

                expect(remove).toHaveBeenCalledWith("testUrl");
                expect(location.search()).toEqual({});
                expect(set).toHaveBeenCalledWith('links', testRawLinks);
                expect(populate).toHaveBeenCalledWith(testUpdate, true, new Spiro.DomainObjectRepresentation());
                expect(setObject).toHaveBeenCalledWith(testUpdatedObject);
            });
        });

        describe('if update has an error', () => {

            var testError = new Spiro.ErrorRepresentation();
            var setInvokeUpdateError;
            var editableProperties = _.filter(testProperties, (tp: any) => tp.isEditable);

            beforeEach(inject(($rootScope, $routeParams, repHandlers: Spiro.Angular.IRepHandlers, repLoader: Spiro.Angular.IRepLoader) => {
                populate = spyOnPromiseFail(repLoader, 'populate', testError);
                setInvokeUpdateError = spyOn(repHandlers, 'setInvokeUpdateError');
                repHandlers.updateObject($scope, testObject, testViewModel);
            }));

            it('should set the error', () => {
                expect(setInvokeUpdateError).toHaveBeenCalledWith($scope, testError, editableProperties, testViewModel);
            });
        });
    });

    describe('setInvokeUpdateError helper', () => {

        var testViewModel = new Spiro.Angular.MessageViewModel();
        testViewModel.message = "";


        beforeEach(inject($rootScope => {
            $scope = $rootScope.$new();
        }));

        describe('if error is errorMap', () => {

            var error = { "one": { "value": "1", "invalidReason": "a reason" }, "two": { "value": "2" }, "x-ro-invalid-reason": "another reason" };

            var errorMap = new Spiro.ErrorMap(error, "status", "a warning message");

            var vm1 = new Spiro.Angular.ValueViewModel();
            var vm2 = new Spiro.Angular.ValueViewModel();

            vm1.id = "one";
            vm2.id = "two";

            var vms = [vm1, vm2];

            beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers) => {
                repHandlers.setInvokeUpdateError($scope, errorMap, vms, testViewModel);
            }));

            it('should set the parameters and error', () => {
                expect(vms[0].value).toBe("1");
                expect(vms[0].message).toBe("a reason");
                expect(vms[1].value).toBe("2");
                expect(vms[1].message).toBeUndefined();

                expect(testViewModel.message).toBe("another reason");
            });
        });

        describe('if error is errorRep', () => {

            var testError = new Spiro.ErrorRepresentation();

            var error;
            var path;
            var errorPath;

            beforeEach(inject((repHandlers: Spiro.Angular.IRepHandlers, $location: ng.ILocationService, context: Spiro.Angular.IContext, urlHelper: Spiro.Angular.IUrlHelper) => {
                error = spyOn(context, 'setError');
                path = spyOn($location, 'path');
                errorPath = spyOn(urlHelper, 'toErrorPath').andReturn("apath");

                repHandlers.setInvokeUpdateError($scope, testError, [], testViewModel);
            }));

            it('should set the location path', () => {
                expect(error).toHaveBeenCalledWith(testError);
                expect(path).toHaveBeenCalledWith("apath");
                expect(errorPath).toHaveBeenCalled();

            });

        });

        describe('if error is string', () => {

            var errorMessage = 'an error message';

            beforeEach(inject(($rootScope, $routeParams, repHandlers: Spiro.Angular.IRepHandlers) => {
                repHandlers.setInvokeUpdateError($scope, errorMessage, [], testViewModel);
            }));

            it('should set the scope ', () => {
                expect(testViewModel.message).toBe(errorMessage);
            });
        });
    });
});