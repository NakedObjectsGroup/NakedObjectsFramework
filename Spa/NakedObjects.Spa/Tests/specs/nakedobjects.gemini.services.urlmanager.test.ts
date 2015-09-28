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

/// <reference path="../../Scripts/typings/karma-jasmine/karma-jasmine.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/nakedobjects.models.ts" />
/// <reference path="../../Scripts/nakedobjects.angular.services.color.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.viewmodels.ts" />

describe("nakedobjects.gemini.services.urlmanager", () => {

    beforeEach(angular.mock.module("app"));

    describe("setError", () => {

        let location: ng.ILocationService;

        beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
            location = $location;

            location.path("/apath");
            location.search({ search: true });

            urlManager.setError();
        }));

        it("sets an error path and clears search", () => {
            expect(location.path()).toBe("/error");
            expect(location.search()).toEqual({});
        });
    });

    describe("setMenu", () => {

        let location: ng.ILocationService;
        const search = { search: true };
        const menuId = "amenu";

        beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
            location = $location;

            location.path("/apath");
            location.search(search);
        }));

        describe("on pane 1", () => {

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search[`menu${1}`] = menuId;
                urlManager.setMenu(menuId, 1);
            }));

            it("sets the menu id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2", () => {

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search[`menu${2}`] = menuId;
                urlManager.setMenu(menuId, 2);
            }));

            it("sets the menu id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

    });

    describe("setDialog", () => {

        let location: ng.ILocationService;
        const search = { search: true };
        const dialogId = "adialog";

        beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
            location = $location;

            location.path("/apath");
            location.search(search);
        }));

        describe("on pane 1", () => {

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search[`dialog${1}`] = dialogId;
                urlManager.setDialog(dialogId, 1);
            }));


            it("sets the dialog id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2", () => {

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search[`dialog${2}`] = dialogId;
                urlManager.setDialog(dialogId, 2);
            }));

            it("sets the dialog id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

    });

    describe("closeDialog", () => {

        let location: ng.ILocationService;
        let search: any = { menu1: "menu1", menu2: "menu2", dialog1: "adialog1", dialog2: "dialog2" };

        beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
            location = $location;

            location.path("/apath");
            location.search(search);

            urlManager.closeDialog(1);
        }));

        describe("on pane 1", () => {

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(search, "dialog1");
                urlManager.closeDialog(1);
            }));


            it("clears the dialog id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2", () => {

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(search, "dialog2");
                urlManager.closeDialog(2);
            }));


            it("clears the dialog id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

    });

    describe("setObject", () => {

        let location: ng.ILocationService;
        const obj1 = new NakedObjects.DomainObjectRepresentation({});
        const obj2 = new NakedObjects.DomainObjectRepresentation({});
        const preSearch: any = { menu1: "menu1", menu2: "menu2" };

        beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
            spyOn(obj1, "domainType").and.returnValue("dt1");
            spyOn(obj1, "instanceId").and.returnValue("id1");
            spyOn(obj2, "domainType").and.returnValue("dt2");
            spyOn(obj2, "instanceId").and.returnValue("id2");

            location = $location;
        }));

        describe("on pane 1 with single pane", () => {
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu1");
                search.object1 = "dt1-id1";
                location.path("/home");
                location.search(preSearch);

                urlManager.setObject(obj1, 1);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/object");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 1 with split pane", () => {
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(preSearch, "menu1");
                search.object1 = "dt1-id1";
                location.path("/home/home");
                location.search(preSearch);

                urlManager.setObject(obj1, 1);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/object/home");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with single pane", () => {
            let search: any;


            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(preSearch, "menu2");
                search.object2 = "dt2-id2";
                location.path("/home");
                location.search(preSearch);

                urlManager.setObject(obj2, 2);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/home/object");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with split pane", () => {
            let search: any;


            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(preSearch, "menu2");
                search.object2 = "dt2-id2";
                location.path("/home/home");
                location.search(preSearch);

                urlManager.setObject(obj2, 2);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/home/object");
                expect(location.search()).toEqual(search);
            });
        });
    });

    describe("setQuery", () => {

        let location: ng.ILocationService;
        const obj = new NakedObjects.DomainObjectRepresentation();
        const menu = new NakedObjects.MenuRepresentation();

        const action11 = new NakedObjects.ActionMember({}, obj, "11");
        const action12 = new NakedObjects.ActionMember({}, menu, "12");
        const action21 = new NakedObjects.ActionMember({}, obj, "21");
        const action22 = new NakedObjects.ActionMember({}, menu, "22");

        beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
            spyOn(obj, "domainType").and.returnValue("dt");
            spyOn(obj, "instanceId").and.returnValue("id");
            spyOn(menu, "menuId").and.returnValue("mid");
          
            location = $location;
        }));

        describe("on pane 1 with single pane parent object", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu1");
                search.object1 = "dt-id";
                search.action1 = "11";
                location.path("/home");
                location.search(preSearch);
            
                urlManager.setQuery(action11, 1);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/query");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 1 with single pane parent menu", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu1");
                search.menu1 = "mid";
                search.action1 = "12";
                location.path("/home");
                location.search(preSearch);

                urlManager.setQuery(action12, 1);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/query");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 1 with split pane parent object", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu1");
                search.object1 = "dt-id";
                search.action1 = "11";
                location.path("/home/home");
                location.search(preSearch);

                urlManager.setQuery(action11, 1);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/query/home");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 1 with split pane parent menu", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu1");
                search.menu1 = "mid";
                search.action1 = "12";
                location.path("/home/home");
                location.search(preSearch);

                urlManager.setQuery(action12, 1);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/query/home");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 1 with single pane parent object and dvm", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            const dvm = new NakedObjects.Angular.Gemini.DialogViewModel(); 
            let search: any;

            const p1 = new NakedObjects.Angular.Gemini.ParameterViewModel();
            const p2 = new NakedObjects.Angular.Gemini.ParameterViewModel();
            p1.id = "pid1";
            p1.value = "val1";
            p1.type = "scalar";
            p2.id = "pid2";
            p2.value = "val2";
            p2.type = "scalar";

            dvm.parameters = [p1, p2];

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu1");
                search.object1 = "dt-id";
                search.action1 = "11";
                search.parm1_pid1 = "val1";
                search.parm1_pid2 = "val2";

                location.path("/home");
                location.search(preSearch);

                urlManager.setQuery(action11, 1, dvm);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/query");
                expect(location.search()).toEqual(search);
            });
        });



        describe("on pane 2 with single pane parent object", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu2");
                search.object2 = "dt-id";
                search.action2 = "21";
                location.path("/home");
                location.search(preSearch);

                urlManager.setQuery(action21, 2);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/home/query");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with single pane parent menu", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu2");
                search.menu2 = "mid";
                search.action2 = "22";
                location.path("/home");
                location.search(preSearch);

                urlManager.setQuery(action22, 2);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/home/query");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with split pane parent object", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu2");
                search.object2 = "dt-id";
                search.action2 = "21";
                location.path("/home/home");
                location.search(preSearch);

                urlManager.setQuery(action21, 2);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/home/query");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with split pane parent menu", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            let search: any;

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu2");
                search.menu2 = "mid";
                search.action2 = "22";
                location.path("/home/home");
                location.search(preSearch);

                urlManager.setQuery(action22, 2);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/home/query");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with single pane parent object and dvm", () => {
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            const dvm = new NakedObjects.Angular.Gemini.DialogViewModel();
            let search: any;

            const p1 = new NakedObjects.Angular.Gemini.ParameterViewModel();
            const p2 = new NakedObjects.Angular.Gemini.ParameterViewModel();
            p1.id = "pid1";
            p1.value = "val1";
            p1.type = "scalar";
            p2.id = "pid2";
            p2.value = "val2";
            p2.type = "scalar";

            dvm.parameters = [p1, p2];

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(preSearch, "menu2");
                search.object2 = "dt-id";
                search.action2 = "21";
                search.parm2_pid1 = "val1";
                search.parm2_pid2 = "val2";

                location.path("/home");
                location.search(preSearch);

                urlManager.setQuery(action21, 2, dvm);
            }));


            it("sets the path, clears other pane settings and sets the object and action in the search", () => {
                expect(location.path()).toBe("/home/query");
                expect(location.search()).toEqual(search);
            });
        });

        describe("setProperty", () => {

            let location: ng.ILocationService;
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            const property = new NakedObjects.PropertyMember({}, {}, "1");

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
                location = $location;

                spyOn(property, "value").and.returnValue({ link : () => { return { href: () => "objects/dt/id" } } });

                location.path("/home");
                location.search(preSearch);
            }));

            describe("on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                    search = _.omit(preSearch, "menu1");
                    search.object1 = "dt-id";

                    urlManager.setProperty(property, 1);
                }));

                it("sets the property id in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                    search = _.omit(preSearch, "menu2");
                    search.object2 = "dt-id";
                    urlManager.setProperty(property, 2);
                }));

                it("sets the property id in the search", () => {
                    expect(location.path()).toBe("/home/object");
                    expect(location.search()).toEqual(search);
                });
            });

        });

        describe("setItem", () => {
                  
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            const link = new NakedObjects.Link({});

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
                location = $location;

                spyOn(link, "href").and.returnValue( "objects/dt/id" );

                location.path("/home");
                location.search(preSearch);
            }));

            describe("on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                    search = _.omit(preSearch, "menu1");
                    search.object1 = "dt-id";

                    urlManager.setItem(link, 1);
                }));

                it("sets the property id in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                    search = _.omit(preSearch, "menu2");
                    search.object2 = "dt-id";
                    urlManager.setItem(link, 2);
                }));

                it("sets the property id in the search", () => {
                    expect(location.path()).toBe("/home/object");
                    expect(location.search()).toEqual(search);
                });
            });

        });

        describe("toggleObjectMenu", () => {

            let location: ng.ILocationService;
            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
      
            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
                location = $location;

                location.path("/home");
                location.search(preSearch);
            }));

            describe("on pane 1 toggle on", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);
                    search.actions1 = "open";

                    urlManager.toggleObjectMenu(1);
                }));

                it("sets the menu id in the search", () => {
                    expect(location.path()).toBe("/home");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("on pane 1 toggle off", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    preSearch.actions1 = "open";
                    location.search(preSearch);

                    urlManager.toggleObjectMenu(1);
                }));

                it("sets the menu id in the search", () => {
                    expect(location.path()).toBe("/home");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("on pane 2 toggle on", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);
                    search.actions2 = "open";

                    urlManager.toggleObjectMenu(2);
                }));

                it("sets the menu id in the search", () => {
                    expect(location.path()).toBe("/home");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("on pane 2 toggle off", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    preSearch.actions2 = "open";
                    location.search(preSearch);

                    urlManager.toggleObjectMenu(2);
                }));

                it("sets the menu id in the search", () => {
                    expect(location.path()).toBe("/home");
                    expect(location.search()).toEqual(search);
                });
            });

        });

        describe("setQueryState", () => {

            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
          

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
                location = $location;   

                location.path("/object");
                location.search(preSearch);
            }));

            describe("summary on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection1 = "Summary";

                    urlManager.setQueryState(1, NakedObjects.Angular.Gemini.CollectionViewState.Summary);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("list on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection1 = "List";

                    urlManager.setQueryState(1, NakedObjects.Angular.Gemini.CollectionViewState.List);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("table on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection1 = "Table";

                    urlManager.setQueryState(1, NakedObjects.Angular.Gemini.CollectionViewState.Table);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("summary on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection2 = "Summary";

                    urlManager.setQueryState(2, NakedObjects.Angular.Gemini.CollectionViewState.Summary);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("list on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection2 = "List";

                    urlManager.setQueryState(2, NakedObjects.Angular.Gemini.CollectionViewState.List);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("table on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection2 = "Table";

                    urlManager.setQueryState(2, NakedObjects.Angular.Gemini.CollectionViewState.Table);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });
        });

        describe("setCollectionMemberState", () => {

            const preSearch: any = { menu1: "menu1", menu2: "menu2" };
            const collectionMember = new NakedObjects.CollectionMember({}, {}, "aName");

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
                location = $location;

                location.path("/object");
                location.search(preSearch);
            }));

            describe("summary on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection1_aName = "Summary";

                    urlManager.setCollectionMemberState(1, collectionMember,  NakedObjects.Angular.Gemini.CollectionViewState.Summary);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("list on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection1_aName = "List";

                    urlManager.setCollectionMemberState(1, collectionMember, NakedObjects.Angular.Gemini.CollectionViewState.List);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("table on pane 1", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection1_aName = "Table";

                    urlManager.setCollectionMemberState(1, collectionMember, NakedObjects.Angular.Gemini.CollectionViewState.Table);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("summary on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection2_aName = "Summary";

                    urlManager.setCollectionMemberState(2, collectionMember, NakedObjects.Angular.Gemini.CollectionViewState.Summary);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("list on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection2_aName = "List";

                    urlManager.setCollectionMemberState(2, collectionMember, NakedObjects.Angular.Gemini.CollectionViewState.List);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

            describe("table on pane 2", () => {

                let search: any;

                beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                    search = _.clone(preSearch);

                    search.collection2_aName = "Table";

                    urlManager.setCollectionMemberState(2, collectionMember, NakedObjects.Angular.Gemini.CollectionViewState.Table);
                }));

                it("sets the collection state in the search", () => {
                    expect(location.path()).toBe("/object");
                    expect(location.search()).toEqual(search);
                });
            });

        });
    });
})