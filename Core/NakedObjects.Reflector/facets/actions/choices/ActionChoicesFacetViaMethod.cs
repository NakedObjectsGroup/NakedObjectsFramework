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
using NakedObjects.Architecture.Facets.Actions.Choices;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Choices {
    public class ActionChoicesFacetViaMethod : ActionChoicesFacetAbstract, IImperativeFacet {
        private readonly MethodInfo choicesMethod;
        private readonly Type choicesType;
        private readonly bool isMultiple;
        private readonly Tuple<string, INakedObjectSpecification>[] parameterNamesAndTypes;
        private readonly string[] parameterNames;


        public ActionChoicesFacetViaMethod(IMetadata metadata, MethodInfo choicesMethod, Type choicesType, IFacetHolder holder, bool isMultiple = false)
            : base(holder) {
            this.choicesMethod = choicesMethod;
            this.choicesType = choicesType;
            this.isMultiple = isMultiple;
            parameterNamesAndTypes = choicesMethod.GetParameters().Select(p => new Tuple<string, INakedObjectSpecification>(p.Name.ToLower(), metadata.GetSpecification(p.ParameterType))).ToArray();
            parameterNames = parameterNamesAndTypes.Select(pnt => pnt.Item1).ToArray();
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return choicesMethod;
        }

        #endregion

        public override Tuple<string, INakedObjectSpecification>[] ParameterNamesAndTypes {
            get { return parameterNamesAndTypes; }
        }

        public override bool IsMultiple {
            get { return isMultiple; }
        }

        public override object[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues) {
            INakedObject[] parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);

            try {
                var options = InvokeUtils.Invoke(choicesMethod, nakedObject, parms) as IEnumerable;
                if (options != null) {
                    return options.Cast<object>().ToArray();
                }
                throw new NakedObjectDomainException("Must return IEnumerable from choices method: " + choicesMethod.Name);
            }
            catch (ArgumentException ae) {
                string msg = string.Format("Choices exception: {0} has mismatched (ie type of choices parameter does not match type of action parameter) parameter types", choicesMethod.Name);
                throw new InvokeException(msg, ae);
            }
        }

        protected override string ToStringValues() {
            return "method=" + choicesMethod + ",Type=" + choicesType;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}