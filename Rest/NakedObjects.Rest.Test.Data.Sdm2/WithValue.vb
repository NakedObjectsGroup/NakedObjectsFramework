' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
Imports sdm.systems.application.control

Namespace RestfulObjects.Test.Data
    Public Class WithValue
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

#Region "AValue"

        Private _aValue As New WholeNumber

        Public Property AValue() As WholeNumber
            Get
                Return _aValue
            End Get
            Set(value As WholeNumber)
                _aValue = value
            End Set
        End Property

#End Region

#Region "AStringValue"

        Private _aStringValue As New TextString

        Public Property AStringValue() As TextString
            Get
                Return _aStringValue
            End Get
            Set(value As TextString)
                _aStringValue = value
            End Set
        End Property


#End Region

#Region "ADateTimeValue"

        Private _aDateTimeValue As New NoDate(Me, 2012, 2, 10)

        Public Property ADateTimeValue() As NoDate
            Get
                Return _aDateTimeValue
            End Get
            Set(value As NoDate)
                _aDateTimeValue = value
            End Set
        End Property


#End Region


#Region "ADisabledValue"

        Private _aDisabledValue As New WholeNumber

        Public ReadOnly Property ADisabledValue() As WholeNumber
            Get
                Return _aDisabledValue
            End Get
        End Property

        Public Overridable Sub aboutADisabledValue(ByVal a As FieldAbout, ByVal value As WholeNumber)
            a.unmodifiable("Field not editable")
        End Sub

#End Region

#Region "AHiddenValue"

        Private _aHiddenValue As New WholeNumber

        Public ReadOnly Property AHiddenValue() As WholeNumber
            Get
                Return _aHiddenValue
            End Get
        End Property

        Public Overridable Sub aboutAHiddenValue(ByVal a As FieldAbout, ByVal value As WholeNumber)
            a.invisible()
        End Sub

#End Region

#Region "AChoicesValue"

        Private _aChoicesValue As New WholeNumber

        Public ReadOnly Property AChoicesValue() As WholeNumber
            Get
                Return _aChoicesValue
            End Get
        End Property

        Public Overridable Sub aboutAChoicesValue(ByVal a As FieldAbout, ByVal value As WholeNumber)
            a.cannotBeEmpty(value)
        End Sub

        'RP:  Choices exist for parameters, not for properties.
        'Public Overridable Function ChoicesAChoicesValue() As Integer()
        '    Return {1, 2, 3}
        'End Function

#End Region

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer, val As Integer, dis As Integer) As WithValue
            Dim wv As WithValue =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(WithValue)),
                        WithValue)


            wv.Id.setValue(id)
            wv.AValue.setValue(val)
            wv.ADisabledValue.setValue(dis)
            wv.actionSave()
            Return wv
        End Function

#End Region
    End Class
End Namespace