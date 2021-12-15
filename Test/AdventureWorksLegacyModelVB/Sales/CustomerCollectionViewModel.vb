

Namespace AW.Types

	'TODO
	'<ViewModel(GetType(CustomerCollectionViewModel_Functions))>
	Partial Public Class CustomerCollectionViewModel
		Public Sub New(ByVal customers As IList(Of Customer))
			Me.Customers = customers
		End Sub

		<Hidden>
		Public Property Customers() As IList(Of Customer)
	End Class
End Namespace