// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Parseable {
    public class ParseableFacetAnnotation<T> : ParseableFacetAbstract<T> {
        public ParseableFacetAnnotation(Type annotatedClass, IFacetHolder holder)
            : this(ParserName(annotatedClass), ParserClass(annotatedClass), holder) {}

        private ParseableFacetAnnotation(string candidateParserName, Type candidateParserClass, IFacetHolder holder)
            : base(candidateParserName, candidateParserClass, holder) {}

        private static string ParserName(Type annotatedClass) {
            var annotation = annotatedClass.GetCustomAttributeByReflection<ParseableAttribute>();
            string parserName = annotation.ParserName;
            return !string.IsNullOrEmpty(parserName) ? parserName : null;
        }

        private static Type ParserClass(Type annotatedClass) {
            return annotatedClass.GetCustomAttributeByReflection<ParseableAttribute>().ParserClass;
        }
    }
}