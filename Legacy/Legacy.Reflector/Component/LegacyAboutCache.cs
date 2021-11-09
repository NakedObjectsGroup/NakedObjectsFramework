using System;
using System.Collections.Generic;
using System.Reflection;
using Legacy.Types;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;

namespace Legacy.Reflector.Component {
    public class LegacyAboutCache {
        private readonly IDictionary<object, IDictionary<MethodInfo, ActionAbout>> cacheDictionary = new Dictionary<object, IDictionary<MethodInfo, ActionAbout>>();

        private ActionAbout InvokeOrReturnCachedAbout(INakedFramework framework, MethodInfo aboutMethod, object target) {
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

            return cacheDictionary[target][aboutMethod];
        }

        public static ActionAbout GetActionAbout(INakedFramework framework, MethodInfo aboutMethod, object target) =>
            framework.ServiceProvider.GetService<LegacyAboutCache>()?.InvokeOrReturnCachedAbout(framework, aboutMethod, target);

        public static ActionAbout GetFieldAbout(INakedFramework framework, MethodInfo aboutMethod, object target) =>
            framework.ServiceProvider.GetService<LegacyAboutCache>()?.InvokeOrReturnCachedAbout(framework, aboutMethod, target);
    }
}