Namespace AW.Types
	<Named("Contact")>
	Partial Public Class BusinessEntityContact

		<Hidden>
		Public Property BusinessEntityID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual BusinessEntity? BusinessEntity {get;set;}
		<Hidden>
		Public Overridable Property BusinessEntity() As BusinessEntity

		<Hidden>
		Public Property PersonID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Person? Person {get;set;}
		<MemberOrder(1)>
		Public Overridable Property Person() As Person

		<Hidden>
		Public Property ContactTypeID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual ContactType? ContactType {get;set;}
		<MemberOrder(2)>
		Public Overridable Property ContactType() As ContactType

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
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

		<Hidden>
		Public Property RowGuid() As Guid

		Public Overrides Function ToString() As String
			Return $"{Person}"
		End Function
	End Class
End Namespace