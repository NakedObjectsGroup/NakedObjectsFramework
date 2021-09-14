// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using AW.Types;

namespace AW.Functions {
    public static class Customer_Functions {
        internal static BusinessEntity BusinessEntity(this Customer c) {
            if (c.IsStore()) {
                return c.Store;
            }

            if (c.IsIndividual()) {
                return c.Person;
            }

            throw new Exception("Customer is neither Store nor Person!");
        }

        public static bool HideStore(this Customer c) => !IsStore(c);

        public static bool HidePerson(this Customer c) => !IsIndividual(c);

        internal static bool IsIndividual(this Customer c) => !IsStore(c);

        internal static bool IsStore(this Customer c) => c.Store != null;
    }
}