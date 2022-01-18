Namespace AW.Types


    Partial Public Class ShipMethod
        Implements ITitledObject, IBounded

        Public Property ShipMethodID() As Integer

#Region "Name"
        Public Property mappedName As String
        Friend myName As TextString

        <MemberOrder(1)>
        Public ReadOnly Property Name As TextString
            Get
                myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
Return myName
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
        Public Property mappedShipBase As Decimal
        Friend myShipBase As Money

        <MemberOrder(2)>
        Public ReadOnly Property ShipBase As Money
            Get
                myShipBase = If(myShipBase, New Money(mappedShipBase, Sub(v) mappedShipBase = v))
Return myShipBase
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
        Public Property mappedShipRate As Decimal
        Friend myShipRate As Money

        <MemberOrder(3)>
        Public ReadOnly Property ShipRate As Money
            Get
                myShipRate = If(myShipRate, New Money(mappedShipRate, Sub(v) mappedShipRate = v))
Return myShipRate
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

        Public Property RowGuid() As Guid

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedName
        End Function
    End Class
End Namespace