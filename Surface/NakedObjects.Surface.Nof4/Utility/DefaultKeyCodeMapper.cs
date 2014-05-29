using System;
using System.Linq;
using NakedObjects.Services;

namespace NakedObjects.Surface.Nof4.Utility {
    public class DefaultKeyCodeMapper : IKeyCodeMapper {
        private readonly string keySeparator;

        public DefaultKeyCodeMapper() {
            keySeparator = LinkObjectId.KeySeparator;
        }

        public string[] KeyFromCode(string code, Type type) {
            return code.Split(new[] {keySeparator}, StringSplitOptions.None);
        }

        public string CodeFromKey(string[] key, Type type) {
            return key.Aggregate("", (s, t) => s + (s == "" ? "" : keySeparator) + t);
        }
    }
}