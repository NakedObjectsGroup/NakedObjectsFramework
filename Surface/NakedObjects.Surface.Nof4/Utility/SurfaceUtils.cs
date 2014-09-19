// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Surface.Nof4.Utility {
    public static class SurfaceUtils {
        public static NakedObjectsSurfaceException Map(Exception e) {
            // map to appropriate exception 

            if (e is FindObjectException) {
                return new ObjectResourceNotFoundNOSException(e.Message, e);
            }

            if (e is InvalidEntryException || e is ArgumentException) {
                return new BadRequestNOSException("Arguments invalid", e); // todo i18n
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

        private static string GetUniqueSuffix(INakedObjectAction action, INakedObjectAction[] actions) {
            INakedObjectAction[] overloadedActions = actions.Where(a => a.Id == action.Id && actions.Count(ac => ac.Id == a.Id) > 1).ToArray();

            if (overloadedActions.Any()) {
                var actionAndParms = overloadedActions.Select(a => new Tuple<INakedObjectAction, string>(a, ((Func<INakedObjectAction, string>) (act => act.Parameters.Aggregate("", (acc, p) => a + p.Id + p.Specification.FullName)))(a)));

                int index = 0;
                var orderedActions = actionAndParms.OrderBy(ap => ap.Item2).Select(ap => ap.Item1).ToDictionary(a => a, a => index++);

                var suffix = orderedActions[action].ToString();

                while (actions.Select(a => a.Id).Contains(action.Id + suffix)) {
                    suffix = "0" + suffix;
                }

                return suffix;
            }
            return "";
        }


        private static Tuple<INakedObjectAction, string>[] GetOverloadedActionsAndUIds(INakedObjectAction[] actions) {
            INakedObjectAction[] overloadedActions = actions.Where(a => actions.Count(ac => ac.Id == a.Id) > 1).ToArray();

            if (overloadedActions.Any()) {
                return overloadedActions.Select(a => new Tuple<INakedObjectAction, string>(a, GetUniqueSuffix(a, actions))).ToArray();
            }
            return new Tuple<INakedObjectAction, string>[] {};
        }

        public static INakedObjectAction GetOverloadedAction(string actionName, INakedObjectSpecification spec) {
            INakedObjectAction action = null;
            INakedObjectAction[] actions = spec.GetActionLeafNodes();
            Tuple<INakedObjectAction, string>[] overloadedActions = GetOverloadedActionsAndUIds(actions);

            if (overloadedActions.Any()) {
                Tuple<INakedObjectAction, string> matchingAction = overloadedActions.SingleOrDefault(oa => oa.Item1.Id + oa.Item2 == actionName);
                if (matchingAction != null) {
                    action = matchingAction.Item1;
                }
            }
            return action;
        }

        public static string GetOverloadedUId(INakedObjectAction action, INakedObjectSpecification spec) {
            INakedObjectAction[] actions = spec.GetActionLeafNodes();
            Tuple<INakedObjectAction, string>[] overloadedActions = GetOverloadedActionsAndUIds(actions);
            return overloadedActions.Where(oa => oa.Item1 == action).Select(oa => oa.Item2).SingleOrDefault();
        }

        public static Tuple<INakedObjectAction, string> GetActionandUidFromSpec(INakedObjectSpecification spec, string actionName, string typeName) {
            INakedObjectAction[] actions = spec.GetActionLeafNodes();
            INakedObjectAction action = actions.SingleOrDefault(p => p.Id == actionName) ?? GetOverloadedAction(actionName, spec);

            if (action == null) {
                throw new TypeActionResourceNotFoundNOSException(actionName, typeName);
            }

            string uid = GetOverloadedUId(action, spec);
            return new Tuple<INakedObjectAction, string>(action, uid);
        }

        public static Tuple<INakedObjectAction, string>[] GetActionsandUidFromSpec(INakedObjectSpecification spec) {
            INakedObjectAction[] actions = spec.GetActionLeafNodes();
            return actions.Select(action => new Tuple<INakedObjectAction, string>(action, GetOverloadedUId(action, spec))).ToArray();
        }
    }
}