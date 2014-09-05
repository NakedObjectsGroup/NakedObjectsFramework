// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Executed;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    public class ExecutedFacetAnnotationForControlMethods : ExecutedControlMethodFacetAbstract {
        private readonly IDictionary<MethodInfo, Where> methodToWhere = new Dictionary<MethodInfo, Where>();

        public ExecutedFacetAnnotationForControlMethods(MethodInfo method, Where where, IFacetHolder holder)
            : base(holder) {
            methodToWhere[method] = where;
        }

        public override Where ExecutedWhere(MethodInfo method) {
            return methodToWhere.ContainsKey(method) ? methodToWhere[method] : Where.Default;
        }

        public override void AddMethodExecutedWhere(MethodInfo method, Where where) {
            methodToWhere[method] = where;
        }

        protected override string ToStringValues() {
            var sb = new StringBuilder();
            foreach (var pair in methodToWhere) {
                sb.Append(pair.Key + " Executed = " + pair.Value).Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}