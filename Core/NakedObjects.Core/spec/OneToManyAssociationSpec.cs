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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Properties.Access;
using NakedObjects.Architecture.Facets.Properties.Set;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class OneToManyAssociationSpec : AssociationSpecAbstract, IOneToManyAssociationSpec {
        private readonly IObjectPersistor persistor;
        private readonly bool isASet;

        public OneToManyAssociationSpec(IMetamodelManager metamodel, IAssociationSpecImmutable association, ISession session, ILifecycleManager lifecycleManager, INakedObjectManager manager, IObjectPersistor persistor)
            : base(metamodel, association, session, lifecycleManager, manager) {
            this.persistor = persistor;
            isASet = association.ContainsFacet<IIsASetFacet>();
        }

        public override bool IsChoicesEnabled {
            get { return false; }
        }

        public override bool IsAutoCompleteEnabled {
            get { return false; }
        }

        #region IOneToManyAssociation Members

        public override INakedObject GetNakedObject(INakedObject inObject) {
            return GetCollection(inObject);
        }

        public override bool IsCollection {
            get { return true; }
        }

        public override bool IsASet {
            get { return isASet; }
        }

        public override bool IsEmpty(INakedObject inObject) {
            return Count(inObject) == 0;
        }

        public virtual int Count(INakedObject inObject) {
            return persistor.CountField(inObject, Id);
        }

        public override bool IsInline {
            get { return false; }
        }

        public override bool IsMandatory {
            get { return false; }
        }


        public override INakedObject GetDefault(INakedObject nakedObject) {
            return null;
        }

        public override TypeOfDefaultValue GetDefaultType(INakedObject nakedObject) {
            return TypeOfDefaultValue.Implicit;
        }

        public override void ToDefault(INakedObject target) {}

        #endregion

        public override INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues) {
            return new INakedObject[0];
        }

        public override Tuple<string, IObjectSpec>[] GetChoicesParameters() {
            return new Tuple<string, IObjectSpec>[0];
        }

        public override INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm) {
            return new INakedObject[0];
        }

        private INakedObject GetCollection(INakedObject inObject) {
            object collection = GetFacet<IPropertyAccessorFacet>().GetProperty(inObject);
            if (collection == null) {
                return null;
            }
            INakedObject adapterFor = Manager.CreateAggregatedAdapter(inObject, ((IAssociationSpec) this).Id, collection);
            adapterFor.TypeOfFacet = GetFacet<ITypeOfFacet>();
            SetResolveStateForDerivedCollections(adapterFor);
            return adapterFor;
        }

        private void SetResolveStateForDerivedCollections(INakedObject adapterFor) {
            bool isDerived = !IsPersisted;
            if (isDerived && !adapterFor.ResolveState.IsResolved()) {
                if (adapterFor.GetAsEnumerable(Manager).Any()) {
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
            str.Append("type", Spec == null ? "unknown" : Spec.ShortName);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}