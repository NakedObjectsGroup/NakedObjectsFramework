using System;
using System.Linq;
using NakedObjects.Facade.Translation;

namespace NakedObjects.Surface.Nof2.Utility {
    public class DefaultKeyCodeMapper : IKeyCodeMapper {
        private readonly string keySeparator;

        public DefaultKeyCodeMapper() {
            keySeparator = OidTranslationSlashSeparatedTypeAndIds.KeySeparator;
        }

        public string[] KeyFromCode(string code, Type type) {
            return code.Split(new[] {keySeparator}, StringSplitOptions.None);
        }

        public string CodeFromKey(string[] key, Type type) {
            return key.Aggregate("", (s, t) => s + (s == "" ? "" : keySeparator) + t);
        }
    }
}