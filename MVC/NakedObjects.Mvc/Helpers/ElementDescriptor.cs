// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
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