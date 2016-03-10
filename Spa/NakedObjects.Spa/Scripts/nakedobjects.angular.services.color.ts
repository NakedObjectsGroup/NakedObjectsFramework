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
        const color = <IColor>this;
        let colorMap: IColorMap = {};

        // array of colors for allocated colors by default
        let defaultColorArray : string[] = [];

        let defaultColor = "darkBlue";
        const colorPrefix = "bg-color-";

        function hashCode(toHash) {
            let hash = 0, i: number, chr: number;
            if (toHash.length === 0) return hash;
            for (i = 0; i < toHash.length; i++) {
                chr = toHash.charCodeAt(i);
                hash = ((hash << 5) - hash) + chr;
                hash = hash & hash; // Convert to 32bit integer
            }
            return hash;
        };

        function getColorMapValues(dt: string) {
            let clr = dt ? colorMap[dt] : defaultColor;
            if (!clr) {
                const hash = Math.abs(hashCode(dt));
                const index = hash % 18;
                clr = defaultColorArray[index];
                colorMap[dt] = clr;
            }
            return clr;
        }

        function typeFromUrl(url: string): string {
            const typeRegex = /(objects|services)\/([\w|\.]+)/;
            const results = (typeRegex).exec(url);
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
            const type = typeFromUrl(href);
            return `${colorPrefix}${getColorMapValues(type)}`;
        }
    
        color.toColorFromType = (type: string): string => `${colorPrefix}${getColorMapValues(type)}`;
    });
}