namespace Template.Model.FSharp

open Microsoft.EntityFrameworkCore
open Template.Model.Fsharp.Types

module Db =

    type ExampleDbContext(cs) =
        inherit DbContext()

        member this.cs: string = cs

        [<DefaultValue>]
        val mutable _Students: DbSet<Student>

        member this.Students
            with public get () = this._Students
            and public set value = this._Students <- value

        override this.OnConfiguring(optionsBuilder: DbContextOptionsBuilder) =
            //optionsBuilder.LogTo(l => Debug.WriteLine(l));
            optionsBuilder.UseSqlServer(this.cs) |> ignore
            optionsBuilder.UseLazyLoadingProxies() |> ignore
            ()

        override this.OnModelCreating(modelBuilder: ModelBuilder) =
            modelBuilder
                .Entity<Student>()
                .HasData(new Student(Id = 1, FullName = "Alie Algol"))
            |> ignore

            modelBuilder
                .Entity<Student>()
                .HasData(new Student(Id = 2, FullName = "Forrest Fortran"))
            |> ignore

            modelBuilder
                .Entity<Student>()
                .HasData(new Student(Id = 3, FullName = "James Java"))
            |> ignore

            ()

        member this.Delete() = this.Database.EnsureDeleted()

        member this.Create() = this.Database.EnsureCreated()
