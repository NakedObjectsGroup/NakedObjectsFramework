Imports Microsoft.VisualBasic
Imports System.Xml.Linq
Imports NakedFramework
Imports System.Text

Namespace AW
    Friend Module Utilities
        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: internal static string FormatXML(string? inputXML)
        Friend Function FormatXML(ByVal inputXML As String) As String
            Dim output = New StringBuilder()

            If Not String.IsNullOrEmpty(inputXML) Then
                XElement.Parse(inputXML).Elements().ToList().ForEach(Function(n) output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) & ": " & n.Value & vbLf))
            End If

            Return output.ToString()
        End Function

        Friend Function HashCode(ByVal obj As Object, ParamArray ByVal keys() As Integer) As Integer
            'Uses Josh Bloch's algorithm
            Dim hash = 17 * 23 + obj.GetType().GetProxiedType().GetHashCode()
            For Each key In keys
                hash = hash * 23 + key.GetHashCode()
            Next key

            Return hash
        End Function
    End Module
End Namespace