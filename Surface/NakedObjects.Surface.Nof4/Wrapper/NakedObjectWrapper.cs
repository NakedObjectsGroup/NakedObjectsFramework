// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Surface.Interface;
using NakedObjects.Value;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectWrapper : ScalarPropertyHolder, INakedObjectSurface {
        private readonly INakedObject nakedObject;

        public static NakedObjectWrapper Wrap(INakedObject nakedObject, INakedObjectsSurface surface) {
            return nakedObject == null ? null : new NakedObjectWrapper(nakedObject, surface);
        }

        protected NakedObjectWrapper(INakedObject nakedObject, INakedObjectsSurface surface) {
            this.nakedObject = nakedObject;
            Surface = surface;
        }

        public INakedObject WrappedNakedObject {
            get { return nakedObject; }
        }

        public bool IsTransient {
            get { return nakedObject.ResolveState.IsTransient(); }
        }

        #region INakedObjectSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(WrappedNakedObject.Specification, Surface); }
        }

        public INakedObjectSpecificationSurface ElementSpecification {
            get {
                ITypeOfFacet typeOfFacet = nakedObject.GetTypeOfFacetFromSpec();
                return new NakedObjectSpecificationWrapper(typeOfFacet.ValueSpec, Surface);
            }
        }

        public object Object {
            get { return nakedObject.Object; }
        }

        public IEnumerable<INakedObjectSurface> ToEnumerable() {
            return WrappedNakedObject.GetAsEnumerable(NakedObjectsContext.ObjectPersistor).Select(no => new NakedObjectWrapper(no, Surface));
        }

        // todo move into adapterutils

        public INakedObjectSurface Page(int page, int size) {
            return new NakedObjectWrapper(Page(nakedObject, page, size), Surface);
        }

        public int Count() {
            return WrappedNakedObject.GetAsQueryable().Count();
        }

        public AttachmentContext GetAttachment() {
            var fa = WrappedNakedObject.Object as FileAttachment;
            var context = new AttachmentContext();

            if (fa != null) {
                context.Content = fa.GetResourceAsStream();
                context.MimeType = fa.MimeType;
                context.ContentDisposition = fa.DispositionType;
                context.FileName = fa.Name;
            }
            return context;
        }

        public PropertyInfo[] GetKeys() {
            if (nakedObject.Specification.IsService) {
                // services don't have keys
                return new PropertyInfo[] {};
            }
            return NakedObjectsContext.ObjectPersistor.GetKeys(nakedObject.Object.GetType());
        }

        public IVersionSurface Version {
            get { return new VersionWrapper(nakedObject.Version); }
        }

        public IOidSurface Oid {
            get { return new OidWrapper(nakedObject.Oid); }
        }

        private static INakedObject Page(INakedObject objectRepresentingCollection, int page, int size) {
            return objectRepresentingCollection.GetCollectionFacetFromSpec().Page(page, size, objectRepresentingCollection, NakedObjectsContext.ObjectPersistor, true);
        }

        #endregion

        public INakedObjectsSurface Surface { get; set; }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();
                INakedObjectSpecification spec = WrappedNakedObject.Specification;

                if (spec.IsService) {
                    ServiceTypes st = NakedObjectsContext.ObjectPersistor.GetServiceType(spec);
                    extData[ServiceType] = st.ToString();
                }

                if (spec.ContainsFacet<IViewModelFacet>() && spec.GetFacet<IViewModelFacet>().IsEditView(WrappedNakedObject)) {
                    extData[RenderInEditMode] = true;
                }

                if (spec.ContainsFacet<IPresentationHintFacet>()) {
                    extData[PresentationHint] = spec.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.IsTransient):
                    return IsTransient;
                case (ScalarProperty.TitleString):
                    return TitleString();
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public string TitleString() {
            return nakedObject.TitleString();
        }

        public override bool Equals(object obj) {
            var nakedObjectWrapper = obj as NakedObjectWrapper;
            if (nakedObjectWrapper != null) {
                return Equals(nakedObjectWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.nakedObject, nakedObject);
        }

        public override int GetHashCode() {
            return (nakedObject != null ? nakedObject.GetHashCode() : 0);
        }
    }
}