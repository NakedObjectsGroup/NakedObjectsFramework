Imports sdm.systems.application.collections

' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

Namespace RestfulObjects.Test.Data
    Public Class WithError
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

#Region "AnErrorValue"

        'Public Overridable Property AnErrorValue() As Integer
        '    Get
        '        Return 0
        '    End Get
        '    Set(ByVal value As Integer)
        '        Throw New DomainException("An error exception")
        '    End Set
        'End Property

        'RP: Can't minic the behaviour above.  Could throw error on Get?
        Public ReadOnly Property AnErrorValue() As WholeNumber
            Get
                Throw New Exception("An error exception")
            End Get
        End Property

#End Region

#Region "AnErrorReference"

        Public Overridable Property AnErrorReference() As MostSimple
            Get
                Return Nothing
            End Get
            Set(ByVal value As MostSimple)
                Throw New Exception("An error exception")
            End Set
        End Property

#End Region

#Region "ACollection"

        Private myACollection As New InternalCollection(GetType(MostSimple), Me)

        'RP:  Not sure why this collection is here  - doesn't throw an error
        Public Overridable ReadOnly Property ACollection() As InternalCollection
            Get
                Return myACollection
            End Get
        End Property

#End Region

#Region "action: AnError"

        Public Overridable Function actionAnError() As WholeNumber
            Throw New Exception("An error exception")
        End Function

#End Region

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer) As WithError
            Dim obj As WithError =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(WithError)),
                        WithError)

            obj.Id.setValue(id)
            obj.actionSave()
            Return obj
        End Function

#End Region
    End Class
End Namespace