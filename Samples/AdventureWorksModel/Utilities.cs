// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AdventureWorksModel {
    internal class Utilities {
        public static string FormatXML(string inputXML) {
            var output = new StringBuilder();

            if (!string.IsNullOrEmpty(inputXML)) {
                XElement.Parse(inputXML).Elements().ToList().ForEach(n => output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n"));
            }
            return output.ToString();
        }
    }
}