// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NakedObjects.Surface {
    public abstract class ScalarPropertyHolder : IScalarPropertyHolder {
        // custom extensions 
        protected const string ServiceType = "x-ro-nof-serviceType";
        protected const string RenderInEditMode = "x-ro-nof-renderInEditMode";
        protected const string PresentationHint = "x-ro-nof-presentationHint";

        #region IScalarPropertyHolder Members

        public T GetScalarProperty<T>(ScalarProperty name) {
            return (T) GetScalarProperty(name);
        }

        public bool SupportsProperty(ScalarProperty name) {
            try {
                GetScalarProperty(name);
            }
            catch (NotImplementedException) {
                return false;
            }

            return true;
        }

        public IEnumerable<ScalarProperty> SupportedProperties() {
            return Enum.GetValues(typeof (ScalarProperty)).Cast<ScalarProperty>().Where(SupportsProperty);
        }

        #endregion

        public abstract object GetScalarProperty(ScalarProperty name);
    }
}