using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Security;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Security {
    public class CustomAuthorizerInstaller : IAuthorizerInstaller {
        private readonly CustomAuthorizationManager authManager;

        public CustomAuthorizerInstaller( ITypeAuthorizer<object> defaultAuthorizer) {
            authManager = new CustomAuthorizationManager(defaultAuthorizer);
        }

        /// <summary>
        /// </summary>
        /// <param name="defaultAuthorizer">This will be used unless the object type exactly matches one of the typeauthorizers</param>
        /// <param name="typeAuthorizers">Each authorizer must implement ITypeAuthorizer for a concrete domain type</param>
        public CustomAuthorizerInstaller(ITypeAuthorizer<object> defaultAuthorizer, params object[] typeAuthorizers) {
            authManager = new CustomAuthorizationManager(defaultAuthorizer, typeAuthorizers);
        }

        /// <summary>
        /// </summary>
        /// <param name="defaultAuthorizer">This will be used unless the object is recognised by one of the namespaceAuthorizers</param>
        public CustomAuthorizerInstaller(ITypeAuthorizer<object> defaultAuthorizer, params INamespaceAuthorizer[] namespaceAuthorizers) {
            authManager = new CustomAuthorizationManager(defaultAuthorizer, namespaceAuthorizers);
        }

        #region IAuthorizerInstaller Members

        public IFacetDecorator[] CreateDecorators(IMetadata metadata) {
            authManager.Metadata = metadata;
            return new IFacetDecorator[] {new SecurityFacetDecorator(authManager)};
        }

        public string Name {
            get { return "TypeAuthorizerInstaller"; }
        }

        public IFacetDecorator[] CreateDecorators(INakedObjectReflector reflector) {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}