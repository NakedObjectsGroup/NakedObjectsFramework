using System;

namespace NakedObjects.Surface.Nof2.Utility {
    public class DefaultTypeCodeMapper : ITypeCodeMapper {
        public Type TypeFromCode(string code) {
            return Type.GetType(code) ?? SurfaceUtils.GetTypeFromLoadedAssemblies(code);
        }

        public string CodeFromType(Type type) {
            return type.FullName;
        }
    }
}