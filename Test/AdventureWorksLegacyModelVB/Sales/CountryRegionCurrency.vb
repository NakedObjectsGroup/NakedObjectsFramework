Namespace AW.Types

	Partial Public Class CountryRegionCurrency
 Implements ITitledObject, INotEditableOncePersistent
		Public Property CountryRegionCode() As String = ""

		Public Property CurrencyCode() As String = ""

		Public Overridable Property CountryRegion() As CountryRegion

		Public Overridable Property Currency() As Currency


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
            Return $"CountryRegionCurrency: {CountryRegion} {Currency}"
        End Function
    End Class
End Namespace