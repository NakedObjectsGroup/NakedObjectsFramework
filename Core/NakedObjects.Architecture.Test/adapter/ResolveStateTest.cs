// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NUnit.Framework;

namespace NakedObjects.Architecture.Adapter {
    [TestFixture]
    public class ResolveStateTest {

        #region Helpers
        private static void ExpectException(Action x)
        {
            try
            {
                x();
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        private static void ExpectNoException(Action x)
        {
            try
            {
                x();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        } 

        public class TestCallbackFacet : ILoadingCallbackFacet, ILoadedCallbackFacet {
            public IFacetHolder FacetHolder {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public bool IsNoOp {
                get { throw new NotImplementedException(); }
            }

            public Type FacetType {
                get { throw new NotImplementedException(); }
            }

            public bool CanAlwaysReplace {
                get { throw new NotImplementedException(); }
            }

            public void Invoke(INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
                
            }
        }

        private class TestSpecification : INakedObjectSpecification {
            public INakedObjectAction[] GetRelatedServiceActions() {
                throw new NotImplementedException();
            }

            public INakedObjectAction[] GetObjectActions() {
                throw new NotImplementedException();
            }

            public INakedObjectAssociation[] Properties {
                get { throw new NotImplementedException(); }
            }

            public INakedObjectAssociation GetProperty(string id) {
                throw new NotImplementedException();
            }

            public INakedObjectValidation[] ValidateMethods() {
                throw new System.NotImplementedException();
            }

            public Type[] FacetTypes {
                get { throw new NotImplementedException(); }
            }

            public IIdentifier Identifier {
                get { throw new NotImplementedException(); }
            }

            public bool ContainsFacet(Type facetType) {
                throw new NotImplementedException();
            }

            public bool ContainsFacet<T>() where T : IFacet  {
                throw new NotImplementedException();
            }

            public IFacet GetFacet(Type type) {
                return new TestCallbackFacet();
            }

            public T GetFacet<T>() where T : IFacet {
                return (T) GetFacet(typeof (T));
            }

            public IFacet[] GetFacets(IFacetFilter filter) {
                throw new NotImplementedException();
            }

            public void AddFacet(IFacet facet) {
                throw new NotImplementedException();
            }

            public void AddFacet(IMultiTypedFacet facet) {
                throw new NotImplementedException();
            }

            public void RemoveFacet(IFacet facet) {
                throw new NotImplementedException();
            }

            public void RemoveFacet(Type facetType) {
                throw new NotImplementedException();
            }

            public bool HasSubclasses {
                get { throw new NotImplementedException(); }
            }

            public INakedObjectSpecification[] Interfaces {
                get { throw new NotImplementedException(); }
            }

            public INakedObjectSpecification[] Subclasses {
                get { throw new NotImplementedException(); }
            }

            public INakedObjectSpecification Superclass {
                get { throw new NotImplementedException(); }
            }

            public void AddSubclass(INakedObjectSpecification specification) {
                throw new NotImplementedException();
            }

            public bool IsOfType(INakedObjectSpecification specification) {
                throw new NotImplementedException();
            }

            public object DefaultValue {
                get { throw new NotImplementedException(); }
            }

            public string FullName {
                get { throw new NotImplementedException(); }
            }

            public string PluralName {
                get { throw new NotImplementedException(); }
            }

            public string ShortName {
                get { throw new NotImplementedException(); }
            }

            public string Description {
                get { throw new NotImplementedException(); }
            }

            public string Help {
                get { throw new NotImplementedException(); }
            }

            public string SingularName {
                get { throw new NotImplementedException(); }
            }

            public string UntitledName {
                get { throw new NotImplementedException(); }
            }

            public bool IsParseable {
                get { throw new NotImplementedException(); }
            }

            public bool IsEncodeable {
                get { throw new NotImplementedException(); }
            }

            public bool IsAggregated {
                get { throw new NotImplementedException(); }
            }

            public bool IsCollection {
                get { throw new NotImplementedException(); }
            }

            public bool IsObject {
                get { throw new NotImplementedException(); }
            }

            public bool IsAbstract {
                get { throw new NotImplementedException(); }
            }

            public bool IsInterface {
                get { throw new NotImplementedException(); }
            }

            public bool IsService {
                get { throw new NotImplementedException(); }
            }

            public bool HasNoIdentity {
                get { throw new NotImplementedException(); }
            }

            public bool IsQueryable {
                get { throw new NotImplementedException(); }
            }

            public bool IsVoid {
                get { throw new NotImplementedException(); }
            }

            public string GetIconName(INakedObject forObject) {
                throw new NotImplementedException();
            }

            public string GetTitle(INakedObject nakedObject, INakedObjectManager manager) {
                throw new NotImplementedException();
            }

            public IConsent ValidToPersist(INakedObject transientObject, ISession session) {
                throw new NotImplementedException();
            }

            public Persistable Persistable {
                get { throw new NotImplementedException(); }
            }

            public bool IsASet {
                get { throw new NotImplementedException(); }
            }

            public bool IsViewModel {
                get { throw new NotImplementedException(); }
            }

            public object CreateObject(INakedObjectPersistor persistor) {
                throw new NotImplementedException();
            }

            public IEnumerable GetBoundedSet(INakedObjectPersistor persistor) {
                throw new NotImplementedException();
            }

            public void MarkAsService() {
                throw new NotImplementedException();
            }

            public string GetInvariantString(INakedObject nakedObject) {
                throw new NotImplementedException();
            }
        }

        private class TestAdapter : INakedObject {
            public object Object {
                get { throw new NotImplementedException(); }
            }

            public INakedObjectSpecification Specification {
                get { return new TestSpecification(); }
            }

            public IOid Oid {
                get { throw new NotImplementedException(); }
            }

            public ResolveStateMachine ResolveState {
                get { throw new NotImplementedException(); }
            }

            public IVersion Version {
                get { throw new NotImplementedException(); }
            }

            public IVersion OptimisticLock {
                set { throw new NotImplementedException(); }
            }

            public ITypeOfFacet TypeOfFacet {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string IconName() {
                throw new NotImplementedException();
            }

            public string TitleString() {
                throw new NotImplementedException();
            }

            public string InvariantString() {
                throw new NotImplementedException();
            }

            public void CheckLock(IVersion otherVersion) {
                throw new NotImplementedException();
            }

            public void ReplacePoco(object poco) {
                throw new NotImplementedException();
            }

            public void FireChangedEvent() {
                throw new NotImplementedException();
            }

            public string ValidToPersist() {
                throw new NotImplementedException();
            }

            public void SetATransientOid(IOid oid) {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region State Machines 

        private static IResolveStateMachine NewSM() {
            return new ResolveStateMachine(new TestAdapter(), null, null);
        }

        private static IResolveStateMachine GhostSM() {
            IResolveStateMachine sm = NewSM();
            sm.Handle(Events.InitializePersistentEvent);
            return sm;
        }

        private static IResolveStateMachine TransientSM() {
            IResolveStateMachine sm = NewSM();
            sm.Handle(Events.InitializeTransientEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvingPartSM() {
            IResolveStateMachine sm = GhostSM();
            sm.Handle(Events.StartPartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvedPartSM() {
            IResolveStateMachine sm = ResolvingPartSM();
            sm.Handle(Events.EndPartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvingSM() {
            IResolveStateMachine sm = GhostSM();
            sm.Handle(Events.StartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvedSM() {
            IResolveStateMachine sm = ResolvingSM();
            sm.Handle(Events.EndResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine UpdatingSM() {
            IResolveStateMachine sm = ResolvedSM();
            sm.Handle(Events.StartUpdatingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingPartResolvedSM() {
            IResolveStateMachine sm = ResolvedPartSM();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingTransientSM() {
            IResolveStateMachine sm = TransientSM();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingResolvedSM() {
            IResolveStateMachine sm = ResolvedSM();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        #endregion

        [Test]
        public void InvalidChangesFromGhost() {
            ExpectException(() => GhostSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => GhostSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => GhostSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => GhostSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => GhostSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => GhostSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromNew() {
            ExpectException(() => NewSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => NewSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => NewSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => NewSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => NewSM().Handle(Events.DestroyEvent));
            ExpectException(() => NewSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => NewSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void InvalidChangesFromPartResolved() {
            ExpectException(() => ResolvedPartSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvedPartSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvedPartSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => ResolvedPartSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvedPartSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedPartSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedPartSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void InvalidChangesFromResolved() {
            ExpectException(() => ResolvedSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvedSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => ResolvedSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvedSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvedSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void InvalidChangesFromResolving() {
            ExpectException(() => ResolvingSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvingSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvingSM().Handle(Events.DestroyEvent));
            ExpectException(() => ResolvingSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromResolvingPart() {
            ExpectException(() => ResolvingPartSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.DestroyEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromSerializingPartResolved() {
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.DestroyEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromSerializingResolved() {
            ExpectException(() => SerializingResolvedSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.DestroyEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromSerializingTransient() {
            ExpectException(() => SerializingTransientSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.DestroyEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromTransient() {
            ExpectException(() => TransientSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => TransientSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => TransientSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => TransientSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => TransientSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => TransientSM().Handle(Events.DestroyEvent));
            ExpectException(() => TransientSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => TransientSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => TransientSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => TransientSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => TransientSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromUpdating() {
            ExpectException(() => UpdatingSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => UpdatingSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.StartResolvingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => UpdatingSM().Handle(Events.DestroyEvent));
            ExpectException(() => UpdatingSM().Handle(Events.StartUpdatingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void ValidChangesFromGhost() {
            ExpectNoException(() => GhostSM().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartPartResolvingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.DestroyEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void ValidChangesFromNew() {
            ExpectNoException(() => NewSM().Handle(Events.InitializePersistentEvent));
            ExpectNoException(() => NewSM().Handle(Events.InitializeTransientEvent));
            ExpectNoException(() => NewSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void ValidChangesFromPartResolved() {
            ExpectNoException(() => ResolvedPartSM().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => ResolvedPartSM().Handle(Events.StartPartResolvingEvent));
            ExpectNoException(() => ResolvedPartSM().Handle(Events.DestroyEvent));
            ExpectNoException(() => ResolvedPartSM().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => ResolvedPartSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void ValidChangesFromResolved() {
            ExpectNoException(() => ResolvedSM().Handle(Events.ResetEvent));
            ExpectNoException(() => ResolvedSM().Handle(Events.DestroyEvent));
            ExpectNoException(() => ResolvedSM().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => ResolvedSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void ValidChangesFromResolving() {
            ExpectNoException(() => ResolvingSM().Handle(Events.EndResolvingEvent));
        }

        [Test]
        public void ValidChangesFromResolvingPart() {
            ExpectNoException(() => ResolvingPartSM().Handle(Events.EndPartResolvingEvent));
            ExpectNoException(() => ResolvingPartSM().Handle(Events.EndResolvingEvent));
        }

        [Test]
        public void ValidChangesFromSerializingPartResolved() {
            ExpectNoException(() => SerializingPartResolvedSM().Handle(Events.EndSerializingEvent));
        }

        [Test]
        public void ValidChangesFromSerializingResolved() {
            ExpectNoException(() => SerializingResolvedSM().Handle(Events.EndSerializingEvent));
        }

        [Test]
        public void ValidChangesFromSerializingTransient() {
            ExpectNoException(() => SerializingTransientSM().Handle(Events.EndSerializingEvent));
        }

        [Test]
        public void ValidChangesFromTransient() {
            ExpectNoException(() => TransientSM().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => TransientSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void ValidChangesFromUpdating() {
            ExpectNoException(() => UpdatingSM().Handle(Events.EndUpdatingEvent));
        }
    }
}