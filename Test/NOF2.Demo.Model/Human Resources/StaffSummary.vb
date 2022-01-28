

Namespace AW.Types

	'<ViewModel(GetType(StaffSummary_Functions))>
	Partial Public Class StaffSummary
 Implements ITitledObject, INotEditableOncePersistent
		<DemoProperty(Order:=1)>
		Public Property Female() As Integer

		<DemoProperty(Order:=2)>
		Public Property Male() As Integer

		<DemoProperty(Order:=3)>
		Public ReadOnly Property TotalStaff() As WholeNumber
			Get
				Return New WholeNumber(Male + Female)
			End Get
		End Property

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return "Staff Summary"
		End Function
	End Class
End Namespace