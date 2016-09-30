// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class SetWrapper<T> : ISet<T> {
        private readonly ICollection<T> wrapped;

        public SetWrapper(ICollection<T> wrapped) {
            this.wrapped = wrapped;
        }

        #region ISet<T> Members

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
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        public bool Add(T item) {
            wrapped.Add(item);
            return true;
        }

        void ICollection<T>.Add(T item) {
            wrapped.Add(item);
        }

        public void Clear() {
            wrapped.Clear();
        }

        public bool Contains(T item) {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            throw new NotImplementedException();
        }

        public int Count {
            get { return wrapped.Count; }
        }

        public bool IsReadOnly {
            get { return wrapped.IsReadOnly; }
        }

        #endregion
    }


    public class WithCollection {
        private ICollection<MostSimple> aCollection = new List<MostSimple>();
        private ICollection<MostSimpleViewModel> aCollectionViewModels = new List<MostSimpleViewModel>();
        private ICollection<MostSimple> aDisabledCollection = new List<MostSimple>();
        private ICollection<MostSimple> aHiddenCollection = new List<MostSimple>();
        private ICollection<MostSimple> aNakedObjectsIgnoredCollection = new List<MostSimple>();
        private ICollection<MostSimple> aSetAsCollection = new List<MostSimple>();
        private ICollection<MostSimple> anEagerCollection = new List<MostSimple>();
        private ICollection<MostSimple> anEmptyCollection = new List<MostSimple>();
        private ISet<MostSimple> anEmptySet = new HashSet<MostSimple>();
        private IDomainObjectContainer container;

        public IDomainObjectContainer Container
            
            {
            set {
                container = value; 
                // hack for testsing 
                var vm1 = container.NewViewModel<MostSimpleViewModel>();
                var vm2 = container.NewViewModel<MostSimpleViewModel>();
                vm1.Id = 1;
                vm2.Id = 2;

                aCollectionViewModels.Add(vm1);
                aCollectionViewModels.Add(vm2);

            }
            protected get { return container; }
        }

        [Key, Title, ConcurrencyCheck, DefaultValue(0)]
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

        [Hidden(WhenTo.Always)]
        public virtual ICollection<MostSimple> AHiddenCollection {
            get { return aHiddenCollection; }
            set { aHiddenCollection = value; }
        }

        [NakedObjectsIgnore]
        public virtual ICollection<MostSimple> ANakedObjectsIgnoredCollection {
            get { return aNakedObjectsIgnoredCollection; }
            set { aNakedObjectsIgnoredCollection = value; }
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

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        public virtual ICollection<MostSimple> AnEagerCollection {
            get { return anEagerCollection; }
            set { anEagerCollection = value; }
        }

        public IList<MostSimple> LocallyContributedAction([ContributedAction] IList<MostSimple> acollection) {
            return acollection;
        }

        public IList<MostSimple> LocallyContributedActionWithParm([ContributedAction] IList<MostSimple> acollection, string p1) {
            return acollection;
        }

    }
}