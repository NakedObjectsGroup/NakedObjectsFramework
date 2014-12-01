using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Menu;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;
using System;
using System.Reflection;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public class MenuFacetDefault : MenuFacetAbstract {

        public MenuFacetDefault(ISpecification holder)
            : base(holder) {
        }

        //Creates a menu based on the object's actions and their specified ordering
        public override void CreateMenu(IMetamodelBuilder metamodel) {
            if ( Spec().Type.FullName.StartsWith("System")) return; //Menu not relevant, and could cause error below
            MethodInfo m = GetType().GetMethod("CreateDefaultMenu").MakeGenericMethod(Spec().Type);
            m.Invoke(this, new object[] { metamodel, GetMenuName(Spec()) });
        }

        public void CreateDefaultMenu<T>(IMetamodelBuilder metamodel, string menuName) {
            var menu = new TypedMenu<T>(metamodel, false, menuName);
            menu.AddRemainingNativeActions();
            menu.AddContributedActions();
            this.menu = menu;
        }
    }
    // Copyright (c) Naked Objects Group Ltd.
}
