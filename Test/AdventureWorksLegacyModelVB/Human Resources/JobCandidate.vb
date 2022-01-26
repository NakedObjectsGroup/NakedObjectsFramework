Namespace AW.Types

	Partial Public Class JobCandidate

		Implements ITitledObject, INotEditableOncePersistent

		Public Property JobCandidateID() As Integer

#Region "Resume"
		Public Property mappedResume As String
		Friend myResume As TextString

		<MemberOrder(1)>
		Public ReadOnly Property Resumee As TextString
			Get
				myResume = If(myResume, New TextString(mappedResume, Sub(v) mappedResume = v))
				Return myResume
			End Get
		End Property

		Public Sub AboutResume(a As FieldAbout, resumee As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		Public Property EmployeeID() As Integer?

		Public Overridable Property Employee() As Employee

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
				Return myModifiedDate
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return "Job Candidate "
		End Function
	End Class
End Namespace