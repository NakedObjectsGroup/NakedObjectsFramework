Namespace AW.Types

	Partial Public Class ProductReview
		<Hidden>
		Public Property ProductReviewID() As Integer

		<MemberOrder(1)>
		Public Property ReviewerName() As String = ""

		<MemberOrder(2)>
		Public Property ReviewDate() As DateTime

		<MemberOrder(3)>
		Public Property EmailAddress() As String = ""

		<MemberOrder(4)>
		Public Property Rating() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Comments {get;set;}
		<MemberOrder(5)>
		Public Property Comments() As String

		<Hidden>
		Public Property ProductID() As Integer

		<Hidden>
		Public Overridable Property Product() As Product

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return "*****".Substring(0, Rating)
		End Function
	End Class
End Namespace