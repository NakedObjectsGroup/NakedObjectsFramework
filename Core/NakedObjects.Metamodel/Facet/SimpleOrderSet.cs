// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Metamodel.Facet {
    public class SimpleOrderSet<T> : OrderSet<T> where T : IOrderableElement<T>, ISpecification {
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