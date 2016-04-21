Imports sdm.systems.application.control
Imports sdm.systems.application.container


Namespace RestfulObjects.Test.Data
    Public Class WithActionService
        Inherits WithAction
        Implements IWithActionRepository

        Public Overrides Function title() As Title
            Dim t As New Title
            t.append("With Action Service")
            Return t
        End Function

        Public Function AwareType(ByVal accessibleType As Type) As Type _
            Implements IAccessibleThroughAware.AwareType
            Return GetType(IWithActionRepositoryAware)
        End Function

        Public Overrides Sub aboutActionSave(ByVal a As ActionAbout)

            a.invisible()
        End Sub
    End Class
End Namespace
