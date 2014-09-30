// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;

namespace NakedObjects {
    [Bounded]
    [ActionOrder("1, 2, 3")]
    [FieldOrder("4, 5, 6")]
    [Immutable(WhenTo.OncePersisted)]
    [Named("singular name")]
    [Plural("plural name")]
    public class ObjectWithAnnotations {
        public void Side([Named("one")] [Optionally] string param) {}


        public void Start() {}


        public void Top() {}

        [Hidden]
        public void Stop() {}

        public int GetOne() {
            return 1;
        }

        public void SetOne(int value) {}

        [Disabled]
        public string GetTwo() {
            return "";
        }


        [DescribedAs("description text")]
        [Named("name text")]
        public IList GetCollection() {
            return null;
        }

        [Executed(Where.Locally)]
        public void Left() {}

        [Executed(Where.Remotely)]
        public void Right() {}

        [Disabled]
        public void Bottom() {}

        public void Complete(
            string notMultiline,
            [MultiLine(NumberOfLines = 10)] string multiLine) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}