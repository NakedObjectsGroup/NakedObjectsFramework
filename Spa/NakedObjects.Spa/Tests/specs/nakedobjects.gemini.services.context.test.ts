/// <reference path="../../Scripts/typings/karma-jasmine/karma-jasmine.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.handlers.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.viewmodels.ts" />

describe("nakedObjects.gemini.services.context ", () => {

    beforeEach(angular.mock.module("app"));

    describe("getHome", () => {
        const testHome = new NakedObjects.HomePageRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.HomePageRepresentation>;
        let populate: jasmine.Spy;
        let timeout: ng.ITimeoutService;


        beforeEach(inject(($q, $timeout, $rootScope, $routeParams, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testHome));
        }));

        describe("populates Home rep", () => {

            beforeEach(inject(() => {
                result = localContext.getHome();
            }));

            it("returns home representation", () => {
                expect(populate).toHaveBeenCalled();
                result.then((hr) => expect(hr).toBe(testHome));
                timeout.flush();
            });
        });
    });

    describe("getVersion", () => {
        const testVersion = new NakedObjects.VersionRepresentation();
        const testHome = new NakedObjects.HomePageRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.VersionRepresentation>;
        let populate: jasmine.Spy;
        let getHome: jasmine.Spy;
        let getVersion: jasmine.Spy;
        let timeout: ng.ITimeoutService;

        beforeEach(inject(($q, $timeout, $rootScope, $routeParams, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testVersion));
            getHome = spyOn(context, "getHome");
            getHome.and.returnValue($q.when(testHome));
            getVersion = spyOn(testHome, "getVersion");
            getVersion.and.returnValue(testVersion);
        }));

        describe("populates Version rep", () => {

            beforeEach(inject(() => {
                result = localContext.getVersion();
                timeout.flush();
            }));

            it("returns version representation", () => {
                expect(populate).toHaveBeenCalled();
                result.then((hr) => expect(hr).toBe(testVersion));
                timeout.flush();
            });
        });
    });

    describe("getMenus", () => {
        const testMenus = new NakedObjects.MenusRepresentation();
        const testHome = new NakedObjects.HomePageRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.MenusRepresentation>;
        let populate: jasmine.Spy;
        let getHome: jasmine.Spy;
        let getMenus: jasmine.Spy;
        let timeout: ng.ITimeoutService;

        beforeEach(inject(($q, $timeout, $rootScope, $routeParams, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testMenus));
            getHome = spyOn(context, "getHome");
            getHome.and.returnValue($q.when(testHome));
            getMenus = spyOn(testHome, "getMenus");
            getMenus.and.returnValue(testMenus);

        }));

        describe("populates menus rep", () => {

            beforeEach(inject(() => {
                result = localContext.getMenus();
                timeout.flush();
            }));

            it("returns menus representation", () => {
                expect(getHome).toHaveBeenCalled();
                expect(getMenus).toHaveBeenCalled();
                expect(populate).toHaveBeenCalled();
                result.then((hr) => expect(hr).toBe(testMenus));
                timeout.flush();
            });
        });
    });

    describe("getMenu", () => {
        const testMenu = new NakedObjects.MenuRepresentation();
        const testMenus = new NakedObjects.MenusRepresentation();

        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.MenuRepresentation>;
        let populate: jasmine.Spy;
        let getMenus: jasmine.Spy;
        let getMenu: jasmine.Spy;
        let timeout: ng.ITimeoutService;

        beforeEach(inject(($q, $timeout, $rootScope, $routeParams, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testMenu));

            getMenus = spyOn(context, "getMenus");
            getMenus.and.returnValue($q.when(testMenus));
            getMenu = spyOn(testMenus, "getMenu");
            getMenu.and.returnValue(testMenu);
        }));

        describe("populates menu rep", () => {

            beforeEach(inject(() => {
                result = localContext.getMenu("anId");
                timeout.flush();
            }));

            it("returns menu representation", () => {
                expect(getMenus).toHaveBeenCalled();
                expect(getMenu).toHaveBeenCalledWith("anId");
                expect(populate).toHaveBeenCalled();
                result.then((hr) => expect(hr).toBe(testMenu));
                timeout.flush();
            });
        });
    });

    describe("getObject", () => {

        const testObject = new NakedObjects.DomainObjectRepresentation();

        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.DomainObjectRepresentation>;
        let getDomainObject: jasmine.Spy;
        let getService: jasmine.Spy;
        let populate: jasmine.Spy;
        let timeout: ng.ITimeoutService;

        describe("getting a domain object", () => {

            beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
                populate = spyOn(repLoader, "populate");
                populate.and.returnValue($q.when(testObject));

                getDomainObject = spyOn(context, "getDomainObject");
                getDomainObject.and.callThrough();

                getService = spyOn(context, "getService");
                getService.and.callThrough();

                spyOn(testObject, "domainType").and.returnValue("test");
                spyOn(testObject, "instanceId").and.returnValue("1");
                spyOn(testObject, "serviceId").and.returnValue(undefined);

                localContext = context;
                timeout = $timeout;
            }));

            describe("when currentObject is set", () => {

                beforeEach(inject(() => {
                    (<any> localContext).setObject(testObject);
                    result = localContext.getObject(1, "test", ["1"]);
                    timeout.flush();
                }));


                it("returns object representation", () => {
                    expect(getDomainObject).toHaveBeenCalledWith(1, "test", "1");
                    expect(getService).not.toHaveBeenCalled();
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });

            describe("when currentObject is set but not same", () => {

                beforeEach(inject(() => {

                    (<any> localContext).setObject(testObject);
                    result = localContext.getObject(1, "test2", ["2"]);
                    timeout.flush();
                }));

                it("returns object representation", () => {
                    expect(populate).toHaveBeenCalled();
                    expect(getDomainObject).toHaveBeenCalledWith(1, "test2", "2");
                    expect(getService).not.toHaveBeenCalled();
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });

            describe("when currentObject is not set", () => {

                beforeEach(inject(() => {
                    result = localContext.getObject(1, "test", ["1"]);
                    timeout.flush();
                }));

                it("returns object representation", () => {
                    expect(populate).toHaveBeenCalled();
                    expect(getDomainObject).toHaveBeenCalledWith(1, "test", "1");
                    expect(getService).not.toHaveBeenCalled();
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });
        });

        describe("getting a service", () => {

            const testServices = new NakedObjects.DomainServicesRepresentation();
            let getServices: jasmine.Spy;
            let getServiceRep: jasmine.Spy;

            beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
                populate = spyOn(repLoader, "populate");
                populate.and.returnValue($q.when(testObject));

                getDomainObject = spyOn(context, "getDomainObject");
                getDomainObject.and.callThrough();

                getService = spyOn(context, "getService");
                getService.and.callThrough();

                getServices = spyOn(context, "getServices");
                getServices.and.returnValue($q.when(testServices));

                getServiceRep = spyOn(testServices, "getService");
                getServiceRep.and.returnValue(testObject);

                spyOn(testObject, "domainType").and.returnValue("test");
                spyOn(testObject, "instanceId").and.returnValue(undefined);
                spyOn(testObject, "serviceId").and.returnValue("sid");

                localContext = context;
                timeout = $timeout;
            }));

            describe("when currentObject is set", () => {

                beforeEach(inject(() => {
                    (<any> localContext).setObject(testObject);
                    result = localContext.getObject(1, "test");
                    timeout.flush();
                }));


                it("returns service representation", () => {
                    expect(getDomainObject).not.toHaveBeenCalled();
                    expect(getService).toHaveBeenCalledWith(1, "test");
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });

            describe("when currentObject is set but not same", () => {


                beforeEach(inject(() => {

                    (<any> localContext).setObject(testObject);
                    result = localContext.getObject(1, "test2");
                    timeout.flush();
                }));


                it("returns service representation", () => {
                    expect(populate).toHaveBeenCalled();
                    expect(getDomainObject).not.toHaveBeenCalled();
                    expect(getService).toHaveBeenCalledWith(1, "test2");
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });


            describe("when currentObject is not set", () => {

                beforeEach(inject(() => {

                    result = localContext.getObject(1, "test");
                    timeout.flush();
                }));


                it("returns service representation", () => {
                    expect(populate).toHaveBeenCalled();
                    expect(getDomainObject).not.toHaveBeenCalled();
                    expect(getService).toHaveBeenCalledWith(1, "test");
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });
        });


    });

    //describe("getObjectByOid", () => {

    //    describe("when get a domain object by id", () => {

    //        let getObject: jasmine.Spy;
    //        let localContext: NakedObjects.Angular.Gemini.IContext;

    //        beforeEach(inject((context) => {
    //            localContext = context;
    //            getObject = spyOn(context, "getObject");                          
    //        }));

    //        describe("getObject is called with pane, dt and id", () => {
    //            let a = localContext.getObjectByOid(1, "adt-anid");
    //            expect(getObject).toHaveBeenCalled();
    //        });
         
    //    });
    
    //});


    describe("getList", () => {

        const testObject = new NakedObjects.ListRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.ListRepresentation>;
        let timeout: ng.ITimeoutService;

        beforeEach(inject(($rootScope, $routeParams, $timeout, context) => {
            localContext = context;
            timeout = $timeout;
        }));

        describe("when collection is set", () => {

            beforeEach(inject(() => {

                (<any>localContext).setList(1, testObject);

                result = localContext.getList(1, "", "", {});
                timeout.flush();
            }));

            it("returns collection representation", () => {
                result.then((hr) => expect(hr).toBe(testObject));
            });
        });
    });

    describe("getListFromObject", () => {

        const testObject = new NakedObjects.ListRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.ListRepresentation>;
        let timeout: ng.ITimeoutService;

        beforeEach(inject(($rootScope, $routeParams, $timeout, context) => {
            localContext = context;
            timeout = $timeout;
        }));

        describe("when collection is set", () => {

            beforeEach(inject(() => {

                (<any>localContext).setList(1, testObject);

                result = localContext.getListFromObject(1, "", "", {});
                timeout.flush();
            }));

            it("returns collection representation", () => {
                result.then((hr) => expect(hr).toBe(testObject));
            });
        });
    });
    
    describe("invokeAction", () => {

        const testAction = new NakedObjects.ActionMember(null, null, null);
        const testInvoke = new NakedObjects.DomainTypeActionInvokeRepresentation();
        const testResult = new NakedObjects.ActionResultRepresentation();

        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.ListRepresentation>;
        let timeout: ng.ITimeoutService;
        let setResult: jasmine.Spy;
        let populate: jasmine.Spy;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
            localContext = context;
            timeout = $timeout;
            spyOn(testAction, "getInvoke").and.returnValue(testInvoke);
            setResult = spyOn(context, "setResult");
            setResult.and.returnValue(testInvoke);
            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testResult));
        }));

        describe("invoke", () => {

            beforeEach(inject(() => {
                localContext.invokeAction(testAction, 1);
                timeout.flush();
            }));

            it("returns collection representation", () => {
                expect(setResult).toHaveBeenCalled();
            });
        });
    });

    describe("updateObject", () => {

        const testObject = new NakedObjects.DomainObjectRepresentation({});
        const testUpdate = <NakedObjects.UpdateMap>{};
        const testUpdatedObject = new NakedObjects.DomainObjectRepresentation();
        const testResult = new NakedObjects.ActionResultRepresentation();
        const testOvm = new NakedObjects.Angular.Gemini.DomainObjectViewModel();
        testOvm.properties = [];

        let localContext: NakedObjects.Angular.Gemini.IContext;
       
        let timeout: ng.ITimeoutService;
        let setResult: jasmine.Spy;
        let populate: jasmine.Spy;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader, urlManager) => {
            localContext = context;
            timeout = $timeout;
            spyOn(testObject, "getUpdateMap").and.returnValue(testUpdate);
            spyOn(testUpdatedObject, "set");
            spyOn(testUpdatedObject, "url").and.returnValue("");
            spyOn(testObject, "get").and.returnValue("");
            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testUpdatedObject));
            spyOn(urlManager, "setObject");
        }));

        describe("update", () => {

            beforeEach(inject(() => {
                localContext.updateObject(testObject, testOvm);
                timeout.flush();
            }));

            it("returns collection representation", () => {
                //expect(setResult).toHaveBeenCalled();
            });
        });
    });

    describe("saveObject", () => {

        const testObject = new NakedObjects.DomainObjectRepresentation({});
        const testPersist = <NakedObjects.PersistMap>{};
        const testUpdatedObject = new NakedObjects.DomainObjectRepresentation();
        const testResult = new NakedObjects.ActionResultRepresentation();
        const testOvm = new NakedObjects.Angular.Gemini.DomainObjectViewModel();
        testOvm.properties = [];

        let localContext: NakedObjects.Angular.Gemini.IContext;

        let timeout: ng.ITimeoutService;
        let setResult: jasmine.Spy;
        let populate: jasmine.Spy;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader, urlManager) => {
            localContext = context;
            timeout = $timeout;
            spyOn(testObject, "getPersistMap").and.returnValue(testPersist);
            spyOn(testUpdatedObject, "set");
            spyOn(testUpdatedObject, "url").and.returnValue("");
            spyOn(testObject, "get").and.returnValue("");
            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testUpdatedObject));
            spyOn(urlManager, "setObject");
        }));

        describe("save", () => {

            beforeEach(inject(() => {
                localContext.saveObject(testObject, testOvm, false);
                timeout.flush();
            }));

            it("returns collection representation", () => {
                //expect(setResult).toHaveBeenCalled();
            });
        });
    });

    describe("isSubTypeOf", () => {

        const testObject = new NakedObjects.DomainObjectRepresentation({});
        const testPersist = <NakedObjects.PersistMap>{};
        const testUpdatedObject = new NakedObjects.DomainTypeActionInvokeRepresentation();
        const testResult = new NakedObjects.ActionResultRepresentation();
        const testOvm = new NakedObjects.Angular.Gemini.DomainObjectViewModel();
        testOvm.properties = [];

        let localContext: NakedObjects.Angular.Gemini.IContext;

        let timeout: ng.ITimeoutService;
        let setResult: jasmine.Spy;
        let populate: jasmine.Spy;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader, urlManager) => {
            localContext = context;
            timeout = $timeout;
            spyOn(testObject, "getPersistMap").and.returnValue(testPersist);
            spyOn(testUpdatedObject, "set");
            spyOn(testUpdatedObject, "value").and.returnValue(true);
            spyOn(testObject, "get").and.returnValue("");
            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testUpdatedObject));
            spyOn(urlManager, "setObject");
        }));

        describe("save", () => {

            beforeEach(inject(() => {
                localContext.isSubTypeOf("", "");
                timeout.flush();
            }));

            it("returns collection representation", () => {
                //expect(setResult).toHaveBeenCalled();
            });
        });
    });



});