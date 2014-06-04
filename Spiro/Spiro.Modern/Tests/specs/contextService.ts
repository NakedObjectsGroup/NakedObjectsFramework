/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.angular.services.handlers.ts" />
/// <reference path="helpers.ts" />
describe('context Service', function() {

    var $scope;

    beforeEach(module('app'));

    describe('getHome', function() {

        var testHome = new Spiro.HomePageRepresentation();
        var localContext;

        var result;

        beforeEach(inject(function($rootScope, $routeParams, context: Spiro.Angular.IContext, repLoader: Spiro.Angular.IRepLoader) {
            spyOnPromise(repLoader, 'populate', testHome);
            localContext = context;

            runs(function() {
                localContext.getHome().then(function(home) {
                    result = home;
                });
                $rootScope.$apply();
            });

            waitsFor(function() {
                return !!result;
            }, "result not set", 1000);

        }));

        describe('when currentHome is set', function() {

            beforeEach(inject(function($rootScope) {
                // currentHome set already 

                result = null;

                runs(function() {
                    localContext.getHome().then(function(home) {
                        result = home;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns home page representation', function() {
                expect(result).toBe(testHome);
            });
        });

        describe('when currentHome is not set', function() {
            it('returns home page representation', function() {
                expect(result).toBe(testHome);
            });
        });
    });

    describe('getServices', function() {

        var testServices = new Spiro.DomainServicesRepresentation();
        var testHome = new Spiro.HomePageRepresentation();
        var localContext;

        var result;

        beforeEach(inject(function($rootScope, $routeParams, context: Spiro.Angular.IContext, repLoader: Spiro.Angular.IRepLoader) {
            spyOnPromise(repLoader, 'populate', testServices);
            spyOnPromise(context, 'getHome', testHome);

            spyOn(testHome, 'getDomainServices').andReturn(testServices);

            localContext = context;

            runs(function() {
                localContext.getServices().then(function(services) {
                    result = services;
                });
                $rootScope.$apply();
            });

            waitsFor(function() {
                return !!result;
            }, "result not set", 1000);

        }));

        describe('when currentServices is set', function() {

            beforeEach(inject(function($rootScope) {
                // currentServices set already 

                result = null;

                runs(function() {
                    localContext.getServices().then(function(services) {
                        result = services;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns home page representation', function() {
                expect(result).toBe(testServices);
            });
        });

        describe('when currentServices is not set', function() {
            it('returns services representation', function() {
                expect(result).toBe(testServices);
            });
        });

    });

    describe('getObject', function() {

        var testObject = new Spiro.DomainObjectRepresentation();

        var localContext;
        var result;

        var getDomainObject;
        var getService;

        beforeEach(inject(function($rootScope, $routeParams, context: Spiro.Angular.IContext, repLoader: Spiro.Angular.IRepLoader) {
            spyOnPromise(repLoader, 'populate', testObject);
            getDomainObject = spyOnPromise(context, 'getDomainObject', testObject);
            getService = spyOnPromise(context, 'getService', testObject);

            spyOn(testObject, 'domainType').andReturn("test");
            spyOn(testObject, 'instanceId').andReturn("1");
            spyOn(testObject, 'serviceId').andReturn(undefined);

            localContext = context;
        }));

        describe('when currentObject is set', function() {


            beforeEach(inject(function($rootScope) {

                localContext.setObject(testObject);

                runs(function() {
                    localContext.getObject("test", "1").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns object representation', function() {
                expect(getDomainObject).wasNotCalled();
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

        describe('when currentObject is set but not same', function() {


            beforeEach(inject(function($rootScope) {

                localContext.setObject(testObject);

                runs(function() {
                    localContext.getObject("test2", "2").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns object representation', function() {
                expect(getDomainObject).toHaveBeenCalledWith("test2", "2");
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });


        describe('when currentObject is not set', function() {

            beforeEach(inject(function($rootScope) {

                runs(function() {
                    localContext.getObject("test", "1").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns object representation', function() {
                expect(getDomainObject).toHaveBeenCalledWith("test", "1");
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

    });

    describe('getNestedObject', function() {

        var testObject = new Spiro.DomainObjectRepresentation();

        var localContext;
        var result;

        var populate;

        beforeEach(inject(function($rootScope, $routeParams, context: Spiro.Angular.IContext) {


            spyOn(testObject, 'domainType').andReturn("test");
            spyOn(testObject, 'instanceId').andReturn("1");
            spyOn(testObject, 'serviceId').andReturn(undefined);

            localContext = context;
        }));

        describe('when nestedObject is set', function() {

            beforeEach(inject(function($rootScope, repLoader: Spiro.Angular.IRepLoader) {

                populate = spyOnPromise(repLoader, 'populate', testObject);

                localContext.setNestedObject(testObject);

                runs(function() {
                    localContext.getNestedObject("test", "1").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns object representation', function() {
                expect(populate).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

        describe('when nestedObject is set but not same', function() {

            var testResult = new Spiro.DomainObjectRepresentation();

            beforeEach(inject(function($rootScope, repLoader: Spiro.Angular.IRepLoader) {

                testResult.hateoasUrl = "objects/test2/2";

                populate = spyOnPromise(repLoader, 'populate', testResult);

                localContext.setNestedObject(testObject);

                runs(function() {
                    localContext.getNestedObject("test2", "2").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));

            it('returns object representation', function() {
                expect(populate).toHaveBeenCalled();
                expect(result).toBe(testResult);
            });
        });

        describe('when nestedObject is not set', function() {

            var testResult = new Spiro.DomainObjectRepresentation();

            beforeEach(inject(function($rootScope, repLoader: Spiro.Angular.IRepLoader) {

                testResult.hateoasUrl = "objects/test/1";

                populate = spyOnPromise(repLoader, 'populate', testResult);

                runs(function() {
                    localContext.getNestedObject("test", "1").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns object representation', function() {
                expect(populate).toHaveBeenCalled();
                expect(result).toBe(testResult);
            });
        });

    });

    describe('getCollection', function() {

        var testObject = new Spiro.ListRepresentation();

        var localContext;
        var result;

        var populate;

        beforeEach(inject(function($rootScope, $routeParams, context: Spiro.Angular.IContext) {


            localContext = context;
        }));

        describe('when collection is set', function() {

            beforeEach(inject(function($rootScope) {

                localContext.setCollection(testObject);

                runs(function() {
                    localContext.getCollection().then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns collection representation', function() {

                expect(result).toBe(testObject);
            });
        });

        describe('when collection is not set', function() {


            beforeEach(inject(function($rootScope) {

                var getCollectionRun = false;

                runs(function() {
                    localContext.getCollection().then(function(object) {
                        result = object;
                        getCollectionRun = true;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return getCollectionRun;
                }, "result not set", 1000);
            }));


            it('returns object representation', function() {

                expect(result).toBeNull();
            });
        });

    });


    describe('getService', function() {

        var testObject = new Spiro.DomainObjectRepresentation();

        var localContext;
        var result;

        var getDomainObject;
        var getService;

        beforeEach(inject(function($rootScope, $routeParams, context: Spiro.Angular.IContext, repLoader: Spiro.Angular.IRepLoader) {
            spyOnPromise(repLoader, 'populate', testObject);
            getDomainObject = spyOnPromise(context, 'getDomainObject', testObject);
            getService = spyOnPromise(context, 'getService', testObject);

            spyOn(testObject, 'domainType').andReturn(undefined);
            spyOn(testObject, 'instanceId').andReturn(undefined);
            spyOn(testObject, 'serviceId').andReturn("test");

            localContext = context;
        }));

        describe('when currentObject is set', function() {

            beforeEach(inject(function($rootScope) {

                localContext.setObject(testObject);

                runs(function() {
                    localContext.getObject("test").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns service representation', function() {
                expect(getDomainObject).wasNotCalled();
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

        describe('when currentObject is set but is not same', function() {

            beforeEach(inject(function($rootScope) {

                localContext.setObject(testObject);

                runs(function() {
                    localContext.getObject("test2").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns service representation', function() {
                expect(getDomainObject).wasNotCalled();
                expect(getService).toHaveBeenCalledWith("test2");
                expect(result).toBe(testObject);
            });
        });

        describe('when currentObject is not set', function() {

            beforeEach(inject(function($rootScope) {

                runs(function() {
                    localContext.getObject("test").then(function(object) {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(function() {
                    return !!result;
                }, "result not set", 1000);
            }));


            it('returns service representation', function() {
                expect(getDomainObject).wasNotCalled();
                expect(getService).toHaveBeenCalledWith("test");
                expect(result).toBe(testObject);
            });
        });

    });

});