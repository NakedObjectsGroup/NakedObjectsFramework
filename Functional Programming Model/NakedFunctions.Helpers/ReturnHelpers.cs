// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFunctions {
    /// <summary>
    /// Helper functions for returning items from action functions
    /// </summary>
    /// 
    public static class Result {

        public static string MessageOnly(string message) {
            return message;
        }

        public static T Display<T>(T item) {
            return item;
        }

        public static (T, string) Display<T>(T item, string withMessage) {
            return (item, withMessage);
        }

        public static (T, T) DisplayAndPersist<T>(T item) {
            return (item, item);
        }

        public static (T, T, string) DisplayAndPersist<T>(T item, string withMessage) {
            return (item, item, withMessage);
        }

        public static (T, U) DisplayAndPersistDifferentItems<T, U>(T itemToDisplay, U toPersist) {
            return (itemToDisplay, toPersist);
        }

        public static (T, U, string) DisplayAndPersistDifferentItems<T, U>(T itemToDisplay, U toPersist, string withMessage) {
            return (itemToDisplay, toPersist, withMessage);
        }
    }
}