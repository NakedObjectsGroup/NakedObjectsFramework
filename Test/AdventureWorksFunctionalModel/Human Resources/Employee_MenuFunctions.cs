using static AW.Helpers;

namespace AW.Functions;

[Named("Employees")]
public static class Employee_MenuFunctions
{
    [PageSize(15)]
    public static IQueryable<Employee> AllEmployees(IContext context) => context.Instances<Employee>();

    [TableView(true, nameof(Employee.Current), nameof(Employee.JobTitle), nameof(Employee.Manager))]
    public static IQueryable<Employee> FindEmployeeByName([Optionally] string? firstName, string lastName, IContext context)
    {
        var employees = context.Instances<Employee>();
        var persons = context.Instances<Person>().Where(p => (firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                                                             p.LastName.ToUpper().StartsWith(lastName.ToUpper()));

        return from emp in employees
               from p in persons
               where emp.PersonDetails.BusinessEntityID == p.BusinessEntityID
               orderby p.LastName
               select emp;
    }

    public static Employee? FindEmployeeByNationalIDNumber(string nationalIDNumber, IContext context)
        => context.Instances<Employee>().FirstOrDefault(e => e.NationalIDNumber == nationalIDNumber);

    public static StaffSummary GenerateStaffSummary(IContext context)
    {
        var staff = context.Instances<Employee>();
        var female = staff.Count(x => x.Gender == "F");
        var male = staff.Count(x => x.Gender == "M");
        return new StaffSummary { Female = female, Male = male };
    }

    [RenderEagerly]
    [TableView(true, "GroupName")]
    public static IQueryable<Department> ListAllDepartments(IContext context) => context.Instances<Department>();

    internal static Employee? CurrentUserAsEmployee(IContext context)
    {
        var login = context.CurrentUser().Identity?.Name;
        return context.Instances<Employee>().FirstOrDefault(x => x.LoginID == login);
    }

    public static Employee? Me(IContext context) => CurrentUserAsEmployee(context);

    public static Employee RandomEmployee(IContext context) => Random<Employee>(context);

    ////This method is to test use of nullable booleans
    public static IQueryable<Employee> ListEmployees(
        bool? current, //mandatory
        [Optionally] bool? married,
        [DefaultValue(false)] bool? salaried,
        [Optionally][DefaultValue(true)] bool? olderThan50,
        IQueryable<Employee> employees
    )
    {
        var cv = current ?? false;
        var emps = employees.Where(e => e.Current == cv);
        if (married != null)
        {
            var value = married.Value ? "M" : "S";
            emps = emps.Where(e => e.MaritalStatus == value);
        }

        var sv = salaried ?? false;
        emps = emps.Where(e => e.Salaried == sv);
        if (olderThan50 != null)
        {
            var date = DateTime.Today.AddYears(-50); //Not an exact calculation!
            if (olderThan50.Value)
            {
                emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth < date);
            }
            else
            {
                emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth > date);
            }
        }

        return emps;
    }

    public static IQueryable<Shift> Shifts(IContext context) => context.Instances<Shift>();

    public static IQueryable<JobCandidate> JobCandidates(IContext context) => context.Instances<JobCandidate>();
}