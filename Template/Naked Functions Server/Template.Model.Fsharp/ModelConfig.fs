namespace Template.Model.Fsharp 

open System.Linq
open System
open Template.Model.FSharp.Db
open Microsoft.Extensions.Configuration

module ModelConfig = 
    type internal Marker = interface end
    let moduleType = typeof<Marker>.DeclaringType
    
    let IsStaticClass (t : Type) = t.IsAbstract && t.IsSealed

    let PublicClassesInterfacesEnums = moduleType.Assembly.GetTypes().Where(fun t -> t.IsPublic && (t.IsClass || t.IsInterface || t.IsEnum))

    let DomainTypes() = PublicClassesInterfacesEnums.Where(fun t -> t.Namespace = "Template.Model.Fsharp.Types" && not (t |> IsStaticClass)).ToArray();
        
    let TypesDefiningDomainFunctions() = PublicClassesInterfacesEnums.Where(fun t -> t.Namespace = "Template.Model.Fsharp.Functions"  &&  (t |> IsStaticClass)).ToArray();

    let MainMenus() = TypesDefiningDomainFunctions().Where(fun t -> t.FullName.Contains("MenuFunctions")).ToArray();

    let EFCoreDbContextCreator =
        fun (c : IConfiguration) -> 
            let db = new ExampleDbContext(c.GetConnectionString("ExampleCS"))
            db.Create() |> ignore
            db

