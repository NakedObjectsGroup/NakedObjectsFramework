// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.SpecImmutable {
    // TODO (in all DotNet...Peer classes) make all methodsArray throw ReflectiveActionException when 
    // an exception occurs when calling a method reflectively (see execute method).  Then instead of 
    // calling invocationExcpetion() the exception will be passed though, and dealt with generally by 
    // the reflection package (which will be the same for all reflectors and will allow the message to
    // be better passed back to the client).

    [Serializable]
    public class ActionSpecImmutable : MemberSpecImmutable, IActionSpecImmutable {
        private readonly IActionParameterSpecImmutable[] parameters;
        private readonly IObjectSpecImmutable specification;

        public ActionSpecImmutable(IIdentifier identifier, IObjectSpecImmutable specification,
                                   IActionParameterSpecImmutable[] parameters)
            : base(identifier) {
            this.specification = specification;
            this.parameters = parameters;
        }

        #region IActionSpecImmutable Members

        public override IObjectSpecImmutable ReturnSpec {
            get { return specification; }
        }

        public IActionParameterSpecImmutable[] Parameters {
            get { return parameters; }
        }

        public IActionSpecImmutable Spec {
            get { return this; }
        }

        public IList<IOrderableElement<IActionSpecImmutable>> Set {
            get { return null; }
        }

        public string GroupFullName {
            get { return ""; }
        }

        public IObjectSpecImmutable ElementType {
            get { return GetFacet<IActionInvocationFacet>().ElementType; }
        }

        public bool IsContributedMethod {
            get {
                if (ReturnSpec.Service && parameters.Any() &&
                    (!ContainsFacet(typeof (INotContributedActionFacet)) ||
                     !GetFacet<INotContributedActionFacet>().NeverContributed())) {
                    return Parameters.Any(p => p.Specification.IsObject || p.Specification.IsCollection);
                }
                return false;
            }
        }


        public virtual IObjectSpecImmutable ReturnType {
            get { return GetFacet<IActionInvocationFacet>().ReturnType; }
        }

        public bool IsFinderMethod {
            get { return HasReturn() && !ContainsFacet(typeof (IExcludeFromFindMenuFacet)); }
        }

        public bool IsContributedTo(IObjectSpecImmutable objectSpecImmutable) {
            return IsContributedMethod
                   && Parameters.Any(parm => ContributeTo(parm.Specification, objectSpecImmutable))
                   &&
                   !(IsCollection(objectSpecImmutable) && IsCollection(GetFacet<IActionInvocationFacet>().ReturnType));
        }

        #endregion

        private bool HasReturn() {
            return ReturnType != null;
        }

        private bool IsCollection(IObjectSpecImmutable spec) {
            return spec.IsCollection && !spec.IsParseable;
        }

        private bool ContributeTo(IObjectSpecImmutable parmSpec, IObjectSpecImmutable contributeeSpec) {
            var ncf = GetFacet<INotContributedActionFacet>();

            if (ncf == null) {
                return contributeeSpec.IsOfType(parmSpec);
            }

            return contributeeSpec.IsOfType(parmSpec) && !ncf.NotContributedTo(contributeeSpec);
        }

        #region ISerializable

        // The special constructor is used to deserialize values. 
        public ActionSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            specification = info.GetValue<IObjectSpecImmutable>("specification");
            parameters = info.GetValue<IActionParameterSpecImmutable[]>("parameters");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<ISpecification>("specification", specification);
            info.AddValue<IActionParameterSpecImmutable[]>("parameters", parameters);

            base.GetObjectData(info, context);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}