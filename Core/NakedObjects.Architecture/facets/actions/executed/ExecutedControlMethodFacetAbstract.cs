// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;

namespace NakedObjects.Architecture.Facets.Actions.Executed {
    public abstract class ExecutedControlMethodFacetAbstract : FacetAbstract, IExecutedControlMethodFacet {
        protected ExecutedControlMethodFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IExecutedControlMethodFacet); }
        }

        public abstract Where ExecutedWhere(MethodInfo method);
        public abstract void AddMethodExecutedWhere(MethodInfo method, Where where);
    }
}