// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions.Reflector.Component;

namespace NakedFunctions.Rest.Test.Data {
    internal static class Helpers {
        internal static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithNew(obj));

        internal static (T, IContext) DisplayAndUpdate<T>(T obj, T proxy, IContext context) => (obj, ((FunctionalContext) context).WithUpdated(proxy, obj));
    }

    public static class SimpleRecordFunctions {
        [Edit]
        [PresentationHint("Hint3")]
        public static (SimpleRecord, IContext) EditSimpleRecord(this SimpleRecord sp, [PresentationHint("Hint4")] string name, IContext context) => Helpers.DisplayAndUpdate(sp with {Name = name}, sp, context);

        [Edit]
        public static (SimpleRecord, IContext) EditSimpleRecordWithPostPersist(this SimpleRecord sp, string name, IContext context) {
            var updated = sp with {Name = name};
            context = context.WithUpdated(sp, updated);
            context = context.WithDeferred(c => {
                var updated2 = updated with {Name = updated.Name + "Updated"};
                c = c.WithUpdated(sp, updated2);
                return c;
            });
            return (updated, context);
        }

        [Edit]
        public static (SimpleRecord, IContext) EditSimpleRecordWithRepeatedPostPersist(this SimpleRecord sp, string name, IContext context) {
            var updated = sp with {Name = name};
            context = context.WithUpdated(sp, updated);
            context = context.WithDeferred(c => {
                var updated2 = updated with {Name = updated.Name + "Updated"};
                c = c.WithUpdated(sp, updated2).WithDeferred(cc => {
                    var updated3 = updated2 with {Name = updated.Name + "Updated" + "Updated"};
                    cc = cc.WithUpdated(sp, updated3).WithDeferred(ccc => {
                        var updated4 = updated3 with {Name = updated.Name + "Updated" + "Updated" + "Updated"};
                        ccc = ccc.WithUpdated(sp, updated4);
                        return ccc;
                    });
                    return cc;
                });
                return c;
            });
            return (updated, context);
        }

        public static (SimpleRecord, IContext) CreateSimpleRecord(this SimpleRecord sp, string name, IContext context) => Helpers.DisplayAndSave(new SimpleRecord {Name = name}, context);

        public static (SimpleRecord, IContext) CreateSimpleRecordWithPostPersist(this SimpleRecord sp, string name, IContext context) {
            var newObj = new SimpleRecord {Name = name};
            context = context.WithNew(newObj);
            context = context.WithDeferred(c => {
                var original = c.Reload(newObj);
                var updated2 = original with {Name = newObj.Name + "Updated"};
                c = c.WithUpdated(original, updated2);
                return c;
            });
            return (newObj, context);
        }

        public static (SimpleRecord, IContext) CreateSimpleRecordWithRepeatedPostPersist(this SimpleRecord sp, string name, IContext context) {
            var newObj = new SimpleRecord {Name = name};
            context = context.WithNew(newObj);
            context = context.WithDeferred(c => {
                var original = c.Reload(newObj);
                var updated2 = original with {Name = newObj.Name + "Updated"};
                c = c.WithUpdated(original, updated2).WithDeferred(cc => {
                    var updated3 = updated2 with {Name = newObj.Name + "Updated"};
                    cc = cc.WithUpdated(original, updated3).WithDeferred(ccc => {
                        var updated4 = updated3 with {Name = newObj.Name + "Updated"};
                        ccc = ccc.WithUpdated(original, updated4);
                        return ccc;
                    });
                    return cc;
                });
                return c;
            });
            return (newObj, context);
        }

        //public static (ReferenceRecord, IContext) AssociateWithDateRecord(this SimpleRecord simpleRecord, DateRecord dateRecord, IContext context) {
        //    return context.Instances<ReferenceRecord>().Any(x => x.UpdatedRecord.Id == simpleRecord.Id && x.DateRecord.Id == dateRecord.Id)
        //        ? (null, context.WithInformUser($"{simpleRecord} is already associated with {dateRecord}"))
        //        : Helpers.DisplayAndSave(new ReferenceRecord() with {UpdatedRecord = simpleRecord, DateRecord = dateRecord}, context);
        //}

        [PageSize(20)]
        public static IQueryable<DateRecord> AutoComplete1AssociateWithDateRecord(this SimpleRecord simpleRecord, [MinLength(2)] string name, IContext context) {
            return context.Instances<DateRecord>().Where(simpleRecord => simpleRecord.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        public static SimpleRecord EnumParmSimpleRecord(this SimpleRecord sp, TestEnum eParm, IContext context) => sp;

        public static SimpleRecord PasswordParmSimpleRecord(this SimpleRecord sp, [Password] string parm, IContext context) => sp;

        public static (SimpleRecord, IContext) SimpleRecordAsCurrentUser(this SimpleRecord sp, IContext context) {
            var updated = sp with {Name = context.CurrentUser().Identity.Name};
            context = context.WithUpdated(sp, updated);

            return (updated, context);
        }

        public static (SimpleRecord, IContext) SimpleRecordAsReset(this SimpleRecord sp, IContext context) {
            var updated = sp with {Name = "Fred"};
            context = context.WithUpdated(sp, updated);

            return (updated, context);
        }
    }

    public static class DateRecordFunctions {
        [Edit]
        public static (DateRecord, IContext) EditDates(this DateRecord sp, DateTime startDate, DateTime endDate, IContext context)
            => Helpers.DisplayAndUpdate(sp with {StartDate = startDate, EndDate = endDate}, sp, context);

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

        public static SimpleRecord WithChoicesNoContext(this SimpleRecord sp, SimpleRecord record, IContext context) => record;
        public static IList<SimpleRecord> Choices1WithChoicesNoContext(this SimpleRecord sp) => new List<SimpleRecord> {sp};

        public static SimpleRecord WithChoicesWithParametersNoContext(this SimpleRecord sp, SimpleRecord record, int parm1, string parm2, IContext context) => record;
        public static IList<SimpleRecord> Choices1WithChoicesWithParametersNoContext(this SimpleRecord sp, int parm1, string parm2) => new List<SimpleRecord> {sp};

        public static IQueryable<SimpleRecord> WithMultipleChoicesNoContext(this SimpleRecord sp, IEnumerable<SimpleRecord> simpleRecords, IEnumerable<DateRecord> dateRecords, IContext context) => simpleRecords.AsQueryable();
        public static IQueryable<SimpleRecord> Choices1WithMultipleChoicesNoContext(this SimpleRecord sp) => new List<SimpleRecord> {sp}.AsQueryable();
        public static IQueryable<DateRecord> Choices2WithMultipleChoicesNoContext(this SimpleRecord sp, IEnumerable<SimpleRecord> simpleRecords) => new List<DateRecord>().AsQueryable();
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

        public static SimpleRecord WithValidationNoContext(this SimpleRecord sp, int validate1, IContext context) => sp;
        public static string Validate1WithValidationNoContext(this SimpleRecord sp, int validate1) => validate1 == 1 ? "" : "invalid";

        public static SimpleRecord WithCrossValidationNoContext(this SimpleRecord sp, int validate1, string validate2, IContext context) => sp;

        public static string ValidateWithCrossValidationNoContext(this SimpleRecord sp, int validate1, string validate2) =>
            validate1 == int.Parse(validate2) ? "" : $"invalid: {validate1}:{validate2}";
    }

    public static class DisabledRecordFunctions {
        public static SimpleRecord WithDisabled1(this SimpleRecord sp, IContext context) => sp;

        public static string DisableWithDisabled1(this SimpleRecord sp, IContext context) => "disabled";

        public static SimpleRecord WithDisabled2(this SimpleRecord sp, IContext context) => sp;

        public static string DisableWithDisabled2(this SimpleRecord sp, IContext context) => "";
    }

    public static class HiddenRecordFunctions {
        public static SimpleRecord WithHidden1(this SimpleRecord sp, IContext context) => sp;

        public static bool HideWithHidden1(this SimpleRecord sp, IContext context) => true;

        public static SimpleRecord WithHidden2(this SimpleRecord sp, IContext context) => sp;

        public static bool HideWithHidden2(this SimpleRecord sp, IContext context) => false;

        public static bool HideName(this SimpleRecord sp, IContext context) => sp.Name == "hide it";
    }

    public static class AutoCompleteRecordFunctions {
        public static SimpleRecord WithAutoComplete(this SimpleRecord sp, SimpleRecord simpleRecord, IContext context) => simpleRecord;

        [PageSize(2)]
        public static IQueryable<SimpleRecord> AutoComplete1WithAutoComplete(this SimpleRecord sp, [MinLength(2)] string name, IContext context) => context.Instances<SimpleRecord>();
    }

    public static class DisplayAsPropertyRecordFunctions {
        [DisplayAsProperty]
        public static DisplayAsPropertyRecord DisplayAsProperty(this DisplayAsPropertyRecord sp, IContext context) => sp;

        [DisplayAsProperty]
        public static IQueryable<DisplayAsPropertyRecord> DisplayAsCollection(this DisplayAsPropertyRecord sp, IContext context) => context.Instances<DisplayAsPropertyRecord>();
    }

    public static class ViewModelFunctions {
        public static string[] DeriveKeys(this ViewModel target) => new[] {target.Name};
        public static ViewModel CreateFromKeys(string[] keys) => new() {Name = keys.First()};
        public static ViewModel UpdateName(this ViewModel vm, string name) => vm with {Name = name};
    }

    public static class OrderedRecordFunctions {
        [MemberOrder("function1_group", 3)]
        public static OrderedRecord Function1(this OrderedRecord or) => or;

        [MemberOrder("function2_group", 2)]
        public static OrderedRecord Function2(this OrderedRecord or) => or;

        [CreateNew]
        public static (OrderedRecord, IContext) CreateNewFunction(this OrderedRecord sp, IContext context) => (sp, context);
    }

    public static class CollectionContributedFunctions {
        public static IContext ContributedFunction1(this IQueryable<SimpleRecord> sr, IContext context) => context;
        public static (SimpleRecord, IContext) ContributedFunction2(this IQueryable<SimpleRecord> sr, IContext context) => (sr.FirstOrDefault(), context);
        public static (ICollection<SimpleRecord>, IContext) ContributedFunction3(this IQueryable<SimpleRecord> sr, int count, IContext context) => (sr.Take(count).ToList(), context);
        public static SimpleRecord ContributedFunction4(this IQueryable<SimpleRecord> sr, IContext context) => sr.FirstOrDefault();

        public static IContext LocalContributedFunction(this CollectionRecord cr, IEnumerable<UpdatedRecord> updatedRecords, IContext context) => context;

        [MemberOrder("UpdatedRecords", 1)]
        public static IContext LocalContributedFunctionByMemberOrder(this CollectionRecord cr, IContext context) => context;
    }

    public static class EditRecordFunctions {
        [Edit]
        public static IContext EditFunction(this EditRecord er, SimpleRecord simpleRecord, string name, string another, IContext context) => context;
    }

    public static class DeleteRecordFunctions {
        public static IContext DeleteFunction(this DeleteRecord dr, IContext context) {
            return context.WithDeleted(dr);
        }

        public static (DeleteRecord, IContext) DeleteFunctionAndReturn(this DeleteRecord dr, IContext context)
        {

            return (dr, context.WithDeleted(dr));
        }
    }
}