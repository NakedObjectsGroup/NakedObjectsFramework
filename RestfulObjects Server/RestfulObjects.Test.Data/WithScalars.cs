// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public enum TestEnum {
        Value1,
        Value2
    }


    public class WithScalars {
        private DateTime dateTime = new DateTime(2012, 03, 27, 08, 42, 36, 0, DateTimeKind.Utc);
        private IList<MostSimple> list = new List<MostSimple>();
        private ISet<MostSimple> set = new HashSet<MostSimple>();

        [Key, Title]
        public virtual int Id { get; set; }

        [NotMapped]
        public virtual sbyte SByte { get; set; }

        public virtual byte Byte { get; set; }
        public virtual short Short { get; set; }
        public virtual ushort UShort { get; set; }
        public virtual int Int { get; set; }
        public virtual uint UInt { get; set; }
        public virtual long Long { get; set; }
        public virtual ulong ULong { get; set; }
        public virtual char Char { get; set; }
        public virtual bool Bool { get; set; }
        public virtual string String { get; set; }
        public virtual float Float { get; set; }
        public virtual double Double { get; set; }
        public virtual decimal Decimal { get; set; }
        public virtual byte[] ByteArray { get; set; }
        public virtual sbyte[] SByteArray { get; set; }
        public virtual char[] CharArray { get; set; }

        public virtual DateTime DateTime {
            get { return dateTime; }
            set { dateTime = value; }
        }

        [NotMapped]
        public virtual IList<MostSimple> List {
            get { return list; }
            set { list = value; }
        }

        [NotMapped]
        public virtual ISet<MostSimple> Set {
            get { return set; }
            set { set = value; }
        }

        [EnumDataType(typeof (TestEnum))]
        public virtual int EnumByAttributeChoices { get; set; }
    }
}