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
using System.Collections.Immutable;

namespace AW.Functions
{
    [Named("Persons")]
    public static class Person_MenuFunctions
    {
        public static IQueryable<Person> FindPersonsByName(
            [Optionally] string firstName, string lastName, IContext context) =>
                context.Instances<Person>().Where(p => (firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                      p.LastName.ToUpper().StartsWith(lastName.ToUpper())).OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

        public static Person RandomPerson(IContext context)
        {
            return Random<Person>(context);
        }

        //To demonstrate use of recursion to create & use multiple random numbers
        public static IList<Person> RandomPersons(int numberRequired, IContext context) =>
            RandomPersons(numberRequired, context.Instances<Person>().OrderBy(p => ""), context.RandomSeed()).ToList();

        internal static ImmutableList<Person> RandomPersons(
            int num, IOrderedQueryable<Person> source, IRandom random) =>
             num < 1 ? ImmutableList<Person>.Empty :
             ImmutableList.Create(RandomPerson(source, random)).AddRange(RandomPersons(num - 1, source, random.Next()));

        internal static Person RandomPerson(IOrderedQueryable<Person> source, IRandom random) =>
           source.Skip(random.ValueInRange(source.Count())).First();

        public static IQueryable<Password> RecentPasswords(IContext context) =>
            context.Instances<Password>().OrderByDescending(pw => pw.ModifiedDate);
    }
}