using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;

namespace NakedLegacy.Reflector.Component;

public class LegacyAboutCache {
    private readonly IDictionary<object, IDictionary<MethodInfo, IActionAbout>> cacheDictionary = new Dictionary<object, IDictionary<MethodInfo, IActionAbout>>();

    private IActionAbout InvokeOrReturnCachedAbout(INakedFramework framework, MethodInfo aboutMethod, object target) =>
        //ActionAbout InvokeAbout() {
        //    var about = new ActionAboutImpl();
        //    aboutMethod.Invoke(target, new object[] { about });
        //    return about;
        //}
        //if (!cacheDictionary.ContainsKey(target)) {
        //    cacheDictionary[target] = new Dictionary<MethodInfo, ActionAbout> { { aboutMethod, InvokeAbout() } };
        //}
        //else if (!cacheDictionary[target].ContainsKey(aboutMethod)) {
        //    cacheDictionary[target][aboutMethod] = InvokeAbout();
        //}
        cacheDictionary[target][aboutMethod];

    public static IActionAbout GetActionAbout(INakedFramework framework, MethodInfo aboutMethod, object target) =>
        framework.ServiceProvider.GetService<LegacyAboutCache>()?.InvokeOrReturnCachedAbout(framework, aboutMethod, target);

    public static IActionAbout GetFieldAbout(INakedFramework framework, MethodInfo aboutMethod, object target) =>
        framework.ServiceProvider.GetService<LegacyAboutCache>()?.InvokeOrReturnCachedAbout(framework, aboutMethod, target);
}