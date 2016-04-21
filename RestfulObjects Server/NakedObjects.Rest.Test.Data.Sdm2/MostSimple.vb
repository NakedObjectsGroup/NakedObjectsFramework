' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


Namespace RestfulObjects.Test.Data
    Public Class MostSimple
        Inherits AbstractBusinessObject

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

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer) As MostSimple
            Dim ms As MostSimple =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(MostSimple)),
                        MostSimple)

            ms.Id.setValue(id)
            ms.actionSave()
            Return ms
        End Function

#End Region

        'Public Function actionGetWithValue() As WithValue

        '    Return Container.allInstances(GetType(WithValue)).Cast(Of WithValue)().First()
        'End Function
    End Class
End Namespace
