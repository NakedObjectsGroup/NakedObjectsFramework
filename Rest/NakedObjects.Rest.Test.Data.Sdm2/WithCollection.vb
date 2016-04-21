Imports sdm.systems.application.control
Imports sdm.systems.application.collections

' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


Namespace RestfulObjects.Test.Data
    Public Class WithCollection
        Inherits AbstractBusinessObject

        Private Sub InitCollectionAsNecessary(coll As InternalCollection)

            'If coll.isEmpty() Then

            '    Dim o1 = Container.allInstances(GetType(MostSimple)).Cast(Of MostSimple)().Single(Function(ms) ms.Id.getValue() = 1)
            '    Dim o2 = Container.allInstances(GetType(MostSimple)).Cast(Of MostSimple)().Single(Function(ms) ms.Id.getValue() = 1)

            '    coll.add(o1)
            '    coll.add(o2)


            'End If
        End Sub


#Region "LifeCycle methods"

        'Public Overrides Sub created()
        '    MyBase.created()
        'End Sub

#End Region

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

#Region "ACollection"

        Private myACollection As New InternalCollection(GetType(MostSimple), Me)

        Public Overridable ReadOnly Property ACollection() As InternalCollection
            Get
                InitCollectionAsNecessary(myACollection)
                Return myACollection
            End Get
        End Property

        Public Sub aboutACollection(a As FieldAbout, value As MostSimple, beingAdded As Boolean)

        End Sub

#End Region


#Region "ADisabledCollection"

        Private myADisabledCollection As New InternalCollection(GetType(MostSimple), Me)

        'This has been included for test compatibility -  all collections are disabled in SDM 2
        Public Overridable ReadOnly Property ADisabledCollection() As InternalCollection
            Get
                InitCollectionAsNecessary(myADisabledCollection)
                Return myADisabledCollection
            End Get
        End Property

        Public Sub aboutADisabledCollection(a As FieldAbout, value As MostSimple, beingAdded As Boolean)
            a.unmodifiable()
        End Sub

#End Region

#Region "AHiddenCollection"

        Private myAHiddenCollection As New InternalCollection(GetType(MostSimple), Me)

        'This has been included for test compatibility -  no apparent way to hide a collection
        Public Overridable ReadOnly Property AHiddenCollection() As InternalCollection
            Get
                InitCollectionAsNecessary(myAHiddenCollection)
                Return myAHiddenCollection
            End Get
        End Property

        Public Sub aboutAHiddenCollection(a As FieldAbout, value As MostSimple, beingAdded As Boolean)
            a.invisible()
        End Sub

#End Region

#Region "AnEmptyCollection"

        Private myEmptyCollection As InternalCollection = createInternalCollection(GetType(MostSimple))

        Public Overridable ReadOnly Property AnEmptyCollection() As InternalCollection
            Get
                Return myEmptyCollection
            End Get
        End Property

        Public Sub aboutAnEmptyCollection(a As FieldAbout, value As MostSimple, beingAdded As Boolean)

        End Sub

#End Region

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer) As WithCollection
            Dim wc As WithCollection =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(WithCollection)),
                        WithCollection)

            wc.Id.setValue(id)
            wc.actionSave()
            Return wc
        End Function

#End Region
    End Class
End Namespace
