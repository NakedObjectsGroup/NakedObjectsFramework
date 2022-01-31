using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;
using NOF2.About;

namespace NOF2.Reflector.Component;

public class AboutCache {
    private readonly Dictionary<object, Dictionary<MethodInfo, Dictionary<AboutTypeCodes, IAbout>>> cacheDictionary = new();
    private readonly object staticPlaceholder = new(); 

    public IAbout GetOrCacheAbout(object target, MethodInfo aboutMethod, AboutTypeCodes code, Func<IAbout> toCache) {
        IAbout about;
        // for static methods use method class
        target ??= aboutMethod.DeclaringType;

        if (cacheDictionary.ContainsKey(target)) {
            if (cacheDictionary[target].ContainsKey(aboutMethod)) {
                if (cacheDictionary[target][aboutMethod].ContainsKey(code)) {
                    about = cacheDictionary[target][aboutMethod][code];
                }
                else {
                    about = toCache();
                    cacheDictionary[target][aboutMethod][code] = about;
                }
            }
            else {
                about = toCache();
                cacheDictionary[target][aboutMethod] = new Dictionary<AboutTypeCodes, IAbout> {
                    [code] = about
                };
            }
        }
        else {
            about = toCache();
            cacheDictionary[target] = new Dictionary<MethodInfo, Dictionary<AboutTypeCodes, IAbout>> {
                [aboutMethod] = new() {
                    [code] = about
                }
            };
        }

        return about;
    }
}
