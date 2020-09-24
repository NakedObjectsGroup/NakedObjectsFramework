// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;

namespace NakedFunctions.Rest.Test.Data {
    public static class SimpleRecordFunctions {
        public static SimpleRecord ReShowRecord(this SimpleRecord simpleRecord) => simpleRecord;

        public static SimpleRecord UpdateSimpleRecord(this SimpleRecord simpleRecord, [Injected] IQueryable<SimpleRecord> allSimpleRecords, string name) {
            var updatedSr = simpleRecord with {
                Name = name
            };

            return updatedSr;
        }


        public static (SimpleRecord, SimpleRecord) UpdateAndPersistSimpleRecord(this SimpleRecord simpleRecord, [Injected] IQueryable<SimpleRecord> allSimpleRecords, string name) {
            var updatedSr = simpleRecord with
            {
                Name = name
            } ;

            return (updatedSr, updatedSr);
        }

        public static (SimpleRecord, Action<IAlert>) GetSimpleRecordWithWarning(this SimpleRecord simpleRecord) =>  (simpleRecord, a => a.WarnUser("a warning"));
    }
}