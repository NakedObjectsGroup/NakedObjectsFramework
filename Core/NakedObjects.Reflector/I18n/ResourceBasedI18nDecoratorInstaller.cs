// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Reflect.Installer;
using NakedObjects.Reflect.Spec;

namespace NakedObjects.Reflect.I18n.Resourcebundle {
    public class ResourceBasedI18nDecoratorInstaller : IReflectionDecoratorInstaller {
        private readonly string resourceFile;

        public ResourceBasedI18nDecoratorInstaller(string resourceFile) {
            this.resourceFile = resourceFile;
        }

        #region IReflectionDecoratorInstaller Members

        public virtual string Name {
            get { return "resource-i18n"; }
        }

        public virtual IFacetDecorator[] CreateDecorators(IReflector reflector) {
            return new IFacetDecorator[] {
                //  new I18nFacetDecorator(new ResourceBasedI18nManager(resourceFile, NakedObjectsContext.MessageBroker), false)
            };
        }

        #endregion
    }
}