using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.ParallelReflect.Component {
    public static class FasterTypeUtils {
        private const string SystemTypePrefix = "System.";
        private const string MicrosoftTypePrefix = "Microsoft.";
        private const string NakedObjectsTypePrefix = "NakedObjects.";
        private const string NakedObjectsProxyPrefix = "NakedObjects.Proxy.";
        private const string EntityProxyPrefix = "System.Data.Entity.DynamicProxies.";
        private const string EntityTypePrefix = "NakedObjects.EntityObjectStore.";
        private const string CastleProxyPrefix = "Castle.Proxies.";

        public static bool IsNakedObjectsProxy(Type type) {
            return IsNakedObjectsProxy(type.FullName ?? "");
        }

        public static bool IsNakedObjectsProxy(string typeName) {
            return typeName.StartsWith(NakedObjectsProxyPrefix, StringComparison.Ordinal);
        }

        public static bool IsCastleProxy(Type type) {
            return IsCastleProxy(type.FullName ?? "");
        }

        public static bool IsCastleProxy(string typeName) {
            return typeName.StartsWith(CastleProxyPrefix, StringComparison.Ordinal);
        }

        public static bool IsEntityProxy(Type type) {
            return IsEntityProxy(type.FullName ?? "");
        }

        public static bool IsEntityProxy(string typeName) {
            return typeName.StartsWith(EntityProxyPrefix, StringComparison.Ordinal);
        }

        public static bool IsProxy(Type type) {
            return IsProxy(type.FullName ?? "");
        }

        public static bool IsProxy(string typeName) {
            return IsEntityProxy(typeName) || IsNakedObjectsProxy(typeName) || IsCastleProxy(typeName);
        }

        public static bool IsSystem(string typeName) {
            return typeName.StartsWith(SystemTypePrefix, StringComparison.Ordinal) && !IsEntityProxy(typeName);
        }

        public static bool IsNakedObjects(string typeName) {
            return typeName.StartsWith(NakedObjectsTypePrefix, StringComparison.Ordinal);
        }

        public static bool IsSystem(Type type) {
            return IsSystem(type.FullName ?? "");
        }
    }
}
