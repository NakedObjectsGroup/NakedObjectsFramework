using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Services;
using NakedObjects.Util;

namespace RestfulObjects.Test.Data {
    public class TestTypeCodeMapper : ITypeCodeMapper {
        private const string DefaultPrefix = "RestfulObjects.Test.Data.";
        private static readonly IDictionary<string, string> PrefixDictionary = new Dictionary<string, string>();

        public Type TypeFromCode(string code) {
            string fullCode = TypeStringFromCode(code);
            return TypeUtils.GetType(fullCode);
        }

        public string CodeFromType(Type type) {
            return CodeFromTypeString(type.FullName);
        }

        public string TypeStringFromCode(string code) {
            if (PrefixDictionary.ContainsKey(code)) {
                return PrefixDictionary[code];
            }
            return DefaultPrefix + code;
        }

        public string CodeFromTypeString(string typeString) {
            string code = typeString.Split('.').Last();
            PrefixDictionary[code] = typeString;
            return code;
        }
    }
}