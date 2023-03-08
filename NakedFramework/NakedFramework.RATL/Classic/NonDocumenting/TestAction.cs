using NakedFramework.RATL.Classic.Interface;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestAction : ITestAction {
    private readonly ActionMember action;

    public TestAction(ActionMember action) => this.action = action;

    public string? Name => action.GetExtensions().Extensions()[ExtensionsApi.ExtensionKeys.friendlyName]?.ToString();
    public string SubMenu => action.GetExtensions().Extensions().TryGetValue(ExtensionsApi.ExtensionKeys.x_ro_nof_menuPath, out var path) ? path?.ToString() ?? "" : "";

    public string? LastMessage { get; private set; }

    public ITestParameter[] Parameters => action.GetParameters().Result.Parameters().Select(x => new TestParameter(x.Value)).Cast<ITestParameter>().ToArray();

    public bool MatchParameters(Type[] typesToMatch) {
        var actualParameters = Parameters;
        if (actualParameters.Count() == typesToMatch.Length) {
            var zip = actualParameters.Zip(typesToMatch);

            return zip.All(x => x.First.Match(x.Second));
        }

        return false;
    }

    public ITestObject InvokeReturnObject(params object[] parameters) => DoInvoke<ITestObject>(null, ParsedParameters(parameters));

    public ITestCollection InvokeReturnCollection(params object[] parameters) => DoInvoke<ITestCollection>(null, ParsedParameters(parameters));

    public void Invoke(params object[] parameters) => DoInvoke<ITestObject>(null, ParsedParameters(parameters));

    public ITestCollection InvokeReturnPagedCollection(int page, params object[] parameters) => DoInvoke<ITestCollection>(page, ParsedParameters(parameters));

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

    public ITestAction AssertIsInvalidWithParms(params object[] parameters) {
        try {
            AssertIsValidWithParms(parameters);
        }
        catch (AssertFailedException) {
            return this;
        }

        Assert.IsFalse(true, $"Action '{Name}' is usable and executable");

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
            action.Validate(parsedParameters).Wait();
        }
        catch (AggregateException ae) {
            canExecute = false;
            message = ae.InnerException?.Message;
        }

        Assert.IsTrue(canExecute, $@"Action '{Name}' is unusable: {message}");
        return this;
    }

    public ITestAction AssertIsVisible() {
        ResetLastMessage();
        Assert.IsTrue(action is not null, $"Action '{Name}' is hidden");
        return this;
    }

    public ITestAction AssertIsInvisible() {
        ResetLastMessage();
        Assert.IsTrue(action is null, $"Action '{Name}' is visible");
        return this;
    }

    public ITestAction AssertIsDescribedAs(string expected) {
        var description = action.GetExtensions().Extensions()[ExtensionsApi.ExtensionKeys.description].ToString();
        Assert.IsTrue(expected.Equals(description), $"Description expected: '{expected}' actual: '{description}'");
        return this;
    }

    public ITestAction AssertLastMessageIs(string message) {
        Assert.IsTrue(message.Equals(LastMessage), $"Last message expected: '{message}' actual: '{LastMessage}'");
        return this;
    }

    public ITestAction AssertLastMessageContains(string message) {
        Assert.IsTrue(LastMessage.Contains(message), $"Last message expected to contain: '{message}' actual: '{LastMessage}'");
        return this;
    }

    private T? DoInvoke<T>(int? page, params object[] parameters) where T : class, ITestNaked {
        ResetLastMessage();
        AssertIsValidWithParms(parameters);
        ActionResult result = null;
        try {
            var options = action.Options;
            if (page is not null) {
                options = options with { ReservedArguments = options.ReservedArguments.Add("x-ro-page", page) };
            }

            result = action.Invoke(options, parameters).Result;
        }
        catch (ArgumentException e) {
            Assert.IsInstanceOfType(e, typeof(ArgumentException));
            Assert.Fail("Invalid Argument(s)");
        }

        if (result.GetResultType() == ActionResultApi.ResultType.Void) {
            return null;
        }

        if (result.GetResultType() == ActionResultApi.ResultType.List) {
            return result.GetList() is { } list ? new TestCollection(list) as T : null;
        }

        return result.GetObject() is { } domainObject ? new TestObject(domainObject) as T : null;
    }

    private void ResetLastMessage() {
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

        return parameters;
    }
}