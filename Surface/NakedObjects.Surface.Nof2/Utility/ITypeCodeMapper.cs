using System;

namespace NakedObjects.Surface.Nof2.Utility {
    public interface ITypeCodeMapper {
        Type TypeFromCode(string code);
        string CodeFromType(Type type);
    }
}