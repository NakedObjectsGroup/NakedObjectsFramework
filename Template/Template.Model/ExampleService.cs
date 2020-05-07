using NakedObjects;
using System.Linq;


namespace Template.Model
{
    //This example service acts as both a 'repository' (with methods for
    //retrieving objects from the database) and as a 'factory' i.e. providing
    //one or more methods for creating new object(s) from scratch.
    public class ExampleService
    {
        #region Injected Services
        //An implementation of this interface is injected automatically by the framework
        public IDomainObjectContainer Container { set; protected get; }
        #endregion
        public Student CreateNewStudent()
        {
            //'Transient' means 'unsaved' -  returned to the user
            //for fields to be filled-in and the object saved.
            return Container.NewTransientInstance<Student>();
        }

        public IQueryable<Student> AllStudents()
        {
            //The 'Container' masks all the complexities of 
            //dealing with the database directly.
            return Container.Instances<Student>();
        }

        public IQueryable<Student> FindStudentByName(string name)
        {
            //Filters students to find a match
            return AllStudents().Where(c => c.FullName.ToUpper().Contains(name.ToUpper()));
        }
    }

}
