using System;

namespace NakedObjects.Surface.Nof2.Utility {
    public interface IKeyCodeMapper {
        string[] KeyFromCode(string code, Type type);
        string CodeFromKey(string[] key, Type type);
    }
}