/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.angular.services.handlers.ts" />
describe('Services', function () {
    // helpers
    function spyOnPromise(tgt, func, mock) {
        var mp = {};

        mp.then = function (f) {
            return f(mock);
        };

        return spyOn(tgt, func).andReturn(mp);
    }

    function spyOnPromiseConditional(tgt, func, mock1, mock2) {
        var mp = {};
        var first = true;

        mp.then = function (f) {
            var result = first ? f(mock1) : f(mock2);
            first = false;
            return result;
        };

        return spyOn(tgt, func).andReturn(mp);
    }

    function mockPromiseFail(mock) {
        var mp = {};

        mp.then = function (fok, fnok) {
            return fnok(mock);
        };

        return mp;
    }

    // TODO sure this could be recursive - fix once tests are running
    function spyOnPromiseFail(tgt, func, mock) {
        var mp = {};

        mp.then = function (fok, fnok) {
            return fnok ? fnok(mock) : fok(mock);
        };

        return spyOn(tgt, func).andReturn(mp);
    }

    function spyOnPromiseNestedFail(tgt, func, mock) {
        var mp = {};

        mp.then = function (fok) {
            var mmp = {};

            mmp.then = function (f1ok, f1nok) {
                return f1nok(mock);
            };

            return mmp;
        };

        return spyOn(tgt, func).andReturn(mp);
    }

    function spyOnPromise2NestedFail(tgt, func, mock) {
        var mp = {};

        mp.then = function (fok) {
            var mmp = {};

            mmp.then = function (f1ok) {
                var mmmp = {};

                mmmp.then = function (f2ok, f2nok) {
                    return f2nok(mock);
                };

                return mmmp;
            };

            return mmp;
        };

        return spyOn(tgt, func).andReturn(mp);
    }

    var $scope;

    beforeEach(module('app'));

    describe('handlers Service', function () {
        describe('handleCollectionResult', function () {
            var getCollection;

            describe('if it finds collection', function () {
                var testObject = new Spiro.ListRepresentation();
                var testViewModel = { test: testObject };

                var collectionViewModel;

                beforeEach(inject(function ($rootScope, handlers, context, viewModelFactory) {
                    $scope = $rootScope.$new();

                    getCollection = spyOnPromise(context, 'getCollection', testObject);
                    collectionViewModel = spyOn(viewModelFactory, 'collectionViewModel').andReturn(testViewModel);

                    handlers.handleCollectionResult($scope);
                }));

                it('should update the scope', function () {
                    expect(getCollection).toHaveBeenCalled();
                    expect(collectionViewModel).toHaveBeenCalledWith(testObject);

                    expect($scope.collection).toEqual(testViewModel);
                    expect($scope.collectionTemplate).toEqual("Content/partials/nestedCollection.html");
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, handlers, context) {
                    $scope = $rootScope.$new();

                    getCollection = spyOnPromiseFail(context, 'getCollection', testObject);
                    setError = spyOn(context, 'setError');

                    handlers.handleCollectionResult($scope);
                }));

                it('should update the context', function () {
                    expect(getCollection).toHaveBeenCalled();
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.collection).toBeUndefined();
                    expect($scope.collectionTemplate).toBeUndefined();
                });
            });
        });

        describe('handleCollection', function () {
            var getObject;

            describe('if it finds object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testMember = new Spiro.CollectionMember({}, testObject);
                var testDetails = new Spiro.CollectionRepresentation();
                var testViewModel = { test: testObject };

                var collectionMember;
                var collectionDetails;
                var populate;
                var collectionViewModel;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory, repLoader) {
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

                it('should update the scope', function () {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(collectionMember).toHaveBeenCalledWith("aCollection");
                    expect(collectionDetails).toHaveBeenCalled();
                    expect(populate).toHaveBeenCalledWith(testDetails);
                    expect(collectionViewModel).toHaveBeenCalledWith(testDetails);

                    expect($scope.collection).toEqual(testViewModel);
                    expect($scope.collectionTemplate).toEqual("Content/partials/nestedCollection.html");
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromiseNestedFail(context, 'getObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.dt = "test";
                    $routeParams.id = "1";

                    handlers.handleCollection($scope);
                }));

                it('should update the context', function () {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.collection).toBeUndefined();
                    expect($scope.collectionTemplate).toBeUndefined();
                });
            });
        });

        describe('handleActionDialog', function () {
            var getObject;

            describe('if it finds object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testMember = new Spiro.ActionMember({}, testObject);
                var testDetails = new Spiro.ActionRepresentation();
                var testViewModel = { test: testObject };

                var actionMember;
                var actionDetails;
                var populate;
                var dialogViewModel;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory, repLoader) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromise(context, 'getObject', testObject);

                    actionMember = spyOn(testObject, "actionMember").andReturn(testMember);
                    actionDetails = spyOn(testMember, "getDetails").andReturn(testDetails);
                    populate = spyOnPromise(repLoader, "populate", testDetails);
                    dialogViewModel = spyOn(viewModelFactory, 'dialogViewModel').andReturn(testViewModel);
                }));

                describe('if it is a service', function () {
                    beforeEach(inject(function ($rootScope, $routeParams, handlers) {
                        spyOn(testDetails, "extensions").andReturn({ hasParams: true });

                        $routeParams.sid = "testService";
                        $routeParams.action = "anAction";

                        handlers.handleActionDialog($scope);
                    }));

                    it('should update the scope', function () {
                        expect(getObject).toHaveBeenCalledWith("testService", undefined);
                        expect(actionMember).toHaveBeenCalledWith("anAction");
                        expect(actionDetails).toHaveBeenCalled();
                        expect(populate).toHaveBeenCalledWith(testDetails);
                        expect(dialogViewModel).toHaveBeenCalledWith(testDetails, jasmine.any(Function));

                        expect($scope.dialog).toEqual(testViewModel);
                        expect($scope.dialogTemplate).toEqual("Content/partials/dialog.html");
                    });
                });

                describe('if it has params', function () {
                    beforeEach(inject(function ($rootScope, $routeParams, handlers) {
                        spyOn(testDetails, "extensions").andReturn({ hasParams: true });
                        $routeParams.dt = "test";
                        $routeParams.id = "1";
                        $routeParams.action = "anAction";

                        handlers.handleActionDialog($scope);
                    }));

                    it('should update the scope', function () {
                        expect(getObject).toHaveBeenCalledWith("test", "1");
                        expect(actionMember).toHaveBeenCalledWith("anAction");
                        expect(actionDetails).toHaveBeenCalled();
                        expect(populate).toHaveBeenCalledWith(testDetails);
                        expect(dialogViewModel).toHaveBeenCalledWith(testDetails, jasmine.any(Function));

                        expect($scope.dialog).toEqual(testViewModel);
                        expect($scope.dialogTemplate).toEqual("Content/partials/dialog.html");
                    });
                });

                describe('if it has no params', function () {
                    beforeEach(inject(function ($rootScope, $routeParams, handlers) {
                        spyOn(testDetails, "extensions").andReturn({ hasParams: false });
                        $routeParams.dt = "test";
                        $routeParams.id = "1";
                        $routeParams.action = "anAction";

                        handlers.handleActionDialog($scope);
                    }));

                    it('should update the scope', function () {
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

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromiseNestedFail(context, 'getObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.dt = "test";
                    $routeParams.id = "1";

                    handlers.handleActionDialog($scope);
                }));

                it('should update the context', function () {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.dialog).toBeUndefined();
                    expect($scope.dialogTemplate).toBeUndefined();
                });
            });
        });

        describe('handleActionResult', function () {
            var getObject;

            describe('if it finds object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testMember = new Spiro.ActionMember({}, testObject);
                var testDetails = new Spiro.ActionRepresentation();
                var testResult = new Spiro.ActionResultRepresentation();

                var actionMember;
                var actionDetails;
                var actionResult;
                var populate;
                var setResult;

                beforeEach(inject(function ($rootScope, $routeParams, repHandlers, context, viewModelFactory, repLoader) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromise(context, 'getObject', testObject);

                    actionMember = spyOn(testObject, "actionMember").andReturn(testMember);
                    actionDetails = spyOn(testMember, "getDetails").andReturn(testDetails);
                    actionResult = spyOn(testDetails, "getInvoke").andReturn(testResult);
                    populate = spyOnPromiseConditional(repLoader, "populate", testDetails, testResult);

                    setResult = spyOn(repHandlers, "setResult");
                }));

                describe('if it is a service', function () {
                    beforeEach(inject(function ($rootScope, $routeParams, handlers) {
                        spyOn(testMember, "extensions").andReturn({ hasParams: false });

                        $routeParams.sid = "testService";
                        $routeParams.action = "anAction";

                        handlers.handleActionResult($scope);
                    }));

                    it('should update the scope', function () {
                        expect(getObject).toHaveBeenCalledWith("testService", undefined);
                        expect(actionMember).toHaveBeenCalledWith("anAction");
                        expect(actionDetails).toHaveBeenCalled();
                        expect(actionResult).toHaveBeenCalled();

                        expect(populate).toHaveBeenCalled();
                        expect(setResult).toHaveBeenCalled();
                    });
                });

                describe('if it has no params', function () {
                    beforeEach(inject(function ($rootScope, $routeParams, handlers) {
                        spyOn(testMember, "extensions").andReturn({ hasParams: false });
                        $routeParams.dt = "test";
                        $routeParams.id = "1";
                        $routeParams.action = "anAction";

                        handlers.handleActionResult($scope);
                    }));

                    it('should update the scope', function () {
                        expect(getObject).toHaveBeenCalledWith("test", "1");
                        expect(actionMember).toHaveBeenCalledWith("anAction");
                        expect(actionDetails).toHaveBeenCalled();
                        expect(actionResult).toHaveBeenCalled();

                        expect(populate).toHaveBeenCalled();
                        expect(setResult).toHaveBeenCalled();
                    });
                });

                describe('if it has params', function () {
                    beforeEach(inject(function ($rootScope, $routeParams, handlers) {
                        spyOn(testMember, "extensions").andReturn({ hasParams: true });
                        $routeParams.dt = "test";
                        $routeParams.id = "1";
                        $routeParams.action = "anAction";

                        handlers.handleActionResult($scope);
                    }));

                    it('should not update the scope', function () {
                        expect(getObject).toHaveBeenCalledWith("test", "1");
                        expect(actionMember).toHaveBeenCalledWith("anAction");
                        expect(actionDetails).wasNotCalled();
                        expect(actionResult).wasNotCalled();

                        expect(setResult).wasNotCalled();
                    });
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromise2NestedFail(context, 'getObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.dt = "test";
                    $routeParams.id = "1";

                    handlers.handleActionResult($scope);
                }));

                it('should update the context', function () {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.dialog).toBeUndefined();
                    expect($scope.dialogTemplate).toBeUndefined();
                });
            });
        });

        describe('handleProperty', function () {
            var getObject;

            describe('if it finds object', function () {
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

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory, repLoader) {
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

                it('should update the scope', function () {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(propertyMember).toHaveBeenCalledWith("aProperty");
                    expect(propertyDetails).toHaveBeenCalled();

                    expect(populate).toHaveBeenCalled();

                    expect(setNestedObject).toHaveBeenCalledWith(testTarget);

                    expect($scope.result).toEqual(testViewModel);
                    expect($scope.nestedTemplate).toEqual("Content/partials/nestedObject.html");
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromise2NestedFail(context, 'getObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.dt = "test";
                    $routeParams.id = "1";

                    handlers.handleProperty($scope);
                }));

                it('should update the context', function () {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.dialog).toBeUndefined();
                    expect($scope.dialogTemplate).toBeUndefined();
                });
            });
        });

        describe('handleResult', function () {
            var getNestedObject;

            describe('if it finds object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testViewModel = { test: testObject };

                var objectViewModel;
                var setNestedObject;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory) {
                    $scope = $rootScope.$new();

                    getNestedObject = spyOnPromise(context, 'getNestedObject', testObject);

                    objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                    setNestedObject = spyOn(context, 'setNestedObject');

                    $routeParams.resultObject = "test-1";

                    handlers.handleResult($scope);
                }));

                it('should update the scope', function () {
                    expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                    expect(objectViewModel).toHaveBeenCalledWith(testObject);
                    expect(setNestedObject).toHaveBeenCalledWith(testObject);

                    expect($scope.result).toEqual(testViewModel);
                    expect($scope.nestedTemplate).toEqual("Content/partials/nestedObject.html");
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getNestedObject = spyOnPromiseFail(context, 'getNestedObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.resultObject = "test-1";

                    handlers.handleResult($scope);
                }));

                it('should update the context', function () {
                    expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.result).toBeUndefined();
                    expect($scope.nestedTemplate).toBeUndefined();
                });
            });
        });

        describe('handleCollectionItem', function () {
            var getNestedObject;

            describe('if it finds object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testViewModel = { test: testObject };

                var objectViewModel;
                var setNestedObject;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory) {
                    $scope = $rootScope.$new();

                    getNestedObject = spyOnPromise(context, 'getNestedObject', testObject);

                    objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                    setNestedObject = spyOn(context, 'setNestedObject');

                    $routeParams.collectionItem = "test/1";

                    handlers.handleCollectionItem($scope);
                }));

                it('should update the scope', function () {
                    expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                    expect(objectViewModel).toHaveBeenCalledWith(testObject);
                    expect(setNestedObject).toHaveBeenCalledWith(testObject);

                    expect($scope.result).toEqual(testViewModel);
                    expect($scope.nestedTemplate).toEqual("Content/partials/nestedObject.html");
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getNestedObject = spyOnPromiseFail(context, 'getNestedObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.collectionItem = "test/1";

                    handlers.handleCollectionItem($scope);
                }));

                it('should update the context', function () {
                    expect(getNestedObject).toHaveBeenCalledWith("test", "1");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.result).toBeUndefined();
                    expect($scope.nestedTemplate).toBeUndefined();
                });
            });
        });

        describe('handleServices', function () {
            var getServices;

            describe('if it finds services', function () {
                var testObject = new Spiro.DomainServicesRepresentation();
                var testViewModel = { test: testObject };

                var servicesViewModel;
                var setObject;
                var setNestedObject;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory) {
                    $scope = $rootScope.$new();

                    getServices = spyOnPromise(context, 'getServices', testObject);

                    servicesViewModel = spyOn(viewModelFactory, 'servicesViewModel').andReturn(testViewModel);
                    setNestedObject = spyOn(context, 'setNestedObject');
                    setObject = spyOn(context, 'setObject');

                    handlers.handleServices($scope);
                }));

                it('should update the scope', function () {
                    expect(getServices).toHaveBeenCalled();
                    expect(servicesViewModel).toHaveBeenCalledWith(testObject);
                    expect(setObject).toHaveBeenCalledWith(null);
                    expect(setNestedObject).toHaveBeenCalledWith(null);

                    expect($scope.services).toEqual(testViewModel);
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getServices = spyOnPromiseFail(context, 'getServices', testObject);
                    setError = spyOn(context, 'setError');

                    handlers.handleServices($scope);
                }));

                it('should update the context', function () {
                    expect(getServices).toHaveBeenCalled();
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.services).toBeUndefined();
                });
            });
        });

        describe('handleService', function () {
            var getObject;

            describe('if it finds service', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testViewModel = { test: testObject };

                var serviceViewModel;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromise(context, 'getObject', testObject);

                    serviceViewModel = spyOn(viewModelFactory, 'serviceViewModel').andReturn(testViewModel);

                    $routeParams.sid = "test";

                    handlers.handleService($scope);
                }));

                it('should update the scope', function () {
                    expect(getObject).toHaveBeenCalledWith("test");
                    expect(serviceViewModel).toHaveBeenCalledWith(testObject);

                    expect($scope.object).toEqual(testViewModel);
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromiseFail(context, 'getObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.sid = "test";

                    handlers.handleService($scope);
                }));

                it('should update the context', function () {
                    expect(getObject).toHaveBeenCalledWith("test");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.object).toBeUndefined();
                });
            });
        });

        describe('handleObject', function () {
            var getObject;

            describe('if it finds object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testViewModel = { test: testObject };

                var objectViewModel;
                var setNestedObject;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context, viewModelFactory) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromise(context, 'getObject', testObject);

                    objectViewModel = spyOn(viewModelFactory, 'domainObjectViewModel').andReturn(testViewModel);
                    setNestedObject = spyOn(context, 'setNestedObject');

                    $routeParams.dt = "test";
                    $routeParams.id = "1";
                }));

                describe('not in edit mode', function () {
                    beforeEach(inject(function ($rootScope, $routeParams, handlers) {
                        handlers.handleObject($scope);
                    }));

                    it('should update the scope', function () {
                        expect(getObject).toHaveBeenCalledWith("test", "1");
                        expect(objectViewModel).toHaveBeenCalledWith(testObject);
                        expect(setNestedObject).toHaveBeenCalledWith(null);

                        expect($scope.object).toEqual(testViewModel);
                        expect($scope.actionTemplate).toEqual("Content/partials/actions.html");
                        expect($scope.propertiesTemplate).toEqual("Content/partials/viewProperties.html");
                    });
                });

                describe('in edit mode', function () {
                    var propertyMem = new Spiro.PropertyMember({}, testObject);
                    var propertyRep = new Spiro.PropertyRepresentation();

                    var populate;

                    beforeEach(inject(function ($rootScope, $q, $routeParams, repLoader, handlers) {
                        spyOn(testObject, 'propertyMembers').andReturn([propertyMem]);
                        spyOn(propertyMem, 'getDetails').andReturn(propertyRep);

                        spyOnPromise($q, 'all', [propertyRep]);

                        populate = spyOnPromise(repLoader, "populate", propertyRep);

                        $routeParams.editMode = "test";
                        handlers.handleEditObject($scope);
                    }));

                    it('should update the scope', function () {
                        expect(getObject).toHaveBeenCalledWith("test", "1");
                        expect(objectViewModel).toHaveBeenCalledWith(testObject, [propertyRep], jasmine.any(Function));
                        expect(setNestedObject).toHaveBeenCalledWith(null);

                        expect($scope.object).toEqual(testViewModel);
                        expect($scope.actionTemplate).toEqual("");
                        expect($scope.propertiesTemplate).toEqual("Content/partials/editProperties.html");
                    });
                });
            });

            describe('if it has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setError;

                beforeEach(inject(function ($rootScope, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    getObject = spyOnPromiseFail(context, 'getObject', testObject);
                    setError = spyOn(context, 'setError');

                    $routeParams.dt = "test";
                    $routeParams.id = "1";

                    handlers.handleObject($scope);
                }));

                it('should update the context', function () {
                    expect(getObject).toHaveBeenCalledWith("test", "1");
                    expect(setError).toHaveBeenCalledWith(testObject);

                    expect($scope.object).toBeUndefined();
                    expect($scope.actionTemplate).toBeUndefined();
                    expect($scope.propertiesTemplate).toBeUndefined();
                });
            });
        });

        describe('handleError', function () {
            beforeEach(inject(function ($rootScope, handlers, context) {
                $scope = $rootScope.$new();

                spyOn(context, 'getError').andReturn(new Spiro.ErrorRepresentation({ message: "", stacktrace: [] }));

                handlers.handleError($scope);
            }));

            it('should set a error data', function () {
                expect($scope.error).toBeDefined();
                expect($scope.errorTemplate).toEqual("Content/partials/error.html");
            });
        });

        describe('handleAppBar', function () {
            function expectAppBarData() {
                expect($scope.appBar).toBeDefined();
                expect($scope.appBar.goHome).toEqual("#/");
                expect($scope.appBar.template).toEqual("Content/partials/appbar.html");
                expect($scope.appBar.goBack).toBeDefined();
                expect($scope.appBar.goForward).toBeDefined();
            }

            describe('handleAppBar when not viewing an  object', function () {
                beforeEach(inject(function ($rootScope, handlers) {
                    $scope = $rootScope.$new();

                    handlers.handleAppBar($scope);
                }));

                it('should set appBar data', function () {
                    expectAppBarData();
                });

                it('should disable edit button', function () {
                    expect($scope.appBar.hideEdit).toEqual(true);
                    expect($scope.appBar.doEdit).toBeUndefined();
                });
            });

            describe('handleAppBar when viewing an editable object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testMember = new Spiro.PropertyMember({}, testObject);

                beforeEach(inject(function ($rootScope, $location, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    $routeParams.dt = "test";
                    $routeParams.id = "1";

                    spyOnPromise(context, 'getObject', testObject);
                    spyOn(testObject, 'propertyMembers').andReturn([testMember]);

                    spyOn($location, 'path').andReturn("aPath");

                    handlers.handleAppBar($scope);
                }));

                it('should set appBar data', function () {
                    expectAppBarData();
                });

                it('should enable edit button', function () {
                    expect($scope.appBar.hideEdit).toBe(false);
                    expect($scope.appBar.doEdit).toEqual("?editMode=true");
                });
            });

            describe('handleAppBar when viewing a non editable object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testMember = new Spiro.PropertyMember({}, testObject);

                beforeEach(inject(function ($rootScope, $location, $routeParams, handlers, context) {
                    $scope = $rootScope.$new();

                    $routeParams.dt = "test";
                    $routeParams.id = "1";

                    spyOnPromise(context, 'getObject', testObject);
                    spyOn(testObject, 'propertyMembers').andReturn([testMember]);
                    spyOn(testMember, 'disabledReason').andReturn("disabled");

                    spyOn($location, 'path').andReturn("aPath");

                    handlers.handleAppBar($scope);
                }));

                it('should set appBar data', function () {
                    expectAppBarData();
                });

                it('should disable edit button', function () {
                    expect($scope.appBar.hideEdit).toBe(true);
                });
            });
        });

        describe('setResult helper', function () {
            var testActionResult = new Spiro.ActionResultRepresentation();
            var testViewModel = new Spiro.Angular.DialogViewModel();
            var location;

            beforeEach(inject(function ($location) {
                location = $location;
            }));

            describe('result is null', function () {
                var testResult = new Spiro.Result(null, 'object');

                beforeEach(inject(function (repHandlers) {
                    spyOn(testActionResult, 'result').andReturn(testResult);
                    spyOn(testActionResult, 'resultType').andReturn("void");
                    repHandlers.setResult(testActionResult, testViewModel);
                }));

                it('should not set view model error', function () {
                    expect(testViewModel.message).toBeUndefined();
                    expect(location.search()).toEqual({});
                });
            });

            describe('result is object', function () {
                var testObject = new Spiro.DomainObjectRepresentation();
                var testResult = new Spiro.Result({}, 'object');
                var setNestedObject;

                beforeEach(inject(function ($routeParams, context) {
                    spyOn(testActionResult, 'result').andReturn(testResult);
                    spyOn(testActionResult, 'resultType').andReturn('object');
                    spyOn(testResult, 'object').andReturn(testObject);
                    setNestedObject = spyOn(context, 'setNestedObject');

                    spyOn(testObject, 'domainType').andReturn("test");
                    spyOn(testObject, 'instanceId').andReturn("1");
                    spyOn(testObject, 'persistLink').andReturn(null);

                    $routeParams.action = "anAction";
                }));

                describe('with show flag', function () {
                    beforeEach(inject(function (repHandlers) {
                        testViewModel.parameters = [];
                        testViewModel.show = true;

                        repHandlers.setResult(testActionResult, testViewModel);
                    }));

                    it('should set nested object and search', function () {
                        expect(setNestedObject).toHaveBeenCalledWith(testObject);
                        expect(location.search()).toEqual({ resultObject: 'test-1', action: 'anAction' });
                    });
                });

                describe('without show flag', function () {
                    beforeEach(inject(function (repHandlers) {
                        repHandlers.setResult(testActionResult);
                    }));

                    it('should set nested object and search', function () {
                        expect(setNestedObject).toHaveBeenCalledWith(testObject);
                        expect(location.search()).toEqual({ resultObject: 'test-1' });
                    });
                });
            });

            describe('result is list', function () {
                var testList = new Spiro.ListRepresentation();
                var testResult = new Spiro.Result([], 'list');
                var testNullResult = new Spiro.Result(null, 'list');
                var setCollection;

                beforeEach(inject(function ($routeParams, context) {
                    spyOn(testActionResult, 'resultType').andReturn('list');
                    spyOn(testResult, 'list').andReturn(testList);
                    setCollection = spyOn(context, 'setCollection');

                    $routeParams.action = "anAction";
                }));

                describe('with show flag', function () {
                    var testParameters = [new Spiro.Angular.ParameterViewModel(), new Spiro.Angular.ParameterViewModel()];
                    testParameters[0].type = "scalar";
                    testParameters[0].value = "1";
                    testParameters[1].type = "scalar";
                    testParameters[1].value = "2";

                    beforeEach(inject(function (repHandlers) {
                        spyOn(testActionResult, 'result').andReturn(testResult);
                        testViewModel.parameters = testParameters;

                        repHandlers.setResult(testActionResult, testViewModel);
                    }));

                    it('should set collection and search', function () {
                        expect(setCollection).toHaveBeenCalledWith(testList);
                        expect(location.search()).toEqual({ resultCollection: 'anAction/1/2', action: 'anAction/1/2' });
                    });
                });

                describe('without show flag', function () {
                    beforeEach(inject(function (repHandlers) {
                        spyOn(testActionResult, 'result').andReturn(testResult);
                        repHandlers.setResult(testActionResult);
                    }));

                    it('should set collection and search', function () {
                        expect(setCollection).toHaveBeenCalledWith(testList);
                        expect(location.search()).toEqual({ resultCollection: 'anAction' });
                    });
                });

                describe('result is null', function () {
                    beforeEach(inject(function (repHandlers) {
                        spyOn(testActionResult, 'result').andReturn(testNullResult);
                        repHandlers.setResult(testActionResult, testViewModel);
                    }));

                    it('should set view model error', function () {
                        expect(testViewModel.message).toBe("no result found");
                        expect(location.search()).toEqual({});
                    });
                });
            });
        });

        describe('invokeAction helper', function () {
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

            beforeEach(inject(function ($rootScope) {
                spyOn(testAction, 'getInvoke').andReturn(testActionResult);

                clearMessages = spyOn(testViewModel, 'clearMessages');
                setParameter = spyOn(testActionResult, 'setParameter');
                $scope = $rootScope.$new();
            }));

            describe('invoke is successful', function () {
                var setResult;

                beforeEach(inject(function (repHandlers, repLoader) {
                    populate = spyOnPromise(repLoader, 'populate', testActionResult);
                    setResult = spyOn(repHandlers, 'setResult');
                    testViewModel.parameters = testParameters;
                    testViewModel.show = true;

                    repHandlers.invokeAction($scope, testAction, testViewModel);
                }));

                it('should set result', function () {
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

            describe('if invoke has an error', function () {
                var testObject = new Spiro.ErrorRepresentation();
                var setInvokeUpdateError;

                beforeEach(inject(function ($rootScope, $routeParams, repHandlers, repLoader, context) {
                    populate = spyOnPromiseFail(repLoader, 'populate', testObject);
                    setInvokeUpdateError = spyOn(repHandlers, 'setInvokeUpdateError');
                    repHandlers.invokeAction($scope, testAction, testViewModel);
                }));

                it('should set the error', function () {
                    expect(setInvokeUpdateError).toHaveBeenCalledWith($scope, testObject, testParameters, testViewModel);
                });
            });
        });

        describe('updateObject helper', function () {
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

            beforeEach(inject(function ($rootScope, repLoader) {
                testUpdate.setProperty = function () {
                };

                spyOn(testObject, 'getUpdateMap').andReturn(testUpdate);
                setProperty = spyOn(testUpdate, 'setProperty');
                testViewModel.properties = testProperties;

                spyOn(testObject, 'get').andReturn(testRawLinks);
                set = spyOn(testUpdatedObject, 'set');

                $scope = $rootScope.$new();
            }));

            describe('update is successful', function () {
                var setObject;

                var location;
                var cacheFactory;
                var testCache = {};
                var remove;

                beforeEach(inject(function ($location, $cacheFactory, repHandlers, repLoader, context) {
                    setObject = spyOn(context, 'setObject');

                    location = $location;
                    cacheFactory = $cacheFactory;
                    testCache.remove = function (url) {
                    };

                    populate = spyOnPromise(repLoader, 'populate', testUpdatedObject);

                    spyOn(cacheFactory, 'get').andReturn(testCache);
                    remove = spyOn(testCache, 'remove');

                    testUpdatedObject.hateoasUrl = "testUrl";

                    repHandlers.updateObject($scope, testObject, testViewModel);
                }));

                it('should set result', function () {
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

            describe('if update has an error', function () {
                var testError = new Spiro.ErrorRepresentation();
                var setInvokeUpdateError;
                var editableProperties = _.filter(testProperties, function (tp) {
                    return tp.isEditable;
                });

                beforeEach(inject(function ($rootScope, $routeParams, repHandlers, repLoader) {
                    populate = spyOnPromiseFail(repLoader, 'populate', testError);
                    setInvokeUpdateError = spyOn(repHandlers, 'setInvokeUpdateError');
                    repHandlers.updateObject($scope, testObject, testViewModel);
                }));

                it('should set the error', function () {
                    expect(setInvokeUpdateError).toHaveBeenCalledWith($scope, testError, editableProperties, testViewModel);
                });
            });
        });

        describe('setInvokeUpdateError helper', function () {
            var testViewModel = new Spiro.Angular.MessageViewModel();
            testViewModel.message = "";

            beforeEach(inject(function ($rootScope) {
                $scope = $rootScope.$new();
            }));

            describe('if error is errorMap', function () {
                var error = { "one": { "value": "1", "invalidReason": "a reason" }, "two": { "value": "2" }, "x-ro-invalid-reason": "another reason" };

                var errorMap = new Spiro.ErrorMap(error, "status", "a warning message");

                var vm1 = new Spiro.Angular.ValueViewModel();
                var vm2 = new Spiro.Angular.ValueViewModel();

                vm1.id = "one";
                vm2.id = "two";

                var vms = [vm1, vm2];

                beforeEach(inject(function (repHandlers) {
                    repHandlers.setInvokeUpdateError($scope, errorMap, vms, testViewModel);
                }));

                it('should set the parameters and error', function () {
                    expect(vms[0].value).toBe("1");
                    expect(vms[0].message).toBe("a reason");
                    expect(vms[1].value).toBe("2");
                    expect(vms[1].message).toBeUndefined();

                    expect(testViewModel.message).toBe("another reason");
                });
            });

            // todo fix
            //describe('if error is errorRep', function () {
            //    var testError = new Spiro.ErrorRepresentation();
            //    var testErrorViewModel = new Spiro.Angular.ErrorViewModel();
            //    var errorViewModel;
            //    beforeEach(inject(function (repHandlers: Spiro.Angular.IRepHandlers, viewModelFactory: Spiro.Angular.IViewModelFactory) {
            //        errorViewModel = spyOn(viewModelFactory, 'errorViewModel').andReturn(testErrorViewModel);
            //        repHandlers.setInvokeUpdateError($scope, testError, [], testViewModel);
            //    }));
            //    it('should set the scope ', function () {
            //        expect(errorViewModel).toHaveBeenCalledWith(testError);
            //        expect($scope.error).toBe(testErrorViewModel);
            //        expect($scope.dialogTemplate).toBe("Content/partials/error.html");
            //    });
            //});
            describe('if error is string', function () {
                var errorMessage = 'an error message';

                beforeEach(inject(function ($rootScope, $routeParams, repHandlers) {
                    repHandlers.setInvokeUpdateError($scope, errorMessage, [], testViewModel);
                }));

                it('should set the scope ', function () {
                    expect(testViewModel.message).toBe(errorMessage);
                });
            });
        });
    });

    describe('context Service', function () {
        describe('getHome', function () {
            var testHome = new Spiro.HomePageRepresentation();
            var localContext;

            var result;

            beforeEach(inject(function ($rootScope, $routeParams, context, repLoader) {
                spyOnPromise(repLoader, 'populate', testHome);
                localContext = context;

                runs(function () {
                    localContext.getHome().then(function (home) {
                        result = home;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function () {
                    return !!result;
                }, "result not set", 1000);
            }));

            describe('when currentHome is set', function () {
                beforeEach(inject(function ($rootScope) {
                    // currentHome set already
                    result = null;

                    runs(function () {
                        localContext.getHome().then(function (home) {
                            result = home;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns home page representation', function () {
                    expect(result).toBe(testHome);
                });
            });

            describe('when currentHome is not set', function () {
                it('returns home page representation', function () {
                    expect(result).toBe(testHome);
                });
            });
        });

        describe('getServices', function () {
            var testServices = new Spiro.DomainServicesRepresentation();
            var testHome = new Spiro.HomePageRepresentation();
            var localContext;

            var result;

            beforeEach(inject(function ($rootScope, $routeParams, context, repLoader) {
                spyOnPromise(repLoader, 'populate', testServices);
                spyOnPromise(context, 'getHome', testHome);

                spyOn(testHome, 'getDomainServices').andReturn(testServices);

                localContext = context;

                runs(function () {
                    localContext.getServices().then(function (services) {
                        result = services;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function () {
                    return !!result;
                }, "result not set", 1000);
            }));

            describe('when currentServices is set', function () {
                beforeEach(inject(function ($rootScope) {
                    // currentServices set already
                    result = null;

                    runs(function () {
                        localContext.getServices().then(function (services) {
                            result = services;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns home page representation', function () {
                    expect(result).toBe(testServices);
                });
            });

            describe('when currentServices is not set', function () {
                it('returns services representation', function () {
                    expect(result).toBe(testServices);
                });
            });
        });

        describe('getObject', function () {
            var testObject = new Spiro.DomainObjectRepresentation();

            var localContext;
            var result;

            var getDomainObject;
            var getService;

            beforeEach(inject(function ($rootScope, $routeParams, context, repLoader) {
                spyOnPromise(repLoader, 'populate', testObject);
                getDomainObject = spyOnPromise(context, 'getDomainObject', testObject);
                getService = spyOnPromise(context, 'getService', testObject);

                spyOn(testObject, 'domainType').andReturn("test");
                spyOn(testObject, 'instanceId').andReturn("1");
                spyOn(testObject, 'serviceId').andReturn(undefined);

                localContext = context;
            }));

            describe('when currentObject is set', function () {
                beforeEach(inject(function ($rootScope) {
                    localContext.setObject(testObject);

                    runs(function () {
                        localContext.getObject("test", "1").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns object representation', function () {
                    expect(getDomainObject).wasNotCalled();
                    expect(getService).wasNotCalled();
                    expect(result).toBe(testObject);
                });
            });

            describe('when currentObject is set but not same', function () {
                beforeEach(inject(function ($rootScope) {
                    localContext.setObject(testObject);

                    runs(function () {
                        localContext.getObject("test2", "2").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns object representation', function () {
                    expect(getDomainObject).toHaveBeenCalledWith("test2", "2");
                    expect(getService).wasNotCalled();
                    expect(result).toBe(testObject);
                });
            });

            describe('when currentObject is not set', function () {
                beforeEach(inject(function ($rootScope) {
                    runs(function () {
                        localContext.getObject("test", "1").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns object representation', function () {
                    expect(getDomainObject).toHaveBeenCalledWith("test", "1");
                    expect(getService).wasNotCalled();
                    expect(result).toBe(testObject);
                });
            });
        });

        describe('getNestedObject', function () {
            var testObject = new Spiro.DomainObjectRepresentation();

            var localContext;
            var result;

            var populate;

            beforeEach(inject(function ($rootScope, $routeParams, context) {
                spyOn(testObject, 'domainType').andReturn("test");
                spyOn(testObject, 'instanceId').andReturn("1");
                spyOn(testObject, 'serviceId').andReturn(undefined);

                localContext = context;
            }));

            describe('when nestedObject is set', function () {
                beforeEach(inject(function ($rootScope, repLoader) {
                    populate = spyOnPromise(repLoader, 'populate', testObject);

                    localContext.setNestedObject(testObject);

                    runs(function () {
                        localContext.getNestedObject("test", "1").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns object representation', function () {
                    expect(populate).wasNotCalled();
                    expect(result).toBe(testObject);
                });
            });

            describe('when nestedObject is set but not same', function () {
                var testResult = new Spiro.DomainObjectRepresentation();

                beforeEach(inject(function ($rootScope, repLoader) {
                    testResult.hateoasUrl = "objects/test2/2";

                    populate = spyOnPromise(repLoader, 'populate', testResult);

                    localContext.setNestedObject(testObject);

                    runs(function () {
                        localContext.getNestedObject("test2", "2").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns object representation', function () {
                    expect(populate).toHaveBeenCalled();
                    expect(result).toBe(testResult);
                });
            });

            describe('when nestedObject is not set', function () {
                var testResult = new Spiro.DomainObjectRepresentation();

                beforeEach(inject(function ($rootScope, repLoader) {
                    testResult.hateoasUrl = "objects/test/1";

                    populate = spyOnPromise(repLoader, 'populate', testResult);

                    runs(function () {
                        localContext.getNestedObject("test", "1").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns object representation', function () {
                    expect(populate).toHaveBeenCalled();
                    expect(result).toBe(testResult);
                });
            });
        });

        describe('getCollection', function () {
            var testObject = new Spiro.ListRepresentation();

            var localContext;
            var result;

            var populate;

            beforeEach(inject(function ($rootScope, $routeParams, context) {
                localContext = context;
            }));

            describe('when collection is set', function () {
                beforeEach(inject(function ($rootScope) {
                    localContext.setCollection(testObject);

                    runs(function () {
                        localContext.getCollection().then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns collection representation', function () {
                    expect(result).toBe(testObject);
                });
            });

            describe('when collection is not set', function () {
                beforeEach(inject(function ($rootScope) {
                    var getCollectionRun = false;

                    runs(function () {
                        localContext.getCollection().then(function (object) {
                            result = object;
                            getCollectionRun = true;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return getCollectionRun;
                    }, "result not set", 1000);
                }));

                it('returns object representation', function () {
                    expect(result).toBeNull();
                });
            });
        });

        describe('getService', function () {
            var testObject = new Spiro.DomainObjectRepresentation();

            var localContext;
            var result;

            var getDomainObject;
            var getService;

            beforeEach(inject(function ($rootScope, $routeParams, context, repLoader) {
                spyOnPromise(repLoader, 'populate', testObject);
                getDomainObject = spyOnPromise(context, 'getDomainObject', testObject);
                getService = spyOnPromise(context, 'getService', testObject);

                spyOn(testObject, 'domainType').andReturn(undefined);
                spyOn(testObject, 'instanceId').andReturn(undefined);
                spyOn(testObject, 'serviceId').andReturn("test");

                localContext = context;
            }));

            describe('when currentObject is set', function () {
                beforeEach(inject(function ($rootScope) {
                    localContext.setObject(testObject);

                    runs(function () {
                        localContext.getObject("test").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns service representation', function () {
                    expect(getDomainObject).wasNotCalled();
                    expect(getService).wasNotCalled();
                    expect(result).toBe(testObject);
                });
            });

            describe('when currentObject is set but is not same', function () {
                beforeEach(inject(function ($rootScope) {
                    localContext.setObject(testObject);

                    runs(function () {
                        localContext.getObject("test2").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns service representation', function () {
                    expect(getDomainObject).wasNotCalled();
                    expect(getService).toHaveBeenCalledWith("test2");
                    expect(result).toBe(testObject);
                });
            });

            describe('when currentObject is not set', function () {
                beforeEach(inject(function ($rootScope) {
                    runs(function () {
                        localContext.getObject("test").then(function (object) {
                            result = object;
                        });
                        $rootScope.$apply();
                    });

                    waitsFor(function () {
                        return !!result;
                    }, "result not set", 1000);
                }));

                it('returns service representation', function () {
                    expect(getDomainObject).wasNotCalled();
                    expect(getService).toHaveBeenCalledWith("test");
                    expect(result).toBe(testObject);
                });
            });
        });
    });
});
//# sourceMappingURL=services.js.map
