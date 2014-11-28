using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Menus;
using NakedObjects.Meta.SpecImmutable;
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
            ObjectSpecImmutable spec = (ObjectSpecImmutable)Specification;
            if (spec.Type.FullName.StartsWith("System")) return; //Menu not relevant, and could cause error below
            MethodInfo m = GetType().GetMethod("CreateDefaultMenu").MakeGenericMethod(spec.Type);
            m.Invoke(this, new object[] { metamodel });
        }

        public void CreateDefaultMenu<T>(IMetamodelBuilder metamodel) {
            var menu = new TypedMenu<T>(metamodel, false, ObjectMenuName);
            menu.AddRemainingNativeActions();
            menu.AddContributedActions();
            this.menu = menu;
        }
    }
    // Copyright (c) Naked Objects Group Ltd.
}
