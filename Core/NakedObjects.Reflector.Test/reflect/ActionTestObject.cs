// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Reflect.DotNet.Reflect {
    public class ActionTestObject {
        public bool ActionCalled { get; set; }

        public void ActionMethod() {
            ActionCalled = true;
        }

        public static string NameMethod() {
            return "about for test";
        }

        public bool InvisibleMethod() {
            return true;
        }

        public string ValidMethod() {
            return "invalid";
        }

        public void ActionWithParam(string str) {}

        public static bool[] MandatoryMethod(string str) {
            return new[] {true};
        }

        public static string[] LabelMethod(string str) {
            return new[] {"label"};
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}