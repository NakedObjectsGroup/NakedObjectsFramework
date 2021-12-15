Namespace AW.Types

	Partial Public Class SpecialOffer

		''<Hidden>
		Public Property SpecialOfferID() As Integer

#Region "Description"
		Friend mappedDescription As String
		Friend myDescription As TextString

		'<MemberOrder(10)>
		Public ReadOnly Property Description As TextString
			Get
				Return If(myDescription, New TextString(mappedDescription, Function(v) mappedDescription = v))
			End Get
		End Property

		Public Sub AboutDescription(a As FieldAbout, Description As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		'<MemberOrder(20)>
		Public Property DiscountPct() As Decimal 'TODO: Percentage ValueHolder

#Region "Type"
		Friend mappedType As String
		Friend myType As TextString

		'<MemberOrder(30)>
		Public ReadOnly Property Type As TextString
			Get
				Return If(myType, New TextString(mappedType, Function(v) mappedType = v))
			End Get
		End Property

		Public Sub AboutType(a As FieldAbout, Type As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Category"
		Friend mappedCategory As String
		Friend myCategory As TextString

		'<MemberOrder(40)>
		Public ReadOnly Property Category As TextString
			Get
				Return If(myCategory, New TextString(mappedCategory, Function(v) mappedCategory = v))
			End Get
		End Property

		Public Sub AboutCategory(a As FieldAbout, Category As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "StartDate"
		Friend mappedStartDate As Date
		Friend myStartDate As NODate

		'<MemberOrder(51)>
		Public ReadOnly Property StartDate As NODate
			Get
				Return If(myStartDate, New NODate(mappedStartDate, Function(v) mappedStartDate = v))
			End Get
		End Property

		Public Sub AboutStartDate(a As FieldAbout, StartDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "EndDate"
		Friend mappedEndDate As Date
		Friend myEndDate As NODate

		'<MemberOrder(52)>
		Public ReadOnly Property EndDate As NODate
			Get
				Return If(myEndDate, New NODate(mappedEndDate, Function(v) mappedEndDate = v))
			End Get
		End Property

		Public Sub AboutEndDate(a As FieldAbout, EndDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "MinQty"
		Friend mappedMinQty As Integer
		Friend myMinQty As WholeNumber

		'<MemberOrder(61)>
		Public ReadOnly Property MinQty As WholeNumber
			Get
				Return If(myMinQty, New WholeNumber(mappedMinQty, Function(v) mappedMinQty = v))
			End Get
		End Property

		Public Sub AboutMinQty(a As FieldAbout, MinQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "MaxQty"
		Friend mappedMaxQty As Integer?
		Friend myMaxQty As WholeNumber

		'<MemberOrder(62)>
		Public ReadOnly Property MaxQty As WholeNumber
			Get
				Return If(myMaxQty, New WholeNumber(mappedMaxQty, Function(v) mappedMaxQty = v))
			End Get
		End Property

		Public Sub AboutMaxQty(a As FieldAbout, MaxQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(1)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout, ModifiedDate As TimeStamp)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property RowGuid() As Guid

		Public Function Title() As Title
			Return New Title(Description)
		End Function
	End Class
End Namespace