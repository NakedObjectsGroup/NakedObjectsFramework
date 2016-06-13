/// <reference path="typings/lodash/lodash.d.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.service("template", function () {
        var templateService = this;
        var templates = {};
        var defaults = [NakedObjects.objectViewTemplate, NakedObjects.objectEditTemplate, NakedObjects.objectEditTemplate, NakedObjects.formTemplate];
        templateService.getTemplateName = function (type, iType) {
            var custom = templates[type];
            if (custom) {
                return custom[iType];
            }
            return defaults[iType];
        };
        templateService.setTemplateName = function (type, iType, name) {
            if (!templates[type]) {
                templates[type] = _.clone(defaults);
            }
            templates[type][iType] = name;
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.template.js.map