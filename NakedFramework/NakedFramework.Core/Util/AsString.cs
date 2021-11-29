// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Text;

namespace NakedFramework.Core.Util {
    public sealed class AsString {
        private readonly StringBuilder buffer;
        private bool addComma;

        public AsString(object forObject) {
            buffer = new StringBuilder();
            CreateName(forObject, buffer);
            buffer.Append(" [");
        }

        private static void CreateName(object forObject, StringBuilder buffer) {
            buffer.Append(Name(forObject));
            buffer.Append("@");
            buffer.Append(Convert.ToString(forObject.GetHashCode(), 16));
        }

        public static string Name(object forObject) {
            var name = forObject.GetType().FullName;
            return name[(name.LastIndexOf('.') + 1)..];
        }

        public AsString Append(string text) {
            buffer.Append(text);
            return this;
        }

        public AsString Append(string name, bool flag) {
            Append(name, flag ? "true" : "false");
            return this;
        }

        public AsString Append(string name, object obj) {
            Append(name, obj == null ? "null" : obj.ToString());
            return this;
        }

        public AsString Append(string name, string stringValue) {
            if (addComma) {
                buffer.Append(',');
            }
            else {
                addComma = true;
            }

            buffer.Append(name);
            buffer.Append('=');
            buffer.Append(stringValue);

            return this;
        }

        public AsString AppendAsHex(string name, long number) {
            Append(name, "#" + Convert.ToString(number, 16));
            return this;
        }

        public void AddComma() {
            addComma = true;
        }

        public override string ToString() {
            buffer.Append(']');
            return buffer.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}