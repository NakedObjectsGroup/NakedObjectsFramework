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

        internal static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));

        internal static Action<IAlert> WarnUser(string message) => ua => ua.WarnUser(message);

        internal static Action<IAlert> InformUser(string message) => ua => ua.InformUser(message);

        internal static (T, IContext) SingleObjectWarnIfNoMatch<T>(this IQueryable<T> query, IContext context) =>
            (query.FirstOrDefault(), query.Any() ? context : context.WithAction(WarnUser("There is no matching object")));

        public static (SimpleRecord, IContext) FindByNumber(string number, IContext context) {
            var id = int.Parse(number);
            return context.Instances<SimpleRecord>().Where(x => x.Id == id).SingleObjectWarnIfNoMatch(context);
        }

        public static IQueryable<SimpleRecord> FindByEnum(TestEnum eParm, IContext context) => context.Instances<SimpleRecord>();
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
    }

    public static class DefaultedMenuFunctions {
        public static SimpleRecord WithDefaults(int default1, SimpleRecord default2, IContext context) => context.Instances<SimpleRecord>().First();

        public static int Default0WithDefaults(IContext context) => 101;
        public static SimpleRecord Default1WithDefaults(IContext context) => context.Instances<SimpleRecord>().First();
    }
}