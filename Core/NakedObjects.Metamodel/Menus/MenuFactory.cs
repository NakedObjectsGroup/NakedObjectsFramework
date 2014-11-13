using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Meta.Menus {

    public class MenuFactory : IMenuFactory {

        #region Injected Services
        protected readonly IMetamodelBuilder metamodel;
        #endregion

        public MenuFactory(IMetamodelBuilder metamodel) {
            this.metamodel = metamodel;
        }

        public IMenu NewMenu(string name) {
            return new Menu(metamodel, name);
        }

        public ITypedMenu<TObject> NewMenu<TObject>(bool addAllActions, string name = null) {
            return new TypedMenu<TObject>(metamodel, addAllActions, name);
        }
    }
}
