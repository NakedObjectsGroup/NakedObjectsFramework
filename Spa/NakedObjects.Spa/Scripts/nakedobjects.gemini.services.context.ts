/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export interface IContext {
        
        getVersion: () => ng.IPromise<VersionRepresentation>;
        getMenus: () => ng.IPromise<MenusRepresentation>;
        getMenu: (menuId: string) => ng.IPromise<MenuRepresentation>;
        getObject: (paneId : number, type: string, id?: string[]) => ng.IPromise<DomainObjectRepresentation>;
        getObjectByOid: (paneId: number, objectId : string) => ng.IPromise<DomainObjectRepresentation>;    
        getListFromMenu: (paneId : number, menuId: string, actionId: string, parms : _.Dictionary<Value>) => angular.IPromise<ListRepresentation>;
        getListFromObject: (paneId: number, objectId: string, actionId: string, parms: _.Dictionary<Value>) => angular.IPromise<ListRepresentation>;

        getLastActionFriendlyName: (paneId: number) => string;
        getError: () => ErrorRepresentation;
        getPreviousUrl: () => string;

        prompt(promptRep: PromptRepresentation, id: string, searchTerm: string): ng.IPromise<ChoiceViewModel[]>;
        conditionalChoices(promptRep: PromptRepresentation, id: string, args: IValueMap): ng.IPromise<ChoiceViewModel[]>;
      
        invokeAction(action: ActionMember, paneId : number, dvm?: DialogViewModel);
        updateObject(object: DomainObjectRepresentation, ovm: DomainObjectViewModel);
        saveObject(object: DomainObjectRepresentation, ovm: DomainObjectViewModel, viewObject: boolean);

        setError: (object: ErrorRepresentation) => void;
    
        isSubTypeOf(toCheckType : string, againstType : string): ng.IPromise<boolean>;
    }

    interface IContextInternal extends IContext {
        getHome: () => ng.IPromise<HomePageRepresentation>;
        getDomainObject: (paneId : number, type: string, id: string) => ng.IPromise<DomainObjectRepresentation>;
        getServices: () => ng.IPromise<DomainServicesRepresentation>;
        getService: (paneId: number, type: string) => ng.IPromise<DomainObjectRepresentation>;

        setObject: (paneId : number, object: DomainObjectRepresentation) => void;          
        setLastActionFriendlyName: (fn : string, paneId : number) => void;
        setList(paneId : number, listRepresentation: ListRepresentation);
        setResult(action: ActionMember, result: ActionResultRepresentation, paneId : number, dvm?: DialogViewModel);
        setInvokeUpdateError(error: any, vms: ValueViewModel[], vm?: MessageViewModel);
        setPreviousUrl: (url: string) => void;
    }

    app.service("context", function ($q: ng.IQService, repLoader: IRepLoader, urlManager : IUrlManager, $cacheFactory: ng.ICacheFactoryService) {
        const context = <IContextInternal>this;

        // cached values
       
        const currentObjects: DomainObjectRepresentation[] = []; // per pane 
        const currentMenuList: _.Dictionary<MenuRepresentation> = {};
        let currentServices: DomainServicesRepresentation = null;
        let currentMenus: MenusRepresentation = null;
        let currentVersion: VersionRepresentation = null;
        const currentCollections:  ListRepresentation[] = []; // per pane 
        const lastActionFriendlyName: string[] = [];

        function getAppPath() {
            if (appPath.charAt(appPath.length - 1) === "/") {
                return appPath.length > 1 ? appPath.substring(0, appPath.length - 2) : "";
            }
            return appPath;
        }

        function isSameObject(object: DomainObjectRepresentation, type: string, id?: string) {
            if (object) {
                const sid = object.serviceId();
                return sid ? sid === type : (object.domainType() === type && object.instanceId() === id);
            }
            return false;
        }

        // exposed for test mocking
        context.getDomainObject = (paneId : number, type: string, id: string): ng.IPromise<DomainObjectRepresentation> => {

            if (isSameObject(currentObjects[paneId], type, id)) {
                return $q.when(currentObjects[paneId]);
            }

            const object = new DomainObjectRepresentation();
            object.hateoasUrl = getAppPath() + "/objects/" + type + "/" + id;

            return repLoader.populate<DomainObjectRepresentation>(object).
                then((obj: DomainObjectRepresentation) => {
                    currentObjects[paneId] = obj;
                    return $q.when(obj);
                });
        };

        context.getService = (paneId : number, serviceType: string): ng.IPromise<DomainObjectRepresentation> => {

            if (isSameObject(currentObjects[paneId], serviceType)) {
                return $q.when(currentObjects[paneId]);
            }

            return context.getServices().
                then((services: DomainServicesRepresentation) => {
                    const service = services.getService(serviceType);
                    return repLoader.populate(service);
                }).
                then((service: DomainObjectRepresentation) => {
                    currentObjects[paneId] = service;
                    return $q.when(service);
                });
        };

        context.getMenu = (menuId: string): ng.IPromise<MenuRepresentation> => {

            if (currentMenuList[menuId]) {
                return $q.when(currentMenuList[menuId]);
            }

            return context.getMenus().
                then((menus: MenusRepresentation) => {
                    const menu = menus.getMenu(menuId);
                    return repLoader.populate(menu);
                }).
                then((menu: MenuRepresentation) => {
                    currentMenuList[menuId] = menu;
                    return $q.when(menu);
                });
        };

        context.getHome = () => {
            // for moment don't bother caching only called on startup and for whatever resaon cacahe doesn't work. 
            // once version cached no longer called.  
            return repLoader.populate<HomePageRepresentation>(new HomePageRepresentation());
        };

        context.getServices = () => {

            if (currentServices) {
                return $q.when(currentServices);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    const ds = home.getDomainServices();
                    return repLoader.populate<DomainServicesRepresentation>(ds);
                }).
                then((services: DomainServicesRepresentation) => {
                    currentServices = services;
                    return $q.when(services);
                });
        };

        context.getMenus = () => {
            if (currentMenus) {
                return $q.when(currentMenus);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    const ds = home.getMenus();
                    return repLoader.populate<MenusRepresentation>(ds);
                }).
                then((menus: MenusRepresentation) => {
                    currentMenus = menus;
                    return $q.when(currentMenus);
                });
        };

        context.getVersion = () => {

            if (currentVersion) {
                return $q.when(currentVersion);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    const v = home.getVersion();
                    return repLoader.populate<VersionRepresentation>(v);
                }).
                then((version: VersionRepresentation) => {
                    currentVersion = version;
                    return $q.when(version);
                });
        };

        context.getObject = (paneId: number, type: string, id?: string[]) => {
            const oid = _.reduce(id, (a, v) => `${a}${a ? "-" : ""}${v}`, "");
            return oid ? context.getDomainObject(paneId, type, oid) : context.getService(paneId, type);
        };

        context.getObjectByOid = (paneId : number, objectId: string) => {
            const [dt, ...id] = objectId.split("-");
            return context.getObject(paneId, dt, id);
        };

        const handleResult = (paneId : number, result: ActionResultRepresentation) => {

            if (result.resultType() === "list") {
                const resultList = result.result().list();
                context.setList(paneId, resultList);
                return $q.when(resultList);
            } else {
                return $q.reject("expect list");
            }
        }

        context.setList = (paneId : number, list: ListRepresentation) => {
            currentCollections[paneId] = list;
        }

        const getList = (paneId: number, resultPromise : () => ng.IPromise<ActionResultRepresentation>) => {
            const currentCollection = currentCollections[paneId];

            if (currentCollection) {
                // use once 
                currentCollections[paneId] = null;
                return $q.when(currentCollection);
            }

            return resultPromise().then(result => handleResult(paneId, result));
        };

        function cacheActionNameAndInvoke(paneId : number, action: ActionMember, parms: _.Dictionary<Value>) {
            lastActionFriendlyName[paneId] = action.extensions().friendlyName;
            return repLoader.invoke(action, parms);
        }

        context.getListFromMenu = (paneId: number, menuId: string, actionId: string, parms: _.Dictionary<Value>) => {
            const promise = () => context.getMenu(menuId).then(menu => cacheActionNameAndInvoke(paneId, menu.actionMember(actionId), parms));
            return getList(paneId, promise);
        };

        context.getListFromObject = (paneId: number, objectId: string, actionId: string, parms: _.Dictionary<Value>) => {
            const promise = () => context.getObjectByOid(paneId, objectId).then(object => cacheActionNameAndInvoke(paneId, object.actionMember(actionId), parms));
            return getList(paneId, promise);
        };

        context.setObject = (paneId : number, co) => currentObjects[paneId] = co;
          
        let currentError: ErrorRepresentation = null;

        context.getError = () => currentError;

        context.setError = (e: ErrorRepresentation) => currentError = e;
       
        let previousUrl: string = null;

        context.getPreviousUrl = () => previousUrl;

        context.setPreviousUrl = (url: string) => previousUrl = url;

        context.getLastActionFriendlyName = (paneId: number) => lastActionFriendlyName[paneId] || "";

        context.setLastActionFriendlyName = (fn: string, paneId: number) => lastActionFriendlyName[paneId] = fn;
       
        const createChoiceViewModels = (id: string, searchTerm: string, p: PromptRepresentation) =>
            $q.when(_.map(p.choices(), (v, k) => ChoiceViewModel.create(v, id, k, searchTerm)));

        const doPrompt = (promptRep: PromptRepresentation, id: string, searchTerm: string, setupPrompt: () => void) => {
            promptRep.reset();
            setupPrompt();
            const createcvm = <(p: PromptRepresentation) => angular.IPromise<Gemini.ChoiceViewModel[]>>(_.partial(createChoiceViewModels, id, searchTerm));
            return repLoader.populate(promptRep, true).then(createcvm);
        };

        context.prompt = (promptRep: PromptRepresentation, id: string, searchTerm: string) =>
            doPrompt(promptRep, id, searchTerm, () => promptRep.setSearchTerm(searchTerm));

        context.conditionalChoices = (promptRep: PromptRepresentation, id: string, args: IValueMap) =>
            doPrompt(promptRep, id, null, () => promptRep.setArguments(args));
      
        context.setResult = (action: ActionMember, result: ActionResultRepresentation, paneId : number, dvm?: DialogViewModel) => {
            if (result.result().isNull() && result.resultType() !== "void") {
                if (dvm) {
                    dvm.message = "no result found";
                }
                return;
            }

            const resultObject = result.result().object(); 

            // transient object
            if (result.resultType() === "object" && resultObject.persistLink()) {
                const domainType = resultObject.extensions().domainType;
                resultObject.set("domainType", domainType);
                resultObject.set("instanceId", "0");
                resultObject.hateoasUrl = `/${domainType}/0`;

                context.setObject(paneId, resultObject);
                urlManager.pushUrlState(paneId);
                urlManager.setObject(resultObject, paneId);
            }

            // persistent object
            if (result.resultType() === "object" && !resultObject.persistLink()) {

                // set the object here and then update the url. That should reload the page but pick up this object 
                // so we don't hit the server again. 

                context.setObject(paneId, resultObject);
                urlManager.setObject(resultObject, paneId);
            }

            if (result.resultType() === "list") {
                const resultList = result.result().list();
                context.setList(paneId, resultList);
                context.setLastActionFriendlyName(action.extensions().friendlyName, paneId);
                urlManager.setList(action, paneId,  dvm);
            }
        };

        context.setInvokeUpdateError = (error: any, vms: ValueViewModel[], vm?: MessageViewModel) => {
            if (error instanceof ErrorMap) {
                _.each(vms, vmi => {
                    const errorValue = (<ErrorMap>error).valuesMap()[vmi.id];

                    if (errorValue) {
                        vmi.value = errorValue.value.toValueString();
                        if (errorValue.invalidReason === "Mandatory") {
                            vmi.description = `REQUIRED ${vmi.description}`;
                        } else {
                            vmi.message = errorValue.invalidReason;
                        }                    
                    }
                });
                if (vm) {
                    vm.message = (<ErrorMap>error).invalidReason();
                }
            }
            else if (error instanceof ErrorRepresentation) {
                context.setError(error);
                urlManager.setError();
            }
            else {
                if (vm) {
                    vm.message = error;
                }
            }
        };

        context.invokeAction = (action: ActionMember, paneId : number,  dvm?: DialogViewModel) => {
            const invoke = action.getInvoke();
            let parameters: ParameterViewModel[] = [];

            if (dvm) {
                dvm.clearMessages();
                parameters = dvm.parameters;
                _.each(parameters, parm => invoke.setParameter(parm.id, parm.getValue()));

                // todo do we still need to do this ? Test
                _.each(parameters, parm => urlManager.setParameterValue(action.actionId(), parm, paneId, false));
            }

            repLoader.populate(invoke, true).
                then((result: ActionResultRepresentation) => {
                    context.setResult(action, result, paneId, dvm);
                }).
                catch((error: any) => {
                    context.setInvokeUpdateError(error, parameters, dvm);
                });
        };

        context.updateObject = (object: DomainObjectRepresentation, ovm: DomainObjectViewModel) => {
            const update = object.getUpdateMap();

            const properties = _.filter(ovm.properties, property => property.isEditable);
            _.each(properties, property => update.setProperty(property.id, property.getValue()));

            repLoader.populate(update, true, new DomainObjectRepresentation()).
                then((updatedObject: DomainObjectRepresentation) => {

                    // This is a kludge because updated object has no self link.
                    const rawLinks = (<any>object).get("links");
                    (<any>updatedObject).set("links", rawLinks);

                    // remove pre-changed object from cache
                    $cacheFactory.get("$http").remove(updatedObject.url());

                    context.setObject(ovm.onPaneId, updatedObject);
                  
                    urlManager.setObject(updatedObject, ovm.onPaneId);             
                }).
                catch((error: any) => {
                    context.setInvokeUpdateError(error, properties, ovm);
                });
        };

        context.saveObject = (object: DomainObjectRepresentation, ovm: DomainObjectViewModel, viewObject: boolean) => {
            const persist = object.getPersistMap();

            const properties = _.filter(ovm.properties, property => property.isEditable);
            _.each(properties, property => persist.setMember(property.id, property.getValue()));

            repLoader.populate(persist, true, new DomainObjectRepresentation()).
                then((updatedObject: DomainObjectRepresentation) => {
                    context.setObject(ovm.onPaneId, updatedObject);   
                
                    if (viewObject) {
                        urlManager.setObject(updatedObject, ovm.onPaneId);
                    } else {
                        urlManager.popUrlState(ovm.onPaneId);
                    }             
                }).
                catch((error: any) => {
                    context.setInvokeUpdateError(error, properties, ovm);
                });
        };

        const subTypeCache: _.Dictionary<_.Dictionary<boolean>> = {};
       
        context.isSubTypeOf = (toCheckType: string, againstType: string): ng.IPromise<boolean> => {

            if (subTypeCache[toCheckType] && typeof subTypeCache[toCheckType][againstType] !== "undefined") {
                return $q.when(subTypeCache[toCheckType][againstType]);
            }

            const isSubTypeOf = new DomainTypeActionInvokeRepresentation();

            // todo should be in model ?
            isSubTypeOf.hateoasUrl = `${appPath}/domain-types/${againstType}/type-actions/isSubtypeOf/invoke?supertype=${toCheckType}`;

            return repLoader.populate(isSubTypeOf, true).
                then((updatedObject: DomainTypeActionInvokeRepresentation) => {
                    const is = updatedObject.value();
                    const entry: _.Dictionary<boolean> = {};
                    entry[againstType] = is;
                    subTypeCache[toCheckType] = entry;
                    return is;
                }).
                catch((error: any) => {
                    return false;
                });            
        }

    });

}