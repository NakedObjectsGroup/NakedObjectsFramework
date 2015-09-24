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

        beforeEach(inject((urlManager, $location) => {
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

        beforeEach(inject((urlManager, $location) => {
            location = $location;

            location.path("/apath");
            location.search(search);                  
        }));

        describe("on pane 1", () => {

            beforeEach(inject((urlManager, $location) => {
                search[`menu${1}`] = menuId;    
                urlManager.setMenu(menuId, 1);
            }));


            it("sets the menu id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2", () => {

            beforeEach(inject((urlManager, $location) => {
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

        beforeEach(inject((urlManager, $location) => {
            location = $location;

            location.path("/apath");
            location.search(search);
        }));

        describe("on pane 1", () => {

            beforeEach(inject((urlManager, $location) => {
                search[`dialog${1}`] = dialogId;
                urlManager.setDialog(dialogId, 1);
            }));


            it("sets the menu id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

        describe("on pane 2", () => {

            beforeEach(inject((urlManager, $location) => {
                search[`dialog${2}`] = dialogId;
                urlManager.setDialog(dialogId, 2);
            }));


            it("sets the menu id in the search", () => {
                expect(location.path()).toBe("/apath");
                expect(location.search()).toEqual(search);
            });
        });

    });


})