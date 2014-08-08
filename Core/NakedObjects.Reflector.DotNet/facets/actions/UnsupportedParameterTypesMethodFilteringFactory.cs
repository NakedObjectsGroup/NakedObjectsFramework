// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    public class UnsupportedParameterTypesMethodFilteringFactory : FacetFactoryAbstract, IMethodFilteringFacetFactory {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UnsupportedParameterTypesMethodFilteringFactory));

        public UnsupportedParameterTypesMethodFilteringFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ActionsOnly) {}

        #region IMethodFilteringFacetFactory Members

        public bool Filters(MethodInfo method) {
            if (method.IsGenericMethod) {
                Log.InfoFormat("Ignoring method: {0}.{1} because it is generic", method.DeclaringType.FullName, method.Name);
                return true;
            }

            foreach (ParameterInfo parameterInfo in method.GetParameters()) {
                if (((DotNetReflector) Reflector).ClassStrategy.IsTypeUnsupportedByReflector(parameterInfo.ParameterType)) {
                    Log.InfoFormat("Ignoring method: {0}.{1} because parameter '{2}' is of type {3}", method.DeclaringType.FullName, method.Name, parameterInfo.Name, parameterInfo.ParameterType);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}