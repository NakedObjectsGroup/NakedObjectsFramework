// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     The recommended mechanism for specifying the order in which fields and/or actions are presented to
    ///     the user. (<see cref="ActionOrderAttribute" /> and <see cref="FieldOrderAttribute" /> provide alternative
    ///     mechanisms)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MemberOrderAttribute : Attribute {
        public MemberOrderAttribute() {
            Sequence = "";
            Name = "";
        }

        public MemberOrderAttribute(string sequence) {
            Sequence = sequence;
            Name = "";
        }

        public MemberOrderAttribute(double sequence) {
            Sequence = "" + sequence;
            Name = "";
        }

        public string Sequence { get; set; }

        public string Name { get; set; }
    }
}