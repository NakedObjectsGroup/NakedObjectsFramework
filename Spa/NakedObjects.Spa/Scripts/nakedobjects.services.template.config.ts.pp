/// <reference path="typings/lodash/lodash.d.ts" />

namespace NakedObjects {
    app.run((template: ITemplate) => {
		//Specify custom templates to associate with specific domain types e.g.
        //template.setTemplateName("myNameSpace.Foo", TemplateType.Object, InteractionMode.View, "Content/customTemplates/fooView.html");
        //template.setTemplateName("myNameSpace.Bar", TemplateType.List, CollectionViewState.List, "Content/customTemplates/barList.html");
    });
}