// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Security;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Reflect.Util {
    public class InvokeUtils {
        /// <summary>
        ///     Invoke the specified method with all its parameter, if any, as null.
        /// </summary>
        public static object InvokeStatic(MethodInfo method) {
            var parameters = new object[method.GetParameters().Length];
            return Invoke(method, null, parameters);
        }

        public static object InvokeStatic(MethodInfo method, INakedObject[] parameters) {
            return Invoke(method, null, parameters);
        }

        public static object InvokeStatic(MethodInfo method, object[] parameters) {
            return Invoke(method, null, parameters);
        }

        public static object Invoke(MethodInfo method, INakedObject nakedObject) {
            var parameters = new object[method.GetParameters().Length];
            return Invoke(method, nakedObject.GetDomainObject(), parameters);
        }

        public static object Invoke(MethodInfo method, INakedObject nakedObject, INakedObject[] parameters) {
            object[] parameterPocos = parameters == null ? new object[0] : new object[parameters.Length];
            for (int i = 0; i < parameterPocos.Length; i++) {
                parameterPocos[i] = parameters[i] == null ? null : parameters[i].Object;
            }
            return Invoke(method, nakedObject.Object, parameterPocos);
        }

        /// <summary>
        ///     Invoke the specified method with the user name (from the specified session) as the one and only parameter.
        /// </summary>
        public static object InvokeForSession(MethodInfo method, ISession session) {
            int len = method.GetParameters().Length;
            var parameters = new object[len];
            parameters[0] = session.Principal;
            return Invoke(method, null, parameters);
        }

        public static object Invoke(MethodInfo method, object obj, object[] parameters) {
            try {
                return method.Invoke(obj, parameters);
            }
            catch (TargetInvocationException e) {
                InvocationException("Exception executing " + method, e);
                return null;
            }
        }

        public static void InvocationException(string error, Exception e) {
            Exception innerException = e.InnerException;
            if (innerException is DomainException) {
                // a domain  exception from the domain code is re-thrown as an NO exception with same
                // semantics
                throw new NakedObjectDomainException(innerException.Message, innerException);
            }
            if (e is TargetInvocationException) {
                throw new InvokeException(innerException.Message, innerException);
            }
            throw new ReflectionException(e.Message, e);
        }
    }

    public class InvokeException : NakedObjectApplicationException {
        public InvokeException(string message, Exception exception) : base(message, exception) {}
    }
}