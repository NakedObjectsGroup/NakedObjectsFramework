// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Peer {
    public interface INakedObjectActionPeer : INakedObjectMemberPeer, IOrderableElement<INakedObjectActionPeer> {
        INakedObjectActionParamPeer[] Parameters { get; }
        bool IsFinderMethod { get; }
        IIntrospectableSpecification ReturnType { get; }
        bool IsContributedMethod { get; }
        bool IsContributedTo(IIntrospectableSpecification introspectableSpecification);
    }

    // Copyright (c) Naked Objects Group Ltd.
}