// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace NakedObjects.Meta.Profile {
    //Add namespace profilers individually via AddNamespaceProfiler, or create the whole dictionary
    //and set the NamespaceProfilers property.
    public class ProfileConfiguration<TDefault> : IProfileConfiguration where TDefault : IProfiler {
        public ProfileConfiguration() {
            DefaultProfiler = typeof (TDefault);
            NamespaceProfilers = new Dictionary<string, Type>();
        }

        #region IProfileConfiguration Members

        public Type DefaultProfiler { get; private set; }
        public Dictionary<string, Type> NamespaceProfilers { get; private set; }

        public void AddNamespaceProfiler<T>(string namespaceCovered) where T : IProfiler {
            NamespaceProfilers.Add(namespaceCovered, typeof (T));
        }

        #endregion
    }
}