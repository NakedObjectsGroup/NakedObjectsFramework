// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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