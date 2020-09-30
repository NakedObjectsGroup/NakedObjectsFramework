using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core;
using NakedObjects.Menu;

namespace NakedObjects.ParallelReflect.Component {
    internal class NullObjectReflectorConfiguration : IObjectReflectorConfiguration {
        public Type[] TypesToIntrospect => Array.Empty<Type>();
        public Type[] Services => Array.Empty<Type>();
        public string[] ModelNamespaces => Array.Empty<string>();
        public List<Type> SupportedSystemTypes => new List<Type>();
        public Func<IMenuFactory, IMenu[]> MainMenus => null;
        public bool IgnoreCase => false;
        public bool ConcurrencyChecking => true;
        public bool HasConfig() => false;
    }


    internal class NullFunctionalReflectorConfiguration : IFunctionalReflectorConfiguration {
        public Type[] Types => Array.Empty<Type>();
        public Type[] Functions => Array.Empty<Type>();
        public Type[] Services => Array.Empty<Type>();
        public bool ConcurrencyChecking => true;
        public bool IgnoreCase => false;
        public Func<IMenuFactory, IMenu[]> MainMenus => null;
        public bool HasConfig() => false;
    }


    public class CompositeReflectorConfiguration {
        private readonly IFunctionalReflectorConfiguration functionalReflectorConfiguration;
        private readonly IObjectReflectorConfiguration objectReflectorConfiguration;


        public CompositeReflectorConfiguration(IObjectReflectorConfiguration userObjectReflectorConfiguration,
                                               IFunctionalReflectorConfiguration userFunctionalReflectorConfiguration) {
            if (userObjectReflectorConfiguration is null && userFunctionalReflectorConfiguration is null) {
                throw new ReflectionException("Must have object or functional reflector configuration");
            }

            objectReflectorConfiguration = userObjectReflectorConfiguration ?? new NullObjectReflectorConfiguration();
            functionalReflectorConfiguration = userFunctionalReflectorConfiguration ?? new NullFunctionalReflectorConfiguration();

            if (objectReflectorConfiguration.HasConfig() && functionalReflectorConfiguration.HasConfig()) {
                if (objectReflectorConfiguration.ConcurrencyChecking != functionalReflectorConfiguration.ConcurrencyChecking) {
                    throw new ReflectionException("ConcurrencyChecking must be consistent between object and functional reflector configuration");
                }

                if (objectReflectorConfiguration.IgnoreCase != functionalReflectorConfiguration.IgnoreCase) {
                    throw new ReflectionException("Ignore must be consistent between object and functional reflector configuration");
                }
            }

            ConcurrencyChecking = objectReflectorConfiguration.HasConfig()
                ? objectReflectorConfiguration.ConcurrencyChecking
                : functionalReflectorConfiguration.ConcurrencyChecking;

            IgnoreCase = objectReflectorConfiguration.HasConfig()
                ? objectReflectorConfiguration.IgnoreCase
                : functionalReflectorConfiguration.IgnoreCase;
        }

        public bool ConcurrencyChecking { get; }
        
        public bool IgnoreCase { get; }

        public Type[] Services => objectReflectorConfiguration.Services.Union(functionalReflectorConfiguration.Services).ToArray();

        public Type[] ObjectTypes => Services.Union(GetObjectTypesToIntrospect()).ToArray();

        public Type[] RecordTypes => functionalReflectorConfiguration.Types;

        public Type[] FunctionTypes => functionalReflectorConfiguration.Functions;

        public ISet<Type> ServiceTypeSet => new HashSet<Type>(Services);

        public Func<IMenuFactory, IMenu[]> MainMenus() =>
            mf => {
                var om = objectReflectorConfiguration.MainMenus?.Invoke(mf);
                var fm = functionalReflectorConfiguration.MainMenus?.Invoke(mf);

                if (om is null && fm is null) {
                    return null;
                }

                fm ??= Array.Empty<IMenu>();
                om ??= Array.Empty<IMenu>();

                return om.Union(fm).ToArray();
            };

        private static Type EnsureGenericTypeIsComplete(Type type) {
            if (type.IsGenericType && !type.IsConstructedGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericParms = genericType.GetGenericArguments().Select(a => typeof(object)).ToArray();

                return type.GetGenericTypeDefinition().MakeGenericType(genericParms);
            }

            return type;
        }

        private Type[] GetObjectTypesToIntrospect() {
            var types = objectReflectorConfiguration.TypesToIntrospect.Select(EnsureGenericTypeIsComplete);
            var systemTypes = objectReflectorConfiguration.SupportedSystemTypes.Select(EnsureGenericTypeIsComplete);
            return types.Union(systemTypes).ToArray();
        }
    }
}