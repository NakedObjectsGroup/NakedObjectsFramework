Namespace AW.Types

    <Bounded>
    Partial Public Class Department
        Implements IHasModifiedDate

#Region "DepartmentID"
        Friend mappedDepartmentID As Short
        Friend myDepartmentID As WholeNumber

        <MemberOrder(1)>
        Public ReadOnly Property DepartmentID As WholeNumber
            Get
                Return If(myDepartmentID, New WholeNumber(mappedDepartmentID, Function(v) mappedDepartmentID = v))
            End Get
        End Property

        Public Sub AboutDepartmentID(a As FieldAbout, DepartmentID As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Visible
                    a.Visible = False
            End Select
        End Sub
#End Region

#Region "Name"
        Friend mappedName As String
        Friend myName As TextString

        <MemberOrder(1)>
        Public ReadOnly Property Name As TextString
            Get
                Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
            End Get
        End Property

        Public Sub AboutName(a As FieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Parameters
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "GroupName"
        Friend mappedGroupName As String
        Friend myGroupName As TextString

        <MemberOrder(2)>
        Public ReadOnly Property GroupName As TextString
            Get
                Return If(myGroupName, New TextString(mappedGroupName, Function(v) mappedGroupName = v))
            End Get
        End Property
#End Region

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

        Public Function Title() As Title
            Return New Title(Name)
        End Function
    End Class
End Namespace