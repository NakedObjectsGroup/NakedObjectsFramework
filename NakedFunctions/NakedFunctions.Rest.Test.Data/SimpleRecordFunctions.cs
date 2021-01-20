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
    internal static class Helpers {
        internal static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));
    }

    public static class SimpleRecordFunctions {
        [Edit]
        [PresentationHint("Hint3")]
        public static (SimpleRecord, IContext) EditSimpleRecord(this SimpleRecord sp, [PresentationHint("Hint4")] string name, IContext context)
            => Helpers.DisplayAndSave(sp with {Name = name}, context);

        public static (SimpleRecord, IContext) CreateSimpleRecord(this SimpleRecord sp, string name, IContext context)
            => Helpers.DisplayAndSave(new SimpleRecord {Name = name}, context);

        public static (ReferenceRecord, IContext) AssociateWithDateRecord(this SimpleRecord simpleRecord, DateRecord dateRecord, IContext context) =>
            context.Instances<ReferenceRecord>().Any(x => x.SimpleRecord.Id == simpleRecord.Id && x.DateRecord.Id == dateRecord.Id)
                ? (null, context.WithInformUser($"{simpleRecord} is already associated with {dateRecord}"))
                : Helpers.DisplayAndSave(new ReferenceRecord() with {SimpleRecord = simpleRecord, DateRecord = dateRecord}, context);

        [PageSize(20)]
        public static IQueryable<DateRecord> AutoComplete1AssociateWithDateRecord(this SimpleRecord simpleRecord, [MinLength(2)] string name, IContext context)
            => context.Instances<DateRecord>().Where(simpleRecord => simpleRecord.Name.ToUpper().StartsWith(name.ToUpper()));

        public static SimpleRecord EnumParmSimpleRecord(this SimpleRecord sp, TestEnum eParm, IContext context) => sp;

        public static SimpleRecord PasswordParmSimpleRecord(this SimpleRecord sp, [Password] string parm, IContext context) => sp;
    }

    public static class DateRecordFunctions {
        [Edit]
        public static (DateRecord, IContext) EditDates(this DateRecord sp, DateTime startDate, DateTime endDate, IContext context)
            => Helpers.DisplayAndSave(sp with {StartDate = startDate, EndDate = endDate}, context);

        public static DateTime Default1EditDates(this DateRecord sp, IContext context) => context.GetService<IClock>().Today();

        public static DateTime Default2EditDates(this DateRecord sp, IContext context) => context.GetService<IClock>().Today().AddDays(90);

        public static DateRecord DateWithDefault(this DateRecord sp, [DefaultValue(22)] DateTime dt, IContext context) => sp;
    }

    public static class ChoicesRecordFunctions {
        public static SimpleRecord WithChoices(this SimpleRecord sp, SimpleRecord record, IContext context) => record;

        public static IList<SimpleRecord> Choices1WithChoices(this SimpleRecord sp, IContext context) => context.Instances<SimpleRecord>().ToList();

        public static SimpleRecord WithChoicesWithParameters(this SimpleRecord sp, SimpleRecord record, int parm1, string parm2, IContext context) => record;

        public static IList<SimpleRecord> Choices1WithChoicesWithParameters(this SimpleRecord sp, int parm1, string parm2, IContext context) =>
            context.Instances<SimpleRecord>().Where(sr => sr.Name.StartsWith(parm2)).Take(parm1).ToList();

        public static IQueryable<SimpleRecord> WithMultipleChoices(this SimpleRecord sp, IEnumerable<SimpleRecord> simpleRecords, IEnumerable<DateRecord> dateRecords, IContext context) => simpleRecords.AsQueryable();

        public static IQueryable<SimpleRecord> Choices1WithMultipleChoices(this SimpleRecord sp, IContext context) => context.Instances<SimpleRecord>();

        public static IQueryable<DateRecord> Choices2WithMultipleChoices(this SimpleRecord sp, IEnumerable<SimpleRecord> simpleRecords, IContext context) => context.Instances<DateRecord>();
    }

    public static class DefaultedRecordFunctions {
        public static SimpleRecord WithDefaults(this SimpleRecord sp, int default1, SimpleRecord default2, IContext context) => sp;

        public static int Default1WithDefaults(this SimpleRecord sp, IContext context) => 101;
        public static SimpleRecord Default2WithDefaults(this SimpleRecord sp, IContext context) => context.Instances<SimpleRecord>().First();
    }

    public static class ValidatedRecordFunctions {
        public static SimpleRecord WithValidation(this SimpleRecord sp, int validate1, IContext context) => sp;

        public static string Validate1WithValidation(this SimpleRecord sp, int validate1, IContext context) => validate1 == 1 ? "" : "invalid";

        public static SimpleRecord WithCrossValidation(this SimpleRecord sp, int validate1, string validate2, IContext context) => sp;

        public static string ValidateWithCrossValidation(this SimpleRecord sp, int validate1, string validate2, IContext context) =>
            validate1 == int.Parse(validate2) ? "" : $"invalid: {validate1}:{validate2}";
    }
}