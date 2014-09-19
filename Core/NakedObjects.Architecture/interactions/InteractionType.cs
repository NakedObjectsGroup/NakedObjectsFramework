// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Interactions {
    public class InteractionType {
        public static readonly InteractionType ActionInvoke = new InteractionType("Invoking action");
        public static readonly InteractionType CandidateArgument = new InteractionType("Candidate argument");
        public static readonly InteractionType CollectionAddTo = new InteractionType("Adding to collection");
        public static readonly InteractionType CollectionRemoveFrom = new InteractionType("Removing from collection");
        public static readonly InteractionType MemberAccess = new InteractionType("Accessing member");
        public static readonly InteractionType ObjectPersist = new InteractionType("Persisting or updating object");
        public static readonly InteractionType ObjectView = new InteractionType("View an object");
        public static readonly InteractionType PropertyParamModify = new InteractionType("Modifying property");

        private readonly string description;

        private InteractionType(string description) {
            this.description = description;
        }

        public virtual string Description {
            get { return description; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}