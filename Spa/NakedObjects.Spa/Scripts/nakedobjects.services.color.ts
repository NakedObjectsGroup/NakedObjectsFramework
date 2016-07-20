/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />

namespace NakedObjects {
    import ObjectIdWrapper = Models.ObjectIdWrapper;

    export interface IColor {

        toColorNumberFromHref(href: string): ng.IPromise<number>;
        toColorNumberFromType(type: string): ng.IPromise<number>;

        addType(type: string, color: number): void;
        addMatch(matcher: RegExp, color: number): void;
        addSubtype(type: string, color: number): void;

        setDefault(def: number): void;
    }

    app.service("color", function(context: IContext, $q: ng.IQService) {
        const colorService = <IColor>this;

        const colorCache: _.Dictionary<number> = {};
        const regexCache: { regex: RegExp, color: number } [] = [];
        const subtypeCache: { type: string, color: number }[] = [];

        let defaultColor = 0;

        function typeFromUrl(url: string): string {
            const oid = ObjectIdWrapper.fromHref(url);
            return oid.domainType;
        }

        function isSubtypeOf(subtype: string, index: number, count: number): ng.IPromise<number> {

            if (index >= count) {
                return $q.reject();
            }

            const entry = subtypeCache[index];
            return context.isSubTypeOf(subtype, entry.type).then(b => b ? $q.when(entry.color) : isSubtypeOf(subtype, index + 1, count));
        }

        function cacheAndReturn(type: string, color: number) {
            colorCache[type] = color;
            return $q.when(color);
        }

        function isSubtype(subtype: string): angular.IPromise<any> {

            const subtypeChecks = subtypeCache.length;

            if (subtypeChecks > 0) {
                return isSubtypeOf(subtype, 0, subtypeChecks).
                    then((c: number) => {
                        return cacheAndReturn(subtype, c);
                    }).
                    catch(() => {
                        return cacheAndReturn(subtype, defaultColor);
                    });
            }

            return cacheAndReturn(subtype, defaultColor);
        }


        function getColor(type: string) {
            // 1 cache 
            // 2 match regex 
            // 3 match subtype 

            // this is potentially expensive - need to filter out non ref types ASAP
       
            if (!type || type === "string" || type === "number" || type === "boolean") {
                return $q.when(defaultColor);
            }

            const cachedEntry = colorCache[type];

            if (cachedEntry) {
                return $q.when(cachedEntry);
            }

            for (const entry of regexCache) {
                if (entry.regex.test(type)) {
                    return cacheAndReturn(type, entry.color);
                }
            }

            return isSubtype(type);
        }


        colorService.toColorNumberFromHref = (href: string) => {
            const type = typeFromUrl(href);
            return colorService.toColorNumberFromType(type);
        };

        colorService.toColorNumberFromType = (type: string) => getColor(type);

        colorService.addType = (type: string, color: number) => {
            colorCache[type] = color;
        }

        colorService.addMatch = (matcher: RegExp, color: number) => {
            regexCache.push({ regex: matcher, color: color });
        }

        colorService.addSubtype = (type: string, color: number) => {
            subtypeCache.push({ type: type, color: color });
        }

        colorService.setDefault = (def: number) => {
            defaultColor = def;
        }
    });
}