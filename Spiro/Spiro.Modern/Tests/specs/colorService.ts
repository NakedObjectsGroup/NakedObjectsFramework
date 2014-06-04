/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.angular.services.color.ts" />
/// <reference path="../../Scripts/spiro.angular.viewmodels.ts" />
/// <reference path="helpers.ts" />

describe('context Service', () => {

    beforeEach(module('app'));

    describe('toColorFromHref', () => {

        describe('with a valid object href', () => {

            var resultColor;
            var href = "http://objects/AdventureWorksModel.Product/1";

            beforeEach(inject((color: Spiro.Angular.IColor) => {
                resultColor = color.toColorFromHref(href);
            }));

            it('returns a matching color', () => {
                expect(resultColor).toBe("bg-color-orangeDark");
            });
        });

        describe('with a valid service href', () => {

            var resultColor;
            var href = "http://services/AdventureWorksModel.CustomerRepository";

            beforeEach(inject((color: Spiro.Angular.IColor) => {
                resultColor = color.toColorFromHref(href);
            }));

            it('returns a matching color', () => {
                expect(resultColor).toBe("bg-color-redLight");
            });
        });

        describe('with an invalid  href', () => {

            var resultColor;
            var href = "invalid";

            beforeEach(inject((color: Spiro.Angular.IColor) => {
                resultColor = color.toColorFromHref(href);
            }));

            it('returns a default color', () => {
                expect(resultColor).toBe("bg-color-darkBlue");
            });
        });

    });

    describe('toColorFromType', () => {

        describe('with a valid type', () => {

            var resultColor;
            var typ = "AdventureWorksModel.Product";

            beforeEach(inject((color: Spiro.Angular.IColor) => {
                resultColor = color.toColorFromType(typ);
            }));

            it('returns a matching color', () => {
                expect(resultColor).toBe("bg-color-orangeDark");
            });
        });

     
        describe('with an invalid  type', () => {

            var colorService;
            var resultColor;
            var typ = "invalid";

            beforeEach(inject((color: Spiro.Angular.IColor) => {
                colorService = color;
                resultColor = color.toColorFromType(typ);
            }));

            it('returns a default color', () => {
                expect(resultColor).toBe("bg-color-purple");
            });

            it('returns a the same default color each time', () => {

                for (var i = 0; i < 10; i++) {
                    resultColor = colorService.toColorFromType(typ);
                    expect(resultColor).toBe("bg-color-purple");
                }
            });
        });

    });

}); 