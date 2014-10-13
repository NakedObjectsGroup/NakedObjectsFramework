// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Transaction.Facets.Actions.Invoke {
    public class ActionInvocationFacetWrapTransaction : ActionInvocationFacetAbstract {
        private static readonly ILog Log;
        private readonly IActionInvocationFacet underlyingFacet;

        static ActionInvocationFacetWrapTransaction() {
            Log = LogManager.GetLogger(typeof (ActionInvocationFacetWrapTransaction));
        }

        public ActionInvocationFacetWrapTransaction(IActionInvocationFacet underlyingFacet)
            : base(underlyingFacet.FacetHolder) {
            this.underlyingFacet = underlyingFacet;
        }

        public override IIntrospectableSpecification ReturnType {
            get { return underlyingFacet.ReturnType; }
        }

        public override IIntrospectableSpecification OnType {
            get { return underlyingFacet.OnType; }
        }

        public override bool GetIsRemoting(INakedObject target) {
            return underlyingFacet.GetIsRemoting(target);
        }


        private INakedObject InvokeInTransaction(Func<INakedObject> action, INakedObjectTransactionManager persistor) {
           
            try {
                persistor.StartTransaction();
                INakedObject result = action();
                persistor.EndTransaction();
                return result;
            }
            catch (Exception e) {
                Log.Info("exception executing " + underlyingFacet + "; aborting transaction", e);
                try {
                    persistor.AbortTransaction();
                }
                catch (Exception e2) {
                    Log.Error("failure during abort", e2);
                }
                throw;
            }
        }

        public override INakedObject Invoke(INakedObject target, INakedObject[] parameters, INakedObjectManager manager, ISession session, INakedObjectTransactionManager transactionManager) {
            return InvokeInTransaction(() => underlyingFacet.Invoke(target, parameters, manager, session, transactionManager), transactionManager);
        }

        public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage, INakedObjectManager manager, ISession session, INakedObjectTransactionManager transactionManager) {
            return InvokeInTransaction(() => underlyingFacet.Invoke(nakedObject, parameters, resultPage, manager, session, transactionManager), transactionManager);
        }

        public override string ToString() {
            return base.ToString() + " --> " + underlyingFacet;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}