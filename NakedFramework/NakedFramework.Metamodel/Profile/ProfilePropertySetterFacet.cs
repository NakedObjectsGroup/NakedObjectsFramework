// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Profile;

namespace NakedFramework.Metamodel.Profile {
    [Serializable]
    public sealed class ProfilePropertySetterFacet : PropertySetterFacetAbstract {
        private readonly IProfileManager profileManager;
        private readonly IPropertySetterFacet underlyingFacet;

        public ProfilePropertySetterFacet(IPropertySetterFacet underlyingFacet, IProfileManager profileManager) : base(underlyingFacet.Specification) {
            this.underlyingFacet = underlyingFacet;
            this.profileManager = profileManager;
        }

        public override string PropertyName {
            get => underlyingFacet.PropertyName;
            protected set { }
        }

        public override void SetProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter nakedValue, INakedObjectsFramework framework) {
            profileManager.Begin(framework.Session, ProfileEvent.PropertySet, PropertyName, nakedObjectAdapter, framework.LifecycleManager);
            try {
                underlyingFacet.SetProperty(nakedObjectAdapter, nakedValue, framework);
            }
            finally {
                profileManager.End(framework.Session, ProfileEvent.PropertySet, PropertyName, nakedObjectAdapter, framework.LifecycleManager);
            }
        }
    }
}