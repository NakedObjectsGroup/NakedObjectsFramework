// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class TypeActionInvokeContext {
        #region ActionType enum

        public enum ActionType {
            IsSubtypeOf,
            IsSupertypeOf
        };

        #endregion

        private readonly ActionType aType;
        private readonly string typeName;

        public TypeActionInvokeContext(string actionName, string typeName) {
            this.typeName = typeName;
            if (actionName == WellKnownIds.IsSupertypeOf) {
                aType = ActionType.IsSupertypeOf;
            }
            else if (actionName == WellKnownIds.IsSubtypeOf) {
                aType = ActionType.IsSubtypeOf;
            }
            else {
                throw new TypeActionResourceNotFoundException(actionName, typeName);
            }
        }

        public string TypeName {
            get { return typeName; }
        }

        public string Id {
            get { return aType == ActionType.IsSubtypeOf ? WellKnownIds.IsSubtypeOf : WellKnownIds.IsSupertypeOf; }
        }

        public string ParameterId {
            get { return aType == ActionType.IsSubtypeOf ? JsonPropertyNames.SuperType : JsonPropertyNames.SubType; }
        }

        public bool Value {
            get { return aType == ActionType.IsSubtypeOf ? ThisSpecification.IsOfType(OtherSpecification) : OtherSpecification.IsOfType(ThisSpecification); }
        }

        public INakedObjectSpecificationSurface ThisSpecification { get; set; }
        public INakedObjectSpecificationSurface OtherSpecification { get; set; }
    }
}