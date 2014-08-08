// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.Peer {
    public class IdentifierImpl : IIdentifier {
        private readonly INakedObjectReflector reflector;
        private readonly string className;
        private readonly bool isField;
        private readonly string name;
        private readonly string[] parameterNames;
        private readonly string[] parameterTypes;
        private string asString;
        private string identityString;

        public IdentifierImpl(INakedObjectReflector reflector, string className)
            : this(reflector, className, "", new string[0], new string[0], false) { }

        public IdentifierImpl(INakedObjectReflector reflector, string className, string fieldName)
            : this(reflector, className, fieldName, new string[0], new string[0], true) { }

        public IdentifierImpl(INakedObjectReflector reflector, string className, string methodName, ParameterInfo[] parameters)
            : this(reflector, className, methodName, parameters.Select(p => p.Name).ToArray(), ToParameterStringArray(parameters.Select(p => p.ParameterType).ToArray()), false) { }

        public IdentifierImpl(INakedObjectReflector reflector, string className, string methodName, string[] parameterTypeNames)
            : this(reflector, className, methodName, parameterTypeNames.Select(p => "").ToArray(), parameterTypeNames, false) { }

        private IdentifierImpl(INakedObjectReflector reflector, string className, string fieldName, string[] parameterNames, string[] parameterTypeNames, bool isField) {
            this.reflector = reflector;
            this.className = className;
            name = fieldName;
            parameterTypes = parameterTypeNames;
            this.parameterNames = parameterNames;
            this.isField = isField;
        }

        #region IIdentifier Members

        public virtual string ClassName {
            get { return className; }
        }

        public virtual string MemberName {
            get { return name; }
        }

        public virtual string[] MemberParameterTypeNames {
            get { return parameterTypes; }
        }

        public virtual string[] MemberParameterNames {
            get { return parameterNames; }
        }

        public virtual bool IsField {
            get { return isField; }
        }

        public virtual INakedObjectSpecification[] MemberParameterSpecifications {
            get {
                var specifications = new List<INakedObjectSpecification>();

                parameterTypes.ForEach(x => specifications.Add(reflector.LoadSpecification(TypeNameUtils.DecodeTypeName(x))));
                return specifications.ToArray();
            }
        }

        public virtual string ToIdentityString(IdentifierDepth depth) {
            Assert.AssertTrue(depth >= IdentifierDepth.Class && depth <= IdentifierDepth.Parms);
            switch (depth) {
                case (IdentifierDepth.Class):
                    return ToClassIdentityString();
                case (IdentifierDepth.ClassName):
                    return ToClassAndNameIdentityString();
                case (IdentifierDepth.ClassNameParams):
                    return ToFullIdentityString();
                case (IdentifierDepth.Name):
                    return ToNameIdentityString();
                case (IdentifierDepth.Parms):
                    return ToParmsIdentityString();
            }
            return null;
        }

        public virtual string ToIdentityStringWithCheckType(IdentifierDepth depth, CheckType checkType) {
            return ToIdentityString(depth) + ":" + checkType;
        }

        public int CompareTo(object o2) {
            return string.CompareOrdinal(ToString(), o2.ToString());
        }

        #endregion

        private static string FullName(Type type) {
            if (type.IsGenericType) {
                if (CollectionUtils.IsGenericEnumerable(type)) {
                    return TypeNameUtils.EncodeGenericTypeName(type);
                }
                if (type.Name.StartsWith("Nullable")) {
                    return type.GetGenericArguments()[0].FullName + "?";
                }
            }
            return type.FullName;
        }


        private static string[] ToParameterStringArray(Type[] fromArray) {
            var parameters = new List<string>();
            fromArray.ForEach(x => parameters.Add(FullName(x)));
            return parameters.ToArray();
        }

        public override bool Equals(object obj) {
            if (this == obj) {
                return true;
            }
            if (obj is IdentifierImpl) {
                var other = (IdentifierImpl) obj;
                return string.Equals(other.className, className) && string.Equals(other.name, other.name) && Equals(other.parameterTypes, parameterTypes);
            }
            return false;
        }

        private static bool Equals(string[] a, string[] b) {
            if (a == null && b == null) {
                return true;
            }

            if (a == null || b == null) {
                // since not both null must be different
                return false;
            }

            if (a.Length != b.Length) {
                return false;
            }

            for (int i = 0; i < b.Length; i++) {
                if (!string.Equals(a[i], b[i])) {
                    return false;
                }
            }

            return true;
        }

        public override string ToString() {
            if (asString == null) {
                var str = new StringBuilder();
                str.Append(className).Append('#').Append(name).Append('(');
                for (int i = 0; i < parameterTypes.Length; i++) {
                    if (i > 0) {
                        str.Append(", ");
                    }
                    str.Append(parameterTypes[i]);
                }
                str.Append(')');
                asString = str.ToString();
            }
            return asString;
        }

        public virtual string ToClassIdentityString() {
            return className;
        }

        public virtual string ToNameIdentityString() {
            return name;
        }

        public virtual string ToClassAndNameIdentityString() {
            return ToClassIdentityString() + "#" + name;
        }

        public virtual string ToParmsIdentityString() {
            var str = new StringBuilder();
            if (!IsField) {
                str.Append('(');
                for (int i = 0; i < parameterTypes.Length; i++) {
                    if (i > 0) {
                        str.Append(",");
                    }
                    str.Append(parameterTypes[i]);
                }
                str.Append(')');
            }
            return str.ToString();
        }

        public virtual string ToFullIdentityString() {
            if (identityString == null) {
                if (name.Length == 0) {
                    identityString = ToClassIdentityString();
                }
                else {
                    var str = new StringBuilder();
                    str.Append(ToClassAndNameIdentityString());
                    str.Append(ToParmsIdentityString());
                    identityString = str.ToString();
                }
            }
            return identityString;
        }

        public static IdentifierImpl FromIdentityString(INakedObjectReflector reflector, string asString) {
            Assert.AssertNotNull(asString);
            int indexOfHash = asString.IndexOf("#");
            int indexOfOpenBracket = asString.IndexOf("(");
            int indexOfCloseBracket = asString.IndexOf(")");
            string className = asString.Substring(0, (indexOfHash == -1 ? asString.Length : indexOfHash) - (0));
            if (indexOfHash == -1 || indexOfHash == (asString.Length - 1)) {
                return new IdentifierImpl(reflector, className);
            }
            string name;
            if (indexOfOpenBracket == -1) {
                name = asString.Substring(indexOfHash + 1);
                return new IdentifierImpl(reflector, className, name);
            }
            name = asString.Substring(indexOfHash + 1, (indexOfOpenBracket) - (indexOfHash + 1));
            string allParms = asString.Substring(indexOfOpenBracket + 1, (indexOfCloseBracket) - (indexOfOpenBracket + 1)).Trim();
            string[] parms = allParms.Length > 0 ? allParms.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries) : new string[] {};
            return new IdentifierImpl(reflector, className, name, parms);
        }

        public override int GetHashCode() {
            return (className + name + parameterTypes.Aggregate("", (s, t) => s + t)).GetHashCode();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}