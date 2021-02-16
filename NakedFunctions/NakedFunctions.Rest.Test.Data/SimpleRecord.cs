// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFramework;

namespace NakedFunctions.Rest.Test.Data {
    [PresentationHint("Hint1")]
    public record SimpleRecord {
        [Key]
        public int Id { get; init; }

        [PresentationHint("Hint2")]
        public string Name { get; init; }

        public virtual bool Equals(SimpleRecord other) => ReferenceEquals(this, other);
        public override string ToString() => Name;
        public override int GetHashCode() => base.GetHashCode();
    }

    public record DeleteRecord
    {
        [Key]
        public int Id { get; init; }

        public virtual bool Equals(SimpleRecord other) => ReferenceEquals(this, other);
        public override string ToString() => "";
        public override int GetHashCode() => base.GetHashCode();
    }

    public record OrderedRecord {
        [Key]
        public int Id { get; init; }

        [MemberOrder("name_group", 1)]
        public string Name { get; init; }

        [MemberOrder("name1_group", 2)]
        public string Name1 { get; init; }

        [Hidden]
        public string Name2 { get; init; }

        public virtual bool Equals(OrderedRecord other) => ReferenceEquals(this, other);
        public override string ToString() => Name;
        public override int GetHashCode() => base.GetHashCode();
    }

    public record DateRecord {
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public virtual bool Equals(DateRecord other) => ReferenceEquals(this, other);

        public override int GetHashCode() => base.GetHashCode();
    }

    public enum TestEnum {
        ValueOne,
        ValueTwo
    }

    public record EnumRecord {
        [Key]
        public int Id { get; init; }

        public TestEnum TestEnum { get; set; }
    }

    public record UpdatedRecord {
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }

        public virtual bool Equals(SimpleRecord other) => ReferenceEquals(this, other);
        public override string ToString() => Name;
        public override int GetHashCode() => base.GetHashCode();
    }

    public record ReferenceRecord {
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }

        public virtual UpdatedRecord UpdatedRecord { get; init; }
        public virtual DateRecord DateRecord { get; init; }
        public virtual bool Equals(ReferenceRecord other) => ReferenceEquals(this, other);

        public override string ToString() => $"{Name}-{Id}-{UpdatedRecord?.Id}-{DateRecord?.Id}";

        public override int GetHashCode() => base.GetHashCode();
    }

    public record CollectionRecord {
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }

        public virtual IList<UpdatedRecord> UpdatedRecords { get; init; } = new List<UpdatedRecord>();
        public virtual bool Equals(CollectionRecord other) => ReferenceEquals(this, other);

        public override string ToString() => $"{Name}-{Id}-{UpdatedRecords.Aggregate("", (i, a) => i + a.Id)}";

        public override int GetHashCode() => base.GetHashCode();
    }

    public record GuidRecord {
        [Key]
        public int Id { get; init; }

        public Guid Name => new(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
        public override string ToString() => Name.ToString();
    }

    public record DisplayAsPropertyRecord {
        [Key]
        public int Id { get; init; }
    }

    [ViewModel(typeof(ViewModelFunctions))]
    public record ViewModel {
        public string Name { get; init; }

        public virtual bool Equals(ViewModel other) => ReferenceEquals(this, other);
        public override string ToString() => Name;
        public override int GetHashCode() => base.GetHashCode();
    }

    public record EditRecord {
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }

        public SimpleRecord SimpleRecord { get; set; }

        public string NotMatched { get; init; }

        public override string ToString() => Name;
    }
}