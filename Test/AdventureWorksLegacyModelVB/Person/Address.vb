Namespace AW.Types
    Partial Public Class Address

        Implements ITitledObject


        Public Property AddressID() As Integer

#Region "City"
        Public mappedCity As String
        Friend myCity As TextString

        '<MemberOrder(13)>
        Public ReadOnly Property City As TextString
            Get
                Return If(myCity, New TextString(mappedCity, Function(v) mappedCity = v))
            End Get
        End Property

        Public Sub AboutCity(a As FieldAbout, City As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "PostalCode"
        Public mappedPostalCode As String
        Friend myPostalCode As TextString

        '<MemberOrder(14)>
        Public ReadOnly Property PostalCode As TextString
            Get
                Return If(myPostalCode, New TextString(mappedPostalCode, Function(v) mappedPostalCode = v))
            End Get
        End Property

        Public Sub AboutPostalCode(a As FieldAbout, PostalCode As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "AddressLine1"
        Public mappedAddressLine1 As String
        Friend myAddressLine1 As TextString

        '<MemberOrder(11)>
        Public ReadOnly Property AddressLine1 As TextString
            Get
                Return If(myAddressLine1, New TextString(mappedAddressLine1, Function(v) mappedAddressLine1 = v))
            End Get
        End Property

        Public Sub AboutAddressLine1(a As FieldAbout, AddressLine1 As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "AddressLine2"
        Public mappedAddressLine2 As String
        Friend myAddressLine2 As TextString

        '<MemberOrder(12)>
        Public ReadOnly Property AddressLine2 As TextString
            Get
                Return If(myAddressLine2, New TextString(mappedAddressLine2, Function(v) mappedAddressLine2 = v))
            End Get
        End Property

        Public Sub AboutAddressLine2(a As FieldAbout, AddressLine2 As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region


        Public Property StateProvinceID() As Integer

        '<MemberOrder(15)>
        Public Overridable Property StateProvince() As StateProvince

#Region "ModifiedDate"
        Public mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        '<MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout, ModifiedDate As TimeStamp)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        Public Property RowGuid() As Guid

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"{AddressLine1}..."
        End Function

    End Class
End Namespace