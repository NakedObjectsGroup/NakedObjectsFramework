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
        public static INakedObjectAdapter WrappedAdapter(this IObjectFacade objectFacade) {
            return objectFacade == null ? null : ((ObjectFacade) objectFacade).WrappedNakedObject;
        }

        public static IActionParameterSpec WrappedSpec(this IActionParameterFacade actionParameterFacade) {
            return actionParameterFacade == null ? null : ((ActionParameterFacade) actionParameterFacade).WrappedSpec;
        }

        public static NakedObjectsFacadeException Map(Exception e) {
            // map to appropriate exception 

            if (e is FindObjectException) {
                return new ObjectResourceNotFoundNOSException(e.Message, e);
            }

            if (e is InvalidEntryException || e is ArgumentException) {
                return new BadRequestNOSException(Resources.NakedObjects.InvalidArguments, e);
            }

            if (e is TargetParameterCountException) {
                return new BadRequestNOSException("Missing arguments", e); // todo i18n
            }

            if (e is InvokeException && e.InnerException != null) {
                // recurse on inner exception
                return Map(e.InnerException);
            }

            return new GeneralErrorNOSException(e);
        }

        private static string GetUniqueSuffix(IActionSpec action, IActionSpec[] actions) {
            IActionSpec[] overloadedActions = actions.Where(a => a.Id == action.Id && actions.Count(ac => ac.Id == a.Id) > 1).ToArray();

            if (overloadedActions.Any()) {
                var actionAndParms = overloadedActions.Select(a => new Tuple<IActionSpec, string>(a, ((Func<IActionSpec, string>) (act => act.Parameters.Aggregate("", (acc, p) => a + p.Id + p.Spec.FullName)))(a)));

                int index = 0;
                var orderedActions = actionAndParms.OrderBy(ap => ap.Item2).Select(ap => ap.Item1).ToDictionary(a => a, a => index++);

                var suffix = orderedActions[action].ToString(Thread.CurrentThread.CurrentCulture);

                while (actions.Select(a => a.Id).Contains(action.Id + suffix)) {
                    suffix = "0" + suffix;
                }

                return suffix;
            }
            return "";
        }

        private static Tuple<IActionSpec, string>[] GetOverloadedActionsAndUIds(IActionSpec[] actions) {
            IActionSpec[] overloadedActions = actions.Where(a => actions.Count(ac => ac.Id == a.Id) > 1).ToArray();

            if (overloadedActions.Any()) {
                return overloadedActions.Select(a => new Tuple<IActionSpec, string>(a, GetUniqueSuffix(a, actions))).ToArray();
            }
            return new Tuple<IActionSpec, string>[] {};
        }

        public static IActionSpec GetOverloadedAction(string actionName, ITypeSpec spec) {
            IActionSpec action = null;
            IActionSpec[] actions = spec.GetActionLeafNodes();
            Tuple<IActionSpec, string>[] overloadedActions = GetOverloadedActionsAndUIds(actions);

            if (overloadedActions.Any()) {
                Tuple<IActionSpec, string> matchingAction = overloadedActions.SingleOrDefault(oa => oa.Item1.Id + oa.Item2 == actionName);
                if (matchingAction != null) {
                    action = matchingAction.Item1;
                }
            }
            return action;
        }

        public static string GetOverloadedUId(IActionSpec action, ITypeSpec spec) {
            IActionSpec[] actions = spec.GetActionLeafNodes();
            Tuple<IActionSpec, string>[] overloadedActions = GetOverloadedActionsAndUIds(actions);
            return overloadedActions.Where(oa => oa.Item1 == action).Select(oa => oa.Item2).SingleOrDefault();
        }

        public static Tuple<IActionSpec, string>[] GetActionsandUidFromSpec(ITypeSpec spec) {
            IActionSpec[] actions = spec.GetActionLeafNodes();
            return actions.Select(action => new Tuple<IActionSpec, string>(action, GetOverloadedUId(action, spec))).ToArray();
        }

        public static void AssertNotNull(object o, string msg) {
            if (o == null) {
                throw new NullReferenceException(msg);
            }
        }
    }
}