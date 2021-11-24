namespace Template.Model.Fsharp.Types

open NakedFunctions

type Student() =
    [<Hidden>]
    abstract Id : int with get, set

    override val Id = 0 with get, set

    abstract member FullName : string with get, set
    override val FullName = "" with get, set
