/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.angular.services.color.ts" />
/// <reference path="../../Scripts/spiro.modern.viewmodels.ts" />
/// <reference path="helpers.ts" />
describe('mask Service', function () {
    beforeEach(module('app'));

    describe('toLocalFilter', function () {
        describe('with a known mask', function () {
            var resultMask;

            beforeEach(inject(function (mask) {
                resultMask = mask.toLocalFilter("d");
            }));

            it('returns a matching mask', function () {
                expect(resultMask.name).toBe("date");
                expect(resultMask.mask).toBe("d MMM yyyy");
            });
        });

        describe('with a unknown mask', function () {
            var resultMask;

            beforeEach(inject(function (mask) {
                resultMask = mask.toLocalFilter("f");
            }));

            it('returns a undefined mask', function () {
                expect(resultMask).toBeUndefined();
            });
        });
    });
});
//# sourceMappingURL=maskService.js.map
