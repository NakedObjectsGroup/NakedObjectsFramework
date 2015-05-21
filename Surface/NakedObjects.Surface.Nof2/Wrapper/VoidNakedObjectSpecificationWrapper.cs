// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class VoidNakedObjectSpecificationWrapper : ScalarPropertyHolder, INakedObjectSpecificationSurface {
        public bool IsParseable {
            get { return false; }
        }

        public bool IsQueryable {
            get { return false; }
        }

        public bool IsService {
            get { return false; }
        }

        public bool IsVoid {
            get { return true; }
        }

        public bool IsDateTime {
            get { return false; }
        }

        public string FullName {
            get { return typeof (void).FullName; }
        }

        public bool IsCollection {
            get { return false; }
        }

        public bool IsObject {
            get { return false; }
        }

        public string SingularName {
            get { return typeof (void).Name; }
        }

        public string PluralName {
            get { return typeof (void).Name; }
        }

        public string Description {
            get { return ""; }
        }

        #region INakedObjectSpecificationSurface Members

        public INakedObjectAssociationSurface[] Properties {
            get { return new INakedObjectAssociationSurface[] {}; }
        }

        public IMenu Menu { get; private set; }

        public INakedObjectSpecificationSurface GetElementType(INakedObjectSurface nakedObject) {
            throw new NotImplementedException();
        }

        bool INakedObjectSpecificationSurface.IsImmutable(INakedObjectSurface nakedObject) {
            return false;
        }

        public string GetIconName(INakedObjectSurface nakedObject) {
            return null;
        }

        public INakedObjectActionSurface[] GetActionLeafNodes() {
            return new INakedObjectActionSurface[] {};
        }

        public INakedObjectSpecificationSurface ElementType {
            get { return null; }
        }

        public bool IsOfType(INakedObjectSpecificationSurface otherSpec) {
            return false;
        }

        public Type GetUnderlyingType() {
            throw new NotImplementedException();
        }

        public INakedObjectActionSurface[] GetCollectionContributedActions() {
            throw new NotImplementedException();
        }

        public INakedObjectActionSurface[] GetFinderActions() {
            throw new NotImplementedException();
        }

        #endregion

        public bool Equals(INakedObjectSpecificationSurface other) {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) {
            var nakedObjectSpecificationWrapper = obj as VoidNakedObjectSpecificationWrapper;
            if (nakedObjectSpecificationWrapper != null) {
                return true;
            }
            return false;
        }

        public override int GetHashCode() {
            return (GetType().GetHashCode());
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.FullName):
                    return FullName;
                case (ScalarProperty.SingularName):
                    return SingularName;
                case (ScalarProperty.PluralName):
                    return PluralName;
                case (ScalarProperty.Description):
                    return Description;
                case (ScalarProperty.IsParseable):
                    return IsParseable;
                case (ScalarProperty.IsQueryable):
                    return IsQueryable;
                case (ScalarProperty.IsService):
                    return IsService;
                case (ScalarProperty.IsVoid):
                    return IsVoid;
                case (ScalarProperty.IsDateTime):
                    return IsDateTime;
                case (ScalarProperty.IsCollection):
                    return IsCollection;
                case (ScalarProperty.IsObject):
                    return IsObject;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public INakedObjectsSurface Surface { get; set; }
    }
}