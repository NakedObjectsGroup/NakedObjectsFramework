// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class VoidNakedObjectSpecificationWrapper :  INakedObjectSpecificationSurface {
        public bool IsComplexType { get; private set; }

        public bool IsParseable {
            get { return false; }
        }

        public bool IsStream { get; private set; }

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

        public string UntitledName { get; private set; }

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

        public bool IsASet { get; private set; }
        public bool IsAggregated { get; private set; }
        public bool IsImage { get; private set; }
        public bool IsFileAttachment { get; private set; }
        public bool IsFile { get; private set; }
        public IDictionary<string, object> ExtensionData { get; private set; }
        public bool IsBoolean { get; private set; }
        public bool IsEnum { get; private set; }

        #region INakedObjectSpecificationSurface Members

        public INakedObjectAssociationSurface[] Properties {
            get { return new INakedObjectAssociationSurface[] {}; }
        }

        public IMenu Menu { get; private set; }
        public string PresentationHint { get; private set; }
        public bool IsAlwaysImmutable { get; private set; }
        public bool IsImmutableOncePersisted { get; private set; }

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
            return typeof (void);
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

       

        public INakedObjectsSurface Surface { get; set; }
    }
}