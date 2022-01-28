Namespace AW.Functions
	Public Module SalesPerson_Functions

		<MemberOrder(1)>
		<System.Runtime.CompilerServices.Extension>
		Public Function RecalulateSalesYTD(ByVal sp As SalesPerson, ByVal context As IContext) As IContext
			Dim startOfYear = New DateTime(DateTime.Now.Year, 1, 1)
			Dim query = From obj In context.Instances(Of SalesOrderHeader)()
						Where obj.SalesPerson IsNot Nothing AndAlso obj.SalesPerson.BusinessEntityID = sp.BusinessEntityID AndAlso obj.StatusByte = 5 AndAlso obj.OrderDate >= startOfYear
						Select obj
			Dim newYTD = If(query.Any(), query.Sum(Function(n) n.SubTotal), 0)

			Return context.WithUpdated(sp, NewDirectCast({
				SalesYTD = newYTD,
				ModifiedDate = context.Now()
			}, sp))
		End Function

		<MemberOrder(2)>
		<System.Runtime.CompilerServices.Extension> _
		Public Function ChangeSalesQuota(ByVal sp As SalesPerson, ByVal newQuota As Decimal, ByVal context As IContext) As IContext
			Return context.WithUpdated(sp, NewDirectCast({
				SalesQuota = newQuota,
				ModifiedDate = context.Now()
			}, sp)).WithNew(New SalesPersonQuotaHistory With {
				.SalesPerson = sp,
				.SalesQuota = newQuota,
				.QuotaDate = context.Now()
			})
		End Function

		<MemberOrder(1)>
		<System.Runtime.CompilerServices.Extension> _
		Public Function ChangeSalesTerritory(ByVal sp As SalesPerson, ByVal newTerritory As SalesTerritory, ByVal context As IContext) As IContext
			Dim newHist = New SalesTerritoryHistory With {
				.SalesPerson = sp,
				.SalesTerritory = newTerritory,
				.StartDate = context.Now()
			}
			Dim prev = sp.TerritoryHistory.First(Function(n) n.EndDate Is Nothing)
			Dim uPrev As New SalesTerritoryHistory(prev)
			If True Then
				EndDate = context.Now()
			End If
			Dim uSp As New SalesPerson(sp)
			If True Then
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: SalesTerritory = newTerritory, ModifiedDate = context.Now()
				newTerritory, ModifiedDate = context.Now()
				SalesTerritory = newTerritory, ModifiedDate
			End If
			Return context.WithNew(newHist).WithUpdated(sp, uSp).WithUpdated(prev, uPrev)
		End Function
	End Module

End Namespace