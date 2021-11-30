Namespace AW.Types

	<Bounded>
	Partial Public Class Shift
		Implements IHasModifiedDate

		<Hidden>
		Public Property ShiftID() As Byte

		<MemberOrder(1)>
		Public Property Name() As String = ""

		<MemberOrder(3), Mask("T")>
		Public Property StartTime() As TimeSpan

		<MemberOrder(4), Mask("T")>
		Public Property EndTime() As TimeSpan

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
			Return Name
		End Function
	End Class
End Namespace