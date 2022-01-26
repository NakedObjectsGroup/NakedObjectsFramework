Namespace AW.Types

    Partial Public Class ProductCostHistory

        Implements ITitledObject, INotEditableOncePersistent

        Public Property ProductID() As Integer

#Region "StartDate"
        Public Property mappedStartDate As Date
        Friend myStartDate As NODate

        <MemberOrder(1)>
        Public ReadOnly Property StartDate As NODate
            Get
                myStartDate = If(myStartDate, New NODate(mappedStartDate, Sub(v) mappedStartDate = v))
Return myStartDate
            End Get
        End Property

        Public Sub AboutStartDate(a As FieldAbout, StartDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

#Region "EndDate"
        Public Property mappedEndDate As Date?
        Friend myEndDate As NODateNullable

        <MemberOrder(1)>
        Public ReadOnly Property EndDate As NODateNullable
            Get
                myEndDate = If(myEndDate, New NODateNullable(mappedEndDate, Sub(v) mappedEndDate = v))
Return myEndDate
            End Get
        End Property

        Public Sub AboutEndDate(a As FieldAbout, EndDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "StandardCost"
        Public Property mappedStandardCost As Decimal
        Friend myStandardCost As Money

        <MemberOrder(1)>
        Public ReadOnly Property StandardCost As Money
            Get
                myStandardCost = If(myStandardCost, New Money(mappedStandardCost, Sub(v) mappedStandardCost = v))
Return myStandardCost
            End Get
        End Property

        Public Sub AboutStandardCost(a As FieldAbout, StandardCost As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region


        Public Overridable Property Product() As Product

        Public Sub AboutProduct(a As FieldAbout, p As Product)
            Select Case a.TypeCode
                Case Else
                    a.Visible = False
            End Select
        End Sub

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

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"{StandardCost} {StartDate}~"
        End Function
    End Class
End Namespace