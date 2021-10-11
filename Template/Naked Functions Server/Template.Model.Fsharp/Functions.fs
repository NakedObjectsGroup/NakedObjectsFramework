namespace Template.Model.Fsharp.Functions

open NakedFunctions
open System.Linq
open Template.Model.Fsharp.Types

module Student_MenuFunctions =

    let CreateNewStudent fullName (context: IContext) =
        let s = new Student(Id = 0, FullName = fullName)
        (s, context.WithNew(s))

    let AllStudents (context: IContext) = context.Instances<Student>()

    let FindStudentByName (name: string) (context: IContext) =
        context
            .Instances<Student>()
            .Where(fun c -> c.FullName.ToUpper().Contains(name.ToUpper()))

    let FindStudentById (id: int) (context: IContext) =
        context
            .Instances<Student>()
            .FirstOrDefault(fun c -> c.Id = id)
