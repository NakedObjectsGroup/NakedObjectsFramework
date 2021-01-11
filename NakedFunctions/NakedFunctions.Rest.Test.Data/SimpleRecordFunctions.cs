// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions.Rest.Test.Data {
    public static class SimpleRecordFunctions {
        internal static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));

        [Edit]
        public static (SimpleRecord, IContext) EditSimpleRecord(this SimpleRecord sp, string name, IContext context)
            => DisplayAndSave(sp with {Name = name}, context);

        //public static SimpleRecord ReShowRecord(this SimpleRecord simpleRecord) => simpleRecord;

        //public static SimpleRecord UpdateSimpleRecord(this SimpleRecord simpleRecord, IQueryable<SimpleRecord> allSimpleRecords, string name) {
        //    var updatedSr = simpleRecord with {
        //        Name = name
        //    };

        //    return updatedSr;
        //}

        //public static (SimpleRecord, SimpleRecord) UpdateAndPersistSimpleRecord(this SimpleRecord simpleRecord, IQueryable<SimpleRecord> allSimpleRecords, string name) {
        //    var updatedSr = simpleRecord with
        //    {
        //        Name = name
        //    } ;

        //    return (updatedSr, updatedSr);
        //}

        //public static (SimpleRecord, Action<IAlert>) GetSimpleRecordWithWarning(this SimpleRecord simpleRecord) =>  (simpleRecord, a => a.WarnUser("a warning"));

        //public static (SimpleRecord, Action<ILogger<SimpleRecord>>) GetSimpleRecordWithLog(this SimpleRecord simpleRecord) => (simpleRecord, l => l.LogInformation("a log"));
    }

    public static class DateRecordFunctions {
        internal static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));

        [Edit]
        public static (DateRecord, IContext) EditDates(this DateRecord sp, DateTime startDate, DateTime endDate, IContext context)
            => DisplayAndSave(sp with {StartDate = startDate, EndDate = endDate}, context);

        public static DateTime Default0EditDates(this DateRecord sp, IContext context) => context.GetService<IClock>().Today();

        public static DateTime Default1EditDates(this DateRecord sp, IContext context) => context.GetService<IClock>().Today().AddDays(90);
    }
}