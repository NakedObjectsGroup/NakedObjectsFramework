/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        Angular.app.service('color', function () {
            var color = this;
            var colorMap = {};

            // array of colors for allocated colors by default
            var defaultColorArray = [];

            var defaultColor = "darkBlue";

            function hashCode(toHash) {
                var hash = 0, i, chr;
                if (toHash.length == 0)
                    return hash;
                for (i = 0; i < toHash.length; i++) {
                    chr = toHash.charCodeAt(i);
                    hash = ((hash << 5) - hash) + chr;
                    hash = hash & hash; // Convert to 32bit integer
                }
                return hash;
            }
            ;

            function getColorMapValues(dt) {
                var clr = dt ? colorMap[dt] : defaultColor;
                if (!clr) {
                    var hash = Math.abs(hashCode(dt));
                    var index = hash % 18;
                    clr = defaultColorArray[index];
                    colorMap[dt] = clr;
                }
                return clr;
            }

            function typeFromUrl(url) {
                var typeRegex = /(objects|services)\/([\w|\.]+)/;
                var results = (typeRegex).exec(url);
                return (results && results.length > 2) ? results[2] : "";
            }

            color.setColorMap = function (map) {
                colorMap = map;
            };

            color.setDefaultColorArray = function (colors) {
                defaultColorArray = colors;
            };

            color.setDefaultColor = function (dfltColor) {
                defaultColor = dfltColor;
            };

            // tested
            color.toColorFromHref = function (href) {
                var type = typeFromUrl(href);
                return "bg-color-" + getColorMapValues(type);
            };

            color.toColorFromType = function (type) {
                return "bg-color-" + getColorMapValues(type);
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.color.js.map
