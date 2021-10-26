using System;
using System.Collections.Generic;
using System.Reflection;
using Legacy.NakedObjects.Application.Control;
using Legacy.NakedObjects.Reflector.Java.Control;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;

namespace Legacy.Reflector.Component {
    public class LegacyAboutCache {
        private readonly IDictionary<object, IDictionary<MethodInfo, ActionAbout>> cacheDictionary = new Dictionary<object, IDictionary<MethodInfo, ActionAbout>>();

        private ActionAbout InvokeOrReturnCachedAbout(INakedObjectsFramework framework, MethodInfo aboutMethod, object target) {
            ActionAbout InvokeAbout() {
                var about = new SimpleActionAbout(framework.Session, target, Array.Empty<object>());
                aboutMethod.Invoke(target, new object[] { about });
                return about;
            }

            if (!cacheDictionary.ContainsKey(target)) {
                cacheDictionary[target] = new Dictionary<MethodInfo, ActionAbout> { { aboutMethod, InvokeAbout() } };
            }
            else if (!cacheDictionary[target].ContainsKey(aboutMethod)) {
                cacheDictionary[target][aboutMethod] = InvokeAbout();
            }

            return cacheDictionary[target][aboutMethod];
        }

        public static ActionAbout GetActionAbout(INakedObjectsFramework framework, MethodInfo aboutMethod, object target) =>
            framework.ServiceProvider.GetService<LegacyAboutCache>()?.InvokeOrReturnCachedAbout(framework, aboutMethod, target);
    }
}