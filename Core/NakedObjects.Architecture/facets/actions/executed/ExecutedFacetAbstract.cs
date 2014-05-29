// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets.Actions.Executed {
    public abstract class ExecutedFacetAbstract : SingleValueFacetAbstract, IExecutedFacet {
        private readonly Where executedWhere;

        protected ExecutedFacetAbstract(Where where, IFacetHolder holder)
            : base(Type, holder) {
            executedWhere = where;
        }

        public static Type Type {
            get { return typeof (IExecutedFacet); }
        }

        #region IExecutedFacet Members

        public virtual Target Target {
            get {
                if (executedWhere == Where.Locally) {
                    return Target.Local;
                }
                if (executedWhere == Where.Remotely) {
                    return Target.Remote;
                }
                return Target.Default;
            }
        }

        public virtual Where ExecutedWhere() {
            return executedWhere;
        }

        #endregion

        protected override string ToStringValues() {
            return "where=" + executedWhere;
        }
    }
}