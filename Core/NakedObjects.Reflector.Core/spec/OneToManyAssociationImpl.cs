// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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

        public OneToManyAssociationImpl(INakedObjectAssociationPeer association)
            : base(association.Identifier.MemberName, association.Specification, association) {
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

        public override bool IsEmpty(INakedObject inObject, ILifecycleManager persistor) {
            return Count(inObject, persistor) == 0;
        }

        public virtual int Count(INakedObject inObject, ILifecycleManager persistor) {
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

        public override INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues, ILifecycleManager persistor) {
            return new INakedObject[0];
        }

        public override Tuple<string, INakedObjectSpecification>[] GetChoicesParameters() {
            return new Tuple<string, INakedObjectSpecification>[0];
        }

        public override INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm, ILifecycleManager persistor) {
            return new INakedObject[0];
        }

        private INakedObject GetCollection(INakedObject inObject, INakedObjectManager manager) {
            object collection = GetFacet<IPropertyAccessorFacet>().GetProperty(inObject);
            if (collection == null) {
                return null;
            }
            INakedObject adapterFor = manager.CreateAggregatedAdapter(inObject, ((INakedObjectAssociation)this).Id, collection);
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