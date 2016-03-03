/// <reference path="../../Scripts/nakedobjects.config.ts" />
/// <reference path="../../Scripts/nakedobjects.models.helpers.ts" />
/// <reference path="../../Scripts/nakedobjects.models.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.app.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.controllers.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.directives.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.routedata.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.context.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.handlers.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.navigation.browser.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.navigation.simple.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.urlmanager.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.focusmanager.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.services.viewmodelfactory.ts" />
/// <reference path="nakedobjects.gemini.test.helpers.ts" />

// todo make sure tests are enhanced to validate contents of view models and urls
// todo tests for invalid url combinations ? 

module NakedObjects.Gemini.Test.Masks {
    import IMask = Angular.IMask;
    describe("nakedobjects.gemini.tests.masks", () => {

        beforeEach(angular.mock.module("app"));

        describe("Mask", () => {

            let maskService: IMask;

            beforeEach(inject((mask: IMask) => {
                maskService = mask;
            }));


            function testMask(input: any, mask: string, format: string, expectedResult: string) {
                const result = maskService.toLocalFilter(mask, format).filter(input);
                expect(result).toBe(expectedResult);
            }

            function testDefaultMask(input: any, format: string, expectedResult: string) {
                testMask(input, "", format, expectedResult);
            }

            describe("default string", () => {
                it("masks empty", () => testDefaultMask("", "string", ""));
                it("masks null", () => testDefaultMask(null, "string", ""));
                it("masks a string", () => testDefaultMask("a string", "string", "a string"));
            });

            describe("default int", () => {
                it("masks empty", () => testDefaultMask("", "int", "0"));
                it("masks null", () => testDefaultMask(null, "int", null));
                it("masks 0", () => testDefaultMask(0, "int", "0"));
                it("masks 101", () => testDefaultMask(101, "int", "101"));
                it("masks 1002", () => testDefaultMask(1002, "int", "1,002"));
                it("masks 10003", () => testDefaultMask(10003, "int", "10,003"));
                it("masks max int", () => testDefaultMask(Number.MAX_VALUE, "int", "1.7976931348623157e+308"));
            });

            describe("default date-time", () => {
                it("masks empty", () => testDefaultMask("", "date-time", ""));
                it("masks null", () => testDefaultMask(null, "date-time", null));
            });


        });


    });
}