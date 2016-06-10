/// <reference path="typings/lodash/lodash.d.ts" />


module NakedObjects {

    export interface ITemplate {
        getTemplateName(type: string, iType: InteractionMode): string;
        setTemplateName(type: string, iType: InteractionMode, name: string): any;
    }

    app.service("template",
        function() {
            const templateService = <ITemplate>this;

            const templates: _.Dictionary<string[]> = {};
            const defaults = [objectViewTemplate, objectEditTemplate, objectEditTemplate, formTemplate];

            templateService.getTemplateName = (type: string, iType: InteractionMode) => {

                const custom = templates[type];

                if (custom) {
                    return custom[iType];
                }

                return defaults[iType];
            };

            templateService.setTemplateName = (type: string, iType: InteractionMode, name: string) => {
                if (!templates[type]) {
                    templates[type] = _.clone(defaults);
                }

                templates[type][iType] = name;
            };

        });
}