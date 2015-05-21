Imports sdm.systems.application.collections

' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


Namespace RestfulObjects.Test.Data
    Public Class WithGetError
        Inherits AbstractBusinessObject

        Private getCount As Integer

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

#Region "AnErrorValue"

        Public Overridable ReadOnly Property AnErrorValue() As WholeNumber
            Get
                Dim tempVar As Boolean = getCount > 2
                getCount += 1
                If tempVar Then ' so no errors on startup
                    Throw New Exception("An error exception")
                End If
                Return New WholeNumber(Me, 0)
            End Get
        End Property

#End Region

#Region "AnErrorReference"

        Public Overridable Property AnErrorReference() As MostSimple
            Get
                Dim tempVar As Boolean = getCount > 4
                getCount += 1
                If tempVar Then ' so no errors on startup
                    Throw New Exception("An error exception")
                End If
                Return Nothing
            End Get
            Set(ByVal value As MostSimple)
            End Set
        End Property

#End Region

#Region "AnErrorCollection"

        Private myAnErrorCollection As New InternalCollection(GetType(MostSimple), Me)

        Public Overridable ReadOnly Property AnErrorCollection() As InternalCollection
            Get
                Dim tempVar As Boolean = getCount > 2
                getCount += 1
                If tempVar Then ' so no errors on startup
                    'Throw New Exception("An error exception")
                End If
                Return myAnErrorCollection
            End Get
        End Property

#End Region

#Region "Action: AnError"

        Public Overridable Function actionAnError() As WholeNumber
            Throw New Exception("An error exception")
        End Function

#End Region

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer) As WithGetError
            Dim obj As WithGetError =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(WithGetError)),
                        WithGetError)

            obj.Id.setValue(id)
            obj.actionSave()
            Return obj
        End Function

#End Region
    End Class
End Namespace