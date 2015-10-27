/// <reference path="../../Scripts/typings/karma-jasmine/karma-jasmine.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.handlers.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.viewmodels.ts" />

describe("nakedObjects.gemini.services.context ", () => {

    beforeEach(angular.mock.module("app"));

    describe("getVersion", () => {
        const testVersion = new NakedObjects.VersionRepresentation();
        const testHome = new NakedObjects.HomePageRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        let result: angular.IPromise<NakedObjects.VersionRepresentation>;
        let populate: jasmine.Spy;
        let getHome: jasmine.Spy;
        let getVersion: jasmine.Spy;
        let timeout: ng.ITimeoutService;

        beforeEach(inject(($q, $timeout, $rootScope, $routeParams, context) => {
            localContext = context;
            timeout = $timeout;

            getHome = spyOn(context, "getHome");
            getHome.and.returnValue($q.when(testHome));
            getVersion = spyOn(testHome, "getVersion");
            getVersion.and.returnValue(testVersion);
        }));

        describe("populates Version rep", () => {

            beforeEach(inject(($q, repLoader) => {
                populate = spyOn(repLoader, "populate");
                populate.and.returnValue($q.when(testVersion));
            }));

            it("returns new version representation on first call", () => {
                result = localContext.getVersion();
                timeout.flush();
                expect(populate).toHaveBeenCalled();
                result.then((hr) => expect(hr).toBe(testVersion));
                timeout.flush();
            });

            it("returns cacahed version representation on second call", () => {
                result = localContext.getVersion();
                timeout.flush();
                result = localContext.getVersion();
                timeout.flush();
                expect(populate).toHaveBeenCalled();
                expect(populate.calls.count()).toBe(1);
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

        beforeEach(inject(($q, $timeout, $rootScope, $routeParams, context) => {
            localContext = context;
            timeout = $timeout;

            getHome = spyOn(context, "getHome");
            getHome.and.returnValue($q.when(testHome));
            getMenus = spyOn(testHome, "getMenus");
            getMenus.and.returnValue(testMenus);
        }));

        describe("populates menus rep", () => {

            beforeEach(inject(($q, repLoader) => {
                populate = spyOn(repLoader, "populate");
                populate.and.returnValue($q.when(testMenus));
            }));

            it("returns new menus representation on first call", () => {
                result = localContext.getMenus();
                timeout.flush();
                expect(getHome).toHaveBeenCalled();
                expect(getMenus).toHaveBeenCalled();
                expect(populate).toHaveBeenCalled();
                result.then((hr) => expect(hr).toBe(testMenus));
                timeout.flush();
            });

            it("returns cached menus representation on second call", () => {
                result = localContext.getMenus();
                timeout.flush();
                result = localContext.getMenus();
                timeout.flush();

                expect(getHome).toHaveBeenCalled();
                expect(getMenus).toHaveBeenCalled();
                expect(populate).toHaveBeenCalled();
                expect(populate.calls.count()).toBe(1);
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


            getMenus = spyOn(context, "getMenus");
            getMenus.and.returnValue($q.when(testMenus));
            getMenu = spyOn(testMenus, "getMenu");
            getMenu.and.returnValue(testMenu);
        }));

        describe("populates menu rep", () => {

            beforeEach(inject(($q, repLoader) => {
                populate = spyOn(repLoader, "populate");
                populate.and.returnValue($q.when(testMenu));
            }));

            it("returns new menu representation", () => {
                result = localContext.getMenu("anId");
                timeout.flush();
                expect(getMenus).toHaveBeenCalled();
                expect(getMenu).toHaveBeenCalledWith("anId");
                expect(populate).toHaveBeenCalled();
                result.then((hr) => expect(hr).toBe(testMenu));
                timeout.flush();
            });

            it("returns cached menu representation with same id", () => {
                result = localContext.getMenu("anId");
                timeout.flush();
                result = localContext.getMenu("anId");
                timeout.flush();
                expect(getMenus).toHaveBeenCalled();
                expect(getMenu).toHaveBeenCalledWith("anId");
                expect(populate).toHaveBeenCalled();
                expect(populate.calls.count()).toBe(1);
                result.then((hr) => expect(hr).toBe(testMenu));
                timeout.flush();
            });

            it("returns new menu representation with different id", () => {
                result = localContext.getMenu("anId1");
                timeout.flush();
                result = localContext.getMenu("anId2");
                timeout.flush();
                expect(getMenus).toHaveBeenCalled();
                expect(getMenu).toHaveBeenCalledWith("anId1");
                expect(getMenu).toHaveBeenCalledWith("anId2");
                expect(populate).toHaveBeenCalled();
                expect(populate.calls.count()).toBe(2);
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

        describe("populates a domain object", () => {

            beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context) => {

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

            describe("on first call", () => {

                beforeEach(inject(($q, repLoader) => {
                    populate = spyOn(repLoader, "populate");
                    populate.and.returnValue($q.when(testObject));
                }));


                it("return new object ", () => {
                    result = localContext.getObject(1, "test", ["1"]);
                    timeout.flush();
                    expect(getDomainObject).toHaveBeenCalledWith(1, "test", "1");
                    expect(getService).not.toHaveBeenCalled();
                    expect(populate).toHaveBeenCalled();
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });

            describe("when cached object is not same", () => {

                beforeEach(inject(($q, repLoader) => {
                    populate = spyOn(repLoader, "populate");
                    populate.and.returnValue($q.when(testObject));
                }));

                it("return new object", () => {
                    result = localContext.getObject(1, "test", ["1"]);
                    timeout.flush();
                    result = localContext.getObject(1, "test2", ["2"]);
                    timeout.flush();
                    expect(populate).toHaveBeenCalled();
                    expect(populate.calls.count()).toBe(2);
                    expect(getDomainObject).toHaveBeenCalledWith(1, "test", "1");
                    expect(getDomainObject).toHaveBeenCalledWith(1, "test2", "2");
                    expect(getService).not.toHaveBeenCalled();
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });

            describe("on second call", () => {

                beforeEach(inject(($q, repLoader) => {
                    populate = spyOn(repLoader, "populate");
                    populate.and.returnValue($q.when(testObject));
                }));

                it("returns a cached object", () => {
                    result = localContext.getObject(1, "test", ["1"]);
                    timeout.flush();
                    result = localContext.getObject(1, "test", ["1"]);
                    timeout.flush();

                    expect(populate).toHaveBeenCalled();
                    expect(populate.calls.count()).toBe(1);
                    expect(getDomainObject).toHaveBeenCalledWith(1, "test", "1");
                    expect(getService).not.toHaveBeenCalled();
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });
        });

        describe("populates a service", () => {

            const testServices = new NakedObjects.DomainServicesRepresentation();
            let getServices: jasmine.Spy;
            let getServiceRep: jasmine.Spy;

            beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context) => {


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

            describe("on first call", () => {

                beforeEach(inject(($q, repLoader) => {
                    populate = spyOn(repLoader, "populate");
                    populate.and.returnValue($q.when(testObject));
                }));


                it("returns service representation", () => {
                    result = localContext.getObject(1, "sid");
                    timeout.flush();
                    expect(getDomainObject).not.toHaveBeenCalled();
                    expect(getService).toHaveBeenCalledWith(1, "sid");
                    expect(populate).toHaveBeenCalled();
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });

            describe("when cached object is not the same", () => {


                beforeEach(inject(($q, repLoader) => {

                    populate = spyOn(repLoader, "populate");
                    populate.and.returnValue($q.when(testObject));

                }));


                it("returns service representation", () => {
                    result = localContext.getObject(1, "sid");
                    timeout.flush();
                    result = localContext.getObject(1, "sid2");
                    timeout.flush();
                    expect(populate).toHaveBeenCalled();
                    expect(populate.calls.count()).toBe(2);
                    expect(getDomainObject).not.toHaveBeenCalled();
                    expect(getService).toHaveBeenCalledWith(1, "sid2");
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });

            describe("on second call", () => {

                beforeEach(inject(($q, repLoader) => {
                    populate = spyOn(repLoader, "populate");
                    populate.and.returnValue($q.when(testObject));
                }));


                it("returns service representation", () => {
                    result = localContext.getObject(1, "sid");
                    timeout.flush();
                    result = localContext.getObject(1, "sid");
                    timeout.flush();

                    expect(populate).toHaveBeenCalled();
                    expect(populate.calls.count()).toBe(1);
                    expect(getDomainObject).not.toHaveBeenCalled();
                    expect(getService).toHaveBeenCalledWith(1, "sid");
                    result.then((hr) => expect(hr).toBe(testObject));
                    timeout.flush();
                });
            });
        });

    });

    describe("getObjectByOid", () => {
        let localContext: NakedObjects.Angular.Gemini.IContext;
        let testObject: angular.IPromise<NakedObjects.DomainObjectRepresentation>;
        let getObject: jasmine.Spy;
        let timeout: ng.ITimeoutService;


        beforeEach(inject(($q, $timeout, $rootScope, $routeParams, context) => {
            localContext = context;
            timeout = $timeout;

            getObject = spyOn(context, "getObject");
            getObject.and.returnValue($q.when(testObject));
        }));

        describe("populates Home rep", () => {
            it("returns object representation", () => {
                const result = localContext.getObjectByOid(1, "type-1");
                timeout.flush();
                expect(getObject).toHaveBeenCalledWith(1, "type", ["1"]);
                result.then((hr) => expect(hr).toBe(testObject));
                timeout.flush();
            });
        });
    });

    //describe("getListFromMenu", () => {

    //    const testObject = new NakedObjects.ListRepresentation();
    //    let localContext: NakedObjects.Angular.Gemini.IContext;
    //    let result: angular.IPromise<NakedObjects.ListRepresentation>;
    //    let timeout: ng.ITimeoutService;

    //    beforeEach(inject(($rootScope, $routeParams, $timeout, context) => {
    //        localContext = context;
    //        timeout = $timeout;
    //    }));

    //    describe("on first call", () => {

    //        beforeEach(inject(() => {
    //            result = localContext.getListFromMenu(1, "", "", {});
    //            timeout.flush();
    //        }));

    //        it("populate a new list", () => {
    //            result.then((hr) => expect(hr).toBe(testObject));
    //        });
    //    });
    //});

    //describe("getListFromObject", () => {

    //    const testObject = new NakedObjects.ListRepresentation();
    //    let localContext: NakedObjects.Angular.Gemini.IContext;
    //    let result: angular.IPromise<NakedObjects.ListRepresentation>;
    //    let timeout: ng.ITimeoutService;

    //    beforeEach(inject(($rootScope, $routeParams, $timeout, context) => {
    //        localContext = context;
    //        timeout = $timeout;
    //    }));

    //    describe("when collection is set", () => {

    //        beforeEach(inject(() => {

    //            (<any>localContext).setList(1, testObject);

    //            result = localContext.getListFromObject(1, "", "", {});
    //            timeout.flush();
    //        }));

    //        it("returns collection representation", () => {
    //            result.then((hr) => expect(hr).toBe(testObject));
    //        });
    //    });
    //});

    describe("prompt", () => {

        const testPrompt = new NakedObjects.PromptRepresentation();
        const testPromptResult = new NakedObjects.PromptRepresentation();

        let localContext: NakedObjects.Angular.Gemini.IContext;
        let timeout: ng.ITimeoutService;
        let setResult: jasmine.Spy;
        let populate: jasmine.Spy;
        let result: ng.IPromise<NakedObjects.Angular.Gemini.ChoiceViewModel[]>;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            spyOn(testPromptResult, "choices").and.returnValue([]);

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testPromptResult));
        }));

        describe("invoke", () => {

            beforeEach(inject(() => {
                result = localContext.prompt(testPrompt, "", "");
                timeout.flush();
            }));

            it("returns collection representation", () => {
                // expect(setResult).toHaveBeenCalled();
            });
        });
    });

    describe("conditionalChoices", () => {

        const testPrompt = new NakedObjects.PromptRepresentation();
        const testPromptResult = new NakedObjects.PromptRepresentation();

        let localContext: NakedObjects.Angular.Gemini.IContext;
        let timeout: ng.ITimeoutService;
        let setResult: jasmine.Spy;
        let populate: jasmine.Spy;
        let result: ng.IPromise<NakedObjects.Angular.Gemini.ChoiceViewModel[]>;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            spyOn(testPromptResult, "choices").and.returnValue([]);

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testPromptResult));
        }));

        describe("invoke", () => {

            beforeEach(inject(() => {
                result = localContext.conditionalChoices(testPrompt, "", {});
                timeout.flush();
            }));

            it("returns collection representation", () => {
                // expect(setResult).toHaveBeenCalled();
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