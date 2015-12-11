/// <reference path="../../Scripts/typings/karma-jasmine/karma-jasmine.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.handlers.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.viewmodels.ts" />

function emptyResource(): NakedObjects.RoInterfaces.IResourceRepresentation {
    return { links: [], extensions: {} };
}

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

    describe("getListFromMenu", () => {

        const testList = new NakedObjects.ListRepresentation();
        const testAction = new NakedObjects.ActionMember({} as any, {} as any, "");
        const testMenu = new NakedObjects.MenuRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        const testActionResult = new NakedObjects.ActionResultRepresentation();
        const testResult = new NakedObjects.Result(emptyResource() as any, "list");

        let timeout: ng.ITimeoutService;
        let getMenu: jasmine.Spy;
        let actionMember: jasmine.Spy;
        let invoke: jasmine.Spy;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
            localContext = context;
            timeout = $timeout;


            actionMember = spyOn(testMenu, "actionMember");
            actionMember.and.returnValue(testAction);
            spyOn(testAction, "extensions").and.returnValue({ friendlyName: "aName" });

            invoke = spyOn(repLoader, "invoke");
            invoke.and.returnValue($q.when(testActionResult));

            spyOn(testActionResult, "result").and.returnValue(testResult);
            spyOn(testResult, "list").and.returnValue(testList);
            spyOn(testActionResult, "resultType").and.returnValue("list");
        }));

        describe("on first call", () => {

            let result: angular.IPromise<NakedObjects.ListRepresentation>;

            beforeEach(inject(($q, context) => {
                getMenu = spyOn(context, "getMenu");
                getMenu.and.returnValue($q.when(testMenu));
            }));

            it("return a new list", () => {
                result = localContext.getListFromMenu(1, "", "", {});
                timeout.flush();
                expect(getMenu).toHaveBeenCalled();
                result.then(hr => expect(hr).toBe(testList));
                timeout.flush();
            });
        });

        //describe("on second call", () => {

        //    let result: angular.IPromise<NakedObjects.ListRepresentation>;

        //    beforeEach(inject(($q, context) => {
        //        getMenu = spyOn(context, "getMenu");
        //        getMenu.and.returnValue($q.when(testMenu));
        //    }));

        //    it("return a cached list", () => {
        //        result = localContext.getListFromMenu(1, "", "", {});
        //        timeout.flush();
        //        result = localContext.getListFromMenu(1, "", "", {});
        //        timeout.flush();

        //        expect(getMenu).toHaveBeenCalled();
        //        expect(getMenu.calls.count()).toBe(1);
        //        result.then(hr => expect(hr).toBe(testList));
        //        timeout.flush();
        //    });
        //});
    });

    describe("getListFromObject", () => {

        const testList = new NakedObjects.ListRepresentation();
        const testAction = new NakedObjects.ActionMember({} as any, {} as any, "");
        const testObject = new NakedObjects.DomainObjectRepresentation();
        let localContext: NakedObjects.Angular.Gemini.IContext;
        const testActionResult = new NakedObjects.ActionResultRepresentation();
        const testResult = new NakedObjects.Result(emptyResource() as any, "list");

        let timeout: ng.ITimeoutService;
        let getObjectByOid: jasmine.Spy;
        let actionMember: jasmine.Spy;
        let invoke: jasmine.Spy;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
            localContext = context;
            timeout = $timeout;


            actionMember = spyOn(testObject, "actionMember");
            actionMember.and.returnValue(testAction);
            spyOn(testAction, "extensions").and.returnValue({ friendlyName: "aName" });

            invoke = spyOn(repLoader, "invoke");
            invoke.and.returnValue($q.when(testActionResult));

            spyOn(testActionResult, "result").and.returnValue(testResult);
            spyOn(testResult, "list").and.returnValue(testList);
            spyOn(testActionResult, "resultType").and.returnValue("list");
        }));

        describe("on first call", () => {

            let result: angular.IPromise<NakedObjects.ListRepresentation>;

            beforeEach(inject(($q, context) => {
                getObjectByOid = spyOn(context, "getObjectByOid");
                getObjectByOid.and.returnValue($q.when(testObject));
            }));

            it("return a new list", () => {
                result = localContext.getListFromObject(1, "", "", {});
                timeout.flush();
                expect(getObjectByOid).toHaveBeenCalled();
                result.then(hr => expect(hr).toBe(testList));
                timeout.flush();
            });
        });

        //describe("on second call", () => {

        //    let result: angular.IPromise<NakedObjects.ListRepresentation>;

        //    beforeEach(inject(($q, context) => {
        //        getObjectByOid = spyOn(context, "getObjectByOid");
        //        getObjectByOid.and.returnValue($q.when(testObject));
        //    }));

        //    it("return a cached list", () => {
        //        result = localContext.getListFromObject(1, "", "", {});
        //        timeout.flush();
        //        result = localContext.getListFromObject(1, "", "", {});
        //        timeout.flush();

        //        expect(getObjectByOid).toHaveBeenCalled();
        //        expect(getObjectByOid.calls.count()).toBe(1);
        //        result.then(hr => expect(hr).toBe(testList));
        //        timeout.flush();
        //    });
        //});
    });

    describe("prompt", () => {

        const testPrompt = new NakedObjects.PromptRepresentation();
        const testPromptResult = new NakedObjects.PromptRepresentation();

        let localContext: NakedObjects.Angular.Gemini.IContext;
        let timeout: ng.ITimeoutService;
        let populate: jasmine.Spy;
        let result: ng.IPromise<NakedObjects.Angular.Gemini.ChoiceViewModel[]>;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            spyOn(testPromptResult, "choices").and.returnValue({ "test1": new NakedObjects.Value("1"), "test2": new NakedObjects.Value("2") });

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testPromptResult));
        }));

        describe("returns array of ChoiceViewModels", () => {

            const cvm1 = NakedObjects.Angular.Gemini.ChoiceViewModel.create(new NakedObjects.Value("1"), "id", "test1", "search");
            const cvm2 = NakedObjects.Angular.Gemini.ChoiceViewModel.create(new NakedObjects.Value("2"), "id", "test2", "search");

            const map = {};
            const map1 = new NakedObjects.PromptMap(testPrompt, map as any)

            testPrompt.getPromptMap = () => map1;

            const cvmExpected = [cvm1, cvm2];

            beforeEach(inject(() => {
                result = localContext.prompt(testPrompt, "id", "search");
                timeout.flush();
            }));

            it("returns collection representation", () => {
                expect(map["x-ro-searchTerm"].value).toBe("search");
                expect(populate).toHaveBeenCalledWith(map1, true, testPrompt);
                result.then(hr => expect(hr).toEqual(cvmExpected));
                timeout.flush();
            });
        });
    });

    describe("conditionalChoices", () => {

        const testPrompt = new NakedObjects.PromptRepresentation();
        const testPromptResult = new NakedObjects.PromptRepresentation();

        let localContext: NakedObjects.Angular.Gemini.IContext;
        let timeout: ng.ITimeoutService;
        let populate: jasmine.Spy;
        let result: ng.IPromise<NakedObjects.Angular.Gemini.ChoiceViewModel[]>;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
            localContext = context;
            timeout = $timeout;

            spyOn(testPromptResult, "choices").and.returnValue({ "test1": new NakedObjects.Value("1"), "test2": new NakedObjects.Value("2") });

            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testPromptResult));
        }));

        describe("returns array of ChoiceViewModels", () => {

            const cvm1 = NakedObjects.Angular.Gemini.ChoiceViewModel.create(new NakedObjects.Value("1"), "id", "test1", null);
            const cvm2 = NakedObjects.Angular.Gemini.ChoiceViewModel.create(new NakedObjects.Value("2"), "id", "test2", null);

            const map = {};
            const map1 = new NakedObjects.PromptMap(testPrompt, map as any);

            testPrompt.getPromptMap = () => map1;        

            const cvmExpected = [cvm1, cvm2];

            beforeEach(inject(() => {
                result = localContext.conditionalChoices(testPrompt, "id", { anarg: new NakedObjects.Value("avalue") });
                timeout.flush();
            }));

            it("returns collection representation", () => {
                expect(map["x-ro-searchTerm"]).toBeUndefined();
                expect(map["anarg"].value).toBe("avalue");
                expect(populate).toHaveBeenCalledWith(map1, true, testPrompt);
                result.then(hr => expect(hr).toEqual(cvmExpected));
                timeout.flush();
            });
        });
    });

    //todo fix
    //describe("invokeAction", () => {

    //    const testAction = new NakedObjects.ActionMember(null, null, null);
    //    const testActionResult = new NakedObjects.ActionResultRepresentation();
    //    const testResult = new NakedObjects.Result(emptyResource() as any, "");
    //    const testObject = new NakedObjects.DomainObjectRepresentation();
    //    const testList = new NakedObjects.ListRepresentation();

    //    let localContext: NakedObjects.Angular.Gemini.IContext;
    //    let timeout: ng.ITimeoutService;
    //    let populate: jasmine.Spy;

    //    beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader) => {
    //        localContext = context;
    //        timeout = $timeout;
    //        spyOn(testAction, "getInvoke").and.returnValue(testActionResult);
    //        spyOn(testAction, "extensions").and.returnValue({ friendlyName: "aName" });
        
    //    }));

    //    describe("and return object", () => {

    //        beforeEach(inject(($q, repLoader) => {
    //            populate = spyOn(repLoader, "populate");
    //            populate.and.returnValue($q.when(testActionResult));
    //            spyOn(testObject, "serviceId").and.returnValue(null);
    //            spyOn(testObject, "instanceId").and.returnValue("id");
    //            spyOn(testObject, "domainType").and.returnValue("dt");
    //            spyOn(testObject, "persistLink").and.returnValue(null);
    //            spyOn(testResult, "object").and.returnValue(testObject);
    //            spyOn(testActionResult, "resultType").and.returnValue("object");
    //            spyOn(testResult, "isNull").and.returnValue(false);
    //            spyOn(testActionResult, "result").and.returnValue(testResult);

    //            localContext.invokeAction(testAction, 1, null);
    //            timeout.flush();
    //        }));

    //        it("sets domain object on context", () => {
    //            (<any>localContext).getDomainObject(1, "dt", "id").then(hr => expect(hr).toEqual(testObject));
    //            timeout.flush();
    //        });
    //    });

    //    //describe("and return transient", () => {

    //    //    beforeEach(inject(($q, repLoader) => {
    //    //        populate = spyOn(repLoader, "populate");
    //    //        populate.and.returnValue($q.when(testActionResult));
    //    //        spyOn(testObject, "serviceId").and.returnValue(null);
    //    //        spyOn(testObject, "instanceId").and.returnValue("id");
    //    //        spyOn(testObject, "domainType").and.returnValue("dt");
    //    //        spyOn(testObject, "extensions").and.returnValue({ domainType: "dt" });
    //    //        spyOn(testObject, "persistLink").and.returnValue({});
    //    //        spyOn(testResult, "object").and.returnValue(testObject);
    //    //        spyOn(testActionResult, "resultType").and.returnValue("object");
    //    //        spyOn(testResult, "isNull").and.returnValue(false);
    //    //        spyOn(testActionResult, "result").and.returnValue(testResult);

    //    //        localContext.invokeAction(testAction, 1);
    //    //        timeout.flush();
    //    //    }));

    //    //    it("sets domain object on context", () => {
    //    //        (<any>localContext).getDomainObject(1, "dt", "id").then(hr => expect(hr).toEqual(testObject));
    //    //        timeout.flush();
    //    //    });

    //    //});

    //    describe("and return list", () => {

    //        beforeEach(inject(($q, repLoader) => {
    //            populate = spyOn(repLoader, "populate");
    //            populate.and.returnValue($q.when(testActionResult));
    //            spyOn(testResult, "list").and.returnValue(testList);
    //            spyOn(testActionResult, "resultType").and.returnValue("list");
    //            spyOn(testResult, "isNull").and.returnValue(false);
    //            spyOn(testActionResult, "result").and.returnValue(testResult);

    //            localContext.invokeAction(testAction, 1, null);
    //            timeout.flush();
    //        }));

    //        it("sets list on context", () => {
    //            //localContext.getListFromObject(1, "dt", "id", {}).then(hr => expect(hr).toEqual(testList));

    //            //timeout.flush();
    //        });
    //    });

    //    describe("and return null", () => {

    //        beforeEach(inject(($q, repLoader) => {
    //            populate = spyOn(repLoader, "populate");
    //            populate.and.returnValue($q.when(testActionResult));
    //            spyOn(testActionResult, "resultType").and.returnValue("object");
    //            spyOn(testResult, "isNull").and.returnValue(true);
    //            spyOn(testActionResult, "result").and.returnValue(testResult);
    //            localContext.invokeAction(testAction, 1, null);
    //            timeout.flush();
    //        }));

    //        it("does nothing", () => {
    //            //expect(setResult).toHaveBeenCalled();
    //        });
    //    });

    //    describe("and return error map", () => {

    //        const testDvm = new NakedObjects.Angular.Gemini.DialogViewModel();
    //        const testPvm = new NakedObjects.Angular.Gemini.ParameterViewModel();
    //        const testInvokeMap = new NakedObjects.InvokeMap({} as any, {} as any );
    //        testDvm.parameters = [testPvm];
    //        testPvm.id = "test";
    //        testPvm.value = "";
    //        testPvm.type = "scalar";
    //        testPvm.description = "description";

    //        beforeEach(inject(($q: ng.IQService, repLoader) => {
    //            const errorMap = new NakedObjects.ErrorMap({}, 0, "");
    //            populate = spyOn(repLoader, "populate");
    //            populate.and.returnValue($q.reject(errorMap));
    //            spyOn(errorMap, "valuesMap").and.returnValue({"test" : {value : new NakedObjects.Value(""), invalidReason : "Mandatory" } });
    //            spyOn(errorMap, "invalidReason").and.returnValue("errormessage");
    //            spyOn(testActionResult, "getInvokeMap").and.returnValue(testInvokeMap);
    //            spyOn(testActionResult, "resultType").and.returnValue("object");
    //            spyOn(testResult, "isNull").and.returnValue(true);
    //            spyOn(testActionResult, "result").and.returnValue(testResult);
    //            localContext.invokeAction(testAction, 1, null);
    //            timeout.flush();
    //        }));

    //        it("it sets error on dvm", () => {
    //            // fix
    //            //expect(testDvm.message).toBe("errormessage");
    //            //expect(testPvm.description).toBe("REQUIRED description");
    //        });
    //    });


    //    describe("and return null", () => {

    //        const testDvm = new NakedObjects.Angular.Gemini.DialogViewModel();
    //        const testPvm = new NakedObjects.Angular.Gemini.ParameterViewModel();
    //        const testInvokeMap = new NakedObjects.InvokeMap({} as any, {} as any);
    //        testDvm.parameters = [testPvm];
    //        testPvm.id = "test";
    //        testPvm.value = "";
    //        testPvm.type = "scalar";

    //        beforeEach(inject(($q, repLoader) => {
    //            populate = spyOn(repLoader, "populate");
    //            populate.and.returnValue($q.when(testActionResult));
    //            spyOn(testActionResult, "getInvokeMap").and.returnValue(testInvokeMap);
    //            spyOn(testActionResult, "resultType").and.returnValue("object");
    //            spyOn(testResult, "isNull").and.returnValue(true);
    //            spyOn(testActionResult, "result").and.returnValue(testResult);
    //            localContext.invokeAction(testAction, 1, null);
    //            timeout.flush();
    //        }));

    //        it("sets dialog message", () => {
    //           // expect(testDvm.message).toBe("no result found");
    //        });
    //    });

    //});

    describe("updateObject", () => {

        const testObject = new NakedObjects.DomainObjectRepresentation();
       
        const testUpdatedObject = new NakedObjects.DomainObjectRepresentation();
        const testOvm = new NakedObjects.Angular.Gemini.DomainObjectViewModel();
        const testPvm = new NakedObjects.Angular.Gemini.PropertyViewModel();
        testOvm.properties = [testPvm];
        testPvm.id = "test";
        testPvm.value = "";
        testPvm.type = "scalar";
        testPvm.isEditable = true;

        let localContext: NakedObjects.Angular.Gemini.IContext;

        let timeout: ng.ITimeoutService;
        let populate: jasmine.Spy;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader, urlManager) => {
            localContext = context;
            timeout = $timeout;
            const testUpdate = <NakedObjects.UpdateMap><any>{ setProperty : () => {} };
            spyOn(testUpdate, "setProperty");
            spyOn(testObject, "getUpdateMap").and.returnValue(testUpdate);
            spyOn(testUpdatedObject, "url").and.returnValue("");
            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testUpdatedObject));
            spyOn(urlManager, "setObject");
        }));

        //describe("successfully update object", () => {

        //    beforeEach(inject(() => {
        //        localContext.updateObject(testObject, testOvm);
        //        timeout.flush();
        //    }));

        //    it("sets object on context", () => {
        //        //(<any>localContext).getDomainObject(1, "dt", "id").then(hr => expect(hr).toEqual(testUpdatedObject));
        //        //timeout.flush();
        //    });
        //});
    });

    describe("saveObject", () => {

        const testObject = new NakedObjects.DomainObjectRepresentation();
        
        const testUpdatedObject = new NakedObjects.DomainObjectRepresentation();
        const testOvm = new NakedObjects.Angular.Gemini.DomainObjectViewModel();
        const testPvm = new NakedObjects.Angular.Gemini.PropertyViewModel();
        testOvm.properties = [testPvm];
        testPvm.id = "test";
        testPvm.value = "";
        testPvm.type = "scalar";
        testPvm.isEditable = true;

        let localContext: NakedObjects.Angular.Gemini.IContext;

        let timeout: ng.ITimeoutService;
        let populate: jasmine.Spy;
        let location: ng.ILocationService;

        beforeEach(inject(($q, $rootScope, $routeParams, $timeout, context, repLoader, urlManager) => {
            localContext = context;
            timeout = $timeout;
            const testPersist = <NakedObjects.PersistMap><any>{ setMember: () => { }};
            spyOn(testPersist, "setMember");
            spyOn(testObject, "getPersistMap").and.returnValue(testPersist);
            spyOn(testUpdatedObject, "getUrl").and.returnValue("");
            populate = spyOn(repLoader, "populate");
            populate.and.returnValue($q.when(testUpdatedObject));
            spyOn(urlManager, "setObject");
        }));

        describe("save on edit", () => {

            beforeEach(inject(($location) => {
                location = $location;
             
                localContext.saveObject(testObject, testOvm, false);
                timeout.flush();
            }));

            it("sets object on context and pops url", () => {
                (<any>localContext).getDomainObject(1, "dt", "id").then(hr => expect(hr).toEqual(testUpdatedObject));
                timeout.flush();
               
            });
        });

        describe("save on view", () => {

            beforeEach(inject(($location) => {
                location = $location;
           
                localContext.saveObject(testObject, testOvm, true);
                timeout.flush();
            }));

            it("sets object on context and updates url", () => {
                (<any>localContext).getDomainObject(1, "dt", "id").then(hr => expect(hr).toEqual(testUpdatedObject));
                timeout.flush();
              
            });
        });
    });

    describe("isSubTypeOf", () => {

        const testObject = new NakedObjects.DomainObjectRepresentation();
        const testPersist = <NakedObjects.PersistMap>{};
        const testUpdatedObject = new NakedObjects.DomainTypeActionInvokeRepresentation("", "");
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
            spyOn(testUpdatedObject, "value").and.returnValue(true);
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