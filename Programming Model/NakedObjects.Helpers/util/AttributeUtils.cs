// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NakedObjects.Util {
    /// <summary>
    /// Utility methods for getting custom attributes declared on domain types,
    /// members or method parameters.  The Naked Objects framework makes extensive
    /// use of these utils, but they are provided within the NakedObjects.Helpers
    /// assembly to permit optional use within domain code.
    /// </summary>
    public static class AttributeUtils {
        /// <summary>
        ///     Get a custom attribute on a parameter using reflection
        /// </summary>
        public static T GetCustomAttributeByReflection<T>(this ParameterInfo parameter) where T : Attribute {
            return (T) Attribute.GetCustomAttribute(parameter, typeof (T));
        }

        /// <summary>
        ///     Get a custom attribute on Type using refection
        /// </summary>
        public static T GetCustomAttributeByReflection<T>(this Type type) where T : Attribute {
            return (T) Attribute.GetCustomAttribute(type, typeof (T));
        }

        /// <summary>
        ///     Get a custom attribute on a MemberInfo using the component model.
        /// </summary>
        public static T GetCustomAttributeFromComponentModel<T>(this MemberInfo member) where T : Attribute {
            bool hasMetaData = EnsureComponentModelInitialised(member);
            if (hasMetaData) {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(member.DeclaringType).Find(member.Name, true);
                return propDesc != null ? propDesc.Attributes.OfType<T>().FirstOrDefault() : null;
            }
            return null;
        }

        /// <summary>
        ///     Get a custom attribute on a MemberInfo first trying the component model then reflection
        /// </summary>
        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute {
            return member.GetCustomAttributeFromComponentModel<T>() ??
                   member.GetCustomAttributeByReflection<T>();
        }

        #region private

        private static readonly Dictionary<Type, bool> MetadataTypes = new Dictionary<Type, bool>();

        private static T GetCustomAttributeByReflection<T>(this MemberInfo member) where T : Attribute {
            return (T) Attribute.GetCustomAttribute(member, typeof (T));
        }

        private static bool EnsureComponentModelInitialised(MemberInfo member) {
            lock (MetadataTypes) {
                if (MetadataTypes.ContainsKey(member.DeclaringType)) {
                    return MetadataTypes[member.DeclaringType];
                }

                bool hasMetaData = false;

                if (member.DeclaringType.GetCustomAttributeByReflection<MetadataTypeAttribute>() != null) {
                    var provider = new AssociatedMetadataTypeTypeDescriptionProvider(member.DeclaringType);
                    TypeDescriptor.AddProvider(provider, member.DeclaringType);
                    hasMetaData = true;
                }

                MetadataTypes.Add(member.DeclaringType, hasMetaData);
                return hasMetaData;
            }
        }

        #endregion
    }
}