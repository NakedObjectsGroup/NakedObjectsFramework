Namespace AW.Types

	'<Bounded>
	Partial Public Class ShipMethod

		Public Property ShipMethodID() As Integer

#Region "Name"
        Friend mappedName As String
        Friend myName As TextString

        '<MemberOrder(1)>
        Public ReadOnly Property Name As TextString
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
        Friend mappedShipBase As Decimal
        Friend myShipBase As Money

        '<MemberOrder(2)>
        Public ReadOnly Property ShipBase As Money
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
        Friend mappedShipRate As Decimal
        Friend myShipRate As Money

        '<MemberOrder(3)>
        Public ReadOnly Property ShipRate As Money
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
        Friend mappedModifiedDate As Date
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

        Public Property rowguid() As Guid

        Public Function Title() As Title
            Return New Title(Name)
        End Function
    End Class
End Namespace