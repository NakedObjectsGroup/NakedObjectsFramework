// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;
using Do = NakedObjects.EagerlyAttribute.Do;

namespace RestfulObjects.Test.Data {

    public class SetWrapper<T> : ISet<T>{
        private readonly ICollection<T> wrapped;

        public SetWrapper(ICollection<T> wrapped ) {
            this.wrapped = wrapped;
        }

        public IEnumerator<T> GetEnumerator() {
            return wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        //public void ICollection<T>.Add(T item) {
        //   wrapped.Add(item);
        //}

        public void UnionWith(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public void ExceptWith(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other) {
            throw new System.NotImplementedException();
        }

        public bool Add(T item) {
            wrapped.Add(item);
            return true;
        }

        void ICollection<T>.Add(T item) {
            wrapped.Add(item);
        }

        public void Clear() {
            throw new System.NotImplementedException();
        }

        public bool Contains(T item) {
            throw new System.NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item) {
            throw new System.NotImplementedException();
        }

        public int Count {
            get { return wrapped.Count; }
        }

        public bool IsReadOnly {
            get { return wrapped.IsReadOnly; }
        }
    }


    public class WithCollection {
        private ICollection<MostSimple> anEmptyCollection = new List<MostSimple>();
        private ISet<MostSimple> anEmptySet = new HashSet<MostSimple>();
        private ICollection<MostSimple> aCollection = new List<MostSimple>();
        private ICollection<MostSimpleViewModel> aCollectionViewModels = new List<MostSimpleViewModel>();
        private ICollection<MostSimple> aDisabledCollection = new List<MostSimple>();
        private ICollection<MostSimple> aHiddenCollection = new List<MostSimple>();
        private ICollection<MostSimple> anEagerCollection = new List<MostSimple>();
        private ICollection<MostSimple> aSetAsCollection = new List<MostSimple>();

        public IDomainObjectContainer Container { set; protected get; }

        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

     
        [PresentationHint("class7 class8")]
        public virtual ICollection<MostSimple> ACollection {
            get { return aCollection; }
            set { aCollection = value; }
        }

        public virtual ICollection<MostSimpleViewModel> ACollectionViewModels {
            get { return aCollectionViewModels; }
            set { aCollectionViewModels = value; }
        }

        [NakedObjectsIgnore]
        public virtual ICollection<MostSimple> ASetAsCollection {
            get { return aSetAsCollection; }
            set { aSetAsCollection = value; }
        }

        [NotMapped] 
        public virtual ISet<MostSimple> ASet {
            get { return new SetWrapper<MostSimple>(ASetAsCollection); }
            set { ASetAsCollection = value; }
        }

        [Disabled]
        public virtual ICollection<MostSimple> ADisabledCollection {
            get { return aDisabledCollection; }
            set { aDisabledCollection = value; }
        }

        [Hidden]
        public virtual ICollection<MostSimple> AHiddenCollection {
            get { return aHiddenCollection; }
            set { aHiddenCollection = value; }
        }

        [DescribedAs("an empty collection for testing")]
        [MemberOrder(Sequence = "2")]
        public virtual ICollection<MostSimple> AnEmptyCollection {
            get { return anEmptyCollection; }
            set { anEmptyCollection = value; }
        }

        [DescribedAs("an empty set for testing")]
        [MemberOrder(Sequence = "2")]
        [NotMapped] 
        public virtual ISet<MostSimple> AnEmptySet {
            get { return anEmptySet; }
            set { anEmptySet = value; }
        }

        [Eagerly(Do.Rendering)]
        public virtual ICollection<MostSimple> AnEagerCollection {
            get { return anEagerCollection; }
            set { anEagerCollection = value; }
        }
    }
}