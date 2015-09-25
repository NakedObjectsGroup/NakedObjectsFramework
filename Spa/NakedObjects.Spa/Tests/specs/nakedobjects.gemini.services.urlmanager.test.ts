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


        beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager, $location) => {
            spyOn(obj1, "domainType").and.returnValue("dt1");
            spyOn(obj1, "instanceId").and.returnValue("id1");
            spyOn(obj2, "domainType").and.returnValue("dt2");
            spyOn(obj2, "instanceId").and.returnValue("id2");

            location = $location;
        }));

        describe("on pane 1 with single pane", () => {
            let search: any = { menu1: "menu1", menu2: "menu2" };


            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {

                search = _.omit(search, "menu1");
                search.object1 = "dt1-id1";
                location.path("/home");
                location.search(search);


                urlManager.setObject(obj1, 1);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/object");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 1 with split pane", () => {
            let search: any = { menu1: "menu1", menu2: "menu2" };

            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(search, "menu1");
                search.object1 = "dt1-id1";
                location.path("/home/home");
                location.search(search);

                urlManager.setObject(obj1, 1);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/object/home");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with single pane", () => {
            let search: any = { menu1: "menu1", menu2: "menu2" };


            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(search, "menu2");
                search.object2 = "dt2-id2";
                location.path("/home");
                location.search(search);

                urlManager.setObject(obj2, 2);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/home/object");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2 with split pane", () => {
            let search: any = { menu1: "menu1", menu2: "menu2" };


            beforeEach(inject((urlManager: NakedObjects.Angular.Gemini.IUrlManager) => {
                search = _.omit(search, "menu2");
                search.object2 = "dt2-id2";
                location.path("/home/home");
                location.search(search);

                urlManager.setObject(obj2, 2);
            }));


            it("sets the path, clears other pane settings and sets the object in the search", () => {
                expect(location.path()).toBe("/home/object");
                expect(location.search()).toEqual(search);
            });
        });
    });
})