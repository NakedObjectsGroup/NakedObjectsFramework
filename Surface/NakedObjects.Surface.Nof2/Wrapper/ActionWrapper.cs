// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using System.Reflection;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using NotImplementedException = System.NotImplementedException;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class ActionWrapper {
        private readonly object action;


        public ActionWrapper(object action) {
            this.action = action;
        }

        private T Invoke<T>(string name) {
            MethodInfo m = action.GetType().GetMethod(name);
            return (T) m.Invoke(action, null);
        }

        public void debugData(DebugString debugString) {
            throw new NotImplementedException();
        }

        public string getDescription() {
            return Invoke<string>("getDescription");
        }

        public object getExtension(Class cls) {
            throw new NotImplementedException();
        }

        public Class[] getExtensions() {
            return Invoke<Class[]>("getExtensions");
        }

        public string getHelp() {
            return Invoke<string>("getHelp");
        }

        public string getId() {
            //MethodInfo m = action.GetType().GetMethod("getId");
            //return (string)m.Invoke(action, new object[] {  });
            return getName().Replace(" ", "");
        }

        public string getName() {
            return Invoke<string>("getName");
        }

        public bool isAuthorised() {
            return Invoke<bool>("isAuthorised");
        }

        public Consent isAvailable(NakedReference target) {
            MethodInfo m = action.GetType().GetMethod("isAvailable");
            return (Consent) m.Invoke(action, new object[] {target});
        }

        public Consent isVisible(NakedReference target) {
            MethodInfo m = action.GetType().GetMethod("isVisible");
            return (Consent) m.Invoke(action, new object[] {target});
        }

        public ActionWrapper[] getActions() {
            MethodInfo m = action.GetType().GetMethod("getActions");
            var aa = (object[]) m.Invoke(action, null);
            return aa.Select(a => new ActionWrapper(a)).ToArray();
        }

        public int getParameterCount() {
            return Invoke<int>("getParameterCount");
        }

        public Action.Type getType() {
            //  return Invoke<Action.Type>("getType");
            throw new NotImplementedException();
        }

        public Action.Target getTarget() {
            //  return Invoke<Action.Target>("getTarget");
            throw new NotImplementedException();
        }

        public bool hasReturn() {
            return Invoke<bool>("hasReturn");
        }

        public bool isOnInstance() {
            return Invoke<bool>("isOnInstance");
        }

        public NakedObjectSpecification[] getParameterTypes() {
            return Invoke<NakedObjectSpecification[]>("getParameterTypes");
        }

        public Naked[] parameterStubs() {
            return Invoke<Naked[]>("parameterStubs");
        }

        public NakedObjectSpecification getReturnType() {
            return Invoke<NakedObjectSpecification>("getReturnType");
        }

        public Naked execute(NakedReference target, Naked[] parameters) {
            MethodInfo m = action.GetType().GetMethod("execute");
            return (Naked) m.Invoke(action, new object[] {target, parameters});
        }

        public Consent isParameterSetValid(NakedReference @object, Naked[] parameters) {
            MethodInfo m = action.GetType().GetMethod("isParameterSetValid");
            return (Consent) m.Invoke(action, new object[] {@object, parameters});
        }

        public ActionParameterSet getParameterSet(NakedReference @object) {
            MethodInfo m = action.GetType().GetMethod("getParameterSet");
            return (ActionParameterSet) m.Invoke(action, new object[] {@object});
        }

        public bool isSet() {
            return getActions().Any();
        }


        public override bool Equals(object obj) {
            var actionWrapper = obj as ActionWrapper;
            if (actionWrapper != null) {
                return Equals(actionWrapper);
            }
            return false;
        }

        public bool Equals(ActionWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.action, action);
        }

        public override int GetHashCode() {
            return (action != null ? action.GetHashCode() : 0);
        }
    }
}