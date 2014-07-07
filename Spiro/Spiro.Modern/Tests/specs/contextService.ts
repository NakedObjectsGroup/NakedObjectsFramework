/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.modern.services.handlers.ts" />
/// <reference path="../../Scripts/spiro.modern.viewmodels.ts" />
/// <reference path="helpers.ts" />

describe('context Service', () => {

    beforeEach(module('app'));

    describe('getHome', () => {

        var testHome = new Spiro.HomePageRepresentation();
        var localContext;

        var result;

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext, repLoader: Spiro.Angular.IRepLoader) => {
            spyOnPromise(repLoader, 'populate', testHome);
            localContext = context;

            runs(() => {
                localContext.getHome().then(home => {
                    result = home;
                });
                $rootScope.$apply();
            });

            waitsFor(() => !!result, "result not set", 1000);

        }));

        describe('when currentHome is set', () => {

            beforeEach(inject($rootScope => {
                // currentHome set already 

                result = null;

                runs(() => {
                    localContext.getHome().then(home => {
                        result = home;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns home page representation', () => {
                expect(result).toBe(testHome);
            });
        });

        describe('when currentHome is not set', () => {
            it('returns home page representation', () => {
                expect(result).toBe(testHome);
            });
        });
    });

    describe('getServices', () => {

        var testServices = new Spiro.DomainServicesRepresentation();
        var testHome = new Spiro.HomePageRepresentation();
        var localContext;

        var result;

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext, repLoader: Spiro.Angular.IRepLoader) => {
            spyOnPromise(repLoader, 'populate', testServices);
            spyOnPromise(context, 'getHome', testHome);

            spyOn(testHome, 'getDomainServices').andReturn(testServices);

            localContext = context;

            runs(() => {
                localContext.getServices().then(services => {
                    result = services;
                });
                $rootScope.$apply();
            });

            waitsFor(() => !!result, "result not set", 1000);

        }));

        describe('when currentServices is set', () => {

            beforeEach(inject($rootScope => {
                // currentServices set already 

                result = null;

                runs(() => {
                    localContext.getServices().then(services => {
                        result = services;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns services representation', () => {
                expect(result).toBe(testServices);
            });
        });

        describe('when currentServices is not set', () => {
            it('returns services representation', () => {
                expect(result).toBe(testServices);
            });
        });

    });

    describe('getObject', () => {

        var testObject = new Spiro.DomainObjectRepresentation();

        var localContext;
        var result;

        var getDomainObject;
        var getService;

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext, repLoader: Spiro.Angular.IRepLoader) => {
            spyOnPromise(repLoader, 'populate', testObject);
            getDomainObject = spyOnPromise(context, 'getDomainObject', testObject);
            getService = spyOnPromise(context, 'getService', testObject);

            spyOn(testObject, 'domainType').andReturn("test");
            spyOn(testObject, 'instanceId').andReturn("1");
            spyOn(testObject, 'serviceId').andReturn(undefined);

            localContext = context;
        }));

        describe('when currentObject is set', () => {


            beforeEach(inject($rootScope => {

                localContext.setObject(testObject);

                runs(() => {
                    localContext.getObject("test", "1").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns object representation', () => {
                expect(getDomainObject).wasNotCalled();
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

        describe('when currentObject is set but not same', () => {


            beforeEach(inject($rootScope => {

                localContext.setObject(testObject);

                runs(() => {
                    localContext.getObject("test2", "2").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns object representation', () => {
                expect(getDomainObject).toHaveBeenCalledWith("test2", "2");
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });


        describe('when currentObject is not set', () => {

            beforeEach(inject($rootScope => {

                runs(() => {
                    localContext.getObject("test", "1").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns object representation', () => {
                expect(getDomainObject).toHaveBeenCalledWith("test", "1");
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

    });

    describe('getNestedObject', () => {

        var testObject = new Spiro.DomainObjectRepresentation();

        var localContext;
        var result;

        var populate;

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext) => {


            spyOn(testObject, 'domainType').andReturn("test");
            spyOn(testObject, 'instanceId').andReturn("1");
            spyOn(testObject, 'serviceId').andReturn(undefined);

            localContext = context;
        }));

        describe('when nestedObject is set', () => {

            beforeEach(inject(($rootScope, repLoader: Spiro.Angular.IRepLoader) => {

                populate = spyOnPromise(repLoader, 'populate', testObject);

                localContext.setNestedObject(testObject);

                runs(() => {
                    localContext.getNestedObject("test", "1").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns object representation', () => {
                expect(populate).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

        describe('when nestedObject is set but not same', () => {

            var testResult = new Spiro.DomainObjectRepresentation();

            beforeEach(inject(($rootScope, repLoader: Spiro.Angular.IRepLoader) => {

                testResult.hateoasUrl = "objects/test2/2";

                populate = spyOnPromise(repLoader, 'populate', testResult);

                localContext.setNestedObject(testObject);

                runs(() => {
                    localContext.getNestedObject("test2", "2").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));

            it('returns object representation', () => {
                expect(populate).toHaveBeenCalled();
                expect(result).toBe(testResult);
            });
        });

        describe('when nestedObject is not set', () => {

            var testResult = new Spiro.DomainObjectRepresentation();

            beforeEach(inject(($rootScope, repLoader: Spiro.Angular.IRepLoader) => {

                testResult.hateoasUrl = "objects/test/1";

                populate = spyOnPromise(repLoader, 'populate', testResult);

                runs(() => {
                    localContext.getNestedObject("test", "1").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns object representation', () => {
                expect(populate).toHaveBeenCalled();
                expect(result).toBe(testResult);
            });
        });

    });

    describe('getCollection', () => {

        var testObject = new Spiro.ListRepresentation();

        var localContext;
        var result;

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext) => {
            localContext = context;
        }));

        describe('when collection is set', () => {

            beforeEach(inject($rootScope => {

                localContext.setCollection(testObject);

                runs(() => {
                    localContext.getCollection().then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));

            it('returns collection representation', () => {
                expect(result).toBe(testObject);
            });
        });

        describe('when collection is not set', () => {

            beforeEach(inject($rootScope => {

                var getCollectionRun = false;

                runs(() => {
                    localContext.getCollection().then(object => {
                        result = object;
                        getCollectionRun = true;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => getCollectionRun, "result not set", 1000);
            }));

            it('returns object representation', () => {
                expect(result).toBeNull();
            });
        });

    });

    describe('getTransientObject', () => {

        var testObject = new Spiro.DomainObjectRepresentation();

        var localContext : Spiro.Angular.Modern.IContext;
        var result;

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext) => {
            localContext = context;
        }));

        describe('when transient is set', () => {

            beforeEach(inject($rootScope => {

                localContext.setTransientObject(testObject);

                runs(() => {
                    localContext.getTransientObject().then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));

            it('returns transient representation', () => {
                expect(result).toBe(testObject);
            });
        });

        describe('when transient is not set', () => {

            beforeEach(inject($rootScope => {

                var getTransientRun = false;

                runs(() => {
                    localContext.getCollection().then(object => {
                        result = object;
                        getTransientRun = true;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => getTransientRun, "result not set", 1000);
            }));

            it('returns object representation', () => {
                expect(result).toBeNull();
            });
        });

    });

    describe('getService', () => {

        var testObject = new Spiro.DomainObjectRepresentation();

        var localContext;
        var result;

        var getDomainObject;
        var getService;

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext, repLoader: Spiro.Angular.IRepLoader) => {
            spyOnPromise(repLoader, 'populate', testObject);
            getDomainObject = spyOnPromise(context, 'getDomainObject', testObject);
            getService = spyOnPromise(context, 'getService', testObject);

            spyOn(testObject, 'domainType').andReturn(undefined);
            spyOn(testObject, 'instanceId').andReturn(undefined);
            spyOn(testObject, 'serviceId').andReturn("test");

            localContext = context;
        }));

        describe('when currentObject is set', () => {

            beforeEach(inject($rootScope => {

                localContext.setObject(testObject);

                runs(() => {
                    localContext.getObject("test").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns service representation', () => {
                expect(getDomainObject).wasNotCalled();
                expect(getService).wasNotCalled();
                expect(result).toBe(testObject);
            });
        });

        describe('when currentObject is set but is not same', () => {

            beforeEach(inject($rootScope => {

                localContext.setObject(testObject);

                runs(() => {
                    localContext.getObject("test2").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns service representation', () => {
                expect(getDomainObject).wasNotCalled();
                expect(getService).toHaveBeenCalledWith("test2");
                expect(result).toBe(testObject);
            });
        });

        describe('when currentObject is not set', () => {

            beforeEach(inject($rootScope => {

                runs(() => {
                    localContext.getObject("test").then(object => {
                        result = object;
                    });
                    $rootScope.$apply();
                });

                waitsFor(() => !!result, "result not set", 1000);
            }));


            it('returns service representation', () => {
                expect(getDomainObject).wasNotCalled();
                expect(getService).toHaveBeenCalledWith("test");
                expect(result).toBe(testObject);
            });
        });

    });

    describe('getSelectedChoice', () => {

        var localContext: Spiro.Angular.Modern.IContext;
        var result: Spiro.Angular.Modern.ChoiceViewModel[];

        beforeEach(inject(($rootScope, $routeParams, context: Spiro.Angular.Modern.IContext) => {       
            localContext = context;
        }));

     
        describe('when selected choice is not set', () => {

            beforeEach(inject($rootScope => {
                runs(() => {
                    result = localContext.getSelectedChoice("test", "test");
                    $rootScope.$apply();
                });
            }));

            it('returns null', () => {
                expect(result).toBeNull();
            });
        });

        describe('when selected choice is set', () => {

            var testCvm = new Spiro.Angular.Modern.ChoiceViewModel();

            beforeEach(inject($rootScope => {

                localContext.setSelectedChoice("test1", "test2", testCvm);

                runs(() => {
                    result = localContext.getSelectedChoice("test1", "test2");
                    $rootScope.$apply();
                });
            }));

            it('returns cvm array', () => {
                expect(result.length).toBe(1);
                expect(result.pop()).toBe(testCvm);
            });
        });

        describe('when multiple selected choices are set', () => {

            var testCvm1 = new Spiro.Angular.Modern.ChoiceViewModel();
            var testCvm2 = new Spiro.Angular.Modern.ChoiceViewModel();

            beforeEach(inject($rootScope => {

                localContext.setSelectedChoice("test3", "test4", testCvm1);
                localContext.setSelectedChoice("test3", "test4", testCvm2);

                runs(() => {
                    result = localContext.getSelectedChoice("test3", "test4");
                    $rootScope.$apply();
                });
            }));

            it('returns cvm array', () => {
                expect(result.length).toBe(2);
                expect(result.pop()).toBe(testCvm2);
                expect(result.pop()).toBe(testCvm1);
            });
        });

        describe('when match parm but not search', () => {

            var testCvm = new Spiro.Angular.Modern.ChoiceViewModel();

            beforeEach(inject($rootScope => {

                localContext.setSelectedChoice("test5", "test6", testCvm);

                runs(() => {
                    result = localContext.getSelectedChoice("test5", "test7");
                    $rootScope.$apply();
                });
            }));

            it('returns undefined', () => {
                expect(result).toBeUndefined();
            });
        });

        describe('when multiple selected choices are set for a parm', () => {

            var testCvm1 = new Spiro.Angular.Modern.ChoiceViewModel();
            var testCvm2 = new Spiro.Angular.Modern.ChoiceViewModel();

            var result1;

            beforeEach(inject($rootScope => {

                localContext.setSelectedChoice("test6", "test8", testCvm1);
                localContext.setSelectedChoice("test6", "test9", testCvm2);

                runs(() => {
                    result = localContext.getSelectedChoice("test6", "test8");
                    result1 = localContext.getSelectedChoice("test6", "test9");
                    $rootScope.$apply();
                });
            }));

            it('returns cvm arrays', () => {
                expect(result.length).toBe(1);
                expect(result1.length).toBe(1);
                expect(result.pop()).toBe(testCvm1);
                expect(result1.pop()).toBe(testCvm2);
            });
        });


    });

});