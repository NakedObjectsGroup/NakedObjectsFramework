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
using NakedFramework;
using NakedObjects;
using RestfulObjects.Test.Data.Wrapper;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data.Wrapper {
    public class SetWrapper<T> : ISet<T> {
        private readonly ICollection<T> wrapped;

        public SetWrapper(ICollection<T> wrapped) => this.wrapped = wrapped;

        #region ISet<T> Members

        public IEnumerator<T> GetEnumerator() => wrapped.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

        public bool IsSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsProperSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsProperSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool Overlaps(IEnumerable<T> other) => throw new NotImplementedException();

        public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();

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

        public bool Contains(T item) => throw new NotImplementedException();

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(T item) => throw new NotImplementedException();

        public int Count => wrapped.Count;

        public bool IsReadOnly => wrapped.IsReadOnly;

        #endregion
    }
}

namespace RestfulObjects.Test.Data {
    public class WithCollection {
        private ICollection<MostSimpleViewModel> aCollectionViewModels = new List<MostSimpleViewModel>();
        private IDomainObjectContainer container;

        public IDomainObjectContainer Container {
            set {
                container = value;
                // hack for testing 
                var vm1 = container.NewViewModel<MostSimpleViewModel>();
                var vm2 = container.NewViewModel<MostSimpleViewModel>();
                vm1.Id = 1;
                vm2.Id = 2;

                aCollectionViewModels.Add(vm1);
                aCollectionViewModels.Add(vm2);
            }
            protected get { return container; }
        }

        [Key]
        [Title]
        [ConcurrencyCheck]
        [DefaultValue(0)]
        public virtual int Id { get; set; }

        [PresentationHint("class7 class8")]
        public virtual ICollection<MostSimple> ACollection { get; set; } = new List<MostSimple>();

        [NotPersisted]
        public virtual ICollection<MostSimpleViewModel> ACollectionViewModels {
            get => aCollectionViewModels;
            set => aCollectionViewModels = value;
        }

        [NakedObjectsIgnore]
        public virtual ICollection<MostSimple> ASetAsCollection { get; set; } = new List<MostSimple>();

        [NotMapped]
        [NotPersisted]
        public virtual ISet<MostSimple> ASet {
            get => new SetWrapper<MostSimple>(ASetAsCollection);
            set => ASetAsCollection = value;
        }

        [Disabled]
        public virtual ICollection<MostSimple> ADisabledCollection { get; set; } = new List<MostSimple>();

        [Hidden(WhenTo.Always)]
        public virtual ICollection<MostSimple> AHiddenCollection { get; set; } = new List<MostSimple>();

        [NakedObjectsIgnore]
        public virtual ICollection<MostSimple> ANakedObjectsIgnoredCollection { get; set; } = new List<MostSimple>();

        [DescribedAs("an empty collection for testing")]
        [MemberOrder(Sequence = "2")]
        public virtual ICollection<MostSimple> AnEmptyCollection { get; set; } = new List<MostSimple>();

        [DescribedAs("an empty set for testing")]
        [MemberOrder(Sequence = "2")]
        [NotMapped]
        [NotPersisted]
        public virtual ISet<MostSimple> AnEmptySet { get; set; } = new HashSet<MostSimple>();

        [Eagerly(Do.Rendering)]
        public virtual ICollection<MostSimple> AnEagerCollection { get; set; } = new List<MostSimple>();

        public IList<MostSimple> LocallyContributedAction([ContributedAction] IList<MostSimple> acollection) => acollection;

        public IList<MostSimple> LocallyContributedActionWithParm([ContributedAction] IList<MostSimple> acollection, string p1) => acollection;
    }
}