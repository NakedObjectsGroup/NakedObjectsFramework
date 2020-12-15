﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Xml.Linq;

namespace NakedObjects.Snapshot.Xml.Utility {
    public static class Helper {
        // Helper method
        public static string TrailingSlash(string str) => str.EndsWith("/") ? str : str + "/";

        // Utility method that returns just the class's name for the supplied fully qualified class name.
        // cf 'basename' in Unix.
        public static string ClassNameFor(string fullyQualifiedClassName) {
            var fullNameLastPeriodIdx = fullyQualifiedClassName.LastIndexOf('.');
            if (fullNameLastPeriodIdx > 0 && fullNameLastPeriodIdx < fullyQualifiedClassName.Length) {
                return fullyQualifiedClassName.Substring(fullNameLastPeriodIdx + 1);
            }

            return fullyQualifiedClassName;
        }

        // Utility method that returns the package name for the supplied fully qualified class name, or
        // <code>default</code> if  the class is in no namespace / in the default namespace.
        // cf 'dirname' in Unix.
        public static string PackageNameFor(string fullyQualifiedClassName) {
            var fullNameLastPeriodIdx = fullyQualifiedClassName.LastIndexOf('.');
            if (fullNameLastPeriodIdx > 0) {
                return fullyQualifiedClassName.Substring(0, fullNameLastPeriodIdx);
            }

            return "default";
        }

        // Returns the root element for the element by looking up the owner document for the element,
        // and from that its document element.
        // If no document element exists, just returns the supplied document.
        public static XElement RootElementFor(XElement element) {
            var doc = element.Document;
            if (doc == null) {
                return element;
            }

            return doc.Root ?? element;
        }

        public static XDocument DocFor(XElement element) => element.Document;
    }
}