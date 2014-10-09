// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Properties.Access;
using NakedObjects.Architecture.Facets.Properties.Set;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class OneToManyAssociationImpl : NakedObjectAssociationAbstract, IOneToManyAssociation {
        private readonly bool isASet;

        public OneToManyAssociationImpl(IMetamodel metamodel, INakedObjectAssociationPeer association)
            : base(metamodel, association) {
            isASet = association.ContainsFacet<IIsASetFacet>();
        }

        public override bool IsChoicesEnabled {
            get { return false; }
        }

        public override bool IsAutoCompleteEnabled {
            get { return false; }
        }

        #region IOneToManyAssociation Members

        public override INakedObject GetNakedObject(INakedObject inObject, INakedObjectManager manager) {
            return GetCollection(inObject, manager);
        }

        public override bool IsCollection {
            get { return true; }
        }

        public override bool IsASet {
            get { return isASet; }
        }

        public override bool IsEmpty(INakedObject inObject, INakedObjectManager manager, IObjectPersistor persistor) {
            return Count(inObject, persistor) == 0;
        }

        public virtual int Count(INakedObject inObject, IObjectPersistor persistor) {
            return persistor.CountField(inObject, Id);
        }

        public override bool IsInline {
            get { return false; }
        }

        public override bool IsMandatory {
            get { return false; }
        }


        public override INakedObject GetDefault(INakedObject nakedObject, INakedObjectManager manager) {
            return null;
        }

        public override TypeOfDefaultValue GetDefaultType(INakedObject nakedObject, INakedObjectManager manager) {
            return TypeOfDefaultValue.Implicit;
        }

        public override void ToDefault(INakedObject target, INakedObjectManager manager) {}

        #endregion

        public override INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues, ILifecycleManager lifecycleManager) {
            return new INakedObject[0];
        }

        public override Tuple<string, INakedObjectSpecification>[] GetChoicesParameters() {
            return new Tuple<string, INakedObjectSpecification>[0];
        }

        public override INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm, ILifecycleManager lifecycleManager) {
            return new INakedObject[0];
        }

        private INakedObject GetCollection(INakedObject inObject, INakedObjectManager manager) {
            object collection = GetFacet<IPropertyAccessorFacet>().GetProperty(inObject);
            if (collection == null) {
                return null;
            }
            INakedObject adapterFor = manager.CreateAggregatedAdapter(inObject, ((INakedObjectAssociation) this).Id, collection);
            adapterFor.TypeOfFacet = GetFacet<ITypeOfFacet>();
            SetResolveStateForDerivedCollections(adapterFor, manager);
            return adapterFor;
        }

        private void SetResolveStateForDerivedCollections(INakedObject adapterFor, INakedObjectManager manager) {
            bool isDerived = !IsPersisted;
            if (isDerived && !adapterFor.ResolveState.IsResolved()) {
                if (adapterFor.GetAsEnumerable(manager).Any()) {
                    adapterFor.ResolveState.Handle(Events.StartResolvingEvent);
                    adapterFor.ResolveState.Handle(Events.EndResolvingEvent);
                }
            }
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append(base.ToString());
            str.Append(",");
            str.Append("persisted", IsPersisted);
            str.Append("type", Specification == null ? "unknown" : Specification.ShortName);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}