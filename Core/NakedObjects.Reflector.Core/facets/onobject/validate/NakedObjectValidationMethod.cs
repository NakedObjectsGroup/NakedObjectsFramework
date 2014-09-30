// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.spec {
    public class NakedObjectValidationMethod : INakedObjectValidation {
        private readonly MethodInfo method;

        public NakedObjectValidationMethod(MethodInfo method) {
            this.method = method;
        }

        public string[] ParameterNames {
            get { return method.GetParameters().Select(p => p.Name.ToLower()).ToArray(); }
        }

        public string Execute(INakedObject obj, INakedObject[] parameters) {
            return InvokeUtils.Invoke(method, obj, parameters) as string;
        }
    }
}