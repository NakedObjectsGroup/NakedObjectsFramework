// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NakedObjects.Web.Mvc.Html {
    internal class ElementDescriptor {
        public string TagType { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public IDictionary<string, object> Attributes { get; set; }
        public IList<ElementDescriptor> Children { get; set; }

        public static TagBuilder BuildElementSet(IEnumerable<ElementDescriptor> elements) {
            var fieldSet = new TagBuilder("div") {InnerHtml = Environment.NewLine};
            foreach (ElementDescriptor field in elements) {
                field.BuildElement(fieldSet);
            }
            return fieldSet;
        }

        public string BuildElement() {
            if (!string.IsNullOrEmpty(TagType)) {
                var tag = new TagBuilder(TagType) {InnerHtml = Label + Value};
                tag.MergeAttributes(Attributes);
                if (Children != null && Children.Any()) {
                    foreach (ElementDescriptor child in Children) {
                        child.BuildElement(tag);
                    }
                }

                return (tag + Environment.NewLine);
            }
            return "";
        }

        public void BuildElement(TagBuilder parentTag) {
            parentTag.InnerHtml += BuildElement();
        }
    }
}