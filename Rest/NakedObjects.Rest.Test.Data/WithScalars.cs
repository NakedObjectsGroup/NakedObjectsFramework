// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data {
    public enum TestEnum {
        Value1,
        Value2
    }

    public class WithScalars {
        private char c;

        public WithScalars() {
            SByte = 10;
            UInt = 14;
            ULong = 15;
            UShort = 16;
            DateTime = DateTime.UtcNow;
        }

        [Key]
        [Title]
        [ConcurrencyCheck]
        [DefaultValue(0)]
        public virtual int Id { get; set; }

        [NotMapped]
        public virtual sbyte SByte { get; set; }

        public virtual byte Byte { get; set; }
        public virtual short Short { get; set; }

        [NotMapped]
        public virtual ushort UShort { get; set; }

        public virtual int Int { get; set; }

        [NotMapped]
        [Range(0, 400)]
        public virtual int IntWithRange { get; set; }

        [NotMapped]
        public virtual uint UInt { get; set; }

        public virtual long Long { get; set; }

        [NotMapped]
        public virtual ulong ULong { get; set; }

        public virtual char Char {
            get => '3';
            set => c = value;
        }

        public virtual bool Bool { get; set; }
        public virtual string String { get; set; }
        public virtual float Float { get; set; }
        public virtual double Double { get; set; }
        public virtual decimal Decimal { get; set; }
        public virtual byte[] ByteArray { get; set; }
        public virtual sbyte[] SByteArray { get; set; }
        public virtual char[] CharArray { get; set; }

        public virtual DateTime DateTime { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        public virtual string Password { get; set; }

        public virtual ICollection<MostSimple> List { get; set; } = new List<MostSimple>();

        [NotMapped]
        public virtual ISet<MostSimple> Set { get; set; } = new HashSet<MostSimple>();

        [EnumDataType(typeof(TestEnum))]
        public virtual int EnumByAttributeChoices { get; set; }
    }
}