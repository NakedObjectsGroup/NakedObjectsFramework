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
    public static class MenuTestFunctions {
        [MemberOrder("findbyname_group", 1)]
        public static IQueryable<SimpleRecord> FindByName(string searchString, IContext context) {
            return context.Instances<SimpleRecord>().Where(x => x.Name.ToUpper().Contains(searchString.ToUpper())).OrderBy(x => x.Name);
        }

        public static IQueryable<SimpleRecord> FindByLength(int length, IContext context) {
            return context.Instances<SimpleRecord>().Where(x => x.Name.Length == length).OrderBy(x => x.Name);
        }

        public static SimpleRecord Random(IContext context) {
            var instances = context.Instances<SimpleRecord>().OrderBy(n => "");
            return instances.Skip(context.RandomSeed().ValueInRange(instances.Count())).FirstOrDefault();
        }

        internal static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithNew(obj));

        internal static Func<IContext, IContext> WarnUser(string message) =>
            c => {
                c.GetService<IAlert>().WarnUser(message);
                return c;
            };

        internal static Action<IAlert> InformUser(string message) => ua => ua.InformUser(message);

        internal static (T, IContext) SingleObjectWarnIfNoMatch<T>(this IQueryable<T> query, IContext context) =>
            (query.FirstOrDefault(), query.Any() ? context : context.WithDeferred(WarnUser("There is no matching object")));

        public static (SimpleRecord, IContext) FindByNumber(string number, IContext context) {
            var id = int.Parse(number);
            return context.Instances<SimpleRecord>().Where(x => x.Id == id).SingleObjectWarnIfNoMatch(context);
        }

        public static IQueryable<SimpleRecord> FindByEnum(TestEnum eParm, IContext context) => context.Instances<SimpleRecord>();

        [CreateNew]
        public static (OrderedRecord, IContext) CreateNewFunction(IContext context) => (context.Instances<OrderedRecord>().FirstOrDefault(), context);
    }

    public static class DateMenuFunctions {
        public static DateRecord DateWithDefault([DefaultValue(22)] DateTime dt, IContext context) => new();
    }

    public static class ChoicesMenuFunctions {
        public static SimpleRecord WithChoices(SimpleRecord record, IContext context) => record;

        public static IList<SimpleRecord> Choices0WithChoices(IContext context) => context.Instances<SimpleRecord>().ToList();

        public static SimpleRecord WithChoicesWithParameters(SimpleRecord record, int parm1, string parm2, IContext context) => record;

        public static IList<SimpleRecord> Choices0WithChoicesWithParameters(int parm1, string parm2, IContext context) =>
            context.Instances<SimpleRecord>().Where(sr => sr.Name.StartsWith(parm2)).Take(parm1).ToList();

        public static IQueryable<SimpleRecord> WithMultipleChoices(IEnumerable<SimpleRecord> simpleRecords, IEnumerable<DateRecord> dateRecords, IContext context) => simpleRecords.AsQueryable();
        public static IQueryable<SimpleRecord> Choices0WithMultipleChoices(IContext context) => context.Instances<SimpleRecord>();
        public static IQueryable<DateRecord> Choices1WithMultipleChoices(IEnumerable<SimpleRecord> simpleRecords, IContext context) => context.Instances<DateRecord>();

        public static SimpleRecord WithChoicesNoContext(SimpleRecord record, IContext context) => record;
        public static IList<SimpleRecord> Choices0WithChoicesNoContext() => new List<SimpleRecord>();

        public static SimpleRecord WithChoicesWithParametersNoContext(SimpleRecord record, int parm1, string parm2, IContext context) => record;
        public static IList<SimpleRecord> Choices0WithChoicesWithParametersNoContext(int parm1, string parm2) => new List<SimpleRecord>();

        public static IQueryable<SimpleRecord> WithMultipleChoicesNoContext(IEnumerable<SimpleRecord> simpleRecords, IEnumerable<DateRecord> dateRecords, IContext context) => simpleRecords.AsQueryable();
        public static IQueryable<SimpleRecord> Choices0WithMultipleChoicesNoContext() => new List<SimpleRecord>().AsQueryable();
        public static IQueryable<DateRecord> Choices1WithMultipleChoicesNoContext(IEnumerable<SimpleRecord> simpleRecords) => new List<DateRecord>().AsQueryable();
    }

    public static class DefaultedMenuFunctions {
        public static SimpleRecord WithDefaults(int default1, SimpleRecord default2, IContext context) => context.Instances<SimpleRecord>().First();

        public static int Default0WithDefaults(IContext context) => 101;
        public static SimpleRecord Default1WithDefaults(IContext context) => context.Instances<SimpleRecord>().First();
    }

    public static class ValidatedMenuFunctions {
        public static SimpleRecord WithValidation(int validate1, IContext context) => context.Instances<SimpleRecord>().First();

        public static string Validate0WithValidation(int validate1, IContext context) => validate1 == 1 ? "" : "invalid";

        public static SimpleRecord WithCrossValidation(int validate1, string validate2, IContext context) => context.Instances<SimpleRecord>().First();

        public static string ValidateWithCrossValidation(int validate1, string validate2, IContext context) =>
            validate1 == int.Parse(validate2) ? "" : $"invalid: {validate1}:{validate2}";

        public static SimpleRecord WithValidationNoContext(int validate1, IContext context) => context.Instances<SimpleRecord>().First();

        public static string Validate0WithValidationNoContext(int validate1) => validate1 == 1 ? "" : "invalid";

        public static SimpleRecord WithCrossValidationNoContext(int validate1, string validate2, IContext context) => context.Instances<SimpleRecord>().First();

        public static string ValidateWithCrossValidationNoContext(int validate1, string validate2) =>
            validate1 == int.Parse(validate2) ? "" : $"invalid: {validate1}:{validate2}";
    }

    public static class DisabledMenuFunctions {
        public static SimpleRecord WithDisabled1(IContext context) => context.Instances<SimpleRecord>().First();

        public static string DisableWithDisabled1(IContext context) => "disabled";

        public static SimpleRecord WithDisabled2(IContext context) => context.Instances<SimpleRecord>().First();

        public static string DisableWithDisabled2(IContext context) => "";
    }

    public static class HiddenMenuFunctions {
        public static SimpleRecord WithHidden1(IContext context) => context.Instances<SimpleRecord>().First();

        public static bool HideWithHidden1(IContext context) => true;

        public static SimpleRecord WithHidden2(IContext context) => context.Instances<SimpleRecord>().First();

        public static bool HideWithHidden2(IContext context) => false;
    }

    public static class AutoCompleteMenuFunctions {
        public static SimpleRecord WithAutoComplete(SimpleRecord simpleRecord, IContext context) => simpleRecord;

        [PageSize(2)]
        public static IQueryable<SimpleRecord> AutoComplete0WithAutoComplete([MinLength(2)] string name, IContext context) => context.Instances<SimpleRecord>();
    }

    public static class ViewModelMenuFunctions {
        public static ViewModel GetViewModel(string name, IContext context) => new() {Name = name};
    }

    public static class ReferenceMenuFunctions {
        public static (ReferenceRecord, IContext) CreateNewWithExistingReferences(IContext context) {
            var sr = context.Instances<UpdatedRecord>().First();
            var dr = context.Instances<DateRecord>().First();
            return Helpers.DisplayAndSave(new ReferenceRecord {Name = "Test1", UpdatedRecord = sr, DateRecord = dr}, context);
        }

        public static (ReferenceRecord, IContext) CreateNewWithNewReferences(IContext context) {
           
            var dr = context.Instances<DateRecord>().First();

            var nsr = new UpdatedRecord {Name = "Test2"};
            var nrr = new ReferenceRecord {Name = "Test1", UpdatedRecord = nsr, DateRecord = dr};

            context = context.WithNew(nrr).WithNew(nsr);

            return (nrr, context);
        }

        public static (ReferenceRecord, IContext) UpdateExisting(IContext context) {
            var rr = context.Instances<ReferenceRecord>().First();

            var updated = rr with {Name = "Test2", UpdatedRecord = rr.UpdatedRecord, DateRecord = rr.DateRecord};

            context = context.WithUpdated(rr, updated);

            return (updated, context);
        }

        public static (ReferenceRecord, IContext) UpdateExistingAndReference(IContext context) {
            var rr = context.Instances<ReferenceRecord>().First();
            var ur = context.Instances<UpdatedRecord>().First();
            var nur = ur with {Name = "Jill"};
            var nrr = rr with {Name = "Test3", UpdatedRecord = nur, DateRecord = rr.DateRecord};

            context = context.WithUpdated(rr, nrr).WithUpdated(ur, nur);

            return (nrr, context);
        }

        public static (ReferenceRecord, IContext) CreateNewUpdateReference(IContext context) {
            var rr = context.Instances<ReferenceRecord>().First();
            var ur = context.Instances<UpdatedRecord>().First();
            var nur = ur with {Name = "Janet"};
            var nrr = new ReferenceRecord {Name = "Test4", UpdatedRecord = nur, DateRecord = rr.DateRecord};

            var rr1 = context.Resolve(rr);

            context = context.WithNew(nrr).WithUpdated(ur, nur);
            return (nrr, context);
        }

        public static (CollectionRecord, IContext) CreateNewWithExistingCollection(IContext context) {
            var sr = context.Instances<UpdatedRecord>();
            return Helpers.DisplayAndSave(new CollectionRecord {Name = "Test1", UpdatedRecords = sr.ToList()}, context);
        }

        public static (CollectionRecord, IContext) UpdateExistingCollectionRecord(IContext context) {
            var sr = context.Instances<CollectionRecord>().First();
            return Helpers.DisplayAndUpdate(sr with {Name = "Test2", UpdatedRecords = sr.UpdatedRecords}, sr, context);
        }

        public static (CollectionRecord, IContext) UpdateExistingAndCollection(IContext context) {
            var rr = context.Instances<CollectionRecord>().ToArray().Last();
            var ur = context.Instances<UpdatedRecord>().First();
            var nur = ur with {Name = "John"};
            var nrr = rr with {Name = "Test3"};

            context = context.WithUpdated(rr, nrr).WithUpdated(ur, nur);

            return (nrr, context);
        }

        public static (CollectionRecord, IContext) UpdateExistingAndAddToCollection(IContext context) {
            var rr = context.Instances<CollectionRecord>().First();
            var ur = context.Instances<UpdatedRecord>().First();
            var nur = ur with {Name = "John"};
            var nrr = rr with {Name = "Test3", UpdatedRecords = rr.UpdatedRecords.Union(new[] {nur}).ToList()};

            context = context.WithUpdated(rr, nrr).WithUpdated(ur, nur);

            return (nrr, context);
        }
    }

    public static class CollectionMenuFunctions {
        public static IQueryable<SimpleRecord> GetQueryable(IContext context) => context.Instances<SimpleRecord>();
    }
}