// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Audit;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.Audit {
    public class AuditManager {
        private readonly IAuditor defaultAuditor;
        private readonly INamespaceAuditor[] namespaceAuditors;
       

        public AuditManager( IAuditor defaultAuditor, params INamespaceAuditor[] namespaceAuditors) {
            this.defaultAuditor = defaultAuditor;
            this.namespaceAuditors = namespaceAuditors;  
        }

        public IMetamodel Reflector { protected get; set; }

        public void Invoke(INakedObject nakedObject, INakedObject[] parameters, bool queryOnly, IIdentifier identifier, ISession session) {
            throw new NotImplementedException("Fix audit manager");
            //IAuditor auditor = GetNamespaceAuditorFor(nakedObject) ?? GetDefaultAuditor();

            //if (nakedObject.Specification.IsService) {
            //    //string serviceName = nakedObject.Specification.GetTitle(nakedObject, manager);
            //    auditor.ActionInvoked(session.Principal, identifier.MemberName, serviceName, queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            //}
            //else {
            //    auditor.ActionInvoked(session.Principal, identifier.MemberName, nakedObject.GetDomainObject(), queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            //}
        }

        public void Updated(INakedObject nakedObject, ISession session) {
            IAuditor auditor = GetNamespaceAuditorFor(nakedObject) ?? GetDefaultAuditor();
            auditor.ObjectUpdated(session.Principal, nakedObject.GetDomainObject());
        }

        public void Persisted(INakedObject nakedObject, ISession session) {
            IAuditor auditor = GetNamespaceAuditorFor(nakedObject) ?? GetDefaultAuditor();
            auditor.ObjectPersisted(session.Principal, nakedObject.GetDomainObject());
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
            throw new NotImplementedException("Fix audit manager");
            //return (IAuditor)persistor.CreateObject(Reflector.LoadSpecification(auditor.GetType()));
            //return (IAuditor)Reflector.LoadSpecification(auditor.GetType()).CreateObject(persistor);
        }

        private IAuditor GetDefaultAuditor() {
            return CreateAuditor(defaultAuditor);
        }
    }
}