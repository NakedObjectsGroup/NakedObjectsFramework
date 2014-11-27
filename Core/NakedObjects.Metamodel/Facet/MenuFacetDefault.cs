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
            Menu menu = new Menu(metamodel, ObjectMenuName);
            //First add the native actions
            ObjectSpecImmutable spec = (ObjectSpecImmutable)Specification;
            menu.AddOrderableElementsToMenu(spec.ObjectActions, menu);
            //Then add the contributed actions
            foreach (var ca in spec.ContributedActions) {
                Menu sub = new Menu(metamodel, ca.Item2); //Item 2 should be friendly name of the contributing service
                //Item2 is contributing service class name, not used.
                sub.AddOrderableElementsToMenu(ca.Item3, sub); //Item 3 should be the actions
                menu.AddAsSubMenu(sub);
            }
            this.menu = menu;
        }
    }
    // Copyright (c) Naked Objects Group Ltd.
}
