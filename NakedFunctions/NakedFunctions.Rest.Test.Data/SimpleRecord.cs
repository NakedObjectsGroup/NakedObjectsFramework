// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFramework;

namespace NakedFunctions.Rest.Test.Data {

    public record SimpleRecord {
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }
        public override string ToString() => Name;

        public virtual bool Equals(SimpleRecord other) => ReferenceEquals(this, other);
        public override int GetHashCode() => base.GetHashCode();
    }

    public record DateRecord {
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; } = DateTime.Now;
    }

    public enum TestEnum {
        ValueOne, 
        ValueTwo
    }

    public record EnumRecord
    {
        [Key]
        public int Id { get; init; }

        public TestEnum TestEnum { get; set; }
    }

    public record ReferenceRecord
    {
        [Key]
        public int Id { get; init; }

        public SimpleRecord SimpleRecord { get; init; }
        public DateRecord DateRecord { get; init; }

        public override string ToString() => $"{Id}-{SimpleRecord.Id}-{DateRecord.Id}";
    }



    public record GuidRecord {
        [Key]
        public int Id { get; init; }

        public Guid Name => new(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
        public override string ToString() => Name.ToString();
    }
}