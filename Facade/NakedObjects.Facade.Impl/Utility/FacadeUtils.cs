// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Facade.Impl.Utility {
    public static class FacadeUtils {
        public static INakedObjectAdapter WrappedAdapter(this IObjectFacade objectFacade) => objectFacade == null ? null : ((ObjectFacade) objectFacade).WrappedNakedObject;

        public static IActionParameterSpec WrappedSpec(this IActionParameterFacade actionParameterFacade) => actionParameterFacade == null ? null : ((ActionParameterFacade) actionParameterFacade).WrappedSpec;

        public static NakedObjectsFacadeException Map(Exception e) =>
            e switch {
                FindObjectException _ => new ObjectResourceNotFoundNOSException(e.Message, e),
                InvalidEntryException _ => new BadRequestNOSException(Resources.NakedObjects.InvalidArguments, e),
                ArgumentException _ => new BadRequestNOSException(Resources.NakedObjects.InvalidArguments, e),
                TargetParameterCountException _ => new BadRequestNOSException("Missing arguments", e), // todo i18n
                InvokeException _ when e.InnerException != null => Map(e.InnerException), // recurse on inner exception
                _ => new GeneralErrorNOSException(e)
            };

        private static string GetUniqueSuffix(IActionSpec action, IActionSpec[] actions) {
            var overloadedActions = actions.Where(a => a.Id == action.Id && actions.Count(ac => ac.Id == a.Id) > 1).ToArray();

            if (overloadedActions.Any()) {
                static string GetParmsString(IActionSpec spec) => spec.Parameters.Aggregate("", (acc, p) => $"{acc}{p.Id}{p.Spec.FullName}");

                var orderedActions = overloadedActions.Select(act => new {act, parmId = GetParmsString(act)}).OrderBy(ap => ap.parmId).Select(ap => ap.act).ToList();

                var suffix = orderedActions.IndexOf(action).ToString(Thread.CurrentThread.CurrentCulture);

                while (actions.Select(a => a.Id).Contains($"{action.Id}{suffix}")) {
                    suffix = $"0{suffix}";
                }

                return suffix;
            }

            return "";
        }

        private static (IActionSpec action, string uid)[] GetOverloadedActionsAndUIds(IActionSpec[] actions) =>
            actions.Where(a => actions.Count(ac => ac.Id == a.Id) > 1).Select(a => (a, GetUniqueSuffix(a, actions))).ToArray();

        public static IActionSpec GetOverloadedAction(string actionName, ITypeSpec spec) {
            var actions = spec.GetActionLeafNodes();
            var overloadedActions = GetOverloadedActionsAndUIds(actions);

            return overloadedActions.Where(oa => $"{oa.action.Id}{oa.uid}" == actionName).Select(oa => oa.action).SingleOrDefault();
        }

        public static string GetOverloadedUId(IActionSpec action, ITypeSpec spec) {
            var actions = spec.GetActionLeafNodes();
            var overloadedActions = GetOverloadedActionsAndUIds(actions);
            return overloadedActions.Where(oa => oa.action == action).Select(oa => oa.uid).SingleOrDefault();
        }

        public static (IActionSpec spec, string uid)[] GetActionsandUidFromSpec(ITypeSpec spec) =>
            spec.GetActionLeafNodes().Select(action => (action, GetOverloadedUId(action, spec))).ToArray();

        public static void AssertNotNull(object o, string msg) {
            if (o == null) {
                throw new NullReferenceException(msg);
            }
        }
    }
}