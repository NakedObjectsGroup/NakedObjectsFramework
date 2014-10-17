// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Contributed;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Reflect.Actions {
    // TODO (in all DotNet...Peer classes) make all methodsArray throw ReflectiveActionException when 
    // an exception occurs when calling a method reflectively (see execute method).  Then instead of 
    // calling invocationExcpetion() the exception will be passed though, and dealt with generally by 
    // the reflection package (which will be the same for all reflectors and will allow the message to
    // be better passed back to the client).


    public class ActionSpecImmutable : MemberSpecImmutable, IActionSpecImmutable {
        private readonly IObjectSpecImmutable specification;
        private readonly IActionParameterSpecImmutable[] parameters;

        public ActionSpecImmutable(IIdentifier identifier,  IObjectSpecImmutable specification, IActionParameterSpecImmutable[] parameters)
            : base(identifier) {
            this.specification = specification;
            this.parameters = parameters;
        }

        public override IObjectSpecImmutable Specification {
            get { return specification; }
        }

        #region INakedObjectActionPeer Members

        public IActionParameterSpecImmutable[] Parameters {
            get { return parameters; }
        }  

        #endregion

        public IActionSpecImmutable Peer { get { return this; }}
        public IOrderSet<IActionSpecImmutable> Set { get { return null; } }

        public  bool IsContributedMethod {
            get {
                if (Specification.Service && parameters.Any() &&
                    (!ContainsFacet(typeof(INotContributedActionFacet)) || !GetFacet<INotContributedActionFacet>().NeverContributed())) {
                    return Parameters.Any(p => p.Specification.IsObject || p.Specification.IsCollection);
                }
                return false;
            }
        }

      
        public virtual IObjectSpecImmutable ReturnType {
            get { return GetFacet<IActionInvocationFacet>().ReturnType; }
        }

        public virtual bool HasReturn() {
            return ReturnType != null;
        }

        public bool IsFinderMethod {
            get { return HasReturn() && !ContainsFacet(typeof(IExcludeFromFindMenuFacet)); }
        }

        public bool IsContributedTo(IObjectSpecImmutable objectSpecImmutable) {
            return IsContributedMethod
                   && Parameters.Any(parm => ContributeTo(parm.Specification, objectSpecImmutable))
                   && !(IsCollection(objectSpecImmutable) && IsCollection(GetFacet<IActionInvocationFacet>().ReturnType));
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}