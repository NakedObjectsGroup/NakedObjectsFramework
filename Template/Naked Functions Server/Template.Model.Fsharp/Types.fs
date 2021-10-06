namespace Template.Model.Fsharp.Types

open NakedFunctions

    //[<CLIMutable>]
    //type Student = {
    //    [<Hidden>]
    //    Id : int
    //    FullName : string
    //}

    type Student() = 
        [<Hidden>]
        abstract member Id: int with get, set
        default val Id = 0 with get, set

        abstract member FullName: string with get, set
        default val FullName = "" with get, set
