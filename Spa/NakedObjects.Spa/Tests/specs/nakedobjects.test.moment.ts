/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />


module NakedObjects.Test.Masks {
    describe("nakedobjects.gemini.tests.moment", () => {

        beforeEach(angular.mock.module("app"));

        describe("Moment", () => {

            function testFormat(toTest: string, valid: boolean, expected: Date) {

                const m = moment(toTest, supportedDateFormats, "en-GB", true);

                expect(m.isValid()).toBe(valid);

                if (valid) {
                    expect(m.toDate().toISOString()).toBe(expected.toISOString());
                }

            }


            describe("test formats", () => {
                it("formats", () => testFormat("1/1/2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("7/8/2016", true, new Date(2016, 7, 7)));
                it("formats", () => testFormat("8/7/2016", true, new Date(2016, 6, 8)));
                it("formats", () => testFormat("1/1/16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("7/8/16", true, new Date(2016, 7, 7)));
                it("formats", () => testFormat("8/7/16", true, new Date(2016, 6, 8)));
                it("formats", () => testFormat("01/01/16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("07/08/16", true, new Date(2016, 7, 7)));
                it("formats", () => testFormat("08/07/16", true, new Date(2016, 6, 8)));

                it("formats", () => testFormat("1 Jan 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 January 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 jan 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 january 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 JAN 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 JANUARY 2016", true, new Date(2016, 0, 1)));

                it("formats", () => testFormat("01 Jan 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 January 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 jan 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 january 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 JAN 2016", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 JANUARY 2016", true, new Date(2016, 0, 1)));

                it("formats", () => testFormat("1 Jan 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 January 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 jan 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 january 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 JAN 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("1 JANUARY 16", true, new Date(2016, 0, 1)));

                it("formats", () => testFormat("01 Jan 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 January 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 jan 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 january 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 JAN 16", true, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 JANUARY 16", true, new Date(2016, 0, 1)));

                // not valid 
                it("formats", () => testFormat("01 Janua 16", false, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 janu 16", false, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 januar 16", false, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 JANUA 16", false, new Date(2016, 0, 1)));
                it("formats", () => testFormat("01 JA 16", false, new Date(2016, 0, 1)));
            });


        });
    });
}