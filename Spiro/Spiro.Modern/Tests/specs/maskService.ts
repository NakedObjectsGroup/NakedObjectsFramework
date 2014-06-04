/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.angular.services.color.ts" />
/// <reference path="../../Scripts/spiro.angular.viewmodels.ts" />
/// <reference path="helpers.ts" />

describe('mask Service', () => {

    beforeEach(module('app'));

    describe('toLocalFilter', () => {

        describe('with a known mask', () => {

            var resultMask: Spiro.Angular.ILocalFilter; 

            beforeEach(inject((mask: Spiro.Angular.IMask) => {
                resultMask = mask.toLocalFilter("d");
            }));

            it('returns a matching mask', () => {
                expect(resultMask.name).toBe("date");
                expect(resultMask.mask).toBe("d MMM yyyy");
            });
        });

        describe('with a unknown mask', () => {

            var resultMask: Spiro.Angular.ILocalFilter;

            beforeEach(inject((mask: Spiro.Angular.IMask) => {
                resultMask = mask.toLocalFilter("f");
            }));

            it('returns a undefined mask', () => {
                expect(resultMask).toBeUndefined();
            });
        });
    });
}); 