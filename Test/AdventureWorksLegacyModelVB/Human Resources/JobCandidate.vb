Namespace AW.Types

	Partial Public Class JobCandidate
		<Hidden>
		Public Property JobCandidateID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? @Resume {get;set;}
		Public Property [Resume]() As String

		<Hidden>
		Public Property EmployeeID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Employee? Employee {get;set;}
		Public Overridable Property Employee() As Employee

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return "Job Candidate "
		End Function
	End Class
End Namespace