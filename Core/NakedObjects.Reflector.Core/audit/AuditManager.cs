// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Audit;
using NakedObjects.Core.Context;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.Audit {
    public class AuditManager {
        private readonly IAuditor defaultAuditor;
        private readonly INamespaceAuditor[] namespaceAuditors;

        private bool isInitialised;

        public AuditManager(IAuditor defaultAuditor, params INamespaceAuditor[] namespaceAuditors) {
            this.defaultAuditor = defaultAuditor;
            this.namespaceAuditors = namespaceAuditors;
        }

        private void Inject() {
            object[] services = NakedObjectsContext.ObjectPersistor.GetServices().Select(no => no.Object).ToArray();
            IContainerInjector injector = NakedObjectsContext.Reflector.CreateContainerInjector(services);
            injector.InitDomainObject(defaultAuditor);
            namespaceAuditors.ForEach(injector.InitDomainObject);
        }

        public void Init() {
            if (!isInitialised) {
                isInitialised = true;
                Inject();
            }
        }

        public void Invoke(INakedObject nakedObject, INakedObject[] parameters, bool queryOnly, IIdentifier identifier) {
            Init();
            IAuditor auditor = GetNamespaceAuthorizerFor(nakedObject) ?? defaultAuditor;

            if (nakedObject.Specification.IsService) {
                string serviceName = nakedObject.Specification.GetTitle(nakedObject);
                auditor.ActionInvoked(NakedObjectsContext.Session.Principal, identifier.MemberName, serviceName, queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                auditor.ActionInvoked(NakedObjectsContext.Session.Principal, identifier.MemberName, nakedObject.GetDomainObject(), queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
        }

        public void Updated(INakedObject nakedObject) {
            Init();
            IAuditor auditor = GetNamespaceAuthorizerFor(nakedObject) ?? defaultAuditor;
            auditor.ObjectUpdated(NakedObjectsContext.Session.Principal, nakedObject.GetDomainObject());
        }

        public void Persisted(INakedObject nakedObject) {
            Init();
            IAuditor auditor = GetNamespaceAuthorizerFor(nakedObject) ?? defaultAuditor;
            auditor.ObjectPersisted(NakedObjectsContext.Session.Principal, nakedObject.GetDomainObject());
        }


        private INamespaceAuditor GetNamespaceAuthorizerFor(INakedObject target) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Specification.FullName;
            return namespaceAuditors.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.NamespaceToAudit)).
                OrderByDescending(x => x.NamespaceToAudit.Length).
                FirstOrDefault();
        }
    }
}