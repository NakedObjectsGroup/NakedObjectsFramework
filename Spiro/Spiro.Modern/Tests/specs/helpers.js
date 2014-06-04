/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
function spyOnPromise(tgt, func, mock) {
    var mp = {};

    mp.then = function (f) {
        return f(mock);
    };

    return spyOn(tgt, func).andReturn(mp);
}

function spyOnPromiseConditional(tgt, func, mock1, mock2) {
    var mp = {};
    var first = true;

    mp.then = function (f) {
        var result = first ? f(mock1) : f(mock2);
        first = false;
        return result;
    };

    return spyOn(tgt, func).andReturn(mp);
}

function mockPromiseFail(mock) {
    var mp = {};

    mp.then = function (fok, fnok) {
        return fnok(mock);
    };

    return mp;
}

// TODO sure this could be recursive - fix once tests are running
function spyOnPromiseFail(tgt, func, mock) {
    var mp = {};

    mp.then = function (fok, fnok) {
        return fnok ? fnok(mock) : fok(mock);
    };

    return spyOn(tgt, func).andReturn(mp);
}

function spyOnPromiseNestedFail(tgt, func, mock) {
    var mp = {};

    mp.then = function (fok) {
        var mmp = {};

        mmp.then = function (f1ok, f1nok) {
            return f1nok(mock);
        };

        return mmp;
    };

    return spyOn(tgt, func).andReturn(mp);
}

function spyOnPromise2NestedFail(tgt, func, mock) {
    var mp = {};

    mp.then = function (fok) {
        var mmp = {};

        mmp.then = function (f1ok) {
            var mmmp = {};

            mmmp.then = function (f2ok, f2nok) {
                return f2nok(mock);
            };

            return mmmp;
        };

        return mmp;
    };

    return spyOn(tgt, func).andReturn(mp);
}
//# sourceMappingURL=helpers.js.map
