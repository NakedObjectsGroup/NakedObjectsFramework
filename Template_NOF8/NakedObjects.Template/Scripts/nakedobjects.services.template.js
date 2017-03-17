/// <reference path="typings/lodash/lodash.d.ts" />
var NakedObjects;
(function (NakedObjects) {
    (function (TemplateType) {
        TemplateType[TemplateType["Object"] = 0] = "Object";
        TemplateType[TemplateType["List"] = 1] = "List";
    })(NakedObjects.TemplateType || (NakedObjects.TemplateType = {}));
    var TemplateType = NakedObjects.TemplateType;
    NakedObjects.app.service("template", function () {
        var templateService = this;
        var templates = [{}, {}];
        var objectDefaults = [NakedObjects.objectViewTemplate, NakedObjects.objectEditTemplate, NakedObjects.objectEditTemplate, NakedObjects.formTemplate, NakedObjects.objectNotpersistentTemplate];
        var listDefaults = ["", NakedObjects.listTemplate, NakedObjects.listAsTableTemplate];
        var defaults = [objectDefaults, listDefaults];
        templateService.getTemplateName = function (type, tType, iType) {
            var custom = templates[tType][type];
            if (custom) {
                return custom[iType];
            }
            return defaults[tType][iType];
        };
        templateService.setTemplateName = function (type, tType, iType, name) {
            if (!templates[tType][type]) {
                templates[tType][type] = _.clone(defaults[tType]);
            }
            templates[tType][type][iType] = name;
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.template.js.map