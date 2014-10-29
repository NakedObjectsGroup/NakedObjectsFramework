// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Component;
using NakedObjects.Audit;
using NakedObjects.Reflect.Spec;

namespace NakedObjects.Reflect.Audit {
    public class AuditInstaller : IAuditorInstaller {
        private readonly AuditManager auditManager;

        public AuditInstaller(IAuditor defaultAuditor) {
            auditManager = new AuditManager(defaultAuditor);
        }

        /// <summary>
        /// </summary>
        /// <param name="defaultAuditor">This will be used unless the object is recognised by one of the namespaceAuthorizers</param>
        /// <param name="namespaceAuditors"></param>
        public AuditInstaller(IAuditor defaultAuditor, params INamespaceAuditor[] namespaceAuditors) {
            auditManager = new AuditManager(defaultAuditor, namespaceAuditors);
        }

        public IFacetDecorator[] CreateDecorators(IMetamodel metamodel) {
            auditManager.Reflector = metamodel;
            return new IFacetDecorator[] {new AuditFacetDecorator(auditManager, metamodel)};
        }

        public string Name {
            get { return "AuditInstaller"; }
        }

        public IFacetDecorator[] CreateDecorators(IReflector reflector) {
            throw new System.NotImplementedException();
        }
    }
}