






using System.Linq;
using System.Text;
using System.Xml.Linq;
using NakedFramework;

namespace AW {
    internal static class Utilities {
        internal static string FormatXML(string? inputXML) {
            var output = new StringBuilder();

            if (!string.IsNullOrEmpty(inputXML)) {
                XElement.Parse(inputXML).Elements().ToList().ForEach(n => output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n"));
            }

            return output.ToString();
        }

        internal static int HashCode(object obj, params int[] keys) {
            //Uses Josh Bloch's algorithm
            var hash = 17 * 23 + obj.GetType().GetProxiedType().GetHashCode();
            foreach (var key in keys) {
                hash = hash * 23 + key.GetHashCode();
            }

            return hash;
        }
    }
}