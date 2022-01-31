using System;
using System.Reflection;
using NOF2.About;

namespace NOF2.Reflector.Component;

public interface IAboutCache {
    public IAbout GetOrCacheAbout(object target, MethodInfo aboutMethod, AboutTypeCodes code, Func<IAbout> toCache);
}