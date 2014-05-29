using System;
using NakedObjects.Services;
using NakedObjects.Util;

namespace NakedObjects.Surface.Nof4.Utility {
    public class DefaultTypeCodeMapper : ITypeCodeMapper {
        public Type TypeFromCode(string code) {
            return Type.GetType(code) ?? TypeUtils.GetType(code);
        }

        public string CodeFromType(Type type) {
            return type.FullName;
        }
    }
}