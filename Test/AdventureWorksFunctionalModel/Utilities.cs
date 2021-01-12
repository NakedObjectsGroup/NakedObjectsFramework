// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Text;
using System.Xml.Linq;
using NakedFramework;

namespace AW {
    internal static class Utilities {
        internal static string FormatXML(string inputXML) {
            var output = new StringBuilder();

            if (!string.IsNullOrEmpty(inputXML)) {
                XElement.Parse(inputXML).Elements().ToList().ForEach(n => output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n"));
            }
            return output.ToString();
        }

        internal static int HashCode<T>(params int[] keys)
        {
            //Uses Josh Bloch's algorithm
            int hash = 17 * 23 + TypeUtils.GetProxiedType(typeof(T)).GetHashCode();           
            foreach (int key in keys)
            {
                hash = hash * 23 + key.GetHashCode();
            }
            return hash;
        }

    }
}