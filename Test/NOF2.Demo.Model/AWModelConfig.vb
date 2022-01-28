Imports System.Data.Entity
Imports Microsoft.Extensions.Configuration

Namespace AW
	Public Module AWModelConfig
		Private ReadOnly Property DomainClasses() As IEnumerable(Of Type)
			Get
				Return GetType(AWModelConfig).Assembly.GetTypes().Where(Function(t) t.IsPublic AndAlso (t.IsClass OrElse t.IsInterface OrElse t.IsEnum))
			End Get
		End Property

		Public ReadOnly Property DbContextCreator() As Func(Of IConfiguration, DbContext)
			Get
				Return Function(c) New AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"))
			End Get
		End Property

		Public ReadOnly Property EFCDbContextCreator() As Func(Of IConfiguration, Microsoft.EntityFrameworkCore.DbContext)
			Get
				Return Function(c) New AdventureWorksEFCoreContext(c.GetConnectionString("AdventureWorksContext"))
			End Get
		End Property

		'IsAbstract && IsSealed defines a static class. Not really necessary here, just extra safety check.
		Public Function FunctionalTypes() As Type()
			Return DomainClasses.Where(Function(t) t.Namespace = "AW.Types" AndAlso Not (t.IsAbstract AndAlso t.IsSealed)).ToArray()
		End Function

		Public Function Functions() As Type()
			Return DomainClasses.Where(Function(t) t.Namespace = "AW.Functions" AndAlso t.IsAbstract AndAlso t.IsSealed).ToArray()
		End Function

		Public Function MainMenuTypes() As Type()
			Return Functions().Where(Function(t) t.FullName?.Contains("MenuFunctions") = True).ToArray()
		End Function
	End Module
End Namespace