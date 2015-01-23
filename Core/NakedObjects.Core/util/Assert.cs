// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Core.Util {
    // TODO not sure it is a good idea to have our own implementation of assert
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