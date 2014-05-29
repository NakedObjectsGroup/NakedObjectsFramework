// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Choices;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Context;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Choices {
    public class PropertyChoicesFacetViaMethod : PropertyChoicesFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;
       
        private readonly Tuple<string, INakedObjectSpecification>[] parameterNamesAndTypes;
        private readonly string[] parameterNames;

        public PropertyChoicesFacetViaMethod(MethodInfo optionsMethod, IFacetHolder holder)
            : base(holder) {
            method = optionsMethod;

            parameterNamesAndTypes = optionsMethod.GetParameters().Select(p => new Tuple<string, INakedObjectSpecification>(p.Name.ToLower(), NakedObjectsContext.Reflector.LoadSpecification(p.ParameterType))).ToArray();
            parameterNames = parameterNamesAndTypes.Select(pnt => pnt.Item1).ToArray();
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override Tuple<string, INakedObjectSpecification>[] ParameterNamesAndTypes {
            get { return parameterNamesAndTypes; }
        }

        public override object[] GetChoices(INakedObject inObject, IDictionary<string, INakedObject> parameterNameValues) {
            INakedObject[] parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);
            try {
                object options = InvokeUtils.Invoke(method, inObject, parms);
                if (options is IEnumerable) {
                    return ((IEnumerable) options).Cast<object>().ToArray();
                }
                throw new NakedObjectDomainException("Must return IEnumerable from choices method: " + method.Name);
            }
            catch (ArgumentException ae) {
                string msg = string.Format("Choices exception: {0} has mismatched (ie type of parameter does not match type of property) parameter types", method.Name);
                throw new InvokeException(msg, ae);
            }
        }

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}