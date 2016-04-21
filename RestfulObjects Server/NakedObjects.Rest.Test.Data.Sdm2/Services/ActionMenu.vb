Imports sdm.systems.application.control

Namespace RestfulObjects.Test.Data
    'Provides static (Shared) actions that delegate to the Action Service
    Public Class ActionMenu
        Inherits AbstractBusinessObject

#Region "AnAction"

        Public Shared Function actionAnAction() As MostSimple
            Return WithActionRepositoryFromThread.actionAnAction
        End Function

#End Region

#Region "AHiddenAction"

        Public Shared Function actionAHiddenAction() As MostSimple
            Return WithActionRepositoryFromThread.actionAHiddenAction
        End Function

        Public Shared Sub aboutActionAHiddenAction(a As ActionAbout)
            WithActionRepositoryFromThread.aboutActionAHiddenAction(a)
        End Sub

#End Region

#Region "ADisabledAction"

        Public Shared Function actionADisabledAction() As MostSimple
            Return WithActionRepositoryFromThread.actionADisabledAction
        End Function

        Public Shared Sub aboutActionADisabledAction(a As ActionAbout)
            WithActionRepositoryFromThread.aboutActionADisabledAction(a)
        End Sub

#End Region

#Region "ADisabledCollectionAction"

        Public Shared Function actionADisabledCollectionAction() As ArrayList
            Return WithActionRepositoryFromThread.actionADisabledCollectionAction
        End Function

        Public Shared Sub aboutActionADisabledCollectionAction(a As ActionAbout)
            WithActionRepositoryFromThread.aboutActionADisabledCollectionAction(a)
        End Sub

#End Region

#Region "AnActionReturnsScalar"

        Public Shared Function actionAnActionReturnsScalar() As WholeNumber
            Return WithActionRepositoryFromThread.actionAnActionReturnsScalar
        End Function

#End Region

#Region "AnActionReturnsVoid"

        Public Sub actionAnActionReturnsVoid()
            WithActionRepositoryFromThread.actionAnActionReturnsVoid()
        End Sub

#End Region

#Region "AnActionReturnsCollection"

        Public Shared Function actionAnActionReturnsCollection() As ArrayList
            Return WithActionRepositoryFromThread.actionAnActionReturnsCollection
        End Function

#End Region

#Region "AnActionReturnsCollectionWithScalarParameters"

        Public Shared Function actionAnActionReturnsCollectionWithScalarParameters(parm1 As WholeNumber,
                                                                                   parm2 As TextString) As ArrayList
            Return WithActionRepositoryFromThread.actionAnActionReturnsCollectionWithScalarParameters(parm1, parm2)
        End Function

        Public Shared Sub aboutActionAnActionReturnsCollectionWithScalarParameters(a As ActionAbout,
                                                                                   ByVal parm5 As WholeNumber)

            a.setParameters({"parm2"})
        End Sub

#End Region

#Region "AnActionReturnsCollectionWithParameters"

        Public Shared Function actionAnActionReturnsCollectionWithParameters(parm1 As WholeNumber, parm2 As MostSimple) _
            As ArrayList
            Return WithActionRepositoryFromThread.actionAnActionReturnsCollectionWithParameters(parm1, parm2)
        End Function

        Public Shared Sub aboutActionAnActionReturnsCollectionWithParameters(a As ActionAbout,
                                                                             ByVal parm5 As WholeNumber)

            a.setParameters({"parm5"})
        End Sub

#End Region

#Region "AnActionReturnsScalarWithParameters"

        Public Shared Function actionAnActionReturnsScalarWithParameters(parm1 As WholeNumber, parm2 As MostSimple) _
            As WholeNumber
            Return WithActionRepositoryFromThread.actionAnActionReturnsScalarWithParameters(parm1, parm2)
        End Function

        Public Shared Sub aboutActionAnActionReturnsScalarWithParameters(a As ActionAbout, ByVal parm5 As WholeNumber)

            a.setParameters({"parm1", "parm2"})
        End Sub

#End Region

#Region "AnActionReturnsVoidWithParameters"

        Public Sub actionAnActionReturnsVoidWithParameters(parm1 As WholeNumber, parm2 As MostSimple)
            WithActionRepositoryFromThread.actionAnActionReturnsVoidWithParameters(parm1, parm2)
        End Sub

        Public Shared Sub aboutActionAnActionReturnsVoidWithParameters(a As ActionAbout, ByVal parm5 As WholeNumber)

            a.setParameters({"parm1", "parm2"})
        End Sub


#End Region

#Region "AnActionReturnsObjectWithParameters"

        Public Shared Function actionAnActionReturnsObjectWithParameters(parm1 As WholeNumber, parm2 As MostSimple) _
            As MostSimple
            Return WithActionRepositoryFromThread.actionAnActionReturnsObjectWithParameters(parm1, parm2)
        End Function

        Public Shared Sub aboutActionAnActionReturnsObjectWithParameters(a As ActionAbout, ByVal parm5 As WholeNumber)

            a.setParameters({"parm2"})
        End Sub


#End Region

#Region "AnActionWithValueParameter"

        Public Shared Function actionAnActionWithValueParameter(parm1 As WholeNumber) As MostSimple
            Return WithActionRepositoryFromThread.actionAnActionWithValueParameter(parm1)
        End Function

        Public Shared Sub aboutActionAnActionWithValueParameter(a As ActionAbout, ByVal parm5 As WholeNumber)

            a.setParameters({"parm5"})
        End Sub

#End Region

#Region "AnActionWithValueParameterWithChoices"

        'RP: Can't specify choices for a value param; only a reference param
        'Public Overridable Function actionAnActionWithValueParameterWithChoices(ByVal parm3 As WholeNumber) As MostSimple
        '    Return MostSimpleWithIdOf1()
        'End Function

#End Region

#Region "AnActionWithValueParameterWithDefault"

        Public Shared Function actionAnActionWithValueParameterWithDefault(parm5 As WholeNumber) As MostSimple
            Return WithActionRepositoryFromThread.actionAnActionWithValueParameterWithDefault(parm5)
        End Function

        Public Shared Sub aboutActionAnActionWithValueParameterWithDefault(a As ActionAbout, ByVal parm5 As WholeNumber)
            WithActionRepositoryFromThread.aboutActionAnActionWithValueParameterWithDefault(a, parm5)
            a.setParameters({"parm5"})
        End Sub

#End Region

#Region "AnActionWithReferenceParameter"

        Public Shared Function actionAnActionWithReferenceParameter(parm2 As MostSimple) As MostSimple
            Return WithActionRepositoryFromThread.actionAnActionWithReferenceParameter(parm2)
        End Function

        Public Shared Sub aboutActionAnActionWithReferenceParameter(a As ActionAbout, ByVal parm4 As MostSimple)
            a.setParameters({"parm4"})
        End Sub

#End Region

#Region "AnActionWithReferenceParameterWithChoices"

        Public Shared Function actionAnActionWithReferenceParameterWithChoices(parm4 As MostSimple) As MostSimple
            Return WithActionRepositoryFromThread.actionAnActionWithReferenceParameterWithChoices(parm4)
        End Function

        Public Shared Sub aboutActionAnActionWithReferenceParameterWithChoices(
                                                                               a As ActionAbout,
                                                                               ByVal parm4 As MostSimple)
            WithActionRepositoryFromThread.aboutActionAnActionWithReferenceParameterWithChoices(a, parm4)
            a.setParameters({"parm4"})
        End Sub

#End Region

#Region "AnActionWithReferenceParameterWithDefault"

        Public Shared Function actionAnActionWithReferenceParameterWithDefault(parm6 As MostSimple) As MostSimple
            Return WithActionRepositoryFromThread.actionAnActionWithReferenceParameterWithDefault(parm6)
        End Function

        Public Shared Sub aboutActionAnActionWithReferenceParameterWithDefault(a As ActionAbout,
                                                                               ByVal parm4 As MostSimple)
            WithActionRepositoryFromThread.aboutActionAnActionWithReferenceParameterWithDefault(a, parm4)
            a.setParameters({"parm4"})
        End Sub

#End Region

#Region "AnActionWithParametersWithChoicesWithDefaults"

        'RP: Cannot do the choices for the scalar property  -  need to alter the tests
        Public Shared Function actionAnActionWithParametersWithChoicesWithDefaults(
                                                                                   parm1 As WholeNumber,
                                                                                   parm7 As WholeNumber,
                                                                                   parm2 As MostSimple,
                                                                                   parm8 As MostSimple) As MostSimple
            Return WithActionRepositoryFromThread.actionAnActionWithParametersWithChoicesWithDefaults(
                parm1, parm7, parm2, parm8)
        End Function

        Public Shared Sub aboutActionAnActionWithParametersWithChoicesWithDefaults(a As ActionAbout,
                                                                                   ByVal parm1 As WholeNumber,
                                                                                   ByVal parm7 As WholeNumber,
                                                                                   ByVal parm2 As MostSimple,
                                                                                   ByVal parm8 As MostSimple)
            WithActionRepositoryFromThread.aboutActionAnActionWithParametersWithChoicesWithDefaults(a, parm1, parm7,
                                                                                                    parm2, parm8)
            a.setParameters({"parm1", "parm7", "parm2", "parm8"})
        End Sub

#End Region

#Region "AnError"

        Public Shared Function actionAnError() As WholeNumber
            Return WithActionRepositoryFromThread.actionAnError
        End Function

#End Region

#Region "AnErrorQuery"

        'RP: I made this return an arraylist  as nof2 can't return a queryable
        Public Shared Function actionAnErrorQuery() As ArrayList
            Return WithActionRepositoryFromThread.actionAnErrorQuery
        End Function

#End Region

#Region "Container, services, repositories"

        Protected Shared Function WithActionRepositoryFromThread() _
            As IWithActionRepository
            Return CType(
                ServiceLocatorFactory.ServiceLocator.Repository(GetType(IWithActionRepository)),
                IWithActionRepository)
        End Function

#End Region
    End Class
End Namespace
