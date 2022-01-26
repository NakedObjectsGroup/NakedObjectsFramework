

Imports AW.Services

Namespace AW.Types

    Partial Public Class Store
        Inherits BusinessEntity
        Implements ITitledObject, INotEditableOncePersistent
#Region "Name"
        Public Property mappedName As String
        Friend myName As TextString

        <MemberOrder(20)>
        Public ReadOnly Property Name As TextString
            Get
                myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
Return myName
            End Get
        End Property

        Public Sub AboutName(a As FieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Store Name"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        ''<Hidden>
        Public Property Demographics() As String

        'TODO: <MultiLine(10)>
        <MemberOrder(30)>
        Public ReadOnly Property FormattedDemographics() As TextString
            Get
                Return New TextString("TODO") 'TODO Utilities.FormatXML(Demographics)
            End Get
        End Property

        Public Sub AboutFormattedDemographics(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Demographics"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub

        ''<Hidden>
        Public Property SalesPersonID() As Integer?

        <MemberOrder(40)>
        Public Overridable Property SalesPerson() As SalesPerson

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
Return myModifiedDate
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        ''<Hidden>
        Public Property RowGuid() As Guid

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedName
        End Function


    End Class
End Namespace