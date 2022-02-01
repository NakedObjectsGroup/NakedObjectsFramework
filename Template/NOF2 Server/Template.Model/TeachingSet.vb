Public Class TeachingSet
    Public Property Container As IContainer

    Public Overridable Property Id As Integer


    Public Property mappedSetName As String
    Private mySetName As TextString

    <DemoProperty(Order:=1)>
    Public ReadOnly Property SetName As TextString
        Get
            mySetName = If(mySetName, New TextString(mappedSetName, Sub(v) mappedSetName = v))
            Return mySetName
        End Get
    End Property


    <DemoProperty(Order:=2)>
    Public Overridable Property Subject As Subject

    Public Property mappedYearGroup As Integer
    Private myYearGroup As WholeNumber

    <DemoProperty(Order:=3)> '< Range(9, 13)>
    Public ReadOnly Property YearGroup As WholeNumber
        Get
            myYearGroup = If(myYearGroup, New WholeNumber(mappedYearGroup, Sub(v) mappedYearGroup = v))
            Return myYearGroup
        End Get
    End Property

    <DemoProperty(Order:=4)>
    Public Overridable Property Teacher As Teacher

    <TableView(False, "FullName")>
    <DemoProperty(Order:=5)>
    Public Overridable Property mappedStudents As ICollection(Of Student) = New List(Of Student)()

    Private myStudents As InternalCollection

    Public ReadOnly Property Students As InternalCollection
        Get
            myStudents = If(myStudents, New InternalCollection(Of Student)(mappedStudents))
            Return myStudents
        End Get
    End Property


    Public Sub ActionAddStudentToSet(ByVal student As Student)
        Students.Add(student)
    End Sub

    Public Function ActionRemoveStudentFromSet(ByVal student As Student)
        Return mappedStudents.Remove(student)
    End Function

    'TODO
    Public Function Choices0RemoveStudentFromSet() As IList(Of Student)
        Return Students.ToList()
    End Function

    Public Sub ActionTransferStudentTo(ByVal student As Student, ByVal newSet As TeachingSet)
        ActionRemoveStudentFromSet(student)
        newSet.ActionAddStudentToSet(student)
    End Sub

    'TODO
    Public Function Choices0TransferStudentTo() As IList(Of Student)
        Return Students.ToList()
    End Function

    'TODO
    Public Function Choices1TransferStudentTo() As IList(Of TeachingSet)
        Dim subjId As Integer = Subject.Id
        Dim yg As Integer = YearGroup.Value
        Return Container.AllInstances(Of TeachingSet)().Where(Function(s) s.Subject.Id = subjId AndAlso s.mappedYearGroup = yg).ToList()
    End Function

    Public Function Title() As Title
        Return New Title(SetName)
    End Function

End Class

