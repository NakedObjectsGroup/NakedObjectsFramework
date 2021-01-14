// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;

namespace NakedFunctions.Rest.Test.Data {
    internal static class Helpers {
        internal static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));
    }

    public static class SimpleRecordFunctions {
        [Edit]
        public static (SimpleRecord, IContext) EditSimpleRecord(this SimpleRecord sp, string name, IContext context)
            => Helpers.DisplayAndSave(sp with {Name = name}, context);

        public static (SimpleRecord, IContext) CreateSimpleRecord(this SimpleRecord sp, string name, IContext context)
            => Helpers.DisplayAndSave(new SimpleRecord() { Name = name }, context);

        public static (ReferenceRecord, IContext) AssociateWithDateRecord(this SimpleRecord simpleRecord, DateRecord dateRecord, IContext context) =>
            context.Instances<ReferenceRecord>().Any(x => x.SimpleRecord.Id == simpleRecord.Id && x.DateRecord.Id == dateRecord.Id)
                ? (null, context.WithInformUser($"{simpleRecord} is already associated with {dateRecord}"))
                : Helpers.DisplayAndSave(new ReferenceRecord() with { SimpleRecord = simpleRecord, DateRecord = dateRecord }, context);

        [PageSize(20)]
        public static IQueryable<DateRecord> AutoComplete1AssociateWithDateRecord(this SimpleRecord simpleRecord, [Length(2)] string name, IContext context)
            => context.Instances<DateRecord>().Where(simpleRecord => simpleRecord.Name.ToUpper().StartsWith(name.ToUpper()));

    }

    public static class DateRecordFunctions {
        [Edit]
        public static (DateRecord, IContext) EditDates(this DateRecord sp, DateTime startDate, DateTime endDate, IContext context)
            => Helpers.DisplayAndSave(sp with {StartDate = startDate, EndDate = endDate}, context);

        public static DateTime Default1EditDates(this DateRecord sp, IContext context) => context.GetService<IClock>().Today();

        public static DateTime Default2EditDates(this DateRecord sp, IContext context) => context.GetService<IClock>().Today().AddDays(90);
    }
}