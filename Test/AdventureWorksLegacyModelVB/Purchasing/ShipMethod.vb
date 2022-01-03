Namespace AW.Types

	'<Bounded>
	Partial Public Class ShipMethod

		Public Property ShipMethodID() As Integer

#Region "Name"
        Public mappedName As String
        Friend myName As TextString

        '<MemberOrder(1)>
        Private ReadOnly Property Name As TextString
            Get
                Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
            End Get
        End Property

        Public Sub AboutName(a As FieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ShipBase"
        Public mappedShipBase As Decimal
        Friend myShipBase As Money

        '<MemberOrder(2)>
        Private ReadOnly Property ShipBase As Money
            Get
                Return If(myShipBase, New Money(mappedShipBase, Function(v) mappedShipBase = v))
            End Get
        End Property

        Public Sub AboutShipBase(a As FieldAbout, ShipBase As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ShipRate"
        Public mappedShipRate As Decimal
        Friend myShipRate As Money

        '<MemberOrder(3)>
        Private ReadOnly Property ShipRate As Money
            Get
                Return If(myShipRate, New Money(mappedShipRate, Function(v) mappedShipRate = v))
            End Get
        End Property

        Public Sub AboutShipRate(a As FieldAbout, ShipRate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ModifiedDate"
        Public mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        '<MemberOrder(99)>
        Private ReadOnly Property ModifiedDate As TimeStamp
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

        Public Function Title() As Title
            Return New Title(Name)
        End Function
    End Class
End Namespace