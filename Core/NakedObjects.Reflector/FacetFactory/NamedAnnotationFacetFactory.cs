// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class NamedAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NamedAnnotationFacetFactory));
        //private Type currentType;
        //private IList<string> namesScratchPad = new List<string>();

        public NamedAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Everything) {}

        //public void UpdateScratchPad(Type type) {
        //    if (currentType != type) {
        //        currentType = type;
        //        namesScratchPad = new List<string>();
        //    }
        //}

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Attribute attribute = type.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) type.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Attribute attribute = method.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) method.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            //UpdateScratchPad(property.ReflectedType);
            Attribute attribute = property.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) property.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(CreateProperty(attribute, specification));
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            Attribute attribute = parameter.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) parameter.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        private INamedFacet Create(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }
            var namedAttribute = attribute as NamedAttribute;
            if (namedAttribute != null) {
                return new NamedFacetAnnotation(namedAttribute.Value, holder);
            }
            var nameAttribute = attribute as DisplayNameAttribute;
            if (nameAttribute != null) {
                return new NamedFacetAnnotation(nameAttribute.DisplayName, holder);
            }
            throw new ArgumentException(Log.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"));
        }

        private INamedFacet CreateProperty(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                //return SaveDefaultName(holder);
                return null;
            }
            var namedAttribute = attribute as NamedAttribute;
            if (namedAttribute != null) {
                return Create(namedAttribute, holder);
            }
            var nameAttribute = attribute as DisplayNameAttribute;
            if (nameAttribute != null) {
                return Create(nameAttribute, holder);
            }
            throw new ArgumentException(Log.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"));
        }

        private INamedFacet Create(NamedAttribute attribute, ISpecification holder) {
            return CreateAnnotation(attribute.Value, holder);
        }

        private INamedFacet Create(DisplayNameAttribute attribute, ISpecification holder) {
            return CreateAnnotation(attribute.DisplayName, holder);
        }

        private INamedFacet CreateAnnotation(string name, ISpecification holder) {
            //if (namesScratchPad.Contains(name)) {
            //    Log.WarnFormat("Duplicate name: {0} found on type: {1}", name, currentType.FullName);
            //}
            //namesScratchPad.Add(name);
            return new NamedFacetAnnotation(name, holder);
        }

        //private static bool IsAlwaysHidden(ISpecification holder) {
        //    var hiddenfacet = holder.GetFacet<IHiddenFacet>();
        //    return hiddenfacet != null && hiddenfacet.Value == WhenTo.Always;
        //}

        //private INamedFacet SaveDefaultName(ISpecification holder) {
        //    string name = holder.Identifier.MemberName;
        //    if (!namesScratchPad.Contains(name)) {
        //        if (!TypeUtils.IsNakedObjects(currentType) && !IsAlwaysHidden(holder) && !string.IsNullOrWhiteSpace(name)) {
        //            namesScratchPad.Add(name);
        //        }
        //        return null;
        //    }
        //    return CreateAnnotation(name, holder);
        //}
    }
}