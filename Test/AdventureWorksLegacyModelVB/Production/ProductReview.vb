Namespace AW.Types

	Partial Public Class ProductReview
 Implements ITitledObject

		Public Property ProductReviewID() As Integer

#Region "ReviewerName"
		Public Property mappedReviewerName As String
		Friend myReviewerName As TextString

		<MemberOrder(1)>
		Public ReadOnly Property ReviewerName As TextString
			Get
				Return If(myReviewerName, New TextString(mappedReviewerName, Sub(v) mappedReviewerName = v))
			End Get
		End Property

		Public Sub AboutReviewerName(a As FieldAbout, ReviewerName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ReviewDate"
		Public Property mappedReviewDate As Date
		Friend myReviewDate As NODate

		<MemberOrder(2)>
		Public ReadOnly Property ReviewDate As NODate
			Get
				Return If(myReviewDate, New NODate(mappedReviewDate, Sub(v) mappedReviewDate = v))
			End Get
		End Property

		Public Sub AboutReviewDate(a As FieldAbout, ReviewDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "EmailAddress"
		Public Property mappedEmailAddress As String
		Friend myEmailAddress As TextString

		<MemberOrder(3)>
		Public ReadOnly Property EmailAddress As TextString
			Get
				Return If(myEmailAddress, New TextString(mappedEmailAddress, Sub(v) mappedEmailAddress = v))
			End Get
		End Property

		Public Sub AboutEmailAddress(a As FieldAbout, EmailAddress As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Rating"
		Public Property mappedRating As Integer
		Friend myRating As WholeNumber

		<MemberOrder(4)>
		Public ReadOnly Property Rating As WholeNumber
			Get
				Return If(myRating, New WholeNumber(mappedRating, Sub(v) mappedRating = v))
			End Get
		End Property

		Public Sub AboutRating(a As FieldAbout, Rating As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Comments"
		Public Property mappedComments As String
		Friend myComments As TextString

		<MemberOrder(5)>
		Public ReadOnly Property Comments As TextString
			Get
				Return If(myComments, New TextString(mappedComments, Sub(v) mappedComments = v))
			End Get
		End Property

		Public Sub AboutComments(a As FieldAbout, Comments As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Property ProductID() As Integer

		Public Overridable Property Product() As Product

		Public Sub AboutProduct(a As FieldAbout, p As Product)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
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
			Return "*****".Substring(0, mappedRating)
		End Function
	End Class
End Namespace