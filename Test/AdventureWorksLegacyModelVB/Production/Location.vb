Namespace AW.Types


    Partial Public Class Location
        Implements ITitledObject, IBounded

        '<Hidden>
        Public Property LocationID() As Short

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

        Public Sub AboutName(a As IFieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "CostRate"
        Public Property mappedCostRate As Decimal
        Friend myCostRate As Money

        <MemberOrder(1)>
        Public ReadOnly Property CostRate As Money
            Get
                myCostRate = If(myCostRate, New Money(mappedCostRate, Sub(v) mappedCostRate = v))
Return myCostRate
            End Get
        End Property

        Public Sub AboutCostRate(a As IFieldAbout, CostRate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Availability"
        Public Property mappedAvailability As Decimal
        Friend myAvailability As Money

        <MemberOrder(1)>
        Public ReadOnly Property Availability As Money
            Get
                myAvailability = If(myAvailability, New Money(mappedAvailability, Sub(v) mappedAvailability = v))
Return myAvailability
            End Get
        End Property

        Public Sub AboutAvailability(a As IFieldAbout, Availability As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
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

        Public Sub AboutModifiedDate(a As IFieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedName
        End Function
    End Class
End Namespace