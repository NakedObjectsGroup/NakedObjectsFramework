// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.Spec {
    public class NakedObjectActionSet : INakedObjectAction {
        private readonly INakedObjectAction[] actions;
        private readonly string id;
        private readonly string name;
        private readonly string shortId; 

        public NakedObjectActionSet(string id, INakedObjectAction[] actions)
            : this(id, null, actions) {}

        public NakedObjectActionSet(string id, string name, INakedObjectAction[] actions) {
            this.name = name;
            this.id = id;
            this.actions = actions;
            this.shortId = TypeNameUtils.GetShortName(id);
        }

        public virtual bool OnInstance {
            get { return false; }
        }

        #region INakedObjectAction Members

        public virtual string DebugData {
            get { return ""; }
        }

        public virtual INakedObjectAction[] Actions {
            get { return actions; }
        }

        public virtual string Description {
            get { return ""; }
        }

        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual Type[] FacetTypes {
            get { return new Type[0]; }
        }

        public virtual IIdentifier Identifier {
            get { return null; }
        }

        public virtual string Help {
            get { return ""; }
        }

        public virtual string Id {
            get { return id; }
        }

        public virtual string GetName(INakedObjectPersistor persistor) {
            if (name == null) {
                var service = persistor.GetService(shortId);
                return service.TitleString();
            }
            return name;
        }

        public virtual INakedObjectSpecification OnType {
            get { return null; }
        }

        public virtual int ParameterCount {
            get { return 0; }
        }

        public virtual INakedObjectSpecification ReturnType {
            get { return null; }
        }

        public virtual Where Target {
            get { return Where.Default; }
        }

        public bool IsContributedTo(INakedObjectSpecification spec) {
            return false;
        }

        public bool IsFinderMethod {
            get { return false; }
        }

        public virtual NakedObjectActionType ActionType {
            get { return NakedObjectActionType.Set; }
        }

        public virtual bool IsContributedMethod {
            get { return false; }
        }

        public virtual bool PromptForParameters(INakedObject nakedObject) {
            return false;
        }

        /// <summary>
        ///     Always returns <c>null</c>
        /// </summary>
        public virtual INakedObjectSpecification Specification {
            get { return null; }
        }

        public virtual INakedObject Execute(INakedObject target, INakedObject[] parameterSet, INakedObjectPersistor persistor, ISession session) {
            throw new UnexpectedCallException();
        }

        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual IFacet GetFacet(Type type) {
            return null;
        }

        public virtual T GetFacet<T>() where T : IFacet {
            return default(T);
        }

        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual IFacet[] GetFacets(IFacetFilter filter) {
            return null;
        }

        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual void AddFacet(IFacet facet) {}

        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual void AddFacet(IMultiTypedFacet facet) {}


        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual void RemoveFacet(IFacet facet) {}


        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual bool ContainsFacet(Type facetType) {
            return false;
        }

        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual bool ContainsFacet<T>() where T : IFacet {
            return false;
        }


        /// <summary>
        ///     Does nothing
        /// </summary>
        public virtual void RemoveFacet(Type facetType) {}

        public virtual INakedObjectActionParameter[] Parameters {
            get { return new INakedObjectActionParameter[0]; }
        }

        public virtual INakedObjectActionParameter[] GetParameters(INakedObjectActionParameterFilter filter) {
            return new INakedObjectActionParameter[0];
        }

        public INakedObject[] RealParameters(INakedObject target, INakedObject[] parameterSet) {
            return new INakedObject[] {};
        }

        public virtual bool HasReturn() {
            return false;
        }

        public virtual IConsent IsParameterSetValid(ISession session, INakedObject nakedObject, INakedObject[] parameterSet, INakedObjectPersistor persistor) {
            throw new UnexpectedCallException();
        }

        public virtual IConsent IsUsable(ISession session, INakedObject target, INakedObjectPersistor persistor) {
            return Allow.Default;
        }

        public bool IsNullable {
            get { return false; }
        }

        public virtual bool IsVisible(ISession session, INakedObject target, INakedObjectPersistor persistor) {
            return true;
        }

        public virtual INakedObject RealTarget(INakedObject target, INakedObjectPersistor persistor) {
            return null;
        }

        #endregion

        public virtual INakedObject[] GetDefaultParameterValues(INakedObject target) {
            throw new UnexpectedCallException();
        }

        public virtual INakedObject[][] GetChoices(INakedObject target) {
            throw new UnexpectedCallException();
        }

        public virtual IConsent IsParameterSetValidDeclaratively(INakedObject nakedObject, INakedObject[] parameters) {
            throw new UnexpectedCallException();
        }

        public virtual IConsent IsParameterSetValidImperatively(INakedObject nakedObject, INakedObject[] parameters) {
            throw new UnexpectedCallException();
        }

        public virtual bool IsVisible(InteractionContext ic) {
            return true;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}