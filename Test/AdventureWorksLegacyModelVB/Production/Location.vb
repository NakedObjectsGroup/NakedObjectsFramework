Namespace AW.Types

	<Bounded, PresentationHint("Topaz")>
	Partial Public Class Location
		<Hidden>
		Public Property LocationID() As Short

#Region "Name"
        Friend mappedName As String
        Friend myName As TextString

        <MemberOrder(1)>
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

#Region "CostRate"
        Friend mappedCostRate As Decimal
        Friend myCostRate As Money

        <MemberOrder(1)>
        Public ReadOnly Property CostRate As Money
            Get
                Return If(myCostRate, New Money(mappedCostRate, Function(v) mappedCostRate = v))
            End Get
        End Property

        Public Sub AboutCostRate(a As FieldAbout, CostRate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Availability"
        Friend mappedAvailability As Decimal
        Friend myAvailability As Money

        <MemberOrder(1)>
        Public ReadOnly Property Availability As Money
            Get
                Return If(myAvailability, New Money(mappedAvailability, Function(v) mappedAvailability = v))
            End Get
        End Property

        Public Sub AboutAvailability(a As FieldAbout, Availability As Money)
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

        <MemberOrder(99)>
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

        Public Function Title() As Title
            Return New Title(Name)
        End Function
    End Class
End Namespace