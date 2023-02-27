using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestAction : ITestAction {
    private readonly ActionMember action;

    public TestAction(ActionMember action, AcceptanceTestCase acceptanceTestCase) {
        AcceptanceTestCase = acceptanceTestCase;
        this.action = action;
    }

    internal AcceptanceTestCase AcceptanceTestCase { get; }

    public string Name => action.GetExtensions().Extensions()[ExtensionsApi.ExtensionKeys.friendlyName].ToString();
    public string SubMenu => action.GetExtensions().Extensions().TryGetValue(ExtensionsApi.ExtensionKeys.x_ro_nof_menuPath, out var path) ? path?.ToString() ?? "" : "";

    public string LastMessage { get; private set; }

    public ITestParameter[] Parameters => action.GetParameters(AcceptanceTestCase.TestInvokeOptions()).Result.Parameters().Select(x => new TestParameter(x.Value, AcceptanceTestCase)).Cast<ITestParameter>().ToArray();

    private static bool Match(ITestParameter parameter, Type t) => parameter.Type == t;

    public bool MatchParameters(Type[] typestoMatch) {
        var actualParameters = Parameters;
        if (actualParameters.Count() == typestoMatch.Length) {
            var zip = actualParameters.Zip(typestoMatch);

            return zip.All(x => Match(x.First, x.Second));
        }

        return false;
    }

    public ITestObject InvokeReturnObject(params object[] parameters) => (ITestObject)DoInvoke(ParsedParameters(parameters));

    public ITestCollection InvokeReturnCollection(params object[] parameters) => (ITestCollection)DoInvoke(ParsedParameters(parameters));

    public void Invoke(params object[] parameters)
    {
        DoInvoke(ParsedParameters(parameters));
    }

    public ITestCollection InvokeReturnPagedCollection(int page, params object[] parameters) => (ITestCollection)DoInvoke(page, ParsedParameters(parameters));

    public ITestAction AssertIsDisabled() {
        ResetLastMessage();
        if (action is not null) {
            var canUse = action.GetDisabledReason();
            LastMessage = canUse;
            Assert.IsFalse(string.IsNullOrEmpty(canUse), $"Action '{Name}' is usable: {canUse}");
        }

        return this;
    }

    public ITestAction AssertIsEnabled() {
        ResetLastMessage();
        AssertIsVisible();
        var canUse = action.GetDisabledReason();
        LastMessage = canUse;
        Assert.IsTrue(string.IsNullOrEmpty(canUse), $"Action '{Name}' is disabled: {canUse}");
        return this;
    }

    public ITestAction AssertIsInvalidWithParms(params object[] parameters)
    {
        ResetLastMessage();

        var parsedParameters = ParsedParameters(parameters);

        if (action is not null)
        {
            var canUse = action.GetDisabledReason();
            LastMessage = canUse;
            if (string.IsNullOrEmpty(canUse))
            {
                //INakedObject[] parameterObjects = parsedParameters.AsTestNakedArray().Select(x => x == null ? null : x.NakedObject).ToArray();
                var result = action.Invoke(AcceptanceTestCase.TestInvokeOptions(), parameters).Result;
                //LastMessage = canExecute.Reason;
                //Assert.IsFalse(canExecute.IsAllowed, $"Action '{Name}' is usable and executable");
            }
        }

        return this;
    }

    public ITestAction AssertIsValidWithParms(params object[] parameters) {
        ResetLastMessage();
        AssertIsVisible();
        AssertIsEnabled();
        var parsedParameters = ParsedParameters(parameters);

        var canExecute = true;
        var message = "";

        try {
            action.Validate(AcceptanceTestCase.TestInvokeOptions(), parsedParameters).Wait();
        }
        catch (AggregateException ae) {
            canExecute = false;
            message = ae.InnerException?.Message;
        }

        Assert.IsTrue(canExecute, $@"Action '{Name}' is unusable: {message}");
        return this;
    }

    public ITestAction AssertIsVisible()
    {
        ResetLastMessage();
        Assert.IsTrue(action is not null, $"Action '{Name}' is hidden");
        return this;
    }

    public ITestAction AssertIsInvisible()
    {
        ResetLastMessage();
        Assert.IsFalse(action is null, $"Action '{Name}' is visible");
        return this;
    }

    public ITestAction AssertIsDescribedAs(string expected) {
        var description = action.GetExtensions().Extensions()[ExtensionsApi.ExtensionKeys.description].ToString();
        Assert.IsTrue(expected.Equals(description), $"Description expected: '{expected}' actual: '{description}'");
        return this;
    }

    public ITestAction AssertLastMessageIs(string message)
    {
        Assert.IsTrue(message.Equals(LastMessage), $"Last message expected: '{message}' actual: '{LastMessage}'");
        return this;
    }

    public ITestAction AssertLastMessageContains(string message)
    {
        Assert.IsTrue(LastMessage.Contains(message), $"Last message expected to contain: '{message}' actual: '{LastMessage}'");
        return this;
    }

    //private ITestNaked DoInvoke(int page, params object[] parameters) {
    //    ResetLastMessage();
    //    AssertIsValidWithParms(parameters);
    //    INakedObject[] parameterObjects = parameters.AsTestNakedArray().Select(x => x.NakedObject).ToArray();

    //    INakedObject[] parms = action.RealParameters(owningObject.NakedObject, parameterObjects);
    //    INakedObject target = action.RealTarget(owningObject.NakedObject);
    //    INakedObject result = action.GetFacet<IActionInvocationFacet>().Invoke(target, parms, page);

    //    if (result == null) {
    //        return null;
    //    }

    //    if (result.Specification.IsCollection) {
    //        return factory.CreateTestCollection(result);
    //    }

    //    return factory.CreateTestObject(result);
    //}

    private ITestNaked DoInvoke(params object[] parameters)
    {
        ResetLastMessage();
        AssertIsValidWithParms(parameters);
        ActionResult result = null;
        try
        {
            result = action.Invoke(AcceptanceTestCase.TestInvokeOptions(), parameters).Result;
        }
        catch (ArgumentException e)
        {
            Assert.IsInstanceOfType(e, typeof(ArgumentException));
            Assert.Fail("Invalid Argument(s)");
        }

        if (result.GetResultType() == ActionResultApi.ResultType.Void)
        {
            return null;
        }

        if (result.GetResultType() == ActionResultApi.ResultType.List)
        {
            //return factory.CreateTestCollection(result);
        }

        return new TestObject(result.GetObject(), AcceptanceTestCase);
    }

    private void ResetLastMessage()
    {
        LastMessage = string.Empty;
    }

    private object[] ParsedParameters(params object[] parameters) {
        var parsedParameters = new List<object>();

        var actualParameters = Parameters;
        Assert.IsTrue(parameters.Length == actualParameters.Length, $"Action '{Name}' is unusable: wrong number of parameters, got {parameters.Length}, expect {actualParameters.Length}");

        var zip = actualParameters.Zip(parameters);

        foreach (var (p, v) in zip) {
            if (p.Type != v.GetType()) {
                Assert.Fail("Invalid Argument(s)");
            }
        }


        //var i = 0;
        //foreach (var parm in actualParameters) {
        //    var value = parameters[i++];

        //    if (value is string && parm.Specification.IsParseable) {
        //        parsedParameters.Add(parm.Specification.GetFacet<IParseableFacet>().ParseTextEntry((string)value).Object);
        //    }
        //    else {
        //        parsedParameters.Add(value);
        //    }
        //}

        //return parsedParameters.ToArray();
        return parameters;
    }
}