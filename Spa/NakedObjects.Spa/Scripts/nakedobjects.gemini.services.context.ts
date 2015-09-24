//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export interface IContext {
        getHome: () => ng.IPromise<HomePageRepresentation>;
        getVersion: () => ng.IPromise<VersionRepresentation>;
        getMenus: () => ng.IPromise<MenusRepresentation>;
        getMenu: (menuId: string) => ng.IPromise<MenuRepresentation>;

        getObject: (paneId : number, type: string, id?: string[]) => ng.IPromise<DomainObjectRepresentation>;
        getObjectByOid: (paneId: number, objectId : string) => ng.IPromise<DomainObjectRepresentation>;
        
        getError: () => ErrorRepresentation;
      
        getPreviousUrl: () => string;
        
        getSelectedChoice: (parm: string, search: string) => ChoiceViewModel[];
       
        getQuery: (paneId : number, menuId: string, actionId: string, parms : {id :string, val : string }[]) => angular.IPromise<ListRepresentation>;
        getQueryFromObject: (paneId : number, objectId: string, actionId: string, parms: { id: string, val: string }[]) => angular.IPromise<ListRepresentation>;
        getLastActionFriendlyName : () => string;
      

        prompt(promptRep: PromptRepresentation, id: string, searchTerm: string): ng.IPromise<ChoiceViewModel[]>;
        conditionalChoices(promptRep: PromptRepresentation, id: string, args: IValueMap): ng.IPromise<ChoiceViewModel[]>;
      
        invokeAction(action: ActionMember, paneId : number, dvm?: DialogViewModel);
        updateObject(object: DomainObjectRepresentation, ovm: DomainObjectViewModel);
        saveObject(object: DomainObjectRepresentation, ovm: DomainObjectViewModel);

        setError: (object: ErrorRepresentation) => void;
        clearSelectedChoice: (parm: string) => void;
        setSelectedChoice: (parm: string, search: string, cvm: ChoiceViewModel) => void;
    }

    interface IContextInternal extends IContext {
        getDomainObject: (paneId : number, type: string, id: string) => ng.IPromise<DomainObjectRepresentation>;
        getServices: () => ng.IPromise<DomainServicesRepresentation>;
        getService: (paneId: number, type: string) => ng.IPromise<DomainObjectRepresentation>;

        setObject: (paneId : number, object: DomainObjectRepresentation) => void;
           
        setLastActionFriendlyName: (fn : string) => void;
        setQuery(paneId : number, listRepresentation: ListRepresentation);
        setResult(action: ActionMember, result: ActionResultRepresentation, paneId : number, dvm?: DialogViewModel);
        setInvokeUpdateError(error: any, vms: ValueViewModel[], vm?: MessageViewModel);
        setPreviousUrl: (url: string) => void;
    }

    app.service("context", function ($q: ng.IQService, repLoader: IRepLoader, urlManager : IUrlManager, $cacheFactory: ng.ICacheFactoryService) {
        const context = <IContextInternal>this;

        // cached values
        let currentHome: HomePageRepresentation = null;
        const currentObjects: DomainObjectRepresentation[] = []; // per pane 
        const currentMenuList: { [index: string]: MenuRepresentation } = {};
        let currentServices: DomainServicesRepresentation = null;
        let currentMenus: MenusRepresentation = null;
        let currentVersion: VersionRepresentation = null;
        const currentCollections:  ListRepresentation[] = []; // per pane 
        let lastActionFriendlyName: string = "";

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

            if (currentHome) {
                return $q.when(currentHome);
            }

            return repLoader.populate<HomePageRepresentation>(new HomePageRepresentation()).
                then((home: HomePageRepresentation) => {
                    currentHome = home;
                    return $q.when(home);
                });
        };

        context.getServices = () => {

            if (currentServices) {
                return $q.when(currentServices);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    var ds = home.getDomainServices();
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
                context.setQuery(paneId, resultList);
                return $q.when(resultList);
            } else {
                return $q.reject("expect list");
            }
        }

        context.setQuery = (paneId : number, query: ListRepresentation) => {
            currentCollections[paneId] = query;
        }

        context.getQuery = (paneId : number, menuId: string, actionId: string, parms : {id: string; val: string }[]) => {
            const currentCollection = currentCollections[paneId];

            if (currentCollection) {
                // use once 
                currentCollections[paneId] = null;

                return $q.when (currentCollection);
            }

            return context.getMenu(menuId).
                then((menu: MenuRepresentation) => {
                    const action = menu.actionMember(actionId);               
                    const valueParms = _.map(parms, (p) => { return { id: p.id, val: new Value(p.val) } });
                    lastActionFriendlyName = action.extensions().friendlyName;
                    return repLoader.invoke(action, valueParms);
                }).
                then((result : ActionResultRepresentation) => handleResult(paneId, result) );
        };

        context.getQueryFromObject = (paneId : number, objectId: string, actionId: string, parms: { id: string; val: string }[]) => {

            const currentCollection = currentCollections[paneId];

            if (currentCollection) {
                // use once 
                currentCollections[paneId] = null;
                return $q.when(currentCollection);
            }

            return context.getObjectByOid(paneId, objectId).
                then((object: DomainObjectRepresentation) => {
                    const action = object.actionMember(actionId);
                    const valueParms = _.map(parms, (p) => { return { id: p.id, val: new Value(p.val) } });
                    lastActionFriendlyName = action.extensions().friendlyName;

                    return repLoader.invoke(action, valueParms);
                }).
                then((result: ActionResultRepresentation) => handleResult(paneId, result));
            
        };

        context.setObject = (paneId : number, co) => currentObjects[paneId] = co;
          
        var currentError: ErrorRepresentation = null;

        context.getError = () => currentError;

        context.setError = (e: ErrorRepresentation) => currentError = e;
       

        var previousUrl: string = null;

        context.getPreviousUrl = () => previousUrl;

        context.setPreviousUrl = (url: string) => previousUrl = url;

        var selectedChoice: { [parm: string]: { [search: string]: ChoiceViewModel[] } } = {};

        context.getSelectedChoice = (parm: string, search: string) => selectedChoice[parm] ? selectedChoice[parm][search] : [];

        context.setSelectedChoice = (parm: string, search: string, cvm: ChoiceViewModel) => {
            selectedChoice[parm] = selectedChoice[parm] || {};
            selectedChoice[parm][search] = selectedChoice[parm][search] || [];
            selectedChoice[parm][search].push(cvm);
        };

        context.clearSelectedChoice = (parm: string) => selectedChoice[parm] = null;

        context.getLastActionFriendlyName = () => {
            return lastActionFriendlyName;
        };
        context.setLastActionFriendlyName = (fn : string) => {
            lastActionFriendlyName = fn;
        };

        // from rh

        const createChoiceViewModels = (id: string, searchTerm: string, p: PromptRepresentation) => {
            const delay = $q.defer<ChoiceViewModel[]>();

            const cvms = _.map(p.choices(), (v, k) => {
                return ChoiceViewModel.create(v, id, k, searchTerm);
            });

            delay.resolve(cvms);
            return delay.promise;
        }

        context.prompt = (promptRep: PromptRepresentation, id: string, searchTerm: string): ng.IPromise<ChoiceViewModel[]> => {
            promptRep.reset();
            promptRep.setSearchTerm(searchTerm);
            const createcvm = <(p: PromptRepresentation) => angular.IPromise<Gemini.ChoiceViewModel[]>>(_.partial(createChoiceViewModels, id, searchTerm));
            return repLoader.populate(promptRep, true).then(createcvm);
        };

        context.conditionalChoices = (promptRep: PromptRepresentation, id: string, args: IValueMap): ng.IPromise<ChoiceViewModel[]> => {
            promptRep.reset();
            promptRep.setArguments(args);
            const createcvm = <(p: PromptRepresentation) => angular.IPromise<Gemini.ChoiceViewModel[]>>(_.partial(createChoiceViewModels, id, null));
            return repLoader.populate(promptRep, true).then(createcvm);
        };

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
                context.setQuery(paneId, resultList);
                context.setLastActionFriendlyName(action.extensions().friendlyName);
                urlManager.setQuery(action, paneId,  dvm);
            }
        };

        context.setInvokeUpdateError = (error: any, vms: ValueViewModel[], vm?: MessageViewModel) => {
            if (error instanceof ErrorMap) {
                _.each(vms, vmi => {
                    var errorValue = error.valuesMap()[vmi.id];

                    if (errorValue) {
                        vmi.value = errorValue.value.toValueString();
                        vmi.message = errorValue.invalidReason;
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
                _.each(parameters, (parm) => invoke.setParameter(parm.id, parm.getValue()));
                _.each(parameters, (parm) => parm.setSelectedChoice());
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

        context.saveObject = (object: DomainObjectRepresentation, ovm: DomainObjectViewModel) => {
            const persist = object.getPersistMap();

            const properties = _.filter(ovm.properties, property => property.isEditable);
            _.each(properties, property => persist.setMember(property.id, property.getValue()));

            repLoader.populate(persist, true, new DomainObjectRepresentation()).
                then((updatedObject: DomainObjectRepresentation) => {
                    urlManager.popUrlState(ovm.onPaneId);
                    context.setObject(ovm.onPaneId, updatedObject);                
                }).
                catch((error: any) => {
                    context.setInvokeUpdateError(error, properties, ovm);
                });
        };

    });

}