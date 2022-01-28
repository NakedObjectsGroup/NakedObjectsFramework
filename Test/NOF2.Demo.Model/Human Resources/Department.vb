Namespace AW.Types


    Partial Public Class Department
        Implements ITitledObject, IBounded 'Does not implement INotEditableOncePersistent to permit testing

        Public Property DepartmentID As Short  'Not visible on UI

#Region "Name"
        Public Property mappedName As String

        Friend myName As TextString

        <DemoProperty(Order:=1, IsRequired:=True)>
        Public ReadOnly Property Name As TextString
            Get
                myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
                Return myName
            End Get
        End Property

#End Region

#Region "GroupName"
        Public Property mappedGroupName As String
        Friend myGroupName As TextString

        <DemoProperty(Order:=2, IsRequired:=True)>
        Public ReadOnly Property GroupName As TextString
            Get
                myGroupName = If(myGroupName, New TextString(mappedGroupName, Sub(v) mappedGroupName = v))
                Return myGroupName
            End Get
        End Property

        Public Sub AboutGroupName(a As FieldAbout, groupName As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                    If (groupName.Value.Length > 50) Then
                        a.IsValid = False
                        a.InvalidReason = "Cannot be > 50 chars"
                    End If
                Case Else
            End Select
        End Sub
#End Region

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <DemoProperty(Order:=99)>
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
            Return mappedName
        End Function
    End Class
End Namespace