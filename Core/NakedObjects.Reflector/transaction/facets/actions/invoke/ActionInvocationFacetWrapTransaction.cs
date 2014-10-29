// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.Transaction.Facets.Actions.Invoke {
    public class ActionInvocationFacetWrapTransaction : ActionInvocationFacetAbstract {
        private static readonly ILog Log;
        private readonly IActionInvocationFacet underlyingFacet;

        static ActionInvocationFacetWrapTransaction() {
            Log = LogManager.GetLogger(typeof (ActionInvocationFacetWrapTransaction));
        }

        public ActionInvocationFacetWrapTransaction(IActionInvocationFacet underlyingFacet)
            : base(underlyingFacet.Specification) {
            this.underlyingFacet = underlyingFacet;
        }

        public override IObjectSpecImmutable ReturnType {
            get { return underlyingFacet.ReturnType; }
        }

        public override IObjectSpecImmutable ElementType {
            get { return underlyingFacet.ElementType; }
        }

        public override IObjectSpecImmutable OnType {
            get { return underlyingFacet.OnType; }
        }

        public override bool GetIsRemoting(INakedObject target) {
            return underlyingFacet.GetIsRemoting(target);
        }


        private INakedObject InvokeInTransaction(Func<INakedObject> action, ITransactionManager persistor) {
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

        public override INakedObject Invoke(INakedObject target, INakedObject[] parameters, INakedObjectManager manager, ISession session, ITransactionManager transactionManager) {
            return InvokeInTransaction(() => underlyingFacet.Invoke(target, parameters, manager, session, transactionManager), transactionManager);
        }

        public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage, INakedObjectManager manager, ISession session, ITransactionManager transactionManager) {
            return InvokeInTransaction(() => underlyingFacet.Invoke(nakedObject, parameters, resultPage, manager, session, transactionManager), transactionManager);
        }

        public override string ToString() {
            return base.ToString() + " --> " + underlyingFacet;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}