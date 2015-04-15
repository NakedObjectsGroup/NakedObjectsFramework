// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface;

namespace NakedObjects.Web.Mvc.Helpers {
    public class ScaffoldAction : INakedObjectActionSurface {
        public ScaffoldAction(IActionSpec spec) {
            WrappedSpec = spec;
        }

        public IActionSpec WrappedSpec { get; set; }


        public static INakedObjectActionSurface Wrap(IActionSpec spec) {
            return new ScaffoldAction(spec);
        }

        public T GetScalarProperty<T>(ScalarProperty name) {
            throw new NotImplementedException();
        }

        public bool SupportsProperty(ScalarProperty name) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ScalarProperty> SupportedProperties() {
            throw new System.NotImplementedException();
        }

        public INakedObjectsSurface Surface {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public string Id {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            throw new System.NotImplementedException();
        }

        public IConsentSurface IsUsable(INakedObjectSurface nakedObject) {
            throw new System.NotImplementedException();
        }

        public INakedObjectSpecificationSurface ReturnType {
            get { throw new System.NotImplementedException(); }
        }

        public INakedObjectSpecificationSurface ElementType {
            get { throw new System.NotImplementedException(); }
        }

        public int ParameterCount {
            get { throw new System.NotImplementedException(); }
        }

        public INakedObjectActionParameterSurface[] Parameters {
            get { throw new System.NotImplementedException(); }
        }

        public INakedObjectSpecificationSurface OnType {
            get { throw new System.NotImplementedException(); }
        }
    }
}