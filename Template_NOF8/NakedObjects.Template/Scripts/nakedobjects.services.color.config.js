/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.run(function (color) {
        //Note: colour is determined by the FIRST matching rule
        //Match specific class
        color.addType("ExampleModel.Customer", 1);
        //Default colour -  must be specified last
        color.setDefault(0);
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.color.config.js.map