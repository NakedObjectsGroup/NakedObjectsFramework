// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Reflect;
using System.Linq;
using NakedObjects.Util;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Component;
using NakedObjects.Core;

namespace NakedObjects.Reflect.FacetFactory {

    /// <summary>
    /// Does not add any facets, but removes members that should be ignored - before they are introspected upon
    /// by other factories.  This factory thus needs to be registered earlier than most other factories.
    /// </summary>
    public class RemoveIgnoredMethodsFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public RemoveIgnoredMethodsFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsAndInterfaces) {
        }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder spec) {
            //var attr = type.GetCustomAttribute<NakedObjectsTypeAttribute>();
            //if (attr == null) {
                RemoveExplicitlyIgnoredMembers(type, methodRemover);
            //} else {
            //    switch (attr.ReflectionScope) {
            //        case ReflectOver.All:
            //            RemoveExplicitlyIgnoredMembers(type, methodRemover);
            //        break;
            //        case ReflectOver.TypeOnlyNoMembers:
            //            foreach (MethodInfo method in type.GetMethods()) {
            //                methodRemover.RemoveMethod(method);
            //            }
            //            break;
            //        case ReflectOver.ExplicitlyIncludedMembersOnly:
            //            foreach (MethodInfo method in type.GetMethods()) {
            //                if (method.GetCustomAttribute<NakedObjectsIncludeAttribute>() == null) {
            //                    methodRemover.RemoveMethod(method);
            //                }
            //            }
            //            break;
            //        case ReflectOver.None:
            //            throw new ReflectionException("Attempting to introspect a class that has been marked with NakedObjectsType with ReflectOver.None");
            //        default:
            //            throw new ReflectionException(String.Format("Unhandled value for ReflectOver: {0}", attr.ReflectionScope));
            //    }
            //}
        }

        private static void RemoveExplicitlyIgnoredMembers(Type type, IMethodRemover methodRemover) {
            foreach (MethodInfo method in type.GetMethods().Where(m => m.GetCustomAttribute<NakedObjectsIgnoreAttribute>() != null)) {
                methodRemover.RemoveMethod(method);
            }
        }
    }
}
