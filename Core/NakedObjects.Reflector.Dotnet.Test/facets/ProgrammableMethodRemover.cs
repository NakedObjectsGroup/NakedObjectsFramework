// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Facets {
    public class ProgrammableMethodRemover : IMethodRemover {
        private readonly IList<RemoveMethodArgs> removeMethodArgsCalls = new List<RemoveMethodArgs>();
        private readonly IList<MethodInfo> removeMethodMethodCalls = new List<MethodInfo>();
        private readonly IList<RemoveMethodsArgs> removeMethodsArgs = new List<RemoveMethodsArgs>();
        private IList<MethodInfo> removeMethodsReturn;

        #region IMethodRemover Members

        public void RemoveMethod(MethodInfo method) {
            removeMethodMethodCalls.Add(method);
        }

        public void RemoveMethods(IList<MethodInfo> methods) {
            foreach (MethodInfo method in methods) {
                RemoveMethod(method);
            }
        }

        #endregion

        public IList<RemoveMethodArgs> GetRemoveMethodArgsCalls() {
            return removeMethodArgsCalls;
        }

        public IList<MethodInfo> GetRemoveMethodMethodCalls() {
            return removeMethodMethodCalls;
        }

        public void SetRemoveMethodsReturn(IList<MethodInfo> removeMethodsReturn) {
            this.removeMethodsReturn = removeMethodsReturn;
        }

        #region Nested Type: RemoveMethodArgs

        public class RemoveMethodArgs {
            public MethodType methodType;
            public string methodName;
            public Type[] parameterTypes;
            public Type returnType;

            public RemoveMethodArgs(MethodType methodType, string methodName, Type returnType, Type[] parameterTypes) {
                this.methodType = methodType;
                this.methodName = methodName;
                this.returnType = returnType;
                this.parameterTypes = parameterTypes;
            }
        }

        #endregion

        #region Nested Type: RemoveMethodsArgs

        public class RemoveMethodsArgs {
            public bool canBeVoid;
            public MethodType forClass;
            public int paramCount;
            public string prefix;
            public Type returnType;

            public RemoveMethodsArgs(MethodType forClass, string prefix, Type returnType, bool canBeVoid, int paramCount) {
                this.forClass = forClass;
                this.prefix = prefix;
                this.returnType = returnType;
                this.canBeVoid = canBeVoid;
                this.paramCount = paramCount;
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}