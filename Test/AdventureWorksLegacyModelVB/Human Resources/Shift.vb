Namespace AW.Types

	<Bounded>
	Partial Public Class Shift
		Implements IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As Shift)
			ShiftID = cloneFrom.ShiftID
			Name = cloneFrom.Name
			StartTime = cloneFrom.StartTime
			EndTime = cloneFrom.EndTime
			ModifiedDate = cloneFrom.ModifiedDate
		End Sub

		<Hidden>
		Public Property ShiftID() As Byte

		<MemberOrder(1)>
		Public Property Name() As String = ""

		<MemberOrder(3), Mask("T")>
		Public Property StartTime() As TimeSpan

		<MemberOrder(4), Mask("T")>
		Public Property EndTime() As TimeSpan


		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace