/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.config.ts" />

module Spiro.Angular {

    export interface IColor {
        toColorFromHref(href: string) : string;
        toColorFromType(type : string) : string;
    }

    app.service('color', function () {

        var color = <IColor>this;

        function hashCode(toHash) {
            var hash = 0, i, char;
            if (toHash.length == 0) return hash;
            for (i = 0; i < toHash.length; i++) {
                char = toHash.charCodeAt(i);
                hash = ((hash << 5) - hash) + char;
                hash = hash & hash; // Convert to 32bit integer
            }
            return hash;
        };

        function getColorMapValues(dt: string) {
            var color = dt ? colorMap[dt] : defaultColor;
            if (!color) {
                var hash = Math.abs(hashCode(dt));
                var index = hash % 18;
                color = defaultColorArray[index];
                colorMap[dt] = color;
            }
            return color;
        }

        function typeFromUrl(url: string): string {
            var typeRegex = /(objects|services)\/([\w|\.]+)/;
            var results = (typeRegex).exec(url);
            return (results && results.length > 2) ? results[2] : "";
        }

        color.toColorFromHref = (href: string): string => {
            var type = typeFromUrl(href);
            return "bg-color-" + getColorMapValues(type);
        }

        color.toColorFromType = (type: string): string => "bg-color-" + getColorMapValues(type);
    });
}