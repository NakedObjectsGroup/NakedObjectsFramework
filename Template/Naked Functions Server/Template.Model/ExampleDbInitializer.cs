﻿using System.Data.Entity;
using Template.Model.Types;

namespace Template.Model
{
    public class ExampleDbInitializer : DropCreateDatabaseAlways<ExampleDbContext>
    {
        private ExampleDbContext Context;

        protected override void Seed(ExampleDbContext context)
        {
            this.Context = context;
            AddNewStudent("Alie Algol");
            AddNewStudent("Forrest Fortran");
            AddNewStudent("James Java");
        }

        private void AddNewStudent(string name)
        {
            var st = new Student() { FullName = name };
            Context.Students.Add(st);
        }
    }
}