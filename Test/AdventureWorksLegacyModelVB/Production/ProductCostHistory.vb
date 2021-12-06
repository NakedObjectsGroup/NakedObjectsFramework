Namespace AW.Types

	Partial Public Class ProductCostHistory
		

		<Hidden>
		Public Property ProductID() As Integer

		Public Property StartDate() As DateTime
		Public Property EndDate() As DateTime?
		Public Property StandardCost() As Decimal

		<Hidden>
		Public Overridable Property Product() As Product

#Region "ModifiedDate"
        Friend mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(1)>
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

        Public Overrides Function ToString() As String
			Return $"{StandardCost} {StartDate}~"
		End Function
	End Class
End Namespace