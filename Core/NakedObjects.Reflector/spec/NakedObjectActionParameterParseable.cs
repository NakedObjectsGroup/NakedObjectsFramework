// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Facets.Propparam.MultiLine;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class NakedObjectActionParameterParseable : NakedObjectActionParameterAbstract, IParseableEntryActionParameter {
        public NakedObjectActionParameterParseable(IMetadata metadata, int index, INakedObjectAction action, INakedObjectActionParamPeer peer)
            : base(metadata, index, action, peer) {}

        public virtual int NoLines {
            get { return GetFacet<IMultiLineFacet>().NumberOfLines; }
        }

        public virtual int MaximumLength {
            get { return GetFacet<IMaxLengthFacet>().Value; }
        }

        public virtual int TypicalLineLength {
            get { return GetFacet<ITypicalLengthFacet>().Value; }
        }
    }
}