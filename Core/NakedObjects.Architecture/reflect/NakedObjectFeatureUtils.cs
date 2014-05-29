// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;

namespace NakedObjects.Architecture.Reflect {
    public static class NakedObjectFeatureUtils {
        public static string PropertyTitle(this INakedObjectFeature feature, INakedObject nakedObject) {
            if (nakedObject == null) {
                return "";
            }
            string text = null;
            var regex = feature.GetFacet<IRegExFacet>();
            if (regex != null) {
                text = regex.Format(nakedObject.TitleString());
            }
            var mask = feature.GetFacet<IMaskFacet>();
            if (mask != null) {
                var title = feature.Specification.GetFacet<ITitleFacet>();
                text = title.GetTitleWithMask(mask.Value, nakedObject);
            }
            if (text == null) {
                text = nakedObject.TitleString();
            }
            return text;
        }
    }
}