﻿

Namespace AW.Types

	'TODO
	'<ViewModel(GetType(CustomerDashboard_Functions))>
	Partial Public Class CustomerDashboard
		Implements ITitledObject, INotEditableOncePersistent

		'<Hidden>
		Public Overridable Property Root() As Customer

		Public ReadOnly Property Name() As String
			Get
				Return Nothing '$"{(If(Root.IsIndividual(), Root.Person, Root.Store?.Name))}"
			End Get
		End Property

		'Empty field, not - to test that fields are not editable in a VM
		Public Property Comments() As String = ""

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{Name} - Dashboard"
		End Function
	End Class
End Namespace