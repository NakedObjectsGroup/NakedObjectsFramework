Namespace AW

	Public Module Helpers
		''' <summary>
		'''     Returns a random instance from the set of all instance of type T
		''' </summary>
		Public Function Random(Of T As Class)(ByVal context As IContext) As T
			'The OrderBy(...) doesn't change the ordering, but is a necessary precursor to using .Skip
			'which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
			Dim instances = context.Instances(Of T)().OrderBy(Function(n) "")
			Return instances.Skip(context.RandomSeed().ValueInRange(instances.Count())).First()
		End Function
	End Module
End Namespace