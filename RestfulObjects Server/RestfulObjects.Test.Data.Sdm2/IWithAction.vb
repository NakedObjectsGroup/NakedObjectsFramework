Imports sdm.systems.application.control
Imports RestfulObjects.Test.Data

Public Interface IWithAction
    Function actionAnAction() As MostSimple

    Function actionAnActionWithOptionalParm(parm As String) As MostSimple

    Function actionAHiddenAction() As MostSimple
    Sub aboutActionAHiddenAction(a As ActionAbout)

    Function actionADisabledAction() As MostSimple
    Sub aboutActionADisabledAction(a As ActionAbout)

    Function actionADisabledQueryAction() As ArrayList
    Sub aboutActionADisabledQueryAction(a As ActionAbout)

    Function actionADisabledCollectionAction() As ArrayList
    Sub aboutActionADisabledCollectionAction(a As ActionAbout)

    Function actionAnActionReturnsScalar() As WholeNumber
    Sub actionAnActionReturnsVoid()
    Function actionAnActionReturnsCollection() As ArrayList

    Function actionAnActionReturnsCollectionWithScalarParameters(ByVal parm1 As WholeNumber, ByVal parm2 As TextString) _
        As ArrayList

    Function actionAnActionReturnsCollectionWithParameters(ByVal parm1 As WholeNumber, ByVal parm2 As MostSimple) _
        As ArrayList

    Function actionAnActionReturnsQuery() As ArrayList

    Function actionAnActionReturnsQueryWithScalarParameters(ByVal parm1 As WholeNumber, ByVal parm2 As TextString) _
        As ArrayList

    Function actionAnActionReturnsQueryWithParameters(ByVal parm1 As WholeNumber, ByVal parm2 As MostSimple) _
        As ArrayList

    Function actionAnActionReturnsScalarWithParameters(ByVal parm1 As WholeNumber, ByVal parm2 As MostSimple) _
        As WholeNumber

    Sub actionAnActionReturnsVoidWithParameters(ByVal parm1 As WholeNumber, ByVal parm2 As MostSimple)

    Function actionAnActionReturnsObjectWithParameters(ByVal parm1 As WholeNumber, ByVal parm2 As MostSimple) _
        As MostSimple

    Function actionAnActionWithValueParameter(ByVal parm1 As WholeNumber) As MostSimple
    Function actionAnActionWithValueParameterWithDefault(ByVal parm5 As WholeNumber) As MostSimple
    Sub aboutActionAnActionWithValueParameterWithDefault(a As ActionAbout, ByVal parm5 As WholeNumber)

    Function actionAnActionWithReferenceParameter(ByVal parm2 As MostSimple) As MostSimple
    Sub aboutActionAnActionWithReferenceParameter(a As ActionAbout, ByVal parm4 As MostSimple)

    Function actionAnActionWithReferenceParameterWithChoices(ByVal parm4 As MostSimple) As MostSimple
    Sub aboutActionAnActionWithReferenceParameterWithChoices(a As ActionAbout, ByVal parm4 As MostSimple)

    Function actionAnActionWithReferenceParameterWithDefault(ByVal parm6 As MostSimple) As MostSimple
    Sub aboutActionAnActionWithReferenceParameterWithDefault(a As ActionAbout, ByVal parm4 As MostSimple)

    Function actionAnActionWithParametersWithChoicesWithDefaults(ByVal parm1 As WholeNumber,
                                                                 ByVal parm7 As WholeNumber,
                                                                 ByVal parm2 As MostSimple,
                                                                 ByVal parm8 As MostSimple) As MostSimple

    Sub aboutActionAnActionWithParametersWithChoicesWithDefaults(a As ActionAbout,
                                                                 ByVal parm1 As WholeNumber,
                                                                 ByVal parm7 As WholeNumber,
                                                                 ByVal parm2 As MostSimple,
                                                                 ByVal parm8 As MostSimple)

    Function actionAnError() As WholeNumber
    Function actionAnErrorQuery() As ArrayList
    Function actionAnErrorCollection() As ArrayList

    Sub aboutActionAnActionReturnsCollectionWithScalarParameters(a As ActionAbout, ByVal parm1 As WholeNumber,
                                                                 ByVal parm2 As TextString)
End Interface
