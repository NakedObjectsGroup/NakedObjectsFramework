/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.models.ts" />
/// <reference path="../../Scripts/spiro.angular.services.color.ts" />
/// <reference path="../../Scripts/spiro.angular.viewmodels.ts" />
/// <reference path="helpers.ts" />

describe('viewModelFactory Service', () => {

    beforeEach(module('app'));

    describe('errorViewModel', () => {
        var resultVm: Spiro.Angular.ErrorViewModel;
        var rawError = { message: "a message" };
        var errorRep = new Spiro.ErrorRepresentation(rawError);
      
        beforeEach(inject((viewModelFactory: Spiro.Angular.IViewModelFactory) => {
            resultVm = viewModelFactory.errorViewModel(errorRep);
        }));

        it('creates a error view model', () => {
            expect(resultVm.message).toBe("a message");
        });
    });
}); 