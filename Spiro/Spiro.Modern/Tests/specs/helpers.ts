/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />


function spyOnPromise(tgt: Object, func: string, mock: Object) {

    var mp: any = {};

    mp.then = (f) => {
        return f(mock);
    };

    return spyOn(tgt, func).andReturn(mp);
}

function spyOnPromiseConditional(tgt: Object, func: string, mock1: Object, mock2: Object) {

    var mp: any = {};
    var first = true;

    mp.then = (f) => {
        var result = first ? f(mock1) : f(mock2);
        first = false;
        return result;
    };

    return spyOn(tgt, func).andReturn(mp);
}

function mockPromiseFail(mock: Object) {
    var mp: any = {};

    mp.then = (fok, fnok) => {
        return fnok(mock);
    };

    return mp;
}

// TODO sure this could be recursive - fix once tests are running 
function spyOnPromiseFail(tgt: Object, func: string, mock: Object) {

    var mp: any = {};

    mp.then = (fok, fnok) => {
        return fnok ? fnok(mock) : fok(mock);
    };

    return spyOn(tgt, func).andReturn(mp);
}

function spyOnPromiseNestedFail(tgt: Object, func: string, mock: Object) {

    var mp: any = {};

    mp.then = (fok) => {
        var mmp: any = {};

        mmp.then = (f1ok, f1nok) => {
            return f1nok(mock);
        };

        return mmp;
    };

    return spyOn(tgt, func).andReturn(mp);
}

function spyOnPromise2NestedFail(tgt: Object, func: string, mock: Object) {

    var mp: any = {};

    mp.then = (fok) => {
        var mmp: any = {};

        mmp.then = (f1ok) => {
            var mmmp: any = {};

            mmmp.then = (f2ok, f2nok) => {
                return f2nok(mock);
            };

            return mmmp;
        };

        return mmp;
    };

    return spyOn(tgt, func).andReturn(mp);
} 