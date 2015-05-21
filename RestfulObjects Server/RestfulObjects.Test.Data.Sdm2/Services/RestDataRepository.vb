Imports System.Linq
Imports sdm.systems.application.container


Namespace RestfulObjects.Test.Data
    Public Class RestDataRepository
        Inherits SdmObject
        Implements IAccessibleThroughAware
        Implements IRestDataRepository

        Public Overrides Function title() As Title
            Dim t As New Title
            t.append("Rest Data Repository")
            Return t
        End Function

        Public Function actionAzContributedAction(withAction As WithActionObject) As MostSimple _
            Implements IRestDataRepository.AzContributedAction
            Return Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().First()
        End Function

        Public Function actionAzContributedActionOnBaseClass(withAction As WithAction) As MostSimple
            Return Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().First()
        End Function

        Public Function actionAzContributedActionWithRefParm(withAction As WithActionObject) As MostSimple _
            Implements IRestDataRepository.AzContributedActionWithRefParm
            Return Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().First()
        End Function

        Public Function actionAzContributedActionWithValueParm(withAction As WithActionObject) As MostSimple _
            Implements IRestDataRepository.AzContributedActionWithValueParm
            Return Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().First()
        End Function


        Public Function actionCreateTransientMostSimple() As MostSimple
            Return Container.createTransientInstance(GetType(MostSimple))
        End Function

        Public Function actionCreateTransientWithValue() As WithValue
            'Return Container.createTransientInstance(GetType(WithValue))

            Dim withValue As WithValue
            withValue = Container.createTransientInstance(GetType(WithValue))

            withValue.AValue.setValue(102)
            withValue.AChoicesValue.setValue(3)
            withValue.ADisabledValue.setValue(103)
            withValue.AStringValue.setValue("one hundred four")
            withValue.AHiddenValue.setValue(105)

            Return withValue
        End Function

        Public Function actionCreateTransientWithReference() As WithReference
            Dim wr As WithReference
            wr = Container.createTransientInstance(GetType(WithReference))

            wr.AReference =
                Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().Single(
                    Function(ms) ms.Id.longValue() = 1)
            wr.AChoicesReference =
                Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().Single(
                    Function(ms) ms.Id.longValue() = 1)
            wr.ADisabledReference =
                Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().Single(
                    Function(ms) ms.Id.longValue() = 1)
            wr.AHiddenReference =
                Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)().Single(
                    Function(ms) ms.Id.longValue() = 1)

            Return wr
        End Function

        Public Function actionCreateTransientWithCollection() As WithCollection
            Return Container.createTransientInstance(GetType(WithCollection))
        End Function


        Public Function AwareType(ByVal accessibleType As Type) As Type _
            Implements IAccessibleThroughAware.AwareType
            Return GetType(IWithActionRepositoryAware)
        End Function
    End Class
End Namespace
