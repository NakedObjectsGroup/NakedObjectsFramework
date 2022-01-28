

Namespace AW.Types

	'<ViewModel(GetType(StaffSummary_Functions))>
	Partial Public Class StaffSummary
 Implements ITitledObject, INotEditableOncePersistent
		<AWProperty(Order:=1)>
		Public Property Female() As Integer

		<AWProperty(Order:=2)>
		Public Property Male() As Integer

		<AWProperty(Order:=3)>
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