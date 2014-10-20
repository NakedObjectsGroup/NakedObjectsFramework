// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Metamodel.Facet {
    public class IconFacetViaMethod : IconFacetAbstract {
        private readonly string iconName; // iconName from attribute
        private readonly MethodInfo method;

        public IconFacetViaMethod(MethodInfo method, ISpecification holder, string iconName)
            : base(holder) {
            this.method = method;
            this.iconName = iconName;
        }

        public override string GetIconName(INakedObject nakedObject) {
            return (string) InvokeUtils.Invoke(method, nakedObject);
        }

        public override string GetIconName() {
            return iconName;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}