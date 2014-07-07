/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.angular.services.color.ts" />
/// <reference path="../../Scripts/spiro.modern.viewmodels.ts" />
/// <reference path="helpers.ts" />
describe('context Service', function () {
    beforeEach(module('app'));

    describe('toColorFromHref', function () {
        describe('with a valid object href', function () {
            var resultColor;
            var href = "http://objects/AdventureWorksModel.Product/1";

            beforeEach(inject(function (color) {
                resultColor = color.toColorFromHref(href);
            }));

            it('returns a matching color', function () {
                expect(resultColor).toBe("bg-color-orangeDark");
            });
        });

        describe('with a valid service href', function () {
            var resultColor;
            var href = "http://services/AdventureWorksModel.CustomerRepository";

            beforeEach(inject(function (color) {
                resultColor = color.toColorFromHref(href);
            }));

            it('returns a matching color', function () {
                expect(resultColor).toBe("bg-color-redLight");
            });
        });

        describe('with an invalid  href', function () {
            var resultColor;
            var href = "invalid";

            beforeEach(inject(function (color) {
                resultColor = color.toColorFromHref(href);
            }));

            it('returns a default color', function () {
                expect(resultColor).toBe("bg-color-darkBlue");
            });
        });
    });

    describe('toColorFromType', function () {
        describe('with a valid type', function () {
            var resultColor;
            var typ = "AdventureWorksModel.Product";

            beforeEach(inject(function (color) {
                resultColor = color.toColorFromType(typ);
            }));

            it('returns a matching color', function () {
                expect(resultColor).toBe("bg-color-orangeDark");
            });
        });

        describe('with an invalid  type', function () {
            var colorService;
            var resultColor;
            var typ = "invalid";

            beforeEach(inject(function (color) {
                colorService = color;
                resultColor = color.toColorFromType(typ);
            }));

            it('returns a default color', function () {
                expect(resultColor).toBe("bg-color-purple");
            });

            it('returns a the same default color each time', function () {
                for (var i = 0; i < 10; i++) {
                    resultColor = colorService.toColorFromType(typ);
                    expect(resultColor).toBe("bg-color-purple");
                }
            });
        });
    });
});
//# sourceMappingURL=colorService.js.map
