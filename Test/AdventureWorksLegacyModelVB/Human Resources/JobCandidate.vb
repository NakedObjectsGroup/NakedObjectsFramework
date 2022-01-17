Namespace AW.Types

	Partial Public Class JobCandidate
 Implements ITitledObject

		Public Property JobCandidateID() As Integer

#Region "Resume"
		Public mappedResume As String
		Friend myResume As TextString

		<MemberOrder(1)>
		Public ReadOnly Property Resumee As TextString
			Get
				Return If(myResume, New TextString(mappedResume, Function(v) mappedResume = v))
			End Get
		End Property

		Public Sub AboutResume(a As FieldAbout, resumee As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Resumé"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Property EmployeeID() As Integer?

		Public Overridable Property Employee() As Employee

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return "Job Candidate "
		End Function
	End Class
End Namespace