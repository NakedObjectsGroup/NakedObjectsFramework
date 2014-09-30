// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Facets.Objects.Key;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect.Proxies {
    public static class ProxyCreator {
       
        private static readonly object ModuleBuilderLock = new object();
        private static readonly ILog Log = LogManager.GetLogger(typeof (ProxyCreator));
        private static ModuleBuilder ModuleBuilder { get; set; }

        private static void EnsureModuleBuilderExists() {
            if (ModuleBuilder == null) {
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("NakedObjectsProxies"), AssemblyBuilderAccess.Run);
                ModuleBuilder = assemblyBuilder.DefineDynamicModule("NakedObjectsProxies");
            }
        }

        private static string GetProxyTypeName(Type typeToProxy) {
            const string ns = "NakedObjects.Proxy.";

            if (typeToProxy.IsGenericType) {
                Type genericType = typeToProxy.GetGenericTypeDefinition();
                Type[] typeParms = typeToProxy.GetGenericArguments();

                string name = ns + genericType.FullName;
                return typeParms.Aggregate(name, (s, t) => s + "." + t.FullName);
            }
            return ns + typeToProxy.FullName;
        }

        public static Type CreateProxyType(INakedObjectReflector reflector, ILifecycleManager persistor, Type typeToProxy) {
            // do not proxy EF domain objects 

            if (TypeUtils.IsEntityDomainObject(typeToProxy) ||
                CollectionUtils.IsCollection(typeToProxy) ||
                !CanProxyType(typeToProxy)) {
                return typeToProxy;
            }

            lock (ModuleBuilderLock) {
                EnsureModuleBuilderExists();
                try {
                    string typeName = GetProxyTypeName(typeToProxy);

                    Type proxyType = ModuleBuilder.GetType(typeName);

                    if (proxyType == null) {
                        TypeBuilder typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeToProxy);
                        FieldBuilder containerField = CreateContainerProperty(typeBuilder);
                        CreateProperties(reflector, typeBuilder, typeToProxy, containerField);
                        SubclassAllCollectionAccessors(reflector, persistor, typeBuilder, typeToProxy, containerField);
                        proxyType = typeBuilder.CreateType();
                    }
                    return proxyType;
                }
                catch (Exception e) {
                    Log.ErrorFormat("Failed to create proxy for {0} reason {1}", typeToProxy.FullName, e.Message);
                    throw;
                }
            }
        }

        private static void CreateProperties(INakedObjectReflector reflector, TypeBuilder typeBuilder, Type typeToProxy, FieldBuilder containerField) {
            // do not proxy key properties as we don't want ObjectChanged called when key is set
            foreach (INakedObjectAssociation assoc in reflector.LoadSpecification(typeToProxy).Properties) {
                PropertyInfo property = typeToProxy.GetProperty(assoc.Id);

                if (!assoc.ContainsFacet<IKeyFacet>()) {
                    CreateBaseSetter(typeBuilder, property, containerField);
                }

                CreateBaseGetter(typeBuilder, property, containerField);
            }
        }

        private static void AddObjectChangedAndReturn(ILGenerator iLGenerator, FieldBuilder containerField) {
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, containerField);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Callvirt, typeof (IDomainObjectContainer).GetMethod("ObjectChanged"));
            iLGenerator.Emit(OpCodes.Ret);
        }

        private static void SubclassCollectionAccessor(MethodInfo method, TypeBuilder typeBuilder, FieldBuilder containerField) {
            MethodAttributes attributes = method.Attributes & MethodAttributes.MemberAccessMask;

            Type[] types = method.GetParameters().Select(pi => pi.ParameterType).ToArray();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, attributes | (MethodAttributes.Virtual), null, types);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();

            // call base method 
            CallBaseMethod(method, types, iLGenerator);

            // call object changed on container 
            AddObjectChangedAndReturn(iLGenerator, containerField);
        }

        private static void CallBaseMethod(MethodInfo method, Type[] types, ILGenerator iLGenerator) {
            switch (types.Count()) {
                case 0:
                    CallBaseMethodNoParms(method, iLGenerator);
                    break;
                case 1:
                    CallBaseMethodOneParm(method, iLGenerator);
                    break;
                default:
                    throw new NakedObjectSystemException(string.Format("Error when proxying method {0} on type {1} unexpected number of parms {2}", method.Name, method.DeclaringType.Name, types.Count()));
            }
        }

        private static void CallBaseMethodOneParm(MethodInfo method, ILGenerator iLGenerator) {
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Call, method);
        }

        private static void CallBaseMethodNoParms(MethodInfo method, ILGenerator iLGenerator) {
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, method);
        }

        private static void SubclassCollectionAccessorIfFound(Type typeToProxy, string methodName, TypeBuilder typeBuilder, FieldBuilder containerField) {
            MethodInfo method = typeToProxy.GetMethods().SingleOrDefault(m => m.Name == methodName && m.IsVirtual);

            if (method != null) {
                SubclassCollectionAccessor(method, typeBuilder, containerField);
            }
        }

        private static void SubclassCollectionAccessors(TypeBuilder typeBuilder, Type typeToProxy, FieldBuilder containerField, string captializedName) {
            var prefixesToUse = new[] {
                PrefixesAndRecognisedMethods.ClearPrefix
            };

            prefixesToUse.Select(p => p + captializedName).
                          ForEach(name => SubclassCollectionAccessorIfFound(typeToProxy, name, typeBuilder, containerField));
        }

        private static void SubclassAllCollectionAccessors(INakedObjectReflector reflector, ILifecycleManager persistor, TypeBuilder typeBuilder, Type typeToProxy, FieldBuilder containerField) {
            INakedObjectAssociation[] associations = reflector.LoadSpecification(typeToProxy).Properties.Where(a => a.IsCollection).ToArray();

            associations.ForEach(assoc => SubclassCollectionAccessors(typeBuilder, typeToProxy, containerField, assoc.GetName(persistor)));
        }

        private static FieldBuilder CreateContainerProperty(TypeBuilder typeBuilder) {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_proxycontainer", typeof (IDomainObjectContainer), FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("ProxyContainer", PropertyAttributes.None, typeof (IDomainObjectContainer), Type.EmptyTypes);

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_ProxyContainer", MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Family, typeof (IDomainObjectContainer), Type.EmptyTypes);
            ILGenerator iLGenerator = getMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_ProxyContainer", MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, null, new[] {typeof (IDomainObjectContainer)});

            iLGenerator = setMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
            return fieldBuilder;
        }

        private static void CreateBaseGetter(TypeBuilder typeBuilder, PropertyInfo baseProperty, FieldBuilder containerField) {
            if (CanProxyGetter(baseProperty)) {
                MethodInfo getMethod = baseProperty.GetGetMethod(true);
                MethodAttributes attributes = getMethod.Attributes & MethodAttributes.MemberAccessMask;
                MethodBuilder methodBuilder = typeBuilder.DefineMethod("get_" + baseProperty.Name, attributes | (MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual), baseProperty.PropertyType, Type.EmptyTypes);
                ILGenerator iLGenerator = methodBuilder.GetILGenerator();

                // call resolve on container 
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, containerField);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Callvirt, typeof (IDomainObjectContainer).GetMethod("Resolve", new[] {typeof (object)}));

                // call base getter 
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Call, getMethod);
                iLGenerator.Emit(OpCodes.Ret);
            }
        }

        private static void CreateBaseSetter(TypeBuilder typeBuilder, PropertyInfo baseProperty, FieldBuilder containerField) {
            if (CanProxySetter(baseProperty)) {
                MethodInfo setMethod = baseProperty.GetSetMethod(true);
                MethodAttributes attributes = setMethod.Attributes & MethodAttributes.MemberAccessMask;
                MethodBuilder methodBuilder = typeBuilder.DefineMethod("set_" + baseProperty.Name, attributes | (MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual), null, new[] {baseProperty.PropertyType});
                ILGenerator iLGenerator = methodBuilder.GetILGenerator();

                // call base setter 
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Call, setMethod);

                // call object changed on container 
                AddObjectChangedAndReturn(iLGenerator, containerField);
            }
        }

        private static bool CanProxyType(Type type) {
            return type.GetProperties().Any(CanProxyProperty);
        }

        private static bool CanProxyProperty(PropertyInfo property) {
            return CanProxySetter(property) || CanProxyGetter(property);
        }

        private static bool CanProxySetter(PropertyInfo property) {
            return CanProxyMethod(property.GetSetMethod(true));
        }

        private static bool CanProxyGetter(PropertyInfo property) {
            return CanProxyMethod(property.GetGetMethod(true));
        }

        private static bool CanProxyMethod(MethodBase method) {
            if (method != null) {
                MethodAttributes attributes = method.Attributes & MethodAttributes.MemberAccessMask;
                return (method.IsVirtual && !method.IsFinal) &&
                       (attributes == MethodAttributes.Public ||
                        attributes == MethodAttributes.Family ||
                        attributes == MethodAttributes.FamORAssem);
            }
            return false;
        }
    }
}