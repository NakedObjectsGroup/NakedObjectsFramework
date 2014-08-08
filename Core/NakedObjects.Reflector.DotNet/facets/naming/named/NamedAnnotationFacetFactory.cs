// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Naming.Named {
    public class NamedAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NamedAnnotationFacetFactory));
        private Type currentType;
        private IList<string> namesScratchPad = new List<string>();

        public NamedAnnotationFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.Everything) { }

        public void UpdateScratchPad(Type type) {
            if (currentType != type) {
                currentType = type;
                namesScratchPad = new List<string>();
            }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            Attribute attribute = type.GetCustomAttributeByReflection<DisplayNameAttribute>() ?? (Attribute) type.GetCustomAttributeByReflection<NamedAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            Attribute attribute = method.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) method.GetCustomAttribute<NamedAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            UpdateScratchPad(property.ReflectedType);
            Attribute attribute = property.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) property.GetCustomAttribute<NamedAttribute>();
            return FacetUtils.AddFacet(CreateProperty(attribute, holder));
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            Attribute attribute = parameter.GetCustomAttributeByReflection<DisplayNameAttribute>() ?? (Attribute) parameter.GetCustomAttributeByReflection<NamedAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private INamedFacet Create(Attribute attribute, IFacetHolder holder) {
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

        private INamedFacet CreateProperty(Attribute attribute, IFacetHolder holder) {
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

        private INamedFacet Create(NamedAttribute attribute, IFacetHolder holder) {
            return CreateAnnotation(attribute.Value, holder);
        }

        private INamedFacet Create(DisplayNameAttribute attribute, IFacetHolder holder) {
            return CreateAnnotation(attribute.DisplayName, holder);
        }

        private INamedFacet CreateAnnotation(string name, IFacetHolder holder) {
            if (namesScratchPad.Contains(name)) {
                Log.WarnFormat("Duplicate name: {0} found on type: {1}", name, currentType.FullName);
            }
            namesScratchPad.Add(name);
            return new NamedFacetAnnotation(name, holder);
        }

        private string SafeGetName(IFacetHolder holder) {
            if (holder.Identifier != null && holder.Identifier.MemberName != null) {
                return holder.Identifier.MemberName;
            }
            return "";
        }

        private static bool IsAlwaysHidden(IFacetHolder holder) {
            var hiddenfacet = holder.GetFacet<IHiddenFacet>();
            return hiddenfacet != null && hiddenfacet.Value == When.Always;
        }

        private INamedFacet SaveDefaultName(IFacetHolder holder) {
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