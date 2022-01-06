

Namespace AW.Types

	'TODO
	'<ViewModel(GetType(CustomerCollectionViewModel_Functions))>
	Partial Public Class CustomerCollectionViewModel
		Implements ITitledObject

		Public Sub New(ByVal customers As IList(Of Customer))
			Me.Customers = customers
		End Sub

		'<Hidden>
		Public Property Customers() As IList(Of Customer)

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{Customers().Count} Customers"
		End Function


	End Class
End Namespace