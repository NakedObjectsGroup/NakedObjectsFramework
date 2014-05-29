// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Xml.Linq;

namespace NakedObjects.Snapshot.Xml.Utility {
    public static class Helper {

        // Helper method
        public static string TrailingSlash(string str) {
            return str.EndsWith("/") ? str : str + "/";
        }

        // Utility method that returns just the class's name for the supplied fully qualified class name.
        // cf 'basename' in Unix.
        public static string ClassNameFor(string fullyQualifiedClassName) {
            int fullNameLastPeriodIdx = fullyQualifiedClassName.LastIndexOf('.');
            if (fullNameLastPeriodIdx > 0 && fullNameLastPeriodIdx < fullyQualifiedClassName.Length) {
                return fullyQualifiedClassName.Substring(fullNameLastPeriodIdx + 1);
            }
            return fullyQualifiedClassName;
        }

        // Utility method that returns the package name for the supplied fully qualified class name, or
        // <code>default</code> if  the class is in no namespace / in the default namespace.
        // cf 'dirname' in Unix.
        public static string PackageNameFor(string fullyQualifiedClassName) {
            int fullNameLastPeriodIdx = fullyQualifiedClassName.LastIndexOf('.');
            if (fullNameLastPeriodIdx > 0) {
                return fullyQualifiedClassName.Substring(0, fullNameLastPeriodIdx);
            }
            return "default"; // TODO: should provide a better way to specify namespace.
        }

        // Returns the root element for the element by looking up the owner document for the element,
        // and from that its document element.
        // If no document element exists, just returns the supplied document.
        public static XElement RootElementFor(XElement element) {
            XDocument doc = element.Document;
            if (doc == null) {
                return element;
            }
            XElement rootElement = doc.Root;
            if (rootElement == null) {
                return element;
            }
            return rootElement;
        }

        public static XDocument DocFor(XElement element) {
            return element.Document;
        }
    }
}