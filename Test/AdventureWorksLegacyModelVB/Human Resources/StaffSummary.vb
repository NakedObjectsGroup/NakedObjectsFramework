

Namespace AW.Types

	'<ViewModel(GetType(StaffSummary_Functions))>
	Partial Public Class StaffSummary
		'<MemberOrder(1)>
		Public Property Female() As Integer

		'<MemberOrder(2)>
		Public Property Male() As Integer

		'<MemberOrder(3)>
		Private ReadOnly Property TotalStaff() As WholeNumber
			Get
				Return New WholeNumber(Male + Female)
			End Get
		End Property

		Public Function Title() As Title
			Return New Title("Staff Summary")
		End Function
	End Class
End Namespace