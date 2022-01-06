

Namespace AW.Types

	'<ViewModel(GetType(StaffSummary_Functions))>
	Partial Public Class StaffSummary
 Implements ITitledObject
		'<MemberOrder(1)>
		Public Property Female() As Integer

		'<MemberOrder(2)>
		Public Property Male() As Integer

		'<MemberOrder(3)>
		Public ReadOnly Property TotalStaff() As WholeNumber
			Get
				Return New WholeNumber(Male + Female)
			End Get
		End Property

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title("Staff Summary")
		End Function
	End Class
End Namespace