// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public class ActionSpecImmutable : MemberSpecImmutable, IActionSpecImmutable {
        private readonly IActionParameterSpecImmutable[] parameters;
        private readonly ITypeSpecImmutable ownerSpec;

        public ActionSpecImmutable(IIdentifier identifier, ITypeSpecImmutable ownerSpec,
            IActionParameterSpecImmutable[] parameters)
            : base(identifier) {
            this.ownerSpec = ownerSpec;
            this.parameters = parameters;
        }

        #region IActionSpecImmutable Members

        public override IObjectSpecImmutable ReturnSpec {
            get { return GetFacet<IActionInvocationFacet>().ReturnType; }
        }

        public  ITypeSpecImmutable OwnerSpec {
            get { return ownerSpec; }
        }

        public IActionParameterSpecImmutable[] Parameters {
            get { return parameters; }
        }

        public string GroupFullName {
            get { return ""; }
        }

        public override IObjectSpecImmutable ElementSpec {
            get { return GetFacet<IActionInvocationFacet>().ElementType; }
        }

        public bool IsFinderMethod {
            get {
                if (HasReturn() && ContainsFacet(typeof(IFinderActionFacet))) {
                    foreach (var p in this.Parameters) {
                        bool parseable = p.Specification.IsParseable;
                        bool choicesEnabled = p.IsChoicesEnabled || p.IsMultipleChoicesEnabled;
                        if (!(parseable || choicesEnabled)) {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        public bool IsFinderMethodFor(IObjectSpecImmutable spec) {
            return IsFinderMethod && (ReturnSpec.IsOfType(spec) || (ReturnSpec.IsCollection && ElementSpec.IsOfType(spec)));
        }

        public bool IsContributedMethod {
            get {
                return OwnerSpec is IServiceSpecImmutable && parameters.Any() &&
                       ContainsFacet(typeof (IContributedActionFacet));
            }
        }

        public bool IsContributedTo(IObjectSpecImmutable objectSpecImmutable) {
            return Parameters.Any(parm => IsContributedTo(parm.Specification, objectSpecImmutable));
        }

        public bool IsContributedToCollectionOf(IObjectSpecImmutable objectSpecImmutable) {
            return Parameters.Any(parm => {
                var facet = GetFacet<IContributedActionFacet>();
                return facet != null && facet.IsContributedToCollectionOf(objectSpecImmutable);
            });
        }

        #endregion

        private bool HasReturn() {
            return ReturnSpec != null;
        }

        private bool IsContributedTo(IObjectSpecImmutable parmSpec, IObjectSpecImmutable contributeeSpec) {
            var facet = GetFacet<IContributedActionFacet>();

            if (facet == null) {
                return false;
            }

            return contributeeSpec.IsOfType(parmSpec) && facet.IsContributedTo(contributeeSpec);
        }

        #region ISerializable

        // The special constructor is used to deserialize values. 
        public ActionSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            ownerSpec = info.GetValue<IObjectSpecImmutable>("specification");
            parameters = info.GetValue<IActionParameterSpecImmutable[]>("parameters");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<ISpecification>("specification", ownerSpec);
            info.AddValue<IActionParameterSpecImmutable[]>("parameters", parameters);

            base.GetObjectData(info, context);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}