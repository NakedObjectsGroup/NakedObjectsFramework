// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedObjects.Value;

namespace RestfulObjects.Test.Data {

    public enum TestEnum {
        Value1, Value2
    }


    public class WithScalars {
        private DateTime dateTime = new DateTime(2012, 03, 27, 08, 42, 36, 0, DateTimeKind.Utc);
        private IList<MostSimple> list = new List<MostSimple>();
        private ISet<MostSimple> set = new HashSet<MostSimple>();

        [Key, Title]
        public virtual int Id { get; set; }

        public sbyte SByte { get; set; }
        public byte Byte { get; set; }
        public short Short { get; set; }
        public ushort UShort { get; set; }
        public int Int { get; set; }
        public uint UInt { get; set; }
        public long Long { get; set; }
        public ulong ULong { get; set; }
        public char Char { get; set; }
        public bool Bool { get; set; }
        public string String { get; set; }
        public float Float { get; set; }
        public double Double { get; set; }
        public decimal Decimal { get; set; }
        public byte[] ByteArray { get; set; }
        public sbyte[] SByteArray { get; set; }
        public char[] CharArray { get; set; }

        public DateTime DateTime {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public IList<MostSimple> List {
            get { return list; }
            set { list = value; }
        }

        public ISet<MostSimple> Set {
            get { return set; }
            set { set = value; }
        }

        [EnumDataType(typeof(TestEnum))]
        public int EnumByAttributeChoices { get; set; }
    }
}