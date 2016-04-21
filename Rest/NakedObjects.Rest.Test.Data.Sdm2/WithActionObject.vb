' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


Namespace RestfulObjects.Test.Data
    Public Class WithActionObject
        Inherits WithAction

#Region "Id and Title"

        Public Overrides Function title() As Title
            Dim t As New Title
            t.append(Id)
            Return t
        End Function

        Private _id As New WholeNumber

        Public ReadOnly Property Id() As WholeNumber
            Get
                Return _id
            End Get
        End Property

#End Region

#Region "AContributedAction"

        'to make tests match nof2 to nof4 
        Public Overridable Function actionAzContributedAction() As MostSimple
            Throw New Exception("An error exception")
        End Function

        Public Overridable Function actionAzContributedActionWithRefParm() As MostSimple
            Throw New Exception("An error exception")
        End Function

        Public Overridable Function actionAzContributedActionWithValueParm() As MostSimple
            Throw New Exception("An error exception")
        End Function

#End Region

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer) As WithActionObject
            Dim wao As WithActionObject =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(WithActionObject)),
                        WithActionObject)

            wao.Id.setValue(id)
            wao.actionSave()
            Return wao
        End Function

#End Region
    End Class
End Namespace