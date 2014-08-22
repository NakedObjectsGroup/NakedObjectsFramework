// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.I18n.Resourcebundle {
    public class ResourceBasedI18nDecoratorInstaller : IReflectionDecoratorInstaller {
        private readonly string resourceFile;

        public ResourceBasedI18nDecoratorInstaller(string resourceFile) {
            this.resourceFile = resourceFile;
        }

        #region IReflectionDecoratorInstaller Members

        public virtual string Name {
            get { return "resource-i18n"; }
        }

        public virtual IFacetDecorator[] CreateDecorators(INakedObjectReflector reflector) {
            return new IFacetDecorator[] {
                new I18nFacetDecorator(new ResourceBasedI18nManager(resourceFile, NakedObjectsContext.MessageBroker), false)
            };
        }

        #endregion
    }
}