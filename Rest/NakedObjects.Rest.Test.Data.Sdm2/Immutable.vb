Imports sdm.systems.application.collections
Imports sdm.systems.application.control
Imports sdm.systems.application.interfaces.data


Namespace RestfulObjects.Test.Data
    'RP:  Not sure how to specify an object as immutable within nof 2 so have used the aboutfield default instead
    Public Class Immutable
        Inherits AbstractBusinessObject
        Implements IRefData

        Public Sub aboutFieldDefault(a As FieldAbout)
            a.unmodifiable()
        End Sub

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

#Region "AValue"

        Private _aValue As New WholeNumber

        Public ReadOnly Property AValue() As WholeNumber
            Get
                Return _aValue
            End Get
        End Property

#End Region

#Region "AReference"

        Private myAReference As MostSimple

        Public Overridable Property AReference() As MostSimple
            Get
                resolve(MethodBase.GetCurrentMethod)
                Return myAReference
            End Get
            Set(ByVal value As MostSimple)
                resolve(MethodBase.GetCurrentMethod)
                myAReference = value
                objectChanged()
            End Set
        End Property

#End Region

#Region "ACollection"

        Private myACollection As New InternalCollection(GetType(MostSimple), Me)

        Public Overridable ReadOnly Property ACollection() As InternalCollection
            Get
                Return myACollection
            End Get
        End Property

#End Region

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer) As Immutable
            Dim obj As Immutable =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(Immutable)),
                        Immutable)

            obj.Id.setValue(id)
            obj.actionSave()
            Return obj
        End Function

#End Region
    End Class
End Namespace