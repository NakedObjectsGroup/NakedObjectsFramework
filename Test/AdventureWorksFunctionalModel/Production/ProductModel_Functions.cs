// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class ProductModel_Functions {
        [MemberOrder(22)]
        public static ProductDescription LocalCultureDescription(ProductModel pm) =>
            pm.ProductModelProductDescriptionCulture
              .Where(obj => obj.Culture != null && obj.Culture.CultureID.StartsWith(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
              .Select(obj => obj.ProductDescription)
              .FirstOrDefault();

        public static string CatalogDescription(ProductModel pm) =>
            string.IsNullOrEmpty(pm.CatalogDescription) ? "" : XElement.Parse(pm.CatalogDescription).Elements().Select(e => Formatted(e)).Aggregate((i, j) => i + j);

        private static string Formatted(XElement n) =>
            n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n";
    }
}