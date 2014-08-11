NakedObjectsFramework
=====================

Framework Build Status [![Build status](https://ci.appveyor.com/api/projects/status/6kqdsbqoet1vy69n)](https://ci.appveyor.com/project/scascarini/nakedobjectsframework)

Core Tests Status [![Build status](https://ci.appveyor.com/api/projects/status/k1pui0bb8924yv8o)](https://ci.appveyor.com/project/scascarini/nakedobjectsframework-951)

RestfulObjects Test Status [![Build status](https://ci.appveyor.com/api/projects/status/jkhue6jrxwx523p1)](https://ci.appveyor.com/project/scascarini/nakedobjectsframework-716)

MVC Tests Status [![Build status](https://ci.appveyor.com/api/projects/status/152gjq86x3pbli8y)](https://ci.appveyor.com/project/scascarini/nakedobjectsframework-958)

Getting Started

Follow these steps to write your first ultra-simple Naked Objects application:
1.	Using Visual Studio 2013 (Express version is fine) create a new ASP.NET Web Application project using the MVC template.
2.	Install the NuGet Package NakedObjects.Mvc-FileTemplates, selecting Yes To All to overwrite existing files.
3.	In the Models folder add a new class Customer as follows. Note that: All properties in a Naked Objects application must be virtual, [Hidden] specifies that this property is not for display on the user interface, and [Title] specifes that the value of the property should be displayed in the Tab.

public class Customer
{   
  [Hidden]        
  public virtual int Id { get; set; } 
 
  [Title] 
   public virtual string Name { get; set; }        
}

4.	Create a DbContext object as follows.

public class MyDbContext : DbContext {
  public DbSet<Customer> Customers { get; set; }
}

5.	In the App_Start folder find the RunWeb class and edit two members as follows. First, the MenuServices property which defines the services to be shown on the main menu. (NakedObjects.Services.SimpleRepository is a ready-made class for early-stage prototyping only.)

protected override IServicesInstaller MenuServices {
    get {
        return new ServicesInstaller(
          new SimpleRepository<Customer>());
    }
}

Second, the Persistor property, in which we need to specify the DbContext(s) that it needs to inspect:

protected override IObjectPersistorInstaller Persistor {
  get {
    var installer = new EntityPersistorInstaller();
    installer.UsingCodeFirstContext(() => new MyDbContext());
    return installer;
  }
}

6.	Run the project.  Using the actions on the Customers menu, try creating and retrieving Customer objects.

