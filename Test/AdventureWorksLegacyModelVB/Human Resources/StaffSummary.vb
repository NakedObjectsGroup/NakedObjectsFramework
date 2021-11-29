

Namespace AW.Types

	'<ViewModel(GetType(StaffSummary_Functions))>
	Partial Public Class StaffSummary
		<MemberOrder(1)>
		Public Property Female() As Integer

		<MemberOrder(2)>
		Public Property Male() As Integer

		<MemberOrder(3)>
		Public ReadOnly Property TotalStaff() As Integer
			Get
				Return Female + Male
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "Staff Summary"
		End Function
	End Class
End Namespace