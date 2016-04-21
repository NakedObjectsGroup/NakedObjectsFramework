' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
Imports System.Collections.Generic
Imports System.Linq
Imports sdm.systems.application.control

Namespace RestfulObjects.Test.Data
    Public MustInherit Class WithAction
        Inherits AbstractBusinessObject
        Implements IWithAction

#Region "Helpers"

        Private Function MostSimpleWithIdOf1() As MostSimple
            Return AllMostSimples.Where(Function(x) x.Id.ctypeIntValue = 1).Single()
        End Function

        Private Function AllMostSimples() As IEnumerable(Of MostSimple)
            Return Container.allInstances(GetType(MostSimple)).Cast (Of MostSimple)()
        End Function

#End Region

#Region "AnAction"

        Public Overridable Function actionAnAction() As MostSimple Implements IWithAction.actionAnAction
            Return MostSimpleWithIdOf1()
        End Function

#End Region

#Region "AnActionReturnsNull"

        Public Overridable Function actionAnActionReturnsNull() As MostSimple
            Return Nothing
        End Function

#End Region

#Region "AnActionWithOptionalParm"

        Public Overridable Function actionAnActionWithOptionalParm(parm As String) As MostSimple _
            Implements IWithAction.actionAnActionWithOptionalParm
            Return MostSimpleWithIdOf1()
        End Function

#End Region

#Region "AnActionAnnotatedQueryOnly"

        Public Overridable Function actionAnActionAnnotatedQueryOnly() As MostSimple
            Return MostSimpleWithIdOf1()
        End Function

        Public Overridable Function actionAnActionAnnotatedQueryOnlyReturnsNull() As MostSimple
            Return Nothing
        End Function

#End Region

#Region "AnActionAnnotatedIdempotent"

        Public Overridable Function actionAnActionAnnotatedIdempotent() As MostSimple
            Return MostSimpleWithIdOf1()
        End Function

        Public Overridable Function actionAnActionAnnotatedIdempotentReturnsNull() As MostSimple
            Return Nothing
        End Function

#End Region

#Region "AHiddenAction"

        Public Overridable Function actionAHiddenAction() As MostSimple Implements IWithAction.actionAHiddenAction
            Return MostSimpleWithIdOf1()
        End Function

        Public Sub aboutActionAHiddenAction(a As ActionAbout) Implements IWithAction.aboutActionAHiddenAction
            a.invisible()
        End Sub

#End Region

#Region "ADisabledAction"

        Public Overridable Function actionADisabledAction() As MostSimple Implements IWithAction.actionADisabledAction
            Return MostSimpleWithIdOf1()
        End Function

        Public Sub aboutActionADisabledAction(a As ActionAbout) Implements IWithAction.aboutActionADisabledAction
            a.unusable()
        End Sub


#End Region

#Region "ADisabledQueryAction"

        Public Overridable Function actionADisabledQueryAction() As ArrayList _
            Implements IWithAction.actionADisabledQueryAction
            Return Container.allInstances(GetType(MostSimple))
        End Function


        Public Sub aboutActionADisabledQueryAction(a As ActionAbout) _
            Implements IWithAction.aboutActionADisabledQueryAction
            a.unusable()
        End Sub


#End Region

#Region "ADisabledCollectionAction"

        Public Overridable Function actionADisabledCollectionAction() As ArrayList _
            Implements IWithAction.actionADisabledCollectionAction
            Return Container.allInstances(GetType(MostSimple))
        End Function

        Public Sub aboutActionADisabledCollectionAction(a As ActionAbout) _
            Implements IWithAction.aboutActionADisabledCollectionAction
            a.unusable()
        End Sub

#End Region

#Region "AnActionReturnsScalar"

        Public Overridable Function actionAnActionReturnsScalar() As WholeNumber _
            Implements IWithAction.actionAnActionReturnsScalar
            Return New WholeNumber(Me, 999)
        End Function

        Public Overridable Function actionAnActionReturnsScalarEmpty() As TextString

            Return New TextString(Me, "")
        End Function

        Public Overridable Function actionAnActionReturnsScalarNull() As TextString

            Dim s As String = Nothing
            Return New TextString(Me, s)
        End Function


#End Region

#Region "AnActionReturnsVoid"

        Public Overridable Sub actionAnActionReturnsVoid() Implements IWithAction.actionAnActionReturnsVoid
        End Sub


#End Region

#Region "AnActionReturnsQueryable"

        Public Overridable Function actionAnActionReturnsQueryable() As ArrayList _
            Implements IWithAction.actionAnActionReturnsQuery
            Return Container.allInstances(GetType(MostSimple))
        End Function

#End Region

#Region "AnActionReturnsCollection"

        Public Overridable Function actionAnActionReturnsCollection() As ArrayList _
            Implements IWithAction.actionAnActionReturnsCollection
            Return Container.allInstances(GetType(MostSimple))
        End Function

        Public Overridable Function actionAnActionReturnsCollectionEmpty() As ArrayList
            Return New ArrayList()
        End Function

        Public Overridable Function actionAnActionReturnsCollectionNull() As ArrayList
            Return Nothing
        End Function

#End Region

#Region "AnActionReturnsQueryableWithScalarParameters"

        Public Overridable Function actionAnActionReturnsQueryableWithScalarParameters(
                                                                                       ByVal parm1 As WholeNumber,
                                                                                       ByVal parm2 As TextString) _
            As ArrayList Implements IWithAction.actionAnActionReturnsQueryWithScalarParameters
            NUnit.Framework.Assert.AreEqual(100, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual("fred", parm2.stringValue)
            Return Container.allInstances(GetType(MostSimple))
        End Function

#End Region

#Region "AnActionWithDateTimeParm"

        Public Overridable Sub actionAnActionWithDateTimeParm(ByVal parm As NoDate)
        End Sub

#End Region


#Region "AnActionReturnsCollectionWithScalarParameters"

        Public Overridable Function actionAnActionReturnsCollectionWithScalarParameters(
                                                                                        ByVal parm1 As WholeNumber,
                                                                                        ByVal parm2 As TextString) _
            As ArrayList Implements IWithAction.actionAnActionReturnsCollectionWithScalarParameters
            NUnit.Framework.Assert.AreEqual(100, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual("fred", parm2.stringValue)
            Return Container.allInstances(GetType(MostSimple))
        End Function

        Public Sub aboutActionAnActionReturnsCollectionWithScalarParameters(a As ActionAbout,
                                                                            ByVal parm1 As WholeNumber,
                                                                            ByVal parm2 As TextString) _
            Implements IWithAction.aboutActionAnActionReturnsCollectionWithScalarParameters
            a.setParameters({"parm1", "parm2"})

            If parm1.isEmpty() Or parm2 Is Nothing Then
                a.unusable("Mandatory")
            End If
        End Sub

#End Region

#Region "AnActionReturnsQueryableWithParameters"

        Public Overridable Function actionAnActionReturnsQueryableWithParameters(ByVal parm1 As WholeNumber,
                                                                                 ByVal parm2 As MostSimple) _
            As ArrayList Implements IWithAction.actionAnActionReturnsQueryWithParameters
            NUnit.Framework.Assert.AreEqual(101, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual(AllMostSimples.First(), parm2)
            Return Container.allInstances(GetType(MostSimple))
        End Function

#End Region

#Region "AnActionReturnsCollectionWithParameters"

        Public Overridable Function actionAnActionReturnsCollectionWithParameters(ByVal parm1 As WholeNumber,
                                                                                  ByVal parm2 As MostSimple) _
            As ArrayList Implements IWithAction.actionAnActionReturnsCollectionWithParameters
            NUnit.Framework.Assert.AreEqual(101, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual(AllMostSimples.First(), parm2)
            Return Container.allInstances(GetType(MostSimple))
        End Function

        Public Sub aboutActionAnActionReturnsCollectionWithParameters(a As ActionAbout, ByVal parm1 As WholeNumber,
                                                                      ByVal parm2 As MostSimple)
            a.setParameters({"parm1", "parm2"})
        End Sub

#End Region

#Region "AnActionReturnsScalarWithParameters"

        Public Overridable Function actionAnActionReturnsScalarWithParameters(
                                                                              ByVal parm1 As WholeNumber,
                                                                              ByVal parm2 As MostSimple) As WholeNumber _
            Implements IWithAction.actionAnActionReturnsScalarWithParameters
            NUnit.Framework.Assert.AreEqual(101, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual(AllMostSimples().First(), parm2)
            Return New WholeNumber(Me, 555)
        End Function

        Public Sub aboutActionAnActionReturnsScalarWithParameters(a As ActionAbout, ByVal parm1 As WholeNumber,
                                                                  ByVal parm2 As MostSimple)
            a.setParameters({"parm1", "parm2"})

            If parm1.isEmpty() Or parm2 Is Nothing Then
                a.unusable("Mandatory")
            End If
        End Sub

#End Region

#Region "AnActionReturnsVoidWithParameters"

        Public Overridable Sub actionAnActionReturnsVoidWithParameters(
                                                                       ByVal parm1 As WholeNumber,
                                                                       ByVal parm2 As MostSimple) _
            Implements IWithAction.actionAnActionReturnsVoidWithParameters
            NUnit.Framework.Assert.AreEqual(101, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual(AllMostSimples.First(), parm2)
        End Sub

        Public Sub aboutActionAnActionReturnsVoidWithParameters(a As ActionAbout, ByVal parm1 As WholeNumber,
                                                                ByVal parm2 As MostSimple)
            a.setParameters({"parm1", "parm2"})
        End Sub

#End Region

#Region "AnActionReturnsObjectWithParameters"

        Public Overridable Function actionAnActionReturnsObjectWithParameters(
                                                                              ByVal parm1 As WholeNumber,
                                                                              ByVal parm2 As MostSimple) As MostSimple _
            Implements IWithAction.actionAnActionReturnsObjectWithParameters
            NUnit.Framework.Assert.AreEqual(101, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual(AllMostSimples.First(), parm2)
            Return AllMostSimples.First()
        End Function

        Public Sub aboutActionAnActionReturnsObjectWithParameters(a As ActionAbout, ByVal parm1 As WholeNumber,
                                                                  ByVal parm2 As MostSimple)
            a.setParameters({"parm1", "parm2"})
        End Sub


#End Region

        Public Overridable Function actionAnActionReturnsObjectWithParametersAnnotatedQueryOnly(
                                                                                                ByVal parm1 As _
                                                                                                   WholeNumber,
                                                                                                ByVal parm2 As _
                                                                                                   MostSimple) _
            As MostSimple
            NUnit.Framework.Assert.AreEqual(101, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual(AllMostSimples.First(), parm2)
            Return AllMostSimples.First()
        End Function


        Public Overridable Function actionAnActionReturnsObjectWithParametersAnnotatedIdempotent(
                                                                                                 ByVal parm1 As _
                                                                                                    WholeNumber,
                                                                                                 ByVal parm2 As _
                                                                                                    MostSimple) _
            As MostSimple
            NUnit.Framework.Assert.AreEqual(101, parm1.ctypeIntValue)
            NUnit.Framework.Assert.AreEqual(AllMostSimples.First(), parm2)
            Return AllMostSimples.First()
        End Function


#Region "AnActionWithValueParameter"

        Public Overridable Function actionAnActionWithValueParameter(
                                                                     ByVal parm1 As WholeNumber) As MostSimple _
            Implements IWithAction.actionAnActionWithValueParameter
            Return MostSimpleWithIdOf1()
        End Function

#End Region

#Region "AnActionWithValueParameterWithChoices"

        'RP: Can't specify choices for a value param; only a reference param
        Public Overridable Function actionAnActionWithValueParameterWithChoices(ByVal parm3 As WholeNumber) _
            As MostSimple
            Return MostSimpleWithIdOf1()
        End Function

#End Region

#Region "AnActionWithValueParameterWithDefault"

        Public Overridable Function actionAnActionWithValueParameterWithDefault(
                                                                                ByVal parm5 As WholeNumber) _
            As MostSimple Implements IWithAction.actionAnActionWithValueParameterWithDefault
            Return MostSimpleWithIdOf1()
        End Function

        Public Sub aboutActionAnActionWithValueParameterWithDefault(a As ActionAbout, ByVal parm5 As WholeNumber) _
            Implements IWithAction.aboutActionAnActionWithValueParameterWithDefault
            a.setParameter(0, New WholeNumber(Me, 4))
        End Sub

#End Region

#Region "AnActionWithReferenceParameter"

        Public Overridable Function actionAnActionWithReferenceParameter(
                                                                         ByVal parm2 As MostSimple) As MostSimple _
            Implements IWithAction.actionAnActionWithReferenceParameter
            Return AllMostSimples().Where(Function(x) x.Id.ctypeIntValue = parm2.Id.ctypeIntValue).Single()
        End Function

        Public Sub aboutActionAnActionWithReferenceParameter(
                                                             a As ActionAbout, ByVal parm4 As MostSimple) _
            Implements IWithAction.aboutActionAnActionWithReferenceParameter
            a.setParameters({"parm2"})
        End Sub

#End Region

#Region "AnActionWithReferenceParameterWithChoices"

        Public Overridable Function actionAnActionWithReferenceParameterWithChoices(
                                                                                    ByVal parm4 As MostSimple) _
            As MostSimple Implements IWithAction.actionAnActionWithReferenceParameterWithChoices
            Return AllMostSimples().Where(Function(x) x.Id.ctypeIntValue = parm4.Id.ctypeIntValue).Single()
        End Function

        Public Sub aboutActionAnActionWithReferenceParameterWithChoices(
                                                                        a As ActionAbout, ByVal parm4 As MostSimple) _
            Implements IWithAction.aboutActionAnActionWithReferenceParameterWithChoices
            a.setParameter(0, AllMostSimples.ToArray())
            a.setParameters({"parm4"})
        End Sub

#End Region

#Region "AnActionWithReferenceParameterWithDefault"

        Public Overridable Function actionAnActionWithReferenceParameterWithDefault(
                                                                                    ByVal parm6 As MostSimple) _
            As MostSimple Implements IWithAction.actionAnActionWithReferenceParameterWithDefault
            Return AllMostSimples().Where(Function(x) x.Id.ctypeIntValue = parm6.Id.ctypeIntValue).Single()
        End Function

        Public Sub aboutActionAnActionWithReferenceParameterWithDefault(
                                                                        a As ActionAbout, ByVal parm4 As MostSimple) _
            Implements IWithAction.aboutActionAnActionWithReferenceParameterWithDefault
            a.setParameter(0, AllMostSimples().First())
            a.setParameters({"parm6"})
        End Sub

#End Region

#Region "AnActionWithParametersWithChoicesWithDefaults"

        'RP: Cannot do the choices for the scalar property  -  need to alter the tests
        Public Overridable Function actionAnActionWithParametersWithChoicesWithDefaults(ByVal parm1 As WholeNumber,
                                                                                        ByVal parm7 As WholeNumber,
                                                                                        ByVal parm2 As MostSimple,
                                                                                        ByVal parm8 As MostSimple) _
            As MostSimple Implements IWithAction.actionAnActionWithParametersWithChoicesWithDefaults
            Return AllMostSimples().First()
        End Function

        Public Sub aboutActionAnActionWithParametersWithChoicesWithDefaults(a As ActionAbout,
                                                                            ByVal parm1 As WholeNumber,
                                                                            ByVal parm7 As WholeNumber,
                                                                            ByVal parm2 As MostSimple,
                                                                            ByVal parm8 As MostSimple) _
            Implements IWithAction.aboutActionAnActionWithParametersWithChoicesWithDefaults
            a.setParameters({"parm1", "parm7", "parm2", "parm8"})
            ' a.setParameter(1, New WholeNumber(Me, 4))
            a.setParameter(3, AllMostSimples().ToArray())
            a.setParameter(3, AllMostSimples().First())
        End Sub

#End Region

#Region "AnError"

        Public Overridable Function actionAnError() As WholeNumber Implements IWithAction.actionAnError
            Throw New Exception("An error exception")
        End Function


#End Region

#Region "AnErrorQuery"

        'RP: I made this return an arraylist  as nof2 can't return a queryable
        Public Overridable Function actionAnErrorQuery() As ArrayList Implements IWithAction.actionAnErrorQuery
            Throw New Exception("An error exception")
        End Function


#End Region

#Region "AnErrorCollection"

        'RP: I made this return an arraylist  as nof2 can't return a queryable
        Public Overridable Function actionAnErrorCollection() As ArrayList _
            Implements IWithAction.actionAnErrorCollection
            Throw New Exception("An error exception")
        End Function

#End Region
    End Class
End Namespace