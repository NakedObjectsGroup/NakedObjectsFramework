// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering {
    public class SimpleOrderSet : OrderSet {
        private readonly INakedObjectMemberPeer[] members;
        private readonly SimpleOrderSet parent;

        private SimpleOrderSet(INakedObjectMemberPeer[] members)
            : base("") {
            this.members = members;
            parent = null;
        }

        private SimpleOrderSet(SimpleOrderSet set, string groupName, string name, INakedObjectMemberPeer[] members)
            : base(groupName) {
            parent = set;
            parent.AddElement(this);
            this.members = members;
            Add(name);
        }

        public static SimpleOrderSet CreateOrderSet(string order, INakedObjectMemberPeer[] members) {
            var set = new SimpleOrderSet(members);

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
            INakedObjectMemberPeer memberWithName = GetMemberWithName(name);
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

        private SimpleOrderSet CreateSubOrderSet(string groupName, string memberName) {
            return new SimpleOrderSet(this, groupName, memberName, members);
        }

        private INakedObjectMemberPeer GetMemberWithName(string name) {
            string searchName = NameUtils.SimpleName(name);
            for (int i = 0; i < members.Length; i++) {
                INakedObjectMemberPeer member = members[i];
                if (member != null) {
                    string testName = NameUtils.SimpleName(member.Identifier.MemberName);
                    if (testName.Equals(searchName)) {
                        members[i] = null;
                        return member;
                    }
                }
            }
            return null;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}