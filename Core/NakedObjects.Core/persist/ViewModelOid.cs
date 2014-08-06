// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Persist {
    public class ViewModelOid : IOid, IEncodedToStrings {
        private readonly INakedObjectReflector reflector;
        private int cachedHashCode;
        private string cachedToString;
        private ViewModelOid previous;
       
        public ViewModelOid(INakedObjectReflector reflector, INakedObjectSpecification specification) {
            Assert.AssertNotNull(reflector);
            this.reflector = reflector;
            IsTransient = false;
            TypeName = TypeNameUtils.EncodeTypeName(specification.FullName);
            Keys = new[] {System.Guid.NewGuid().ToString()};
            CacheState();
        }

        public ViewModelOid(INakedObjectReflector reflector, string[] strings) {
            Assert.AssertNotNull(reflector);
            this.reflector = reflector;
            var helper = new StringDecoderHelper(reflector, strings);
            TypeName = helper.GetNextString();

            Keys = helper.HasNext ? helper.GetNextArray() : new[] {System.Guid.NewGuid().ToString()};

            IsTransient = false;
            CacheState();
        }

        private void CacheState() {
            cachedHashCode = HashCodeUtils.Seed;
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, TypeName);
            cachedHashCode = HashCodeUtils.Hash(cachedHashCode, Keys);

            object keys = Keys.Aggregate((s, t) => s + ":" + t);

            cachedToString = string.Format("{0}VMOID#{1}{2}", IsTransient ? "T" : "", keys, previous == null ? "" : "+");
        }

        public string TypeName { get; private set; }
        public string[] Keys { get; private set; }

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper();
            helper.Add(TypeName);

            if (Keys.Any()) {
                helper.Add(Keys);
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            return ToEncodedStrings();
        }

        public void CopyFrom(IOid oid) {}

        public IOid Previous {
            get { return previous; }
        }

        public bool IsFinal { get; private set; }

        public bool IsTransient { get; private set; }
        public bool HasPrevious {
            get { return previous != null; }  
        }

        public INakedObjectSpecification Specification {
            get { return reflector.LoadSpecification(TypeNameUtils.DecodeTypeName(TypeName)); }
        }

        public void UpdateKeys(string[] newKeys, bool final) { 
            previous = new ViewModelOid(reflector, Specification) { Keys = Keys };
            Keys = newKeys; // after old key is saved ! 
            IsFinal = final; 
            CacheState();
        }

        #region Object Overrides

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }
            var oid = obj as ViewModelOid;
            if (oid != null) {
                return TypeName.Equals(oid.TypeName) && Keys.SequenceEqual(oid.Keys);
            }
            return false;
        }

        public override int GetHashCode() {
            return cachedHashCode;
        }

        public override string ToString() {
            return cachedToString;
        }

        #endregion
    }
}