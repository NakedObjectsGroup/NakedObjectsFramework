// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Facade.Nof2.Contexts;
using org.nakedobjects.@object;
using sdm.systems.application;
using sdm.systems.application.objectstore;
using Action = org.nakedobjects.@object.Action;

namespace NakedObjects.Facade.Nof2.Utility {
    public static class SurfaceUtils {
        public static NakedObjectsSurfaceException Map(Exception e) {
            // map to appropriate exception 

            //if (e is FindObjectException) {
            //    return new ObjectResourceNotFoundNOSException(e.Message, e);
            //}

            if (e is InvalidEntryException) {
                return new BadRequestNOSException("Arguments invalid", e); // todo i18n
            }

            if (e is TargetParameterCountException) {
                return new BadRequestNOSException("Missing arguments", e); // todo i18n
            }

            return new GeneralErrorNOSException(e);
        }

        public static ActionWrapper[] GetActionLeafNodes(this ActionWrapper action) {
            return action.isSet() ? action.getActions().SelectMany(GetActionLeafNodes).ToArray() : new[] {action};
        }

        public static ActionWrapper[] GetActionLeafNodes(this NakedObject nakedObject) {
            return nakedObject.getSpecification().GetActionLeafNodes();
        }

        public static ActionWrapper[] GetObjectActions(this NakedObjectSpecification spec) {
            MethodInfo getObjectActions = typeof (NakedObjectSpecification).GetMethod("getObjectActions");
            FieldInfo t = typeof (Action).GetField("USER");
            object o = t.GetValue(null);
            var result = (object[]) getObjectActions.Invoke(spec, new[] {o});
            return result.Select(r => new ActionWrapper(r)).ToArray();
        }

        public static ActionWrapper[] GetActionLeafNodes(this NakedObjectSpecification spec) {
            return spec.GetObjectActions().SelectMany(GetActionLeafNodes).OrderBy(a => a.getId()).ToArray();
        }

        public static ActionWrapper GetActionLeafNode(this NakedObject nakedObject, string actionName) {
            return nakedObject.GetActionLeafNodes().Single(x => x.getId() == actionName);
        }

        private static string GetShortName(NakedObjectSpecification spec) {
            return spec.getFullName().Split('.').Last();
        }

        public static NakedObjectActionParameter[] GetParameters(this ActionWrapper action, NakedReference obj) {
            var parms = new List<NakedObjectActionParameter>();

            NakedObjectSpecification[] parameterTypes = action.getParameterTypes();
            int index = 0;
            ActionParameterSet set = action.getParameterSet(obj);

            foreach (NakedObjectSpecification nakedObjectSpecification in parameterTypes) {
                string name = (set == null ? GetShortName(nakedObjectSpecification) : set.getParameterLabels()[index] ?? GetShortName(nakedObjectSpecification));
                object[] choices = set == null ? null : set.getOptions()[index];
                object dflt = set == null ? null : set.getDefaultParameterValues()[index];

                parms.Add(new NakedObjectActionParameter(name, index++, nakedObjectSpecification, action, choices, dflt));
            }

            return parms.ToArray();
        }

        public static NakedObjectActionParameter[] GetParameters(this ActionWrapper action) {
            NakedObjectSpecification[] parameterTypes = action.getParameterTypes();
            int index = 0;
            return parameterTypes.Select(nakedObjectSpecification => new NakedObjectActionParameter(GetShortName(nakedObjectSpecification), index++, nakedObjectSpecification, action, null, null)).ToArray();
        }

        public static NakedObject[] GetServicesInternal() {
            var cc = (ClientComponents) ClientSpringContext.SharedClientSpringContext.GetObject("ClientComponents");
            IEnumerable<NakedObject> repositories = cc.Repositories.Values.Cast<object>().Select(o => org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(o));
            return repositories.ToArray();
        }

        // Can I just grab the object like this ? 
        public static IObjectStore GetObjectResolver() {
            var os = (IObjectStore) ClientSpringContext.SharedClientSpringContext.GetObject("NOFObjectStore");
            return os;
        }

        //public static NakedObject[] GetServicesInternal() {
        //    var ss = (SdmStatic) ServerSpringContexts.SdmStaticSpringContext.GetObject("SdmStatic");
        //    IEnumerable<NakedObject> repositories = ss.Repositories.Values.Cast<object>().Select(o => org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(o));
        //    return repositories.ToArray();
        //}

        //public static IObjectStore GetObjectResolver() {
        //    var os = (IObjectStore) ServerSpringContexts.SdmRequestSpringContext.GetObject("NOFObjectStore");
        //    return os;
        //}

        // cloned from typeutils 
        public static Type GetTypeFromLoadedAssemblies(string typeName) {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType(typeName);
                if (type != null) {
                    return type;
                }
            }
            return null;
        }

        public static Type GetType(string typeName) {
            return Type.GetType(typeName) ?? GetTypeFromLoadedAssemblies(typeName);
        }

        // end cloned

        public static string MakeSpaced(string name) {
            List<char> sRev = name.Reverse().ToList();

            for (int i = 0; i < sRev.Count(); i++) {
                if (Char.IsUpper(sRev[i])) {
                    sRev.Insert(i + 1, ' ');
                }
            }
            IEnumerable<char> s = sRev.AsEnumerable().Reverse();
            return new string(sRev.AsEnumerable().Reverse().ToArray()).Trim();
        }

        public static string Capitalize(string name) {
            return char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}