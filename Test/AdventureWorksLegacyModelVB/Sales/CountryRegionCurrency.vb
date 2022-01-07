Namespace AW.Types

	Partial Public Class CountryRegionCurrency
 Implements ITitledObject
		Public Property CountryRegionCode() As String = ""

		Public Property CurrencyCode() As String = ""

		Public Overridable Property CountryRegion() As CountryRegion

		Public Overridable Property Currency() As Currency


#Region "ModifiedDate"
        Public mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        '<MemberOrder(99)>
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

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"CountryRegionCurrency: {CountryRegion} {Currency}"
        End Function
    End Class
End Namespace