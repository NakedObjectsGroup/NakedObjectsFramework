Namespace AW.Types

    Public Class GenericMenuFunctions
        Public Shared Function Random(Of T As Class)() As T
            Dim max = ThreadLocals.Container.Instances(Of T).Count
            Dim rnd = (New Random).Next(max)
            Return ThreadLocals.Container.Instances(Of T).OrderBy(Function(e) "").Skip(rnd).First
        End Function

        Public Shared Function ListAll(Of T As Class)() As IQueryable(Of T)
            Return ThreadLocals.Container.Instances(Of T)
        End Function
    End Class
End Namespace
