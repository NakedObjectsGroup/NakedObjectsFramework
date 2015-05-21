' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
Imports sdm.systems.application.control

Namespace RestfulObjects.Test.Data
    Public Class WithReference
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

#Region "ANullReference"

        Private myANullReference As MostSimple

        Public Overridable Property ANullReference() As MostSimple
            Get
                resolve(MethodBase.GetCurrentMethod)
                Return myANullReference
            End Get
            Set(ByVal value As MostSimple)
                resolve(MethodBase.GetCurrentMethod)
                myANullReference = value
                objectChanged()
            End Set
        End Property

#End Region


#Region "ADisabledReference"

        Private myADisabledReference As MostSimple

        Public Overridable Property ADisabledReference() As MostSimple
            Get
                resolve(MethodBase.GetCurrentMethod)
                Return myADisabledReference
            End Get
            Set(ByVal value As MostSimple)
                resolve(MethodBase.GetCurrentMethod)
                myADisabledReference = value
                objectChanged()
            End Set
        End Property

        Public Overridable Sub aboutADisabledReference(ByVal a As FieldAbout, ByVal value As MostSimple)
            a.unmodifiable("Field not editable")
        End Sub

#End Region

#Region "AHiddenReference"

        Private myAHiddenReference As MostSimple

        Public Overridable Property AHiddenReference() As MostSimple
            Get
                resolve(MethodBase.GetCurrentMethod)
                Return myAHiddenReference
            End Get
            Set(ByVal value As MostSimple)
                resolve(MethodBase.GetCurrentMethod)
                myAHiddenReference = value
                objectChanged()
            End Set
        End Property

        Public Overridable Sub aboutAHiddenReference(ByVal a As FieldAbout, ByVal value As MostSimple)
            a.invisible()
        End Sub

#End Region

#Region "AChoicesReference"

        'RP:  Choices exist for parameters, not for properties.
        'Public Overridable Property AChoicesValue() As Integer

        Private myAChoicesReference As MostSimple

        Public Overridable Property AChoicesReference() As MostSimple
            Get
                resolve(MethodBase.GetCurrentMethod)
                Return myAChoicesReference
            End Get
            Set(ByVal value As MostSimple)
                resolve(MethodBase.GetCurrentMethod)
                myAChoicesReference = value
                objectChanged()
            End Set
        End Property

        'Public Overridable Function ChoicesAChoicesReference() As MostSimple()
        '    'Return Container.Instances(Of MostSimple)().ToArray()
        'End Function

#End Region

#Region "Set up (Fixtures)"

        Public Shared Function SetUp(id As Integer, ref As MostSimple, dis As MostSimple) As WithReference
            Dim wr As WithReference =
                    CType(
                        createTransientInstanceFromShared(
                            GetType(WithReference)),
                        WithReference)

            wr.Id.setValue(id)
            wr.AReference = ref
            wr.ADisabledReference = dis
            wr.AChoicesReference = dis
            wr.actionSave()
            Return wr
        End Function

#End Region
    End Class
End Namespace
