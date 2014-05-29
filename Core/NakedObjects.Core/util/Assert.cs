// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Core.Util {
    public static class Assert {
        public static void AssertEquals(object expected, object actual) {
            AssertEquals("", expected, actual);
        }

        public static void AssertEquals(string message, int expected, int intValue) {
            if (expected != intValue) {
                throw new NakedObjectAssertException(message + " expected " + expected + "; but was " + intValue);
            }
        }

        public static void AssertEquals(string message, object expected, object actual) {
            AssertTrue(message + ": expected " + expected + " but was " + actual, (expected == null && actual == null) || (expected != null && expected.Equals(actual)));
        }

        public static void AssertFalse(bool flag) {
            AssertFalse("expected false", flag);
        }

        public static void AssertFalse(string message, bool flag) {
            AssertTrue(message, !flag);
        }

        public static void AssertFalse(string message, object target, bool flag) {
            AssertTrue(message, target, !flag);
        }

        public static void AssertNotNull(object identified) {
            AssertNotNull("", identified);
        }

        public static void AssertNotNull(string message, object obj) {
            AssertTrue("unexpected null: " + message, obj != null);
        }

        public static void AssertNotNull(string message, object target, object obj) {
            AssertTrue(message, target, obj != null);
        }

        public static void AssertNull(object obj) {
            AssertTrue("unexpected reference; should be null", obj == null);
        }

        public static void AssertNull(string message, object obj) {
            AssertTrue(message, obj == null);
        }

        public static void AssertSame(object expected, object actual) {
            AssertSame("", expected, actual);
        }

        public static void AssertSame(string message, object expected, object actual) {
            AssertTrue(message + ": expected " + expected + " but was " + actual, expected == actual);
        }

        public static void AssertTrue(bool flag) {
            AssertTrue("expected true", flag);
        }

        public static void AssertTrue(string message, bool flag) {
            AssertTrue(message, null, flag);
        }

        public static void AssertTrue(string message, object target, bool flag) {
            if (!flag) {
                throw new NakedObjectAssertException(message + (target == null ? "" : (": " + target)));
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}