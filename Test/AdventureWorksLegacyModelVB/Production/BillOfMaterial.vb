Namespace AW.Types

	Partial Public Class BillOfMaterial
		Implements IHasModifiedDate

		<Hidden>
		Public Property BillOfMaterialID() As Integer

		Public Property StartDate() As DateTime

		Public Property EndDate() As DateTime?

		Public Property BOMLevel() As Short

		Public Property PerAssemblyQty() As Decimal

		<Hidden>
		Public Property ProductAssemblyID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Product? Product {get;set;}
		Public Overridable Property Product() As Product

		<Hidden>
		Public Property ComponentID() As Integer

		Public Overridable Property Product1() As Product

		<Hidden>
		Public Property UnitMeasureCode() As String = ""

		Public Overridable Property UnitMeasure() As UnitMeasure

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

		Public Overrides Function ToString() As String
			Return $"BillOfMaterial: {BillOfMaterialID}"
		End Function
	End Class
End Namespace