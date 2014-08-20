// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Audit;
using NakedObjects.Core.Context;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.Audit {
    public class AuditManager {
        private readonly IAuditor defaultAuditor;
        private readonly INamespaceAuditor[] namespaceAuditors;
       

        public AuditManager( IAuditor defaultAuditor, params INamespaceAuditor[] namespaceAuditors) {
            this.defaultAuditor = defaultAuditor;
            this.namespaceAuditors = namespaceAuditors;
         
        }

        public void Invoke(INakedObject nakedObject, INakedObject[] parameters, bool queryOnly, IIdentifier identifier) {

            IAuditor auditor = GetNamespaceAuditorFor(nakedObject) ?? GetDefaultAuditor();

            if (nakedObject.Specification.IsService) {
                string serviceName = nakedObject.Specification.GetTitle(nakedObject);
                auditor.ActionInvoked(NakedObjectsContext.Session.Principal, identifier.MemberName, serviceName, queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                auditor.ActionInvoked(NakedObjectsContext.Session.Principal, identifier.MemberName, nakedObject.GetDomainObject(), queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
        }

        public void Updated(INakedObject nakedObject) {
            IAuditor auditor = GetNamespaceAuditorFor(nakedObject) ?? GetDefaultAuditor();
            auditor.ObjectUpdated(NakedObjectsContext.Session.Principal, nakedObject.GetDomainObject());
        }

        public void Persisted(INakedObject nakedObject) {
            IAuditor auditor = GetNamespaceAuditorFor(nakedObject) ?? GetDefaultAuditor();
            auditor.ObjectPersisted(NakedObjectsContext.Session.Principal, nakedObject.GetDomainObject());
        }

        private IAuditor GetNamespaceAuditorFor(INakedObject target) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Specification.FullName;
            var auditor = namespaceAuditors.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.NamespaceToAudit)).
                OrderByDescending(x => x.NamespaceToAudit.Length).
                FirstOrDefault();

            return auditor != null ? CreateAuditor(auditor) : null;
        }

        private IAuditor CreateAuditor(IAuditor auditor) {
            return (IAuditor) NakedObjectsContext.Reflector.LoadSpecification(auditor.GetType()).CreateObject();
        }

        private IAuditor GetDefaultAuditor() {
            return CreateAuditor(defaultAuditor);
        }
    }
}