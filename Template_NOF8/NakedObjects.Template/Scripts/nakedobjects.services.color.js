/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    NakedObjects.app.service("color", function (context, $q) {
        var colorService = this;
        var colorCache = {};
        var regexCache = [];
        var subtypeCache = [];
        var defaultColor = 0;
        function typeFromUrl(url) {
            var oid = ObjectIdWrapper.fromHref(url);
            return oid.domainType;
        }
        function isSubtypeOf(subtype, index, count) {
            if (index >= count) {
                return $q.reject();
            }
            var entry = subtypeCache[index];
            return context.isSubTypeOf(subtype, entry.type).then(function (b) { return b ? $q.when(entry.color) : isSubtypeOf(subtype, index + 1, count); });
        }
        function cacheAndReturn(type, color) {
            colorCache[type] = color;
            return color;
        }
        function isSubtype(subtype) {
            var subtypeChecks = subtypeCache.length;
            if (subtypeChecks > 0) {
                return isSubtypeOf(subtype, 0, subtypeChecks).
                    then(function (c) {
                    return cacheAndReturn(subtype, c);
                }).
                    catch(function () {
                    return cacheAndReturn(subtype, defaultColor);
                });
            }
            return $q.when(cacheAndReturn(subtype, defaultColor));
        }
        function getColor(type) {
            // 1 cache 
            // 2 match regex 
            // 3 match subtype 
            // this is potentially expensive - need to filter out non ref types ASAP
            if (!type || type === "string" || type === "number" || type === "boolean") {
                return $q.when(defaultColor);
            }
            var cachedEntry = colorCache[type];
            if (cachedEntry) {
                return $q.when(cachedEntry);
            }
            for (var _i = 0, regexCache_1 = regexCache; _i < regexCache_1.length; _i++) {
                var entry = regexCache_1[_i];
                if (entry.regex.test(type)) {
                    return $q.when(cacheAndReturn(type, entry.color));
                }
            }
            return isSubtype(type);
        }
        colorService.toColorNumberFromHref = function (href) {
            var type = typeFromUrl(href);
            return colorService.toColorNumberFromType(type);
        };
        colorService.toColorNumberFromType = function (type) { return getColor(type); };
        colorService.addType = function (type, color) {
            colorCache[type] = color;
        };
        colorService.addMatch = function (matcher, color) {
            regexCache.push({ regex: matcher, color: color });
        };
        colorService.addSubtype = function (type, color) {
            subtypeCache.push({ type: type, color: color });
        };
        colorService.setDefault = function (def) {
            defaultColor = def;
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.color.js.map