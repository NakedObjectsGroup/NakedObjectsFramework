/// <reference path="typings/lodash/lodash.d.ts" />


namespace NakedObjects {

    export interface ITemplate {
        getTemplateName(type: string, tType: TemplateType, iType: InteractionMode | CollectionViewState): string;
        setTemplateName(type: string, tType: TemplateType, iType: InteractionMode | CollectionViewState, name: string): void;
    }

    export enum TemplateType {
        Object, 
        List
    }

    app.service("template",
        function() {
            const templateService = <ITemplate>this;

            const templates: _.Dictionary<string[]>[] = [{}, {}];
            const objectDefaults = [objectViewTemplate, objectEditTemplate, objectEditTemplate, formTemplate, objectNotpersistentTemplate];
            const listDefaults = ["", listTemplate, listAsTableTemplate];

            const defaults = [objectDefaults, listDefaults];

            templateService.getTemplateName = (type: string, tType : TemplateType,  iType: InteractionMode | CollectionViewState) : string => {

                const custom = templates[tType][type];

                if (custom) {
                    return custom[iType];
                }

                return defaults[tType][iType];
            };

            templateService.setTemplateName = (type: string, tType: TemplateType, iType: InteractionMode | CollectionViewState, name: string) => {
                if (!templates[tType][type]) {
                    templates[tType][type] = _.clone(defaults[tType]);
                }

                templates[tType][type][iType] = name;
            };

        });
}