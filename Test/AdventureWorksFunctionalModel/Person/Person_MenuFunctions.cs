// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using AW.Types;

using static AW.Helpers;
using System;
using System.Collections.Immutable;

namespace AW.Functions {
    [Named("Persons")]
    public static class Person_MenuFunctions {

        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IQueryable<Person> FindContactByName(
            [Optionally] string firstName, string lastName, IContext context) =>
                context.Instances<Person>().Where(p => (firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                      p.LastName.ToUpper().StartsWith(lastName.ToUpper())).OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

        public static Person RandomContact(IContext context) {
            return Random<Person>(context);
        }

        [TableView(true, nameof(Person.AdditionalContactInfo))]
        public static IList<Person> RandomContacts(IContext context) {
            var instances = context.Instances<Person>().OrderBy(n => "");
            IRandom random1 = context.GetService<IRandomSeedGenerator>().Random;
            IRandom random2 = random1.Next();
            Person p1 = instances.Skip(random1.ValueInRange(instances.Count())).FirstOrDefault();
            Person p2 = instances.Skip(random2.ValueInRange(instances.Count())).FirstOrDefault();
            return new[] { p1, p2 };
        }

        //To demonstrate use of recursion to create & use multiple random numbers
        public static IList<Person> RandomPersons(int numberRequired, IContext context) =>
            RandomPersons(numberRequired, context.Instances<Person>().OrderBy(p => ""), context.RandomSeed()).ToList();

        //Test if an immutablelist can be returned
        internal static ImmutableList<Person> RandomPersons(
            int num, IOrderedQueryable<Person> source, IRandom random) =>
             num < 1 ? ImmutableList<Person>.Empty:
             ImmutableList.Create(RandomPerson(source, random)).AddRange(RandomPersons(num - 1, source, random.Next()));

    internal static Person RandomPerson(IOrderedQueryable<Person> source, IRandom random) =>
       source.Skip(random.ValueInRange(source.Count())).First();

    }
}