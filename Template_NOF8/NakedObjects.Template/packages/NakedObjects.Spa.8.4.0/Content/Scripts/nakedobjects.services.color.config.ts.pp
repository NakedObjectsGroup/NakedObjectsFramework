/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />

namespace NakedObjects {

    app.run((color: IColor) => {
        //Note: colour is determined by the FIRST matching rule
		//Colour numbers are interpreted by CSS

        //Match specific class
        //e.g. color.addType("MyNameSpace.MyConcreteClass", 1);

        //Match Regex on name
        //e.g. color.addMatch(/.*PartialClassName.*/, 2); 

        //Match on sub-type
        //e.g. color.addSubtype("MyNameSpace.MyAbstractClassOrInterface", 3); 

        //Default colour -  must be specified last
        color.setDefault(0);
    });
}