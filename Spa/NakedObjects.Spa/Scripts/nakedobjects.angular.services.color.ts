//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular {

    export interface IColorMap {
        [index: string]: string;
    }

    export interface IColor {
        toColorFromHref(href: string) : string;
        toColorFromType(type: string): string;
        setColorMap(map: IColorMap);
        setDefaultColorArray(colors: string[]);
        setDefaultColor(dfltColor : string); 
    }

    app.service('color', function () {

        var color = <IColor>this;
        var colorMap: IColorMap = {};

        // array of colors for allocated colors by default
        var defaultColorArray : string[] = [];

        var defaultColor: string = "darkBlue";

        function hashCode(toHash) {
            var hash = 0, i, chr;
            if (toHash.length === 0) return hash;
            for (i = 0; i < toHash.length; i++) {
                chr = toHash.charCodeAt(i);
                hash = ((hash << 5) - hash) + chr;
                hash = hash & hash; // Convert to 32bit integer
            }
            return hash;
        };

        function getColorMapValues(dt: string) {
            var clr = dt ? colorMap[dt] : defaultColor;
            if (!clr) {
                var hash = Math.abs(hashCode(dt));
                var index = hash % 18;
                clr = defaultColorArray[index];
                colorMap[dt] = clr;
            }
            return clr;
        }

        function typeFromUrl(url: string): string {
            var typeRegex = /(objects|services)\/([\w|\.]+)/;
            var results = (typeRegex).exec(url);
            return (results && results.length > 2) ? results[2] : "";
        }

        color.setColorMap = (map: IColorMap) => {
            colorMap = map;
        }

        color.setDefaultColorArray = (colors: string[]) => {
            defaultColorArray = colors;
        }

        color.setDefaultColor = (dfltColor: string) => {
            defaultColor = dfltColor;
        }

        // tested
        color.toColorFromHref = (href: string): string => {
            var type = typeFromUrl(href);
            return "bg-color-" + getColorMapValues(type);
        }
    
        color.toColorFromType = (type: string): string => "bg-color-" + getColorMapValues(type);
    });
}