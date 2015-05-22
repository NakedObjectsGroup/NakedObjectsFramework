// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Surface;
using org.nakedobjects.@object;
using sdm.systems.reflector;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class NakedObjectWrapper : ScalarPropertyHolder,  INakedObjectSurface {
        private readonly Naked nakedObject;

        public NakedObjectWrapper(Naked nakedObject, INakedObjectsSurface surface) {
            this.nakedObject = nakedObject;
            Surface = surface; 
        }

        public Naked NakedObject {
            get { return nakedObject; }
        }

        #region INakedObjectSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(NakedObject.getSpecification(), NakedObject, Surface); }
        }

        public INakedObjectSpecificationSurface ElementSpecification {
            get {
                if (nakedObject is InternalCollectionAdapter) {
                    return new NakedObjectSpecificationWrapper(((InternalCollectionAdapter) nakedObject).getElementSpecification(), null, Surface);
                }
                return null;
            }
        }

        public object Object {
            get { return nakedObject.getObject(); }
        }

        public string TitleString() {
            return nakedObject.titleString();
        }

        public IEnumerable<INakedObjectSurface> ToEnumerable() {
            return ((IEnumerable) Object).Cast<object>().Select(o => new NakedObjectWrapper(org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(o), Surface));
        }

        public INakedObjectSurface Page(int page, int size) {
            return new NakedObjectWrapper(nakedObject, Surface);
        }

        public INakedObjectSurface Select(object[] selection, bool forceEnumerable) {
            throw new NotImplementedException();
        }

        public int Count() {
            var nakedCollection = nakedObject as NakedCollection;
            if (nakedCollection != null) {
                return nakedCollection.size();
            }

            return 0;
        }

        public AttachmentContext GetAttachment() {
            throw new NotImplementedException();
        }

        public object[] GetSelected() {
            throw new NotImplementedException();
        }

        public void Resolve() {
            throw new NotImplementedException();
        }

        public void SetIsNotQueryableState(bool state) {
            throw new NotImplementedException();
        }

        public PropertyInfo[] GetKeys() {
            // return NakedObjectsContext.ObjectPersistor.GetKeys(nakedObject.Object.GetType());
            return null;
        }

        public bool IsTransient {
            get {
                var nakedRef = nakedObject as NakedReference;
                if (nakedRef != null) {
                    return nakedRef.getResolveState().isTransient() && !(new NakedObjectSpecificationWrapper(nakedRef.getSpecification(), nakedRef, Surface).IsService);
                }

                return false;
            }
        }

        public IVersionSurface Version {
            get { return new VersionWrapper(((NakedReference) nakedObject).getVersion()); }
        }

        public IOidSurface Oid {
            get { return new OidWrapper(nakedObject.getOid()); }
        }

        #endregion

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

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.IsTransient):
                    return IsTransient;
                case (ScalarProperty.IsDestroyed):
                    return false;
                case (ScalarProperty.TitleString):
                    return TitleString();
                case (ScalarProperty.InvariantString):
                    return "";
                case (ScalarProperty.IsNotPersistent):
                    return false;
                case (ScalarProperty.IsUserPersistable):
                    return true;
                case (ScalarProperty.IsCollectionMemento):
                    return false;
                case (ScalarProperty.IsPaged):
                    return false;
                case (ScalarProperty.IsViewModelEditView):
                    return false;
                case (ScalarProperty.IsViewModel):
                    return false;
                case (ScalarProperty.EnumIntegralValue):
                    return false;
                case (ScalarProperty.MementoAction):
                    return null;
                case (ScalarProperty.ExtensionData):
                    return null;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public INakedObjectsSurface Surface { get; set; }
    }
}