// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering {
    public class SimpleOrderSet<T> : OrderSet<T> where T : IOrderableElement<T>, IFacetHolder {
        private readonly T[] members;
        private readonly SimpleOrderSet<T> parent;

        private SimpleOrderSet(T[] members)
            : base("") {
            this.members = members;
            parent = null;
        }

        private SimpleOrderSet(SimpleOrderSet<T> set, string groupName, string name, T[] members)
            : base(groupName) {
            parent = set;
            parent.AddElement(this);
            this.members = members;
            Add(name);
        }

        public static SimpleOrderSet<T> CreateOrderSet(string order, T[] members) {
            var set = new SimpleOrderSet<T>(members);

            string[] st = order.Split(new[] {','});
            foreach (string element in st) {
                string tempStr = element.Trim();

                bool ends = tempStr.EndsWith(")");
                if (ends) {
                    tempStr = tempStr.Substring(0, tempStr.Length - 1).Trim();
                }

                if (tempStr.StartsWith("(")) {
                    int colon = tempStr.IndexOf(':');
                    string groupName = tempStr.Substring(1, colon).Trim();
                    tempStr = tempStr.Substring(colon + 1).Trim();
                    set = set.CreateSubOrderSet(groupName, tempStr);
                }
                else {
                    set.Add(tempStr);
                }

                if (ends) {
                    set = set.parent;
                }
            }
            set.AddAnyRemainingMember();
            return set;
        }


        private void Add(string name) {
            T memberWithName = GetMemberWithName(name);
            if (memberWithName != null) {
                AddElement(memberWithName);
            }
        }

        private void AddAnyRemainingMember() {
            for (int i = 0; i < members.Length; i++) {
                if (members[i] != null) {
                    AddElement(members[i]);
                }
            }
        }

        private SimpleOrderSet<T> CreateSubOrderSet(string groupName, string memberName) {
            return new SimpleOrderSet<T>(this, groupName, memberName, members);
        }

        private T GetMemberWithName(string name) {
            string searchName = NameUtils.SimpleName(name);
            for (int i = 0; i < members.Length; i++) {
                T member = members[i];
                if (member != null) {
                    string testName = NameUtils.SimpleName(member.Peer.Identifier.MemberName);
                    if (testName.Equals(searchName)) {
                        members[i] = default(T);
                        return member;
                    }
                }
            }
            return default(T);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}