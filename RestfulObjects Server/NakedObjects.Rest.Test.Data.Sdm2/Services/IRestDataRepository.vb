Imports sdm.systems.application.container

Namespace RestfulObjects.Test.Data
    Public Interface IRestDataRepository
        Inherits IRepository

        Function AzContributedAction(withAction As WithActionObject) As MostSimple
        Function AzContributedActionWithRefParm(withAction As WithActionObject) As MostSimple
        Function AzContributedActionWithValueParm(withAction As WithActionObject) As MostSimple
    End Interface
End Namespace
