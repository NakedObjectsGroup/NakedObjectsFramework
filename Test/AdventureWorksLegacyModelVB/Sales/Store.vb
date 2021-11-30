

Namespace AW.Types

	Partial Public Class Store
		Inherits BusinessEntity
		Implements IBusinessEntityWithContacts, IHasModifiedDate

		<Named("Store Name")>
		<MemberOrder(20)>
		Public Property Name() As String = ""

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Demographics {get;set;}
		<Hidden>
		Public Property Demographics() As String

		<Named("Demographics")>
		<MemberOrder(30)>
		<MultiLine(10)>
		Public ReadOnly Property FormattedDemographics() As String
			Get
				Return Utilities.FormatXML(Demographics)
			End Get
		End Property

		<Hidden>
		Public Property SalesPersonID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual SalesPerson? SalesPerson {get;set;}
		<MemberOrder(40)>
		Public Overridable Property SalesPerson() As SalesPerson

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
		Public ReadOnly Property ModifiedDate As TimeStamp Implements IHasModifiedDate.ModifiedDate
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

		<Hidden>
		Public Property rowguid() As Guid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace