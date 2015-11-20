/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export interface IContext {
        
        getCachedList: (paneId : number, page : number, pageSize : number) => ListRepresentation; 

        getVersion: () => ng.IPromise<VersionRepresentation>;
        getMenus: () => ng.IPromise<MenusRepresentation>;
        getMenu: (menuId: string) => ng.IPromise<MenuRepresentation>;
        getObject: (paneId : number, type: string, id?: string[]) => ng.IPromise<DomainObjectRepresentation>;
        getObjectByOid: (paneId: number, objectId : string) => ng.IPromise<DomainObjectRepresentation>;    
        getListFromMenu: (paneId : number, menuId: string, actionId: string, parms : _.Dictionary<Value>, page? : number, pageSize? : number) => angular.IPromise<ListRepresentation>;
        getListFromObject: (paneId: number, objectId: string, actionId: string, parms: _.Dictionary<Value>, page? : number, pageSize?: number) => angular.IPromise<ListRepresentation>;

        getActionFriendlyName: (action : ActionMember) => ng.IPromise<string>;
        getError: () => ErrorRepresentation;
        getPreviousUrl: () => string;

        prompt(promptRep: PromptRepresentation, id: string, searchTerm: string): ng.IPromise<ChoiceViewModel[]>;
        conditionalChoices(promptRep: PromptRepresentation, id: string, args: IValueMap): ng.IPromise<ChoiceViewModel[]>;
      
        invokeAction(action: ActionMember, paneId : number, ovm? : DomainObjectViewModel, dvm?: DialogViewModel);
        updateObject(object: DomainObjectRepresentation, ovm: DomainObjectViewModel);
        saveObject(object: DomainObjectRepresentation, ovm: DomainObjectViewModel, viewObject: boolean);
        reloadObject: (paneId : number, object: DomainObjectRepresentation) => angular.IPromise<DomainObjectRepresentation>;

        setError: (object: ErrorRepresentation) => void;
    
        isSubTypeOf(toCheckType: string, againstType: string): ng.IPromise<boolean>;

        getCiceroVM(): CiceroViewModel;
        setCiceroVMIfNecessary(cf: ICommandFactory);

        getActionFriendlyNameFromMenu: (menuId: string, actionId: string) => angular.IPromise<string>;
        getActionFriendlyNameFromObject: (paneId: number, objectId: string, actionId: string) => angular.IPromise<string>;
    }

    interface IContextInternal extends IContext {
        getHome: () => ng.IPromise<HomePageRepresentation>;
        getDomainObject: (paneId : number, type: string, id: string) => ng.IPromise<DomainObjectRepresentation>;
        getServices: () => ng.IPromise<DomainServicesRepresentation>;
        getService: (paneId: number, type: string) => ng.IPromise<DomainObjectRepresentation>;

        setObject: (paneId : number, object: DomainObjectRepresentation) => void;          
       
        setResult(action: ActionMember, result: ActionResultRepresentation, paneId : number, page : number, pageSize : number,   dvm?: DialogViewModel);
        setInvokeUpdateError(error: any, vms: ValueViewModel[], vm?: MessageViewModel);
        setPreviousUrl: (url: string) => void;
        
    }

    app.service("context", function ($q: ng.IQService,
        repLoader: IRepLoader,
        urlManager: IUrlManager,
        $cacheFactory: ng.ICacheFactoryService) {
        const context = <IContextInternal>this;

        // cached values
       
        const currentObjects: DomainObjectRepresentation[] = []; // per pane 
        const currentMenuList: _.Dictionary<MenuRepresentation> = {};
        let currentServices: DomainServicesRepresentation = null;
        let currentMenus: MenusRepresentation = null;
        let currentVersion: VersionRepresentation = null;
     
        let currentLists: _.Dictionary<ListRepresentation>[] = []; // cache last 'listCacheSize' lists

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

        context.reloadObject = (paneId: number, object: DomainObjectRepresentation) => {

            const reloadedObject = new DomainObjectRepresentation();
            reloadedObject.hateoasUrl = getAppPath() + "/objects/" + object.domainType() + "/" + object.instanceId();

            return repLoader.populate<DomainObjectRepresentation>(reloadedObject, true).
                then((obj: DomainObjectRepresentation) => {
                    currentObjects[paneId] = obj;
                    return $q.when(obj);
                });
        }

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

        function cacheList(list: ListRepresentation, index: string) {
            if (currentLists.length >= listCacheSize) {
                const firstIndex = (currentLists.length - listCacheSize) + 1;
                currentLists = currentLists.slice(firstIndex);
            }

            const entry: _.Dictionary<ListRepresentation> = {};
            entry[index] = list;
            currentLists.push(entry);
        }


        const handleResult = (paneId : number, result: ActionResultRepresentation, page : number, pageSize : number) => {

            if (result.resultType() === "list") {
                const resultList = result.result().list();
                const index = urlManager.getListCacheIndex(paneId, page, pageSize);
                cacheList(resultList, index);
                return $q.when(resultList);
            } else {
                return $q.reject("expect list");
            }
        }


        const getList = (paneId: number, resultPromise : () => ng.IPromise<ActionResultRepresentation>, page : number, pageSize : number) => {
            return resultPromise().then(result => handleResult(paneId, result, page, pageSize));
        };

        context.getActionFriendlyNameFromMenu = (menuId: string, actionId: string) =>
            context.getMenu(menuId).then(menu => $q.when(menu.actionMember(actionId).extensions().friendlyName));

        context.getActionFriendlyNameFromObject = (paneId: number, objectId: string, actionId: string) =>
            context.getObjectByOid(paneId, objectId).then(object => $q.when(object.actionMember(actionId).extensions().friendlyName));

        function getPagingParms(page: number, pageSize: number): _.Dictionary<string> {
            return (page && pageSize) ? { "x-ro-page": page.toString(), "x-ro-pageSize": pageSize.toString() } : {};
        }

        context.getListFromMenu = (paneId: number, menuId: string, actionId: string, parms: _.Dictionary<Value>, page?: number, pageSize?: number) => {
            const urlParms = getPagingParms(page, pageSize);
            const promise = () => context.getMenu(menuId).then(menu => repLoader.invoke(menu.actionMember(actionId), parms, urlParms));
            return getList(paneId, promise, page, pageSize);
        };

        context.getListFromObject = (paneId: number, objectId: string, actionId: string, parms: _.Dictionary<Value>, page?: number, pageSize?: number) => {
            const urlParms = getPagingParms(page, pageSize);
            const promise = () => context.getObjectByOid(paneId, objectId).then(object => repLoader.invoke(object.actionMember(actionId), parms, urlParms));
            return getList(paneId, promise, page, pageSize);
        };

        context.setObject = (paneId : number, co) => currentObjects[paneId] = co;
          
        let currentError: ErrorRepresentation = null;

        context.getError = () => currentError;

        context.setError = (e: ErrorRepresentation) => currentError = e;
       
        let previousUrl: string = null;

        context.getPreviousUrl = () => previousUrl;

        context.setPreviousUrl = (url: string) => previousUrl = url;
       
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
      
        context.setResult = (action: ActionMember, result: ActionResultRepresentation, paneId : number, page : number, pageSize : number , dvm?: DialogViewModel) => {
            if (result.result().isNull() && result.resultType() !== "void") {
                if (dvm) {
                    dvm.message = "no result found";
                }
                return;
            }

            const resultObject = result.result().object(); 
          
            if (result.resultType() === "object") {
                if (resultObject.persistLink()) {
                    // transient object
                    const domainType = resultObject.extensions().domainType;
                    resultObject.set("domainType", domainType);
                    resultObject.set("instanceId", "0");
                    resultObject.hateoasUrl = `/${domainType}/0`;

                    context.setObject(paneId, resultObject);
                    urlManager.pushUrlState(paneId);
                    urlManager.setObject(resultObject, paneId);
                }
                else {

                    // persistent object
                    // set the object here and then update the url. That should reload the page but pick up this object 
                    // so we don't hit the server again. 

                    context.setObject(paneId, resultObject);
                    urlManager.setObject(resultObject, paneId);
                }
            }

            else if (result.resultType() === "list") {
                const resultList = result.result().list();
               
                urlManager.setList(action, paneId, dvm);
    
                const index = urlManager.getListCacheIndex(paneId, page, pageSize);
                cacheList(resultList, index);
            } else if (dvm) {
                urlManager.closeDialog(dvm.onPaneId);
            }
        };

        context.getCachedList = (paneId : number, page: number, pageSize: number) => {
            const index = urlManager.getListCacheIndex(paneId, page, pageSize);
            const e = _.find(currentLists, entry => entry[index]);
            return e ? e[index] : null;
        }

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

     
        context.invokeAction = (action: ActionMember, paneId : number, ovm? : DomainObjectViewModel, dvm?: DialogViewModel) => {
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
                    // todo hard coded page size 
                    context.setResult(action, result, paneId, 1, 20, dvm);
                    
                    if (ovm) {
                        const actionIsNotQueryOnly = action.invokeLink().method() !== "GET";
                        if (actionIsNotQueryOnly) {
                            ovm.doReload();
                        }
                    }
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

        let cachedCvm: CiceroViewModel = null;

        context.setCiceroVMIfNecessary = (cf: ICommandFactory) => {
            if (cachedCvm == null) {
                cachedCvm = new CiceroViewModel();
                cachedCvm.parseInput = (input: string) => {
                    cf.parseInput(input);
                };
            }
            return cachedCvm;
        };

        context.getCiceroVM = () => {
            return cachedCvm;
        };
    });

}