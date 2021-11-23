






using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


using static AW.Helpers;

namespace AW.Functions;
[Named("Persons")]
public static class Person_MenuFunctions
{
    public static IQueryable<Person> FindPersonsByName(
        [Optionally] string? firstName, string lastName, IContext context) =>
        context.Instances<Person>().Where(p => (firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                                               p.LastName.ToUpper().StartsWith(lastName.ToUpper())).OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

    public static Person RandomPerson(IContext context) => Random<Person>(context);

    //To demonstrate use of recursion to create & use multiple random numbers
    public static IList<Person> RandomPersons(int numberRequired, IContext context) =>
        RandomPersons(numberRequired, context.Instances<Person>().OrderBy(p => ""), context.RandomSeed()).ToList();

    internal static ImmutableList<Person> RandomPersons(
        int num, IOrderedQueryable<Person> source, IRandom random) =>
        num < 1 ? ImmutableList<Person>.Empty : ImmutableList.Create(RandomPerson(source, random)).AddRange(RandomPersons(num - 1, source, random.Next()));

    internal static Person RandomPerson(IOrderedQueryable<Person> source, IRandom random) =>
        source.Skip(random.ValueInRange(source.Count())).First();
}