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
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Naming.Named {
    public class NamedAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NamedAnnotationFacetFactory));
        private Type currentType;
        private IList<string> namesScratchPad = new List<string>();

        public NamedAnnotationFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.Everything) {}

        public void UpdateScratchPad(Type type) {
            if (currentType != type) {
                currentType = type;
                namesScratchPad = new List<string>();
            }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            Attribute attribute = type.GetCustomAttributeByReflection<DisplayNameAttribute>() ?? (Attribute) type.GetCustomAttributeByReflection<NamedAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            Attribute attribute = AttributeUtils.GetCustomAttribute<DisplayNameAttribute>(method) ?? (Attribute) AttributeUtils.GetCustomAttribute<NamedAttribute>(method);
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            UpdateScratchPad(property.ReflectedType);
            Attribute attribute = AttributeUtils.GetCustomAttribute<DisplayNameAttribute>(property) ?? (Attribute) AttributeUtils.GetCustomAttribute<NamedAttribute>(property);
            return FacetUtils.AddFacet(CreateProperty(attribute, specification));
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            Attribute attribute = parameter.GetCustomAttributeByReflection<DisplayNameAttribute>() ?? (Attribute) parameter.GetCustomAttributeByReflection<NamedAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private INamedFacet Create(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }
            if (attribute is NamedAttribute) {
                return new NamedFacetAnnotation(((NamedAttribute) attribute).Value, holder);
            }
            if (attribute is DisplayNameAttribute) {
                return new NamedFacetAnnotation(((DisplayNameAttribute) attribute).DisplayName, holder);
            }
            throw new ArgumentException("Unexpected attribute type: " + attribute.GetType());
        }

        private INamedFacet CreateProperty(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return SaveDefaultName(holder);
            }
            if (attribute is NamedAttribute) {
                return Create((NamedAttribute) attribute, holder);
            }
            if (attribute is DisplayNameAttribute) {
                return Create((DisplayNameAttribute) attribute, holder);
            }
            throw new ArgumentException("Unexpected attribute type: " + attribute.GetType());
        }

        private INamedFacet Create(NamedAttribute attribute, ISpecification holder) {
            return CreateAnnotation(attribute.Value, holder);
        }

        private INamedFacet Create(DisplayNameAttribute attribute, ISpecification holder) {
            return CreateAnnotation(attribute.DisplayName, holder);
        }

        private INamedFacet CreateAnnotation(string name, ISpecification holder) {
            if (namesScratchPad.Contains(name)) {
                Log.WarnFormat("Duplicate name: {0} found on type: {1}", name, currentType.FullName);
            }
            namesScratchPad.Add(name);
            return new NamedFacetAnnotation(name, holder);
        }

        private string SafeGetName(ISpecification holder) {
            if (holder.Identifier != null && holder.Identifier.MemberName != null) {
                return holder.Identifier.MemberName;
            }
            return "";
        }

        private static bool IsAlwaysHidden(ISpecification holder) {
            var hiddenfacet = holder.GetFacet<IHiddenFacet>();
            return hiddenfacet != null && hiddenfacet.Value == WhenTo.Always;
        }

        private INamedFacet SaveDefaultName(ISpecification holder) {
            string name = NameUtils.NaturalName(SafeGetName(holder));
            if (!namesScratchPad.Contains(name)) {
                if (!TypeUtils.IsNakedObjects(currentType) && !IsAlwaysHidden(holder) && !string.IsNullOrWhiteSpace(name)) {
                    namesScratchPad.Add(name);
                }
                return null;
            }
            return CreateAnnotation(name, holder);
        }
    }
}