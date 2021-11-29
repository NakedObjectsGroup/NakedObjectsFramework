Namespace AW.Types

	Partial Public Class Illustration
		Public Property IllustrationID() As Integer
		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Diagram {get;set;}
		Public Property Diagram() As String

		Public Overridable Property ProductModelIllustration() As ICollection(Of ProductModelIllustration) = New List(Of ProductModelIllustration)()

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"Illustration: {IllustrationID}"
		End Function
	End Class
End Namespace