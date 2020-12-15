// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


using System;
using System.Collections.Generic;
using System.Linq;

namespace NakedFunctions.Rest.Test.Data {
    public static class SimpleMenuFunctions {
        public static SimpleRecord GetSimpleRecord(IQueryable<SimpleRecord> allSimpleRecords) => allSimpleRecords.First();
        public static IList<SimpleRecord> GetSimpleRecordsSingle(IQueryable<SimpleRecord> allSimpleRecords) => new[] {allSimpleRecords.First()};
        public static IList<SimpleRecord> GetSimpleRecordsMultiple(IQueryable<SimpleRecord> allSimpleRecords) => allSimpleRecords.ToList().SkipLast(1).ToList();

        public static (SimpleRecord, SimpleRecord) GetAndUpdateSimpleRecord(IQueryable<SimpleRecord> allSimpleRecords) {
            var sr = allSimpleRecords.ToList().Last();
            var updatedSr = UpdateName(sr, "0");
            return (updatedSr, updatedSr);
        }

        public static (IList<SimpleRecord>, IList<SimpleRecord>) GetAndUpdateSimpleRecords(IQueryable<SimpleRecord> allSimpleRecords) {
            var updated = allSimpleRecords.ToList().Select(sr => UpdateName(sr, "1")).ToList();
            return (updated, updated);
        }

        public static (SimpleRecord, IList<SimpleRecord>) GetSimpleRecordAndUpdateSimpleRecords(IQueryable<SimpleRecord> allSimpleRecords) {
            var updated = allSimpleRecords.ToList().Select(sr => UpdateName(sr, "2")).ToList();
            return (updated.First(), updated);
        }

        public static (SimpleRecord, SimpleRecord, SimpleRecord) GetSimpleRecordAndUpdateSimpleRecordsByTuple(IQueryable<SimpleRecord> allSimpleRecords) {
            var updated = allSimpleRecords.ToList().Select(sr => UpdateName(sr, "3")).ToList();
            return (updated.First(), updated[0], updated[1]);
        }

        public static (IList<SimpleRecord>, SimpleRecord, SimpleRecord) GetAndUpdateSimpleRecordsByTuple(IQueryable<SimpleRecord> allSimpleRecords) {
            var updated = allSimpleRecords.ToList().Select(sr => UpdateName(sr, "4")).ToList();
            return (updated, updated[0], updated[1]);
        }

        public static (SimpleRecord, (SimpleRecord, SimpleRecord)) GetSimpleRecordAndUpdateSimpleRecordsBySubTuple(IQueryable<SimpleRecord> allSimpleRecords) {
            var updated = allSimpleRecords.ToList().Select(sr => UpdateName(sr, "5")).ToList();
            return (updated.First(), (updated[0], updated[1]));
        }

        public static (IList<SimpleRecord>, (SimpleRecord, SimpleRecord)) GetAndUpdateSimpleRecordsBySubTuple(IQueryable<SimpleRecord> allSimpleRecords) {
            var updated = allSimpleRecords.ToList().Select(sr => UpdateName(sr, "6")).ToList();
            return (updated, (updated[0], updated[1]));
        }

        public static (SimpleRecord, Action<IAlert>) GetSimpleRecordWithWarning(IQueryable<SimpleRecord> allSimpleRecords) =>
            (allSimpleRecords.First(), a => a.WarnUser("a warning"));

        private static SimpleRecord UpdateName(SimpleRecord sr, string suffix) => sr with {
            Name = $"{sr.Name.Substring(0, 4)}{suffix}"
        };
    }
}