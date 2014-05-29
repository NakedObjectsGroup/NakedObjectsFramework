using System.Data.Entity;

namespace $rootnamespace$
{
    public class $safeitemname$ : DropCreateDatabaseIfModelChanges<MyDbContext> {
       
        protected override void Seed(MyDbContext context){           
            //var f1 = NewFoo("Foo 1", context);
        }

        //private Foo NewFoo(string name, MyDbContext context)
        //{
        //    Foo foo = new Foo() {Name = name};
        //    context.Foos.Add(foo);
        //    return foo;
        //}
    }
}